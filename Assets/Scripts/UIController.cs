using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject startButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
       
    }

    public void startGame()
    {
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
}
