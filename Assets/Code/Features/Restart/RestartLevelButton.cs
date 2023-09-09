using Leopotam.EcsLite;

namespace JoyTeam
{
    public class RestartLevelButton : PresenterBase
    {
        public void OnClick()
        {
            if (!World.IsAlive()) return;

            var entityIsAlive = Entity.Unpack(World, out int entity);
            if (!entityIsAlive) return;
            
            World.GetPool<ClickEvent>().Add(entity);
        }
    }
}