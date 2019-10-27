using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using WCFReference.ServerObjects;

namespace WCFReference.Objects
{
    public interface IPlayable
    {
        bool CanEnter { get; set; }
        ServerObject Inside { get; set; }
        float Speed { get; set; }
        Body Body { get; set; }
        Vector2 Position { get; set; }
        void UpdateByPlayer(Controll ctrl);
        bool Enabled { get; set; }
        float Angle { get; set; }
    }
}
