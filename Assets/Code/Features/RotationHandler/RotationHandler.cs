using UnityEditor;
using UnityEngine;

namespace JoyTeam
{
    public class RotationHandler : MonoBehaviour
    {
        public float MinZoneRadius = 1f;
        public float MaxZoneRadius = 2f;

#if UNITY_EDITOR_WIN && DEBUG
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, MinZoneRadius);
            Gizmos.DrawWireSphere(transform.position, MaxZoneRadius);
        }
#endif
    }
}