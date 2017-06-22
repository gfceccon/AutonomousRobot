using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController car; // the car controller we want to use
        private bool _using;
        public bool Using { get { return _using; } }

        private void Awake()
        {
            // get the car controller
            car = GetComponent<CarController>();
        }


        private void FixedUpdate()
        {
            // pass the input to the car!
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");

            Vector3 carMove = new Vector3(h, v, handbrake);
            if (carMove != Vector3.zero)
            {
                _using = true;
                car.Move(h, v, v, handbrake);
            }
            else
                _using = false;
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
    }
}
