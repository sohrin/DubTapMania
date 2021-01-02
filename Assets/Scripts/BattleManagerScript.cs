using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DubTapMusic;
using DubTapMania_acf;

public class BattleManagerScript : MonoBehaviour
{
    public EnemyData enemyData;
    private int enemyHp;
    private int enemyIdx = 0;
    private List<EnemyData> enemyList;
    public Text hpText;
    public Text enemyNameText;
    int stockDamage = 0;
    public SpriteRenderer mainSpriteRenderer;

    int damagedRemainingFrame = 0;
    int defeatedRemainingFrame = 0;

    CriAtomSource battleBgmSound;
    CriAtomSource attackSeSound;
    CriAtomSource enemySeSound;
    int enemyDefeatedSoundIdx = 0;

    private static readonly string ACF_FILE_PATH_FROM_STREAMING_ASSETS  = "DubTapMania.acf";
    private static readonly string ACB_FILE_NAME_SUFFIX = "CueSheet.acb";
    private static readonly string CUE_SHEET_NAME_BATTLE_BGM_STAGE1 = "BattleBgmStage1";
    private static readonly string CUE_SHEET_NAME_ATTACK_SE = "AttackSe";
    private static readonly string CUE_SHEET_NAME_ENEMY_DEFEATED_SE = "EnemyDefeatedSe";

    int musicFrame = 0;

    // MEMO: スプライトのファイルパス指定時は拡張子不要。
    private static readonly string BASE_PATH_ENEMY = "Enemy/DubTapMusic/";
    private static readonly string ENEMY_STATUS_NORMAL = "01";
    private static readonly string ENEMY_STATUS_DAMAGED = "11";
    private static readonly string ENEMY_STATUS_DEFEATED = "21";

    public enum AtkCode
    {
        KICK,
        PUNCH
    }
    private const string ATK_CODE_KICK = "KICK";
    private const string ATK_CODE_PUNCH = "PUNCH";

    private CriAtomSource createCriAtomSource(string cueSheetName, bool loopFlg)
    {
        CriAtom.AddCueSheet(cueSheetName, cueSheetName + ACB_FILE_NAME_SUFFIX, null, null);
        CriAtomSource criAtomSource = new GameObject().AddComponent<CriAtomSource>();
        criAtomSource.loop = loopFlg;
        criAtomSource.cueSheet = cueSheetName;
        return criAtomSource;
    }
    
    void Awake()
    {
        Debug.Log("BattleManagerScript.Awake() BEGIN.");

        // 「CRI ADX2」設定
        string path = CriWare.streamingAssetsPath + "/" + ACF_FILE_PATH_FROM_STREAMING_ASSETS;
        CriAtomEx.RegisterAcf(null, path);
        new GameObject().AddComponent<CriAtom>();
        attackSeSound = createCriAtomSource(CUE_SHEET_NAME_ATTACK_SE, false);
        enemySeSound = createCriAtomSource(CUE_SHEET_NAME_ENEMY_DEFEATED_SE, false);

        // 敵データ準備
        enemyList = new List<EnemyData>();
        EnemyData enemy;

        enemy = Resources.Load<EnemyData>(BASE_PATH_ENEMY + "0001_takoyaki/0001_takoyaki");
        enemyList.Add(enemy);

        enemy = Resources.Load<EnemyData>(BASE_PATH_ENEMY + "0002_pizzicato/0002_pizzicato");
        enemyList.Add(enemy);
        
        // バトル中の敵を設定
        setEnemy();

        // BGMフレーム初期化
        musicFrame = 0;

        Debug.Log("BattleManagerScript.Awake() END.");
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("BattleManagerScript.Start() BEGIN.");

        // MEMO: OnActiveSceneChanged動作時にbattleBgmSoundがnullになる件の対策。
        DontDestroyOnLoad(this);

        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        Debug.Log("BattleManagerScript.Start() END.");
    }

    void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
    {
        Debug.Log(prevScene.name + "->"  + nextScene.name);

        // 戦闘BGM再生（このシーンに遷移してきた時に再生開始）
        // MEMO: シーン遷移時に再生する音はOnActiveSceneChangedで本メソッドを呼び出してメンバに代入しておかないと何故かエラーが発生する（AwakeやStartだとダメ）。シーン遷移後ならAwake内でOK。
        battleBgmSound = createCriAtomSource(CUE_SHEET_NAME_BATTLE_BGM_STAGE1, true);
        battleBgmSound.Play(0);

        // BattleSceneをアクティブに設定
        Scene battleScene = SceneManager.GetSceneByName("BattleScene");
        SceneManager.SetActiveScene(battleScene);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(scene.name + " scene loaded");
    }

    void OnSceneUnloaded(Scene scene)
    {
        Debug.Log(scene.name + " scene unloaded");
    }

    private bool isBattleSceneActive()
    {
        Scene scene = SceneManager.GetActiveScene();
        Debug.LogFormat( "ActiveScene = {0}", scene.name );
        return (scene.name == "BattleScene");
    }

    void setEnemy()
    {
        enemyData = enemyList[enemyIdx];
        enemyNameText.text = enemyData.name;
        enemyHp = enemyData.hp - stockDamage;
        if (enemyHp <= 0)
        {
            // 撃破中に次の敵がHP0以下になったら
            // TODO: この状態にならないようなゲームバランス調整
            enemyHp = 1;
        }
        stockDamage = 0;
        hpText.text = enemyHp.ToString();
        enemyDefeatedSoundIdx = enemyData.getIdIdx();
    }

