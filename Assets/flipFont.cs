using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;

public class flipFont : MonoBehaviour
{

    Text myText; //You can also make this public and attach your UI text here.

    string individualLine = ""; //Control individual line in the multi-line text component.

    public string sampleString;

    void Awake()
    {
        myText = GetComponent<Text>();
    }
    void Start()
    {
        //List<string> listofWords = sampleString.Split(' ').ToList(); //Extract words from the sentence

        //foreach (string s in listofWords)
        //{
        //        myText.text += Reverse(s); //Add a new line feed at the end, since we cannot accomodate more characters here.
        //}
        myText.text = Reverse(sampleString);
    }

    public static string Reverse(string s)
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }
}
