using UnityEngine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public class UnloadLevelSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject world = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<UnloadLevelEvent, LevelCmp>> unloadFilter = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<LevelElementCmp>> levelElementFilter = Worlds.PresentationWorld;

        private EcsPoolInject<LevelCmp> levelCmpPool = Worlds.PresentationWorld;

        public void Run(IEcsSystems systems)
        {   
            foreach(var levelEntity in unloadFilter.Value)
            {
                ref LevelCmp levelCmp = ref levelCmpPool.Value.Get(levelEntity);
                foreach(var levelElementEntity in levelElementFilter.Value)
                {
                    world.Value.DelEntity(levelElementEntity);
                }

                GameObject.Destroy(levelCmp.Level.gameObject);
                GameObject.Destroy(levelCmp.UI.gameObject);

                levelCmpPool.Value.Del(levelEntity);

                //if (restartLevelEventPool.Value.Has(levelEntity)) continue;
                //world.Value.DelEntity(levelEntity);
            }
        }
    }
}