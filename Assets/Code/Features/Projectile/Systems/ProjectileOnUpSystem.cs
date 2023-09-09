using UnityEngine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public class ProjectileOnUpSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject world = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<PositionCmp, TransformRefCmp, ProjectileCmp, LinkedToSlot, DraggingTag>> projectileFilter = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<PositionCmp, ShotPointCmp>> shotPointFilter = Worlds.PresentationWorld;

        private readonly EcsPoolInject<PositionCmp> positionCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<PositionTweenCmp> positionTweenCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<TransformRefCmp> transformRefCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<ProjectileCmp> projectileCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<ShotPointCmp> shotPointCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<AttachedProjectile> attachedProjectilePool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<ReadyForThrowingTag> readyForThrowingTagPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<LinkedToSlot> linkedToSlotPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<DraggingTag> draggingTagPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<SlotCmp> slotCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<ActiveTag> activeSlotTagPool = Worlds.PresentationWorld;

        private readonly EcsCustomInject<StaticData> staticData;
        private readonly EcsCustomInject<SceneData> sceneData;

        public void Run(IEcsSystems systems)
        {
            if (!Input.GetMouseButtonUp(0)) return;

            Vector2 touchScreenPos = Input.mousePosition;
            var touchWorldPos = sceneData.Value.Camera.ScreenToWorldPoint(touchScreenPos);
            touchWorldPos.z = 0;

            foreach (var projectileEntity in projectileFilter.Value)
            {
                ref var positionCmp = ref positionCmpPool.Value.Get(projectileEntity);
                ref var projectileTransformRefCmp = ref transformRefCmpPool.Value.Get(projectileEntity);
                ref var projectileCmp = ref projectileCmpPool.Value.Get(projectileEntity);

                if (!linkedToSlotPool.Value.Get(projectileEntity).Value.Unpack(world.Value, out var slotEntity)) continue;
                ref var slotCmp = ref slotCmpPool.Value.Get(slotEntity);

                var closestShotPointEntity = -1;
                foreach (var shotPointEntity in shotPointFilter.Value)
                {
                    ref var shotPointPositionCmp = ref positionCmpPool.Value.Get(shotPointEntity);
                    //ref var shotPointTransformRefCmp = ref transformRefCmpPool.Value.Get(shotPointEntity);
                    ref var shotPointCmp = ref shotPointCmpPool.Value.Get(shotPointEntity);

                    var distance = Vector3.Distance(shotPointPositionCmp.Value, touchWorldPos);
                    if (distance > shotPointCmp.CatchRadius) continue;

                    closestShotPointEntity = shotPointEntity;
                    break;
                }

                if (closestShotPointEntity != -1)
                {
                    ref var shotPointPositionCmp = ref positionCmpPool.Value.Get(closestShotPointEntity);

                    if (!readyForThrowingTagPool.Value.Has(closestShotPointEntity))
                    {
                        slotCmp.Transform.gameObject.SetActive(false);
                        activeSlotTagPool.Value.Del(slotEntity);
                        readyForThrowingTagPool.Value.Add(closestShotPointEntity);
                    }
                    else if (attachedProjectilePool.Value.Get(closestShotPointEntity).Value.Unpack(world.Value, out var entity))
                    {
                        linkedToSlotPool.Value.Add(entity).Value = world.Value.PackEntity(slotEntity);
                        SetProjectileToSlotFromShotPoint(entity, slotEntity);
                        attachedProjectilePool.Value.Del(closestShotPointEntity);
                    }
                    
                    positionCmp.Value = shotPointPositionCmp.Value;
                    projectileTransformRefCmp.Value.gameObject.layer = (int)Layers.Game;
                    projectileTransformRefCmp.Value.localScale = new Vector3(2f, 2f, 1); // FIXME: remove const value
                    projectileCmp.SpriteRenderer.sortingLayerID = SortingLayer.layers[(int)SortingLayers.Game].id;
                    projectileCmp.SpriteRenderer.sortingOrder = 0;

                    draggingTagPool.Value.Del(projectileEntity);
                    linkedToSlotPool.Value.Del(projectileEntity);
                    attachedProjectilePool.Value.Add(closestShotPointEntity).Value = world.Value.PackEntity(projectileEntity);
                }
                else SetProjectileBackToSlot(projectileEntity, slotEntity);
            }
        }

        private void SetProjectileToSlotFromShotPoint(int projectileEntity, int slotEntity)
        {
            ref var positionCmp = ref positionCmpPool.Value.Get(projectileEntity);
            ref var transformRefCmp = ref transformRefCmpPool.Value.Get(projectileEntity);
            ref var projectileCmp = ref projectileCmpPool.Value.Get(projectileEntity);
            ref var slotCmp = ref slotCmpPool.Value.Get(slotEntity);

            transformRefCmp.Value.gameObject.layer = (int)Layers.UI;
            transformRefCmp.Value.parent = slotCmp.Origin;
            projectileCmp.SpriteRenderer.sortingLayerID = SortingLayer.layers[(int)SortingLayers.UI].id;
            projectileCmp.SpriteRenderer.sortingOrder++;

            if (!positionTweenCmpPool.Value.Has(projectileEntity))
            {
                ref var positionTweenCmp = ref positionTweenCmpPool.Value.Add(projectileEntity);
                positionTweenCmp.StartPosition = positionCmp.Value;
                positionTweenCmp.EndPosition = slotCmp.Transform.position;
                positionTweenCmp.Duration = staticData.Value.BackToSlotDuration;
            }

            draggingTagPool.Value.Del(projectileEntity);

            slotCmp.Transform.gameObject.SetActive(true);
            if (!activeSlotTagPool.Value.Has(slotEntity)) activeSlotTagPool.Value.Add(slotEntity);
        }

        private void SetProjectileBackToSlot(int projectileEntity, int slotEntity)
        {
            ref var positionCmp = ref positionCmpPool.Value.Get(projectileEntity);
            ref var transformRefCmp = ref transformRefCmpPool.Value.Get(projectileEntity);
            ref var projectileCmp = ref projectileCmpPool.Value.Get(projectileEntity);
            ref var slotCmp = ref slotCmpPool.Value.Get(slotEntity);

            transformRefCmp.Value.gameObject.layer = (int)Layers.UI;
            transformRefCmp.Value.parent = slotCmp.Origin;
            projectileCmp.SpriteRenderer.sortingLayerID = SortingLayer.layers[(int)SortingLayers.UI].id;
            projectileCmp.SpriteRenderer.sortingOrder++;

            if (!positionTweenCmpPool.Value.Has(projectileEntity))
            {
                ref var positionTweenCmp = ref positionTweenCmpPool.Value.Add(projectileEntity);
                positionTweenCmp.StartPosition = positionCmp.Value;
                positionTweenCmp.EndPosition = slotCmp.Transform.position;
                positionTweenCmp.Duration = staticData.Value.BackToSlotDuration;
            }

            draggingTagPool.Value.Del(projectileEntity);

            slotCmp.Transform.gameObject.SetActive(true);
            if (!activeSlotTagPool.Value.Has(slotEntity)) activeSlotTagPool.Value.Add(slotEntity);
        }
    }
}