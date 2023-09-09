using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public class UpdateTransformFromPosition : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PositionCmp, TransformRefCmp>> filter = Worlds.PresentationWorld;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in filter.Value)
            {
                filter.Pools.Inc2.Get(entity).Value.position = filter.Pools.Inc1.Get(entity).Value;
            }
        }
    }
}