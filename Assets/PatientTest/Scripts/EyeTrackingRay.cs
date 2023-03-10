using System.Collections.Generic;
using UnityEngine;

namespace PatientTest.Scripts
{
    [RequireComponent(typeof(LineRenderer))]
public class EyeTrackingRay : MonoBehaviour
{


    [SerializeField]
    private float rayDistance = 1.0f;

    [SerializeField]
    private float rayWidth = .01f;

    [SerializeField]
    private LayerMask layersInclude;

    [SerializeField]
    private Color rayDefaultColor = Color.green;

    [SerializeField]
    private Color rayColorState = Color.red;

    private LineRenderer lineRenderer;

    private List<EyeTarget> eyeList = new List<EyeTarget>();

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupRay();
    }

    void SetupRay()
    {
        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = rayWidth;
        lineRenderer.endWidth = rayWidth;
        lineRenderer.startColor = rayDefaultColor;
        lineRenderer.endColor = rayDefaultColor;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, new Vector3(transform.position.x, transform.position.y, transform.position.z + rayDistance));
    }

    void FixedUpdate()
    {
        RaycastHit hit;

        Vector3 rayCastDirection = transform.TransformDirection(Vector3.forward) * rayDistance;

            //if(Physics.Raycast(transform.position, new Vector3(3,3,3), out hit, Mathf.Infinity, layersInclude))
            if(Physics.Raycast(transform.position, rayCastDirection, out hit, Mathf.Infinity, layersInclude))
            {
                //Debug.Log("Ray Hit:" + hit.transform.name);
                UnSelect();
            lineRenderer.startColor = rayColorState;
            lineRenderer.endColor = rayColorState;
            var eyeInteractable = hit.transform.GetComponent<EyeTarget>();
            eyeList.Add(eyeInteractable);
            //if(eyeInteractable != null)
            eyeInteractable.IsHovered = true;
        }

        else
        {
            lineRenderer.startColor = rayDefaultColor;
            lineRenderer.endColor = rayDefaultColor;
            UnSelect(true);
        }
    }

    void UnSelect(bool clear = false)
    {
        //Debug.Log(eyeList[0]);
        foreach(var interactable in eyeList)
        {
            interactable.IsHovered = false;
        }

        if(clear)
        {
            eyeList.Clear();
        }
    }
}
}
