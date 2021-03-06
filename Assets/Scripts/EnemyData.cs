﻿using UnityEngine;

namespace DubTapMusic
{
    [CreateAssetMenu(menuName = "DubTapManiaScript/Create EnemyData")]
    public class EnemyData : ScriptableObject
    {
        public int id;
        public string name;
        // TODO: 表示名(displayName)、多言語対応
        public int hp;
        public int atk;
        public int def;
        public Sprite normalSprite;
        public Sprite damagedSprite;
        public Sprite defeatedSprite;

        public string getResoursePathFromBase(string status)
        {
            return getIdAndName()
                 + "/"
                 + getIdAndName()
                 + status == null ? "" : "_" + status;
        }

        private string getIdAndName()
        {
            return id.ToString("0000") + "_" + name;
        }

        public int getIdIdx()
        {
            return id - 1;
        }

    }
}