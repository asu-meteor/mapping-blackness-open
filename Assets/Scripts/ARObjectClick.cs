using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ARObjectClick : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Assign this in the inspector
    public GameObject videoScreen;
    public string videoLink;
    public int assetTag;
    public bool assetClicked;


    void Start()
    {
        // Ensure the video player is not playing at start
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }
        assetClicked = false;
        videoScreen.SetActive(false);
    }

    void Update()
    {
        // Check for a touch or click
        if (Input.GetMouseButtonDown(0)) // for touch use Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;


            if (Physics.Raycast(ray, out hit))
            {
                
                // Check if the AR object is clicked
                if (hit.transform == transform)
                {
                    
                    // Play or pause the video
                    if (!videoPlayer.isPlaying)
                    {
                        videoPlayer.url = videoLink;
                        videoPlayer.Play();
                        videoScreen.SetActive(true);
                        ExpStateManager.Instance.VideoCanvas.gameObject.SetActive(true);
                        
                    }
                }
            }
        }


    }

    public void OnPauseButtonClick()
    {
        videoPlayer.Pause();
    }

    public void OnStopButtonClick()
    {
        videoPlayer.Stop();
        videoScreen.SetActive(false);
        assetClicked = true;
        //ExpStateManager.Instance.assetsArray.GetValue(assetTag);
        ExpStateManager.Instance.VideoCanvas.gameObject.SetActive(false);
    }

}
