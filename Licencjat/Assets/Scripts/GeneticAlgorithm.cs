using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour
{
    public int populationSize;
    public GameObject carPrefabe;
    public TMPro.TextMeshProUGUI textMesh;
    private int generation = 0;
    public int generationMax = 25;
    private float startTime = 0;
    public float generationTime = 60;

    public GameObject[] population;
    public float mutationRate = .010f;
    System.Random rnd = new System.Random();

    void Start()
    {
        population = new GameObject[populationSize];

        for (int i = 0; i < populationSize; ++i)
        {
            GameObject c = Instantiate(carPrefabe, transform.position, transform.rotation);
            c.GetComponent<CarController>().setRandom();
            population[i] = c;
        }
    }

    GameObject[] generationFun()
    {
        GameObject[] populationPom = new GameObject[populationSize];

        startTime = Time.realtimeSinceStartup;

        for(int i=0; i < (populationSize); i = i + 2)
        {
            GameObject parent1 = Selection(populationSize/10);
            GameObject parent2 = Selection(populationSize/10);

            GameObject child1, child2;
            (child1, child2) = Crossover(parent1, parent2);


            Mutation(child1);
            Mutation(child2);

            populationPom[i] = child1;
            populationPom[i+1] = child2;
        }

        for (int i = 0; i < (populationSize); i++)
        {
            Destroy(population[i]);
        }

        generation++;

        return populationPom;
    }

    //One-point crossover
    (GameObject child1, GameObject child2) Crossover(GameObject car1, GameObject car2)
    {
        GameObject child1 = Instantiate(carPrefabe, transform.position, transform.rotation); ;
        GameObject child2 = Instantiate(carPrefabe, transform.position, transform.rotation); ;
        int random = rnd.Next(5);
        
        child2.GetComponent<CarController>().maxTurnAngle = (random >= 0) ? car1.GetComponent<CarController>().maxTurnAngle : car2.GetComponent<CarController>().maxTurnAngle;
        child2.GetComponent<CarController>().acceleration = (random >= 1) ? car1.GetComponent<CarController>().acceleration : car2.GetComponent<CarController>().acceleration;
        child1.GetComponent<CarController>().breakForce = (random >= 2) ? car1.GetComponent<CarController>().breakForce : car2.GetComponent<CarController>().breakForce;
        child1.GetComponent<CarController>().lookAhead = (random >= 3) ? car1.GetComponent<CarController>().lookAhead : car2.GetComponent<CarController>().lookAhead;
        child1.GetComponent<CarController>().controlSensitivity = (random >= 4) ? car1.GetComponent<CarController>().controlSensitivity : car2.GetComponent<CarController>().controlSensitivity;

        child2.GetComponent<CarController>().maxTurnAngle = (random >= 0) ? car2.GetComponent<CarController>().maxTurnAngle : car1.GetComponent<CarController>().maxTurnAngle;
        child2.GetComponent<CarController>().acceleration = (random >= 1) ? car2.GetComponent<CarController>().acceleration : car1.GetComponent<CarController>().acceleration;
        child2.GetComponent<CarController>().breakForce = (random >= 2) ? car2.GetComponent<CarController>().breakForce : car1.GetComponent<CarController>().breakForce;
        child2.GetComponent<CarController>().lookAhead = (random >= 3) ? car2.GetComponent<CarController>().lookAhead : car1.GetComponent<CarController>().lookAhead;
        child2.GetComponent<CarController>().controlSensitivity = (random >= 4) ? car2.GetComponent<CarController>().controlSensitivity : car1.GetComponent<CarController>().controlSensitivity;
        
        return (child1, child2);
    }

    void Mutation(GameObject child)
    {
        if (rnd.NextDouble() < mutationRate)
        {
            child.GetComponent<CarController>().maxTurnAngle = rnd.Next(30, 75);
        }

        if (rnd.NextDouble() < mutationRate)
        {
            child.GetComponent<CarController>().acceleration = rnd.Next(100, 1000);
        }

        if (rnd.NextDouble() < mutationRate)
        {
            child.GetComponent<CarController>().breakForce = rnd.Next(100, 500);
        }

        if (rnd.NextDouble() < mutationRate)
        {
            child.GetComponent<CarController>().lookAhead = rnd.Next(15, 35);
        }

        if (rnd.NextDouble() < mutationRate)
        {
            child.GetComponent<CarController>().controlSensitivity = (float)(rnd.Next(1, 55) / 100);
        }

    }

    //Tournament Selection
    GameObject Selection(int k = 3)
    {
        GameObject best = null, x;

        for(int i=0; i < k; ++i)
        {
            Debug.Log("rnd.Next(populationSize) : " + rnd.Next(populationSize));
            x = population[rnd.Next(populationSize)];
            if(best == null || best.GetComponent<CarController>().fitness < x.GetComponent<CarController>().fitness)
            {
                best = x;
            }
        }
        return best;
    }

    private void Update()
    {
        textMesh.text = "Generation: " + generation + "\n" + "Time: " + Time.realtimeSinceStartup % generationTime;

        if (Time.realtimeSinceStartup > startTime + generationTime)
        {
            population =  generationFun();
        }
    }
}
