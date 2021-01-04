using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DubTapMusic;
using UniRx;
using UniRx.Triggers;

public class BattleManagerScript : MonoBehaviour
{
    [SerializeField] private  Text hpText;
    [SerializeField] private  Text enemyNameText;
    [SerializeField] private  Text bpmText;
    [SerializeField] private  Text beatText;
    [SerializeField] private  Text judgeText;
    [SerializeField] private  SpriteRenderer mainSpriteRenderer;
    [SerializeField] private Button kickButton;
    [SerializeField] private Button punchButton;

    private EnemyData enemyData;
    private int enemyHp;
    private int enemyIdx = 0;
    private List<EnemyData> enemyList;
    int stockDamage = 0;
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

    private static readonly int MINSEC = 60;
    private string stageName;
    private static float musicStartTimeMs;
    private bool recalcMusicStartTimeMsFinishFlg = false;
    private int bpm = 0;
    private int bar = 0;
    private int beat = 0;

    // MEMO: スプライトのファイルパス指定時は拡張子不要。
    private static readonly string BASE_PATH_ENEMY = "Enemy/DubTapMusic/";

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

        // ビートのコールバックを設定
        CriAtomExBeatSync.SetCallback(this.BeatOn);

        // ステージ設定
        stageName = "STAGE 1";
        bpm = 150;
        bpmText.text = bpm.ToString();

        // 敵データ準備
        enemyList = new List<EnemyData>();
        EnemyData enemy;

        enemy = Resources.Load<EnemyData>(BASE_PATH_ENEMY + "0001_takoyaki/0001_takoyaki");
        enemyList.Add(enemy);

        enemy = Resources.Load<EnemyData>(BASE_PATH_ENEMY + "0002_pizzicato/0002_pizzicato");
        enemyList.Add(enemy);
        
        // バトル中の敵を設定
        setEnemy();

        Debug.Log("BattleManagerScript.Awake() END.");
    }

    void OnEnable() {

        // MEMO: OnPointerDownAsObservableを使うには「using UniRx.Triggers;」が必要。
        kickButton
            .OnPointerDownAsObservable()
            .Subscribe(_ => OnPointerDownKickButton())
            .AddTo(this);

        punchButton
            .OnPointerDownAsObservable()
            .Subscribe(_ => OnPointerDownPunchButton())
            .AddTo(this);

        // // BGMフレーム加算＆ビート情報のインクリメント
        // this
        //     .UpdateAsObservable()
        //     .Subscribe(_ => {
        //         musicFrame++;
        //         bool updBeatFlg = false;

        //         // BPMは60秒に4分音符がいくつ起きるかの数。
        //         // フレームは60フレーム設定の場合、1/60秒
        //         // つまり1拍のフレーム数は、「60秒*60フレーム/BPM」で算出できる。
        //         // →没！！！フレームレート60を必ずキープできるわけではない！
        //         if (musicFrame % (MINSEC * FPS / bpm) == 0)
        //         {
        //             updBeatFlg = true;
        //             quarterNote++;
        //             if (quarterNote == 5)
        //             {
        //                 bar++;
        //                 quarterNote = 1;
        //                 eighthNote = 0;
        //                 sixteenthNote = 0;
        //             }
        //         }

        //         if (musicFrame % (MINSEC * FPS / bpm / 2) == 0)
        //         {
        //             updBeatFlg = true;
        //             eighthNote++;
        //         }

        //         if (musicFrame % (MINSEC * FPS / bpm / 2 / 2) == 0)
        //         {
        //             sixteenthNote++;
        //         }

        //         if (updBeatFlg)
        //         {
        //             beatText.text = bar.ToString()
        //                           + " "
        //                           + quarterNote.ToString()
        //                           + " "
        //                           + eighthNote.ToString();
        //         }
        //     });
        
        // 敵撃破演出フレーム残あり
        this
            .UpdateAsObservable()
            .Where(_ => defeatedRemainingFrame > 0)
            .Subscribe(_ => {
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
            });

        // 敵ダメージ演出フレーム残あり
        this
            .UpdateAsObservable()
            .Where(_ => damagedRemainingFrame > 0)
            .Subscribe(_ => {
                damagedRemainingFrame--;
                if (damagedRemainingFrame == 0)
                {
                    mainSpriteRenderer.sprite = enemyData.normalSprite;
                }
            });

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
        // MEMO: CriAtomCraftで設定したシーケンスループマーカーでループさせるため、CriAtomSource側のループ設定はオフ
        battleBgmSound = createCriAtomSource(CUE_SHEET_NAME_BATTLE_BGM_STAGE1, false);
        battleBgmSound.Play(0);

        // BGM開始時刻の初期化
        musicStartTimeMs = Time.time * 1000;
        Debug.Log("★★★★★★ musicStartTimeMs ★★★★★");
        Debug.Log(musicStartTimeMs.ToString());

        // BattleSceneをアクティブに設定
        Scene battleScene = SceneManager.GetSceneByName("BattleScene");
        SceneManager.SetActiveScene(battleScene);
    }

    void BeatOn(ref CriAtomExBeatSync.Info info)
    {
        Debug.Log("BattleManagerScript.BeatOn() BEGIN.");

        beat++;
        beatText.text = beat.ToString();

        // if (!recalcMusicStartTimeMsFinishFlg) {
        //     // 1拍目の時間から1拍分の時間を引いた時間をBGM開始時刻として再設定する。
        //     // 1ビート目が始まるまではズレが補正できない。演出で耐えるか・・・？
        //     // TODO: うまくいったら「BGM開始時刻の初期化」処理を削除

        //     // BGM開始時刻の初期化
        //     musicStartTimeMs = Time.time * 1000 - ( (float) MINSEC * 1000 / (float) bpm );

        //     recalcMusicStartTimeMsFinishFlg = true;
        // }
    
        Debug.Log("BattleManagerScript.BeatOn() END.");
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

        // タッチ時刻
        float touchTimeMs = Time.time * 1000;
        Debug.Log("★★★★★★ touchTimeMs ★★★★★");
        Debug.Log(touchTimeMs.ToString());

        // 曲開始からの経過時間
        float timeMsFromMusicStart = touchTimeMs - musicStartTimeMs;
        Debug.Log("★★★★★★ timeMsFromMusicStart ★★★★★");
        Debug.Log(timeMsFromMusicStart.ToString());

        // 開始から何拍目かを小数点で算出(1足すことでbeatと整合性をあわせている。beatは開始時に1、経過時間は1拍経って1になる)
        float beatFromMusicStart = timeMsFromMusicStart /  ( (float) MINSEC * 1000 / (float) bpm ) + 1f;
        Debug.Log("★★★★★★ beatFromMusicStart ★★★★★");
        Debug.Log(beatFromMusicStart.ToString());
        judgeText.text = beatFromMusicStart.ToString();
        
        // 攻撃音再生
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

