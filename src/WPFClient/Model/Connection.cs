using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;
using Newtonsoft.Json;
using WCFReference;
using WCFReference.ServerObjects;
using WPFClient.View;

namespace WPFClient.Model
{
    /// <summary>
    /// Class which one help to manage connection with server.
    /// </summary>
    public class Connection : IConnection
    {
        public IClientCallback Callback { get; private set; } = new ClientCallback(); // Callback object.
        public DuplexChannelFactory<IWCFService> Factory { get; private set; }
        public IWCFService Channel { get; private set; }
        public string Name { get; private set; } // Name of current player.
        public Dictionary<string, ServerPlayer> Players { get; private set; } = new Dictionary<string, ServerPlayer>();
        public Dictionary<int, ServerObject> Objects { get; set; } = new Dictionary<int, ServerObject>();
        public bool IsConnected { get; set; } = false;
        public string Address
        {
            get { return Factory.Endpoint.Address.ToString(); }
            set { Factory.Endpoint.Address = new EndpointAddress(value); }
        }

        public Connection()
        {
            Factory = new DuplexChannelFactory<IWCFService>(Callback, new NetTcpBinding()); // Create communication channel.
        }

        public void Close()
        {
            try
            {
                ((ICommunicationObject)Channel).Close(); // Close channel.
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Debug");
            }
        }
        
        public bool Login(string address, string name, string pass)
        {
            Address = address;

            try
            {
                Channel = Factory.CreateChannel();
                ((ICommunicationObject)Channel).Open();

                switch (Channel.Login(name, pass))
                {
                    case 0:
                        ((ICommunicationObject)Channel).Faulted += (s, e) =>
                        {
                            Disconnected(this, EventArgs.Empty);
                            if (Msg.Error(Msg.ErrorMsgs.DisconnectionError, MessageBoxButton.YesNo) == MessageBoxResult.Yes) Login(address, name, pass);
                        };
                        ((ICommunicationObject)Channel).Closed += (s, e) =>
                        {
                            Disconnected(this, EventArgs.Empty);
                        };
                        Name = name;
                        Connected(this, EventArgs.Empty);
                        return true;
                    case 1:
                        Msg.Error(Msg.ErrorMsgs.WrongPass, MessageBoxButton.OK);
                        break;
                    case 2:
                        Msg.Error(Msg.ErrorMsgs.NotRegistered, MessageBoxButton.OK);
                        break;
                    case 3:
                        Msg.Warning(Msg.WarningMsgs.AlreadyLoggedIn, MessageBoxButton.OK);
                        break;
                    case 4:
                        Msg.Error(Msg.ErrorMsgs.Banned, MessageBoxButton.OK);
                        break;
                }
                Close();
                return false;
            }
            catch (Exception)
            {
                if (Msg.Warning(Msg.WarningMsgs.ConnectionWarning, MessageBoxButton.YesNo) == MessageBoxResult.Yes) return Login(address, name, pass);
            }
            return true;
        }

        public void UpdatePlayers()
        {
            try
            {
                string data = Channel.GetPlayers();
                if (data != "[]") Players = JsonConvert.DeserializeObject<Dictionary<string, ServerPlayer>>(data);
                else Players = new Dictionary<string, ServerPlayer>();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Close();
            }
        }

        public void UpdateObjects()
        {
            try
            {
                string data = Channel.GetObjects();
                if (data != "[]") Objects = JsonConvert.DeserializeObject<Dictionary<int, ServerObject>>(data);
                else Objects = new Dictionary<int, ServerObject>();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Close();
            }
        }

        public void SendPosition(PositionData position)
        {
            try
            {
                Channel.SendPosition(JsonConvert.SerializeObject(position));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Close();
            }
        }

        public event EventHandler Connected = (s, e) => { (s as Connection).IsConnected = true; };
        public event EventHandler Disconnected = (s, e) => { (s as Connection).IsConnected = false; };
    }
}
