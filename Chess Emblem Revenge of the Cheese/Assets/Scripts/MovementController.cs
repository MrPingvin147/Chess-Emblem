using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer)), RequireComponent(typeof(CombatController))]
public class MovementController : MonoBehaviour
{
    public int gridXPosition { get; private set; } = 14;
    public int gridYPosition { get; private set; } = 14;
    public float yOffset = 0.5f;

    [HideInInspector]
    public Vector2 startGridPosition;

    [HideInInspector]
    public int targetLocationX = 5;
    [HideInInspector]
    public int targetLocationY = 5;

    public float speed = 0.5f;

    private List<GameObject> path;

    GridBehaviour gridBehaviour;

    public string team;
    [HideInInspector]
    public Material selectedMaterial;
    [HideInInspector]
    public Material deSelectedMaterial;
    MeshRenderer meshRenderer;

    public Material arrowMat;
    LineRenderer lineRenderer;
    CombatController combatController;

    public UnitStats unitStats;

    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        lineRenderer = GetComponent<LineRenderer>();
        combatController = GetComponent<CombatController>();

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

        meshRenderer.material = deSelectedMaterial;
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

    public void MoveToLocation(int x, int y, int movement, GameObject objectOnTile = null)
    {
        int movementLeft = movement;
        int sum = 0;
        path = gridBehaviour.GetPath(this, x, y);

        if (objectOnTile)
        {
            sum = Mathf.Abs(gridXPosition - objectOnTile.GetComponent<MovementController>().gridXPosition) + Mathf.Abs(gridYPosition - objectOnTile.GetComponent<MovementController>().gridYPosition);
            
            if (sum <= unitStats.atkRange || unitStats.atkRange == 0 && sum <= 1)
            {
                print("Attacker: " + gameObject.name + " Attacked: " + objectOnTile.name);

                CombatController cmController = objectOnTile.GetComponent<CombatController>();

                cmController.TakeDamage(combatController.GetDamageValue());

                if (unitStats.splashDamage)
                {
                    if (gridBehaviour.gridArray[x + 1, y].GetComponent<GridStat>().objektOnTile)
                    {
                        GameObject tmpObject = gridBehaviour.gridArray[x + 1, y].GetComponent<GridStat>().objektOnTile;
                        if (tmpObject.GetComponent<CombatController>() && tmpObject.GetComponent<MovementController>().team != team)
                        {
                            tmpObject.GetComponent<CombatController>().TakeDamage(combatController.GetDamageValue());
                        }
                    }
                    if (gridBehaviour.gridArray[x - 1, y].GetComponent<GridStat>().objektOnTile)
                    {
                        GameObject tmpObject = gridBehaviour.gridArray[x - 1, y].GetComponent<GridStat>().objektOnTile;
                        if (tmpObject.GetComponent<CombatController>() && tmpObject.GetComponent<MovementController>().team != team)
                        {
                            tmpObject.GetComponent<CombatController>().TakeDamage(combatController.GetDamageValue());
                        }
                    }
                    if (gridBehaviour.gridArray[x, y + 1].GetComponent<GridStat>().objektOnTile)
                    {
                        GameObject tmpObject = gridBehaviour.gridArray[x, y + 1].GetComponent<GridStat>().objektOnTile;
                        if (tmpObject.GetComponent<CombatController>() && tmpObject.GetComponent<MovementController>().team != team)
                        {
                            tmpObject.GetComponent<CombatController>().TakeDamage(combatController.GetDamageValue());
                        }
                    }
                    if (gridBehaviour.gridArray[x, y - 1].GetComponent<GridStat>().objektOnTile)
                    {
                        GameObject tmpObject = gridBehaviour.gridArray[x, y - 1].GetComponent<GridStat>().objektOnTile;
                        if (tmpObject.GetComponent<CombatController>() && tmpObject.GetComponent<MovementController>().team != team)
                        {
                            tmpObject.GetComponent<CombatController>().TakeDamage(combatController.GetDamageValue());
                        }
                    }
                }

                playerController.ChangeTurn();

                return;
            }
        }

        if (path.Count <= 0)
        {
            return;
        }

        if (path.Count > movementLeft)
        {
            path.RemoveRange(0, path.Count - movementLeft - 1);
            movementLeft -= path.Count - 1;
        }

        if (objectOnTile && unitStats.atkRange != 0)
        {
            int pathPosition = 0;
            for (int i = 1; i < path.Count; i++)
            {
                sum = Mathf.Abs(path[i].GetComponent<GridStat>().x - objectOnTile.GetComponent<MovementController>().gridXPosition) + Mathf.Abs(path[i].GetComponent<GridStat>().y - objectOnTile.GetComponent<MovementController>().gridYPosition);
                if (sum == unitStats.atkRange)
                {
                    pathPosition = i;
                }
            }
            path.RemoveRange(0, pathPosition);
            movementLeft += pathPosition;
        }

        x = path[0].GetComponent<GridStat>().x;
        y = path[0].GetComponent<GridStat>().y;

        ShowPathToMouse(x, y);

        StartCoroutine(LerpPosition(x,y,speed, objectOnTile, movementLeft));
    }

