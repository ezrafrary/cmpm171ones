using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance;
    private bool _isInitalized = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
        _isInitalized = true;
    }


    public void GameStarted(int currentLevel)
    {
        if (!_isInitalized)
        {
            return;
        }
        CustomEvent myEvent = new CustomEvent("next_level")
        {
            {"level_index", currentLevel}
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
        AnalyticsService.Instance.Flush();
    }

    public void GameCompleted()
    {
        AnalyticsService.Instance.RecordEvent("GameCompleted");
        AnalyticsService.Instance.Flush();
    }

    public void PlayerKilled(string weaponName)
    {
        if (!_isInitalized)
        {
            return;
        }
        CustomEvent myEvent = new CustomEvent("player_killed")
        {
            {"weaponName", weaponName}
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
        AnalyticsService.Instance.Flush();
    }

}
