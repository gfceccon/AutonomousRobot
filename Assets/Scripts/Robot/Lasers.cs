using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Lasers : MonoBehaviour
{
	public const int LASER_COUNT = 360;
	public const float MAX_DISTANCE = 8f;
	public const float LASER_PER_ANGLE = 2f;
    
	private Vector3[] vectors = new Vector3[LASER_COUNT];
    private float?[] collisions = new float?[LASER_COUNT];


    public Vector3[] Vectors { get { return vectors; } }
    public float?[] Collisions { get { return collisions; } }

    public Vector3 offset;
    public LayerMask collisionLayer;

    public bool renderRays;
    public Material rayHitMaterial;
    public Material rayMissMaterial;

    public void OnRenderObject()
    {
        if (!renderRays)
            return;
        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);

        rayMissMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        for (int index = 0; index < LASER_COUNT; index++)
        {
            if (collisions[index] == null)
            {
                GL.Vertex(offset);
                GL.Vertex(offset + vectors[index] * MAX_DISTANCE);
            }
        }
        GL.End();

        rayHitMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        for (int index = 0; index < LASER_COUNT; index++)
        {
            if (collisions[index] != null)
            {
                GL.Vertex(offset);
                GL.Vertex(offset + vectors[index] * collisions[index].Value);
            }
        }
        GL.End();
        GL.PopMatrix();
    }

	void Update ()
	{
		UpdateVectors();
	}

	void UpdateVectors()
	{
		float angle = LASER_COUNT / LASER_PER_ANGLE;
		float start = - angle / 2f;

        Vector3 forward = Vector3.forward;
        RaycastHit hitInfo;
        
        for (int index = 0; index < LASER_COUNT; index++)
        {
            vectors[index] = Quaternion.Euler(0, start + index / LASER_PER_ANGLE, 0) * forward;
            if (Physics.Raycast(transform.position + offset, transform.rotation * vectors[index], out hitInfo, MAX_DISTANCE, collisionLayer))
                collisions[index] = hitInfo.distance;
            else
                collisions[index] = null;
        }
    }
}
