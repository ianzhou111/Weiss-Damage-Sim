using System.Text.Json.Serialization;

namespace MyWebApp.Models
{
    public class AttackRequest
    {
        public class AttackNameValuePair
        {
            public string AttackName { get; set; }
            public int Value { get; set; }

            // Default constructor for deserialization
            public AttackNameValuePair() { }

            // Parameterized constructor for your custom use
            public AttackNameValuePair(string attackName, int value)
            {
                AttackName = attackName;
                Value = value;
            }
        }

        public List<AttackNameValuePair> AttackNameValuePairs { get; set; }

        // Parameterless constructor for deserialization
        public AttackRequest() { }

        // Use JsonConstructor to bind the correct constructor for deserialization
        [JsonConstructor]
        public AttackRequest(List<AttackNameValuePair> attackNameValuePairs)
        {
            AttackNameValuePairs = attackNameValuePairs;
        }
    }
}
