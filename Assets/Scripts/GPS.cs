using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class GPS : MonoBehaviour
{
    [System.Serializable]
    public struct MinMax
    {
        public float min;
        public float max;
    }
    public int seed;
    public MinMax distance;
    public Vector3 constraint;
    [Header("Abduction")]
    public bool abduction;
    [Range(0f, 1f)]
    public float probOnInit;
    public MinMax abductionTime;
    public MinMax abductionDelay;
    public MinMax abductionDistance;

    private Vector3 pos;
    private Vector3 lastPos;
    private Vector3 abductionNoise;
    private float abductionTimer;
    private float abductionDuration;

	void Start ()
	{
        Random.InitState(seed);
        StartCoroutine(Abduction());
        if (Random.value > probOnInit)
        {
            abductionTimer = Random.Range(abductionDelay.min, abductionDelay.max);
            abductionDuration = -1f;
        }
        else
        {
            abductionTimer = -1f;
            abductionDuration = -1f;
        }
        pos = transform.position;
    }
	
	void Update ()
    {
        lastPos = pos;
        pos = transform.position;

        Vector3 noise = new Vector3(Random.value * constraint.x,
                                    Random.value * constraint.y,
                                    Random.value * constraint.z).normalized;
        noise *= Random.Range(distance.min, distance.max);
        pos += noise;

        if (abductionTimer < 0f && abductionDuration < 0f)
        {
            abductionDuration = Random.Range(abductionTime.min, abductionTime.max);
            abductionTimer = Random.Range(abductionDelay.min, abductionDelay.max);

            abductionNoise = new Vector3(Random.value * constraint.x,
                                         Random.value * constraint.y,
                                         Random.value * constraint.z).normalized;
            abductionNoise *= Random.Range(abductionDistance.min, abductionDistance.max);
            lastPos += abductionNoise;
        }

        if(abductionDuration > 0f)
        {
            pos += abductionNoise;
            abductionDuration -= Time.deltaTime;
        }
        else if(abductionTimer > 0f)
            abductionTimer -= Time.deltaTime;


        pos = (pos + lastPos) / 2f;
    }

    IEnumerator Abduction()
    {
        while(true)
        {
            if(Random.value > probOnInit)
                yield return new WaitForSeconds(Random.Range(abductionDelay.min, abductionDelay.max));
            float timer = Random.Range(abductionTime.min, abductionTime.max);
            while(timer > 0f)
            {
                Vector3 abduction = new Vector3(Random.value, Random.value, Random.value).normalized;
                abduction *= Random.Range(distance.min, distance.max);
                pos += abduction;
                yield return null;
                timer -= Time.deltaTime;
            }
        }
    }

    public Vector3 GetPosition()
    {
        return pos;
    }
}
