using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class textPopulate : MonoBehaviour
{
    private Transform tf;
    private Screenshot ss;
    private List<Vector4> data = new List<Vector4>();
    //private List<Vector4> data = DataTransfer.data; //enable once supported
    public GameObject camera;
    private void Awake()
    {
        tf = gameObject.transform;
        ss = camera.GetComponent<Screenshot>();
        data.Add(new Vector4(27, 9, 0, 50));
        data.Add(new Vector4(9, 9, 0, 30));
        data.Add(new Vector4(3, -9, 0, 25));

    }


    void Start()
    {
        for (int i = 0; i < tf.childCount; i++)
        {
            Transform childTF = tf.GetChild(i);
            GameObject childGO = childTF.gameObject;
            RectTransform rTF = childGO.GetComponent<RectTransform>();
            float posX = rTF.anchoredPosition.x/10;
            float posY = rTF.anchoredPosition.y/10;
            foreach(Vector4 v in data)
            {
                if(posX == v.x && posY == v.y)
                {
                    childGO.GetComponent<TextMeshProUGUI>().text = v.w.ToString();
                }
            }
        }
        ss.CaptureScreen();
    }
}
