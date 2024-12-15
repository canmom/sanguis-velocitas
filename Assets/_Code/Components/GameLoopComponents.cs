using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace SV
{
    public struct GameState : IComponentData
    {
        public bool win;
        public bool lose;
        public bool deathByPoison;
        public bool deathByPropulsion;
    }
}

