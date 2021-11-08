using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GraphicsQuality
{
    LOW,
    MEDIUM,
    HIGH
}

public static class GraphicsQualityExtensions
{
    private static string[] qualityTexts = { "Low", "Medium", "High" };

    public static int getValue(this GraphicsQuality quality, float val)
    {
        return (int)(val * 2);
    }

    public static float getValue(this GraphicsQuality quality)
    {
        return (int)quality * 0.5f;
    }

    public static void setValue(this GraphicsQuality quality, float val)
    {
        quality = (GraphicsQuality) quality.getValue(val);
    }

    public static string text(this GraphicsQuality quality)
    {
        return qualityTexts[(int)quality];
    }
}

public class UIController : MonoBehaviour
{
    private static GraphicsQuality _GraphicsQuality = GraphicsQuality.HIGH;
    public static GraphicsQuality getGraphicsQuality() { return _GraphicsQuality; }

    [SerializeField] private GameObject startButton = null;
    [SerializeField] private GameObject settingsButton = null;
    [SerializeField] private GameObject settingsPanel = null;
    [SerializeField] private TextMeshProUGUI qualityText = null;
    [SerializeField] private Scrollbar qualityScroll = null;
    [SerializeField] private GameObject levelText = null;

    [SerializeField] private bool firstStart;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("graphics_quality"))
        {
            _GraphicsQuality = (GraphicsQuality) PlayerPrefs.GetInt("graphics_quality");
        }
        else
        {
            _GraphicsQuality = GraphicsQuality.HIGH;
            PlayerPrefs.SetInt("graphics_quality", (int)_GraphicsQuality);
        }

        applyGraphicsSettings();
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
        var player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<BallController>().activate();
        startButton.SetActive(false);
        settingsButton.SetActive(false);
        if (_GraphicsQuality == GraphicsQuality.LOW)
        {
            levelText.SetActive(true);
            levelText.GetComponent<TextMeshProUGUI>().text = "";
        }
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

    public void onQualityChange()
    {
        float val = qualityScroll.value;
        if (val < 0.25f)
            qualityText.text = "Low";
        else if (val < 0.75f)
            qualityText.text = "Medium";
        else
            qualityText.text = "High";
    }

    public void onClick_settingsButton()
    {
        enableSettingsPanel();
    }

    public void onClick_applyButton()
    {
        _GraphicsQuality = (GraphicsQuality)_GraphicsQuality.getValue(qualityScroll.value);
        
        PlayerPrefs.SetInt("graphics_quality", (int)_GraphicsQuality);

        applyGraphicsSettings();

        disableSettingsPanel();
    }

    public void onClick_cancelButton()
    {
        disableSettingsPanel();
    }

    private void enableSettingsPanel()
    {
        settingsPanel.SetActive(true);

        qualityScroll.value = _GraphicsQuality.getValue();
        qualityText.text = _GraphicsQuality.text();

        settingsButton.SetActive(false);
    }

    private void disableSettingsPanel()
    {
        settingsPanel.SetActive(false);
        settingsButton.SetActive(true);
    }

    private void applyGraphicsSettings()
    {
        // first 3 levels
        GameObject.FindGameObjectsWithTag("wall").ToList().ForEach(obj =>
            {
                Wall wall = obj.GetComponent<Wall>();
                wall.setColor(wall.color);
            }
        );

        LowQualityBinder lqBinder = GameObject.FindGameObjectWithTag("Player").GetComponent<LowQualityBinder>();
        MidQualityBinder mqBinder = GameObject.FindGameObjectWithTag("Player").GetComponent<MidQualityBinder>();

        switch (_GraphicsQuality)
        {
            case GraphicsQuality.LOW:
                lqBinder.enabled = true;
                mqBinder.enabled = false;
                Shader.EnableKeyword("QUALITY_LOW");
                Shader.DisableKeyword("QUALITY_MEDIUM");
                Shader.DisableKeyword("QUALITY_HIGH");
                break;
            case GraphicsQuality.MEDIUM:
                lqBinder.enabled = false;
                mqBinder.enabled = true;
                Shader.DisableKeyword("QUALITY_LOW");
                Shader.EnableKeyword("QUALITY_MEDIUM");
                Shader.DisableKeyword("QUALITY_HIGH");
                break;
            case GraphicsQuality.HIGH:
                lqBinder.enabled = false;
                mqBinder.enabled = false;
                Shader.DisableKeyword("QUALITY_LOW");
                Shader.DisableKeyword("QUALITY_MEDIUM");
                Shader.EnableKeyword("QUALITY_HIGH");
                break;
            default:
                break;
        }
    }

    private void OnApplicationQuit()
    {
        clearLevels();
    }
}
