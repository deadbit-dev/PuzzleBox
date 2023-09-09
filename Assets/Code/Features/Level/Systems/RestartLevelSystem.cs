using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public class RestartLevelSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<RestartLevelEvent, LevelCmp>> filter = Worlds.PresentationWorld;

        private EcsPoolInject<UnloadLevelEvent> unloadLevelEventPool = Worlds.PresentationWorld;
        private EcsPoolInject<LoadLevelEvent> loadLevelEventPool = Worlds.PresentationWorld;

        public void Run(IEcsSystems systems)
        {
            foreach(var levelEntity in filter.Value)
            {
                unloadLevelEventPool.Value.Add(levelEntity);
                loadLevelEventPool.Value.Add(levelEntity);
            }
        }
    }
}