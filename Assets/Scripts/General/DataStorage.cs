using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStorage : MonoBehaviour
{
    public static DataStorage instance;

    public int score
    {
        get;
        private set;
    }
    [field: SerializeField]
    public int playerHealth
    {
        get;
        private set;
    }

    [field: SerializeField]
    public int maxPlayerHealth
    {
        get;
        private set;
    }


    [field: SerializeField]
    public int maxPlayerMana
    {
        get;
        private set;
    }



    [field: SerializeField]
    public int playerMana
    {
        get;
        private set;
    }


    [field: SerializeField]
    public int enemyHealth
    {
        get;
        private set;
    }


    public int nrOfEnemiesKilled
    {
        get;
        private set;
    }
    

    public void Start()
    {
        if (instance == null)
            instance = this;
    }

    public void IncreaseScore(int increaseBy)
    {
        score += increaseBy;
        
    }

    public void IncreaseHealth(int increaseBy)
    {
        playerHealth += increaseBy;
       
    }

    public void DecreaseHealth(int decreaseBy)
    {
        playerHealth -= decreaseBy;
      
    }

    public void DecreaseEnemyHealth(int decreaseBy)
    {
        enemyHealth -= decreaseBy;
     
    }

    public void EnemyKilled(int decreaseBy)
    {
        nrOfEnemiesKilled += decreaseBy;
        
    }


    public void SetPlayerMana(int decreaseBy)
    {
        playerMana -= decreaseBy;
        
    }

    public void IncreaseMana(int increaseBy)
    {
        playerMana += increaseBy;
        
    }

    public void SummonUsingMana(int decreaceManaBy)
    {
        playerMana -= decreaceManaBy;
        
    }

    
}

