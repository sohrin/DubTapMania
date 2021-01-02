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
        Debug.Log("TitleManagerScript.Awake() BEGIN.");

        // acf設定
        string path = CriWare.streamingAssetsPath + "/DubTapMania.acf";
        CriAtomEx.RegisterAcf(null, path);

        // CriAtom作成
        new GameObject().AddComponent<CriAtom>();

        // BGM acb追加
        CriAtom.AddCueSheet("AttackSe", "AttackSeCueSheet.acb", null, null);
        // SE acb追加
        CriAtom.AddCueSheet("EnemySe", "EnemySeCueSheet.acb", null, null);

        // SEを鳴らす
        CriAtomSource se = new GameObject().AddComponent<CriAtomSource>();
        se.loop = false;
        se.cueSheet = "EnemySe";
        se.Play(0);

        Debug.Log("TitleManagerScript.Awake() END.");
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("TitleManagerScript.Start() BEGIN.");

        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        Debug.Log("TitleManagerScript.Start() END.");
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
        Debug.Log("TitleManagerScript.OnClickPlayButton() BEGIN.");

        // BGMを鳴らす
        CriAtomSource bgm = new GameObject().AddComponent<CriAtomSource>();
        bgm.loop = true;
        bgm.cueSheet = "AttackSe";
        bgm.Play(0);

        playButtonText.text = "Wait...";
        Invoke("ChangeScene", 0.5f);

        Debug.Log("TitleManagerScript.OnClickPlayButton() END.");
    }

    void ChangeScene()
    {
        Debug.Log("TitleManagerScript.ChangeScene() BEGIN.");

        SceneManager.LoadScene("BattleScene");

        Debug.Log("TitleManagerScript.ChangeScene() END.");
    }
}
