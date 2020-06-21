namespace FunctionalLiving.Knx
{
    using System.Threading.Tasks;

    public interface IKnxHub
    {
        Task SendKnxMessage(string message);
    }
}
