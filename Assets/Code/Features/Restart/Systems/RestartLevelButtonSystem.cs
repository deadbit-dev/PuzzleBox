using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public class RestartLevelButtonSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<RestartLevelButtonTag, ClickEvent>> restartLevelButtonFilter = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<LevelCmp>, Exc<LoadLevelEvent, UnloadLevelEvent, RestartLevelEvent>> levelFilter = Worlds.PresentationWorld;

        private EcsPoolInject<LevelElementCmp> levelElementCmpPool = Worlds.PresentationWorld;
        private EcsPoolInject<LevelCmp> levelCmpPool = Worlds.PresentationWorld;
        private EcsPoolInject<RestartLevelEvent> restartLevelEventPool = Worlds.PresentationWorld;
        //private EcsPoolInject<RestartLevelButtonTag> restartLevelButtonTagPool = Worlds.PresentationWorld;

        public void Run(IEcsSystems systems)
        {
            foreach(var restartLevelButtonEntity in restartLevelButtonFilter.Value)
            {
                ref LevelElementCmp levelElementCmp = ref levelElementCmpPool.Value.Get(restartLevelButtonEntity);

                foreach (var levelEntity in levelFilter.Value)
                {
                    ref LevelCmp levelCmp = ref levelCmpPool.Value.Get(levelEntity);
                    if (levelElementCmp.LevelIndex != levelCmp.Index) continue;

                    restartLevelEventPool.Value.Add(levelEntity);
                }

                //restartLevelButtonTagPool.Value.Del(restartLevelButtonEntity);
            }
        }
    }
}