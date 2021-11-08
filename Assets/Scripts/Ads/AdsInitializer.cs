using System.IO;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] string _androidGameId = "";
    [SerializeField] string _iOsGameId = "";
    [SerializeField] bool _testMode = true;
    [SerializeField] bool _enablePerPlacementMode = true;
    private string _gameId = "";

    void Awake()
    {
        InitializeAds();
    }

    private void loadGameIds()
    {
        string path = $"Assets/Scripts/Ads/App_ids.id";
        if (File.Exists(path))
        {
            try
            {
                var reader = new StreamReader(path);
                _androidGameId = reader.ReadLine();
                _iOsGameId = reader.ReadLine();
                Debug.Log($"{_androidGameId}\n" +
                    $"{_iOsGameId}");
                reader.Close();

            }
            catch (System.Exception)
            {
                Debug.LogError("2 lines text named \"App_ids.id\" that contains app ids should be like this ->\n" +
                    "androidAppId\n" +
                    "iosAppId");
                throw;
            }
        }
        else
        {
            Debug.LogError($"You should have some text file and path should be \"{path}\"");
            return;
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