using System;
using System.Collections.Generic;
using WCFReference;
using WPFClient.View.ViewModel;

namespace WPFClient.Model
{
    public class ClientCallback : IClientCallback
    {
        public void SendMsg(string name, string msg)
        {
            ((MainWindow)App.Current.MainWindow).output.AppendText($"<{DateTime.Now.ToLongTimeString()}> {name} {msg}\n");
        }

        public void ServerMsg(string msg)
        {
            ((MainWindow)App.Current.MainWindow).output.AppendText($"<{DateTime.Now.ToLongTimeString()}> [Server] {msg}\n");
        }

        public void UpdatePlayerList(string[] players)
        {
            List<Player> list = new List<Player>();
            foreach (var item in players) list.Add(new Player() { Nazwa = item });
            ((MainWindow)App.Current.MainWindow).playersList.ItemsSource = list;
        }
    }
}
