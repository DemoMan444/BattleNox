using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class PlayerEverything : MonoBehaviour
{
    // Animation state names
    const string IDLE = "Idle";
    const string WALK = "Walk";
    const string ATTACK = "Attack";
    const string PICKUP = "Pickup";

    // Current animation state
    string currentAnimation;

    // Custom input actions
    CustomActions input;

    // Components references
    NavMeshAgent agent;
    Animator animator;

    // Movement settings
    [Header("Movement")]
    [SerializeField] ParticleSystem clickEffect;
    [SerializeField] LayerMask clickableLayers;
    float lookRotationSpeed = 8f;

    // Attack settings
    [Header("Attack")]
    [SerializeField] float attackSpeed = 1.5f;
    [SerializeField] float attackDelay = 0.3f;
    [SerializeField] float attackDistance = 1.5f;
    [SerializeField] int attackDamage = 1;
    [SerializeField] ParticleSystem hitEffect;

    // Player's busy state and current interaction target
    bool playerBusy = false;
    PlayerCanInteractWith target;

    // Initializing components and input actions
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        input = new CustomActions();
        AssignInputs();
    }

    // Assigning input actions
    void AssignInputs()
    {
        input.Main.Move.performed += ctx => ClickToMove();
    }

    // Handling player movement based on mouse click
    void ClickToMove()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, clickableLayers))
        {
            if (hit.transform.CompareTag("Interactable") || hit.transform.CompareTag("InteractableItem"))
            {
                target = hit.transform.GetComponent<PlayerCanInteractWith>();
                if (clickEffect != null)
                {
                    Instantiate(clickEffect, hit.transform.position + new Vector3(0, 0.1f, 0), clickEffect.transform.rotation);
                }
            }
            else
            {
                target = null;

                agent.destination = hit.point;
                if (clickEffect != null)
                {
                    Instantiate(clickEffect, hit.point + new Vector3(0, 0.1f, 0), clickEffect.transform.rotation);
                }
            }
        }
    }

    // Enabling input actions
    void OnEnable()
    {
        input.Enable();
    }

    // Disabling input actions
    void OnDisable()
    {
        input.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        FollowTarget();
        FaceTarget();
        SetAnimations();
    }

    // Following the target and attacking if within range
    void FollowTarget()
    {
        if (target == null) return;

        if (Vector3.Distance(target.transform.position, transform.position) <= attackDistance)
        {
            ReachDistance();
        }
        else
        {
            agent.SetDestination(target.transform.position);
        }
    }

    // Facing the target or movement direction
    void FaceTarget()
    {
        if (agent.destination == transform.position) return;

        Vector3 facing = Vector3.zero;
        if (target != null)
        {
            facing = target.transform.position;
        }
        else
        {
            facing = agent.destination;
        }

        Vector3 direction = (facing - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
    }

    // Handling interaction when reaching attack distance
    void ReachDistance()
    {
        agent.SetDestination(transform.position);

        if (playerBusy) return;

        // playerBusy we use for animation purpose. When busy then attack animation else walking or idle
        playerBusy = true;

        switch (target.interactionType)
        {
            case PlayerCanInteractWithType.Enemy:
                
                animator.Play(ATTACK);

                // Invoke schedules the execution of the SendAttack method after 'attackDelay' seconds
                Invoke(nameof(SendAttack), attackDelay);

                // Schedule the execution of the ResetBusyState method after 'attackSpeed' seconds
                Invoke(nameof(ResetBusyState), attackSpeed);
                break;
            case PlayerCanInteractWithType.Item:

                // Call the InteractWithItem method on the 'target' object
                target.InteractWithItem();

                // Set 'target' to null
                target = null;

                // Schedule the execution of the ResetBusyState method after 0.5 seconds
                Invoke(nameof(ResetBusyState), 0.5f);
                break;
        }
    }

    // Performing attack action
    void SendAttack()
    {
        if (target == null) return;
        // specificCreatureHealth
        if (target.specificCreatureHealth.currentHealth <= 0)
        {
            target = null;
            return;
        }

        Instantiate(hitEffect, target.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        target.GetComponent<CreaturesHealthManager>().TakeDamage(attackDamage);

        DataStorage.instance.IncreaseScore(1);

        Debug.Log(DataStorage.instance.score);
    }

    // Resetting busy state after an action
    void ResetBusyState()
    {
        playerBusy = false;
        SetAnimations();
    }

    // Setting animations based on player state
    void SetAnimations()
    {
        if (playerBusy) return;

        if (agent.velocity == Vector3.zero)
        {
            animator.Play(IDLE);
        }
        else
        {
            animator.Play(WALK);
        }
    }
}
