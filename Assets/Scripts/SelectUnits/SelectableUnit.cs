using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SelectableUnit : MonoBehaviour
{
    private NavMeshAgent Agent;             // Reference to the NavMeshAgent component
    [SerializeField] private SpriteRenderer SelectionSprite; // Reference to the selection sprite renderer
    public GameObject turnOffAI;            // Reference to the GameObject with MoveNCollide script

    private void Awake()
    {
        // Register this unit in the SelectionManager's list of available units
        SelectionManager.Instance.AvailableUnits.Add(this);

        // Get the NavMeshAgent component attached to this GameObject
        Agent = GetComponent<NavMeshAgent>();
    }

    // Move the unit to the specified position
    public void MoveTo(Vector3 Position)
    {
        Agent.SetDestination(Position);
    }

    // Handle unit's behavior when it's selected
    public void OnSelected()
    {
        if (SelectionSprite != null)
        {
            SelectionSprite.gameObject.SetActive(true); // Show the selection sprite
        }
        else
        {
            Debug.LogWarning("SelectionSprite is null");
        }

        // Disable the MoveNCollide script to turn off AI movement
        if (turnOffAI != null)
        {
            var moveScript = turnOffAI.GetComponent<MoveNCollide>();
            if (moveScript != null)
            {
                moveScript.enabled = false;
            }
            else
            {
                Debug.LogWarning("MoveNCollide script not found on turnOffAI GameObject");
            }
        }
        else
        {
            Debug.LogWarning("turnOffAI is null");
        }
    }

    // Handle unit's behavior when it's deselected
    public void OnDeselected()
    {
        if (SelectionSprite != null)
        {
            SelectionSprite.gameObject.SetActive(false); // Hide the selection sprite
        }
        else
        {
            Debug.LogWarning("SelectionSprite is null");
        }

        // Enable the MoveNCollide script to turn on AI movement
        if (turnOffAI != null)
        {
            var moveScript = turnOffAI.GetComponent<MoveNCollide>();
            if (moveScript != null)
            {
                moveScript.enabled = true;
            }
            else
            {
                Debug.LogWarning("MoveNCollide script not found on turnOffAI GameObject");
            }
        }
        else
        {
            Debug.LogWarning("turnOffAI is null");
        }
    }
}
