using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeMSic.Core.BmsDefinition
{
    /// <summary>
    /// MAIN行の#WAVインデックスを1つずつ取り出す
    /// </summary>
    internal class MainDefinitionReader
    {
        internal const int DataStart = 7;
        private readonly string _line;
        private int _pos;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="line">MAIN行</param>
        internal MainDefinitionReader(string line)
        {
            _line = line;
            _pos = DataStart;
        }

        /// <summary>
        /// 次の#WAV番号を確認
        /// </summary>
        /// <returns>次の#WAV番号があればtrue</returns>
        internal bool HasNext()
        {
            return _pos < (_line.Length - 1);
        }

        /// <summary>
        /// 次の#WAV番号を取得
        /// </summary>
        /// <returns>#WAV番号</returns>
        internal string Next()
        {
            var ret = _line.Substring(_pos, 2);
            _pos += 2;
            return ret;
        }
    }
}
