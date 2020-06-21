namespace FunctionalLiving.Api.Knx
{
    using System.Threading.Tasks;
    using FunctionalLiving.Knx;
    using Microsoft.AspNetCore.SignalR;

    public interface IKnxClient
    {
        Task ReceiveKnxMessage(string message);
    }

    public class KnxHubSender : IKnxHub
    {
        private readonly IHubContext<KnxHub, IKnxClient> _hubContext;

        public KnxHubSender(IHubContext<KnxHub, IKnxClient> hubContext)
            => _hubContext = hubContext;

        public async Task SendKnxMessage(string message)
            => await _hubContext.Clients.All.ReceiveKnxMessage(message);
    }

    public class KnxHub : Hub<IKnxClient>
    {
    }
}
