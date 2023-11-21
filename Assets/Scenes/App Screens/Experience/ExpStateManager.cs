using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Google.XR.ARCoreExtensions;
using System.ComponentModel;
using Unity.VisualScripting;
using System;
using UnityEngine.InputSystem.LowLevel;
using DilmerGames.Core.Singletons;


public enum AppState
{
    MapOpen,
    AtLocation,
    AssetViewing,
    AllAssetsViewed,
    NavigatingToNext
}

public class ExpStateManager : MonoBehaviour
{

    [System.Serializable]
    public struct Asset
    {
        public GameObject assetObject;
    }

    [System.Serializable]
    public class AssetList
    {
        public List<Asset> assets = new List<Asset>();
    }

    public AppState currentState;
    public Button startButton;
    public Transform playerPosition;
    public List<Location> locations;
    private int currentLocationIndex = 0;
    public GameObject pathway;
    //public List<GameObject>[] assets; // List of assets (videos, photos, newspapers, etc.)

    public AssetList[] assetsArray;
    //public List<Asset>[] assets;
    public AudioSource audioSource;
    public ARCoreExtensions arCoreExtensions;
    public bool atLocation;
    GeospatialMode geospatialMode = GeospatialMode.Enabled;

    private bool[] assetsViewed;
    
    private static ExpStateManager _instance;

    public static ExpStateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // This will only occur if the singleton is not attached to any GameObject in the scene.
                Debug.LogError("MySingleton instance is not initialized yet.");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            // Destroy the duplicate instance and keep the first one.
            Destroy(gameObject);
            return;
        }

        _instance = this;

        // Optional: if you want this singleton to persist across scene loads.
        DontDestroyOnLoad(gameObject);
    }




    private HashSet<GameObject> viewedAssets = new HashSet<GameObject>();



    public Canvas instCanvas;
    public Canvas VideoCanvas;

    void Start()
    {
        
        instCanvas.gameObject.SetActive(true);
        VideoCanvas.gameObject.SetActive(false);
        
        
        currentState = AppState.MapOpen;
        UpdateState();
    }

    void Update()
    {
        switch (currentState)
        {
            case AppState.MapOpen:
                if (atLocation)
                {
                    ChangeState(AppState.AtLocation);
                }
                break;
            case AppState.AtLocation:
                
                break;
            case AppState.AssetViewing:
                // Check if all assets have been viewed
                if (assetsViewed[currentLocationIndex])
                {
                    ChangeState(AppState.AllAssetsViewed);
                }
                //if (AllAssetsViewed())
                //{
                //    ChangeState(AppState.AllAssetsViewed);
                //}
                break;
            case AppState.AllAssetsViewed:
                // Waiting for user to start navigating

                break;
            case AppState.NavigatingToNext:

                // Handle navigation logic
                break;
        }
    }

    public void ViewAsset(GameObject asset)
    {
        // Logic when an asset is viewed
        

        // Logic if the player clicks on the asset once, it is considered as viewed. Add some inspector variables here..
        if (!viewedAssets.Contains(asset))
        {
            viewedAssets.Add(asset);
            // Additional logic for handling the viewed asset (e.g., play video, show image)
        }

        // Check if all assets are viewed after viewing this asset
        if (AllAssetsViewed())
        {
            ChangeState(AppState.AllAssetsViewed);
        }
    }

    public void NextLocation()
    {
        // Logic to navigate to the next location
        // Enable all pointers in the area to the next location
        ChangeState(AppState.NavigatingToNext);
    }

    private void ChangeState(AppState newState)
    {
        currentState = newState;
        UpdateState();
    }

    private void UpdateState()
    {
        switch (currentState)
        {
            case AppState.MapOpen:
                // Initial state logic
                break;
            case AppState.AtLocation:
                // Enable interaction with assets
                foreach (var assetList in assetsArray[currentLocationIndex].assets)
                {
                    assetList.assetObject.SetActive(true);
                    // Add listeners or interaction logic for each asset
                }
                ChangeState(AppState.AssetViewing);
                break;
            case AppState.AssetViewing:
                // Waiting for all assets to be viewed
                break;
            case AppState.AllAssetsViewed:
                instCanvas.gameObject.SetActive(true);
                startButton.gameObject.SetActive(true);
                startButton.GetComponentInChildren<Text>().text = "Next";
                
                break;
            case AppState.NavigatingToNext:
                pathway.SetActive(true);
                //Additional navigation logic

                //ResetAssets();
                break;
        }
    }

    private bool PlayerReachedPosition()
    {
        if (currentLocationIndex >= locations.Count)
        {
            return false; // No more locations
        }

        Location targetLocation = locations[currentLocationIndex];

        // Get the current geospatial pose
        //GeospatialPose geospatialPose = arCoreExtensions.SessionOrigin.GetComponent<ARCoreExtensions>().CurrentFrame.Earth.GetGeospatialPose();

        GeospatialPose geospatialPose = arCoreExtensions.SessionOrigin.GetComponent<GeospatialPose>();

        double playerLat = geospatialPose.Latitude;
        double playerLon = geospatialPose.Longitude;

        if (IsWithinThreshold(playerLat, playerLon, targetLocation.getLat(), targetLocation.getLong()))
        {
            currentLocationIndex++; // Move to the next location
            return true;
        }

        return false;
    }

    private bool IsWithinThreshold(double lat1, double lon1, double lat2, double lon2, double threshold = 10f) // Threshold in meters
    {
        float distance = CalculateDistance(lat1, lon1, lat2, lon2);
        return distance <= threshold;
    }

    private float CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        // Calculate the distance between two sets of coordinates
        // You can use Haversine formula or Unity's LocationService for this
        // This is a placeholder for the actual distance calculation

        

        return 0f;
    }


    private bool playerToLocationDistance(Transform location, Transform player)
    {

        if (Vector3.Distance(location.position, player.position) < 30f) 
        {
            return true;
        }
        return false;
    }


    private void GetCurrentGPSLocation(out float latitude, out float longitude)
    {
        // Implement the logic to get the current GPS location
        // This could be from Unity's LocationService or another GPS plugin
        latitude = 0f; // Placeholder
        longitude = 0f; // Placeholder
    }


    private bool AllAssetsViewed()
    {
        return viewedAssets.Count == assetsArray[currentLocationIndex].assets.Count;
    }

    private void ResetAssets()
    {
        // Reset the assets for the next level
        viewedAssets.Clear();
        foreach (var assetList in assetsArray[currentLocationIndex].assets)
        {
            assetList.assetObject.SetActive(false);
            // Reset each asset (e.g., stop video, hide image)
        }
    }

    public void OnStartButtonClick()
    {
       
            if (playerToLocationDistance(locations[currentLocationIndex].firstAsset.transform, playerPosition))
            {
                atLocation = true;
            }

            instCanvas.gameObject.SetActive(false);


       
    }

    //public void OnNextClickButton()
    //{
    //    startButton.GetComponentInChildren<Text>().text = "Next";
    //}


}
