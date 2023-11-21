using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Video;

public class ExperienceManager : MonoBehaviour
{
    [System.Serializable]
    public class LocationAssets
    {
        public List<GameObject> assets; // List of assets for each location
        public HashSet<GameObject> interactedAssets; // Track interacted assets
        public Transform locationAnchor; // Anchor or marker for this location
        public string locationName;
        public string locationText;

        public LocationAssets()
        {
            interactedAssets = new HashSet<GameObject>();
        }
    }

    public ARRaycastManager arRaycastManager; // AR Raycast Manager
    public List<LocationAssets> locationAssets; // List to hold assets for each location
    public GameObject[] directionalArrow; // Directional arrow to guide player
    public float proximityThreshold = 5.0f; // Proximity threshold to change location
    public float assetThreshold;
    private bool started;

    private int currentLocation = 0; // Starting location index
    private Transform playerTransform; // Player's transform
    public Canvas mainCanvas;
    public Canvas secondaryCanvas;
    public TMP_Text InfoText;
    public TMP_Text locationText;
    public VideoPlayer videoPlayer;
    //private string videoUrl;

    void Start()
    {
        mainCanvas.gameObject.SetActive(true);
        playerTransform = this.transform; // Assuming this script is attached to the player
        for(int i=0; i<directionalArrow.Length;i++)
        {
            directionalArrow[i].SetActive(false);
        }
        
        started = false;
        secondaryCanvas.gameObject.SetActive(false);
        videoPlayer.gameObject.SetActive(false);

    }

    void Update()
    {

        if(started)
        // Detect both touch and mouse input
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Touch touch = Input.GetTouch(0);
            CheckAssetInteraction(touch.position);
        }
        else if (Input.GetMouseButtonDown(0))
        {
            CheckAssetInteraction(Input.mousePosition);
        }

        // Check if all assets in the current location have been interacted with
        if (locationAssets[currentLocation].interactedAssets.Count == locationAssets[currentLocation].assets.Count)
        {
            Debug.Log("Level Complete- " + currentLocation);
            Debug.Log(Vector3.Distance(playerTransform.position, locationAssets[currentLocation + 1].locationAnchor.position));
            secondaryCanvas.gameObject.SetActive(true);
            CheckPlayerProximityAndGuide();
        }
    }

    void CheckPlayerProximityAndGuide()
    {
        float distanceToNextLocation = Vector3.Distance(playerTransform.position, locationAssets[currentLocation+1].locationAnchor.position);

        if (distanceToNextLocation <= proximityThreshold)
        {
            MoveToNextLocation();
            //if(currentLocation < locationAssets.Count - 1)
            //directionalArrow[currentLocation].SetActive(false);
        }
        else
        {
            // Update arrow direction
            //if(currentLocation < locationAssets.Count - 1)
            //directionalArrow[currentLocation].SetActive(true);
            //directionalArrow[currentLocation].transform.LookAt(locationAssets[currentLocation].locationAnchor);
        }
    }

    void MoveToNextLocation()
    {
        if(currentLocation < locationAssets.Count - 1)
        {
            currentLocation = (currentLocation + 1);
            locationAssets[currentLocation].interactedAssets.Clear(); // Reset for the new location
            secondaryCanvas.gameObject.SetActive(false);

            // Set everything on the canvas here

            locationText.SetText(locationAssets[currentLocation].locationName);
            InfoText.SetText(locationAssets[currentLocation].locationText);
            mainCanvas.gameObject.SetActive(true);
            started = false;
        }
        else
        {
            OnGameCompleted();
        }

    }

    void OnGameCompleted()
    {
        // Logic for game completion
        Debug.Log("All locations completed!");
        // Additional code for ending the game or transitioning to another scene

        started = false;
        mainCanvas.gameObject.SetActive(true);
        InfoText.SetText("You have now completed the experience. Thank you!");
        locationText.SetText("Completed Okemah Community");
        // play some audio, etc.
    }


    void CheckAssetInteraction(Vector2 screenPosition)
    {
        Ray ray;
        if (Application.isMobilePlatform)
        {
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            if (arRaycastManager.Raycast(screenPosition, hits, TrackableType.FeaturePoint))
            {
                ARRaycastHit hit = hits[0];
                Vector3 hitPosition = hit.pose.position;

                foreach (var asset in locationAssets[currentLocation].assets)
                {
                    if (Vector3.Distance(asset.transform.position, hitPosition) < assetThreshold)
                    {
                        AssetInteraction(asset);
                        break;
                    }
                }
            }
        }
        else // Desktop platform
        {
            ray = Camera.main.ScreenPointToRay(screenPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                foreach (var asset in locationAssets[currentLocation].assets)
                {
                    if (hit.transform.gameObject.tag == asset.tag)
                    {
                        AssetInteraction(asset);
                        break;
                    }
                }
            }
        }
    }

    void AssetInteraction(GameObject asset)
    {
        Debug.Log("Interacted with asset: " + asset.name + " of level - " + currentLocation);

        if (videoPlayer.isActiveAndEnabled)
        {
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Pause();
            }
            else if(!videoPlayer.isPlaying)
            {
                videoPlayer.Stop();
                videoPlayer.gameObject.SetActive(false);
            }
        }
        else
        {
            videoPlayer.gameObject.SetActive(true);
            videoPlayer.url = asset.gameObject.GetComponent<AssetDetails>().assetVideoUrl;
            videoPlayer.Play();
            locationAssets[currentLocation].interactedAssets.Add(asset); // Mark as interacted
        }

    }

    public void OnStartClick()
    {

        if (currentLocation < locationAssets.Count)
        {
            mainCanvas.gameObject.SetActive(false);
            started = true;
        }
        else
        {
            // Exit this scene. Same as bac button.
        }

    }
}
