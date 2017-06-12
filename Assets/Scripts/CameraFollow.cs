using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CameraFollow : MonoBehaviour
{
	public Transform objectTransform;
	public float distance;
	public float height;
    public bool topView;
    public float orthoSize;

    private Camera _camera;

	void Start ()
	{
	}

	void LateUpdate ()
	{
        if (topView)
        {
            Camera.main.orthographic = true;
            Camera.main.orthographicSize = orthoSize;
            transform.forward = -Vector3.up;
            transform.position = objectTransform.position + height * Vector3.up;
        }
        else
        {
            Camera.main.orthographic = false;
            transform.position = objectTransform.position - objectTransform.forward * distance + Vector3.up * height;
            transform.forward = objectTransform.position - transform.position;
        }
	}
}
