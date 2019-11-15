using System.ServiceModel;

namespace WCFReference
{
  [ServiceContract(CallbackContract = typeof(IClientCallback))]
  public interface IWCFService
  {
    [OperationContract]
    int Login(string name, string pass);

    [OperationContract]
    void ConsoleWrite(string input);

    [OperationContract]
    void SendPosition(string position);

    [OperationContract]
    void CreateBullet(string position);

    [OperationContract]
    void GameOpen(bool action);

    [OperationContract]
    string GetPlayers();

    [OperationContract]
    string GetObjects();

    [OperationContract]
    void SetInside(int targetId);

    [OperationContract]
    void UnsetInside();
  }
}
