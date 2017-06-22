using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

public class WinUI : MonoBehaviour
{
    public Robot robot;
    public GameObject winScreen;
    public GameObject controller;

    void Update()
    {
        controller.SetActive(true);
        if (robot.win)
            winScreen.SetActive(true);
    }
}
