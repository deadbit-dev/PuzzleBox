using UnityEngine;

namespace JoyTeam
{
    public class ShotPoint : PresenterBase
    {
        [Header("Parameters")]
        public float ForceMultiplier = 12f;
        public float CatchRadius = 2.5f;
        public float DismissRadius = 1f;
        public float MaxDistance = 5f;
        public float InterferingDistance = 2f;
        [Range(0f, 1f)] public float InterferingFactor = 0.01f;

        [Header("Linked views")]
        public RotationHandler RotationHandler;
        public Trajectory Trajectory;
    }
}