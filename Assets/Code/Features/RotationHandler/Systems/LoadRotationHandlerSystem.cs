using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public class LoadRotationHandlerSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject world = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<LoadLevelEvent, LevelCmp>> loadLevelEventFilter = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<ShotPointCmp>> shotPointFilter = Worlds.PresentationWorld;
        
        private EcsPoolInject<LoadLevelEvent> loadLevelEventPool = Worlds.PresentationWorld;
        private EcsPoolInject<LevelCmp> levelCmpPool = Worlds.PresentationWorld;
        private EcsPoolInject<PositionCmp> positionCmpPool = Worlds.PresentationWorld;
        private EcsPoolInject<TransformRefCmp> transformRefCmpPool = Worlds.PresentationWorld;
        private EcsPoolInject<RotationHandlerCmp> rotationHandlerCmpPool = Worlds.PresentationWorld;
        private EcsPoolInject<LinkedToShotPointCmp> linkedToShotPointPool = Worlds.PresentationWorld;
        private EcsPoolInject<LevelElementCmp> levelElementCmpPool = Worlds.PresentationWorld;

        public void Run(IEcsSystems systems)
        {
            foreach (var levelEntity in loadLevelEventFilter.Value)
            {
                foreach (var levelObject in levelCmpPool.Value.Get(levelEntity).Level.LevelObjects)
                {
                    if (levelObject.GetType() != typeof(ShotPoint)) continue;
                    ShotPoint shotPointView = (ShotPoint)levelObject;

                    RotationHandler rotationHandlerView = shotPointView.RotationHandler;

                    int entity = world.Value.NewEntity();

                    positionCmpPool.Value.Add(entity);
                    transformRefCmpPool.Value.Add(entity).Value = rotationHandlerView.transform;

                    ref RotationHandlerCmp rotationHandlerCmp = ref rotationHandlerCmpPool.Value.Add(entity);
                    rotationHandlerCmp.MinZoneRadius = rotationHandlerView.MinZoneRadius;
                    rotationHandlerCmp.MaxZoneRadius = rotationHandlerView.MaxZoneRadius;

                    int necessaryEntity = -1;
                    foreach (var shotPointEntity in shotPointFilter.Value)
                    {
                        ref TransformRefCmp transformRefCmp = ref transformRefCmpPool.Value.Get(shotPointEntity);
                        if (shotPointView.transform != transformRefCmp.Value) continue;

                        necessaryEntity = shotPointEntity;
                    }

                    if (necessaryEntity != -1) linkedToShotPointPool.Value.Add(entity).Value = world.Value.PackEntity(necessaryEntity);

                    levelElementCmpPool.Value.Add(entity).LevelIndex = loadLevelEventPool.Value.Get(levelEntity).LevelIndex;
                }
            }
        }
    }
}