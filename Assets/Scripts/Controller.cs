using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public float speed;

	private Rigidbody rigid;
    private Lasers lasers;
    
	void Start ()
    {
		rigid = GetComponent<Rigidbody>();
        lasers = GetComponent<Lasers>();
	}
	
	void Update ()
    {
		rigid.velocity = transform.forward * speed;
	}
}
