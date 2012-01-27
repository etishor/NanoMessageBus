namespace NanoMessageBus.Endpoints
{
    using System;
    using System.Net;
    using System.Text.RegularExpressions;

    public class MsmqAddress
    {
        private const string LocalHost = ".";
        private const string DirectOS = "OS";
        private const string DirectTCP = "TCP";
        private const string MsmqFormat = @"FormatName:Direct={0}:{1}\PRIVATE$\{2}";
        private const string CanonicalFormat = @"msmq://{0}/{1}";
        private const string Pattern = @"^((msmq\://)?([A-Za-z0-9-_.]+)/)?([A-Za-z0-9-_.]+)(/)?$";
        private const int HostNameCapture = 3;
        private const int QueueNameCapture = 4;
        private static readonly Regex AddressFormatRegex = new Regex(Pattern, RegexOptions.Compiled);

        private readonly Uri canonical;
        private readonly string proprietary;

        public MsmqAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentException(Diagnostics.InvalidAddress.FormatWith(address), "address");

            var match = AddressFormatRegex.Match(address);
            if (!match.Success)
                throw new ArgumentException(Diagnostics.InvalidAddress.FormatWith(address), "address");

            var host = match.Groups[HostNameCapture].Value;

            IPAddress ip;
            string protocol = DirectOS;
            // if an IP is specified use Direct=TCP since NetBios name resolution might not be available
            if (IPAddress.TryParse(host, out ip))
            {
                protocol = DirectTCP;
            }

            var machineName = GetMachineName(host);
            var queueName = match.Groups[QueueNameCapture].Value;

            this.proprietary = MsmqFormat.FormatWith(protocol, machineName, queueName);
            this.canonical = new Uri(CanonicalFormat.FormatWith(machineName, queueName));
        }

        private static string GetMachineName(string value)
        {
            value = (value ?? string.Empty).Trim();
            value = value.Length == 0 || value == LocalHost ? Environment.MachineName : value;
            return value.ToLowerInvariant();
        }

        public virtual Uri Canonical
        {
            get { return this.canonical; }
        }
        public virtual string Proprietary
        {
            get { return this.proprietary; }
        }

        public override string ToString()
        {
            return this.proprietary;
        }
    }
}