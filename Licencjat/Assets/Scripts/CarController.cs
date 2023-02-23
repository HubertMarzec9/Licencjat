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
    public float controlSensitivityAcceleration = 1;
    public float controlSensitivityAngle = 0;
    public float controlSensitivity;

    public float currentAcceleration = 0;
    public float currentTurnAngle = 0;
    public float currentBreakForce = 0;
    public bool braking = false;
    public bool wall = false;

    private int layerMask = 1 << 2;

    public float bestTime = float.MaxValue;
    private float time = 0;

    public void setRandom()
    {
        maxTurnAngle = Random.Range(30, 85);
        acceleration = Random.Range(50, 1000);
        breakForce = Random.Range(50, 200);
        lookAhead = Random.Range(5, 35);
        controlSensitivity = Random.Range(.03f, .055f);
    }
    private void Start()
    {
        if (acceleration.Equals(500))
            Debug.Log("500");
        controlSensitivityAcceleration = 1;
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

        Debug.LogWarning("META: " + meta.name);
    }

    private void Update()
    {
        time += Time.deltaTime;
        //Debug.Log(time);
    }

    void FixedUpdate()
    {
        float speed = Vector3.Magnitude( this.GetComponent<Rigidbody>().velocity);
        Debug.Log("SPEED: " + Mathf.RoundToInt((speed)) + " KM/H");

        RaycastHit hit;
        
        if (!wall)
        {
            //Debug.DrawRay(rayForward.transform.position, this.target.transform.position, Color.green);
            Debug.DrawRay(rayForward.transform.position, rayForward.transform.TransformDirection(Vector3.forward) * lookAhead, Color.red);

            if (Physics.Raycast(rayForward.transform.position, rayForward.transform.TransformDirection(Vector3.forward), out hit, lookAhead, layerMask))
            {
                currentBreakForce = breakForce * (1 - Vector3.Distance(rayForward.transform.position, hit.transform.position) / lookAhead);
            }
            else
            {
                currentBreakForce = 0;
            }

            
            {
                Vector3 rV = transform.InverseTransformPoint(this.target.transform.position);
                currentTurnAngle = maxTurnAngle * ((((rV.x / rV.magnitude))) - (((rV.x / rV.magnitude)) * controlSensitivity));
                //currentTurnAngle = maxTurnAngle * (rV.x / rV.magnitude);
                currentAcceleration = acceleration * controlSensitivityAcceleration;
                //Debug.Log(currentAcceleration + " = " + acceleration + " * " + controlSensitivityAcceleration);
            }

            /**/

            if (braking)
                currentBreakForce = breakForce;
            else
                currentBreakForce = 0f;
            

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
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!wall)
            this.fitness -= 5;
        wall = true;
        braking = true;
        
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
