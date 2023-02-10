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

    List<Vector3> points = new List<Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        //row 1
        points.Add(new Vector3(-9, 27, 10));
        points.Add(new Vector3(-3, 27, 10));
        points.Add(new Vector3(3, 27, 10));
        points.Add(new Vector3(9, 27, 10));
        //row 2
        points.Add(new Vector3(-15, 21, 10));
        points.Add(new Vector3(-9, 21, 10));
        points.Add(new Vector3(-3, 21, 10));
        points.Add(new Vector3(3, 21, 10));
        points.Add(new Vector3(9, 21, 10));
        points.Add(new Vector3(15, 21, 10));
        //row 3
        points.Add(new Vector3(-21, 15, 10));
        points.Add(new Vector3(-15, 15, 10));
        points.Add(new Vector3(-9, 15, 10));
        points.Add(new Vector3(-3, 15, 10));
        points.Add(new Vector3(3, 15, 10));
        points.Add(new Vector3(9, 15, 10));
        points.Add(new Vector3(15, 15, 10));
        points.Add(new Vector3(21, 15, 10));
        //row 4
        points.Add(new Vector3(-27, 9, 10));
        points.Add(new Vector3(-21, 9, 10));
        points.Add(new Vector3(-15, 9, 10));
        points.Add(new Vector3(-9, 9, 10));
        points.Add(new Vector3(-3, 9, 10));
        points.Add(new Vector3(3, 9, 10));
        points.Add(new Vector3(9, 9, 10));
        points.Add(new Vector3(15, 9, 10));
        points.Add(new Vector3(21, 9, 10));
        points.Add(new Vector3(27, 9, 10));
        //row 5
        points.Add(new Vector3(-27, 3, 10));
        points.Add(new Vector3(-21, 3, 10));
        points.Add(new Vector3(-15, 3, 10));
        points.Add(new Vector3(-9, 3, 10));
        points.Add(new Vector3(-3, 3, 10));
        points.Add(new Vector3(3, 3, 10));
        points.Add(new Vector3(9, 3, 10));
        points.Add(new Vector3(15, 3, 10));
        points.Add(new Vector3(21, 3, 10));
        points.Add(new Vector3(27, 3, 10));
        //negative
        //row 1
        points.Add(new Vector3(-9, -27, 10));
        points.Add(new Vector3(-3, -27, 10));
        points.Add(new Vector3(3, -27, 10));
        points.Add(new Vector3(9, -27, 10));
        //row 2
        points.Add(new Vector3(-15, -21, 10));
        points.Add(new Vector3(-9, -21, 10));
        points.Add(new Vector3(-3, -21, 10));
        points.Add(new Vector3(3, -21, 10));
        points.Add(new Vector3(9, -21, 10));
        points.Add(new Vector3(15, -21, 10));
        //row 3
        points.Add(new Vector3(-21, -15, 10));
        points.Add(new Vector3(-15, -15, 10));
        points.Add(new Vector3(-9, -15, 10));
        points.Add(new Vector3(-3, -15, 10));
        points.Add(new Vector3(3, -15, 10));
        points.Add(new Vector3(9, -15, 10));
        points.Add(new Vector3(15, -15, 10));
        points.Add(new Vector3(21, -15, 10));
        //row 4
        points.Add(new Vector3(-27, -9, 10));
        points.Add(new Vector3(-21, -9, 10));
        points.Add(new Vector3(-15, -9, 10));
        points.Add(new Vector3(-9, -9, 10));
        points.Add(new Vector3(-3, -9, 10));
        points.Add(new Vector3(3, -9, 10));
        points.Add(new Vector3(9, -9, 10));
        points.Add(new Vector3(15, -9, 10));
        points.Add(new Vector3(21, -9, 10));
        points.Add(new Vector3(27, -9, 10));
        //row 5
        points.Add(new Vector3(-27, -3, 10));
        points.Add(new Vector3(-21, -3, 10));
        points.Add(new Vector3(-15, -3, 10));
        points.Add(new Vector3(-9, -3, 10));
        points.Add(new Vector3(-3, -3, 10));
        points.Add(new Vector3(3, -3, 10));
        points.Add(new Vector3(9, -3, 10));
        points.Add(new Vector3(15, -3, 10));
        points.Add(new Vector3(21, -3, 10));
        points.Add(new Vector3(27, -3, 10));

        points = Shuffle(points);
        foreach(Vector3 point in points)
        {
            GameObject prefab = Instantiate(pointPrefab, point, Quaternion.identity);
            prefab.transform.parent = pointsFolder.transform;
        }
        pointsFolder.SetActive(false);
        
        StartCoroutine(nextPos());
    }

    IEnumerator nextPos()
    {
        int pointcount = points.Count;
        int i = 0;
        while (true)
        {
            if (i < pointcount)
            {
                // Debug.Log("move");
                sphere.transform.position = points[i];
                i++;
                yield return new WaitForSeconds(DelayTime);
            }
            else
            {
                points = Shuffle(points);
                i = 0;
            }
        }
    }


}
