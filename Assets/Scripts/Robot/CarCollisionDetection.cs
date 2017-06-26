using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

public class CarCollisionDetection : MonoBehaviour
{
    public TurnWall wall;

    void OnCollisionEnter(Collision collision)
    {
        if((collision.collider.gameObject.layer & LayerMask.NameToLayer("Map")) != 0)
        {
            wall.Collide();
        }
    }
}
