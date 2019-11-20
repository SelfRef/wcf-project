using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WCFReference.Objects
{
  public class Bullet : GameObject
  {
    public Bullet(World world, Texture2D tex, Vector2 position, float angle) : base(world, null, tex, null, position, angle)
    {
      var bodyWidth = ConvertUnits.ToSimUnits(5);
      var bodyHeight = ConvertUnits.ToSimUnits(10);
      var bodyPosition = ConvertUnits.ToSimUnits(position);
      Body = BodyFactory.CreateRectangle(world, bodyWidth, bodyHeight, 1, bodyPosition);
      Origin = new Vector2(5, 5);
      Body.BodyType = BodyType.Dynamic;
      Body.IsBullet = true;
      Body.Mass = 0.005f;
      Body.Restitution = 0.5f;
      Body.LinearDamping = 1;
      Body.AngularDamping = 2;
      Body.Rotation = angle;
    }
  }
}
