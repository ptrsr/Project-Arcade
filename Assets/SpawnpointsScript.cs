using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnpointsScript : MonoBehaviour
{
    [SerializeField]
    public GameObject[] points;

	void Awake ()
    {
        points = new GameObject[transform.childCount];
        for (int i = 0; i < points.Length ; i++)
        {
            points[i] = transform.GetChild(i).gameObject;
        }
	}
}
