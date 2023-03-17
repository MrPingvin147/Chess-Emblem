using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    private bool canInteract = true;

    bool unitSelected = false;
    MovementController selectedUnit;

    [SerializeField]
    string playersTeam = "white";

    public LayerMask groundLayer;

    CinemachineVirtualCamera virtualCamera;
    [HideInInspector]
    public Transform cameraRotationPoint;

    private void Awake()
    {
        CreateCamera();

        MoveCamera();
    }

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
        LeftClickChecks();
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

                if (objectOnTile.GetComponent<MovementController>())
                {
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
        selectedUnit.GetComponent<CombatController>().UpdateHealthbar();
    }
    private void SelectUnit(MovementController unit = null)
    {
        if (selectedUnit != null)
        {
            selectedUnit.DeselectUnit();
        }

        selectedUnit = unit;
        selectedUnit.SelectUnit();
        unitSelected = true;
        selectedUnit.GetComponent<CombatController>().UpdateHealthbar();
    }

    private void MoveUnit(RaycastHit hit, bool attack = false)
    {
        canInteract = false;

        int x = hit.transform.GetComponent<GridStat>().x;
        int y = hit.transform.GetComponent<GridStat>().y;

        if (attack)
        {
            selectedUnit.MoveToLocation(x, y, selectedUnit.unitStats.spd, hit.transform.GetComponent<GridStat>().objektOnTile);
        }
        else
        {
            selectedUnit.MoveToLocation(x, y, selectedUnit.unitStats.spd);
        }

        if (unitSelected)
        {
            unitSelected = false;
            selectedUnit.DeselectUnit();
            selectedUnit = null;
        }
    }

    public IEnumerator EnableInteract()
    {
        yield return new WaitForSeconds(0.1f);
        canInteract = true;
    }

    public void ChangeTurn()
    {
        unitSelected = false;
        
        if (selectedUnit != null)
        {
            selectedUnit.DeselectUnit();
            selectedUnit = null;
        }

        canInteract = true;
        
        if (playersTeam == "white")
        {
            playersTeam = "black";
        }
        else
        {
            playersTeam = "white";
        }

        MoveCamera();
    }

    private void MoveCamera()
    {
        if (playersTeam == "white")
        {
            virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = new Vector3(0, 4, 8.25f);
        }
        else
        {
            virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = new Vector3(0, 4, -8.25f);
        }

        StartCoroutine(EnableInteract());
    }

    private void CreateCamera()
    {
        GameObject tmpGameobject = new GameObject("CinemachineVirtualCamera");
        virtualCamera = tmpGameobject.AddComponent<CinemachineVirtualCamera>();
        CinemachineFramingTransposer tmpTransposer = virtualCamera.AddCinemachineComponent<CinemachineFramingTransposer>();
        virtualCamera.AddCinemachineComponent<CinemachineComposer>();

        tmpGameobject = new GameObject("cameraRotationPoint");

        cameraRotationPoint = tmpGameobject.transform;

        virtualCamera.m_Lens.FieldOfView = 60f;
        virtualCamera.LookAt = cameraRotationPoint;
        virtualCamera.Follow = cameraRotationPoint;
    }

    public void OnUnitDeath(GameObject unit)
    {
        if (selectedUnit == unit.GetComponent<MovementController>())
        {
            unitSelected = false;
            selectedUnit.DeselectUnit();
            selectedUnit = null;
        }

        if (unit.GetComponent<MovementController>().unitStats.className == "King")
        {
            playersTeam = "white";
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
