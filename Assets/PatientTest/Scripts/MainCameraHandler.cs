using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraHandler : MonoBehaviour
{
    [SerializeField]
    private bool right;

    [SerializeField]
    private bool left;

    private bool set = false;

    [SerializeField]
    private GameObject[] rightObjects;
    
    [SerializeField]
    private GameObject[] leftObjects;

    public void setRight()
    {
        right = true;
        left = false;
        Activation();
    }

    public void Activation()
    {
        if(left)
        {
            foreach(GameObject i in leftObjects)
                {
                    i.SetActive(true);
                }
            set = true;
        }
        else if(right)
        {
            foreach(GameObject i in rightObjects)
            {
                i.SetActive(true);
            }
            set = true;
        }
    }

    public void Deactivation(bool l)
    {
        if(l)
        {
            foreach(GameObject i in leftObjects)
            {
                i.SetActive(false);
            }
            set = false;
        }
        else
        {
            foreach(GameObject i in rightObjects)
            {
                i.SetActive(false);
            }
            set = false;
        }
    }
    public void setLeft()
    {
        right = false;
        left = true;
        Activation();
    }

    public void updateRight()
    {
        if(!set)
            setRight();
        else
        {
            Deactivation(true);
            setRight();
        }
    }

    public void updateLeft()
    {
        if(!set)
            setLeft();
        else
        {
            Deactivation(false);
            setLeft();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(left)
            setLeft();
        else
            setRight();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
