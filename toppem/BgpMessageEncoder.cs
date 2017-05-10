using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public class BgpMessageEncoder : StreamParser, IBgpMessageVisitor
    {
        public Tlv tlv = null;

        public BgpMessageEncoder()
        {
            _networkByteOrder = true;
        }

        public void Visit(BgpOpenMessage openMessage)
        {
            //var data = EncodeNumber(openMessage.version, 1).
            //    Concat(EncodeNumber(openMessage.asNum, 2)).
            //    Concat(EncodeNumber(openMessage.holdTime, 2)).
            //    Concat(EncodeNumber(openMessage.identifier, 4)).
            //    Concat(EncodeCapabilities(openMessage.capabilities));
            var data = new FudgePacker().Pack(openMessage).Concat(EncodeCapabilities(openMessage.capabilities));
            tlv = new Tlv(1, data.ToArray());
        }

        IEnumerable<byte> EncodeCapabilities(IEnumerable<Tlv> capabilities)
        {
            var outStream = new MemoryStream();
            var parser = new TlvParser(1, 1);
            capabilities.ToList().ForEach(tlv => parser.Encode(outStream, tlv));
            var encodedCapabilities = outStream.GetBuffer().Take(Convert.ToInt32(outStream.Position)).ToArray();

            return EncodeNumber(Convert.ToUInt32(encodedCapabilities.Length), 1).Concat(encodedCapabilities);
        }

        public void Visit(BgpUpdateMessage updateMessage)
        {
            var data = EncodePrefixes(updateMessage.prefixes);
            tlv = new Tlv(2, data.ToArray());
        }

        IEnumerable<byte> EncodePrefixes(IEnumerable<Prefix> prefixes)
        {
            var outStream = new MemoryStream();
            var parser = new PrefixParser();
            prefixes.ToList().ForEach(prefix => parser.Encode(outStream, prefix));
            var encodedPrefixes = outStream.GetBuffer().Take(Convert.ToInt32(outStream.Position)).ToArray();

            return EncodeNumber(Convert.ToUInt32(encodedPrefixes.Length), 2).Concat(encodedPrefixes);
        }

        public void Visit(BgpKeepaliveMessage keepaliveMessage)
        {
            tlv = new Tlv(3, new byte[] { });
        }

        public void Visit(BgpNotificationMessage notificationMessage)
        {
            tlv = new Tlv(4, notificationMessage.data);
        }
    }
}
