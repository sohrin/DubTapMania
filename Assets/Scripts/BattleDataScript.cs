using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DubTapMusic;

public class BattleDataScript : MonoBehaviour
{
    public EnemyData enemyData;
    private int enemyHp;
    private int enemyIdx = 0;
    private List<EnemyData> enemyList;
    private static readonly int ENEMY_LIST_MAX_SIZE = 1;
    public Text hpText;
    public Text enemyNameText;
    int stockDamage = 0;
    public SpriteRenderer mainSpriteRenderer;

    int damagedRemainingFrame = 0;
    int defeatedRemainingFrame = 0;

    AudioSource kickAtkSound;
    AudioSource punchAtkSound;
    AudioSource defeatedSound;
    AudioSource bgmSound;

    int musicFrame = 0;

    private static readonly string BASE_PATH_ENEMY_SPRITE = "Enemy/DubTapMusic/";
    private static readonly string BASE_PATH_SE = "Audio/SE/";
    private static readonly string BASE_PATH_ENEMY_SE = BASE_PATH_SE + "Enemy/";
    private static readonly string BASE_PATH_BGM = "Audio/BGM/";

    private static readonly string ENEMY_STATUS_NORMAL = "01";
    private static readonly string ENEMY_STATUS_DAMAGED = "11";
    private static readonly string ENEMY_STATUS_DEFEATED = "21";

    private const string ATK_CODE_KICK = "KICK";
    private const string ATK_CODE_PUNCH = "PUNCH";
    

    // Start is called before the first frame update
    void Start()
    {
        // BGM準備
        bgmSound = gameObject.AddComponent<AudioSource> ();
        AudioClip bgmSoundClip = Resources.Load<AudioClip>(BASE_PATH_BGM + "stage/1/DubTapMania_stage1");
        bgmSound.clip = bgmSoundClip;
        bgmSound.loop = true;

        // SE準備
        kickAtkSound = gameObject.AddComponent<AudioSource> ();
        AudioClip kickAtkSoundClip = Resources.Load<AudioClip>(BASE_PATH_SE + "basic/drum/electronic/BassDrum_0001");
        kickAtkSound.clip = kickAtkSoundClip;

        punchAtkSound = gameObject.AddComponent<AudioSource> ();
        AudioClip punchAtkSoundClip = Resources.Load<AudioClip>(BASE_PATH_SE + "basic/drum/electronic/Snare_0001");
        punchAtkSound.clip = punchAtkSoundClip;

        defeatedSound = gameObject.AddComponent<AudioSource> ();

        // 敵データ準備
        enemyList = new List<EnemyData>();
        EnemyData enemy = null;

        // TAKOYAKI
        enemy = new EnemyData();
        enemy.id = "0001";
        enemy.name = "takoyaki";
        enemy.hp = 8;
        enemy.atk = 1;
        enemy.def = 1;
        // MEMO: 拡張子不要
        enemy.normalSprite = Resources.Load<Sprite>(BASE_PATH_ENEMY_SPRITE + enemy.getResoursePathFromBase(ENEMY_STATUS_NORMAL));
        enemy.damagedSprite = Resources.Load<Sprite>(BASE_PATH_ENEMY_SPRITE + enemy.getResoursePathFromBase(ENEMY_STATUS_DAMAGED));
        enemy.defeatedSprite = Resources.Load<Sprite>(BASE_PATH_ENEMY_SPRITE + enemy.getResoursePathFromBase(ENEMY_STATUS_DEFEATED));
        enemy.defeatedSoundClip = Resources.Load<AudioClip>(BASE_PATH_ENEMY_SE + enemy.getResoursePathFromBase(ENEMY_STATUS_DEFEATED));

        enemyList.Add(enemy);
        
        // バトル中の敵を設定
        setEnemy();

        // BGM再生
        bgmSound.Play();

        // BGMフレーム初期化
        musicFrame = 0;

    }

    void setEnemy()
    {
        enemyData = enemyList[enemyIdx];
        enemyNameText.text = enemyData.name;
        enemyHp = enemyData.hp - stockDamage;
        stockDamage = 0;
        hpText.text = enemyHp.ToString();
        defeatedSound.clip = enemyData.defeatedSoundClip;
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
                if (enemyIdx > ENEMY_LIST_MAX_SIZE - 1)
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

    public void OnClickKickButton()
    {
        OnClickAttackButton(ATK_CODE_KICK);
    }

    public void OnClickPunchButton()
    {
        OnClickAttackButton(ATK_CODE_PUNCH);
    }

    public void OnClickAttackButton(string atkCode)
    {
        if (defeatedRemainingFrame > 0) 
        {
            PlayAtkSound(atkCode);
            stockDamage++;
        }
        else {
            enemyHp--;
            hpText.text = enemyHp.ToString();
            if (enemyHp == 0)
            {
                defeatedSound.Play();
                defeatedRemainingFrame = 90;
                mainSpriteRenderer.sprite = enemyData.defeatedSprite;
            }
            else
            {
                PlayAtkSound(atkCode);
                damagedRemainingFrame = 20;
                mainSpriteRenderer.sprite = enemyData.damagedSprite;
            }
        }
    }

    private void PlayAtkSound(string atkCode)
    {
        switch (atkCode)
        {
            case ATK_CODE_KICK:
                kickAtkSound.Play();
                break;
            case ATK_CODE_PUNCH:
                punchAtkSound.Play();
                break;
        }
    }
}

