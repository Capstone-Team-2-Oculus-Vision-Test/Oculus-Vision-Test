using System;
using System.Collections;
using System.Collections.Generic;
using PractitionerMenu.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Utility.UtilityScripts;
using static PatientTest.Scripts.DataTransfer;
using Random = UnityEngine.Random;

namespace PatientTest.Scripts
{
    public class GridLogic : MonoBehaviour
    {
        public List<Vector4> eyeResults;
        public GameObject sphere;
        public GameObject pointPrefab;
        public GameObject pointsFolder;
        public GameObject practitionerLight;
        public Camera leftEyeCamera;
        public Camera rightEyeCamera;
        public float minInputDelay;
        public float maxInputDelay;
        public float minNextDelay;
        public float maxNextDelay;
        public float sphereDelay;
        public float brightness;
        public float pointFOV;
        public float pointDistance;
        public float maxBrightness;
        private Material _sphereMaterial;
        private IEnumerator _testCoroutine;
        private bool _pause;
        private (EyeEnum eye, TestEnum test) _testParameters;

        public List<Vector4> points = new();

        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        private Renderer _renderer;

        // Start is called before the first frame update
        private void Start()
        {
            _renderer = sphere.GetComponent<Renderer>();
            sphere.transform.localScale = new Vector3(pointFOV / 1.2f, pointFOV / 1.2f, pointFOV / 1.2f);
            practitionerLight.transform.localScale = new Vector3(pointFOV / 1.2f, pointFOV / 1.2f, pointFOV / 1.2f);
            //_testCoroutine = StartTest((EyeEnum)PlayerPrefs.GetInt("Eye"), (TestEnum)PlayerPrefs.GetInt("TestType"));
            _testCoroutine = StartTest(resultsDTO.EyeTested, resultsDTO.Test);
            StartCoroutine(_testCoroutine);
        }

        public void StopTest()
        {
            if (OVRInput.GetDown(OVRInput.Button.Any))
                return;
            StopCoroutine(_testCoroutine);
            Debug.Log("Test Stopped.");
        }

        public void ResetTest()
        {
            if (OVRInput.GetDown(OVRInput.Button.Any))
                return;
            StopCoroutine(_testCoroutine);
            eyeResults.Clear();
            points.Clear();
            _testCoroutine = null;
            _testCoroutine = StartTest(_testParameters.Item1, _testParameters.Item2);
            StartCoroutine(_testCoroutine);
        }

        public void PauseTest()
        {
            if (OVRInput.GetDown(OVRInput.Button.Any))
                return;
            _pause = !_pause;
        }

        public void DebugResults()
        {
            
            foreach (Vector4 item in points)
            {
                Vector4 point = item;
                point.w = Random.Range(1, 11) * 10;
                point.w /= 2;
                eyeResults.Add(point);
            }
            resultsDTO.Data = eyeResults;
            MainMenu.SetShowResults(true);
            SceneManager.LoadScene("PractitionerMenu");
        }
        public IEnumerator StartTest(EyeEnum eye, TestEnum test)
        {
            TestResultsDTO results = resultsDTO;

            results.StimulusSize = pointPrefab.transform.localScale[0];
            results.StimulusColor = pointPrefab.GetComponent<Renderer>().sharedMaterial.color;
            
            if (eye == EyeEnum.Right)
            {
                leftEyeCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Default"));

            }
            else
            {
                rightEyeCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Default"));
            }

            _testParameters = (eye, test);
            _pause = false;
            _sphereMaterial = _renderer.material;
            eyeResults = new List<Vector4>();
            CreateGridCoordinates(eye, test);
            CreateVisualGrid();

            results.StartTime = DateTime.Now;
            while (points.Count > 0)
            {
                yield return new WaitUntil(() => !_pause);
                var responded = false;
                int randomIndex = Random.Range(0, points.Count);
                float delayTime = Random.Range(minInputDelay, maxInputDelay);
                Vector4 randomPoint = points[randomIndex];
                var position = new Vector3(randomPoint.x, randomPoint.y, randomPoint.z);
                float opacity = randomPoint.w;
                sphere.transform.localPosition = position;
                practitionerLight.transform.localPosition = position;
                SetSphereOpacity(opacity);
                sphere.SetActive(true);
                yield return new WaitForSeconds(sphereDelay); //I think this needs to be a seperate coroutine as it prohibits grabbing input while sphere is active
                sphere.SetActive(false);
                var time = 0f;
                while (time < delayTime) //currently delay in between light flashes is = to time given to input (I think these should be seperate)?
                {
                    if (OVRInput.GetDown(OVRInput.Button.One))
                    {
                        HandleInput(randomIndex);
                        responded = true;
                    }
                    time += Time.deltaTime;
                    yield return null;
                }

                if (responded) continue;
                randomPoint.w += 10f;
                if (randomPoint.w < maxBrightness)
                {
                    points[randomIndex] = randomPoint;
                }
            }
            results.EndTime = DateTime.Now;

            results.Duration = results.EndTime.Subtract(results.StartTime).Seconds;

            results.Data = eyeResults;
            results.NumPoints = eyeResults.Count;
            SceneManager.LoadScene("PractitionerMenu");
        }

