using BeMSic.Core.BmsDefinition;

namespace BeMSic.BmsFileOperator
{
    public sealed class BmsConverter
    {
        string _bms;
        public string Bms { get { return _bms; } }

        public BmsConverter(string? bms)
        {
            if(bms == null)
            {
                bms = "";

            }
            _bms = bms;
        }

        public BmsConverter DeleteUnusedWav()
        {
            _bms = BmsDefinitionReplace.GetUnusedWavDeletedBmsFile(_bms);
            return this;
        }

        public BmsConverter ArrangeWav()
        {
            _bms = BmsDefinitionReplace.GetWavArrangedBmsFile(_bms);
            return this;
        }

        public BmsConverter Offset(int offset)
        {
            _bms = BmsDefinitionReplace.GetOffsetedBmsFile(_bms, offset);
            return this;
        }

        public BmsConverter AddRange(BmsConverter bms)
        {
            _bms = BmsDefinitionReplace.GetMargedBms(_bms, bms.Bms);
            return this;
        }

        public BmsConverter Replace(List<BmsReplace> replaceList)
        {
            _bms = BmsDefinitionReplace.GetReplacedBmsFile(_bms, replaceList);
            return this;
        }
    }
}
