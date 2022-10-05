using UnityEngine;
using UnityEngine.Advertisements;
 
public class UnityAdsRewarded : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
	[SerializeField] string _androidAdUnitId = "Interstitial_Android";
	[SerializeField] string _iOsAdUnitId = "Interstitial_iOS";
	string _adUnitId;

	static UnityAdsRewarded _this = null;
	
	public static System.Action<string> onCompleted;

	public static void Show() => Advertisement.Show(_this?._adUnitId ?? "", _this);
	public static bool IsReady() => Advertisement.IsReady();
	void Awake()
	{
		_this = this;

	#if UNITY_ANDROID
		_adUnitId = _androidAdUnitId;
	#elif UNITY_IOS
		_adUnitId = _iOsAdUnitId;
	#endif

		UnityAdsInit.onInitialized += LoadAd;
	}
	void OnDestroy()
	{
		UnityAdsInit.onInitialized -= LoadAd;
	}

	void LoadAd()
	{
	// IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
		Debug.Log("Loading Ad: " + _adUnitId);
		Advertisement.Load(_adUnitId, this);
	}

	public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState) 
	{ 
		if(showCompletionState == UnityAdsShowCompletionState.COMPLETED)
		{
			onCompleted?.Invoke(adUnitId);
		}
	}
	// Implement Load Listener and Show Listener interface methods: 
	public void OnUnityAdsAdLoaded(string adUnitId)
	{
		// Optionally execute code if the Ad Unit successfully loads content.
	}

	public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
	{
		Debug.Log($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");
		// Optionally execute code if the Ad Unit fails to load, such as attempting to try again.
	}

	public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
	{
		Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
		// Optionally execute code if the Ad Unit fails to show, such as loading another ad.
	}

	public void OnUnityAdsShowStart(string adUnitId) { }
	public void OnUnityAdsShowClick(string adUnitId) { }
}
