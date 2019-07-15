using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Eze.Quantbox
{
    public class MasterHub : Hub
    {
        public AlgoMaster AlgoMaster { get; private set; }

        public MasterHub(AlgoMaster algoMaster)
        {
            AlgoMaster = algoMaster;
        }

        public override async Task OnConnectedAsync()
        {
            var connId = Context.ConnectionId;
            Console.WriteLine("Connected = " + connId);

            AlgoMaster.Publisher = Clients.All;
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connId = Context.ConnectionId;
            Console.WriteLine("Disconnected = " + connId);
            await base.OnDisconnectedAsync(exception);
        }

        //public async Task Subscribe(string command, string serviceName)
        //{
        //    // Really no need to use this now, since client sends data via REST API
        //}
    }
}
