namespace FunctionalLiving.Domain.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Domain.Knx;

    public class LightsRepository
    {
        public IEnumerable<Light> Lights { get; }

        public LightsRepository()
        {
            Lights = CustomerData
                .KnxObjects
                .Where(x =>
                    x.KnxControlType == KnxControlType.Control &&
                    x.FeedbackAddress != null &&
                    x.Description.Contains("Verlichting") &&
                    x.Description.Contains("Aan/Uit"))
                .Select(x =>
                    new Light(
                        x,
                        x.FeedbackAddress != null && FeedbackGroupAddresses.All.ContainsKey(x.FeedbackAddress)
                            ? FeedbackGroupAddresses.All[x.FeedbackAddress]
                            : null));
        }
    }
}
