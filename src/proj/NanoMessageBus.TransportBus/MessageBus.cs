namespace NanoMessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Logging;
    using SubscriptionStorage;
    using Transports;

    public class MessageBus : ISendMessages, IPublishMessages
    {
        private static readonly ILog Log = LogFactory.BuildLogger(typeof(MessageBus));

        // TODO: make Func<string, ITransport> where address determines transport
        private readonly ITransportMessages transport;
        private readonly IStoreSubscriptions subscriptions;
        private readonly IDictionary<Type, ICollection<Uri>> recipients;
        private readonly IMessageContext context;
        private readonly MessageBuilder builder;
        private readonly IDiscoverMessageTypes discoverer;

        public MessageBus(
            ITransportMessages transport,
            IStoreSubscriptions subscriptions,
            IDictionary<Type, ICollection<Uri>> recipients,
            IMessageContext context,
            MessageBuilder builder,
            IDiscoverMessageTypes discoverer)
        {
            this.transport = transport;
            this.subscriptions = subscriptions;
            this.recipients = recipients;
            this.context = context;
            this.builder = builder;
            this.discoverer = discoverer;
        }

        public virtual void Send(params object[] messages)
        {
            Log.Debug(Diagnostics.SendingMessage);
            this.Send(new Dictionary<string, string>(), messages);
        }

        public virtual void Send(IDictionary<string, string> headers, params object[] messages)
        {
            Log.Debug(Diagnostics.SendingMessage);
            this.Dispatch(headers, messages, this.GetRecipients);
        }

        private ICollection<Uri> GetRecipients(object primaryMessage)
        {
            var discoveredTypes = this.discoverer.GetTypes(primaryMessage);
            return this.recipients.GetMatching(discoveredTypes);
        }


        public virtual void Reply(params object[] messages)
        {
            Log.Debug(Diagnostics.Replying, this.context.CurrentMessage.ReturnAddress);
            this.Reply(new Dictionary<string, string>(), messages);
        }

        public virtual void Reply(IDictionary<string, string> headers, params object[] messages)
        {
            Log.Debug(Diagnostics.Replying, this.context.CurrentMessage.ReturnAddress);
            this.Dispatch(headers, messages, msg => new[] { this.context.CurrentMessage.ReturnAddress });
        }

        public virtual void Publish(params object[] messages)
        {
            Log.Debug(Diagnostics.Publishing);
            this.Publish(new Dictionary<string, string>(), messages);
        }

        public virtual void Publish(IDictionary<string, string> headers, params object[] messages)
        {
            Log.Debug(Diagnostics.Publishing);
            this.Dispatch(headers, messages, this.GetSubscribers);
        }

        private ICollection<Uri> GetSubscribers(object primaryMessage)
        {
            var discoveredTypes = this.discoverer.GetTypeNames(primaryMessage);
            return this.subscriptions.GetSubscribers(discoveredTypes);
        }

        private void Dispatch(IDictionary<string, string> headers, object[] messages, Func<object, ICollection<Uri>> getRecipients)
        {
            messages = PopulatedMessagesOnly(messages);
            if (!messages.Any())
                return;

            var primaryMessage = messages[0];
            var addresses = getRecipients(primaryMessage);
            this.Dispatch(headers, messages, addresses);

            if (!addresses.Any())
                Log.Warn(Diagnostics.DroppingMessage, primaryMessage.GetType());
        }

        private static object[] PopulatedMessagesOnly(object[] messages)
        {
            messages = (messages ?? new object[] { }).Where(x => x != null).ToArray();
            return messages;
        }

        private void Dispatch(IDictionary<string, string> headers, object[] messages, IEnumerable<Uri> addresses)
        {
            if (!addresses.Any())
                return;

            var transportMessage = this.builder.BuildMessage(headers, messages);

            this.transport.Send(transportMessage, addresses.ToArray());
        }
    }
}