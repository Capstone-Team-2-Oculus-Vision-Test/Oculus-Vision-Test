using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataTransfer : MonoBehaviour
{
    //Patient info (may not be necessary, could just put placeholders)
    public string patientName;
    public int patientID;
    public string patientDOB;

    //Test info
    public static List<Vector4> data;
    public string eyeTested; //Left or right eye
    public float testStartTime;
    public float testEndTime;
    public float testDuration;
    public string testAlgorithm;
    public float stimulusSize;
    public string stimulusColor;
    public int numPoints;
    public float errorRate;
}
