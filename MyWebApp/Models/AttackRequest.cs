using System.Text.Json.Serialization;

namespace MyWebApp.Models
{
    public class AttackRequest
    {
        public class AttackNameValuePair
        {
            public string AttackName { get; set; }

            public object[] Args { get; set; }

            // Parameterless constructor for deserialization
            public AttackNameValuePair() { }

            // Optional convenience constructor for single-value calls
            public AttackNameValuePair(string attackName, params object[] args)
            {
                AttackName = attackName;
                Args = args;
            }
        }

        public List<AttackNameValuePair> AttackNameValuePairs { get; set; }

        // Parameterless constructor for deserialization
        public AttackRequest() { }

        [JsonConstructor]
        public AttackRequest(List<AttackNameValuePair> attackNameValuePairs)
        {
            AttackNameValuePairs = attackNameValuePairs;
        }
    }
}
