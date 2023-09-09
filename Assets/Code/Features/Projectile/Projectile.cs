using Leopotam.EcsLite;
using UnityEngine;

namespace JoyTeam
{
    [RequireComponent(typeof(Collider2D))]
    public class Projectile : PresenterBase
    {
        public SpriteRenderer SpriteRenderer;
        public Rigidbody2D Rigidbody2D;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!IsInitialized) return;

            var entityIsAlive = Entity.Unpack(World, out int entity);
            if (!entityIsAlive) return;

            EcsPool<CollisionEnterEvent> collisionEnterEventPool = World.GetPool<CollisionEnterEvent>();
            if (collisionEnterEventPool.Has(entity)) return;
            collisionEnterEventPool.Add(entity).Value = collision;
        }
    }
}