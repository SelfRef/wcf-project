using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using GameWindow.GameScenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using WCFReference;
using WCFReference.Objects;
using WCFReference.ServerObjects;

namespace GameWindow.Objects
{
    public class Container
    {
        /// <summary>
        /// Connection object.
        /// </summary>
        public IConnection Connection { get; private set; }
        /// <summary>
        /// Scene manager object.
        /// </summary>
        public SceneManager Manager { get; private set; }
        /// <summary>
        /// Name of current player.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Tile map.
        /// </summary>
        public Map Map { get; private set; }
        /// <summary>
        /// World object contains all phisics objects.
        /// </summary>
        public World World { get; private set; }
        /// <summary>
        /// Contains game bodies of players.
        /// </summary>
        public Dictionary<string, GameObject> PlayersObj { get; private set; }
        /// <summary>
        /// Contains all other bodies.
        /// </summary>
        public Dictionary<int, GameObject> ObjectsObj { get; private set; }
        /// <summary>
        /// Updated list of Player objects from server.
        /// </summary>
        public Dictionary<string, ServerPlayer> Players { get; private set; }
        /// <summary>
        /// Updated list of other objects from server.
        /// </summary>
        public Dictionary<int, ServerObject> Objects { get; private set; }
        /// <summary>
        /// Tileset with pedestrian texture.
        /// </summary>
        public TileSet HumanTS { get; set; }
        /// <summary>
        /// Texture of cars.
        /// </summary>
        public Texture2D CarTX { get; set; }
        /// <summary>
        /// Font texture.
        /// </summary>
        public SpriteFont FontTX { get; set; }
        /// <summary>
        /// Just dot. Nothing more.
        /// </summary>
        public Texture2D DotTX { get; set; }


        private List<string> playersToDelete;
        private List<int> objectsToDelete;
        private PositionData position;

        /// <summary>
        /// Provides container for better managing game methods.
        /// </summary>
        /// <param name="manager">Screen manager.</param>
        /// <param name="con">Connection object.</param>
        public Container(SceneManager manager, IConnection con)
        {
            Connection = con;
            Manager = manager;
            Name = con.Name;
            Players = con.Players;
            Objects = con.Objects;
            World = new World(Vector2.Zero);
            Map = new Map();
            PlayersObj = new Dictionary<string, GameObject>();
            ObjectsObj = new Dictionary<int, GameObject>();

            playersToDelete = new List<string>();
            objectsToDelete = new List<int>();

            Map.DrawCollisions(World);
        }

