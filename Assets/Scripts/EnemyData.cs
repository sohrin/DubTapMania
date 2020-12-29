using UnityEngine;

namespace DubTapMusic
{
public class EnemyData
    {
        public string id;
        public string name;
        // TODO: 表示名(displayName)、多言語対応
        public int hp;
        public int atk;
        public int def;
        public Sprite normalSprite;
        public Sprite damagedSprite;
        public Sprite defeatedSprite;
        public AudioClip defeatedSoundClip;

        public string getResoursePathFromBase(string status)
        {
            return getIdAndName()
                 + "/"
                 + getIdAndName()
                 + "_"
                 + status;
        }

        private string getIdAndName()
        {
            return id + "_" + name;
        }

    }
}