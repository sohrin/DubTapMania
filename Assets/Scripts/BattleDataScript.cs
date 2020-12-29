﻿using System.Collections;
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
    public SpriteRenderer mainSpriteRenderer;

    int damagedRemainingFrame = 0;
    int defeatedRemainingFrame = 0;

    AudioSource damagedSound;
    AudioSource defeatedSound;

    private static readonly string BASE_PATH_ENEMY_SPRITE = "Enemy/DubTapMusic/";
    private static readonly string BASE_PATH_SE = "Audio/SE/";
    private static readonly string BASE_PATH_ENEMY_SE = BASE_PATH_SE + "Enemy/";

    private static readonly string ENEMY_STATUS_NORMAL = "01";
    private static readonly string ENEMY_STATUS_DAMAGED = "11";
    private static readonly string ENEMY_STATUS_DEFEATED = "21";

    // Start is called before the first frame update
    void Start()
    {
        // SE準備
        damagedSound = gameObject.AddComponent<AudioSource> ();
        AudioClip damagedSoundClip = Resources.Load<AudioClip>(BASE_PATH_SE + "basic/drum/electronic/BassDrum_0001");
        damagedSound.clip = damagedSoundClip;
        defeatedSound = gameObject.AddComponent<AudioSource> ();

        // 敵データ準備
        enemyList = new List<EnemyData>();
        EnemyData enemy = null;

        // TAKOYAKI
        enemy = new EnemyData();
        enemy.id = "0001";
        enemy.name = "takoyaki";
        enemy.hp = 10;
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
    }

    void setEnemy()
    {
        enemyData = enemyList[enemyIdx];
        enemyNameText.text = enemyData.name;
        enemyHp = enemyData.hp;
        hpText.text = enemyHp.ToString();
        defeatedSound.clip = enemyData.defeatedSoundClip;
    }

    // Update is called once per frame
    void Update()
    {
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

    public void OnClickAttackButton() {
        if (defeatedRemainingFrame > 0) 
        {
            damagedSound.Play();
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
                damagedSound.Play();
                damagedRemainingFrame = 20;
                mainSpriteRenderer.sprite = enemyData.damagedSprite;
            }
        }
    }
}

