using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Vehicles.Car;

public class TurnWall : MonoBehaviour
{

    // References
    public GPS gps;
    public GPS destination;
    public Lasers lasers;
    public CarController car;
    public CarUserControl userControl;

    private float?[] collisions;
    private Vector3[] vectors;

    float steering;
    float accel;
    bool back;
    Vector3 direction;

    void Start()
    {
        // Get components
        gps = GetComponent<GPS>();
        lasers = GetComponent<Lasers>();
        car = GetComponent<CarController>();
        userControl = GetComponent<CarUserControl>();
        direction = transform.forward;
        accel = 1f;
        back = false;
    }

    void Update()
    {
        vectors = lasers.Vectors;
        collisions = lasers.Collisions;
        direction = destination.GetPosition() - gps.GetPosition();
        if (collisions[Lasers.LASER_COUNT/2].HasValue)
        {
            if (collisions[Lasers.LASER_COUNT / 2].Value < 4)
                direction = -transform.right;
        }
        
    }

    public void Collide()
    {
        StartCoroutine(Back());
    }

    IEnumerator Back()
    {
        accel = -1f;
        steering = 0f;
        back = true;
        yield return new WaitForSeconds(2f);
        back = false;
        accel = 1f;
    }

    void FixedUpdate()
    {

        if(!back)
            steering = Vector3.Cross(transform.forward, direction.normalized).y;
        if (!userControl.Using)
            car.Move(steering, accel, accel, 0f);
    }
}
