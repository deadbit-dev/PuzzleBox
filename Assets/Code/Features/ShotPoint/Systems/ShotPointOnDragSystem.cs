using UnityEngine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public class ShotPointOnDragSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject world = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<PositionCmp, ShotPointCmp, AttachedProjectile, AimingTag>> filter = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<SlotCmp>, Exc<ActiveTag>> slotFilter = Worlds.PresentationWorld;

        private readonly EcsPoolInject<PositionCmp> positionCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<TransformRefCmp> transformRefCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<ProjectileCmp> projectileCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<ShotPointCmp> shotPointCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<AttachedProjectile> attachedProjectilePool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<ReadyForThrowingTag> readyForThrowingTagPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<DraggingTag> draggingTagPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<AimingTag> aimingTagPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<LinkedToSlot> linkedToSlotPool = Worlds.PresentationWorld;

        private readonly EcsCustomInject<SceneData> sceneData;

        public void Run(IEcsSystems systems)
        {
            Vector2 touchScreenPos = Input.mousePosition;
            var touchWorldPos = sceneData.Value.Camera.ScreenToWorldPoint(touchScreenPos);
            touchWorldPos.z = 0;

            foreach (var shotPointEntity in filter.Value)
            {
                ref var shotPointPositionCmp = ref positionCmpPool.Value.Get(shotPointEntity);
                ref var shotPointCmp = ref shotPointCmpPool.Value.Get(shotPointEntity);
                ref var attachedProjectile = ref attachedProjectilePool.Value.Get(shotPointEntity);

                var shotPointPos = shotPointPositionCmp.Value;

                var projectileIsAlive = attachedProjectile.Value.Unpack(world.Value, out var entity);
                if (!projectileIsAlive) continue;

                ref var positionCmp = ref positionCmpPool.Value.Get(entity);
                ref var transformRefCmp = ref transformRefCmpPool.Value.Get(entity);
                ref var projectileCmp = ref projectileCmpPool.Value.Get(entity);

                var distance = touchWorldPos - shotPointPos;
                var magnitude = Vector2.SqrMagnitude(distance);

                shotPointCmp.CurrentForce = -1 * shotPointCmp.ForceMultiplier *
                    Vector2.ClampMagnitude(distance, shotPointCmp.MaxDistance);

                var delta = Mathf.Abs(shotPointCmp.MaxDistance - magnitude) * shotPointCmp.InterferingFactor;
                positionCmp.Value = shotPointPos +
                    Vector3.ClampMagnitude(distance, shotPointCmp.MaxDistance + delta);

                if (delta < shotPointCmp.InterferingDistance) continue;

                positionCmp.Value = touchWorldPos;
                transformRefCmp.Value.gameObject.layer = (int)Layers.UI;
                projectileCmp.SpriteRenderer.sortingLayerID = SortingLayer.layers[(int)SortingLayers.Selected].id;

                draggingTagPool.Value.Add(entity);
                attachedProjectilePool.Value.Del(shotPointEntity);
                readyForThrowingTagPool.Value.Del(shotPointEntity);
                aimingTagPool.Value.Del(shotPointEntity);

                if (slotFilter.Value.GetEntitiesCount() == 0) continue;
                var slotEntity = slotFilter.Value.GetRawEntities()[0];

                linkedToSlotPool.Value.Add(entity).Value = world.Value.PackEntity(slotEntity);
            }
        }
    }
}