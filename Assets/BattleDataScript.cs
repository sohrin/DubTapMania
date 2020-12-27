using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDataScript : MonoBehaviour
{
    private int enemyIdx = 0;
    private static readonly int ENEMY_MAX_HP = 10;
    private int enemyHp = ENEMY_MAX_HP;
    private List<string> enemyList;
    private static readonly int ENEMY_LIST_MAX_IDX = 2;
    public Text hpText;
    public Text enemyNameText;
    public SpriteRenderer mainSpriteRenderer;
    public Sprite dwarfSprite;
    public Sprite behemothSprite;
    public Sprite elementalSprite;

    // Start is called before the first frame update
    void Start()
    {
        enemyList = new List<string>();
        enemyList.Add("Dwarf");
        enemyList.Add("Behemoth");
        enemyList.Add("Elemental");
        hpText.text = enemyHp.ToString();
        enemyNameText.text = enemyList[enemyIdx];
        mainSpriteRenderer.sprite = dwarfSprite;
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
            enemyHp = ENEMY_MAX_HP;
            hpText.text = enemyHp.ToString();
            enemyNameText.text = enemyList[enemyIdx];
            if (enemyList[enemyIdx] == "Dwarf") {
                mainSpriteRenderer.sprite = dwarfSprite;
            } else if (enemyList[enemyIdx] == "Behemoth") {
                mainSpriteRenderer.sprite = behemothSprite;
            } else if (enemyList[enemyIdx] == "Elemental") {
                mainSpriteRenderer.sprite = elementalSprite;
            }
        } 
    }
}