    // Update is called once per frame
    void Update()
    {
        // BGMフレーム加算
        musicFrame++;

        if (defeatedRemainingFrame > 0) {
            damagedRemainingFrame = 0;
            defeatedRemainingFrame--;
            if (defeatedRemainingFrame == 0)
            {
                enemyIdx++;
                if (enemyIdx > enemyList.Count - 1)
                {
                    enemyIdx = 0;
                }
                setEnemy();
                mainSpriteRenderer.sprite = enemyData.normalSprite;
            }
        }
        else if (damagedRemainingFrame > 0)
        {
            damagedRemainingFrame--;
            if (damagedRemainingFrame == 0)
            {
                mainSpriteRenderer.sprite = enemyData.normalSprite;
            }
        }
    }

    public void OnPointerDownKickButton()
    {
        Debug.Log("BattleManagerScript.OnPointerDownKickButton() BEGIN.");

        // BattleSceneに遷移して来ていない場合はUIを動作しないようにする（TitleSceneから押せてしまう件の対策）。
        // TODO: もっとスマートな解決方法はないのか？シーンが3つ4つになってきたら判定が大変だしUIのOnClick処理すべてに判定が必要になる。
        if (!isBattleSceneActive())
        {
            return;
        }

        AttackEnemy(AtkCode.KICK);

        Debug.Log("BattleManagerScript.OnPointerDownKickButton() END.");
    }

    public void OnPointerDownPunchButton()
    {
        Debug.Log("BattleManagerScript.OnPointerDownPunchButton() BEGIN.");

        // BattleSceneに遷移して来ていない場合はUIを動作しないようにする（TitleSceneから押せてしまう件の対策）。
        if (!isBattleSceneActive())
        {
            return;
        }

        AttackEnemy(AtkCode.PUNCH);

        Debug.Log("BattleManagerScript.OnPointerDownPunchButton() END.");
    }

    public void AttackEnemy(AtkCode atkCode)
    {
        Debug.Log("BattleManagerScript.OnPointerDownPunchButton() BEGIN. atkCode:[" + atkCode + "]");

        PlayAtkSound(atkCode);

        if (defeatedRemainingFrame > 0) 
        {
            stockDamage++;
        }
        else
        {
            enemyHp--;
            hpText.text = enemyHp.ToString();
            if (enemyHp == 0)
            {
                enemySeSound.Play(enemyDefeatedSoundIdx);
                defeatedRemainingFrame = 90;
                mainSpriteRenderer.sprite = enemyData.defeatedSprite;
            }
            else
            {
                damagedRemainingFrame = 20;
                mainSpriteRenderer.sprite = enemyData.damagedSprite;
            }
        }

        Debug.Log("BattleManagerScript.OnPointerDownPunchButton() END.");
    }

    private void PlayAtkSound(AtkCode atkCode)
    {
        switch (atkCode)
        {
            case AtkCode.KICK:
                attackSeSound.Play(0);
                break;
            case AtkCode.PUNCH:
                attackSeSound.Play(1);
                break;
        }
    }

    // /// <summary>
    // /// ＜使用例＞
    // /// ・メンバ
    // /// CriAtomExPlayer battleBgmSound;
    // /// CriAtomExWaveVoicePool waveVoicePool;
    // /// private static readonly int CRI_ATOM_EX_PLAYER_MAX_PATH = 256;
    // /// private static readonly int CRI_ATOM_EX_PLAYER_MAX_PATH_STRINGS = 1;
    // /// private static readonly uint VOICE_POOL_ID_BATTLE_BGM = 0x00000011;
    // /// ・OnDestroyメソッド内
    // /// if (battleBgmSound != null) {
    // ///     battleBgmSound.Dispose();
    // /// }
    // /// ・使用箇所
    // /// battleBgmSound = createWavePlayer(VOICE_POOL_ID_BATTLE_BGM, BASE_PATH_BGM + "stage/1/DubTapMania_stage1_BPM150.wav", true);
    // /// battleBgmSound.Start();
    // /// </summary>
    // private CriAtomExPlayer createWavePlayer(uint voicePoolId, string pathFromStreamingAssets, bool loopFlg)
    // {
    //     // MEMO: https://game.criware.jp/manual/unity_plugin/latest/contents/classCriAtomExWaveVoicePool.html
    //     waveVoicePool = new CriAtomExWaveVoicePool(
    //         1,                    // int 	numVoices,
    //         2,                      // int 	maxChannels,
    //         44100,                  // int 	maxSamplingRate,
    //         true,                   // bool 	streamingFlag,
    //         voicePoolId      // uint 	identifier = 0 
    //     );
    //
    //     CriAtomExPlayer player = new CriAtomExPlayer(CRI_ATOM_EX_PLAYER_MAX_PATH, CRI_ATOM_EX_PLAYER_MAX_PATH_STRINGS);
    //     player.SetVoicePoolIdentifier(voicePoolId);
    //     player.SetFile(null, CriWare.streamingAssetsPath + "/" + pathFromStreamingAssets);
    //     player.SetFormat(CriAtomEx.Format.WAVE);
    //     player.SetNumChannels(2);
    //     player.SetSamplingRate(44100);
    //     player.Loop(loopFlg);
    //     return player;
    // }

}

