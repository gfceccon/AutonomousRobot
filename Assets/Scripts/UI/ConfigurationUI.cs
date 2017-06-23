﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

public class ConfigurationUI : MonoBehaviour
{
    private int tab;
    public GameObject[] alternate;

    [System.Serializable]
    public struct GPSConfig
    {
        public Toggle x, y, z;
        public Slider damping, minDistance, maxDistance;
    }

    [System.Serializable]
    public struct ViewConfig
    {
        public Toggle renderRays;
        public Toggle topView;
        public Slider distance, height, topViewSize;
    }

    public GPSConfig carConfig;
    public GPSConfig destinationConfig;

    public GPS carGPS;
    public GPS destinationGPS;

    public ViewConfig viewConfig;

    public Lasers lasers;
    public CameraFollow mainCamera;
    public CameraFollow minimap;

    private bool init = true;

    void Start()
    {
        init = true;

        carConfig.x.isOn = carGPS.constraint.x == 1f ? true : false;
        carConfig.y.isOn = carGPS.constraint.y == 1f ? true : false;
        carConfig.z.isOn = carGPS.constraint.z == 1f ? true : false;

        carConfig.damping.value = carGPS.damping;
        carConfig.minDistance.value = carGPS.distance.min;
        carConfig.maxDistance.value = carGPS.distance.max;

        destinationConfig.x.isOn = destinationGPS.constraint.x == 1f ? true : false;
        destinationConfig.y.isOn = destinationGPS.constraint.y == 1f ? true : false;
        destinationConfig.z.isOn = destinationGPS.constraint.z == 1f ? true : false;

        destinationConfig.damping.value = destinationGPS.damping;
        destinationConfig.minDistance.value = destinationGPS.distance.min;
        destinationConfig.maxDistance.value = destinationGPS.distance.max;

        viewConfig.distance.value = mainCamera.distance;
        viewConfig.height.value = mainCamera.height;
        viewConfig.renderRays.isOn = lasers.renderRays;
        viewConfig.topView.isOn = minimap.enabled;
        viewConfig.topViewSize.value = minimap.orthoSize;

        tab = 0;
        for (int i = 0; i < alternate.Length; i++)
            alternate[i].SetActive(false);
        alternate[tab].SetActive(true);

        init = false;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            tab = (tab + 1) % alternate.Length;
            for (int i = 0; i < alternate.Length; i++)
                alternate[i].SetActive(false);
            alternate[tab].SetActive(true);
        }
    }

    public void ToggleConstraint()
    {
        if (init)
            return;
        Vector3 gpsConstraint;

        gpsConstraint.x = carConfig.x.isOn ? 1f : 0f;
        gpsConstraint.y = carConfig.y.isOn ? 1f : 0f;
        gpsConstraint.z = carConfig.z.isOn ? 1f : 0f;

        carGPS.constraint = gpsConstraint;

        gpsConstraint.x = destinationConfig.x.isOn ? 1f : 0f;
        gpsConstraint.y = destinationConfig.y.isOn ? 1f : 0f;
        gpsConstraint.z = destinationConfig.z.isOn ? 1f : 0f;

        destinationGPS.constraint = gpsConstraint;
    }

    public void ToggleRays()
    {
        if (init)
            return;
        lasers.renderRays = viewConfig.renderRays.isOn;
    }

    public void ToggleOrtho()
    {
        if (init)
            return;
        minimap.gameObject.SetActive(viewConfig.topView.isOn);
    }


    public void OnCarGPSSlide()
    {
        if (init)
            return;
        float min = carConfig.minDistance.value;
        float max = carConfig.maxDistance.value;
        max = Mathf.Max(min, max);
        carConfig.maxDistance.value = max;
        float damping = carConfig.damping.value;

        carGPS.distance = new GPS.MinMax(min, max);
        carGPS.damping = damping;
    }

    public void OnDestinationGPSSlide()
    {
        if (init)
            return;
        float min = destinationConfig.minDistance.value;
        float max = destinationConfig.maxDistance.value;
        max = Mathf.Max(min, max);
        destinationConfig.maxDistance.value = max;
        float damping = destinationConfig.damping.value;

        destinationGPS.distance = new GPS.MinMax(min, max);
        destinationGPS.damping = damping;
    }

    public void OnCameraSlide()
    {
        if (init)
            return;
        minimap.orthoSize = viewConfig.topViewSize.value;
        mainCamera.height = viewConfig.height.value;
        mainCamera.distance = viewConfig.distance.value;
    }

    public void OnTopView()
    {
        if (init)
            return;
        minimap.gameObject.SetActive(viewConfig.topView.isOn);
    }
}
