using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using UnityEngine;

public class GridBehaviour : MonoBehaviour
{
    public int rows = 15;
    public int columns = 15;
    public float scale = 1f;
    private Vector3 leftBottomLocation = Vector3.zero;
    public GameObject gridPrefab;
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

    public GameObject[] obstacles;

    public GameObject[] decorations;
    GameObject[,][] decorationMatrix;
    GameObject decorationsParent;

    GameObject[,][] gridSqaureMatrix;

    public GameObject[,] positionMatrix;

    public Transform CameraRotationPoint;

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

        GenerateLevel();
    }

    public void GenerateLevel()
    {
        gridArray = new GameObject[columns, rows];
        positionMatrix = new GameObject[columns, rows];
        decorationMatrix = new GameObject[columns, rows][];
        gridSqaureMatrix = new GameObject[columns, rows][];

        if (gridPrefab)
        {
            GenerateGrid();
        }
        else
        {
            Debug.LogWarning("Prefab er tom, vær sød at fyld den");
        }

        unitsParent = new GameObject("UnitParent");
        decorationsParent = new GameObject("DecorationsParent");
        ArrangeUnits();
        SpawnUnits();
        SpawnDecorations();
    }

    public void DeleteLevel()
    {
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                Destroy(gridArray[i, j]);
                Destroy(positionMatrix[i, j]);

                for (int k = 0; k < decorationMatrix[i,j].Length; k++)
                {
                    Destroy(decorationMatrix[i, j][k]);
                }

                for (int k = 0; k < gridSqaureMatrix[i,j].Length; k++)
                {
                    Destroy(gridSqaureMatrix[i, j][k]);
                }
            }
        }

        Destroy(unitsParent);
        Destroy(decorationsParent);
    }

    void ArrangeUnits()
    {
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                //Pawns
                if (i == 0 || i == 2 || i == 4 || i == 6 || i == 7 || i == 9 || i == 11 || i == columns - 1)
                {
                    if (j == 1 || j == columns - 2)
                    {
                        positionMatrix[i, j] = units[0];
                    }
                }

                //Knights
                if (i == 0 || i == columns - 1)
                {
                    if (j == 0 || j == columns - 1)
                    {
                        positionMatrix[i, j] = units[1];
                    }
                }

                //Archer
                if (i == 2 || i == 11)
                {
                    if (j == 0 || j == columns - 1)
                    {
                        positionMatrix[i, j] = units[2];
                    }
                }

                //Mage
                if (i == 4 || i == 9)
                {
                    if (j == 0 || j == columns - 1)
                    {
                        positionMatrix[i, j] = units[3];
                    }
                }

                //Konge
                if (i == 6)
                {
                    if (j == 0 || j == columns - 1)
                    {
                        positionMatrix[i, j] = units[4];
                    }
                }

                //Commander
                if (i == 7)
                {
                    if (j == 0 || j == columns - 1)
                    {
                        positionMatrix[i, j] = units[5];
                    }
                }

                //Obstacles
                if (j == 5 || j == 6 || j == 7 || j == 8)
                {
                    if (Random.Range(0, 100) > 80 && !positionMatrix[i, j])
                    {
                        positionMatrix[i, j] = obstacles[0];
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

                    GameObject obj = Instantiate(positionMatrix[i, j], tmpPos, Quaternion.identity, unitsParent.transform);

                    positionMatrix[i, j] = obj;

                    gridArray[i, j].GetComponent<GridStat>().objektOnTile = obj;

                    if (obj.GetComponent<MovementController>())
                    {
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
    }

    void SpawnDecorations()
    {
        Texture2D texture = PerlinNoise.GenerateTexture(30, 14, 14);

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                float value = texture.GetPixel(i, j).g;

                decorationMatrix[i, j] = new GameObject[1];

                if (value > 0.6f)
                {
                    decorationMatrix[i, j][0] = decorations[Random.Range(4, 6)];
                }

                if (value > 0.7f)
                {
                    decorationMatrix[i, j][0] = decorations[Random.Range(6, 8)];
                }

                if (value > 0.75f)
                {
                    decorationMatrix[i, j][0] = decorations[Random.Range(1, 3)];
                }

                if (value > 0.85f)
                {
                    if (Random.Range(1,3) == 1)
                    {
                        decorationMatrix[i, j][0] = decorations[0];
                    }
                    else
                    {
                        decorationMatrix[i, j][0] = decorations[decorations.Length - 1];
                    }
                }
            }
        }

        /*for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                int randomValue = Random.Range(0, 100);
                int amountOfDecorations = 0;

                if (randomValue <= 80)
                {
                    amountOfDecorations = 0;
                }
                else if (randomValue > 80 && randomValue <= 88)
                {
                    amountOfDecorations = 1;
                }
                else if (randomValue > 88 && randomValue <= 95)
                {
                    amountOfDecorations = 2;
                }
                else if (randomValue > 95 && randomValue <= 97)
                {
                    amountOfDecorations = 3;
                }
                else if (randomValue > 97 && randomValue <= 100)
                {
                    amountOfDecorations = 4;
                }
                decorationMatrix[i, j] = new GameObject[amountOfDecorations];
                for (int k = 0; k < amountOfDecorations; k++)
                {
                    decorationMatrix[i, j][k] = decorations[Random.Range(0, decorations.Length)];
                }
            }
        }*/

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                for (int k = 0; k < decorationMatrix[i,j].Length; k++)
                {
                    if (decorationMatrix[i, j][k])
                    {
                        GameObject obj = Instantiate(decorationMatrix[i, j][k], gridArray[i, j].transform.position, Quaternion.identity, decorationsParent.transform);

                        decorationMatrix[i, j][k] = obj;

                        Vector3 decorationPosition = new Vector3(Random.Range(scale * -0.4f, scale * 0.4f), obj.transform.position.y + 0.05f, Random.Range(scale * -0.4f, scale * 0.4f));
                        Quaternion decorationRotation = Quaternion.Euler(0, Random.Range(0, 359), 0);
                        float currentScale = decorationMatrix[i, j][k].transform.localScale.x;
                        float newScale = currentScale - Random.Range(-0.2f, 0.2f);
                        Vector3 decorationScale = new Vector3(newScale, newScale, newScale);

                        obj.transform.localScale = decorationScale;
                        obj.transform.position += decorationPosition;
                        obj.transform.rotation = decorationRotation;
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

            try { if (gridArray[endX + 1, endY].GetComponent<GridStat>().objektOnTile == null) { tmpArray[0] = Mathf.Abs(startX - endX - 1) + Mathf.Abs(startY - endY); }
            else { tmpArray[0] = 50; } }
            catch { tmpArray[0] = 100; }

            try { if (gridArray[endX - 1, endY].GetComponent<GridStat>().objektOnTile == null) { tmpArray[1] = Mathf.Abs(startX - endX + 1) + Mathf.Abs(startY - endY); } 
            else { tmpArray[1] = 51; } }
            catch { tmpArray[1] = 101; }

            try { if (gridArray[endX, endY + 1].GetComponent<GridStat>().objektOnTile == null) { tmpArray[2] = Mathf.Abs(startX - endX) + Mathf.Abs(startY - endY - 1); } 
            else { tmpArray[2] = 52; } }
            catch { tmpArray[2] = 102; }

            try { if (gridArray[endX, endY - 1].GetComponent<GridStat>().objektOnTile == null) { tmpArray[3] = Mathf.Abs(startX - endX) + Mathf.Abs(startY - endY + 1); } 
            else { tmpArray[3] = 53; } }
            catch { tmpArray[3] = 103; }

            float minValue = 100000;
            int tmpInt = -10;
            for (int i = 0; i < 4; i++)
            {
                if (tmpArray[i] < minValue) { tmpInt = i; minValue = tmpArray[i]; }
            }

            if (tmpInt == 0) { endX = endX + 1; }
            if (tmpInt == 1) { endX = endX - 1; }
            if (tmpInt == 2) { endY = endY + 1; }
            if (tmpInt == 3) { endY = endY - 1; }


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

        CameraRotationPoint.position = gridArray[rows - 1, columns - 1].transform.position / 2;

        float distance = (gridArray[1, 0].transform.position.x - gridArray[0, 0].transform.position.x) / 2;
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                List<GameObject> tmpList = new List<GameObject>();
                Vector3 tmpPos = new Vector3(leftBottomLocation.x + scale * i, leftBottomLocation.y, leftBottomLocation.z + scale * j);
                tmpPos.x -= distance;
                tmpPos.y = 0.05f;

                tmpList.Add(Instantiate(gridSqaure, tmpPos, Quaternion.Euler(new Vector3(0, 0, 0)), gameObject.transform));

                if (i == columns - 1)
                {
                    tmpPos.x += distance * 2;
                    tmpList.Add(Instantiate(gridSqaure, tmpPos, Quaternion.Euler(new Vector3(0, 0, 0)), gameObject.transform));
                    tmpPos.x -= distance * 2;
                }

                tmpPos.x += distance;
                tmpPos.z += distance;
                tmpList.Add(Instantiate(gridSqaure, tmpPos, Quaternion.Euler(new Vector3(0, 90, 0)), gameObject.transform));

                if (j == 0)
                {
                    tmpPos.z -= distance * 2;
                    tmpList.Add(Instantiate(gridSqaure, tmpPos, Quaternion.Euler(new Vector3(0, 90, 0)), gameObject.transform));
                }

                gridSqaureMatrix[i, j] = tmpList.ToArray();
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
