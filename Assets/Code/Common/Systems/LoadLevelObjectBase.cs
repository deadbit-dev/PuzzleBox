using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public abstract class LoadLevelObjectBase<T> : IEcsRunSystem where T : PresenterBase
    {
        protected readonly EcsWorldInject PresentationWorld = Worlds.PresentationWorld;
        protected readonly EcsFilterInject<Inc<LoadLevelEvent, LevelCmp>> LevelFilter = Worlds.PresentationWorld;
        
        protected readonly EcsPoolInject<PositionCmp> PositionCmpPool = Worlds.PresentationWorld;
        protected readonly EcsPoolInject<TransformRefCmp> TransformRefCmpPool = Worlds.PresentationWorld;
        protected readonly EcsPoolInject<LevelElementCmp> LevelElementCmpPool = Worlds.PresentationWorld;

        public void Run(IEcsSystems systems)
        {
            foreach (var levelEntity in LevelFilter.Value)
            {
                ref var levelCmp = ref LevelFilter.Pools.Inc2.Get(levelEntity).Level;
                foreach (var levelObject in levelCmp.LevelObjects)
                {
                    if(levelObject.GetType() != typeof(T)) continue;
                    
                    var presenter = (T) levelObject;
                    var entity = PresentationWorld.Value.NewEntity();
            
                    PositionCmpPool.Value.Add(entity);
                    TransformRefCmpPool.Value.Add(entity).Value = presenter.transform;
                    
                    Load(entity, presenter);
            
                    ref var loadLevelEvent = ref LevelFilter.Pools.Inc1.Get(levelEntity); 
                    LevelElementCmpPool.Value.Add(entity).LevelIndex = loadLevelEvent.LevelIndex;
                    
                    presenter.Init(PresentationWorld.Value, PresentationWorld.Value.PackEntity(entity));
                }
            }
        }

        protected abstract void Load(int entity, T presenter);
    }
}