namespace BeMSic.Core.BmsDefinition
{
    /// <summary>
    /// #WAV定義一覧
    /// </summary>
    public class WavDefinitions
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WavDefinitions()
        {
            Wavs = new List<WavDefinition>();
        }

        public List<WavDefinition> Wavs { get; }

        /// <summary>
        /// #WAV定義を追加する
        /// </summary>
        /// <param name="wav">#WAV定義</param>
        public void Add(WavDefinition wav)
        {
            Wavs.Add(wav);
        }

        /// <summary>
        /// #WAV定義一覧を追加する
        /// </summary>
        /// <param name="wavs">#WAV定義一覧</param>
        public void AddRange(WavDefinitions wavs)
        {
            foreach (WavDefinition wav in wavs.Wavs)
            {
                Wavs.Add(wav);
            }
        }

        /// <summary>
        /// 引数の#WAV番号があるか確認
        /// </summary>
        /// <param name="wav">#WAV番号</param>
        /// <returns>引数の#WAV番号があればtrue</returns>
        public bool Contains(WavDefinition wav)
        {
            return Wavs.Contains(wav);
        }

        /// <summary>
        /// 重複なしのものを返す
        /// </summary>
        /// <returns>#WAV番号一覧</returns>
        public WavDefinitions GetUnique()
        {
            var dt = Wavs.Distinct().OrderBy(i => i.Num);

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
            return new WavDefinition(Wavs.Max(x => x.Num));
        }
    }
}
