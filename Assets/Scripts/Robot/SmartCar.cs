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
public class SmartCar : MonoBehaviour
{
    // Control booleans
    [HideInInspector]
    public bool win;
    private bool stop;
    
    // References
    public GPS gps;
    public Lasers lasers;
    public CarController car;
    public CarUserControl userControl;

    [Header("Configurations")]
    [Tooltip("Destination GPS")]
    public GPS destination;
    [Tooltip("Wall threshold along normal")]
    public float wallThreshold;
    [Tooltip("Corner threshold based on derivative")]
    public float cornerThreshold;
    [Tooltip("Round corner threshold based on accumlated derivatives")]
    public float roundThreshold;
    [Tooltip("Vertex threshold based on distance")]
    public float vertexThreshold;

    [Header("Vertices Prefabs")]
    [Tooltip("Wall vertice indicator")]
    public GameObject wallObj;
    [Tooltip("Open corner indicator")]
    public GameObject openCornerObj;
    [Tooltip("Closed corner vertice indicator")]
    public GameObject closedCornerObj;
    
    // Lasers information
    private float?[] collisions;
    private Vector3[] vectors;

    // Squared distances
    float sqrCornerThreshold;
    float sqrWallThreshold;
    float sqrVertexThreshold;
    float sqrWallVertexThreshold;

    // Car position and direction movement
    private Vector3 position;
    private Vector3 direction;
    
    // Map graph
    private List<Vertex> vertices;
    private Vertex following;

    // Accumulated derivatives
    float accumulated;

    enum CarState
    {
        Free,
        Wall,
        Corner
    }

    private CarState state;

    void Start ()
	{
        // Get components
        gps = GetComponent<GPS>();
        lasers = GetComponent<Lasers>();
        car = GetComponent<CarController>();
        userControl = GetComponent<CarUserControl>();

        // Calculate squared distances
        sqrWallThreshold = wallThreshold * wallThreshold;
        sqrCornerThreshold = cornerThreshold * cornerThreshold;
        sqrVertexThreshold = vertexThreshold * vertexThreshold;

        // Init state
        state = CarState.Free;
    }
	
	void Update ()
    {
        // Update values
        vectors = lasers.Vectors;
        position = gps.GetPosition();
        collisions = lasers.Collisions;

        CalculateVertices();

        switch (state)
        {
            case CarState.Free:
                FollowPath();
                break;
            case CarState.Wall:
                FollowWall();
                break;
            case CarState.Corner:
                Backtrack();
                break;
            default:
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
        float steering = 0f;
        float accel = 1f;
        float handbreak = 0f;

        steering = Vector3.Cross(transform.forward, direction.normalized).y;
        if (!userControl.Using || stop)
            car.Move(steering, accel, accel, handbreak);
    }

    void FollowPath()
    {

    }

    void FollowWall()
    {

    }

    void Backtrack()
    {

    }

    /// <summary>
    /// Populate graph with first collision and the follow last collision
    /// </summary>
    Vertex CreateCorners(int index)
    {
        Vertex vertex = null;
        bool first = true;
        bool collision = false;
        for (int ls = index; ls < Lasers.LASER_COUNT; ls++)
        {
            if (collisions[ls].HasValue)
            {
                if (collision)
                    continue;
                collision = true;
                first = false;

                vertex = CreateVertex(VType.OpenCorner, CollisionPos(ls), null);
            }
            else if(!first)
            {
                CreateVertex(VType.OpenCorner, CollisionPos(ls), vertex);
                break;
            }
        }
        return vertex;
    }

    /// <summary>
    /// Update vertecies map
    /// </summary>
    void CalculateVertices()
    {
        if (vertices.Count == 0)
            CreateCorners(0);
        // Last values
        Vertex lVertex = null;
        Vector3 lPosition = Vector3.zero;
        Vector3 lDerivative = Vector3.zero;

        // Current values
        Vector3 cDerivative = Vector3.zero;
        Vector3 cPosition = Vector3.zero;
        Vertex cVertex = null;
        float sqrDistance;
        float linearity;
        float cornerDirection;

        bool collision = false;

        // First collisions
        if (collisions[0].HasValue)
        {
            lPosition = CollisionPos(0);
            lVertex = CheckVertex(lPosition, null);
            // Create a new open corner if do not exist
            if (lVertex == null)
                CreateCorners(0);
            collision = true;
        }

        for (int ls = 1; ls < Lasers.LASER_COUNT; ls++)
        {
            if (collisions[ls].HasValue)
            {
                cPosition = CollisionPos(ls);
                cVertex = CheckVertex(position, lVertex);
                if (cVertex != null)
                    sqrDistance = Vector3.SqrMagnitude(position - cVertex.pos);
                else if(!collision)
                {
                    lVertex = CreateCorners(ls);
                    lDerivative = Vector3.zero;
                    lPosition = cPosition;
                }

                linearity = Vector3.Cross(lDerivative.normalized, cDerivative.normalized).y;
                cDerivative = cPosition - lPosition;
                accumulated += linearity;

                if (lVertex.next != null)
                    cornerDirection = Vector3.Dot(lVertex.next.direction, cDerivative);
                else
                    cornerDirection = -1f;

                bool wall = Mathf.Abs(linearity) < cornerThreshold;
                bool corner = !wall & linearity < 0f;
                bool round = accumulated > roundThreshold;
                bool overrideVertex = cornerDirection > 0f;


                if (lVertex != cVertex)
                    lVertex = cVertex;
                collision = true;
            }
            else
                collision = false;
        }
    }

    Vector3 CollisionPos(int index) { return position + vectors[index] * collisions[index].Value; }

    /// <summary>
    /// Find closest vertex based on position
    /// </summary>
    /// <param name="position">Position</param>
    /// <returns></returns>
    Vertex FindClosest(Vector3 position)
    {
        Vertex closest = null;
        float min = float.PositiveInfinity;
        foreach (Vertex vert in vertices)
        {
            float distance = Vector3.SqrMagnitude(vert.pos - position);
            if( distance < sqrVertexThreshold
                && distance < min)
                closest = vert;
        }
        return closest;
    }

    /// <summary>
    /// Check if alredy exist a vertex close to the position
    /// </summary>
    /// <param name="position">Current position</param>
    /// <param name="last">Last vertex</param>
    /// <param name="ind">Laser index</param>
    /// <returns></returns>
    Vertex CheckVertex(Vector3 position, Vertex last)
    {
        Vertex closest = FindClosest(position);

        return null;
    }

    /// <summary>
    /// Create a vertex and connect with last one
    /// </summary>
    /// <param name="type"></param>
    /// <param name="position"></param>
    /// <param name="last"></param>
    /// <param name="ind"></param>
    /// <returns></returns>
    Vertex CreateVertex(VType type, Vector3 position, Vertex last)
    {
        Vertex created = new Vertex(position, type);
        if(last != null)
        {
            last.Next(created);
            created.Prev(last);
        }
        return created;
    }
}
