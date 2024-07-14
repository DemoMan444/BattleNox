using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class MoveNCollide : MonoBehaviour
{
    [SerializeField] float range = 1.5f; // Range for detection
    Transform enemy; // Transform of the enemy

    private GameObject Target; // Current target
    private float maxDistance; // Maximum distance for targeting

    [SerializeField] private int decreasePlayerHealthBy; // Amount to decrease player's health
    public bool canDamagePlayer = true; // Bool to control if object is ally or enemy
    private string Tag1; // Tag for detecting player
    private string Tag2; // Tag for detecting other targets

    public NavMeshAgent agent; // Reference to the NavMeshAgent component
    Animator animator; // Reference to the Animator component
    public Transform closestObject; // Transform representing the closest object to be attacked position
    public GameObject player; // Reference to player object

    const string IDLE = "Walk 1";
    const string WALK = "Walk 1";
    const string ATTACK = "Attack1h1";
    const string HIT = "Hit1";
    const string FALL = "Fall1";

    bool skeletonFighting = false; // Flag for fighting state

    private bool CollisionBool; // Flag for collision detection

    void Start()
    {
        // Find the closest player and initialize references
        closestObject = GameObject.FindWithTag("Player").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Logic for targeting and attacking based on the canDamagePlayer setting
        if (canDamagePlayer)
        {
            Tag1 = "Player";
            Tag2 = "Warrior";
            FindClosestTo();
        }
        else if (canDamagePlayer == false)
        {
            Tag1 = "Player";
            Tag2 = "Interactable";
            FindClosestTo();

            bool isAttacking = animator.GetBool("isAttacking");

            if (CollisionBool == false)
            {
                animator.SetBool("isAttacking", false);
            }
        }
    }

    void LateUpdate()
    {
        CollisionBool = false; // Reset collision flag
    }

    void SetAnimations()
    {
        if (skeletonFighting) return;

        animator.SetBool("isAttacking", false);
    }

    public void FindClosestTo()
    {
        List<GameObject> nearestEnemies = new List<GameObject>();
        GameObject lastAddedEnemy = null;
        GameObject enemyTemp = null;

        float maxDistance = Mathf.Infinity;
        Vector3 position = transform.position;

        // Find the nearest GameObject with Tag1 (e.g., "Player")
        enemyTemp = FindNearestWithTag(Tag1);
        if (enemyTemp != null && enemyTemp != lastAddedEnemy)
        {
            nearestEnemies.Add(enemyTemp);
            lastAddedEnemy = enemyTemp;
        }

        // Find the nearest GameObject with Tag2 (e.g., "Warrior" or "Interactable")
        enemyTemp = FindNearestWithTag(Tag2);
        if (enemyTemp != null && enemyTemp != lastAddedEnemy)
        {
            nearestEnemies.Add(enemyTemp);
            lastAddedEnemy = enemyTemp;
        }

        // Find the closest enemy from the list of nearestEnemies
        foreach (var enemy in nearestEnemies)
        {
            Vector3 diff = enemy.transform.position - position;
            float currentDistance = diff.sqrMagnitude;
            if (currentDistance < maxDistance)
            {
                enemyTemp = enemy;
                maxDistance = currentDistance;
            }
        }

        // Set the closest enemy as the target
        Target = enemyTemp;
        closestObject = Target?.GetComponent<Transform>();

        // Check the distance between the closestObject and the object carrying the component
        if (closestObject != null && Vector3.Distance(transform.position, closestObject.position) > 7)
        {
            // Rotate gameobject to face the closestObject
            transform.LookAt(closestObject);

            // Perform raycast to check for obstacles and adjust NavMeshAgent behavior
            RaycastHit hit;
            var didHit = Physics.Raycast(transform.position, transform.forward, out hit, 40f);
            if (didHit && hit.collider.tag == "Interactable")
            {
                agent.enabled = true; // Enable the NavMeshAgent for pathfinding               
            }
            else
            {
                agent.enabled = false; // Disable the NavMeshAgent
            }
        }
        else if (closestObject != null)
        {
            agent.enabled = true; // Enable the NavMeshAgent
        }

        // If a closestObject exists, enable NavMeshAgent and perform pathfinding
        if (closestObject != null)
        {
            agent.enabled = true; // Enable the NavMeshAgent

            // Perform A* pathfinding to find the path to the player's position
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(transform.position, closestObject.position, NavMesh.AllAreas, path))
            {
                // Set the calculated path to the NavMeshAgent
                agent.SetPath(path);
            }
        }
    }

    private GameObject FindNearestWithTag(string tagName)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(tagName);
        GameObject closest = null;
        float maxDistance = Mathf.Infinity;
        Vector3 position = transform.position;

        foreach (var enemy in enemies)
        {
            Vector3 diff = enemy.transform.position - position;
            float currentDistance = diff.sqrMagnitude;
            if (currentDistance < maxDistance)
            {
                closest = enemy;
                maxDistance = currentDistance;
            }
        }

        return closest;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Logic for collision interactions based on canDamagePlayer setting
        if (canDamagePlayer == false)
        {
            CollisionBool = true;

            if (collision.gameObject.CompareTag("Interactable") && closestObject != null)
            {
                var healthManager = closestObject.GetComponent<CreaturesHealthManager>();
                if (healthManager != null)
                {
                    healthManager.TakeDamage(1);
                    skeletonFighting = true;
                    animator.SetBool("isAttacking", true);
                }
            }
        }
        else if (canDamagePlayer == true)
        {
            CollisionBool = true;

            if (collision.gameObject.CompareTag("Warrior") && closestObject != null)
            {
                var healthManager = closestObject.GetComponent<CreaturesHealthManager>();
                if (healthManager != null)
                {
                    healthManager.TakeDamage(1);

                    if (healthManager.currentHealth <= 0)
                    {
                        Destroy(closestObject.gameObject);
                    }
                }
            }

            if (collision.gameObject.CompareTag("Player") && canDamagePlayer)
            {
                DataStorage.instance.DecreaseHealth(decreasePlayerHealthBy); // Decrease the player's health using the DataStorage class

                if (DataStorage.instance.playerHealth <= 0)
                {
                    PlayerDeath();
                }
            }
        }
    }

    void PlayerDeath()
    {
        Destroy(player);
        SceneManager.LoadSceneAsync("EndMenuLost");
    }
}
