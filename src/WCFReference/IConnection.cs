using System;
using System.Collections.Generic;
using System.ServiceModel;
using WCFReference.ServerObjects;

namespace WCFReference
{
    public interface IConnection
    {

        IClientCallback Callback { get; }
        DuplexChannelFactory<IWCFService> Factory { get; }
        IWCFService Channel { get; }
        Dictionary<string, ServerPlayer> Players { get; }
        Dictionary<int, ServerObject> Objects { get; }
        string Name { get; }
        bool IsConnected { get; set; }
        string Address { get; set; }
        void Close();
        bool Login(string address, string name, string pass);
        void UpdatePlayers();
        void UpdateObjects();
        void SendPosition(PositionData position);

        event EventHandler Connected;
        event EventHandler Disconnected;
    }
}
