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
    private static readonly int ENEMY_LIST_MAX_IDX = 1;
    public Text hpText;
    public Text enemyNameText;
    public SpriteRenderer mainSpriteRenderer;
    public Sprite dwarfSprite;
    public Sprite behemothSprite;
    public Sprite elementalSprite;

    // Start is called before the first frame update
    void Start()
    {
        enemyList = new List<EnemyData>();

        // 敵データ準備
        EnemyData enemy = null;

        // TAKOYAKI
        enemy = new EnemyData();
        enemy.name = "TAKOYAKI";
        enemy.hp = 10;
        enemy.atk = 1;
        enemy.def = 1;
        enemy.normalSprite = Resources.Load<Sprite>("Enemies/sample/Static/Front/Megapack III Axe Warrior Dwarf");
        enemy.damagedSprite = Resources.Load<Sprite>("Enemies/sample/Static/Front/Megapack III Axe Warrior Dwarf");
        enemy.defeatedSprite = Resources.Load<Sprite>("Enemies/sample/Static/Front/Megapack III Axe Warrior Dwarf");
        enemyList.Add(enemy);

        // test
        enemy = new EnemyData();
        enemy.name = "test";
        enemy.hp = 15;
        enemy.atk = 2;
        enemy.def = 3;
        enemy.normalSprite = Resources.Load<Sprite>("Enemies/sample/Static/Front/Megapack III Behemoth");
        enemy.damagedSprite = Resources.Load<Sprite>("Enemies/sample/Static/Front/Megapack III Behemoth");
        enemy.defeatedSprite = Resources.Load<Sprite>("Enemies/sample/Static/Front/Megapack III Behemoth");
        enemyList.Add(enemy);
        
        // バトル中の敵を設定
        setEnemy();
    }

    void setEnemy()
    {
        EnemyData enemy = enemyList[enemyIdx];
        enemyNameText.text = enemy.name;
        enemyHp = enemy.hp;
        hpText.text = enemyHp.ToString();
        mainSpriteRenderer.sprite = enemy.normalSprite;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickAttackButton() {
        enemyHp--;
        hpText.text = enemyHp.ToString();
        if (enemyHp == 0) {
            enemyIdx++;
            if (enemyIdx > ENEMY_LIST_MAX_IDX) {
                enemyIdx = 0;
            }
            setEnemy();
        } 
    }
}

