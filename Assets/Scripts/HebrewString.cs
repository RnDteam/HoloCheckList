using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HebrewString : MonoBehaviour {

    private Text textToHebrew;

	// Use this for initialization
	void Start () {
        textToHebrew = GetComponent<Text>();
        string strHebrew;

        if(textToHebrew != null)
        {
            strHebrew = string.Empty;

            string[] words = textToHebrew.text.Split(' ');
            for (int wordIndex = words.Length - 1; wordIndex >= 0; wordIndex--)
            {
                strHebrew += ReverseHebrewName(words[wordIndex]) + ' ';
            }

            textToHebrew.text = strHebrew;
        }
	}

    string Reverse(string s)
    {
        char[] charArray = s.ToCharArray();
        return new string(charArray.Reverse().ToArray());
    }

    string ReverseHebrewName(string s)
    {
        if (s.Any(c => IsHebrew(c)))
        {
            return Reverse(s);
        }
        return s;
    }

    bool IsHebrew(char c)
    {
        return "אבגדהוזחטיכלמנסעפצקרשתךםןףץ".Contains(c);
    }
}
