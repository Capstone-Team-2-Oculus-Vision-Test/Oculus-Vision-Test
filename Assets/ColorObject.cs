using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ColorObject : MonoBehaviour
{
    // This is an example for reading value from an input.
    public InputActionReference colorRef = null;

    private MeshRenderer meshRenderer = null;
    // Start is called before the first frame update
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        float value = colorRef.action.ReadValue<float>();
        updateColor(value);
    }

    private void updateColor(float value)
    {
        meshRenderer.material.color = new Color(value, value, value);
    }
}
