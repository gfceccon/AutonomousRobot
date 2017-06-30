using UnityEngine;
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
        public Slider minDistance, maxDistance, damping;
        public Toggle x, y, z;
    }

    [System.Serializable]
    public struct ViewConfig
    {
        public Toggle renderRays;
        public Toggle topView;
        public Slider distance, height, topViewSize;
    }

    [System.Serializable]
    public struct BUGConfig
    {
        public Slider lineThreshold, lineFollowDistance;
        public Toggle wallSide;
        public Slider wallThreshold, wallDistance;
        public Slider laserSlices, laserPercentage;
    }

    public BUGConfig bugConfig;
    public ViewConfig viewConfig;
    public GPSConfig gpsCarConfig;
    public GPSConfig gpsDestinationConfig;

    public GPS carGPS;
    public GPS destinationGPS;
    public BUG bug;


    public Lasers lasers;
    public CameraFollow perspective;
    public CameraFollow ortho;

    private bool init = true;

    void Start()
    {
        init = true;

        gpsCarConfig.x.isOn = carGPS.constraint.x == 1f ? true : false;
        gpsCarConfig.y.isOn = carGPS.constraint.y == 1f ? true : false;
        gpsCarConfig.z.isOn = carGPS.constraint.z == 1f ? true : false;

        gpsCarConfig.damping.value = carGPS.damping;
        gpsCarConfig.minDistance.value = carGPS.distance.min;
        gpsCarConfig.maxDistance.value = carGPS.distance.max;

        gpsDestinationConfig.x.isOn = destinationGPS.constraint.x == 1f ? true : false;
        gpsDestinationConfig.y.isOn = destinationGPS.constraint.y == 1f ? true : false;
        gpsDestinationConfig.z.isOn = destinationGPS.constraint.z == 1f ? true : false;

        gpsDestinationConfig.damping.value = destinationGPS.damping;
        gpsDestinationConfig.minDistance.value = destinationGPS.distance.min;
        gpsDestinationConfig.maxDistance.value = destinationGPS.distance.max;

        viewConfig.distance.value = perspective.distance;
        viewConfig.height.value = perspective.height;
        viewConfig.renderRays.isOn = lasers.renderRays;

        Camera orthoCamera = ortho.GetComponent<Camera>();
        Camera perspectiveCamera = perspective.GetComponent<Camera>();
        Rect orthoViewport = orthoCamera.rect;
        Rect perspectiveViewport = perspectiveCamera.rect;

        viewConfig.topView.isOn = orthoViewport.Contains(perspectiveViewport.min);
        viewConfig.topViewSize.value = ortho.orthoSize;

        bugConfig.lineThreshold.value = bug.lineThreshold;
        bugConfig.lineFollowDistance.value = bug.lineFollowDamp;
        bugConfig.wallSide.isOn = bug.wallSide == BUG.Direction.Left ? true : false;
        bugConfig.wallThreshold.value = bug.wallThreshold;
        bugConfig.wallDistance.value = bug.wallDistance;
        bugConfig.laserSlices.value = bug.slices;
        bugConfig.laserPercentage.value = bug.wallPercentage;

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

    public void BUGChange()
    {
        if (init)
            return;
        bug.lineThreshold = bugConfig.lineThreshold.value;
        bug.lineFollowDamp = bugConfig.lineFollowDistance.value;
        bug.wallSide = bugConfig.wallSide.isOn ? BUG.Direction.Left : BUG.Direction.Right;
        bug.wallThreshold = bugConfig.wallThreshold.value;
        bug.wallDistance = bugConfig.wallDistance.value;
        bug.slices = (int)bugConfig.laserSlices.value;
        bug.wallPercentage = bugConfig.laserPercentage.value;
    }

    public void ToggleConstraint()
    {
        if (init)
            return;
        Vector3 gpsConstraint;

        gpsConstraint.x = gpsCarConfig.x.isOn ? 1f : 0f;
        gpsConstraint.y = gpsCarConfig.y.isOn ? 1f : 0f;
        gpsConstraint.z = gpsCarConfig.z.isOn ? 1f : 0f;

        carGPS.constraint = gpsConstraint;

        gpsConstraint.x = gpsDestinationConfig.x.isOn ? 1f : 0f;
        gpsConstraint.y = gpsDestinationConfig.y.isOn ? 1f : 0f;
        gpsConstraint.z = gpsDestinationConfig.z.isOn ? 1f : 0f;

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
        Camera orthoCamera = ortho.GetComponent<Camera>();
        Camera perspectiveCamera = perspective.GetComponent<Camera>();
        Rect orthoViewport = orthoCamera.rect;
        Rect perspectiveViewport = perspectiveCamera.rect;
        float orthoDepth = orthoCamera.depth;
        float perspectiveDepth = perspectiveCamera.depth;
        orthoCamera.rect = perspectiveViewport;
        perspectiveCamera.rect = orthoViewport;
        orthoCamera.depth = perspectiveDepth;
        perspectiveCamera.depth = orthoDepth;
    }


    public void OnCarGPSSlide()
    {
        if (init)
            return;
        float min = gpsCarConfig.minDistance.value;
        float max = gpsCarConfig.maxDistance.value;
        max = Mathf.Max(min, max);
        gpsCarConfig.maxDistance.value = max;
        float damping = gpsCarConfig.damping.value;

        carGPS.distance = new GPS.MinMax(min, max);
        carGPS.damping = damping;
    }

    public void OnDestinationGPSSlide()
    {
        if (init)
            return;
        float min = gpsDestinationConfig.minDistance.value;
        float max = gpsDestinationConfig.maxDistance.value;
        max = Mathf.Max(min, max);
        gpsDestinationConfig.maxDistance.value = max;
        float damping = gpsDestinationConfig.damping.value;

        destinationGPS.distance = new GPS.MinMax(min, max);
        destinationGPS.damping = damping;
    }

    public void OnCameraSlide()
    {
        if (init)
            return;
        ortho.orthoSize = viewConfig.topViewSize.value;
        perspective.height = viewConfig.height.value;
        perspective.distance = viewConfig.distance.value;
    }

    public void OnTopView()
    {
        if (init)
            return;
        ortho.gameObject.SetActive(viewConfig.topView.isOn);
    }
}
