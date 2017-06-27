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
        Right
    }

    enum BUGState
    {
        Line,
        Wall
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
    [Tooltip("Wall side")]
    public Direction wallSide;
    [Tooltip("Line distance threshold")]
    public float lineThreshold;
    [Tooltip("Wall distance along normal")]
    public float wallDistance;
    [Tooltip("Wall threshold along normal")]
    public float wallThreshold;

    [Header("Lasers Configuration")]
    [Tooltip("Number of slices")]
    [Range(3, Lasers.LASER_COUNT)]
    public int slices;
    [Tooltip("Lasers percentage to follow")]
    [Range(0f, 1f)]
    public float wallPercentage;

    [Header("Prefabs")]
    [Tooltip("Wall vertice indicator")]
    public GameObject wallObj;
    [Tooltip("Render vectors")]
    public bool renderRays;
    [Tooltip("Normal vectors")]
    public Material rayNormalMaterial;
    [Tooltip("Free path vectors")]
    public Material rayFreeMaterial;

    // Car position, direction and movement
    private Vector3 position;
    private Vector3 direction;

    float accel;
    float steering;
    float handbreak;

    private int _slices;
    private Vector3[] normalVectors;
    private Vector3[] freeVectors;
    private bool[] isFree;

    private bool findNewLine;
    private Vector3 linePoint;
    private Vector3 lineVector;

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
        _slices = 0;
        steering = 0f;
        handbreak = 0f;
        findNewLine = false;
        state = BUGState.Line;
        lasers.OnComplete(UpdateVectors);

        win = false;
        stop = false;
        linePoint = gps.GetPosition();
        lineVector = (destination.GetPosition() - gps.GetPosition()).normalized;

        direction = lineVector;
    }

    void Update()
    {
        Vector3 n = Vector3.zero;
        if (normalVectors != null)
            foreach (Vector3 normal in normalVectors)
                if (normal.magnitude > wallThreshold) n += normal;
        n.Normalize();
        Vector3 u = (wallSide == Direction.Left ? Vector3.up : -Vector3.up);
        direction = Vector3.Cross(n, u);
        OnPath();
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

    void FollowPath()
    {

    }

    void FollowWall()
    {

    }

    void UpdateStates()
    {
        position = gps.GetPosition();
        if (_slices != slices)
        {
            normalVectors = new Vector3[slices];
            freeVectors = new Vector3[slices];
            isFree = new bool[slices];
            _slices = slices;
        }
    }

    Vector3 CollisionPos(Vector3[] vectors, float?[] collisions, int index) { return position + vectors[index] * collisions[index].Value + lasers.offset; }

    void UpdateVectors(Vector3[] vectors, float?[] collisions)
    {
        UpdateStates();
        int size = Lasers.LASER_COUNT / slices;
        for (int i = 0; i < slices; i++)
        {
            int index = size * i;
            Vector3 free = Vector3.zero;
            Vector3 normal = Vector3.zero;

            int counter = 0;
            int freeCounter = 0;

            for (int ls = index + 1; ls < index + size; ls++)
            {
                if (ls >= Lasers.LASER_COUNT)
                    break;
                if (collisions[ls].HasValue)
                {
                    normal += position - CollisionPos(vectors, collisions, ls);
                }
                else
                {
                    free += vectors[ls];
                    freeCounter++;
                }
                counter++;
            }
            float percentage = (freeCounter * 1f) / (counter * 1f);

            if (percentage < wallPercentage)
            {
                isFree[i] = false;
                freeVectors[i] = Vector3.zero;
                normalVectors[i] = normal / (counter - freeCounter);
            }
            else
            {
                isFree[i] = true;
                normalVectors[i] = Vector3.zero;
                freeVectors[i] = free / (freeCounter);
            }
        }
    }

    bool OnPath()
    {
        bool onPath = false;

        Vector3 ap = linePoint - position;
        Vector3 dist = ap - Vector3.Dot(ap, lineVector) * lineVector;
        
        if (dist.magnitude < lineThreshold)
            onPath = true;

        return onPath;
    }

    Vector3 ClosestVector(Vector3[] vectors, Vector3 dir, out int sliceIndex)
    {
        int index = 0;
        Vector3 closest = vectors[index];
        float cross = Vector3.Cross(dir.normalized, transform.forward).y;
        float dot = Vector3.Dot(dir.normalized, transform.forward);

        if (dot < 0f)
        {
            if (cross < 0f)
                index = 0;
            else if (cross > 0f)
                index = Lasers.LASER_COUNT - 1;
        }
        else if (dot > 0f)
        {
            index = Mathf.FloorToInt(cross * Lasers.LASER_COUNT / 2);
            index += Lasers.LASER_COUNT / 2 - 1;
            index = Mathf.Clamp(index, 0, Lasers.LASER_COUNT - 1);
        }
        else
        {
            switch (wallSide)
            {

                case Direction.Left:
                default:
                    index = 0;
                    break;
                case Direction.Right:
                    index = Lasers.LASER_COUNT - 1;
                    break;
            }
        }
        closest = vectors[index];
        sliceIndex = (index * slices) / Lasers.LASER_COUNT;

        return closest;
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

    public void OnRenderObject()
    {
        if (!renderRays)
            return;
        Matrix4x4 translate = Matrix4x4.Translate(transform.position + lasers.offset);

        GL.PushMatrix();
        GL.MultMatrix(translate);

        rayNormalMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        for (int index = 0; index < normalVectors.Length; index++)
        {
            GL.Vertex(Vector3.zero);
            GL.Vertex(normalVectors[index] * wallDistance);
        }
        GL.End();

        rayFreeMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        for (int index = 0; index < freeVectors.Length; index++)
        {
            if (!isFree[index])
                continue;
            GL.Vertex(Vector3.zero);
            GL.Vertex(freeVectors[index] * Lasers.MAX_DISTANCE);
        }
        GL.End();
        GL.PopMatrix();
    }

    void OnCollisionEnter(Collision collision)
    {
        if ((collision.collider.gameObject.layer & LayerMask.NameToLayer("Map")) != 0)
        {
        }
    }
}
