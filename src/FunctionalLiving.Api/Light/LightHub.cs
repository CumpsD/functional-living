namespace FunctionalLiving.Api.Light
{
    using System.Threading.Tasks;
    using FunctionalLiving.Light;
    using ValueObjects;
    using Microsoft.AspNetCore.SignalR;

    public interface ILightClient
    {
        Task ReceiveLightTurnedUnknownMessage(string lightId);
        Task ReceiveLightTurnedOnMessage(string lightId);
        Task ReceiveLightTurnedOffMessage(string lightId);
    }

    public class LightHubSender : ILightHub
    {
        private readonly IHubContext<LightHub, ILightClient> _hubContext;

        public LightHubSender(IHubContext<LightHub, ILightClient> hubContext)
            => _hubContext = hubContext;

        public async Task SendLightTurnedUnknownMessage(LightId lightId)
            => await _hubContext.Clients.All.ReceiveLightTurnedUnknownMessage(lightId.ToString());

        public async Task SendLightTurnedOnMessage(LightId lightId)
            => await _hubContext.Clients.All.ReceiveLightTurnedOnMessage(lightId.ToString());

        public async Task SendLightTurnedOffMessage(LightId lightId)
            => await _hubContext.Clients.All.ReceiveLightTurnedOffMessage(lightId.ToString());
    }

    public class LightHub : Hub<ILightClient>
    {
    }
}
