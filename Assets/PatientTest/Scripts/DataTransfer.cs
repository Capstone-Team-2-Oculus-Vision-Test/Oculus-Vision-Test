using System.Collections.Generic;
using UnityEngine;
using static Utility.UtilityScripts;

namespace PatientTest.Scripts
{
    public static class DataTransfer
    {
        public static TestResultsDTO resultsDTO = new();
        public static PatientDTO patientDTO = new();
    }

    public class TestResultsDTO
    {
        public List<Vector4> Data;
        public EyeEnum EyeTested; //Left or right eye
        public TestEnum Test;
        public System.DateTime StartTime;
        public System.DateTime EndTime;
        public float Duration;
        public string Algorithm = "Specvis";
        public float StimulusSize;
        public Color StimulusColor;
        public int NumPoints;
        public float ErrorRate;
    }

    public class PatientDTO
    {
        //Patient info (may not be necessary, could just put placeholders)
        public string ID;
        public string FirstName;
        public string LastName;
        public string Age;
        public string Sex;
    }
}
