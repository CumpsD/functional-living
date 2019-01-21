namespace KNXLibTests.Unit.Addressing
{
    using FunctionalLiving.Knx.Addressing;
    using FunctionalLiving.Knx.Exceptions;
    using Xunit;
    using Xunit.Categories;

    public class KnxIndividualAddressTest
    {
        [Category("KNXLib.Unit.Address.Individual"), Theory]
        [InlineData(0, 0, 0)]     // Not allowed
        [InlineData(16, 1, 1)]    // Area too high
        [InlineData(10, 16, 1)]   // Line too high
        [InlineData(10, 10, 300)] // Participant too high
        public void InvalidTest(int area, int line, int participant)
        {
            var pa = new KnxIndividualAddress(area, line, participant);
            Assert.False(pa.IsValid());

            // Test if exception is thrown when using an invalid GA
            Assert.Throws<InvalidKnxAddressException>(() => pa.GetAddress());
        }

        [Category("KNXLib.Unit.Address.Individual"), Theory]
        [InlineData(0, 0, 1)]
        [InlineData(15, 15, 255)]
        [InlineData(10, 10, 10)]
        public void ValidTest(int area, int line, int participant)
        {
            var pa = new KnxIndividualAddress(area, line, participant);

            Assert.True(pa.IsValid());
            Assert.Equal(area, pa.Area);
            Assert.Equal(line, pa.Line);
            Assert.Equal(participant, pa.Participant);
            Assert.True(pa.Equals(area, line, participant));
            Assert.True(pa.Equals($"{area}.{line}.{participant}"));
        }

        [Category("KNXLib.Unit.Address.Individual"), Theory]
        [InlineData("0.0.1", 0, 0, 1)]
        [InlineData("15.15.255", 15, 15, 255)]
        [InlineData("10.10.10", 10, 10, 10)]
        public void ValidParserTest(string address, int area, int line, int participant)
        {
            var pa = new KnxIndividualAddress(address);

            Assert.True(pa.IsValid());
            Assert.Equal(area, pa.Area);
            Assert.Equal(line, pa.Line);
            Assert.Equal(participant, pa.Participant);
            Assert.True(pa.Equals(area, line, participant));
            Assert.True(pa.Equals($"{area}.{line}.{participant}"));
        }

        [Category("KNXLib.Unit.Address.Individual"), Theory]
        [InlineData("16.16.16")]
        [InlineData("0/0")]
        [InlineData("0")]
        [InlineData("15,15,15")]
        [InlineData("5,6")]
        [InlineData("")]
        public void InvalidParserTest(string address)
        {
            var pa = new KnxIndividualAddress(address);
            Assert.False(pa.IsValid());

            // Test if exception is thrown when using an invalid GA
            Assert.Throws<InvalidKnxAddressException>(() => pa.GetAddress());
        }

        [Category("KNXLib.Unit.Address.Individual"), Theory]
        [InlineData(1, 1, 80, new byte[] { 0x11, 0x50 })]
        [InlineData(1, 1, 127, new byte[] { 0x11, 0x7f })]
        [InlineData(15, 15, 15, new byte[] { 0xFF, 0x0F })]
        public void ConversionTest(int area, int line, int participant, byte[] expected)
        {
            var pa = new KnxIndividualAddress(area, line, participant);
            var address = pa.GetAddress();
            var paNew = new KnxIndividualAddress(address);

            Assert.Equal(expected, address);
            Assert.Equal(pa.Area, paNew.Area);
            Assert.Equal(pa.Line, paNew.Line);
            Assert.Equal(pa.Participant, paNew.Participant);
        }
    }
}
