using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ByteBrewSDK;
using Facebook.Unity;
public class AnalyticsManager : MonoBehaviour
{
    public enum AnalyticsEnvironment { Test, Production }
    [SerializeField] AnalyticsEnvironment activeAnalyticsEnvironment = AnalyticsEnvironment.Test;
    
    private void Start() {
        FB.Init();
        ByteBrew.InitializeByteBrew();
        Debug.Log("Analytics System | Initialized");
    }

    //TODO:
    // - log store entry
    // - log main nav menu navigation (section visited)
    // - log first bad merge - 
    // - log fist / second log out - how menu levels completed
    // - log if user returned after notification 
    // - log separate feeding events

    private void OnEnable() {
        Level.onStart += LogLevelStartData;
        Level.onFinished += LogLevelSummarySuccess;
        Game.onLevelRestart += LogLevelSummaryFail;
    }
    private void OnDisable() {
        Level.onStart -= LogLevelStartData;
        Level.onFinished -= LogLevelSummarySuccess;
        Game.onLevelRestart -= LogLevelSummaryFail;
    }

    void LogLevelStartData(Level sender) => LogEvent(ByteBrewProgressionTypes.Started, sender);
    void LogLevelSummarySuccess(Level sender) => LogEvent(ByteBrewProgressionTypes.Completed, sender);
    void LogLevelSummaryFail(Level sender) => LogEvent(ByteBrewProgressionTypes.Failed, sender);

    void LogEvent(ByteBrewProgressionTypes progressionType, Level sender){
        ByteBrew.NewProgressionEvent(progressionType, GetEventEnvironmentName(activeAnalyticsEnvironment, sender), GetLevelID(sender));
        
        // if (activeAnalyticsEnvironment == AnalyticsEnvironment.Production) return;
        
        Debug.Log(
            "<color=cyan> Analytics Event " + _div +
            GetEventEnvironmentName(activeAnalyticsEnvironment, sender) + _div +
            GetProgressionTypeName(progressionType) + _div +
            GetLevelID(sender)
            + "</color>"
        );
    }

    #region formatters

        const string _div = " | ";
        string GetProgressionTypeName(ByteBrewProgressionTypes progressionType) => progressionType.ToString();
        string GetEventEnvironmentName(AnalyticsEnvironment environment, Level level = null){
            if (environment == AnalyticsEnvironment.Test) return "TestEnvironment";

            return level.GetMode().ToString();
        }

        string GetLevelID(Level level) => level.GetMode() switch{
            Level.Mode.Feeding => level.visitsCnt.ToString("000"),
            Level.Mode.Clearing => level.visitsCnt.ToString("000"),
            _ => level.locationIdx.ToString("0000")
        };

    #endregion
}
