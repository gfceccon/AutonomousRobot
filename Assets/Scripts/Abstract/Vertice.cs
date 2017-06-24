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

public class Vertice
{
    public VType type;
    public Vector3 pos;
    public Edge next;
    public Edge prev;

    private Vertice() { }

    public Vertice(Vector3 from, VType type)
    {
        this.type = type;
        this.pos = from;
    }

    public void Next(Vertice to)
    {
        this.next = new Edge(this, to, Vector3.up);
    }

    public void Prev(Vertice to)
    {
        this.prev = new Edge(to, this, Vector3.down);
    }
}
