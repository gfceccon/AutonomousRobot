using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class GPS : MonoBehaviour
{
    [System.Serializable]
    public struct MinMax
    {
        public float min;
        public float max;
        public MinMax(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
    public MinMax distance;
    public Vector3 constraint;
    [Range(0f, 1f)]
    public float damping;

    private Vector3 pos;
    private Vector3 noise;
    private Vector3 lastNoise;
    private Vector3 abductionNoise;
    private float abductionTimer;
    private float abductionDuration;

	void Start ()
	{
        pos = transform.position;
        noise = Vector3.zero;
        lastNoise = Vector3.zero;
}
	
	void Update ()
    {
        pos = transform.position;

        noise = new Vector3(Random.value * constraint.x,
                                    Random.value * constraint.y,
                                    Random.value * constraint.z).normalized;
        noise *= Random.Range(distance.min, distance.max);

        pos += (1f - damping) * noise + damping * lastNoise;
    }

    public Vector3 GetPosition()
    {
        return pos;
    }
}
