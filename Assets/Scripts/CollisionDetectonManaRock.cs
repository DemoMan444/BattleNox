using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetectonManaRock : MonoBehaviour
{
   
    [SerializeField] private int increaseManaBy; 

    public GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Player") )
        {

            if (DataStorage.instance.playerMana < DataStorage.instance.maxPlayerMana)
            {
                
                DataStorage.instance.IncreaseMana(increaseManaBy); // Using the DataStorage
            }

        }

    }
    
}

