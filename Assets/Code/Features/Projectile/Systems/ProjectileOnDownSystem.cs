using UnityEngine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public class ProjectileOnDownSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<LevelCmp>> levelFilter = Worlds.PresentationWorld;
        // TODO: ADD THE BUSY Cmp FOR ENTITIES WHO DONT WANT TO BE OF THE CONTROL BY A USER
        private readonly EcsFilterInject<Inc<TransformRefCmp, ProjectileCmp, LinkedToSlot>, Exc<DraggingTag, PositionTweenCmp>> projectileFilter = Worlds.PresentationWorld;

        private EcsPoolInject<LevelCmp> levelCmpPool = Worlds.PresentationWorld;
        private EcsPoolInject<TransformRefCmp> transformRefCmpPool = Worlds.PresentationWorld;
        private EcsPoolInject<ProjectileCmp> projectileCmpPool = Worlds.PresentationWorld;
        private EcsPoolInject<DraggingTag> draggingTagPool = Worlds.PresentationWorld;

        private EcsCustomInject<SceneData> sceneData;

        public void Run(IEcsSystems systems)
        {
            if (!Input.GetMouseButtonDown(0)) return;

            Vector2 touchScreenPos = Input.mousePosition;
            Ray ray = sceneData.Value.Camera.ScreenPointToRay(touchScreenPos);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
            if (hit.transform == null) return;

            foreach(var levelEntity in levelFilter.Value)
            {
                ref LevelCmp levelCmp = ref levelCmpPool.Value.Get(levelEntity);
                foreach (var projectileEntity in projectileFilter.Value)
                {
                    ref TransformRefCmp transformRefCmp = ref transformRefCmpPool.Value.Get(projectileEntity);
                    ref ProjectileCmp projectileCmp = ref projectileCmpPool.Value.Get(projectileEntity);
                    if (hit.transform.gameObject != transformRefCmp.Value.gameObject) continue;

                    transformRefCmp.Value.parent = levelCmp.Level.transform;
                    projectileCmp.SpriteRenderer.sortingLayerID = SortingLayer.layers[(int)SortingLayers.Selected].id;

                    draggingTagPool.Value.Add(projectileEntity);
                }
            }
        }
    }
}