using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public bool playersTurn = false;
    bool unitSelected = false;
    MovementController selectedUnit;

    string playersTeam = "white";

    public LayerMask groundLayer;

    private void Update()
    {
        if (unitSelected)
        {
            RaycastHit hit = MouseRayCast();

            if (hit.transform != null && hit.transform.GetComponent<GridStat>())
            {
                GridStat tmpGridStat = hit.transform.GetComponent<GridStat>();
                if (hit.transform.GetComponent<MovementController>() != null && hit.transform.GetComponent<MovementController>().team != playersTeam)
                {
                    selectedUnit.GetComponent<MovementController>().ShowPathToMouse(tmpGridStat.x, tmpGridStat.y, true);
                }
                else
                {
                    selectedUnit.GetComponent<MovementController>().ShowPathToMouse(tmpGridStat.x, tmpGridStat.y);
                }
            }
        }
    }

    public void OnFire(InputValue value)
    {
        if (playersTurn)
        {
            LeftClickChecks();
        }
    }

    private void LeftClickChecks()
    {
        RaycastHit hit = MouseRayCast();

        if (!hit.transform)
        {
            return;
        }

        if (unitSelected)
        {
            if (hit.transform.GetComponent<GridStat>().objektOnTile != null)
            {
                if (hit.transform.GetComponent<MovementController>() != null)
                {
                    if (hit.transform.GetComponent<MovementController>().team == playersTeam)
                    {
                        SelectUnit(hit);
                        return;
                    }
                    else if (hit.transform.GetComponent<MovementController>().team != playersTeam)
                    {
                        AttackUnit(hit);
                        return;
                    }
                }
            }

            MoveUnit(hit);
            return;
        }

        if (hit.transform.GetComponent<GridStat>().objektOnTile != null)
        {
            SelectUnit(hit);
            return;
        }
        else
        {
            Debug.Log(hit.transform.GetComponent<GridStat>().x + "x; " + hit.transform.GetComponent<GridStat>().y + "y");
        }
    }

    RaycastHit MouseRayCast()
    {
        RaycastHit hit;
        Ray rayCast = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(rayCast, out hit, 100, groundLayer))
        {
            return hit;
        }
        return hit;
    }

    private void SelectUnit(RaycastHit hit)
    {
        if (selectedUnit != null)
        {
            selectedUnit.DeselectUnit();
        }

        Debug.Log(hit.transform.GetComponent<GridStat>().objektOnTile.name);
        selectedUnit = hit.transform.GetComponent<GridStat>().objektOnTile.GetComponent<MovementController>();
        selectedUnit.SelectUnit();
        unitSelected = true;
    }

    private void MoveUnit(RaycastHit hit)
    {
        int x = hit.transform.GetComponent<GridStat>().x;
        int y = hit.transform.GetComponent<GridStat>().y;

        selectedUnit.MoveToLocation(x, y);
        unitSelected = false;
        selectedUnit.DeselectUnit();
        selectedUnit = null;
    }

    private void AttackUnit(RaycastHit hit)
    {
        int x = hit.transform.GetComponent<GridStat>().x;
        int y = hit.transform.GetComponent<GridStat>().y;

        selectedUnit.AttackAtLocation(x, y);
        unitSelected = false;
        selectedUnit.DeselectUnit();
        selectedUnit = null;
    }

}
