using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public class BgpMessageParser : StreamParser
    {
        public BgpMessageParser()
        {
            _networkByteOrder = true;
        }

        public IBgpMessage Decode(Tlv tlv)
        {
            switch (tlv.Type)
            {
                case 1:
                    return DecodeBgpOpenMessage(tlv);
                case 2:
                    return DecodeBgpUpdateMessage(tlv);
                case 3:
                    return new BgpKeepaliveMessage();
                case 4:
                    return DecodeBgpNotificationMessage(tlv);
            }
            throw new Exception("Could not decode message with type " + tlv.Type);
        }

        public Tlv Encode(IBgpMessage bgpMessage)
        {
            var encoder = new BgpMessageEncoder();
            bgpMessage.Accept(encoder);
            return encoder.tlv;
        }

        public void EncodeSpecific(BgpKeepaliveMessage bgpMessage)
        {
            new Tlv(3, new byte[] { });
        }

        BgpOpenMessage DecodeBgpOpenMessage(Tlv tlv)
        {
            using(var dataStream = new MemoryStream(tlv.Data))
            {
                var version = ReadNumber(dataStream, 1);
                var asNum = ReadNumber(dataStream, 2);
                var holdTime = ReadNumber(dataStream, 2);
                var identifier = ReadNumber(dataStream, 4);
                var capabilities = ParseCapabilities(dataStream);

                return new BgpOpenMessage(version, asNum, holdTime, identifier, capabilities);
            }
        }

        BgpUpdateMessage DecodeBgpUpdateMessage(Tlv tlv)
        {
            using (var dataStream = new MemoryStream(tlv.Data))
            {
                var withdrawnRoutesLength = ReadNumber(dataStream, 2);
                var withdrawnRoutes = ParsePrefixes(dataStream, withdrawnRoutesLength);

                return new BgpUpdateMessage(withdrawnRoutes);
            }
        }

        IEnumerable<Tlv> ParseCapabilities(Stream dataStream)
        {
            var capabilitiesLength = ReadNumber(dataStream, 1);
            var capabilities = new List<Tlv>();
            var parser = new TlvParser(1, 1);
            while(dataStream.Position < dataStream.Length)
            {
                capabilities.Add(parser.Decode(dataStream));
            }

            return capabilities;
        }

        IEnumerable<Prefix> ParsePrefixes(Stream dataStream, int withdrawnRoutesLength)
        {
            var prefixes = new List<Prefix>();
            var parser = new PrefixParser();
            var stopLength = dataStream.Position + withdrawnRoutesLength;
            while (dataStream.Position < stopLength)
            {
                var prefix = parser.Decode(dataStream);

                prefixes.Add(prefix);
            }

            return prefixes;
        }

        BgpNotificationMessage DecodeBgpNotificationMessage(Tlv tlv)
        {
            return new BgpNotificationMessage(tlv.Data);
        }
    }
}
