using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelcroPhysics.Utilities;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WCFReference.Objects
{
  public class Bullet : GameObject
  {
    public Bullet(World world, Texture2D tex, Vector2 position, float angle) : base(world, null, tex, null, position, angle)
    {
      TexRect = new Rectangle((int)position.X, (int)position.Y, 10, 10);
      var bodyHeight = ConvertUnits.ToSimUnits(10);
      var bodyWidth = ConvertUnits.ToSimUnits(10);
      var bodyPosition = ConvertUnits.ToSimUnits(position);
      Body = BodyFactory.CreateRectangle(world, bodyWidth, bodyHeight, 1, bodyPosition);
      Body.BodyType = BodyType.Dynamic;
      //Body.IsBullet = true;
      Body.Mass = 0.0010f;
      Body.Rotation = angle;
      Debug.WriteLine("Bullet created");

      Body.LinearVelocity = new Vector2(5);
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2? velSnap = null)
    {
      base.Draw(spriteBatch, velSnap);
      Debug.WriteLine($"Position: {Body.Position}");
      Debug.WriteLine($"World Position: {Body.WorldCenter}");
    }
  }
}
