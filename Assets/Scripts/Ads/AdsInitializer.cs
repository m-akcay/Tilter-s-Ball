using System.IO;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Scripting;


public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{

    [SerializeField] string _androidGameId = "";
    [SerializeField] string _iOsGameId = "";
    [SerializeField] bool _testMode = true;
    [SerializeField] bool _enablePerPlacementMode = true;
    private string _gameId = "";
    
    private void Awake()
    {
        InitializeAds();
    }

    private void loadGameIds()
    {
        try
        {
            TextAsset gameIdText = Resources.Load("App_ids") as TextAsset;
            var lines = gameIdText.text.Split('\n');
            _androidGameId = lines[0].Substring(0, lines[0].Length - 1);
            _iOsGameId = lines[1];
        }
        catch (System.Exception)
        {
            Debug.LogError("Error loading app ids, ads won't work.");
            throw;
        }
    }

    
    public void InitializeAds()
    {
        loadGameIds();

        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOsGameId
            : _androidGameId;
        Advertisement.Initialize(_gameId, _testMode, _enablePerPlacementMode, this);
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
}