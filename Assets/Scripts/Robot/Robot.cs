using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityStandardAssets.Vehicles.Car;

public class Robot : MonoBehaviour
{
    //[HideInInspector]
    //public bool win;
    //public GPS gps;
    //public GPS destination;
    //public float wallThreshold;
    //public float cornerThreshold;
    //public GameObject indicatorPrefab;
    //
    //private Lasers lasers;
    //private CarController car;
    //private CarUserControl userControl;
    //
    //private Vector3[] means = new Vector3[Lasers.LASER_COUNT];
    //private List<Vector3> clears = new List<Vector3>();
    //private List<Vector3> walls = new List<Vector3>();
    //private List<GameObject> indicators = new List<GameObject>();
    //
    //
    //private float?[] collisions;
    //private Vector3[] vectors;
    //
    //enum CarState
    //{
    //    Free,
    //    WallFollowing,
    //    Backtracking,
    //    Testing
    //}
    //
    //private CarState state;
    //
    //void Start()
    //{
    //    state = CarState.Free;
    //
    //    stop = false;
    //    win = false;
    //    
    //    lasers = GetComponent<Lasers>();
    //    car = GetComponent<CarController>();
    //    userControl = GetComponent<CarUserControl>();
    //}
    //
    //void Update()
    //{
    //    collisions = lasers.Collisions;
    //    vectors = lasers.Vectors;
    //    switch (state)
    //    {
    //        case CarState.Free:
    //            FreeState();
    //            break;
    //        case CarState.WallFollowing:
    //            WallState();
    //            break;
    //        case CarState.Backtracking:
    //            break;
    //        case CarState.Testing:
    //            break;
    //        default:
    //            break;
    //    }
    //}
    //
    //void FreeState()
    //{
    //    direction = destination.GetPosition() - gps.GetPosition();
    //    if (collisions[Lasers.LASER_COUNT / 2].HasValue && collisions[Lasers.LASER_COUNT / 2].Value < wallThreshold)
    //        state = CarState.WallFollowing;
    //}
    //
    //void WallState()
    //{
    //    bool collide = false;
    //    float sqrCornerThreshold = cornerThreshold * cornerThreshold;
    //    for (int i = 0; i < Lasers.LASER_COUNT; i++)
    //    {
    //        if (collisions[i].HasValue)
    //        {
    //            if (collide)
    //                continue;
    //
    //            Vector3 collisionPosition = gps.GetPosition() + vectors[i] * collisions[i].Value;
    //            GameObject indicator = FindClosest(collisionPosition, sqrCornerThreshold);
    //            if (!indicator)
    //            {
    //                indicator = Instantiate(indicatorPrefab);
    //                indicator.transform.position = collisionPosition;
    //                indicators.Add(indicator);
    //            }
    //            else
    //                indicator.transform.position = collisionPosition;
    //            collide = true;
    //        }
    //        else
    //            collide = false;
    //    }
    //    if(collide)
    //    {
    //        Vector3 collisionPosition = gps.GetPosition() + vectors[Lasers.LASER_COUNT - 1] * collisions[Lasers.LASER_COUNT - 1].Value;
    //        GameObject indicator = FindClosest(collisionPosition, sqrCornerThreshold);
    //        if (!indicator)
    //        {
    //            indicator = Instantiate(indicatorPrefab);
    //            indicator.transform.position = collisionPosition;
    //            indicators.Add(indicator);
    //        }
    //        else
    //            indicator.transform.position = collisionPosition;
    //    }
    //
    //    float minDistance = float.PositiveInfinity;
    //    Vector3 indPosition = indicators[0].transform.position;
    //    foreach (var indicator in indicators)
    //    {
    //        float distance = Vector3.SqrMagnitude(destination.GetPosition() - indicator.transform.position);
    //        if (distance < minDistance)
    //        {
    //            minDistance = distance;
    //            indPosition = indicator.transform.position;
    //        }
    //    }
    //
    //    direction = indPosition - gps.GetPosition();
    //
    //}
    //
    //GameObject FindClosest(Vector3 collisionPosition, float threshold)
    //{
    //    float minDistance = float.PositiveInfinity;
    //    GameObject indicator = null;
    //    foreach (var ind in indicators)
    //    {
    //        float sqrDistance = Vector3.SqrMagnitude(ind.transform.position - collisionPosition);
    //        if (sqrDistance < threshold)
    //        {
    //            indicator = ind;
    //            if (sqrDistance < minDistance)
    //                minDistance = sqrDistance;
    //        }
    //    }
    //    return indicator;
    //}
    //
    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.magenta;
    //    Gizmos.DrawRay(transform.position, direction.normalized * 20f);
    //}
    //
    //void FixedUpdate()
    //{
    //    if (stop)
    //        return;
    //    if (win)
    //    {
    //        userControl.enabled = false;
    //        car.Move(1f, 0f, 0f, 1f);
    //        return;
    //    }
    //    float steering = 0f;
    //    float accel = 1f;
    //    float handbreak = 0f;
    //
    //    steering = Vector3.Cross(transform.forward, direction.normalized).y;
    //    if(!userControl.Using || stop)
    //        car.Move(steering, accel, accel, handbreak);
    //}
    //
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Destination"))
    //        win = true;
    //}
}
