using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackGenerator : MonoBehaviour
{

    public GameObject wayP;
    public int count = 30;

    private List<Vector3> vectors = new List<Vector3>();
    private int[] vectorsR;
    private int time;

    void Start()
    {
        time = (int) GameObject.Find("Values").GetComponent<wartosc>().c;
        count = (int) (time * .8f - 2);
        vectorsR = new int[count];
        wayPoints(count);
        draw();
    }

    void wayPoints(int count)
    {
        GameObject waypoints = GameObject.FindGameObjectWithTag("Waypoint");
        System.Random rnd = new System.Random();

        Vector3 poz = this.transform.position;
        Quaternion rot = this.transform.rotation;

        GameObject gameObject = Instantiate(wayP, poz, rot);
        gameObject.name = ("Waypoint_" + 0);
        gameObject.transform.SetParent(waypoints.transform, false);
        vectorsR[0] = rnd.Next(15) + 5;

        for (int i=1;i<count;i++)
        {
            poz += new Vector3( (rnd.Next(20)+5) , 0f, (rnd.Next(20)+5));
            vectors.Add(poz);
            vectorsR[i] = rnd.Next(15)+5;
            gameObject = Instantiate(wayP, poz, rot);
            gameObject.name = ("Waypoint_" + i);
            gameObject.transform.SetParent(waypoints.transform, false);

            float newScaleX = 25.0f;
            Vector3 currentScale = gameObject.transform.localScale;
            gameObject.transform.localScale = new Vector3(newScaleX, currentScale.y, currentScale.z);

        }
    }

    private void Update()
    {
        Vector3 poz = this.transform.position;
        foreach (Vector3 vector in vectors)
        {
            Vector3 start = new Vector3(poz.x, 0, poz.z);
            Vector3 end = new Vector3(vector.x, 0, vector.z);
            Debug.DrawLine(start, end, Color.red);
            poz = vector;
        }

    }

    void draw()
    {
        Vector3 poz = this.transform.position;
        int i = 0;
        foreach (Vector3 vector in vectors)
        {
            add(poz.x + vectorsR[i], poz.z, vector.x + vectorsR[i+1], vector.z);
            add(poz.x - vectorsR[i], poz.z, vector.x - vectorsR[i + 1], vector.z);
            poz = vector;
            i++;
        }

    }

    void add(float x1, float z1, float x2, float z2)
    {
        Vector3 start = new Vector3(x1, 0, z1);
        Vector3 end = new Vector3(x2, 0, z2);
        Debug.DrawLine(start, end, Color.red, 0);
        float lineLength = Vector3.Distance(start, end);

        Debug.DrawLine(start, end, Color.red);

        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.position = (start + end) / 2;
        cylinder.transform.localScale = new Vector3(lineLength, 3f, 0.1f);

        Vector3 direction = end - start;
        cylinder.transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);

        Destroy(cylinder.GetComponent<Collider>());
        MeshCollider meshCollider = cylinder.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = cylinder.GetComponent<MeshFilter>().mesh;
        meshCollider.convex = true;
    }
}