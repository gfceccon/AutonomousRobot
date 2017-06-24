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
        public MinMax(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
    public int seed;
    public MinMax distance;
    public Vector3 constraint;
    [Range(0f, 1f)]
    public float damping;


    //Header("Abduction")]
    //public bool abduction;
    //Range(0f, 1f)]
    //public float probOnInit;
    //public MinMax abductionTime;
    //public MinMax abductionDelay;
    //public MinMax abductionDistance;

    private Vector3 pos;
    private Vector3 noise;
    private Vector3 lastNoise;
    private Vector3 abductionNoise;
    private float abductionTimer;
    private float abductionDuration;

	void Start ()
	{
        Random.InitState(seed);
        //if(abduction)
        //{
        //    if (Random.value > probOnInit)
        //        abductionTimer = Random.Range(abductionDelay.min, abductionDelay.max);
        //    abductionDuration = -1f;
        //}
        pos = transform.position;
        noise = Vector3.zero;
        lastNoise = Vector3.zero;
}
	
	void Update ()
    {
        pos = transform.position;

        Vector3 noise = new Vector3(Random.value * constraint.x,
                                    Random.value * constraint.y,
                                    Random.value * constraint.z).normalized;
        noise *= Random.Range(distance.min, distance.max);

        //if(abduction)
        //    Abduction();

        pos += (1f - damping) * noise + damping * lastNoise;
    }

    //void Abduction()
    //{
    //    if (abductionTimer < 0f && abductionDuration < 0f)
    //    {
    //        abductionDuration = Random.Range(abductionTime.min, abductionTime.max);
    //        abductionTimer = Random.Range(abductionDelay.min, abductionDelay.max);
    //
    //        abductionNoise = new Vector3((2 * Random.value - 1) * constraint.x,
    //                                     (2 * Random.value - 1) * constraint.y,
    //                                     (2 * Random.value - 1) * constraint.z).normalized;
    //        abductionNoise *= Random.Range(abductionDistance.min, abductionDistance.max);
    //        lastPos += abductionNoise;
    //    }
    //
    //    if (abductionDuration > 0f)
    //    {
    //        pos += abductionNoise;
    //        abductionDuration -= Time.deltaTime;
    //    }
    //    else if (abductionTimer > 0f)
    //        abductionTimer -= Time.deltaTime;
    //}

    public Vector3 GetPosition()
    {
        return pos;
    }
}
