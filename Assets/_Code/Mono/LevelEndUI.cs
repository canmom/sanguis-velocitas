using Latios;
using TMPro;
using Unity.Entities;
using UnityEngine;

namespace SV
{
    public class LevelEndUI : MonoBehaviour
    {
        public GameObject winThingToEnable;
        public GameObject loseThingToEnable;
        public TextMeshProUGUI failureDescription;

        [TextArea(1, 5)]
        public string poisonedText;
        [TextArea(1, 5)]
        public string bledOutText;
        [TextArea(1, 5)]
        public string poisonedAndBledOutText;

        LatiosWorldUnmanaged latiosWorld;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            latiosWorld = World.DefaultGameObjectInjectionWorld.Unmanaged.GetLatiosWorldUnmanaged();
        }

        // Update is called once per frame
        void Update()
        {
            if (!latiosWorld.sceneBlackboardEntity.HasComponent<GameState>()) {return;}
            var state = latiosWorld.sceneBlackboardEntity.GetComponentData<GameState>();
            if (state.win)
            {
                winThingToEnable.SetActive(true);
            }
            else if (state.lose)
            {
                loseThingToEnable.SetActive(true);
                if (state.deathByPoison && state.deathByPropulsion)
                {
                    failureDescription.text = poisonedAndBledOutText;
                }
                else if (state.deathByPoison)
                {
                    failureDescription.text = poisonedText;
                }
                else if (state.deathByPropulsion)
                {
                    failureDescription.text = bledOutText;
                }
            }
        }
    }
}

