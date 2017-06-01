using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

	private Rigidbody rigid;


	// Use this for initialization
	void Start () {
		rigid = GetComponent<Rigidbody>();//pegando o tipo to componente
	}
	
	// Update is called once per frame
	void Update () {
		rigid.velocity = new Vector3 (1, 0, 0);
	}
}
