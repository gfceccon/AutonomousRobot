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
    private GPS gps;
    private Lasers lasers;
    private CarController car;
    private CarUserControl userControl;

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
    private List<Vertex> graph;
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
        graph = new List<Vertex>();
        vertices = new List<Vertex>();
        lasers.OnComplete(CalculateVertices);
}
	
	void Update ()
    {
        // Update values
        position = gps.GetPosition();

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
    /// Create the graph with first collision and the follow last collision
    /// </summary>
    Vertex CreateCorners(Vector3[] vectors, float?[] collisions, int index)
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

                // Add begining
                vertex = CreateVertex(VType.OpenCorner, CollisionPos(vectors, collisions, ls), null);
                vertices.Add(vertex);
                graph.Add(vertex);
            }
            else if(!first)
            {
                // Add end
                Vertex last = CreateVertex(VType.OpenCorner, CollisionPos(vectors, collisions, ls - 1), vertex);
                vertices.Add(vertex);
                graph.Add(last);
                return vertex;
            }
        }
        if(!first && collision)
        {
            // Add end
            Vertex last = CreateVertex(VType.OpenCorner, CollisionPos(vectors, collisions, Lasers.LASER_COUNT - 1), vertex);
            vertices.Add(last);
            graph.Add(last);
        }
        return vertex;
    }

    /// <summary>
    /// Update vertecies map
    /// </summary>
    void CalculateVertices(Vector3[] vectors, float?[] collisions)
    {
        // Last values
        Vertex lVertex = null;
        Vector3 lPosition = Vector3.zero;
        Vector3 lDerivative = Vector3.zero;

        // Current values
        Vector3 cDerivative = Vector3.zero;
        Vector3 cPosition = Vector3.zero;
        Vertex cVertex = null;
        float linearity;
        float cornerDirection;
        bool collision = false;

        for (int ls = 0; ls < Lasers.LASER_COUNT; ls++)
        {
            if (collisions[ls].HasValue)
            {
                cPosition = CollisionPos(vectors, collisions, ls);
                cVertex = FindClosest(position);
                // If it is a new collision and it's probably a new wall
                // Then create the wall
                if (cVertex == null && !collision)
                {
                    lVertex = CreateCorners(vectors, collisions, ls);
                    lDerivative = Vector3.zero;
                    lPosition = cPosition;
                    collision = true;
                    continue;
                }

                cDerivative = cPosition - lPosition;
                // How linear is the last derivative with this one, then sum
                linearity = Vector3.Cross(lDerivative.normalized, cDerivative.normalized).y;
                accumulated += linearity;

                if (lVertex.next != null)
                    cornerDirection = Vector3.Dot(lVertex.next.direction, cDerivative);
                else
                    cornerDirection = -1f;

                bool hasCurrent = cVertex != null;
                bool wall = Mathf.Abs(linearity) < cornerThreshold;
                bool corner = !wall & linearity < 0f;
                bool round = accumulated > roundThreshold;
                bool overrideVertex = cornerDirection > 0f;
                if (hasCurrent)
                    overrideVertex = overrideVertex & (cVertex.type == VType.OpenCorner);
                else
                    overrideVertex = false;

                if (!hasCurrent & corner)
                    CreateVertex(VType.ClosedCorner, cPosition, lVertex);
                else if (!hasCurrent & round)
                    CreateVertex(VType.OpenCorner, cPosition, lVertex);
                else if (!hasCurrent & wall)
                    CreateVertex(VType.Wall, cPosition, lVertex);
                else if (overrideVertex)
                    lVertex.pos = cPosition;

                if (lVertex != cVertex)
                    lVertex = cVertex;
                if (!hasCurrent & round)
                    accumulated = 0f;
                collision = true;
            }
            else
                collision = false;
        }
    }

    Vector3 CollisionPos(Vector3[] vectors, float?[] collisions, int index) { return position + vectors[index] * collisions[index].Value; }

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
    /// Create a vertex and connect with last one
    /// Create a new game object indicator
    /// </summary>
    /// <param name="type">Type of the vertex</param>
    /// <param name="position">Current position</param>
    /// <param name="last">Previous one</param>
    /// <returns>A new vertex</returns>
    Vertex CreateVertex(VType type, Vector3 position, Vertex last)
    {
        GameObject obj = null;
        switch (type)
        {
            case VType.Wall:
                obj = Instantiate(wallObj);
                break;
            case VType.OpenCorner:
                obj = Instantiate(openCornerObj);
                break;
            case VType.ClosedCorner:
                obj = Instantiate(closedCornerObj);
                break;
            default:
                break;
        }

        Vertex vertex = new Vertex(position, type, obj);
        if(last != null)
        {
            last.Next(vertex);
            vertex.Prev(last);
        }
        vertices.Add(vertex);
        return vertex;
    }
}
