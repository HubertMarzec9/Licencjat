using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wartosc : MonoBehaviour
{
    public int a = 20, b = 20;
    public float c = 20;

    void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void setUser(int a, int b, float c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
    }
}
