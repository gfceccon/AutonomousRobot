using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

public class SmartCar : MonoBehaviour
{
    /// <summary>
    /// Control booleans
    /// </summary>
    [HideInInspector]
    public bool win;
    private bool stop;

    [Tooltip("Car GPS")]
    public GPS gps;
    [Tooltip("Destination GPS")]
    public GPS destination;

    [Tooltip("Wall threshold along normal")]
    public float wallThreshold;
    [Tooltip("Corner threshold based on derivative")]
    public float cornerThreshold;

    [Tooltip("Wall vertice indicator")]
    public GameObject wallObj;
    [Tooltip("Open corner indicator")]
    public GameObject openCornerObj;
    [Tooltip("Closed corner vertice indicator")]
    public GameObject closedCornerObj;

    /// <summary>
    /// Lasers information
    /// </summary>
    private float?[] collisions;
    private Vector3[] vectors;

    ///
    private Vector3 direction;

    void Start ()
	{
	}
	
	void Update ()
	{
	}
}
