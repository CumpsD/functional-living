namespace FunctionalLiving.Light
{
    using System.Threading.Tasks;
    using ValueObjects;

    public interface ILightHub
    {
        Task SendLightTurnedOnMessage(LightId lightId);
        Task SendLightTurnedOffMessage(LightId lightId);
    }
}
