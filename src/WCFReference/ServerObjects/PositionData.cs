using Microsoft.Xna.Framework;

namespace WCFReference.ServerObjects
{
    public class PositionData
    {
        public Vector2 Position { get; set; }
        public float Angle { get; set; }
        public Vector2 VelocitySnap { get; set; }

        public PositionData(Vector2 position, float angle, Vector2 velocity)
        {
            Position = position;
            Angle = angle;
            VelocitySnap = velocity;
        }
    }
}
