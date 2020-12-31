using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManagerScript : MonoBehaviour
{
    public Text playButtonText;

    void Awake()
    {
        Debug.Log("TitleManager.Awake() BEGIN.");

        Debug.Log("TitleManager.Awake() END.");
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("TitleManager.Start() BEGIN.");

        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;   

        Debug.Log("TitleManager.Start() END.");
    }

    void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
    {
        Debug.Log(prevScene.name + "->"  + nextScene.name);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(scene.name + " scene loaded");
    }

    void OnSceneUnloaded(Scene scene)
    {
        Debug.Log(scene.name + " scene unloaded");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickPlayButton()
    {
        Debug.Log("TitleManager.OnClickPlayButton() BEGIN.");

        playButtonText.text = "Wait...";
        Invoke("ChangeScene", 0.5f);

        Debug.Log("TitleManager.OnClickPlayButton() END.");
    }

    void ChangeScene()
    {
        Debug.Log("TitleManager.ChangeScene() BEGIN.");

        SceneManager.LoadScene("BattleScene");

        Debug.Log("TitleManager.ChangeScene() END.");
    }
}
