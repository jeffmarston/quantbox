using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Eze.SchemaCompare
{
    public class MessageHub : Hub
    {
        public DataAccessor DataAccessor { get; private set; }

        public MessageHub(DataAccessor da)
        {
            DataAccessor = da;
        }

        public override async Task OnConnectedAsync()
        {
            var connId = Context.ConnectionId;
            Console.WriteLine("Connected = " + connId);

            DataAccessor.Publisher = Clients.All;
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connId = Context.ConnectionId;
            Console.WriteLine("Disconnected = " + connId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
