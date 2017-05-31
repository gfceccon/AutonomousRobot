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

	void Start ()
	{
	}

	void LateUpdate ()
	{
		transform.position = objectTransform.position - objectTransform.forward * distance + Vector3.up * height;
		transform.forward = objectTransform.position - transform.position;
	}
}
