using UnityEngine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public class ShotPointOnUpSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject world = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<PositionCmp, ShotPointCmp, AttachedProjectile, AimingTag>> filter = Worlds.PresentationWorld;

        private readonly EcsPoolInject<PositionCmp> positionCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<TransformRefCmp> transformRefCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<ProjectileCmp> projectileCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<ShotPointCmp> shotPointCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<AttachedProjectile> attachedProjectilePool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<ReadyForThrowingTag> readyForThrowingTagPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<AimingTag> aimingTagPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<ThrewTag> threwTagPool = Worlds.PresentationWorld;

        public void Run(IEcsSystems systems)
        {
            if (!Input.GetMouseButtonUp(0)) return;

            foreach (var shotPointEntity in filter.Value)
            {
                ref var shotPointPositionCmp = ref positionCmpPool.Value.Get(shotPointEntity);
                ref var shotPointCmp = ref shotPointCmpPool.Value.Get(shotPointEntity);
                ref var attachedProjectile = ref attachedProjectilePool.Value.Get(shotPointEntity);

                var projectileIsAlive = attachedProjectile.Value.Unpack(world.Value, out var entity);
                if (!projectileIsAlive) continue;
                
                ref var positionCmp = ref positionCmpPool.Value.Get(entity);
                ref var projectileTransformRefCmp = ref transformRefCmpPool.Value.Get(entity);
                ref var projectileCmp = ref projectileCmpPool.Value.Get(entity);

                // leave projectile on ShotPoint
                var distance = Vector3.Distance(shotPointPositionCmp.Value, positionCmp.Value);
                if (distance < shotPointCmp.DismissRadius)
                {
                    positionCmp.Value = shotPointPositionCmp.Value;
                    aimingTagPool.Value.Del(shotPointEntity);
                    continue;
                }

                // shot
                projectileTransformRefCmp.Value.gameObject.layer = (int)Layers.Game;
                projectileCmp.SpriteRenderer.sortingOrder = (int)Layers.Game;
                projectileCmp.Rigidbody2D.isKinematic = false;
                projectileCmp.Rigidbody2D.AddForce(shotPointCmp.CurrentForce, ForceMode2D.Impulse);

                threwTagPool.Value.Add(entity);

                attachedProjectilePool.Value.Del(shotPointEntity);
                aimingTagPool.Value.Del(shotPointEntity);
                readyForThrowingTagPool.Value.Del(shotPointEntity);
            }
        }
    }
}