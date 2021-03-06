﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ProjectData
{
	public Header[] headers;
	public Card[] cards;
}

[Serializable]
public class Header
{
	public string name;
	public string text;
}

[Serializable]
public class Card
{
    public string name;
    public string folder;
    public Task[] tasks;
    public string start;
    public string finish;
}

[Serializable]
public class Task
{
    public string instruction;
    public string hasExtraInfo;
    public bool signedTask;
    public string file;

    [NonSerialized]
    public bool isAlreadySigned = false;
}

public class TextsBridge
{

	private static Dictionary<string, string> headers = new Dictionary<string, string>();
    private static Card[] cards;
    
    // json file name to load
    public static string TASK_FILE_NAME_TO_LOAD = "TasksData";
    public static void SetTaskFileName(bool isFullPreflight)
    {
        TASK_FILE_NAME_TO_LOAD = isFullPreflight ? "TasksData" : "TasksData-shorter";
        LoadCards();
    }
    private static bool isTasksAreLoaded = false;
    
    public static bool IsTasksLoaded()
    {
        return isTasksAreLoaded;
    }

	public static Card[] LoadCards()
	{
        TextAsset textFile = Resources.Load<TextAsset>(TASK_FILE_NAME_TO_LOAD);
        ProjectData projectData = JsonUtility.FromJson<ProjectData>(textFile.text);

        headers = new Dictionary<string, string>();
        foreach (Header header in projectData.headers)
        {
            headers.Add(header.name, header.text);
        }

        cards = (Card[])projectData.cards.Clone();

        string strHebrew;
        foreach(Card c in cards)
        {
            for (int nTaskIndex = 0; nTaskIndex < c.tasks.Length; nTaskIndex++)
            {
                strHebrew = string.Empty;

                string[] wordsInTask = c.tasks[nTaskIndex].instruction.Split(' ');
                for (int wordIndex = wordsInTask.Length - 1; wordIndex >= 0; wordIndex--)
                {
                    strHebrew += ReverseHebrewName(wordsInTask[wordIndex]) + ' ';
                }

                c.tasks[nTaskIndex].instruction = strHebrew;
            }

            c.name = Reverse(c.name);
        }

        isTasksAreLoaded = true;

        return cards;
    }

    static string Reverse(string s)
    {
        char[] charArray = s.ToCharArray();
        return new string(charArray.Reverse().ToArray());
    }

    public static string ReverseHebrewName(string s)
    {
        if (s.Any(c => IsHebrew(c)))
        {
            return Reverse(s);
        }
        return s;
    }

    static bool IsHebrew(char c)
    {
        return "אבגדהוזחטיכלמנסעפצקרשתךםןףץ".Contains(c);
    }

    //public static void CreateTexts()
    //{
    //	ProjectData projectData = new ProjectData();
    //	projectData.texts = new TextData[2];
    //	TextData textD1 = new TextData();
    //	textD1.name = "t1";
    //	textD1.text = "v1";
    //	TextData textD2 = new TextData();
    //	textD2.name = "t2";
    //	textD2.text = "v2";
    //	projectData.texts[0] =textD1;
    //	projectData.texts[1] =textD2;

    //	projectData.tasks = new string[2];
    //	projectData.tasks[0] = "task1";
    //	projectData.tasks[1] = "task2";
    //	Debug.Log(JsonUtility.ToJson(projectData));
    //}

    public static string GetText(string name)
	{
		if (!isTasksAreLoaded) LoadCards();

        return headers.ContainsKey(name) ? headers[name] : string.Empty;
	}

	public static Card[] GetCards()
	{
		if (!isTasksAreLoaded) LoadCards();

		return cards;
	}
}
