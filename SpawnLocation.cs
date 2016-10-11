using UnityEngine;

namespace AntiSuicide
{
    class SpawnLocation
    {
        public Vector3 Position { get; private set; }
        public float Angle { get; private set; }

        public SpawnLocation(Vector3 position, float angle)
        {
            Position = position;
            Angle = angle;
        }
    }
}
