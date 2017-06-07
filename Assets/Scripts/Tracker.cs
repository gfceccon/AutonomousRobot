using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

public class Tracker : MonoBehaviour
{
    public GPS gps;
    public Vector3 offset;

    void Update()
    {
        transform.position = gps.GetPosition() + offset;
    }
}
