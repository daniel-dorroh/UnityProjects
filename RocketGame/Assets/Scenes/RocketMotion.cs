using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketMotion : MonoBehaviour
{
    public Rigidbody Rocket;
    public float Thrust = 14.0f;
    public float ManeuverAngle = 10.0f;

    public void Start()
    {
        if (Rocket == default(Rigidbody))
        {
            Rocket = GetComponent<Rigidbody>();
        }
    }

    public void Update()
    {
        ProcessInputs();
    }

    private void ProcessInputs()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Rocket.AddRelativeForce(new Vector3(0, Thrust, 0));
        } 

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward, ManeuverAngle * Time.deltaTime);
        } 
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.back, ManeuverAngle * Time.deltaTime);
        }
    }
}
