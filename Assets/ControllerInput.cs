using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerInput : MonoBehaviour
{
    public InputActionReference primaryButtonRef = null;
    public InputActionReference secondaryButtonRef = null;
    public GameObject sphere;
    public GameObject gridGen;
    public Vector4 previousResponse = new Vector4(0,0,0,0);
    private Renderer sphereRenderer;
    private Camera camera_Component;
    List<Vector4> data = new List<Vector4>();
    // Start is called before the first frame update
    private void Start()
    {
        AssignButtonActions();
        sphereRenderer = sphere.GetComponent<Renderer>();
    }

    private void AssignButtonActions()
    {
        primaryButtonRef.action.started += RespondToStimulus;
        secondaryButtonRef.action.started += SwapEye;
    }

    private void OnDestroy()
    {
        primaryButtonRef.action.started -= RespondToStimulus;
        secondaryButtonRef.action.started -= SwapEye;

    }

    private void RespondToStimulus(InputAction.CallbackContext context)
    {
        Vector4 curr_Response = new Vector4(sphere.transform.position.x, sphere.transform.position.y, sphere.transform.position.z, gridGen.GetComponent<GridLogic>().brightness);
        if (previousResponse != curr_Response)
        {
            Debug.Log(curr_Response);
            data.Add(curr_Response);
            previousResponse = curr_Response;
        }
    }
    private void SwapEye(InputAction.CallbackContext context)
    {
        string curr_Eye = "Both";
        if (curr_Eye == "Both")
        {
            curr_Eye = "Left";
            camera_Component.stereoTargetEye = StereoTargetEyeMask.Left;
        }
        else if (curr_Eye == "Left")
        {
            curr_Eye = "Right";
            camera_Component.stereoTargetEye = StereoTargetEyeMask.Right;
        }
        else
        {
            curr_Eye = "Both";
            camera_Component.stereoTargetEye = StereoTargetEyeMask.Both;
        }
    }
}