        /// <summary>
        /// Update step in game world.
        /// </summary>
        /// <param name="gameTime">Time span object.</param>
        /// <param name="cam">Camera object.</param>
        /// <param name="keys">Current keyboard state.</param>
        /// <param name="prevKeys">Previous keyboard state.</param>
        /// <param name="mouse">Current mouse state.</param>
        /// <param name="prevMouse">Previous mouse state.</param>
        public void Update(GameTime gameTime, Camera2D cam, KeyboardState keys, KeyboardState prevKeys, MouseState mouse, MouseState prevMouse)
        {
            Connection.UpdatePlayers(); // Update Player objects.
            Connection.UpdateObjects(); // Update all other objects.
            Players = Connection.Players; // Import reference
            Objects = Connection.Objects; // ...

            foreach (var pl in Players) // Creating Player game objects based on imported data.
            {
                if (!PlayersObj.ContainsKey(pl.Key)) PlayersObj.Add(pl.Key, new Pedestrian(World, 10, HumanTS, pl.Value.Position, pl.Value.Angle));
                else if(pl.Key != Name)
                {
                    PlayersObj[pl.Key].Position = pl.Value.Position; // Update position if object already exist.
                    PlayersObj[pl.Key].Angle = pl.Value.Angle;
                }
                if (pl.Value.InsideID.HasValue) PlayersObj[pl.Key].Enabled = false;
                else PlayersObj[pl.Key].Enabled = true;
            }
            foreach (var pl in PlayersObj) if (!Players.ContainsKey(pl.Key)) playersToDelete.Add(pl.Key); // Check for not-existing objects...
            foreach (var key in playersToDelete) PlayersObj.Remove(key); // ...and delete them.

            foreach (var ob in Objects) // The same for objects.
            {
                if(!ObjectsObj.ContainsKey(ob.Key))
                {
                    if (ob.Value.BodyType == ServerObject.BodyTypes.Car) ObjectsObj.Add(ob.Key, new Car(World, CarTX, ob.Value.Position, ob.Value.Angle, Car.Type.Type1, Car.CarColor.Color1, DotTX, null));
                    // TODO: Add other types.
                }
                else if(ob.Key != Players[Name].InsideID)
                {
                    ObjectsObj[ob.Key].Position = ob.Value.Position;
                    ObjectsObj[ob.Key].Angle = ob.Value.Angle;
                }
            }
            foreach (var ob in ObjectsObj) if (!Objects.ContainsKey(ob.Key)) objectsToDelete.Add(ob.Key); // Also check for usused objects.
            foreach (var key in objectsToDelete) ObjectsObj.Remove(key);

            if (Manager.IsActive) // Handle input only when window is focused.
            {
                if (keys.IsKeyDown(Keys.Escape)) Manager.ExitGame(); // Exit on escape key.

                Controll controll = new Controll(keys.IsKeyDown(Keys.W), keys.IsKeyDown(Keys.S), keys.IsKeyDown(Keys.A), keys.IsKeyDown(Keys.D), keys.IsKeyDown(Keys.Space), keys.IsKeyDown(Keys.LeftShift)); // Create Controll object contains pressed keys.
                if (!Players[Name].InsideID.HasValue)
                {
                    if ((Players[Name].BodyType == ServerPlayer.BodyTypes.Pedestrian) && (prevMouse.Position != mouse.Position)) // Mouse controll.
                    {
                        if (mouse.LeftButton == ButtonState.Pressed) controll.Front = true;
                        PlayersObj[Name].Angle = (float)Math.Atan2(PlayersObj[Name].Position.Y - (mouse.Y + cam.Position.Y - Manager.GraphicsDevice.Viewport.Height / 2), PlayersObj[Name].Position.X - (mouse.X + cam.Position.X - Manager.GraphicsDevice.Viewport.Width / 2)) - MathHelper.ToRadians(90);
                    }
                    PlayersObj[Name].Update(controll);
                    position = new PositionData(PlayersObj[Name].Position, PlayersObj[Name].Angle, PlayersObj[Name].Body.LinearVelocity); // Create position data using to send vector, angle and velocity.
                }
                else
                {
                    // TODO: Support for mouse control while in car. Not done yet!!!
                    //if ((Objects[Players[Name].InsideID.Value].BodyType == ServerObject.BodyTypes.Car) && (prevMouse.Position != mouse.Position))
                    //{
                    //    if (mouse.LeftButton == ButtonState.Pressed) controll.Front = true;
                    //    float curve = (float)Math.Atan2(PlayersObj[Name].Position.Y - (mouse.Y + cam.Position.Y - Manager.GraphicsDevice.Viewport.Height / 2), PlayersObj[Name].Position.X - (mouse.X + cam.Position.X - Manager.GraphicsDevice.Viewport.Width / 2)) - MathHelper.ToRadians(90);
                    //    (ObjectsObj[Players[Name].InsideID.Value] as Car).mouse = true;
                    //    (ObjectsObj[Players[Name].InsideID.Value] as Car).Wheels[0].Angle = curve;
                    //    (ObjectsObj[Players[Name].InsideID.Value] as Car).Wheels[1].Angle = curve;
                    //}
                    ObjectsObj[Players[Name].InsideID.Value].Update(controll);
                    PlayersObj[Name].Position = ObjectsObj[Players[Name].InsideID.Value].Position + MathUtils.Mul(new Rot(ObjectsObj[Players[Name].InsideID.Value].Angle), new Vector2(-40, 20)); // Add offset for Player object, when leave car.
                    PlayersObj[Name].Angle = ObjectsObj[Players[Name].InsideID.Value].Angle;

                    position = new PositionData(ObjectsObj[Players[Name].InsideID.Value].Position, ObjectsObj[Players[Name].InsideID.Value].Angle, ObjectsObj[Players[Name].InsideID.Value].Body.LinearVelocity);
                }
                

                if(mouse.RightButton == ButtonState.Released && prevMouse.RightButton == ButtonState.Pressed) // Car entering mechanism.
                {
                    if (!Players[Name].InsideID.HasValue) // Is already in?
                    {
                        Fixture test = World.TestPoint(cam.ConvertScreenToWorld(mouse.Position.ToVector2())); // Test for fixture under cursor.
                        if (test != null)
                        {
                            foreach (var objects in ObjectsObj) if (objects.Value.Body.BodyId == test.Body.BodyId)
                            {
                                // TODO: Create separate method in Connection class.
                                Connection.Channel.SetInside(objects.Key);
                                break;
                            }
                        }
                    }
                    else Connection.Channel.UnsetInside(); // Leave car.
                }
            }

            Connection.SendPosition(position); // Important! Sending created data to server.

            cam.TrackingBody = Players[Name].InsideID.HasValue ? ObjectsObj[Players[Name].InsideID.Value].Body : PlayersObj[Name].Body; // Set camera on object.
            World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f); // One small step for iteration but big jump for project.
        }

        /// <summary>
        /// Draw all Player and other objects to screen.
        /// </summary>
        /// <param name="spriteBatch">Drawing object.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var ob in ObjectsObj.Where(i => i.Value.Enabled)) ob.Value.Draw(spriteBatch); // Draw enabled objects.
            foreach (var pl in PlayersObj.Where(i => i.Value.Enabled && Players[i.Key].Active)) pl.Value.Draw(spriteBatch, Players[pl.Key]?.VelocitySnap); // Draw enabled Players.
        }
    }
}
