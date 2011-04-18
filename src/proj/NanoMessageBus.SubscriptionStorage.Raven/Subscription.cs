using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NanoMessageBus.SubscriptionStorage.Raven
{
    public class Subscription
    {
        public Subscription(string subscriber, string messageType, DateTime? expiration)
        {
            if (subscriber == null)
            {
                throw new ArgumentNullException("subscriber");
            }

            if (messageType == null)
            {
                throw new ArgumentNullException("messageType");
            }

            this.Id = FormatId(subscriber, messageType);
            this.Subscriber = subscriber.ToString();
            this.MessageType = messageType;
            this.Expiration = expiration;
        }

        public static string FormatId(string subscriber, string messageType)
        {
            return string.Format("Subscriptions/{0}/{1}", subscriber.Replace('/','-'), messageType);
        }

        public string Id { get; private set; } 
        public string Subscriber { get; private set; }
        public string MessageType { get; set; }
        public DateTime? Expiration { get; set; }
    }
}
