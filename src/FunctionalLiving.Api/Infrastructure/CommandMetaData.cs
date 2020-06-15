namespace FunctionalLiving.Api.Infrastructure
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;

    public class CommandMetaData
    {
        public static class Keys
        {
            public const string FirstName = "FirstName";
            public const string LastName = "LastName";
            public const string Ip = "Ip";
            public const string UserId = "UserId";
            public const string UserClaims = "User";
            public const string CorrelationId = "CorrelationId";
        }

        private readonly ClaimsPrincipal _claimsPrincipal;
        private const string AcmIdClaimName = "urn:cumps-consulting:functional-living:acmid";

        public string Ip { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string UserId { get; private set; }
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
            FirstName = claimsPrincipal.FindFirst(ClaimTypes.GivenName)?.Value;
            LastName = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
            UserId = claimsPrincipal.FindFirst(AcmIdClaimName)?.Value;
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
                FirstName = StringOrEmpty(source, Keys.FirstName),
                LastName = StringOrEmpty(source, Keys.LastName),
                Ip = StringOrEmpty(source, Keys.Ip),
                UserId = StringOrEmpty(source, Keys.UserId),
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
                { Keys.FirstName, FirstName },
                { Keys.LastName, LastName },
                { Keys.Ip, Ip },
                { Keys.UserId, UserId },
                { Keys.UserClaims,  UserClaims},
                { Keys.CorrelationId, CorrelationId }
            };
        }
    }
}
