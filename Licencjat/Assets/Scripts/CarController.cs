using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public WheelCollider frontRight;
    public WheelCollider frontLeft;
    public WheelCollider backRight;
    public WheelCollider backLeft;

    public Transform frontRightTransform;
    public Transform frontLeftTransform;
    public Transform backRightTransform;
    public Transform backLeftTransform;

    private GameObject waypoints;
    public GameObject[] ArrayWaypoints = new GameObject[1000];
    private int sizeOfArrayWaypoints = 0;
    private int targetP = 0;
    public GameObject target;
    private GameObject meta;

    public GameObject rayForward;

    /// 

    public float fitness = 0;

    public float acceleration;
    public float maxTurnAngle;
    public float breakForce;
    public float lookAhead;
    public float controlSensitivity;

    public float currentAcceleration = 0;
    public float currentTurnAngle = 0;
    public float currentBreakForce = 0;
    public bool braking = false;
    public bool wall = false;

    private int layerMask = 1 << 2;

    public float bestTime = float.MaxValue;
    private float time = 0;
    private float minSpeedToBrake = 10f;

    public void setRandom()
    {
        acceleration =       Random.Range(1, 1000);
        maxTurnAngle =       Random.Range(5, 85);
        breakForce =         Random.Range(1, 500);
        lookAhead =          Random.Range(1, 15);
        controlSensitivity = Random.Range(.01f, .085f);
    }
    private void Start()
    {
        layerMask = ~layerMask;
        setWaypoints();
    }
    void setWaypoints()
    {
        waypoints = GameObject.FindGameObjectWithTag("Waypoint");

        foreach (Transform child in waypoints.transform)
        {
            //Debug.Log(child.gameObject.name);
            ArrayWaypoints[sizeOfArrayWaypoints] = child.gameObject;
            //Debug.Log(ArrayWaypoints[sizeOfArrayWaypoints].name);
            sizeOfArrayWaypoints++;
        }

        target = ArrayWaypoints[0];
        meta = ArrayWaypoints[sizeOfArrayWaypoints-1];

        //Debug.LogWarning("META: " + meta.name);
    }

    private void Update()
    {
        time += Time.deltaTime;
        //Debug.Log(time);
    }

    void FixedUpdate()
    {
        float speed = Vector3.Magnitude( this.GetComponent<Rigidbody>().velocity);
        //Debug.Log("SPEED: " + Mathf.RoundToInt((speed)) + " KM/H");

        RaycastHit hit;
        
        if (!wall)
        {
            Debug.DrawRay(rayForward.transform.position, this.target.transform.position, Color.green);
            Debug.DrawRay(rayForward.transform.position, rayForward.transform.TransformDirection(Vector3.forward) * lookAhead, Color.red);

            if (Physics.Raycast(rayForward.transform.position, rayForward.transform.TransformDirection(Vector3.forward), out hit, lookAhead, layerMask))
            {
                braking = true;
                currentBreakForce = breakForce * (1 - Vector3.Distance(rayForward.transform.position, hit.transform.position) / lookAhead);
            }
            else
            {
                braking = false;
                currentBreakForce = 0;
            }

            {
                Vector3 rV = transform.InverseTransformPoint(this.target.transform.position);
                currentTurnAngle = maxTurnAngle * ((((rV.x / rV.magnitude))) - (((rV.x / rV.magnitude)) * controlSensitivity));
                if(currentAcceleration < acceleration)
                    currentAcceleration += (controlSensitivity * 100);
            }

            if (braking)
            {
                if (speed > minSpeedToBrake)
                {
                    currentBreakForce = breakForce * 2;
                }
                else
                {
                    currentBreakForce = breakForce;
                }
            }
            else
            {
                currentBreakForce = 0f;
            }

            {
                frontLeft.motorTorque = currentAcceleration;
                frontRight.motorTorque = currentAcceleration;

                frontLeft.steerAngle = currentTurnAngle;
                frontRight.steerAngle = currentTurnAngle;

                frontLeft.brakeTorque = currentBreakForce;
                frontRight.brakeTorque = currentBreakForce;
                backLeft.brakeTorque = currentBreakForce;
                backRight.brakeTorque = currentBreakForce;

                UpdateWheel(frontLeft, frontLeftTransform);
                UpdateWheel(frontRight, frontRightTransform);
                UpdateWheel(backLeft, backLeftTransform);
                UpdateWheel(backRight, backRightTransform);
            }
        }
        else
        {
            currentAcceleration = 0;
            currentBreakForce = breakForce;

            frontLeft.motorTorque = currentAcceleration;
            frontRight.motorTorque = currentAcceleration;

            frontLeft.brakeTorque = currentBreakForce;
            frontRight.brakeTorque = currentBreakForce;
            backLeft.brakeTorque = currentBreakForce;
            backRight.brakeTorque = currentBreakForce;

            UpdateWheel(frontLeft, frontLeftTransform);
            UpdateWheel(frontRight, frontRightTransform);
            UpdateWheel(backLeft, backLeftTransform);
            UpdateWheel(backRight, backRightTransform);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        
        if (collider.gameObject.name.Equals(meta.name) && targetP == sizeOfArrayWaypoints-1)
        {
            Debug.Log("TIME: " + time);
            if (time < bestTime)
                bestTime = time;
            time = 0;
        }

        if (collider.gameObject.name.Equals(target.name))
        {
            this.fitness++;
            this.targetP++;
            if (targetP > sizeOfArrayWaypoints - 1)
                targetP = 0;
            target = ArrayWaypoints[targetP];
        }
        Debug.Log(target.name);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //TUTAJ
        if(!(collision.gameObject.name.Equals("Cube")))
        {
            if (!wall)
                this.fitness -= 5;
            wall = true;
            braking = true;
        }
    }

    private void UpdateWheel(WheelCollider col, Transform transform)
    {
        Vector3 pos;
        Quaternion rot;
        col.GetWorldPose(out pos, out rot);

        transform.position = pos;
        transform.rotation = rot;
    }
}
