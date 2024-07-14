using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreaturesHealthManager : MonoBehaviour
{
    public int maxHealth;

    public int currentHealth { get; private set; }

    public static CreaturesHealthManager instance;

    void Awake()
    { currentHealth = maxHealth; }

    void Update()
    {
       // if (DataStorage.instance.enemyHealth <= 0) { Death(); }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if(currentHealth <= 0)
        {  Death(); }

        // Debug.Log("Enemy attacked currentHealth: " + currentHealth);
        
    }

    void Death()
    {
        // gameObject.SetActive(false);
        Destroy(gameObject);
        DataStorage.instance.EnemyKilled(1);
    }
}
