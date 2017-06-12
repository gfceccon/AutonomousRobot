using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Lasers : MonoBehaviour
{
	public const int quantity = 360;
	public const float maxDistance = 8f;
	public const float laserPerAngle = 2f;
    
	private Vector3[] vectors = new Vector3[quantity];
    private float?[] collisions = new float?[quantity];
    private Vector3[] means = new Vector3[quantity];
    private List<Vector3> wayPoints = new List<Vector3>();
    private List<GameObject> indicators = new List<GameObject>();
    private int meanCount;


    public Vector3[] Vectors { get { return vectors; } }
    public float?[] Collisions { get { return collisions; } }
    public Vector3[] Means { get { return means; } }
    public List<Vector3> WayPoints { get { return wayPoints; } }


    public Vector3 offset;
    public LayerMask collisionLayer;

    public bool renderRays;
    public Material rayHitMaterial;
    public Material rayMissMaterial;
    public Material watPointRayMaterial;

    public GameObject wayPointPrefab;

    public void OnRenderObject()
    {
        if (!renderRays)
            return;
        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);

        rayMissMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        for (int index = 0; index < quantity; index++)
        {
            if (collisions[index] == null)
            {
                GL.Vertex(offset);
                GL.Vertex(offset + vectors[index] * maxDistance);
            }
        }
        GL.End();

        rayHitMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        for (int index = 0; index < quantity; index++)
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
		float angle = quantity / laserPerAngle;
		float start = - angle / 2f;

        Vector3 forward = Vector3.forward;
        RaycastHit hitInfo;

        Vector3 collisionMean = Vector3.zero;
        float min = float.PositiveInfinity;
        bool lastCollision = false;
        int collisionCount = 1;
        meanCount = 0;

        for (int index = 0; index < quantity; index++)
        {
            vectors[index] = Quaternion.Euler(0, start + index / laserPerAngle, 0) * forward;
            if (Physics.Raycast(transform.position + offset, transform.rotation * vectors[index], out hitInfo, maxDistance, collisionLayer))
            {
                if (lastCollision)
                {
                    collisionMean += vectors[index];
                    collisionCount++;
                }
                else
                {
                    collisionMean = vectors[index];
                    collisionCount = 1;
                }
                if (min > hitInfo.distance)
                    min = hitInfo.distance;
                collisions[index] = hitInfo.distance;
                lastCollision = true;
            }
            else
            {
                if (lastCollision)
                {
                    means[meanCount] = collisionMean / collisionCount;
                    means[meanCount] = means[meanCount].normalized * maxDistance; /* * (maxDistance - min); */
                    min = float.PositiveInfinity;
                    meanCount++;
                }
                collisions[index] = null;
                lastCollision = false;
                
            }
        }
        if (lastCollision)
        {
            means[meanCount] = collisionMean / collisionCount;
            means[meanCount] = means[meanCount].normalized * maxDistance; /* * (maxDistance - min); */
            meanCount++;
        }
    }
}
