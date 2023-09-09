using UnityEngine;
using Leopotam.EcsLite;

namespace JoyTeam
{
    public abstract class PresenterBase : MonoBehaviour
    {
        protected EcsWorld World;
        protected EcsPackedEntity Entity;

        public void Init(EcsWorld world, EcsPackedEntity entity)
        {
            this.World = world;
            Entity = entity;
        }

        protected bool IsInitialized => World != null && World.IsAlive();
    }
}