namespace FunctionalLiving.Knx.Sender.Infrastructure
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;

    public class CommandMetaData
    {
        public static class Keys
        {
            public const string Ip = "Ip";
            public const string UserClaims = "User";
            public const string CorrelationId = "CorrelationId";
        }

        private readonly ClaimsPrincipal _claimsPrincipal;

        public string Ip { get; private set; }
        public string CorrelationId { get; private set; }
        public IEnumerable<KeyValuePair<string, string>> UserClaims { get; private set; }

        private CommandMetaData() { }

        public CommandMetaData(
            ClaimsPrincipal claimsPrincipal,
            string ipClaimName,
            string correlationClaimName)
        {
            _claimsPrincipal = claimsPrincipal;

            Ip = claimsPrincipal.FindFirst(ipClaimName)?.Value;
            CorrelationId = claimsPrincipal.FindFirst(correlationClaimName)?.Value;
            UserClaims = claimsPrincipal.Claims.Select(claim => new KeyValuePair<string, string>(claim.Type, claim.Value));
        }

        public bool HasRole(string roleName) =>
            UserClaims != null &&
            UserClaims.Any(pair =>
                pair.Key == ClaimTypes.Role &&
                pair.Value == roleName);

        public static CommandMetaData FromDictionary(IDictionary<string, object> source) =>
            new CommandMetaData
            {
                Ip = StringOrEmpty(source, Keys.Ip),
                UserClaims = UserClaimsOrNull(source, Keys.UserClaims),
                CorrelationId = StringOrEmpty(source, Keys.CorrelationId)
            };

        private static IEnumerable<KeyValuePair<string, string>> UserClaimsOrNull(IDictionary<string, object> source, string key) =>
            source.ContainsKey(key)
                ? (IEnumerable<KeyValuePair<string, string>>)source[key]
                : null;

        private static string StringOrEmpty(IDictionary<string, object> source, string key) =>
            source.ContainsKey(key)
                ? (string)source[key]
                : string.Empty;

        public IDictionary<string, object> ToDictionary()
        {
            if (_claimsPrincipal == null)
                return new Dictionary<string, object>();

            return new Dictionary<string, object>
            {
                { Keys.Ip, Ip },
                { Keys.UserClaims,  UserClaims},
                { Keys.CorrelationId, CorrelationId }
            };
        }
    }
}
