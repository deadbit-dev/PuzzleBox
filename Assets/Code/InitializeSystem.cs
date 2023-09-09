using UnityEngine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public class InitializeSystem : IEcsInitSystem
    {
        private readonly EcsWorldInject world = Worlds.PresentationWorld;

        private readonly EcsPoolInject<LoadLevelEvent> loadLevelEventPool = Worlds.PresentationWorld;

        public void Init(IEcsSystems systems)
        {
            var loadLevelEventEntity = world.Value.NewEntity();
            loadLevelEventPool.Value.Add(loadLevelEventEntity).LevelIndex = 0;

            Application.targetFrameRate = Screen.currentResolution.refreshRate;

#if UNITY_EDITOR_WIN
            Time.fixedDeltaTime = 0.008f;
#elif PLATFORM_ANDROID
            Time.fixedDeltaTime = 0.016f;
#endif
        }
    }
}