using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class TasksLoader : MonoBehaviour
{
    public List<string> Tasks = new List<string>();
    public List<Color> Colors = new List<Color>();
    public List<Color> TextColors = new List<Color>();
    public GameObject TaskPrefab;
    public string ObjectName = "Task";
    public float distance = 30f;

    public TextToSpeechManager TextToSpeech;

    private int currentTask = 0;
    private bool lastTaskComplete = false;
    private Vector3 position = Vector3.zero;
    private List<GameObject> TaskGameObjects = new List<GameObject>();

    void Start()
    {
        for (int i=0; i < 3; i++)
        {
            var gameobject = Instantiate(TaskPrefab, transform);
            gameobject.name = ObjectName + i.ToString();
            gameobject.transform.localScale = Vector3.one;

            var color = Colors[i % Colors.Count];
            var textColor = TextColors[i % TextColors.Count];
            gameobject.transform.FindChild("Background").GetComponent<Image>().color = color;
            gameobject.transform.FindChild("Toggle").FindChild("Background").GetComponent<Image>().color = color;
            gameobject.transform.FindChild("Toggle").FindChild("Background").FindChild("Checkmark").GetComponent<Image>().color = textColor;
            gameobject.transform.FindChild("Toggle").FindChild("Label").GetComponent<Text>().color = textColor;

            gameobject.transform.localPosition = position;
            position = position - distance * transform.up;
            gameobject.AddComponent<InteractableTask>();
            
            TaskGameObjects.Add(gameobject);
        }
        UpdateTaskNames();

    }

    private void UpdateTaskNames()
    {
        var text = string.Empty;
        if (currentTask > 0)
        {
            text = string.Format("{0}. {1}", currentTask, Tasks.ElementAt(currentTask - 1));
            TaskGameObjects.ElementAt(0).transform.FindChild("Toggle").GetComponent<Toggle>().isOn = true;
        }
        else
        {
            TaskGameObjects.ElementAt(0).transform.FindChild("Toggle").GetComponent<Toggle>().isOn = false;
        }
        TaskGameObjects.ElementAt(0).transform.FindChild("Toggle").Find("Label").GetComponent<Text>().text = text;

        TaskGameObjects.ElementAt(1).transform.FindChild("Toggle").Find("Label").GetComponent<Text>().text = string.Format("{0}. {1}", currentTask + 1, Tasks.ElementAt(currentTask));

        text = string.Empty;
        if (currentTask < Tasks.Count - 1)
        {
            text = string.Format("{0}. {1}", currentTask + 2, Tasks.ElementAt(currentTask + 1));
        }
        TaskGameObjects.ElementAt(2).transform.FindChild("Toggle").Find("Label").GetComponent<Text>().text = text;

        if (lastTaskComplete)
        {
            TaskGameObjects.ElementAt(1).transform.FindChild("Toggle").GetComponent<Toggle>().isOn = true;
        } else
        {
            TaskGameObjects.ElementAt(1).transform.FindChild("Toggle").GetComponent<Toggle>().isOn = false;
        }
    }

    private string GetCurrentTask()
    {
        if (currentTask < Tasks.Count)
            return Tasks[currentTask];
        return string.Empty;
    }

    public void Check()
    {
        if (currentTask < Tasks.Count - 1)
        {
            currentTask++;
        }
        else
        {
            lastTaskComplete = true;
        }
        UpdateTaskNames();
    }

    public void Uncheck()
    {
        if (currentTask == Tasks.Count - 1 && lastTaskComplete)
        {
            lastTaskComplete = false;
        }
        else if (currentTask > 0)
        {
            currentTask--;
        }
        UpdateTaskNames();
    }

    public void SayCurrentTask()
    {
        var text = GetCurrentTask();
        if (text == string.Empty)
        {
            text = "All done. Good job.";
        }
        TextToSpeech.SpeakText(text);
    }

    public void Hide()
    {
        Debug.Log("Hide");
        gameObject.SetActive(false);
    }

    public void Show()
    {
        Debug.Log("Show");
        gameObject.SetActive(true);
    }

    public void SelectTask(int index)
    {
        //transform.Find(TaskStatus.ToArray()[index - 1].Key).GetComponent<InteractableTask>().Designate();

        //var taskKey = TaskStatus.ElementAt(index - 1).Key;
        //TaskStatus[taskKey] = !TaskStatus[taskKey];
        //transform.Find(taskKey).GetComponent<InteractableTask>().OnSelect();
    }
}

