using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject startButton;
    private bool firstStart;
    // Start is called before the first frame update
    void Start()
    {
        firstStart = true;
    }

    public void startGame()
    {
        if (!firstStart)
        {
            clearLevels();
            GameObject.Find("GameManagerObject").GetComponent<LevelManager>().Start();
            GameObject.Find("Ball").GetComponent<BallController>().Start();
        }
        firstStart = false;
        Debug.Log("START");
        LevelManager.gameIsStopped = false;
        var levels = new List<GameObject>(GameObject.FindGameObjectsWithTag("level"));
        levels.ForEach(level => level.GetComponent<Level>().activate());
        GameObject.FindGameObjectWithTag("Player").GetComponent<BallController>().activate();
        startButton.SetActive(false);
    }

    public void stopGame()
    {
        LevelManager.gameIsStopped = true;
        var levels = new List<GameObject>(GameObject.FindGameObjectsWithTag("level"));
        levels.ForEach(level => level.GetComponent<Level>().deactivate());
        GameObject.FindGameObjectWithTag("Player").GetComponent<BallController>().deactivate();
    }

    private void clearLevels()
    {
        var dummyObjs = new List<GameObject>(GameObject.FindGameObjectsWithTag("level"));
        dummyObjs.AddRange(GameObject.FindGameObjectsWithTag("wall"));
        dummyObjs.AddRange(GameObject.FindGameObjectsWithTag("hole"));
        for (int i = 0; i < dummyObjs.Count; i++)
        {
            Destroy(dummyObjs[i]);
        }
    }

    private void OnApplicationQuit()
    {
        clearLevels();
    }
}
