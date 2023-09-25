using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class Menu : MonoBehaviour
{

    public GameObject values;

    public TMP_InputField inputValue;
    public TMP_InputField inputValue2;
    public TMP_InputField inputValue3;


    public void Start()
    {
        values = GameObject.Find("Values");
    }

    public void Play()
    {
        int a = (inputValue.text != "") ? int.Parse(inputValue.text) : 25;
        int b = (inputValue2.text != "") ? int.Parse(inputValue2.text) : 20;
        float c = (inputValue3.text != "") ? float.Parse(inputValue3.text) : 60;

        if (a % 2 != 0)
            a++;

        values.GetComponent<wartosc>().setUser(a, b, c);
        SceneManager.LoadScene("SampleScene");
    }
}
