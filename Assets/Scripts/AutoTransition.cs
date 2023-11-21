using UnityEngine;
using System.Collections;

public class AutoTransition : MonoBehaviour
{
    public int nextSceneIndex = 1; // Index of the next scene in the Build Settings
    public float delay = 2.0f; // Delay in seconds before transitioning

    private SceneLoader sceneLoader; // Reference to the SceneLoader

    void Start()
    {
        // Find the SceneLoader in the scene
        sceneLoader = FindObjectOfType<SceneLoader>();

        StartCoroutine(TransitionAfterDelay());
    }

    IEnumerator TransitionAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        // Use SceneLoader to load the next scene
        if (sceneLoader != null)
        {
            sceneLoader.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogError("SceneLoader not found in the scene.");
        }
    }
}
