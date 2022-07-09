﻿using System.Collections.Immutable;
using BeMSic.Core.Helpers;

namespace BeMSic.Core.BmsDefinition
{
    /// <summary>
    /// WAVファイル定義操作
    /// </summary>
    public class WavFileUnitUtility
    {
        private List<WavFileUnit> _files;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WavFileUnitUtility()
        {
            _files = new List<WavFileUnit>();
        }

        /// <summary>
        /// WAVファイル情報追加
        /// </summary>
        /// <param name="num">#WAV番号</param>
        /// <param name="name">ファイル名</param>
        public void Add(int num, string name)
        {
            _files.Add(new WavFileUnit(num, name));
        }

        /// <summary>
        /// WAVファイル情報追加
        /// </summary>
        /// <param name="wav">#WAV情報</param>
        public void Add(WavFileUnit wav)
        {
            _files.Add(wav);
        }

        /// <summary>
        /// #WAV定義数を返す
        /// </summary>
        /// <returns>#WAV定義数</returns>
        public int Count()
        {
            return _files.Count;
        }

        /// <summary>
        /// #WAVリストを返す
        /// </summary>
        /// <returns>#WAVリスト</returns>
        public ImmutableArray<WavFileUnit> Get()
        {
            return _files.ToImmutableArray();
        }

        /// <summary>
        /// Get partial wav file list
        /// </summary>
        /// <param name="start">#WAV定義開始番号</param>
        /// <param name="end">#WAV定義最終番号</param>
        /// <returns>#WAV定義一覧(startからendまで)</returns>
        public WavFileUnitUtility GetPartialWavs(WavDefinition start, WavDefinition end)
        {
            if (!IsInRange(start, end))
            {
                throw new ArgumentOutOfRangeException(nameof(start) + "and" + nameof(end), "Not in range");
            }

            return GetPartialWavsCore(start, end);
        }

        /// <summary>
        /// #WAVインデックスが"01"から"ZZ"の範囲ならtrue
        /// </summary>
        /// <param name="start">#WAV定義開始番号</param>
        /// <param name="end">#WAV定義最終番号</param>
        /// <returns>startとendが"01"から"ZZ"の範囲ならtrue</returns>
        private static bool IsInRange(WavDefinition start, WavDefinition end)
        {
            if (end.Num <= start.Num)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get partial wav file list
        /// </summary>
        /// <param name="start">#WAV定義開始番号</param>
        /// <param name="end">#WAV定義最終番号</param>
        /// <returns>#WAV定義一覧(startからendまで)</returns>
        private WavFileUnitUtility GetPartialWavsCore(WavDefinition start, WavDefinition end)
        {
            WavFileUnitUtility partialWavs = new ();

            foreach (WavFileUnit wav in _files)
            {
                if (wav.Wav.Num < start.Num)
                {
                    continue;
                }

                if (end.Num < wav.Wav.Num)
                {
                    break;
                }

                partialWavs.Add(wav);
            }

            return partialWavs;
        }
    }
}
