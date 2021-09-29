using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private bool firstTime = true;
    public static bool gameIsStopped = true;
    private readonly int levelOffset = 2;

    public int currentLevel { get; private set; } = 1;
    [HideInInspector]
    public bool shouldCreateNewLevel;
    private float deltaTime;
    private Level lastCreatedLevel;
    //[SerializeField]
    //private Text fpsText = null;
    [SerializeField]
    private GameObject finisherObj = null;
    [SerializeField]
    private GameObject startButton = null;

    private Queue<Color> levelColors;

    public void Start()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        Application.targetFrameRate = -1;
#elif UNITY_ANDROID
        Application.targetFrameRate = 60;
        Input.gyro.enabled = true;
#endif
        //PlayerPrefs.DeleteAll();
        if (!PlayerPrefs.HasKey("highscore"))
        {
            PlayerPrefs.SetInt("highscore", 0);
            PlayerPrefs.SetInt("highscore_level", 0);
        }

        if (firstTime)
        {
            Util.createPointTexture();
            Debug.LogWarning("CREATED_PT_TEX");
        }

        levelColors = new Queue<Color>();
        Wall.SPAWN_Z = Wall.SPAWN_Z_DEFAULT;
        shouldCreateNewLevel = false;
        createFirstLevels();
        currentLevel = 1;
        firstTime = false;
    }

    void Update()
    {
        if (gameIsStopped)
            return;

        //deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        //float fps = 1.0f / deltaTime;
        //fpsText.text = fps.ToString();
        Wall.SPAWN_Z = lastCreatedLevel.walls[0].transform.position.z + 2;

        // put finisherObj between this and next level so that
        // if player misses the hole, trigger happens immediately
        // 2 is the Z distance between 2 levels
        finisherObj.transform.setPositionZ(Wall.SPAWN_Z - 5);
        
        if (shouldCreateNewLevel)
        {
            shouldCreateNewLevel = false;
            var color = Util.randColor();
            var level = createLevel(currentLevel + levelOffset);
            level.setColorAndActivateLevel(color);
            levelColors.Enqueue(color);
            lastCreatedLevel = level;
        }
    }
    private void createFirstLevels()
    {
        var firstLevels = new List<Level> { createLevel(1), createLevel(2), createLevel(3) };

        for (int i = 0; i < firstLevels.Count - 1; i++)
        {
            firstLevels[i].walls.ForEach(wall => wall.transform.position -= (2 - i) * new Vector3(0, 0, 2));
        }

        foreach (var level in firstLevels)
        {
            var color = Util.randColor();
            level.setColor(color);
            levelColors.Enqueue(color);
        }

        lastCreatedLevel = firstLevels[2];
        Shader.SetGlobalColor("_LevelColor", levelColors.Dequeue());
    }

    public void levelUp()
    {
        shouldCreateNewLevel = true;
        currentLevel++;
        Shader.SetGlobalColor("_LevelColor", levelColors.Dequeue());
    }
    private Level createLevel(int currentLevel)
    {
        GameObject levelObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        levelObj.name = "LEVEL_" + currentLevel;
        levelObj.tag = "level";
        levelObj.transform.position = new Vector3(0.5f, -0.5f, Wall.SPAWN_Z);
        levelObj.GetComponent<Renderer>().enabled = false;
        Level level = levelObj.AddComponent<Level>().init(currentLevel);
        return level;
    }
    public IEnumerator endGame()
    {
        gameIsStopped = true;
        var levels = new List<GameObject>(GameObject.FindGameObjectsWithTag("level"));
        levels.ForEach(level => level.GetComponent<Level>().destroy());
        yield return new WaitForSeconds(3);
        startButton.SetActive(true);
    }
}
