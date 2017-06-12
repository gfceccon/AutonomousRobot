using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityStandardAssets.Vehicles.Car;

public class Robot : MonoBehaviour
{
    [HideInInspector]
    public bool win;
    public GPS gps;
    public GPS destination;

    private Lasers lasers;
    private CarController car;
    private CarUserControl userControl;

    void Start()
    {
        win = false;
        lasers = GetComponent<Lasers>();
        car = GetComponent<CarController>();
        userControl = GetComponent<CarUserControl>();
    }

    void Update()
    {
        if (win)
        {
            userControl.enabled = false;
            car.Move(1f, 0f, 0f, 1f);
            return;
        }
        float steering = Vector3.Cross(transform.forward, (destination.GetPosition() - gps.GetPosition()).normalized).y;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destination"))
            win = true;
    }
}
