using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLogic : MonoBehaviour
{
    private static System.Random rng = new System.Random();
    public GameObject sphere;
    public GameObject pointPrefab;
    public GameObject pointsFolder;
    public float DelayTime;
    public float brightness;

    public List<Vector3> Shuffle(List<Vector3> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Vector3 value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

    public List<Vector4> points = new List<Vector4>();
    // Start is called before the first frame update
    void Start()
    {
        CreateGridCoordinates();
        CreateVisualGrid();
        StartCoroutine(MoveLightFlash());
    }

    IEnumerator MoveLightFlash()
    {
        while (true)
        {
            int randomIndex = Random.Range(0, points.Count);
            Vector4 randomPoint = points[randomIndex];
            Vector3 position = new Vector3(randomPoint.x,randomPoint.y,randomPoint.z);
            float opacity = randomPoint.w;
            sphere.transform.position = position;
            SetSphereOpacity(sphere, opacity);

            randomPoint.w -= 30f;
            if (randomPoint.w <= 0f)
            {
                points.RemoveAt(randomIndex);
            }
            else
            {
                points[randomIndex] = randomPoint;
            }
            yield return new WaitForSeconds(DelayTime);
        }
        
    }

    private void SetSphereOpacity(GameObject sphere, float opacity)
    {
        Material material = sphere.GetComponent<Renderer>().material;
        Color color = Color.HSVToRGB(0,0,opacity);
        material.SetColor("_EmissionColor", color * 0.01f);
    }

    void CreateGridCoordinates()
    {
        int max = 27;
        for (int y = 3; y <= 27; y += 6)
        {
            if (y > 9)
            {
                max -= 6;
            }

            for (int x = -max; x <= max; x += 6)
            {
                Vector4 vec = new Vector4(x, y, 10, brightness);
                Vector4 vec2 = new Vector4(x, -y, 10, brightness);
                points.Add(vec);
                points.Add(vec2);
            }
        }
    }

    private void CreateVisualGrid()
    {
        foreach (Vector4 point in points)
        {
            Vector3 position = new Vector3(point.x, point.y, point.z);
            GameObject prefab = Instantiate(pointPrefab, position, Quaternion.identity);
            prefab.transform.parent = pointsFolder.transform;
        }
        pointsFolder.SetActive(false);
    }

}
