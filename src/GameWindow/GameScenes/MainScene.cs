using FarseerPhysics;
using FarseerPhysics.DebugView;
using WCFReference.GameScenes.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using WCFReference;
using WCFReference.Objects;
using GameWindow.Objects;

namespace GameWindow.GameScenes
{
  public class MainScene : IGameScene
  {
    private IConnection con;
    private Container cont;

    private SceneManager manag;
    private SpriteBatch spriteBatch;
    private DebugViewXNA debug;
    private Camera2D cam;

    private bool multi;
    private bool showDebug;

    public KeyboardState keys;      //
    public KeyboardState prevKeys;  //  Keyboard and mouse state objects.
    public MouseState mouse;        //
    public MouseState prevMouse;    //

    public MainScene(SceneManager manager) // TODO: Must clean up a little here.
    {
      manag = manager;
      con = manag.Connection;
      multi = manag.multi;
    }

    public void Initialize()
    {
      manag.IsMouseVisible = true; // Nope, it's not.
      con.Channel.GameOpen(true); // Tell server I'm in game.
      cont = new Container(manag, con); // Create container.

      cam = new Camera2D(manag.GraphicsDevice);
      // Camera bounds clamp to map size.
      cam.MinPosition = new Vector2(manag.GraphicsDevice.Viewport.Width / 2, manag.GraphicsDevice.Viewport.Height / 2);
      cam.MaxPosition = new Vector2(cont.Map.TmxMap.Width * cont.Map.TmxMap.TileWidth - manag.GraphicsDevice.Viewport.Width / 2, cont.Map.TmxMap.Height * cont.Map.TmxMap.TileHeight - manag.GraphicsDevice.Viewport.Height / 2);
      cam.Position = new Vector2(manag.GraphicsDevice.Viewport.Width / 2, manag.GraphicsDevice.Viewport.Height / 2);
      cam.Jump2Target(); // Speed up animation at beginning.

      showDebug = false; // At start debug view is disable. Press TAB in game window to enable instantly.
      debug = new DebugViewXNA(cont.World);

      #region Debug Settings // Some debug flags.

      //debug.AppendFlags(DebugViewFlags.AABB);
      debug.AppendFlags(DebugViewFlags.CenterOfMass);
      debug.AppendFlags(DebugViewFlags.ContactNormals);
      debug.AppendFlags(DebugViewFlags.ContactPoints);
      debug.AppendFlags(DebugViewFlags.Controllers);
      debug.AppendFlags(DebugViewFlags.DebugPanel);
      debug.AppendFlags(DebugViewFlags.Joint);
      debug.AppendFlags(DebugViewFlags.PerformanceGraph);
      //debug.AppendFlags(DebugViewFlags.PolygonPoints);
      debug.AppendFlags(DebugViewFlags.Shape);
      debug.LoadContent(manag.GraphicsDevice, manag.Content);

      #endregion
    }

    /// <summary>
    /// Load a game content like textures and fonts.
    /// </summary>
    public void LoadContent()
    {
      spriteBatch = new SpriteBatch(manag.GraphicsDevice);

      cont.HumanTS = new TileSet(manag.Content.Load<Texture2D>("People/man"), 4, 1, new Vector2(42, 98));
      cont.CarTX = manag.Content.Load<Texture2D>("Cars/enemy");
      cont.BulletTX = manag.Content.Load<Texture2D>("Materials/bullet");
      cont.LifeSymTX = manag.Content.Load<Texture2D>("Materials/heart");
      cont.FontTX = manag.Content.Load<SpriteFont>("Fonts/Century");
      cont.DotTX = Map.GetDot(manag.GraphicsDevice);

      cont.Map.LoadTextures(manag.Content); // Load map tiles.

    }

    /// <summary>
    /// Update everything like positions.
    /// </summary>
    /// <param name="gameTime">Time span object.</param>
    public void Update(GameTime gameTime)
    {
      keys = Keyboard.GetState();
      mouse = Mouse.GetState();

      #region General Key Actions
      if (manag.IsActive)
      {
        if (keys.IsKeyDown(Keys.Escape)) manag.ExitGame();
        if (keys.IsKeyDown(Keys.Subtract)) cam.Zoom -= 0.01f;
        if (keys.IsKeyDown(Keys.Add)) cam.Zoom += 0.01f;
        if (prevKeys.IsKeyDown(Keys.Tab) && keys.IsKeyUp(Keys.Tab)) showDebug = !showDebug;
      }
      #endregion

      cont.Update(gameTime, cam, keys, prevKeys, mouse, prevMouse); // Update container.

      cam.Update(gameTime); // Update camera.

      prevKeys = keys;
      prevMouse = mouse;
    }

    /// <summary>
    /// Draw it.
    /// </summary>
    /// <param name="gameTime">Time span object.</param>
    public void Draw(GameTime gameTime)
    {
      manag.GraphicsDevice.Clear(Color.CornflowerBlue);
      spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, cam.View);
      cont.Map.Draw(spriteBatch, Map.DrawLevel.Bottom);

      cont.Draw(spriteBatch); // Use container.

      cont.Map.Draw(spriteBatch, Map.DrawLevel.Top);
      spriteBatch.End();

      #region Debug Draw
      if (showDebug)
      {
        debug.RenderDebugData(cam.SimProjection, cam.SimView);
      }
      #endregion
    }
  }
}
