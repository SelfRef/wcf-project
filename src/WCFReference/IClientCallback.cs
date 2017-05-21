using System.ServiceModel;

namespace WCFReference
{
    public interface IClientCallback
    {
        [OperationContract]
        void ServerMsg(string msg);

        [OperationContract]
        void SendMsg(string name, string msg);

        [OperationContract]
        void UpdatePlayerList(string[] players);
    }
}
