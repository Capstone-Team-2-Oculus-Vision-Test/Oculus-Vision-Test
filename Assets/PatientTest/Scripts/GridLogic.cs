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
        public float delayTime;
        public float brightness;
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
                Vector4 randomPoint = points[randomIndex];
                var position = new Vector3(randomPoint.x, randomPoint.y, randomPoint.z);
                float opacity = randomPoint.w;
                sphere.transform.localPosition = position;
                practitionerLight.transform.localPosition = position;
                SetSphereOpacity(opacity);
                sphere.SetActive(true);
                yield return new WaitForSeconds(0.2f);
                sphere.SetActive(false);
                var time = 0f;
                while (time < delayTime)
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
                points[randomIndex] = randomPoint;
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
            const int maxHorizontal = 27;
            int maxVertical = (test == TestEnum.TwentyDashTwo ? 21 : 27);

            for (var xAxis = 3; xAxis <= maxHorizontal; xAxis += 6)
            {
                if (xAxis > 9)
                {
                    maxVertical -= 6;
                }

                for (var yAxis = 3; yAxis <= maxVertical; yAxis += 6)
                {
                    var quadrantOne = new Vector4(xAxis, yAxis, 10, brightness);
                    var quadrantTwo = new Vector4(-xAxis, yAxis, 10, brightness);
                    var quadrantThree = new Vector4(-xAxis, -yAxis, 10, brightness);
                    var quadrantFour = new Vector4(xAxis, -yAxis, 10, brightness);
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
                }
            }
        }

        private void CreateVisualGrid()
        {
            foreach (Vector4 point in points)
            {

                var position = new Vector3(point.x, point.y, point.z + 1);
                GameObject prefab = Instantiate(pointPrefab, position, Quaternion.identity);
                prefab.transform.parent = pointsFolder.transform;
                prefab.layer = pointsFolder.layer;
                prefab.transform.localPosition = position;
            }
        }

    }
}
