using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public class LoadShotPointSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject world = Worlds.PresentationWorld;
        private readonly EcsFilterInject<Inc<LoadLevelEvent, LevelCmp>> filter = Worlds.PresentationWorld;

        private readonly EcsPoolInject<LoadLevelEvent> loadLevelEventPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<LevelCmp> levelCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<PositionCmp> positionCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<TransformRefCmp> transformRefCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<ShotPointCmp> shotPointCmpPool = Worlds.PresentationWorld;
        private readonly EcsPoolInject<LevelElementCmp> levelElementCmpPool = Worlds.PresentationWorld;

        public void Run(IEcsSystems systems)
        {
            foreach(var levelEntity in filter.Value)
            {
                foreach (var levelObject in levelCmpPool.Value.Get(levelEntity).Level.LevelObjects)
                {
                    if (levelObject.GetType() != typeof(ShotPoint)) continue;
                    var view = (ShotPoint) levelObject;

                    var entity = world.Value.NewEntity();

                    positionCmpPool.Value.Add(entity);
                    transformRefCmpPool.Value.Add(entity).Value = view.transform;

                    ref var shotPointCmp = ref shotPointCmpPool.Value.Add(entity);
                    shotPointCmp.CatchRadius = view.CatchRadius;
                    shotPointCmp.DismissRadius= view.DismissRadius;
                    shotPointCmp.MaxDistance = view.MaxDistance;
                    shotPointCmp.ForceMultiplier = view.ForceMultiplier;
                    shotPointCmp.InterferingFactor = view.InterferingFactor;
                    shotPointCmp.InterferingDistance = view.InterferingDistance;

                    levelElementCmpPool.Value.Add(entity).LevelIndex = loadLevelEventPool.Value.Get(levelEntity).LevelIndex;

                    view.Init(world.Value, world.Value.PackEntity(entity));
                }
            }
        }
    }
}