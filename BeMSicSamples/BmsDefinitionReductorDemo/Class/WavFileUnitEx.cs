using BeMSic.Core.BmsDefinition;
using BeMSic.Core.Helpers;

namespace BmsDefinitionReductorDemo.Class
{
    public class WavFileUnitEx : WavFileUnit
    {
        /// <summary>
        /// For display
        /// </summary>
        public string NumText { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="wfu"></param>
        public WavFileUnitEx(WavFileUnit wfu) : base(wfu.Wav.Num, wfu.Name)
        {
            NumText = RadixConvert.IntToZZ(wfu.Wav.Num);
        }
    }
}
