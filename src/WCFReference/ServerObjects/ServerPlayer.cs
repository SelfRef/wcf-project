using Microsoft.Xna.Framework;
using WCFReference.Objects;
using Newtonsoft.Json;

namespace WCFReference.ServerObjects
{

    public class ServerPlayer : ServerObject
    {
        public new enum BodyTypes
        {
            Pedestrian
        }

        public new BodyTypes BodyType { get; set; }

        [JsonIgnore]
        public ServerObject InsideObject { get; set; } = null;
        public int? InsideID { get; set; } = null;

        public string Name { get; set; }

        public ServerPlayer(GameObject obj, string name, Vector2 position, float angle, BodyTypes bodyType, bool active, int? id) : base(obj, position, angle, active, null, id)
        {
            Name = name;
            BodyType = bodyType;
        }

        public override void UpdateData()
        {
            if (InsideID.HasValue)
            {
                Position = InsideObject.Position;
                Angle = InsideObject.Angle;

                GameObject.Position = Position;
                GameObject.Angle = Angle;
            }
            else base.UpdateData();
        }

        public override void UpdateObject(PositionData pos)
        {
            base.UpdateObject(pos);

            if(InsideID.HasValue)
            {
                InsideObject.Position = pos.Position;
                InsideObject.GameObject.Position = pos.Position;

                InsideObject.Angle = pos.Angle;
                InsideObject.GameObject.Angle = pos.Angle;
            }
        }

        public void SetInside(int id, ServerObject obj)
        {
            InsideID = id;
            InsideObject = obj;

            UpdateData();
        }

        public void UnsetInside()
        {
            InsideID = null;
            InsideObject = null;
        }
    }
}
