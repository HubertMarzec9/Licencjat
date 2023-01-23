using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public Genes genes;
    public GameObject car;

    public Car(GameObject carPrefabe)
    {
        this.car = Instantiate(carPrefabe, transform.position, transform.rotation);
        this.genes = new Genes();
    }

}
