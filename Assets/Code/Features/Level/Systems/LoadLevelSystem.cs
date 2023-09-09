using UnityEngine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public class LoadLevelSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<LoadLevelEvent>, Exc<LevelCmp>> filter =  Worlds.PresentationWorld;
        private readonly EcsPoolInject<LevelCmp> levelCmpPool =  Worlds.PresentationWorld;

        private readonly EcsCustomInject<StaticData> staticData;
        private readonly EcsCustomInject<SceneData> sceneData;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in filter.Value)
            {
                ref var loadLevelEvent = ref filter.Pools.Inc1.Get(entity);
                var level = staticData.Value.Levels[loadLevelEvent.LevelIndex];

                sceneData.Value.Camera.orthographicSize = level.Size;

                ref var levelCmp = ref levelCmpPool.Value.Add(entity);
                levelCmp.Index = loadLevelEvent.LevelIndex;
                levelCmp.Level = Object.Instantiate(level);
                levelCmp.UI = Object.Instantiate(staticData.Value.LevelUI, sceneData.Value.UI);
            }
        }
    }
}