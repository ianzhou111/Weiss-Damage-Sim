using System.Collections.Generic;

namespace MyWebApp.Models
{
    public class FullDamageRequest
    {
        public List<AttackRequest.AttackNameValuePair> AttackNameValuePairs { get; set; }
        public DeckInfo SelfDeckInfo { get; set; }
        public DeckInfo OppDeckInfo { get; set; }
        public DeckInfo Opp2ndDeckInfo { get; set; }

        // Optional constructor for manual creation
        public FullDamageRequest() { }

        public FullDamageRequest(
            List<AttackRequest.AttackNameValuePair> attackNameValuePairs,
            DeckInfo selfDeckInfo,
            DeckInfo oppDeckInfo,
            DeckInfo opp2ndDeckInfo)
        {
            AttackNameValuePairs = attackNameValuePairs;
            SelfDeckInfo = selfDeckInfo;
            OppDeckInfo = oppDeckInfo;
            Opp2ndDeckInfo = opp2ndDeckInfo;
        }
    }
}
