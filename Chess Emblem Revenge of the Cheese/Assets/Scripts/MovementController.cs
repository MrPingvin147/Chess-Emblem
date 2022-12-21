using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
public class MovementController : MonoBehaviour
{
    public int gridXPosition { get; private set; } = 14;
    public int gridYPosition { get; private set; } = 14;
    public float yOffset = 0.5f;

    public Vector2 startGridPosition;

    public int targetLocationX = 5;
    public int targetLocationY = 5;

    public float speed = 0.5f;

    public bool move = false;

    private List<GameObject> path;

    GridBehaviour gridBehaviour;

    public Material selectedMaterial;
    public Material deSelectedMaterial;
    MeshRenderer meshRenderer;

    public Material arrowMat;
    LineRenderer lineRenderer;

    public UnitStats unitStats;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.startWidth = 0.3f;
        lineRenderer.positionCount = 0;
        lineRenderer.numCornerVertices = 90;
        lineRenderer.numCapVertices = 1;
        lineRenderer.material = arrowMat;
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        if (GetComponent<MeshRenderer>() == null)
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();
        }
        else
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
        

        gridBehaviour = GridBehaviour.instance.GetComponent<GridBehaviour>();

        gridXPosition = (int)startGridPosition.x;
        gridYPosition = (int)startGridPosition.y;

        Transform startTransform = gridBehaviour.gridArray[gridXPosition, gridYPosition].transform;
        gridBehaviour.gridArray[gridXPosition, gridYPosition].GetComponent<GridStat>().objektOnTile = gameObject;
        gameObject.transform.position = new Vector3(startTransform.position.x, startTransform.position.y + yOffset, startTransform.position.z);

        meshRenderer.material = deSelectedMaterial;
    }

    private void Update()
    {
        if (move)
        {
            MoveToLocation(targetLocationX, targetLocationY);
            move = false;
        }
    }

    public void SelectUnit()
    {
        meshRenderer.material = selectedMaterial;
    }

    public void DeselectUnit()
    {
        meshRenderer.material = deSelectedMaterial;
        RemoveArrowPath();
    }

    public void MoveToLocation(int x, int y)
    {
        path = gridBehaviour.GetPath(this, x, y);

        if (path.Count > unitStats.spd)
        {
            path.RemoveRange(0, path.Count - unitStats.spd - 1);
        }
        x = path[0].GetComponent<GridStat>().x;
        y = path[0].GetComponent<GridStat>().y;

        ShowPathToMouse(x, y);

        StartCoroutine(LerpPosition(x,y,speed));
        
    }

    IEnumerator LerpPosition(int x, int y, float duration)
    {
        gridBehaviour.gridArray[gridXPosition, gridYPosition].GetComponent<GridStat>().objektOnTile = null;
        gridBehaviour.gridArray[x, y].GetComponent<GridStat>().objektOnTile = gameObject;
        for (int i = path.Count - 1; i > -1; i--)
        {
            float time = 0;
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = path[i].transform.position;
            targetPosition.y += yOffset;
            while (time < duration)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPosition;
            gridXPosition = path[0].GetComponent<GridStat>().x;
            gridYPosition = path[0].GetComponent<GridStat>().y;
        }
        RemoveArrowPath();
    }

    public void ShowPathToMouse(int mouseX, int mouseY)
    {
        List<GameObject> tmpPath = gridBehaviour.GetPath(this, mouseX, mouseY);

        if (tmpPath.Count > unitStats.spd)
        {
            tmpPath.RemoveRange(0, tmpPath.Count - unitStats.spd - 1);
        }

        lineRenderer.positionCount = tmpPath.Count;

        Vector3[] positions = new Vector3[tmpPath.Count];

        for (int i = tmpPath.Count - 1; i > -1; i--)
        {
            Vector3 tmpPos = tmpPath[i].GetComponent<GridStat>().transform.position;
            tmpPos.y += 0.2f;

            positions[i] = tmpPos;
        }
        lineRenderer.SetPositions(positions);
    }

    public void RemoveArrowPath()
    {
        lineRenderer.positionCount = 0;
    }
}
