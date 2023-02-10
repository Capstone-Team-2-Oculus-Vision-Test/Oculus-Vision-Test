using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerInput : MonoBehaviour
{
    public InputActionReference primaryButtonRef = null;
    public GameObject sphere;
    public Vector3 previousResponse = new Vector3(0,0,0);
    public List<Vector3> flashesRespondedTo = new List<Vector3>();
    private Renderer sphereRenderer;
    // Start is called before the first frame update
    private void Start()
    {
        AssignButtonActions();
        sphereRenderer = sphere.GetComponent<Renderer>();
    }

    private void AssignButtonActions()
    {
        primaryButtonRef.action.started += RespondToStimulus;
    }

    private void OnDestroy()
    {
        primaryButtonRef.action.started -= RespondToStimulus;
        
    }

    private void RespondToStimulus(InputAction.CallbackContext context)
    {
        Vector3 curr_Response = sphere.transform.position;
        if (previousResponse != curr_Response)
        {
            Debug.Log(curr_Response);
            flashesRespondedTo.Add(curr_Response);
            previousResponse = curr_Response;
        }
    }
}
