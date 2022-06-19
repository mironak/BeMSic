using BeMSic.Core.BmsDefinition;

namespace BeMSic.BmsFileOperator
{
    public sealed class BmsConverter
    {
        string _bms;
        public string Bms { get { return _bms; } }

        int _finalWavIndex;
        public int FinalWavIndex { get { return _finalWavIndex; } }

        public BmsConverter(string? bms)
        {
            if(bms == null)
            {
                bms = "";

            }
            _bms = bms;
            UpdateFinalWavIndex();
        }

        public BmsConverter DeleteUnusedWav()
        {
            _bms = BmsDefinitionReplace.GetUnusedWavDeletedBmsFile(_bms);
            UpdateFinalWavIndex();
            return this;
        }

        public BmsConverter ArrangeWav()
        {
            _bms = BmsDefinitionReplace.GetWavArrangedBmsFile(_bms);
            UpdateFinalWavIndex();
            return this;
        }

        public BmsConverter Offset(int offset)
        {
            _bms = BmsDefinitionReplace.GetOffsetedBmsFile(_bms, offset);
            UpdateFinalWavIndex();
            return this;
        }

        public BmsConverter AddRange(BmsConverter bms)
        {
            // 定義数ZZ確認
            if (_finalWavIndex + bms.FinalWavIndex > 1295)
            {
                throw new ArgumentOutOfRangeException();
            }

            _bms = BmsDefinitionReplace.GetMargedBms(_bms, bms.Bms, _finalWavIndex);
            return this;
        }

        public BmsConverter Replace(List<BmsReplace> replaceList)
        {
            _bms = BmsDefinitionReplace.GetReplacedBmsFile(_bms, replaceList);
            return this;
        }

        private void UpdateFinalWavIndex()
        {
            _finalWavIndex = BmsDefinitionReplace.GetUsedWavList(_bms).Max();
        }
    }
}
