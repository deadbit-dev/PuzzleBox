using UnityEngine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public class ShotPointOnDownSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject world = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<ShotPointCmp, AttachedProjectile, ReadyForThrowingTag>> filter = Worlds.PresentationWorld;

        private readonly EcsPoolInject<AttachedProjectile> attachedProjectilePool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<TransformRefCmp> transformRefCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<AimingTag> aimingTagPool = Worlds.PresentationWorld;

        private readonly EcsCustomInject<SceneData> sceneData;

        public void Run(IEcsSystems systems)
        {
            if (!Input.GetMouseButtonDown(0)) return;

            var ray = sceneData.Value.Camera.ScreenPointToRay(Input.mousePosition);
            var hit = Physics2D.GetRayIntersection(ray);
            if (hit.transform == null) return;

            foreach (var shotPointEntity in filter.Value)
            {
                ref var attachedProjectile = ref attachedProjectilePool.Value.Get(shotPointEntity);

                var projectileIsAlive = attachedProjectile.Value.Unpack(world.Value, out var entity);
                if (!projectileIsAlive) continue;
                
                if (hit.transform.gameObject != transformRefCmpPool.Value.Get(entity).Value.gameObject) continue;
                aimingTagPool.Value.Add(shotPointEntity);
            }
        }
    }
}