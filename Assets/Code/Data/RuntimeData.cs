using UnityEngine;
using UnityEngine.SceneManagement;

namespace JoyTeam
{
    public sealed class RuntimeData
    {
        public Scene Scene;
        public PhysicsScene2D PhysicsScene;
        
        public Scene PredictionScene;
        public PhysicsScene2D PredictionPhysicsScene;
    }
}