using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Lasers : MonoBehaviour
{
	const int quantity = 360;
	const float maxDistance = 8f;
	const float laserPerAngle = 2f;
	Vector3[] vectors = new Vector3[quantity];
    Vector3?[] collision = new Vector3?[quantity];
/*
    public int lineCount = 100;
    public float radius = 3.0f;
    static Material lineMaterial;

    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        // Draw lines
        GL.Begin(GL.LINES);
        for (int i = 0; i < lineCount; ++i)
        {
            float a = i / (float)lineCount;
            float angle = a * Mathf.PI * 2;
            // Vertex colors change from red to green
            GL.Color(new Color(a, 1 - a, 0, 0.8F));
            // One vertex at transform position
            GL.Vertex3(0, 0, 0);
            // Another vertex at edge of circle
            GL.Vertex3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
        }
        GL.End();
        GL.PopMatrix();
    }
*/
	void Update ()
	{
		UpdateVectors();
	}

	void UpdateVectors()
	{
		float angle = quantity / laserPerAngle;
		float start = - angle / 2f;

        Vector3 forward = transform.forward;
        RaycastHit hitInfo;

        for (int index = 0; index < quantity; index++)
        {
            vectors[index] = Quaternion.Euler(0, start + index / laserPerAngle, 0) * forward;
            if (Physics.Raycast(transform.position, vectors[index], out hitInfo, maxDistance))
                collision[index] = hitInfo.point;
            else
                collision[index] = null;
        }
	}

	void OnDrawGizmos()
	{
        for (int index = 0; index < quantity; index++)
        {
            if (collision[index] == null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, vectors[index] * maxDistance);
            }
            else
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(transform.position, vectors[index] * Vector3.Distance(transform.position, collision[index].Value));
            }
            Gizmos.color = Color.green;
            if (collision[index] != null)
                Gizmos.DrawSphere(collision[index].Value, 0.05f);
        }
	}
}
