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
    public const int MIN_SLICES = 3;
    public const int MAX_SLICES = 10;
    public enum Direction
    {
        Left,
        Right
    }

    public enum BUGState
    {
        Line,
        Wall,
        Reverse
    }

    // Control booleans
    [HideInInspector]
    public bool win;

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
    [Tooltip("Follow vector length")]
    public float lineFollowDamp;
    [Tooltip("Wall distance along normal")]
    public float wallDistance;
    [Tooltip("Wall threshold along normal")]
    public float wallThreshold;
    [Tooltip("Reverse time")]
    public float reverseTime;

    [Header("Lasers Configuration")]
    [Tooltip("Number of slices")]
    [Range(BUG.MIN_SLICES, BUG.MAX_SLICES)]
    public int slices;
    [Tooltip("Lasers percentage to follow")]
    [Range(0f, 1f)]
    public float wallPercentage;

    [Header("Prefabs")]
    [Tooltip("Render vectors")]
    public bool renderRays;
    [Tooltip("Normal vectors")]
    public Material rayNormalMaterial;
    [Tooltip("Free path vectors")]
    public Material rayFreeMaterial;
    [Tooltip("Target path line")]
    public Material rayPathMaterial;

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

    private Vector3 linePoint;
    private Vector3 lineVector;

    public BUGState state;

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
        state = BUGState.Line;

        win = false;
        linePoint = gps.GetPosition();
        lineVector = (destination.GetPosition() - gps.GetPosition()).normalized;
    }

    void Update()
    {
        UpdateVectors(lasers.Vectors, lasers.Collisions);
        switch (state)
        {
            case BUGState.Line:
                FollowPath();
                break;
            case BUGState.Wall:
                FollowWall();
                break;
            case BUGState.Reverse:
            default:
                break;
        }
    }

    void FollowPath()
    {
        Vector3 tangent;
        bool onPath = OnPath(out tangent);
        direction = tangent + lineVector * lineFollowDamp;

        int dirIndex;
        int sliceIndex;
        dirIndex = ClosestVector(lasers.Vectors, direction, out sliceIndex);
        if (!isFree[sliceIndex])
            state = BUGState.Wall;
    }

    void FollowWall()
    {
        int ind = 0, step = 1;
        if (wallSide == Direction.Left)
        {
            ind = 0;
            step = 1;
        }
        else if (wallSide == Direction.Right)
        {
            ind = slices - 1;
            step = -1;
        }
        Vector3 normal = Vector3.zero;
        Vector3 target = Vector3.zero;
        while (ind >= 0 && ind < slices)
        {
            if (!isFree[ind])
            {
                float len = normalVectors[ind].magnitude;
                if (len > wallDistance)
                    len = 0;
                else
                    len = len - wallDistance;
                normal -= normalVectors[ind].normalized * len;
            }
            else
            {
                normal = normal.normalized;
                target = freeVectors[ind];
                break;
            }
            ind += step;
        }

        Vector3 tangent;
        bool onPath = OnPath(out tangent);
        Vector3 lineDir = tangent + lineVector * lineFollowDamp;
        int sliceIndex;
        int lineInd = ClosestVector(lasers.Vectors, lineDir, out sliceIndex);

        if (onPath && isFree[sliceIndex])
            state = BUGState.Line;
        direction = target + normal;
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

    bool OnPath(out Vector3 tangent)
    {
        bool onPath = false;

        Vector3 ap = linePoint - position;
        tangent = ap - Vector3.Dot(ap, lineVector) * lineVector;

        if (tangent.magnitude < lineThreshold)
            onPath = true;

        return onPath;
    }

    int ClosestVector(Vector3[] vectors, Vector3 dir, out int sliceIndex)
    {
        int index = 0;
        Vector3 normalized = dir.normalized;
        float cross = Vector3.Cross(normalized, transform.forward).y;
        float dot = Vector3.Dot(normalized, transform.forward);

        if (dot < 0f)
        {
            if (cross < 0f)
                index = Lasers.LASER_COUNT - 1;
            else if (cross > 0f)
                index = 0;
        }
        else if (dot > 0f)
        {
            float max = -1f;
            for (int ls = 0; ls < Lasers.LASER_COUNT; ls++)
            {
                float d = Vector3.Dot(vectors[ls], normalized);
                if (d > max)
                {
                    max = d;
                    index = ls;
                }
                else break;
            }
        }
        sliceIndex = (index * slices) / Lasers.LASER_COUNT;

        return index;
    }

    void FixedUpdate()
    {
        if (win)
        {
            userControl.enabled = false;
            car.Move(1f, 0f, 0f, 1f);
            return;
        }

        if (accel > 0)
            steering = Vector3.Cross(transform.forward, direction.normalized).y;
        if (!userControl.Using)
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

        rayPathMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Vertex(linePoint + lasers.offset - lineVector * 100f);
        GL.Vertex(linePoint + lineVector * 100f + lasers.offset);
        GL.Vertex(transform.position + lasers.offset);
        GL.Vertex(transform.position + lasers.offset + direction.normalized * 10f);
        GL.End();
    }

    void OnCollisionEnter(Collision collision)
    {
        if ((collision.collider.gameObject.layer & LayerMask.NameToLayer("Map")) != 0)
        {
            StartCoroutine(Collided());
            state = BUGState.Reverse;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destination"))
            win = true;
    }

    IEnumerator Collided()
    {
        accel = -1f;
        steering = 0f;
        yield return new WaitForSeconds(reverseTime);
        state = BUGState.Line;
        accel = 1f;
    }
}
