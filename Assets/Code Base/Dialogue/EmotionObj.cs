using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EmotionObj", menuName = "Scriptable Objects/EmotionObj")]
public class EmotionObj : ScriptableObject
{
    public Emotion[] npc = new Emotion[Enum.GetNames(typeof(Npc)).Length];
    
    [Serializable]
    public class Emotion
    {
        public Npc npc;
        public EmotionsSprites[] sprites = new EmotionsSprites[Enum.GetNames(typeof(NpcEmotion)).Length];
    }
    
    [Serializable]
    public class EmotionsSprites
    {
        [SerializeField] private NpcEmotion npcEmotion;
        [SerializeField] private Sprite[] sprites;
        public NpcEmotion NpcEmotion => npcEmotion;
        public Sprite[] Sprites => sprites;
    }
}
