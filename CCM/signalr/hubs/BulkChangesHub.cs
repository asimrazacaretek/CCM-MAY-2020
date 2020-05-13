using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace CCM.signalr.hubs
{
    public class BulkChangingesHub : Hub
    {

        public static void BulkChangesProgress(string toUserId, double Percentage)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<BulkChangingesHub>();
            hubContext.Clients.User(toUserId).BulkChangesProgress(toUserId, Percentage);

            // Call js function

        }

        public override Task OnConnected()
        {

            string clientId = Context.ConnectionId;
            string data = clientId;

            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string count = "NA";

            string clientId = Context.ConnectionId;
            string[] Exceptional = new string[1];
            Exceptional[0] = clientId;
            Clients.AllExcept(Exceptional).receiveMessage(clientId, "NewConnection", clientId + " leave", count, "");

            return base.OnDisconnected(stopCalled);
        }
    }
}