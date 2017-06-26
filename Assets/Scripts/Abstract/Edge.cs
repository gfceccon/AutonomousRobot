using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

public class Edge
{
    public Vertex from;
    public Vertex to;
    public Vector3 direction;
    public Vector3 normal;

    private Edge() { }

    public Edge(Vertex from, Vertex to, Vector3 cross)
    {
        this.from = from;
        this.to = to;
        this.direction = (to.pos - from.pos).normalized;
        this.normal = Vector3.Cross(this.direction, cross).normalized;
    }
}
