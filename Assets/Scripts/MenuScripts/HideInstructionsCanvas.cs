using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideInstructionsCanvas : MonoBehaviour
{
    public GameObject Instructions;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void hide()
    {
        Instructions.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
