using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using WCFReference;

namespace WCFServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(new WCFService(), new Uri("net.tcp://localhost:9999/Service"));
            host.AddServiceEndpoint(typeof(IWCFService), new NetTcpBinding(), "");

            host.Description.Behaviors.Add(new ServiceMetadataBehavior());
            host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), "mex");

            host.Description.Behaviors.Add(new ServiceDiscoveryBehavior());
            host.AddServiceEndpoint(new UdpDiscoveryEndpoint());
            try
            {
                host.Open();
                Console.WriteLine("Address: {0}", host.Description.Endpoints[0].Address);
                Console.WriteLine("Service started!\tPress q + ENTER to stop.");
                while (true)
                {
                    Console.Write(">");
                    switch (Console.ReadLine())
                    {
                        case "q":
                            Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine("Unknown command, try again.");
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.Write("\n\nPress ENTER to exit... ");
                Console.Read();
            }
        }
        public static void Write(string msg)
        {
            Console.Write("\r" + msg + "\n>");
        }
    }
}
