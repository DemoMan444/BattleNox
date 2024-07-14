using System.Collections.Generic;
using UnityEngine;

public class SelectionManager
{
    private static SelectionManager _instance;

    public static SelectionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SelectionManager();
            }
            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }

    public HashSet<SelectableUnit> SelectedUnits = new HashSet<SelectableUnit>();
    public List<SelectableUnit> AvailableUnits = new List<SelectableUnit>();

    private SelectionManager() { }

    public void Select(SelectableUnit Unit)
    {
        if (Unit != null)
        {
            SelectedUnits.Add(Unit);
            Unit.OnSelected();
        }
        else
        {
            Debug.LogWarning("Attempted to select a null unit.");
        }
    }

    public void Deselect(SelectableUnit Unit)
    {
        if (Unit != null)
        {
            Unit.OnDeselected();
            SelectedUnits.Remove(Unit);
        }
        else
        {
            Debug.LogWarning("Attempted to deselect a null unit.");
        }
    }

    public void DeselectAll()
    {
        var nonNullUnits = new List<SelectableUnit>();

        foreach (SelectableUnit unit in SelectedUnits)
        {
            if (unit != null)
            {
                unit.OnDeselected();
                nonNullUnits.Add(unit);
            }
            else
            {
                Debug.LogWarning("Found a null unit in SelectedUnits.");
            }
        }

        SelectedUnits.Clear();
        foreach (var unit in nonNullUnits)
        {
            SelectedUnits.Add(unit);
        }
    }

    public bool IsSelected(SelectableUnit Unit)
    {
        return SelectedUnits.Contains(Unit);
    }
}
