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
            sphere.transform.localPosition = position;
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
        int maxHorizontal = 27;
        for (int yAxis = 3; yAxis <= 27; yAxis += 6)
        {
            if (yAxis > 9)
            {
                maxHorizontal -= 6;
            }

            for (int xAxis = -maxHorizontal; xAxis <= maxHorizontal; xAxis += 6)
            {
                Vector4 positive = new Vector4(xAxis, yAxis, 10, brightness);
                Vector4 negative = new Vector4(xAxis, -yAxis, 10, brightness);
                points.Add(positive);
                points.Add(negative);
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
            prefab.transform.localPosition = position;
        }
        pointsFolder.SetActive(false);
    }

}
