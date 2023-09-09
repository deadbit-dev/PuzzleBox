using UnityEngine.SceneManagement;
using Leopotam.EcsLite.Di;

namespace JoyTeam
{
    public abstract class LoadPhysicsLevelObjectBase<T> : LoadLevelObjectBase<T> where T : PhysicsPresenterBase
    {
        protected readonly EcsWorldInject PhysicsWorld = Worlds.PhysicsWorld;
        protected readonly EcsCustomInject<RuntimeData> RuntimeData = default;

        protected override void Load(int entity, T presenter)
        {
            //SceneManager.MoveGameObjectToScene(presenter.transform.gameObject, RuntimeData.Value.Scene);
        }
    }
}