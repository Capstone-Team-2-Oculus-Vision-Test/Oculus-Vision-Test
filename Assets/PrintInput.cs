using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PrintInput : MonoBehaviour
{
    public InputActionReference printRef = null;
    public GameObject sphere;
    // Start is called before the first frame update
    private void Awake()
    {
        printRef.action.started += PrintPressed;
    }

    // Update is called once per frame
    private void OnDestroy()
    {
        printRef.action.started -= PrintPressed;
        
    }

    private void PrintPressed(InputAction.CallbackContext context)
    {
        Debug.Log(sphere.transform.position);
    }
}
