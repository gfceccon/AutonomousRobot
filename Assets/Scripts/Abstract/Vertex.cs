using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

public enum VType
{
    Wall,
    OpenCorner,
    ClosedCorner
}

public class Vertex
{
    public VType type;
    public Vector3 pos;
    public Edge next;
    public Edge prev;
    public Vector3 normal;

    public GameObject obj;

    private Vertex() { }

    public Vertex(Vector3 pos, VType type, GameObject obj = null)
    {
        this.type = type;
        this.pos = pos;
        this.obj = obj;
        if (obj != null)
            obj.transform.position = pos;
    }

    public void Next(Vertex to)
    {
        next = new Edge(this, to, Vector3.up);
        if(prev != null)
            normal = (prev.normal + next.normal).normalized;
    }

    public void Prev(Vertex to)
    {
        prev = new Edge(to, this, Vector3.down);
        if (next != null)
            normal = (prev.normal + next.normal).normalized;
    }

    public void Pos(Vector3 position)
    {
        pos = position;
        if(obj != null)
            obj.transform.position = position;
    }
}
