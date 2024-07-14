using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellsManager : MonoBehaviour
{
    // Summoning part

    public GameObject[] objects; // Array of GameObjects for different creatures that can be summoned
    private GameObject pendingObject; // The currently selected creature object to be placed

    private Vector3 pos; // Position where the creature object will be placed

    private RaycastHit hit; // Used to detect raycast hits
    [SerializeField] private LayerMask layerMask; // Layer mask for raycasting

    public int addHealth; // Amount of health to add

    // Update is called once per frame
    void Update()
    {
        if (pendingObject != null)
        {
            pendingObject.transform.position = pos; // Update the pending object's position

            if (Input.GetMouseButtonDown(0))
            {
                PlaceObject(); // Place the pending object when left mouse button is pressed
                DataStorage.instance.SummonUsingMana(4); // Reduce player's mana when object is placed
            }
        }
    }

    void PlaceObject()
    {
        pendingObject = null; // Reset the pending object after it's placed
    }

    private void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Cast a ray from the camera to the mouse position
        if (Physics.Raycast(ray, out hit, 1000, layerMask)) // Perform a raycast and check for hit
        {
            pos = hit.point; // Update the position where the creature object will be placed
        }
    }

    public void ChooseCreatureToBeSummoned(int index)
    {
        if (DataStorage.instance.playerMana >= 4) // Check if player has enough mana
        {
            pendingObject = Instantiate(objects[index], pos, transform.rotation); // Instantiate the selected creature object at the current position
        }
    }

    // Healing part

    public void LesserHeal()
    {
        if (DataStorage.instance.playerMana >= 2 && DataStorage.instance.playerHealth <= DataStorage.instance.maxPlayerHealth - addHealth)
        {
            // Check if player has enough mana and if adding health won't exceed maximum health
            DataStorage.instance.IncreaseHealth(addHealth); // Increase player's health
            DataStorage.instance.SetPlayerMana(2); // Reduce player's mana
        }
    }
}
