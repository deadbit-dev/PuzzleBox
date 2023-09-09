using UnityEngine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public class RotationHandlerOnDownSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject world = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<RotationHandlerCmp, LinkedToShotPointCmp, ActiveTag>, Exc<RotatingTag>> rotationHandlerFilter = Worlds.PresentationWorld;

        private EcsPoolInject<PositionCmp> positionCmpPool = Worlds.PresentationWorld;
        private EcsPoolInject<TransformRefCmp> transformRefCmpPool = Worlds.PresentationWorld;
        private EcsPoolInject<RotationHandlerCmp> rotationHandlerCmpPool = Worlds.PresentationWorld;
        private EcsPoolInject<RotatingTag> rotatingTagPool = Worlds.PresentationWorld;
        private EcsPoolInject<LinkedToShotPointCmp> linkedToShotPointPool = Worlds.PresentationWorld;
        private EcsPoolInject<AttachedProjectile> attachedProjectilePool = Worlds.PresentationWorld;

        private EcsCustomInject<SceneData> sceneData;

        public void Run(IEcsSystems systems)
        {
            if (!Input.GetMouseButtonDown(0)) return;

            Vector2 touchScreenPos = Input.mousePosition;
            Vector3 touchWorldPos = sceneData.Value.Camera.ScreenToWorldPoint(touchScreenPos);
            touchWorldPos.z = 0;

            foreach (var rotationHandlerEntity in rotationHandlerFilter.Value)
            {
                ref LinkedToShotPointCmp linkedToShotPointCmp = ref linkedToShotPointPool.Value.Get(rotationHandlerEntity);

                var shotPoint_is_alive = linkedToShotPointCmp.Value.Unpack(world.Value, out int shotPointEntity);
                if (!shotPoint_is_alive) continue;

                ref PositionCmp shotPointPositionCmp = ref positionCmpPool.Value.Get(shotPointEntity);
                ref RotationHandlerCmp rotationHandlerCmp = ref rotationHandlerCmpPool.Value.Get(rotationHandlerEntity);
                ref AttachedProjectile attachedProjectile = ref attachedProjectilePool.Value.Get(shotPointEntity);

                var projectile_alive = attachedProjectile.Value.Unpack(world.Value, out int projectileEntity);
                if (!projectile_alive) continue;

                ref TransformRefCmp transformRefCmp = ref transformRefCmpPool.Value.Get(projectileEntity);

                Vector3 pressedPointInShotPointSpace = touchWorldPos - shotPointPositionCmp.Value;
                float magnitude = pressedPointInShotPointSpace.magnitude;
                if ((rotationHandlerCmp.MinZoneRadius > magnitude) || (magnitude > rotationHandlerCmp.MaxZoneRadius)) continue;

                rotationHandlerCmp.StartPressedPoint = pressedPointInShotPointSpace;
                rotationHandlerCmp.StartProjectileAngle = transformRefCmp.Value.localRotation.eulerAngles.z;
                rotatingTagPool.Value.Add(rotationHandlerEntity);
            }
        }
    }
}