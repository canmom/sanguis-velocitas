using Latios;
using Unity.Entities;
using UnityEngine;

namespace SV
{
    public class ButtonBehaviors : MonoBehaviour
    {
        public void Quit()
        {
            Application.Quit();
        }

        public void ChangeScene(string newScene)
        {
            var latiosWorld = World.DefaultGameObjectInjectionWorld.Unmanaged.GetLatiosWorldUnmanaged();
            latiosWorld.sceneBlackboardEntity.AddComponentData(new RequestLoadScene { newScene = newScene });
        }

        public void RestartScene()
        {
            var latiosWorld = World.DefaultGameObjectInjectionWorld.Unmanaged.GetLatiosWorldUnmanaged();
            var currentScene = latiosWorld.worldBlackboardEntity.GetComponentData<CurrentScene>().current;
            latiosWorld.sceneBlackboardEntity.AddComponentData(new RequestLoadScene { newScene = currentScene });
        }
    }
}