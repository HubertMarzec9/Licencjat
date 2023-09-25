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
    public float mutationRate = .01f;
    public bool startSimulation = false;
    public bool startSimulation_ = false;

    void Start()
    {
        GameObject values = GameObject.Find("Values");
        int a = values.GetComponent<wartosc>().a;
        int b = values.GetComponent<wartosc>().b;
        float c = values.GetComponent<wartosc>().c;

        populationSize = a;

        generationMax = b;

        generationTime = c;

        startSimulation_ = true;
    }

    void bestTime()
    {
        var minVal = population.Min(x => x.GetComponent<CarController>().bestTime);
        Debug.Log(minVal);
        if(minVal != float.MaxValue && minVal != 0)
        {
            var gO = population.FirstOrDefault(x => x.GetComponent<CarController>().bestTime.Equals(minVal));
            gO.GetComponent<CarController>().fitness += 5;

            Debug.Log("Pojazd dojecha³ do koñca: " + gO.name);
            Debug.Log("Generacja: " + generation);
        }
            
    }

    GameObject[] generationFun()
    {
        GameObject[] populationPom = new GameObject[populationSize];

        bestTime();
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
    (GameObject, GameObject) Crossover(GameObject car1, GameObject car2)
    {
        GameObject child1 = Instantiate(carPrefabe, transform.position, transform.rotation);
        GameObject child2 = Instantiate(carPrefabe, transform.position, transform.rotation);
        System.Random rnd = new System.Random();
        int random = rnd.Next(5);

        CarController ch1 = child1.GetComponent<CarController>();
        CarController ch2 = child2.GetComponent<CarController>();
        CarController c1 = car1.GetComponent<CarController>();
        CarController c2 = car2.GetComponent<CarController>();

        ch1.maxTurnAngle = (random >= 0) ? c1.maxTurnAngle : c2.maxTurnAngle;
        ch1.breakForce = (random >= 1) ? c1.breakForce : c2.breakForce;
        ch1.lookAhead = (random >= 2) ? c1.lookAhead : c2.lookAhead;
        ch1.acceleration = (random >= 3) ? c1.acceleration : c2.acceleration;
        ch1.controlSensitivity = (random >= 4) ? c1.controlSensitivity : c2.controlSensitivity;

        ch2.maxTurnAngle = (random >= 0) ? c2.maxTurnAngle : c1.maxTurnAngle;
        ch2.breakForce = (random >= 1) ? c2.breakForce : c1.breakForce;
        ch2.lookAhead = (random >= 2) ? c2.lookAhead : c1.lookAhead;
        ch2.acceleration = (random >= 3) ? c2.acceleration : c1.acceleration;
        ch2.controlSensitivity = (random >= 4) ? c2.controlSensitivity : c1.controlSensitivity;
        
        return (child1, child2);
    }

    void Mutation(GameObject child)
    {
        System.Random rnd = new System.Random();
        if (rnd.NextDouble() < mutationRate)
        {
            child.GetComponent<CarController>().maxTurnAngle = rnd.Next(30, 85);
        }

        if (rnd.NextDouble() < mutationRate)
        {
            child.GetComponent<CarController>().acceleration = rnd.Next(50, 1000);
        }

        if (rnd.NextDouble() < mutationRate)
        {
            child.GetComponent<CarController>().breakForce = rnd.Next(50, 200);
        }

        if (rnd.NextDouble() < mutationRate)
        {
            child.GetComponent<CarController>().lookAhead = rnd.Next(5, 35);
        }

        if (rnd.NextDouble() < mutationRate)
        {
            child.GetComponent<CarController>().controlSensitivity = (float)(rnd.Next(3, 55) / 100);
        }

    }

    //Tournament Selection
    GameObject Selection(int k = 3)
    {
        System.Random rnd = new System.Random();
        GameObject best = null, x;

        for(int i=0; i < k; ++i)
        {
            x = population[rnd.Next(populationSize)];
            if(best == null || best.GetComponent<CarController>().fitness < x.GetComponent<CarController>().fitness)
            {
                best = x;
            }
        }

        if (best.GetComponent<CarController>().fitness < 0)
            best = Selection(k+1);

        return best;
    }

    private void Update()
    {
        if (startSimulation_)
        {
            population = new GameObject[populationSize];

            for (int i = 0; i < populationSize; ++i)
            {
                GameObject c = Instantiate(carPrefabe, transform.position, transform.rotation);
                c.GetComponent<CarController>().setRandom();
                population[i] = c;
            }

            startSimulation_ = false;
            startSimulation = true;

            generation = 0;
            startTime = Time.realtimeSinceStartup;
        }

        if (startSimulation)
        {
            
            float elapsedTime = Time.realtimeSinceStartup - startTime;

            textMesh.text = "Generation: " + generation + "\n" + "Time: " + Math.Round(elapsedTime, 2);

            if (elapsedTime >= generationTime)
            {
                population = generationFun();
                startTime = Time.realtimeSinceStartup;
            }

            if(generation == generationMax)
            {
                generationTime = 3600;
            }

        }
        else
        {
            startTime = 0f;
            generation = 0;
            if (textMesh != null)
                textMesh.text = "Generation: 1\nTime: 0.00";
        }
    }

}
