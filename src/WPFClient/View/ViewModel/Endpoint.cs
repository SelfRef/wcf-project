namespace WPFClient.View.ViewModel
{
    public struct Endpoint
    {
        public string Protocol { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public string Service { get; set; }
        public Endpoint(string prot, string addr, int port, string serv)
        {
            Protocol = prot;
            Address = addr;
            Port = port;
            Service = serv;
        }
    }
}
