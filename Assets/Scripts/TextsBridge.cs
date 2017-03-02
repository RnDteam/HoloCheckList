using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProjectData
{
	public TextData[] texts;
	public string[] tasks;
}

[Serializable]
public class TextData
{
	public string name;
	public string text;
}

public class TextsBridge
{
	private static Dictionary<string, string> Texts = new Dictionary<string, string>();
	private static string[] Tasks = new string[0];

	private static bool loadedTexts = false;

	public static void LoadTexts()
	{
		TextAsset textFile = Resources.Load<TextAsset>("ProjectData");
		ProjectData projectData = JsonUtility.FromJson<ProjectData>(textFile.text);

		Texts = new Dictionary<string, string>();
		foreach (TextData textData in projectData.texts)
		{
			Texts.Add(textData.name, textData.text);
		}

		Tasks = (string[])projectData.tasks.Clone();

		loadedTexts = true;
	}

	public static void CreateTexts()
	{
		ProjectData projectData = new ProjectData();
		projectData.texts = new TextData[2];
		TextData textD1 = new TextData();
		textD1.name = "t1";
		textD1.text = "v1";
		TextData textD2 = new TextData();
		textD2.name = "t2";
		textD2.text = "v2";
		projectData.texts[0] =textD1;
		projectData.texts[1] =textD2;

		projectData.tasks = new string[2];
		projectData.tasks[0] = "task1";
		projectData.tasks[1] = "task2";
		Debug.Log(JsonUtility.ToJson(projectData));
	}

	public static string GetText(string name)
	{
		if (!loadedTexts)
		{
			LoadTexts();
		}
		if (Texts.ContainsKey(name))
		{
			return Texts[name];
		}
		return "";
	}

	public static string[] GetTasks()
	{
		if (!loadedTexts)
		{
			LoadTexts();
		}
		return Tasks;
	}
}
