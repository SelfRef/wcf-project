using System.ServiceModel;
using WCFReference;

namespace WCFServer
{
    /// <summary>
    /// Logged client with session, callback, name and channel data.
    /// </summary>
    class Client
    {
        /// <summary>
        /// Get callback object and use to call client methods.
        /// </summary>
        public IClientCallback Callback { get; private set; }
        /// <summary>
        /// Get channel object of current session.
        /// </summary>
        public ICommunicationObject Channel { get; private set; }
        /// <summary>
        /// Get current session ID.
        /// </summary>
        public string SessionID { get; private set; }
        public string Name { get; private set; }
        public CommunicationState State
        {
            get
            {
                return Channel.State;
            }
        }
        /// <summary>
        /// Create instance of Client class which represents one of connected clients.
        /// </summary>
        /// <param name="channel">Represents current channel which is used to connect client with server.</param>
        /// <param name="callback">Callback object contains client methods.</param>
        /// <param name="id">Session ID of current channel.</param>
        /// <param name="name">Client name/nick.</param>
        public Client(ICommunicationObject channel, IClientCallback callback, string id, string name)
        {
            Channel = channel;
            Callback = callback;
            SessionID = id;
            Name = name;
        }
        /// <summary>
        /// Disconnect this client from server.
        /// </summary>
        /// <returns></returns>
        public int Disconnect() // 0 - OK, 1 - Error
        {
            if (Channel.State != CommunicationState.Closed)
            {
                Channel.Close();
                return 0;
            }
            else return 1;
        }
    }
}
