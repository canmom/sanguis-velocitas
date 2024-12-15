using Latios;
using Unity.Entities;
using UnityEngine;

namespace SV
{
    public class LevelEndUI : MonoBehaviour
    {
        public GameObject winThingToEnable;
        public GameObject loseThingToEnable;
        public GameObject poisonThingToEnable;
        public GameObject propulsionThingToEnable;
        public GameObject poisonAndPropulsionThingToEnable;

        LatiosWorldUnmanaged latiosWorld;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            latiosWorld = World.DefaultGameObjectInjectionWorld.Unmanaged.GetLatiosWorldUnmanaged();
        }

        // Update is called once per frame
        void Update()
        {
            var state = latiosWorld.sceneBlackboardEntity.GetComponentData<GameState>();
            if (state.win)
                winThingToEnable.SetActive(true);
            else if (state.lose)
            {
                loseThingToEnable.SetActive(true);
                if (state.deathByPoison && state.deathByPropulsion)
                    poisonAndPropulsionThingToEnable.SetActive(true);
                else if (state.deathByPoison)
                    poisonThingToEnable.SetActive(true);
                else if (state.deathByPropulsion)
                    propulsionThingToEnable.SetActive(true);
            }
        }
    }
}

