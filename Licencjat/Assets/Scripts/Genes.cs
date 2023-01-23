using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genes : MonoBehaviour
{
    public float acceleration;
    public float maxTurnAngle;
    public float breakForce;
    public float lookAhead;
    public float lookAngle;
    public float controlSensitivityAcceleration;
    public float controlSensitivityAngle;
    public float controlSensitivity;

    public Genes()
    {
        maxTurnAngle = Random.Range(30, 75);
        acceleration = Random.Range(100, 1000);
        breakForce = Random.Range(100, 500);
        lookAhead = Random.Range(15, 35);
        controlSensitivityAcceleration = 1;
        controlSensitivityAngle = 0;
        controlSensitivity = Random.Range(.01f, .055f);
    }
}
