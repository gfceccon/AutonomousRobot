using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Vehicles.Car;

[RequireComponent(typeof(GPS))]
[RequireComponent(typeof(Lasers))]
[RequireComponent(typeof(CarController))]
[RequireComponent(typeof(CarUserControl))]
public class BUG : MonoBehaviour
{
    public enum Direction
    {
        Left,
        Right,
        Closest
    }
    // Control booleans
    [HideInInspector]
    public bool win;
    private bool stop;

    // References
    private GPS gps;
    private Lasers lasers;
    private CarController car;
    private CarUserControl userControl;

    [Header("Configurations")]
    [Tooltip("Destination GPS")]
    public GPS destination;
    [Tooltip("Turn direction")]
    public Direction turn;
    [Tooltip("Number of slices")]
    [Range(3, Lasers.LASER_COUNT)]
    public int slices;
    [Tooltip("Wall distance along normal")]
    public float wallDistance;
    [Tooltip("Wall threshold along normal")]
    public float wallThreshold;

    // Car position, direction and movement
    private Vector3 position;
    private Vector3 direction;
    float accel;
    float steering;
    float handbreak;

    private int _slices;
    private Vector3[] normals;

    enum BUGState
    {
        Line,
        Wall,
    }

    private BUGState state;

    void Start()
    {
        // Get components
        gps = GetComponent<GPS>();
        lasers = GetComponent<Lasers>();
        car = GetComponent<CarController>();
        userControl = GetComponent<CarUserControl>();

        // Init state
        accel = 1f;
        steering = 0f;
        handbreak = 0f;
        state = BUGState.Line;
        lasers.OnComplete(UpdateVectors);
    }

    void Update()
    {
        if (_slices != slices)
            normals = new Vector3[slices];
        _slices = slices;

        // Update values
        position = gps.GetPosition();

        switch (state)
        {
            case BUGState.Line:
                FollowPath();
                break;
            case BUGState.Wall:
            default:
                FollowWall();
                break;
        }
    }

    void FixedUpdate()
    {
        if (stop)
            return;
        if (win)
        {
            userControl.enabled = false;
            car.Move(1f, 0f, 0f, 1f);
            return;
        }

        steering = Vector3.Cross(transform.forward, direction.normalized).y;
        if (!userControl.Using || stop)
            car.Move(steering, accel, accel, handbreak);
    }

    void UpdateVectors(Vector3[] vectors, float?[] collisions)
    {
        int size = Lasers.LASER_COUNT / slices;
        int hitCount = 0;
        for (int i = 0; i < slices; i++)
        {
            int index = size * i;
            Vector3 sum;
            for (int ls = index + 1; ls < index + size; ls++)
            {
                if (ls >= Lasers.LASER_COUNT)
                    break;

            }
        }
    }

    bool OnPath()
    {
        bool line = false;
        return line;
    }

    Vector3 ClosestVector(Vector3[] vectors, Vector3 dir)
    {
        Vector3 closest = vectors[0];

        float cross = Vector3.Cross(dir.normalized, transform.forward).y;
        float dot = Vector3.Dot(dir.normalized, transform.forward);

        if (dot < 0f)
        {
            if (cross < 0f)
                closest = vectors[0];
            else if (cross > 0f)
                closest = vectors[Lasers.LASER_COUNT - 1];
        }
        else if (dot > 0f)
        {
            int index = Mathf.FloorToInt(cross * Lasers.LASER_COUNT / 2);
            index += Lasers.LASER_COUNT / 2 - 1;
            index = Mathf.Clamp(index, 0, Lasers.LASER_COUNT - 1);
            closest = vectors[index];
        }
        else
        {
            switch (turn)
            {

                case Direction.Left:
                default:
                    closest = vectors[0];
                    break;
                case Direction.Right:
                    closest = vectors[Lasers.LASER_COUNT - 1];
                    break;
            }
        }

        return closest;
    }

    void FollowPath()
    {

    }

    void FollowWall()
    {

    }
}
