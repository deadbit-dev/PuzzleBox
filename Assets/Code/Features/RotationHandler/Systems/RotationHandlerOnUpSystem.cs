using UnityEngine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public class RotationHandlerOnUpSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<RotationHandlerCmp, RotatingTag>> rotationHandlerFilter = Worlds.PresentationWorld;

        private EcsPoolInject<RotatingTag> rotatingTagPool = Worlds.PresentationWorld;

        public void Run(IEcsSystems systems)
        {
            if (!Input.GetMouseButtonUp(0)) return;

            foreach (var rotationHandlerEntity in rotationHandlerFilter.Value)
            {
                rotatingTagPool.Value.Del(rotationHandlerEntity);
            }
        }
    }
}