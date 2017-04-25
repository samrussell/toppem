using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using toppem;
using Xunit;

namespace toppemtests
{
    public class PrefixParserTests
    {
        [Fact]
        void PrefixParserDecodesVarious()
        {
            PrefixParserEncodes(new byte[] { 24, 10, 1, 2 }, new Prefix(new byte[] { 10, 1, 2 }, 24));
            PrefixParserEncodes(new byte[] { 25, 10, 1, 2, 128 }, new Prefix(new byte[] { 10, 1, 2, 128 }, 25));
            PrefixParserEncodes(new byte[] { 23, 10, 1, 128 }, new Prefix(new byte[] { 10, 1, 128 }, 23));
            PrefixParserEncodes(new byte[] { 16, 10, 1 }, new Prefix(new byte[] { 10, 1 }, 16));
            PrefixParserEncodes(new byte[] { 4, 64 }, new Prefix(new byte[] { 64 }, 4));
        }

        void PrefixParserDecodes(byte[] serializedPrefix, Prefix expectedPrefix)
        {
            var parser = new PrefixParser();
            using (var memStream = new MemoryStream(serializedPrefix))
            {
                var prefix = parser.Decode(memStream);
                Assert.Equal(expectedPrefix, prefix);
            }
        }

        [Fact]
        void PrefixParserEncodesVarious()
        {
            PrefixParserEncodes(new byte[] { 24, 10, 1, 2 }, new Prefix(new byte[] { 10, 1, 2 }, 24));
            PrefixParserEncodes(new byte[] { 25, 10, 1, 2, 128 }, new Prefix(new byte[] { 10, 1, 2, 128 }, 25));
            PrefixParserEncodes(new byte[] { 23, 10, 1, 128 }, new Prefix(new byte[] { 10, 1, 128 }, 23));
            PrefixParserEncodes(new byte[] { 16, 10, 1 }, new Prefix(new byte[] { 10, 1 }, 16));
            PrefixParserEncodes(new byte[] { 4, 64 }, new Prefix(new byte[] { 64 }, 4));
        }

        void PrefixParserEncodes(byte[] expectedOutput, Prefix prefix)
        {
            var parser = new PrefixParser();
            using (var memStream = new MemoryStream())
            {
                parser.Encode(memStream, prefix);
                var output = memStream.ToArray();
                Assert.Equal(expectedOutput, output);
            }
        }
    }
}
