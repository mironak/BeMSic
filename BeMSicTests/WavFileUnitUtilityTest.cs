using BeMSic.Core.BmsDefinition;

namespace BeMSicTests
{
    public class WavFileUnitUtilityTest
    {
        [Fact]
        public void PartialWavsTest()
        {
            // files: 1, 6, 11, 16, ... 1291
            var files = new List<WavFileUnit>();
            for(int i = 1; i < 1296; i += 5)
            {
                files.Add(new WavFileUnit(i, i.ToString()));
            }

            // partials: 101, 106, 111, ... 196
            var partials = WavFileUnitUtility.GetPartialWavs(files, 100, 200);

            for(int i = 0; i < partials.Count; i++)
            {
                Assert.Equal(i * 5 + 101, partials[i].Wav.Num);
            }

            Assert.Equal(20, partials.Count);
        }
    }
}