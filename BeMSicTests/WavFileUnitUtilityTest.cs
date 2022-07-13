using BeMSic.Core.BmsDefinition;

namespace BeMSicTests
{
    public class WavFileUnitUtilityTest
    {
        [Fact]
        public void PartialWavsTest()
        {
            // files: 1, 6, 11, 16, ... 1291
            var files = new WavFileUnitUtility();
            for(int i = 1; i < 1296; i += 5)
            {
                files.Add(new WavFileUnit(i, i.ToString()));
            }

            // partials: 101, 106, 111, ... 196
            var partials = files.GetPartialWavs(new WavDefinition(100), new WavDefinition(200));

            int unitNum = 0;
            foreach (var wav in partials)
            {
                Assert.Equal(unitNum * 5 + 101, wav.Wav.Num);
                unitNum++;
            }

            Assert.Equal(20, unitNum);
        }
    }
}