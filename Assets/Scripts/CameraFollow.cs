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

    [Range(3f, 8f)]
    public float height;
    [Range(6f, 18f)]
    public float distance;

    public bool topView;
    [Range(10f, 50f)]
    public float orthoSize;

    private Camera _camera;

	void LateUpdate ()
    {
        _camera = GetComponent<Camera>();
        if (topView)
        {
            _camera.orthographic = true;
            _camera.orthographicSize = orthoSize;
            transform.forward = -Vector3.up;
            transform.position = objectTransform.position + height * Vector3.up;
        }
        else
        {
            _camera.orthographic = false;
            transform.position = objectTransform.position - objectTransform.forward * distance + Vector3.up * height;
            transform.forward = objectTransform.position - transform.position;
        }
	}
}
