using UnityEngine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public class ProjectileOnDragSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PositionCmp, ProjectileCmp, LinkedToSlot, DraggingTag>> filter = Worlds.PresentationWorld;

        private EcsPoolInject<PositionCmp> positionCmpPool = Worlds.PresentationWorld;

        private EcsCustomInject<SceneData> sceneData;

        public void Run(IEcsSystems systems)
        {
            Vector2 touchScreenPos = Input.mousePosition;
            Vector3 touchWorldPos = sceneData.Value.Camera.ScreenToWorldPoint(touchScreenPos);
            touchWorldPos.z = 0;

            foreach (var projectileEntity in filter.Value)
            {
                positionCmpPool.Value.Get(projectileEntity).Value = touchWorldPos;
            }
        }
    }
}