using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridBehaviour : MonoBehaviour
{
    public int rows = 15;
    public int columns = 15;
    public float scale { private set; get; } = 1f;
    public GameObject gridPrefab;
    public Vector3 leftBottomLocation = Vector3.zero;
    public GameObject gridSqaure;
    public GameObject[,] gridArray;
    public int startX = 0;
    public int startY = 0;
    [HideInInspector]
    public int endX = 2;
    [HideInInspector]
    public int endY = 2;
    [HideInInspector]
    public List<GameObject> path = new List<GameObject>();

    private GameObject unitsParent;
    public GameObject[] units;
    public Material[] unitMaterials;

    public GameObject[,] positionMatrix;

    public static GameObject instance;


    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = gameObject;
        }
        else
        {
            return;
        }

        gridArray = new GameObject[columns, rows];
        positionMatrix = new GameObject[columns, rows];

        if (gridPrefab)
        {
            GenerateGrid();
        }
        else
        {
            Debug.LogWarning("Prefab er tom, vær sød at fyld den");
        }

        unitsParent = new GameObject("UnitParent");
        ArrangeUnits();
        SpawnUnits();
    }

    void ArrangeUnits()
    {
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                //Pawns
                if (i == 0 || i == 2 || i == 4 || i == 6 || i == 7 || i == 10 || i == 12 || i == 14)
                {
                    if (j == 1 || j == 13)
                    {
                        positionMatrix[i, j] = units[0];
                    }
                }

                //Knights
                if (i == 0 || i == 14)
                {
                    if (j == 0 || j == 14)
                    {
                        positionMatrix[i, j] = units[1];
                    }
                }

                //Archer
                if (i == 2 || i == 12)
                {
                    if (j == 0 || j == 14)
                    {
                        positionMatrix[i, j] = units[2];
                    }
                }

                //Mage
                if (i == 4 || i == 10)
                {
                    if (j == 0 || j == 14)
                    {
                        positionMatrix[i, j] = units[3];
                    }
                }

                //Konge
                if (i == 6)
                {
                    if (j == 0 || j == 14)
                    {
                        positionMatrix[i, j] = units[4];
                    }
                }

                //Commander
                if (i == 7)
                {
                    if (j == 0 || j == 14)
                    {
                        positionMatrix[i, j] = units[5];
                    }
                }

            }
        }
    }

    void SpawnUnits()
    {
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                if (positionMatrix[i, j] != null)
                {
                    Vector3 tmpPos = gridArray[i, j].transform.position;
                    tmpPos.y += positionMatrix[i, j].GetComponent<MovementController>().yOffset;

                    GameObject obj = Instantiate(positionMatrix[i, j], tmpPos, Quaternion.identity, unitsParent.transform);

                    MovementController movementController = obj.GetComponent<MovementController>();

                    movementController.startGridPosition.x = i;
                    movementController.startGridPosition.y = j;

                    if (j > rows / 2)
                    {
                        movementController.selectedMaterial = unitMaterials[0];
                        movementController.deSelectedMaterial = unitMaterials[1];
                        movementController.team = "white";
                    }
                    if (j < rows / 2)
                    {
                        obj.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                        movementController.selectedMaterial = unitMaterials[2];
                        movementController.deSelectedMaterial = unitMaterials[3];
                        movementController.team = "black";
                    }
                }
            }
        }
    }

    public List<GameObject> GetPath(MovementController unit, int desiredX, int desiredY)
    {
        startX = unit.gridXPosition;
        startY = unit.gridYPosition;
        endX = desiredX;
        endY = desiredY;
        int sum = 0;

        sum = Mathf.Abs(startX - endX) + Mathf.Abs(startY - endY);
        //if X and Y is on an enemy then find the closest sqaure around the unit
        if (gridArray[endX, endY].GetComponent<GridStat>().objektOnTile && sum > 1)
        {
            float[] tmpArray = new float[4];
            try { tmpArray[0] = Vector3.Distance(gridArray[startX, startY].transform.position, gridArray[endX + 1, endY].transform.position); }
            catch { tmpArray[0] = 1000000; }
            try { tmpArray[1] = Vector3.Distance(gridArray[startX, startY].transform.position, gridArray[endX - 1, endY].transform.position); }
            catch { tmpArray[1] = 1000000; }
            try { tmpArray[2] = Vector3.Distance(gridArray[startX, startY].transform.position, gridArray[endX, endY + 1].transform.position); }
            catch { tmpArray[2] = 1000000; }
            try { tmpArray[3] = Vector3.Distance(gridArray[startX, startY].transform.position, gridArray[endX, endY - 1].transform.position); }
            catch { tmpArray[3] = 1000000; }

            float minValue = tmpArray.Min<float>();

            for (int i = 0; i < 4; i++)
            {
                if (minValue == tmpArray[i]) { minValue = i; }
            }

            if (minValue == 0) { endX = endX + 1; }
            if (minValue == 1) { endX = endX - 1; }
            if (minValue == 2) { endY = endY + 1; }
            if (minValue == 3) { endY = endY - 1; }

            sum = Mathf.Abs(startX - endX) + Mathf.Abs(startY - endY);

            if (sum <= 0.9f) { return new List<GameObject>(); }
        }

        SetDistance();
        SetPath();

        return path;
    }

    void GenerateGrid()
    {
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                GameObject obj = Instantiate(gridPrefab, new Vector3(leftBottomLocation.x + scale * i, leftBottomLocation.y, leftBottomLocation.z + scale * j), Quaternion.identity);
                obj.transform.SetParent(gameObject.transform);
                obj.GetComponent<GridStat>().x = i;
                obj.GetComponent<GridStat>().y = j;

                gridArray[i, j] = obj;
            }
        }

        float distance = (gridArray[1, 0].transform.position.x - gridArray[0, 0].transform.position.x) / 2;
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                Vector3 tmpPos = new Vector3(leftBottomLocation.x + scale * i, leftBottomLocation.y, leftBottomLocation.z + scale * j);
                tmpPos.x -= distance;
                tmpPos.y = 0.05f;

                Instantiate(gridSqaure, tmpPos, Quaternion.Euler(new Vector3(0, 0, 0)), gameObject.transform);

                if (i == 14)
                {
                    tmpPos.x += distance * 2;
                    Instantiate(gridSqaure, tmpPos, Quaternion.Euler(new Vector3(0, 0, 0)), gameObject.transform);
                    tmpPos.x -= distance * 2;
                }

                tmpPos.x += distance;
                tmpPos.z += distance;
                Instantiate(gridSqaure, tmpPos, Quaternion.Euler(new Vector3(0, 90, 0)), gameObject.transform);

                if (j == 0)
                {
                    tmpPos.z -= distance * 2;
                    Instantiate(gridSqaure, tmpPos, Quaternion.Euler(new Vector3(0, 90, 0)), gameObject.transform);
                }
            }
        }
    }

    void SetDistance()
    {
        InitialSetUp();
        int x = startX;
        int y = startY;
        for (int step = 1; step < rows * columns; step++)
        {
            foreach(GameObject obj in gridArray)
            {
                if (obj && obj.GetComponent<GridStat>().visited == step - 1)
                {
                    TestFourDirection(obj.GetComponent<GridStat>().x, obj.GetComponent<GridStat>().y, step);
                }
            }
        }
    }

    void SetPath()
    {
        int step;
        int x = endX;
        int y = endY;

        List<GameObject> tempList = new List<GameObject>();
        path.Clear();
        if (gridArray[x, y] && gridArray[x,y].GetComponent<GridStat>().visited > 0)
        {
            path.Add(gridArray[x, y]);
            step = gridArray[x, y].GetComponent<GridStat>().visited - 1;
        }
        else
        {
            return;
        }

        for (int i = step; step > -1; step--)
        {
            if (TestDirection(x, y, step, 1))
            {
                tempList.Add(gridArray[x, y + 1]);
            }
            if (TestDirection(x, y, step, 2))
            {
                tempList.Add(gridArray[x + 1, y]);
            }
            if (TestDirection(x, y, step, 3))
            {
                tempList.Add(gridArray[x, y - 1]);
            }
            if (TestDirection(x, y, step, 4))
            {
                tempList.Add(gridArray[x - 1, y]);
            }
            GameObject tempObj = FindClosest(gridArray[x, y].transform, tempList);
            path.Add(tempObj);
            x = tempObj.GetComponent<GridStat>().x;
            y = tempObj.GetComponent<GridStat>().y;
            tempList.Clear();
        }
    }

    void InitialSetUp()
    {
        foreach(GameObject obj in gridArray)
        {
            obj.GetComponent<GridStat>().visited = -1;
        }
        gridArray[startX, startY].GetComponent<GridStat>().visited = 0;
    }

    bool TestDirection(int x, int y, int step, int direction)
    {
        switch (direction)
        {
            case 1:
                if (y + 1 < rows && gridArray[x, y] && gridArray[x, y + 1].GetComponent<GridStat>().visited == step)
                {
                    return true;
                }
                else { return false; }
            case 2:
                if (x + 1 < columns && gridArray[x, y] && gridArray[x + 1, y].GetComponent<GridStat>().visited == step)
                {
                    return true;
                }
                else { return false; }
            case 3:
                if (y - 1 > -1 && gridArray[x, y] && gridArray[x, y - 1].GetComponent<GridStat>().visited == step)
                {
                    return true;
                }
                else { return false; }
            case 4:
                if (x - 1 > -1 && gridArray[x, y] && gridArray[x - 1, y].GetComponent<GridStat>().visited == step)
                {
                    return true;
                }
                else { return false; }
        }
        return false;
    }

    void TestFourDirection(int x, int y, int step)
    {
        //Tjekker opad
        if (TestDirection(x,y,-1, 1))
        {
            SetVisited(x, y + 1, step);
        }
        //Tjekker mod højre
        if (TestDirection(x, y, -1, 2))
        {
            SetVisited(x + 1, y, step);
        }
        //Tjekker ned af
        if (TestDirection(x, y, -1, 3))
        {
            SetVisited(x, y - 1, step);
        }
        //Tjekker mod venstre
        if (TestDirection(x, y, -1, 4))
        {
            SetVisited(x - 1, y, step);
        }
    }

    void SetVisited(int x, int y, int step)
    {
        if (gridArray[x, y] && gridArray[x, y].GetComponent<GridStat>().objektOnTile == null)
        {
            gridArray[x, y].GetComponent<GridStat>().visited = step;
        }
    }

    GameObject FindClosest(Transform targetLocation, List<GameObject> list)
    {
        float currentDistance = scale * rows * columns;
        int indexNumber = 0;

        for (int i = 0; i < list.Count; i++)
        {
            if (Vector3.Distance(targetLocation.position, list[i].transform.position) < currentDistance)
            {
                currentDistance = Vector3.Distance(targetLocation.position, list[i].transform.position);
                indexNumber = i;
            }
        }
        return list[indexNumber];
    }
}
