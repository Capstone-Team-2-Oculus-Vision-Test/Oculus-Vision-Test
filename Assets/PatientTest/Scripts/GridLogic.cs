using System;
using System.Collections;
using System.Collections.Generic;
using PractitionerMenu.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Utility;
using static Utility.UtilityScripts;
using static PatientTest.Scripts.DataTransfer;
using Random = UnityEngine.Random;

namespace PatientTest.Scripts
{
    public class GridLogic : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textPrefab;
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
        public List<Vector4> eyeResults;
        private Material _sphereMaterial;
        private IEnumerator _testCoroutine;
        private bool _pause;
        private bool _allowInput;
        private bool _responded;
        private int _randomIndex;
        private (EyeEnum eye, TestEnum test) _testParameters;


        public List<Vector4> points = new();

        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        private Renderer _renderer;

        // Start is called before the first frame update
        private void Start()
        {
            // Get settings from PractitionerUI
            minInputDelay = PlayerPrefs.GetFloat("MinInputDelay", 1f);
            maxInputDelay = PlayerPrefs.GetFloat("MaxInputDelay", 2.3f); 
            brightness = PlayerPrefs.GetFloat("MinBrightness", 20f);
            maxBrightness = PlayerPrefs.GetFloat("MaxBrightness", 100f);
            sphereDelay = PlayerPrefs.GetFloat("StimulusTiming", 0.5f);
            
            // Set up test
            _renderer = sphere.GetComponent<Renderer>();
            sphere.transform.localScale = new Vector3(pointFOV / 1.2f, pointFOV / 1.2f, pointFOV / 1.2f);
            practitionerLight.transform.localScale = new Vector3(pointFOV / 1.2f, pointFOV / 1.2f, pointFOV / 1.2f);
            _testCoroutine = StartTest(resultsDTO.EyeTested, resultsDTO.Test);
            StartCoroutine(_testCoroutine);
        }

        private void FixedUpdate()
        {
            if (!_allowInput || !CheckInput() || _responded) return;
            HandleInput(_randomIndex);
            _responded = true;
        }

        private static bool CheckInput()
        {
            return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyUp(KeyCode.Space) || Input.GetKey(KeyCode.Space) 
                   || OVRInput.GetDown(OVRInput.Button.One);
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
            sphere.transform.localPosition = Vector3.zero;
            sphere.SetActive(false);
            practitionerLight.transform.localPosition = Vector3.zero;
            eyeResults.Clear();
            points.Clear();
            // Get the parent object's Transform component
            Transform parentTransform = pointsFolder.transform;

            // Iterate over all the child objects and destroy them
            for (var i = 0; i < parentTransform.childCount; i++)
            {
                GameObject childObject = parentTransform.GetChild(i).gameObject;
                GameObject.Destroy(childObject);
            }
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
            if (OVRInput.GetDown(OVRInput.Button.Any))
                return;
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

        private IEnumerator StartTest(EyeEnum eye, TestEnum test)
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
            _pause = true;
            _sphereMaterial = _renderer.material;
            eyeResults = new List<Vector4>();
            CreateGridCoordinates(eye, test);
            CreateVisualGrid();

            results.StartTime = DateTime.Now;
            while (points.Count > 0)
            {
                _responded = false;
                yield return new WaitUntil(() => !_pause);
                _randomIndex = Random.Range(0, points.Count);
                float delayTime = Random.Range(minInputDelay, maxInputDelay);
                Vector4 randomPoint = points[_randomIndex];
                var position = new Vector3(randomPoint.x, randomPoint.y, randomPoint.z);
                float opacity = randomPoint.w;
                sphere.transform.localPosition = position;
                practitionerLight.transform.localPosition = position;
                SetSphereOpacity(opacity);
                _allowInput = true;
                sphere.SetActive(true);
                yield return new WaitForSeconds(sphereDelay);
                sphere.SetActive(false);
                yield return new WaitForSeconds(delayTime);
                _allowInput = false;
                if (_responded) continue;
                randomPoint.w += 10f;
                if (randomPoint.w <= maxBrightness)
                {
                    points[_randomIndex] = randomPoint;
                }
                else
                {
                    HandleInput(_randomIndex);
                    
                }
            }
            results.EndTime = DateTime.Now;

            results.Duration = results.EndTime.Subtract(results.StartTime).Seconds;

            results.Data = eyeResults;
            results.NumPoints = eyeResults.Count;
            MainMenu.SetShowResults(true);
            SceneManager.LoadScene("PractitionerMenu");
        }

        private void HandleInput(int index)
        {
            Vector4 point = points[index];
            point.w /= 2;
            eyeResults.Add(point);
            points.RemoveAt(index);

            // replace practitioner sphere with text
            Transform pointTransform = pointsFolder.transform.Find($"{point.x} {point.y}");
            if (!pointTransform) return;
            GameObject pointObjectReference = pointTransform.gameObject;
            TextMeshPro textObject = Instantiate(textPrefab, pointTransform);
            textObject.transform.SetParent(pointsFolder.transform);
            textObject.text = $"{point.w}";
            Destroy(pointObjectReference);
        }

        private void SetSphereOpacity(float opacity)
        {
            Color color = Color.HSVToRGB(0, 0, opacity);
            _sphereMaterial.SetColor(EmissionColor, color * 0.01f);
        }

        private void CreateGridCoordinates(EyeEnum eye, TestEnum test)
        {
            float scalingFactor = pointDistance / (7.2f);
            resultsDTO.ScalingFactor = scalingFactor;
            const float safetyPad = 0.0001f;
            float maxHorizontal = 27 * scalingFactor;
            float maxVertical = (test == TestEnum.TwentyDashTwo ? 21 * scalingFactor : 27 * scalingFactor);

            for (float xAxis = 3 * scalingFactor; xAxis <= maxHorizontal + safetyPad; xAxis += 6 * scalingFactor)
            {

                if (xAxis > 9 * scalingFactor)
                {
                    maxVertical -= 6 * scalingFactor;
                }

                for (float yAxis = 3 * scalingFactor; yAxis <= maxVertical + safetyPad; yAxis += 6 * scalingFactor)
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
                Vector3 localPosition = prefab.transform.localPosition;
                prefab.name = $"{localPosition.x} {localPosition.y}";
                localPosition = position;
                prefab.transform.localPosition = localPosition;
            }
        }

    }
}
