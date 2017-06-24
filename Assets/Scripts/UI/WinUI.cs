using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

public class WinUI : MonoBehaviour
{
    public GameObject winScreen;
    public GameObject controller;

    private SmartCar car;

    private void Start()
    {
        car = GameObject.Find("Car").GetComponent<SmartCar>();
    }

    void Update()
    {
        if (car.win)
        {
            winScreen.SetActive(true);
            controller.SetActive(false);
        }
        else
            controller.SetActive(true);
    }
}
