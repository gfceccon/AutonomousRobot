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

    private Vector3[] means = new Vector3[Lasers.LASER_COUNT];
    private List<Vector3> clears = new List<Vector3>();
    private List<Vector3> walls = new List<Vector3>();
    private List<GameObject> indicators = new List<GameObject>();

    void Start()
    {
        win = false;
        lasers = GetComponent<Lasers>();
        car = GetComponent<CarController>();
        userControl = GetComponent<CarUserControl>();
    }

    void FixedUpdate()
    {
        if (win)
        {
            userControl.enabled = false;
            car.Move(1f, 0f, 0f, 1f);
            return;
        }
        float steering = 0f;
        float accel = 1f;
        float handbreak = 0f;
        Vector3 direction = destination.GetPosition() - gps.GetPosition();

        steering = Vector3.Cross(transform.forward, direction.normalized).y;
        if(!userControl.Using)
            car.Move(steering, accel, accel, handbreak);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destination"))
            win = true;
    }
}
