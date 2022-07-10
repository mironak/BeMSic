namespace BeMSic.Core.BmsDefinition
{
    /// <summary>
    /// #WAV定義一覧
    /// </summary>
    public class WavDefinitions
    {
        private List<WavDefinition> _wavs;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WavDefinitions()
        {
            _wavs = new List<WavDefinition>();
        }

        /// <summary>
        /// #WAV定義を追加する
        /// </summary>
        /// <param name="wav">#WAV定義</param>
        public void Add(WavDefinition wav)
        {
            _wavs.Add(wav);
        }

        /// <summary>
        /// #WAV定義一覧を追加する
        /// </summary>
        /// <param name="wavs">#WAV定義一覧</param>
        public void AddRange(WavDefinitions wavs)
        {
            foreach (WavDefinition wav in wavs.GetUnit())
            {
                _wavs.Add(wav);
            }
        }

        /// <summary>
        /// 引数の#WAV番号があるか確認
        /// </summary>
        /// <param name="wav">#WAV番号</param>
        /// <returns>引数の#WAV番号があればtrue</returns>
        public bool Contains(WavDefinition wav)
        {
            return _wavs.Contains(wav);
        }

        /// <summary>
        /// 重複なしのものを返す
        /// </summary>
        /// <returns>#WAV番号一覧</returns>
        public WavDefinitions GetUnique()
        {
            var dt = _wavs.Distinct().OrderBy(i => i);

            var aaa = new WavDefinitions();
            foreach (var a in dt)
            {
                aaa.Add(a);
            }

            return aaa;
        }

        /// <summary>
        /// 最大の#WAV番号を返す
        /// </summary>
        /// <returns>#WAV番号</returns>
        public WavDefinition GetMax()
        {
            return new WavDefinition(_wavs.Max(x => x.Num));
        }

        /// <summary>
        /// #WAV番号を順に返す
        /// </summary>
        /// <returns>#WAV番号</returns>
        public IEnumerable<WavDefinition> GetUnit()
        {
            for (int i = 0; i < _wavs.Count; i++)
            {
                yield return _wavs[i];
            }
        }
    }
}
