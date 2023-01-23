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
    private Queue<GameObject> queueWaypoints = new Queue<GameObject>();
    public GameObject target;

    public GameObject rayForward;

    /// 

    public float fitness = 0;

    public float acceleration;
    public float maxTurnAngle;
    public float breakForce;
    public float lookAhead;
    public float lookAngle;
    public float controlSensitivityAcceleration;
    public float controlSensitivityAngle;
    public float controlSensitivity;

    private float currentAcceleration = 0;
    private float currentTurnAngle = 0;
    private float currentBreakForce = 0;
    public bool braking = false;
    public bool wall = false;

    private int layerMask = 1 << 2;

    public void setRandom()
    {
        maxTurnAngle = Random.Range(30, 75);
        acceleration = Random.Range(100, 1000);
        breakForce = Random.Range(100, 500);
        lookAhead = Random.Range(15, 35);
        controlSensitivityAcceleration = 1;
        controlSensitivityAngle = 0;
        controlSensitivity = Random.Range(.01f, .055f);
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
            queueWaypoints.Enqueue(child.gameObject);
        }

        target = queueWaypoints.Dequeue();
    }

    void FixedUpdate()
    {

        //Debug.DrawRay(rayForward.transform.position, target.transform.position, Color.green);
        Debug.DrawRay(rayForward.transform.position, rayForward.transform.TransformDirection(Vector3.forward) * lookAhead, Color.red);

        RaycastHit hit;
        //SPEED
        if (!wall)
        {
            if (Physics.Raycast(rayForward.transform.position, rayForward.transform.TransformDirection(Vector3.forward), out hit, lookAhead, layerMask))
            {
                Debug.Log("Did Hit");
                braking = true;
            }
            else
            {
                braking = false;
                controlSensitivityAcceleration += controlSensitivity;
                if (controlSensitivityAcceleration > 1f)
                {
                    controlSensitivityAcceleration = 1f;
                }

            }

            {
                Vector3 rV = transform.InverseTransformPoint(target.transform.position);
                currentTurnAngle = maxTurnAngle * ((rV.x / rV.magnitude));

                currentAcceleration = acceleration * controlSensitivityAcceleration;
            }

            if (braking)
            {
                currentBreakForce = breakForce;
            }
            else
                currentBreakForce = 0f;

            {
                frontLeft.motorTorque = currentAcceleration;
                frontRight.motorTorque = currentAcceleration;

                frontLeft.steerAngle = currentTurnAngle;
                frontRight.steerAngle = currentTurnAngle;

                UpdateWheel(frontLeft, frontLeftTransform);
                UpdateWheel(frontRight, frontRightTransform);
                UpdateWheel(backLeft, backLeftTransform);
                UpdateWheel(backRight, backRightTransform);
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.name.Equals(target.name))
        {
            this.fitness++;
            target = queueWaypoints.Dequeue();
            if (queueWaypoints.Count == 0)
                setWaypoints();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
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
