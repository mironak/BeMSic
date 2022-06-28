using BeMSic.Core.BmsDefinition;

namespace BeMSic.BmsFileOperator
{
    /// <summary>
    /// BMSファイルを変換する
    /// </summary>
    public sealed class BmsConverter
    {
        private string _bms;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="bms">BMSテキスト</param>
        public BmsConverter(string? bms)
        {
            if (bms == null)
            {
                bms = string.Empty;
            }

            _bms = bms;
        }

        /// <summary>
        /// BMSファイルテキスト
        /// </summary>
        public string Bms
        {
            get { return _bms; }
        }

        /// <summary>
        /// 未使用#WAVの削除
        /// </summary>
        /// <returns>BmsConverter</returns>
        public BmsConverter DeleteUnusedWav()
        {
            _bms = BmsDefinitionReplace.GetUnusedWavDeletedBmsFile(_bms);
            return this;
        }

        /// <summary>
        /// #WAV定義を詰める
        /// </summary>
        /// <returns>BmsConverter</returns>
        public BmsConverter ArrangeWav()
        {
            _bms = BmsDefinitionReplace.GetWavArrangedBmsFile(_bms);
            return this;
        }

        /// <summary>
        /// #WAV定義をoffsetの分だけ後ろにずらす
        /// </summary>
        /// <param name="offset">ずらす数</param>
        /// <returns>BmsConverter</returns>
        public BmsConverter Offset(int offset)
        {
            _bms = BmsDefinitionReplace.GetOffsetedBmsFile(_bms, offset);
            return this;
        }

        /// <summary>
        /// BMSを合体する
        /// </summary>
        /// <param name="bms">合体させるBMSテキスト</param>
        /// <returns>BmsConverter</returns>
        public BmsConverter AddRange(BmsConverter bms)
        {
            _bms = BmsDefinitionReplace.GetMargedBms(_bms, bms.Bms);
            return this;
        }

        /// <summary>
        /// #WAV定義を置換する
        /// </summary>
        /// <param name="replaces">置換テーブル</param>
        /// <returns>BmsConverter</returns>
        public BmsConverter Replace(List<BmsReplace> replaces)
        {
            _bms = BmsDefinitionReplace.GetReplacedBmsFile(_bms, replaces);
            return this;
        }
    }
}
