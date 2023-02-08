using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public bool playersTurn = false;

    private bool canInteract = true;

    bool unitSelected = false;
    MovementController selectedUnit;

    [SerializeField]
    string playersTeam = "white";

    public LayerMask groundLayer;

    private void Update()
    {
        if (unitSelected)
        {
            RaycastHit hit = MouseRayCast();
            GameObject lastHit = null;

            if (hit.transform == null)
            {
                return;
            }

            if (hit.transform.gameObject == lastHit)
            {
                return;
            }

            lastHit = hit.transform.gameObject;

            if (hit.transform.GetComponent<GridStat>())
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

    public void OnSpace()
    {
        selectedUnit.GetComponent<CombatController>().TakeDamage(2);
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
        
        if (!canInteract)
        {
            return;
        }

        if (unitSelected)
        {
            if (hit.transform.GetComponent<GridStat>().objektOnTile)
            {
                GameObject objectOnTile = hit.transform.GetComponent<GridStat>().objektOnTile;


                if (objectOnTile.GetComponent<MovementController>().team == playersTeam)
                {
                    SelectUnit(hit);
                    return;
                }
                else if (objectOnTile.GetComponent<MovementController>().team != playersTeam)
                {
                    MoveUnit(hit, true);
                    return;
                }
            }
            MoveUnit(hit);
            return;
        }

        if (hit.transform.GetComponent<GridStat>().objektOnTile != null && hit.transform.GetComponent<GridStat>().objektOnTile.GetComponent<MovementController>().team == playersTeam)
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

        selectedUnit = hit.transform.GetComponent<GridStat>().objektOnTile.GetComponent<MovementController>();
        selectedUnit.SelectUnit();
        unitSelected = true;
    }

    private void MoveUnit(RaycastHit hit, bool attack = false)
    {
        canInteract = false;

        int x = hit.transform.GetComponent<GridStat>().x;
        int y = hit.transform.GetComponent<GridStat>().y;

        if (attack)
        {
            selectedUnit.MoveToLocation(x, y, hit.transform.GetComponent<GridStat>().objektOnTile);
        }
        else
        {
            selectedUnit.MoveToLocation(x, y);
        }
        unitSelected = false;
        selectedUnit.DeselectUnit();
        selectedUnit = null;
    }

    public IEnumerator EnableInteract()
    {
        yield return new WaitForSeconds(0.1f);
        canInteract = true;
    }

    public void OnUnitDeath(GameObject unit)
    {
        if (selectedUnit == unit.GetComponent<MovementController>())
        {
            unitSelected = false;
            selectedUnit.DeselectUnit();
            selectedUnit = null;
        }
    }

    private void OnEnable()
    {
        CombatController.onDie += OnUnitDeath;
    }

    private void OnDisable()
    {
        CombatController.onDie -= OnUnitDeath;
    }

}
