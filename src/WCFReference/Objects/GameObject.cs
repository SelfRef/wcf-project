using VelcroPhysics.Utilities;
using VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WCFReference.Objects
{
  public class GameObject
  {
    public GameObject(World world, Body body = null, Texture2D tex = null, Rectangle? rect = null, Vector2? position = null, float? angle = null)
    {
      World = world;
      Body = body;
      if (Body != null)
        Body.BodyType = BodyType.Dynamic;
      Texture = tex;
      Position = position ?? Vector2.Zero;
      if (rect.HasValue) TexRect = rect.Value;
      if (angle.HasValue) Angle = angle.Value;
    }
    public World World { get; set; }
    public Body Body { get; set; }
    public Texture2D Texture { get; set; }
    public Rectangle _TexRect;
    public Rectangle TexRect
    {
      get
      {
        return _TexRect;
      }
      set
      {
        _TexRect = value;
        Origin = new Vector2(TexRect.Width / 2, TexRect.Height / 2);
      }
    }
    public Vector2 Origin
    {
      get
      {
        return ConvertUnits.ToDisplayUnits(Body.LocalCenter);
      }
      set
      {
        if (Body != null)
          Body.LocalCenter = ConvertUnits.ToSimUnits(value);
      }
    }
    public Vector2 Position
    {
      get
      {
        return ConvertUnits.ToDisplayUnits(Body.WorldCenter);
      }
      set
      {
        if (Body != null)
          Body.Position = ConvertUnits.ToSimUnits(value);
      }
    }
    public float Angle
    {
      get
      {
        return Body.Rotation;
      }
      set
      {
        if (Body != null)
          Body.Rotation = value;
      }
    }
    public Vector2 Size
    {
      get
      {
        return new Vector2(TexRect.Width, TexRect.Height);
      }
      set
      {
        TexRect = new Rectangle(TexRect.X, TexRect.Y, (int)value.X, (int)value.Y);
      }
    }

    public bool Enabled
    {
      get
      {
        return Body.Enabled;
      }
      set
      {
        Body.Enabled = value;
      }
    }

    public virtual void UpdateByPlayer(Control ctrl)
    {
      Body.ApplyForce(new Vector2((ctrl.Back ? 1 : 0) + (ctrl.Front ? -1 : 0), (ctrl.Right ? 1 : 0) + (ctrl.Left ? -1 : 0)));
    }

    public virtual void UpdatePosition(Vector2 position, float? angle)
    {
      Position = position;
      if (angle.HasValue)
      {
        Angle = angle.Value;
      }
    }

    public virtual void Draw(SpriteBatch spriteBatch, Vector2? velSnap = null)
    {
      spriteBatch.Draw(Texture, Position, origin: Origin, rotation: Angle);
    }
  }
}
