using UnityEngine;

namespace JoyTeam
{
    [CreateAssetMenu]
    public sealed class StaticData : ScriptableObject
    {
        public LevelUI LevelUI;
        public Level[] Levels;
        [Space]
        public float BackToSlotDuration = 0.3f;
    }
}