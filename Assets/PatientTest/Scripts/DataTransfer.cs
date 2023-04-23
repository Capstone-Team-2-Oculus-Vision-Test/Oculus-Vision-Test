using System;
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
        public DateTime StartTime;
        public DateTime EndTime;
        public float Duration;
        public string Algorithm = "Specvis";
        public float StimulusSize;
        public Color StimulusColor;
        public int NumPoints;
        public float ErrorRate;
        public float ScalingFactor;
    }

    public class PatientDTO
    {
        //Patient info
        public string ID;
        public string FirstName;
        public string LastName;
        public string Age;
        public string Sex;

        public bool MissingData()
        {
            // if any fields empty, return true
            return ID == "" || FirstName == "" || LastName == "" || Age == "" || Sex == "";
        }
    }

    public class TestListData
    {
        public TestResultsDTO Results;
        public string Name;
        public string Date;
        public int ID;
    }
}
