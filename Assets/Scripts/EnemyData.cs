using UnityEngine;

namespace DubTapMusic
{
public class EnemyData
    {
        public int id;
        public string name;
        public int hp;
        public int atk;
        public int def;
        public Sprite normalSprite;
        public Sprite damagedSprite;
        public Sprite defeatedSprite;
        public AudioClip defeatedSoundClip;
    }
}