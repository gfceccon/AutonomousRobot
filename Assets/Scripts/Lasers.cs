using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

public class Lasers : MonoBehaviour
{
	const int quantity = 360;
	const float maxDistance = 8f;
	const float laserPerAngle = 2;
	Vector3[] vectors = new Vector3[quantity];
	void Start ()
	{
	}

	void Update ()
	{
		UpdateVectors();
	}

	void UpdateVectors()
	{
		float angle = quantity * 1f / laserPerAngle;
		float start = - Mathf.Floor(quantity / 2) * angle;
		Vector3 forward = transform.forward;
		for (int index = 0; index < quantity; index++)
			vectors[index] = Quaternion.Euler(0, start + angle * index, 0) * forward * maxDistance;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		for (int index = 0; index < quantity; index++)
			Gizmos.DrawRay(transform.position, vectors[index]);
	}
}