        private void HandleInput(int index)
        {
            Vector4 point = points[index];
            point.w /= 2;
            eyeResults.Add(point);
            points.RemoveAt(index);
        }

        private void SetSphereOpacity(float opacity)
        {
            Color color = Color.HSVToRGB(0, 0, opacity);
            _sphereMaterial.SetColor(EmissionColor, color * 0.01f);
        }

        private void CreateGridCoordinates(EyeEnum eye, TestEnum test)
        {
            float scalingFactor = pointDistance / (7.2f);
            const float safetyPad = 0.0001f;
            float maxHorizontal = 27 * scalingFactor;
            float maxVertical = (test == TestEnum.TwentyDashTwo ? 21 * scalingFactor : 27 * scalingFactor);

            for (var xAxis = 3 * scalingFactor; xAxis <= maxHorizontal + safetyPad; xAxis += 6 * scalingFactor)
            {

                if (xAxis > 9 * scalingFactor)
                {
                    maxVertical -= 6 * scalingFactor;
                }

                for (var yAxis = 3 * scalingFactor; yAxis <= maxVertical + safetyPad; yAxis += 6 * scalingFactor)
                {
                    var quadrantOne = new Vector4(xAxis, yAxis, 10, brightness);
                    var quadrantTwo = new Vector4(-xAxis, yAxis, 10, brightness);
                    var quadrantThree = new Vector4(-xAxis, -yAxis, 10, brightness);
                    var quadrantFour = new Vector4(xAxis, -yAxis, 10, brightness);

                    if ((xAxis <= maxHorizontal + safetyPad && xAxis >= maxHorizontal - safetyPad) && (yAxis <= 3 * scalingFactor + safetyPad && yAxis >= 3 * scalingFactor - safetyPad) && eye == EyeEnum.Right && test == TestEnum.TwentyDashTwo)
                    {
                        points.Add(quadrantTwo);
                        points.Add(quadrantThree);
                    }
                    else if ((xAxis <= maxHorizontal + safetyPad && xAxis >= maxHorizontal - safetyPad) && (yAxis <= 3 * scalingFactor + safetyPad && yAxis >= 3 * scalingFactor - safetyPad) && eye == EyeEnum.Left && test == TestEnum.TwentyDashTwo)
                    {
                        points.Add(quadrantOne);
                        points.Add(quadrantFour);
                    }
                    else
                    {
                        points.Add(quadrantOne);
                        points.Add(quadrantTwo);
                        points.Add(quadrantThree);
                        points.Add(quadrantFour);
                    }
                    /* CHANGED TO IF/ELSE so I can use non-constants for the cases
                    switch (xAxis)
                    {
                        case 27 when yAxis == 3 && eye == EyeEnum.Right && test == TestEnum.TwentyDashTwo:
                            points.Add(quadrantTwo);
                            points.Add(quadrantThree);
                            break;
                        case 27 when yAxis == 3 && eye == EyeEnum.Left && test == TestEnum.TwentyDashTwo:
                            points.Add(quadrantOne);
                            points.Add(quadrantFour);
                            break;
                        default:
                            points.Add(quadrantOne);
                            points.Add(quadrantTwo);
                            points.Add(quadrantThree);
                            points.Add(quadrantFour);
                            break;
                    }
                    */
                }
            }
        }

        private void CreateVisualGrid()
        {
            foreach (Vector4 point in points)
            {
                var position = new Vector3(point.x, point.y, point.z + 1);
                GameObject prefab = Instantiate(pointPrefab, position, Quaternion.identity);
                prefab.transform.localScale = new Vector3(pointFOV / 1.2f, pointFOV / 1.2f, pointFOV / 1.2f);
                prefab.transform.parent = pointsFolder.transform;
                prefab.layer = pointsFolder.layer;
                prefab.transform.localPosition = position;
            }
        }

    }
}
