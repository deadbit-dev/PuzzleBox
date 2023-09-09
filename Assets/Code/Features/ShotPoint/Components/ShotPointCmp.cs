using UnityEngine;

namespace JoyTeam
{
    public struct ShotPointCmp
    {
        public float CatchRadius;
        public float DismissRadius;
        public float MaxDistance;
        public float ForceMultiplier;
        public float InterferingFactor;
        public float InterferingDistance;
        public Vector2 CurrentForce;
    }
}