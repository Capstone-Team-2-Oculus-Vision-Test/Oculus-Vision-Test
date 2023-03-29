using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Screenshot : MonoBehaviour
{
    public bool enablePicture = false;
    public string fileName = "screenshot.png";

    public void CaptureScreen()
    {
        if (enablePicture)
        {
            string path = Path.Combine(Application.dataPath, fileName);
            ScreenCapture.CaptureScreenshot(path);
        }
    }
}