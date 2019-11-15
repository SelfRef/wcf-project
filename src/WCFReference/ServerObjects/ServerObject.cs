using System;
using Microsoft.Xna.Framework;
using WCFReference.Objects;
using Newtonsoft.Json;

namespace WCFReference.ServerObjects
{
    public class ServerObject
    {
        public enum BodyTypes
        {
            Car,
            Bullet,
        }
        public virtual BodyTypes? BodyType { get; set; }

        [JsonIgnore]
        public GameObject GameObject;

        public int ID { get; set; }

        public Vector2 Position { get; set; }

        public float Angle { get; set; }

        public bool Active { get; set; }

        public Vector2? VelocitySnap { get; set; }

        public ServerObject(GameObject gameObject, Vector2 position, float angle, bool active, BodyTypes? bodyType = null, int? id = null)
        {
            Random rm = new Random();
            ID = id ?? rm.Next();

            GameObject = gameObject;
            BodyType = bodyType;
            Position = position;
            Active = active;
            Angle = angle;
        }

        public virtual void UpdateData()
        {
            Position = GameObject.Position;
            Angle = GameObject.Angle;
            Active = GameObject.Enabled;
        }

        public virtual void UpdateObject(PositionData pos)
        {
            Position = pos.Position;
            GameObject.Position = pos.Position;

            Angle = pos.Angle;
            GameObject.Angle = pos.Angle;

            VelocitySnap = pos.VelocitySnap;
        }
    }
}
