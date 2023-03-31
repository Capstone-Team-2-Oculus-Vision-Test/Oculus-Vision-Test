using System.Collections.Generic;
using UnityEngine;

namespace PatientTest.Scripts
{
    public static class DataTransfer
    {
        public static TestResultsDTO resultsDTO = new TestResultsDTO();
        public static PatientDTO patientDTO = new PatientDTO();
    }

    public class TestResultsDTO
    {
        public List<Vector4> data;
        public string eyeTested; //Left or right eye
        public System.DateTime startTime;
        public System.DateTime endTime;
        public float duration;
        public string algorithm = "Specvis";
        public float stimulusSize;
        public Color stimulusColor;
        public int numPoints;
        public float errorRate;
    }

    public class PatientDTO
    {
        //Patient info (may not be necessary, could just put placeholders)
        public string id;
        public string name;
        public string age;
        public string sex;
    }
}
