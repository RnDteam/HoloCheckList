using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmallChecklistManager : MonoBehaviour
{

    public List<string> Tasks = new List<string>();
    public List<Color> Colors = new List<Color>();
    public List<Color> TextColors = new List<Color>();
    public GameObject TaskPrefab;
    public float distanceBetweenTasks;
    private Vector3 position = Vector3.zero;
    private int checkAnimationLoops;

    private int currentTask = 0;

    private struct Task
    {
        public string strText;
        public bool bIsChecked;
    }
    private List<GameObject> shownTasks;
    private List<Task> lstTasks;
    private bool lastTaskComplete;
    public Text taskNumberText;

    void Start()
    {
        distanceBetweenTasks = TaskPrefab.GetComponent<RectTransform>().rect.height;
        taskNumberText.text = string.Format("{0}/{1}", currentTask + 1, Tasks.Count);
        checkAnimationLoops = 0;
        lstTasks = new List<Task>();
        shownTasks = new List<GameObject>();

        // Create task list
        for (int nTaskIndex = 0; nTaskIndex < Tasks.Count; nTaskIndex++)
        {
            Task t;
            t.bIsChecked = false;
            t.strText = Tasks[nTaskIndex];
            lstTasks.Add(t);
        }

        // Manage shown tasks
        shownTasks.Add(CreateEmptyTask(true));

        for (int nTaskIndex = 0; nTaskIndex < 3; nTaskIndex++)
        {
            shownTasks.Add(CreateTask(nTaskIndex + 1, "Task" + (nTaskIndex + 1), Colors[0], TextColors[0]));
            shownTasks[nTaskIndex + 1].transform.FindChild("Toggle").Find("Label").GetComponent<Text>().text = Tasks[nTaskIndex];
        }

        ChangeColor(shownTasks[1], Colors[1], TextColors[1]);
    }

    private GameObject CreateEmptyTask(bool bIsTop)
    {
        GameObject goTask = Instantiate(TaskPrefab, transform);
        goTask.transform.FindChild("Toggle").FindChild("Label").GetComponent<Text>().text = string.Empty;

        goTask.name = "EmptyTask";
        goTask.transform.localScale = Vector3.one;
        ChangeColor(goTask, Colors[0], TextColors[0]);
        int nTaskIndex = bIsTop ? 0 : 4;
        goTask.transform.localPosition = new Vector3(0, -nTaskIndex * distanceBetweenTasks, 0);
        goTask.transform.localRotation = Quaternion.identity;
        goTask.AddComponent<InteractableTask>();

        return goTask;
    }

    private void MarkTask(int nCurrentTask, bool bIsChecked)
    {
        Task t = lstTasks[currentTask];
        t.bIsChecked = bIsChecked;
        lstTasks[currentTask] = t;

        shownTasks[1].transform.FindChild("Toggle").GetComponent<Toggle>().isOn = bIsChecked;
    }

    public void Check()
    {
        if (!lastTaskComplete)
        {
            checkAnimationLoops++;

            if (checkAnimationLoops == 1)
            {
                StartCoroutine(CheckMove());
            }
        }
        else
        {
            if (checkAnimationLoops > 0)
            {
                StartCoroutine(CheckMove());
            }
        }
    }

    public void Uncheck()
    {
        if (currentTask > 0)
        {
            currentTask--;
            StartCoroutine(UncheckMove());
        }

        if (lastTaskComplete)
        {
            lastTaskComplete = false;
        }
    }

    private IEnumerator UncheckMove()
    {
        // TODO add currentTask > 0
        //while (checkAnimationLoops > 0)
        //{
        Task t = lstTasks[currentTask];
        t.bIsChecked = false;
        lstTasks[currentTask] = t;
        shownTasks[0].transform.FindChild("Toggle").GetComponent<Toggle>().isOn = false;

        yield return new WaitForSeconds(1f);

        Vector3 offsetVec = new Vector3(0, -1, 0);
        int nCurrentTask = 1;
        ChangeColor(shownTasks[nCurrentTask], Colors[0], TextColors[0]);
        ChangeColor(shownTasks[nCurrentTask - 1], Colors[1], TextColors[1]);
        taskNumberText.text = string.Format("{0}/{1}", currentTask + 1, Tasks.Count);
        yield return new WaitForSeconds(1f);

        // Move up all tasks - starting from index 1
        for (int i = 0; i < 30; i++)
        {
            for (int nTaskNumber = 0; nTaskNumber < 4; nTaskNumber++)
            {
                shownTasks[nTaskNumber].transform.localPosition += offsetVec;
            }

            yield return new WaitForSeconds(0.03f);
        }

        yield return new WaitForSeconds(0.5f);

        GameObject g;

        if (currentTask > 0)
        {
            g = CreateTask(0, "Task" + currentTask, Colors[0], TextColors[0]);
        }
        else
        {
            g = CreateEmptyTask(false);
        }
        //MarkTask(0, lstTasks[currentTask - 1].bIsChecked);

        // Remove last task
        shownTasks.Insert(0, g);
        GameObject.Destroy(shownTasks[4]);
        shownTasks.RemoveAt(4);

        //checkAnimationLoops--;
        //}
    }

    private IEnumerator CheckMove()
    {
        while (checkAnimationLoops > 0 && !lastTaskComplete)
        {
            Task t = lstTasks[currentTask];
            t.bIsChecked = true;
            lstTasks[currentTask] = t;

            MarkTask(currentTask, true);

            currentTask = currentTask < Tasks.Count ? currentTask + 1 : currentTask;

            if (currentTask >= Tasks.Count)
                lastTaskComplete = true;

            yield return new WaitForSeconds(1f);

            Vector3 offsetVec = new Vector3(0, 1, 0);
            int nShownTask = 1;
            ChangeColor(shownTasks[nShownTask], Colors[0], TextColors[0]);
            ChangeColor(shownTasks[nShownTask + 1], Colors[1], TextColors[1]);

            taskNumberText.text = string.Format("{0}/{1}", !lastTaskComplete ? currentTask + 1 : currentTask, Tasks.Count);
            yield return new WaitForSeconds(1f);

            GameObject g;

            if (!lastTaskComplete)
            {
                g = CreateTask(4, "Task" + (currentTask + 1), Colors[0], TextColors[0]);
            }
            else
            {
                g = CreateEmptyTask(false);
            }

            yield return new WaitForSeconds(0.5f);

            // Remove first task
            GameObject.Destroy(shownTasks[0]);

            // Move up all tasks - starting from index 1
            for (int i = 0; i < 30; i++)
            {
                for (int nTaskNumber = 1; nTaskNumber < 4; nTaskNumber++)
                {
                    shownTasks[nTaskNumber].transform.localPosition += offsetVec;
                }

                g.transform.localPosition += offsetVec;

                yield return new WaitForSeconds(0.03f);
            }

            shownTasks.RemoveAt(0);
            shownTasks.Add(g);

            checkAnimationLoops--;
        }
    }

    public GameObject CreateTask(int nTaskPosition, string name, Color backColor, Color textColor)
    {
        GameObject goTask = Instantiate(TaskPrefab, transform);
        if(0 < currentTask && currentTask < Tasks.Count - 2)
        {
            int wantedTaskIndex = nTaskPosition == 0 ? currentTask - 1 : currentTask + 2;
            goTask.transform.FindChild("Toggle").FindChild("Label").GetComponent<Text>().text = Tasks[wantedTaskIndex];
            goTask.transform.FindChild("Toggle").GetComponent<Toggle>().isOn = lstTasks[wantedTaskIndex].bIsChecked;
        }
        else
        {
            goTask.transform.FindChild("Toggle").FindChild("Label").GetComponent<Text>().text = string.Empty;
        }

        goTask.name = "Task" + nTaskPosition;
        goTask.transform.localScale = Vector3.one;
        ChangeColor(goTask, backColor, textColor);

        goTask.transform.localPosition = new Vector3(0, -nTaskPosition * distanceBetweenTasks, 0);
        goTask.transform.localRotation = Quaternion.identity;
        
        goTask.AddComponent<InteractableTask>();

        return goTask;
    }

    public void ChangeColor(GameObject gameobject, Color backColor, Color textColor)
    {
        gameobject.transform.FindChild("Background").GetComponent<Image>().color = backColor;
        gameobject.transform.FindChild("Toggle").FindChild("Background").GetComponent<Image>().color = backColor;
        gameobject.transform.FindChild("Toggle").FindChild("Background").FindChild("Checkmark").GetComponent<Image>().color = textColor;
        gameobject.transform.FindChild("Toggle").FindChild("Label").GetComponent<Text>().color = textColor;
    }
}
