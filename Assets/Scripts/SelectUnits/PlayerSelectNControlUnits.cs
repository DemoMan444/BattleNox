using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectNControlUnits : MonoBehaviour
{
    [SerializeField] private Camera Camera;              // Reference to the camera
    [SerializeField] private RectTransform SelectionBox; // UI element for selection box
    [SerializeField] private LayerMask UnitLayers;       // Layers for selectable units
    [SerializeField] private LayerMask FloorLayers;      // Layers for floor or terrain
    [SerializeField] private float DragDelay = 0.1f;     // Delay before drag selection starts

    private float MouseDownTime;
    private Vector2 StartMousePosition;

    private HashSet<SelectableUnit> newlySelectedUnits = new HashSet<SelectableUnit>();
    private HashSet<SelectableUnit> deselectedUnits = new HashSet<SelectableUnit>();

    private void Update()
    {
        HandleSelectionInputs();    // Handle unit selection inputs
        HandleMovementInputs();     // Handle movement inputs
    }

    private void HandleSelectionInputs()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SelectionBox.sizeDelta = Vector2.zero;   // Initialize selection box
            SelectionBox.gameObject.SetActive(true); // Activate selection box UI
            StartMousePosition = Input.mousePosition; // Record initial mouse position
            MouseDownTime = Time.time;                // Record mouse down time
        }
        else if (Input.GetKey(KeyCode.Mouse0) && MouseDownTime + DragDelay < Time.time)
        {
            ResizeSelectionBox(); // Resize the selection box during drag
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            // Finalize selection and deselection based on user input
            SelectionBox.sizeDelta = Vector2.zero;     // Reset selection box
            SelectionBox.gameObject.SetActive(false); // Deactivate selection box UI

            // Select newly selected units and deselect units that were deselected
            foreach (SelectableUnit newUnit in newlySelectedUnits)
            {
                if (newUnit != null) // Check if the unit is not null
                {
                    SelectionManager.Instance.Select(newUnit);
                }
            }
            foreach (SelectableUnit deselectedUnit in deselectedUnits)
            {
                if (deselectedUnit != null) // Check if the unit is not null
                {
                    SelectionManager.Instance.Deselect(deselectedUnit);
                }
            }

            // Clear the sets for newly selected and deselected units
            newlySelectedUnits.Clear();
            deselectedUnits.Clear();

            // Check for click or drag selection of units
            if (Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, UnitLayers)
                && hit.collider.TryGetComponent<SelectableUnit>(out SelectableUnit unit))
            {
                // Use shift to select more units or deselect some
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    if (SelectionManager.Instance.IsSelected(unit))
                    {
                        SelectionManager.Instance.Deselect(unit);
                    }
                    else
                    {
                        SelectionManager.Instance.Select(unit);
                    }
                }
                // Without shift select only 1 unit
                else
                {
                    SelectionManager.Instance.DeselectAll();
                    SelectionManager.Instance.Select(unit);
                }
            }
            // If no units were selected, and it was a click (not drag), deselect all units
            else if (MouseDownTime + DragDelay > Time.time)
            {
                SelectionManager.Instance.DeselectAll();
            }

            MouseDownTime = 0; // Reset mouse down time
        }
    }

    private void ResizeSelectionBox()
    {
        // Calculate selection box dimensions and position based on mouse movement
        float width = Input.mousePosition.x - StartMousePosition.x;
        float height = Input.mousePosition.y - StartMousePosition.y;
        SelectionBox.anchoredPosition = StartMousePosition + new Vector2(width / 2, height / 2);
        SelectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));

        // Create bounds for the selection box
        Bounds bounds = new Bounds(SelectionBox.anchoredPosition, SelectionBox.sizeDelta);

        // Iterate through available units and update selection/deselection status
        for (int i = 0; i < SelectionManager.Instance.AvailableUnits.Count; i++)
        {
            SelectableUnit unit = SelectionManager.Instance.AvailableUnits[i];

            // Check if the unit has been destroyed
            if (unit == null)
            {
                continue;
            }

            Vector2 unitPosition = Camera.WorldToScreenPoint(unit.transform.position);
            if (UnitIsInSelectionBox(unitPosition, bounds))
            {
                // Handle selection and deselection based on unit position in selection box
                if (!SelectionManager.Instance.IsSelected(unit))
                {
                    newlySelectedUnits.Add(unit);
                }
                deselectedUnits.Remove(unit);
            }
            else
            {
                deselectedUnits.Add(unit);
                newlySelectedUnits.Remove(unit);
            }
        }
    }

    private bool UnitIsInSelectionBox(Vector2 Position, Bounds Bounds)
    {
        // Check if a unit's position is within the selection box
        return Position.x > Bounds.min.x && Position.x < Bounds.max.x
            && Position.y > Bounds.min.y && Position.y < Bounds.max.y;
    }

    private void HandleMovementInputs()
    {
        // Handle movement inputs for selected units
        if (Input.GetKeyUp(KeyCode.Mouse1) && SelectionManager.Instance.SelectedUnits.Count > 0)
        {
            if (Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out RaycastHit Hit, FloorLayers))
            {
                foreach (SelectableUnit unit in SelectionManager.Instance.SelectedUnits)
                {
                    // Check if the unit has been destroyed
                    if (unit == null)
                    {
                        continue;
                    }

                    unit.MoveTo(Hit.point);
                }
            }
        }
    }
}
