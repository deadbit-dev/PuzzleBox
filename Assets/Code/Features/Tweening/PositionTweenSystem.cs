using UnityEngine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public struct PositionTweenCmp
    {
        public Vector3 StartPosition;
        public Vector3 EndPosition;
        public float Duration;
        public float Timer;
    }
    
    public struct PositionTweenEndEvent {}
    
    public class PositionTweenSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PositionCmp, PositionTweenCmp>, Exc<PositionTweenEndEvent>> filter = Worlds.PresentationWorld;
        private EcsPoolInject<PositionCmp> positionCmpPool = Worlds.PresentationWorld;
        private EcsPoolInject<PositionTweenCmp> positionTweenCmpPool = Worlds.PresentationWorld;
        private EcsPoolInject<PositionTweenEndEvent> positionTweenEndEventPool = Worlds.PresentationWorld;

        public  void Run(IEcsSystems systems)
        {
            foreach (var entity in filter.Value)
            {
                ref PositionCmp positionCmp = ref positionCmpPool.Value.Get(entity);
                ref PositionTweenCmp positionTweenCmp = ref positionTweenCmpPool.Value.Get(entity);

                positionTweenCmp.Timer = Mathf.Clamp(positionTweenCmp.Timer + Time.deltaTime, 0f, positionTweenCmp.Duration);

                var t = EasingFunctions.Linear(positionTweenCmp.Timer / positionTweenCmp.Duration);

                positionCmp.Value.x = Mathf.Lerp(positionTweenCmp.StartPosition.x, positionTweenCmp.EndPosition.x, t);
                positionCmp.Value.y = Mathf.Lerp(positionTweenCmp.StartPosition.y, positionTweenCmp.EndPosition.y, t);
                positionCmp.Value.z = Mathf.Lerp(positionTweenCmp.StartPosition.z, positionTweenCmp.EndPosition.z, t);

                if (positionCmp.Value == positionTweenCmp.EndPosition)
                {
                    positionTweenCmpPool.Value.Del(entity);
                    positionTweenEndEventPool.Value.Add(entity);
                }
            }
        }
    }
}