using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public float speed;

	private Rigidbody rigid;
    
	void Start ()
    {
		rigid = GetComponent<Rigidbody>();
	}
	
	void Update ()
    {
		rigid.velocity = transform.forward * speed;
	}
}
