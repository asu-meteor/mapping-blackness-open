using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Loads scenes based on scene in index
    public void LoadScene(int sceneIndex){
        SceneManager.LoadScene(sceneIndex);
    }

}
