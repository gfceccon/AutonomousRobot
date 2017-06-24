using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

public class Edge
{
    public Vertice from;
    public Vertice to;
    public Vector3 direction;
    public Vector3 normal;

    private Edge() { }

    public Edge(Vertice from, Vertice to, Vector3 cross)
    {
        
    }
}
