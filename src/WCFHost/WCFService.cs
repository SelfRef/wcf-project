using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using Microsoft.Xna.Framework;
using WCFReference;
using WCFReference.Objects;
using FarseerPhysics.Dynamics;
using WCFReference.ServerObjects;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using System.Threading;

namespace WCFServer
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class WCFService : IWCFService
    {
        private Client client; // Current client object.
        private Dictionary<string, Client> loggedUsers = new Dictionary<string, Client>(); // List of logged users.
        private Dictionary<int, ServerObject> objects = new Dictionary<int, ServerObject>(); // List of objects data.
        private Dictionary<string, ServerPlayer> players = new Dictionary<string, ServerPlayer>(); // List of Players data.

        private System.Timers.Timer saveTimer = new System.Timers.Timer(3000); // Timer to save data to file every 3 seconds.
        private System.Timers.Timer gameUpdateTimer = new System.Timers.Timer(1000/30); // Timer for phisics engine update. 

        private World world = new World(Vector2.Zero);
        private GameServiceContainer gameServiceContainer = new GameServiceContainer();
        private GraphicsDevice graphicsDevice;
        private IGraphicsDeviceService graphicsDeviceService;
        private ContentManager contentManager;

        public WCFService()
        {
            graphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.Reach, new PresentationParameters());
            graphicsDeviceService = new GraphicsDeviceService(graphicsDevice);
            gameServiceContainer.AddService(graphicsDeviceService);
            contentManager = new ContentManager(gameServiceContainer, "Content");

            #region Loading Save File
            try
            {
                players = JsonConvert.DeserializeObject<Dictionary<string, ServerPlayer>>(File.ReadAllText("PlayersData.json"));
                objects = JsonConvert.DeserializeObject<Dictionary<int, ServerObject>>(File.ReadAllText("ObjectsData.json"));
            }
            catch (FileNotFoundException)
            {
                Program.Write("At least one save file not found");
            }

            if(objects.Count == 0)
            {
                ServerObject so = new ServerObject(null, new Vector2(300), 0, true, ServerObject.BodyTypes.Car);
                objects.Add(so.ID, so);
            }

            foreach (var item in objects)
            {
                if (item.Value.BodyType == ServerObject.BodyTypes.Car) item.Value.GameObject = new Car(world, contentManager.Load<Texture2D>("Cars/enemy"), item.Value.Position, item.Value.Angle, Car.Type.Type1, Car.CarColor.Color1, Map.GetDot(graphicsDevice), null);
            }

            foreach (var item in players)
            {
                item.Value.Active = false;
                item.Value.InsideID = null;
                item.Value.InsideObject = null;
            }

            #endregion
            #region Saving Save File
            saveTimer.Elapsed += (s, e) =>
            {
                if (players.Count > 0)
                {
                    Dictionary<string, ServerPlayer> pl = new Dictionary<string, ServerPlayer>(players);
                    File.WriteAllText("PlayersData.json", JsonConvert.SerializeObject(pl));

                    Dictionary<int, ServerObject> ob = new Dictionary<int, ServerObject>(objects);
                    File.WriteAllText("ObjectsData.json", JsonConvert.SerializeObject(ob));
                }
            };
            saveTimer.Start();
            #endregion

            #region World Update
            gameUpdateTimer.Elapsed += (s, e) =>
                {
                    world.Step(1 / 30f);
                };
            // TODO: Fix phisics engine to calculate collisions correctly.
            //gameUpdateTimer.Start();  
            #endregion
        }

        /// <summary>
        /// Most important method which allows client register himself and use other methods.
        /// </summary>
        /// <param name="name">Name of client.</param>
        /// <param name="password">Password of client.</param>
        /// <returns></returns>
        public int Login(string name, string password) // 1 - wrong password, 2 - not registered (wrong name), 4 - banned // TODO: Implement the rest of codes.
        {
            if (!loggedUsers.Any(i => i.Value.Name == name))
            {
                OperationContext context = OperationContext.Current; // Operate in current context.
                string id = context.SessionId;
                client = new Client(context.Channel, context.GetCallbackChannel<IClientCallback>(), id, name); // Create client object.
                client.Channel.Closed += (sender, e) => { DisconnectEvent(sender, e, name, id); }; // Disconnect event.
                Program.Write($"\r<{DateTime.Now.ToLongTimeString()}> \"{name}\" connected!"); // Instead of using String.Format(), C# 6 provides new way to create simple strings with variables inside.
                loggedUsers.Add(id, client); // Add client to list.

                Pedestrian playerObject = new Pedestrian(world, 10, null, new Vector2(300, 300), 0); // New game object for new player...
                if (!players.ContainsKey(name))
                {
                    ServerPlayer player = new ServerPlayer(playerObject, name, playerObject.Position, playerObject.Angle, ServerPlayer.BodyTypes.Pedestrian, false, null);
                    players.Add(name, player);
                }
                else if (players.ContainsKey(name) && players[name].GameObject == null) // ...or just update.
                {
                    playerObject.Position = players[name].Position;
                    playerObject.Angle = players[name].Angle;
                    players[name].GameObject = playerObject;
                }
                Thread update = new Thread(() => { foreach (var item in loggedUsers) item.Value.Callback.UpdatePlayerList(players.Keys.ToArray()); }); // Must use new thread because burrent operation context doesnt end yet.
                update.Start();
                return 0; // ok
            }
            else return 3; // already logged in
        }

        /// <summary>
        /// What will happen when client disconnects.
        /// </summary>
        /// <param name="name">Name of client.</param>
        /// <param name="id">ID of client.</param>
        private void DisconnectEvent(object sender, EventArgs e, string name, string id)
        {
            Program.Write($"\r<{DateTime.Now.ToLongTimeString()}> \"{name}\" disconnected!");
            loggedUsers.Remove(id);
            players[name].Active = false;
        }

        /// <summary>
        /// "Call me maybe" generally for debug purposes.
        /// </summary>
        /// <param name="input">Message for server.</param>
        public void ConsoleWrite(string input)
        {
            Program.Write($"\r<{DateTime.Now.ToLongTimeString()}> {input}");
        }

        /// <summary>
        /// Send a controll object to server with controll data eg. whether player should move to front, back, twist or just stay in place.
        /// </summary>
        /// <param name="position">Serialized KeyValuePair contains Position and angle of the player.</param>
        public void SendPosition(string position)
        {
            string id = OperationContext.Current.SessionId;
            if (loggedUsers.ContainsKey(id))
            {
                PositionData pos = JsonConvert.DeserializeObject<PositionData>(position);
                players[loggedUsers[id].Name].UpdateObject(pos);
            }
        }

        /// <summary>
        /// Tell server that client just open or close game window so server knows the player is active and "in game" or not.
        /// </summary>
        /// <param name="action">TRUE if game windows is open or FALSE if not.</param>
        public void GameOpen(bool action)
        {
            string id = OperationContext.Current.SessionId;
            if (loggedUsers.ContainsKey(id))
                players[loggedUsers[id].Name].Active = action;
        }

        /// <summary>
        /// Get from server currently logged players.
        /// </summary>
        /// <returns>List of logged players.</returns>
        public string GetPlayers()
        {

            string id = OperationContext.Current.SessionId;
            if (loggedUsers.ContainsKey(id))
            {
                foreach (var item in players.Where(i => i.Value.GameObject != null)) item.Value.UpdateData();
                Dictionary<string, ServerPlayer> pl = players.Where(i => i.Value.GameObject != null).ToDictionary(i => i.Key, i => i.Value);
                return JsonConvert.SerializeObject(pl); // ok
            }
            else return null; // autorization error
        }

        /// <summary>
        /// Get other objects data.
        /// </summary>
        /// <returns>Objects data.</returns>
        public string GetObjects()
        {

            string id = OperationContext.Current.SessionId;
            if (loggedUsers.ContainsKey(id))
            {
                foreach (var item in objects) item.Value.UpdateData();
                Dictionary<int, ServerObject> ob = new Dictionary<int, ServerObject>(objects);
                return JsonConvert.SerializeObject(ob); // ok
            }
            else return null; // autorization error
        }

        /// <summary>
        /// Set inner game object for player e.g. Car.
        /// </summary>
        /// <param name="targetId"></param>
        public void SetInside(int targetId)
        {
            string id = OperationContext.Current.SessionId;
            if (loggedUsers.ContainsKey(id))
            {
                bool exist = false;
                foreach (var item in players) if (item.Value.InsideID.HasValue && item.Value.InsideID.Value == targetId) exist = true;
                if (!exist && objects[targetId] != null) players[loggedUsers[id].Name].SetInside(targetId, objects[targetId]);
            }
        }

        /// <summary>
        /// Unset inner object.
        /// </summary>
        public void UnsetInside()
        {
            string id = OperationContext.Current.SessionId;
            if (loggedUsers.ContainsKey(id))
            {
                players[loggedUsers[id].Name].UnsetInside();
            }
        }
    }
}
