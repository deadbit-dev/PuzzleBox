using UnityEngine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public class LoadProjectileSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject world = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<LoadLevelEvent, LevelCmp>> filter = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<SlotCmp>, Exc<ActiveTag>> slotFilter = Worlds.PresentationWorld;

        private readonly EcsPoolInject<PositionCmp> positionCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<TransformRefCmp> transformRefCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<ProjectileCmp> projectileCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<ActiveTag> activeSlotTagPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<LinkedToSlot> linkedToSlotPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<LevelElementCmp> levelElementCmpPool = Worlds.PresentationWorld;

        public void Run(IEcsSystems systems)
        {
            foreach (var levelEntity in filter.Value)
            {
                ref var levelCmp = ref filter.Pools.Inc2.Get(levelEntity);
                ref var projectiles = ref levelCmp.Level.Projectiles;
                for (var i = 0; (i < projectiles.Length) && (i < slotFilter.Value.GetEntitiesCount()); i++)
                {
                    var slotEntity = slotFilter.Value.GetRawEntities()[i];
                    ref var slotCmp = ref slotFilter.Pools.Inc1.Get(slotEntity);
                    slotCmp.Transform.gameObject.SetActive(true);
                    activeSlotTagPool.Value.Add(slotEntity);

                    var view = Object.Instantiate(levelCmp.Level.Projectiles[i], slotCmp.Origin);

                    view.transform.localScale = new Vector3(2.25f, 2.25f, 1); // FIXME: remove const value

                    view.gameObject.layer = (int)Layers.UI;
                    view.SpriteRenderer.sortingLayerID = SortingLayer.layers[(int)SortingLayers.UI].id;
                    view.SpriteRenderer.sortingOrder++;
                    view.Rigidbody2D.isKinematic = true;

                    var projectileEntity = world.Value.NewEntity();

                    positionCmpPool.Value.Add(projectileEntity);
                    transformRefCmpPool.Value.Add(projectileEntity).Value = view.transform;

                    ref var projectileCmp = ref projectileCmpPool.Value.Add(projectileEntity);
                    projectileCmp.SpriteRenderer = view.SpriteRenderer;
                    projectileCmp.Rigidbody2D = view.Rigidbody2D;

                    linkedToSlotPool.Value.Add(projectileEntity).Value = world.Value.PackEntity(slotEntity);
                    levelElementCmpPool.Value.Add(projectileEntity).LevelIndex = filter.Pools.Inc1.Get(levelEntity).LevelIndex;

                    view.Init(world.Value, world.Value.PackEntity(projectileEntity));
                }
            }
        }
    }
}