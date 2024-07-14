using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerCanInteractWithType { Enemy, Item }

public class PlayerCanInteractWith : MonoBehaviour
{
    public CreaturesHealthManager specificCreatureHealth { get; private set; }

    public PlayerCanInteractWithType interactionType;

    void Awake() 
    {
        // Set it in inspector
        if(interactionType == PlayerCanInteractWithType.Enemy)
        { specificCreatureHealth = GetComponent<CreaturesHealthManager>(); }
    }

    public void InteractWithItem()
    {
        // Pickup Item
        Destroy(gameObject);
    }
}