    IEnumerator LerpPosition(int x, int y, float duration, GameObject objectOnTile, int movementLeft)
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

        if (objectOnTile)
        {
            int sum = Mathf.Abs(x- objectOnTile.GetComponent<MovementController>().gridXPosition) + Mathf.Abs(y- objectOnTile.GetComponent<MovementController>().gridYPosition);
            
            if (sum <= unitStats.atkRange && movementLeft >= 1 || unitStats.atkRange == 0 && sum <= 1 && movementLeft >= 1)
            {
                print(gameObject.name + " moved to attack " + objectOnTile.name);

                CombatController cmController = objectOnTile.GetComponent<CombatController>();

                cmController.TakeDamage(combatController.GetDamageValue());

                if (unitStats.splashDamage)
                {
                    x = objectOnTile.GetComponent<MovementController>().gridXPosition;
                    y = objectOnTile.GetComponent<MovementController>().gridYPosition;
                    if (gridBehaviour.gridArray[x + 1, y].GetComponent<GridStat>().objektOnTile)
                    {
                        GameObject tmpObject = gridBehaviour.gridArray[x + 1, y].GetComponent<GridStat>().objektOnTile;
                        if (tmpObject.GetComponent<CombatController>() && tmpObject.GetComponent<MovementController>().team != team)
                        {
                            tmpObject.GetComponent<CombatController>().TakeDamage(combatController.GetDamageValue());
                        }
                    }
                    if (gridBehaviour.gridArray[x - 1, y].GetComponent<GridStat>().objektOnTile)
                    {
                        GameObject tmpObject = gridBehaviour.gridArray[x - 1, y].GetComponent<GridStat>().objektOnTile;
                        if (tmpObject.GetComponent<CombatController>() && tmpObject.GetComponent<MovementController>().team != team)
                        {
                            tmpObject.GetComponent<CombatController>().TakeDamage(combatController.GetDamageValue());
                        }
                    }
                    if (gridBehaviour.gridArray[x, y + 1].GetComponent<GridStat>().objektOnTile)
                    {
                        GameObject tmpObject = gridBehaviour.gridArray[x, y + 1].GetComponent<GridStat>().objektOnTile;
                        if (tmpObject.GetComponent<CombatController>() && tmpObject.GetComponent<MovementController>().team != team)
                        {
                            tmpObject.GetComponent<CombatController>().TakeDamage(combatController.GetDamageValue());
                        }
                    }
                    if (gridBehaviour.gridArray[x, y - 1].GetComponent<GridStat>().objektOnTile)
                    {
                        GameObject tmpObject = gridBehaviour.gridArray[x, y - 1].GetComponent<GridStat>().objektOnTile;
                        if (tmpObject.GetComponent<CombatController>() && tmpObject.GetComponent<MovementController>().team != team)
                        {
                            tmpObject.GetComponent<CombatController>().TakeDamage(combatController.GetDamageValue());
                        }
                    }
                }
            }
        }

        if (movementLeft <= 0)
        {
            playerController.ChangeTurn();
        }
        else
        {
            StartCoroutine(playerController.EnableInteract());
        }
    }

    public void ShowPathToMouse(int mouseX, int mouseY, bool isEnemy = false)
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
