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

    [HideInInspector]
	public Vector3[] vectors = new Vector3[quantity];
    [HideInInspector]
    public float?[] collisions = new float?[quantity];
    [HideInInspector]
    public Vector3[] means = new Vector3[quantity];
    public int meanCount;
    
    public Material hit;
    public Material miss;
    public Material mean;

    public void OnRenderObject()
    {

        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);

        miss.SetPass(0);
        GL.Begin(GL.LINES);
        for (int index = 0; index < quantity; index++)
        {
            if (collisions[index] == null)
            {
                GL.Vertex3(0, 0, 0);
                GL.Vertex(vectors[index] * maxDistance);
            }
        }
        GL.End();

        hit.SetPass(0);
        GL.Begin(GL.LINES);
        for (int index = 0; index < quantity; index++)
        {
            if (collisions[index] != null)
            {
                GL.Vertex3(0, 0, 0);
                GL.Vertex(vectors[index] * collisions[index].Value);
            }
        }
        GL.End();

        mean.SetPass(0);
        GL.Begin(GL.LINES);
        for (int index = 0; index < meanCount; index++)
        {
            GL.Vertex3(0, 0, 0);
            GL.Vertex(means[index]);
        }
        GL.Vertex3(0, 0, 0);
        GL.Vertex(Vector3.forward * maxDistance);
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
            if (Physics.Raycast(transform.position, transform.rotation * vectors[index], out hitInfo, maxDistance))
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
                    means[meanCount] = means[meanCount].normalized * (maxDistance - min);
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
            means[meanCount] = means[meanCount].normalized * (maxDistance - min);
            meanCount++;
        }
    }
}
