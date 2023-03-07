using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PatientTest.Scripts
{
    public class GridLogic : MonoBehaviour
    {
        public enum EyeEnum
        {
            Left,
            Right
        }

        public enum TestEnum
        {
            ThirtyDashTwo,
            TwentyDashTwo
        }
        public List<Vector4> eyeResults;
        public GameObject sphere;
        public GameObject pointPrefab;
        public GameObject pointsFolder;
        public float delayTime;
        public float brightness;
        private Material _sphereMaterial;

        public List<Vector4> points = new();

        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        private Renderer _renderer;

        // Start is called before the first frame update
        private void Start()
        {
            _renderer = sphere.GetComponent<Renderer>();
            StartCoroutine(StartTest(EyeEnum.Left, TestEnum.TwentyDashTwo));
        }

        public IEnumerator StartTest(EyeEnum eye, TestEnum test)
        {
            _sphereMaterial = _renderer.material;
            eyeResults = new List<Vector4>();
            CreateGridCoordinates(eye, test);
            CreateVisualGrid();
            while (points.Count > 0)
            {
                var responded = false;
                int randomIndex = Random.Range(0, points.Count);
                Vector4 randomPoint = points[randomIndex];
                var position = new Vector3(randomPoint.x,randomPoint.y,randomPoint.z);
                float opacity = randomPoint.w;
                sphere.transform.localPosition = position;
                SetSphereOpacity(opacity);

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
            yield return eyeResults;
        }
        
        private void HandleInput(int index)
        {
            eyeResults.Add(points[index]);
            points.RemoveAt(index);
        }

        private void SetSphereOpacity(float opacity)
        {
            Color color = Color.HSVToRGB(0,0,opacity);
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
            
                var position = new Vector3(point.x, point.y, point.z);
                GameObject prefab = Instantiate(pointPrefab, position, Quaternion.identity);
                prefab.transform.parent = pointsFolder.transform;
                prefab.transform.localPosition = position;
            }
            pointsFolder.SetActive(false);
        }

    }
}
