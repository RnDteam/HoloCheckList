using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class TasksLoader : MonoBehaviour
{
    public List<string> Tasks = new List<string>();
    public List<Color> Colors = new List<Color>();
    public GameObject TaskPrefab;
    public string ObjectName = "Task";
    public float distance = 30f;

    public TextToSpeechManager TextToSpeech;

    public Dictionary<string, bool> TaskStatus = new Dictionary<string, bool>();

    private Vector3 position = Vector3.zero;
    private int counter = 0;

    void Start()
    {
        foreach (var task in Tasks)
        {
            var gameobject = Instantiate(TaskPrefab, transform);
            gameobject.name = ObjectName + counter.ToString();
            gameobject.transform.localScale = Vector3.one;

            var color = Colors[counter % Colors.Count];
            gameobject.transform.FindChild("Background").GetComponent<Image>().color = color;
            gameobject.transform.FindChild("Toggle").FindChild("Background").GetComponent<Image>().color = color;

            gameobject.transform.FindChild("Toggle").Find("Label").GetComponent<Text>().text = string.Format("{0}. {1}", counter + 1, task);
            gameobject.transform.localPosition = position;
            position = position - distance * transform.up;
            gameobject.AddComponent<InteractableTask>();
            counter++;

            TaskStatus.Add(gameobject.name, false);
        }
    }

    private string GetNextTask()
    {
        for (int i = 0; i < Tasks.Count; i++)
        {
            if (!TaskStatus.ElementAt(i).Value)
            {
                return Tasks[i];
            }
        }
        return string.Empty;
    }

    public void Check()
    {
        foreach (var task in TaskStatus)
        {
            if (!task.Value)
            {
                TaskStatus[task.Key] = true;
                Debug.Log(task.Key);
                transform.Find(task.Key).GetComponent<InteractableTask>().OnSelect();
                return;
            }
        }
    }

    public void Uncheck()
    {
        for (int i = TaskStatus.Count - 1; i >= 0; i--)
        {
            if (TaskStatus.ElementAt(i).Value)
            {
                var taskKey = TaskStatus.ElementAt(i).Key;
                TaskStatus[taskKey] = false;
                transform.Find(taskKey).GetComponent<InteractableTask>().OnSelect();
                return;
            }
        }
    }

    public void SayNextTask()
    {
        var text = GetNextTask();
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
        var taskKey = TaskStatus.ElementAt(index - 1).Key;
        TaskStatus[taskKey] = !TaskStatus[taskKey];
        transform.Find(taskKey).GetComponent<InteractableTask>().OnSelect();
    }
}

