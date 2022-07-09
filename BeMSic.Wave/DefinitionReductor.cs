using System.Collections.Immutable;
using BeMSic.Core.BmsDefinition;
using BeMSic.Wave.FileOperation;
using BeMSic.Wave.Validators;
using NAudio.Wave;

namespace BeMSic.Wave
{
    /// <summary>
    /// 定義削減
    /// </summary>
    public class DefinitionReductor
    {
        private readonly ImmutableArray<WavFileUnit> _originalFiles;
        private readonly WaveStream[] _readers;
        private readonly bool _isSameLength;
        private readonly float _r2val;
        private readonly WaveCompare.ValidComparator _comparator;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="originalFiles">ファイル一覧</param>
        /// <param name="isSameLength">同じ長さのみ比較する場合はtrue</param>
        /// <param name="r2val">相関係数</param>
        /// <param name="comparator">比較関数</param>
        public DefinitionReductor(ImmutableArray<WavFileUnit> originalFiles, bool isSameLength, float r2val, WaveCompare.ValidComparator comparator)
        {
            _originalFiles = originalFiles;
            _readers = PreserveWavFileReader(_originalFiles);
            _isSameLength = isSameLength;
            _r2val = r2val;
            _comparator = comparator;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="originalFiles">ファイル一覧</param>
        /// <param name="isSameLength">同じ長さのみ比較する場合はtrue</param>
        /// <param name="r2val">相関係数</param>
        /// <param name="comparator">比較関数</param>
        public DefinitionReductor(ImmutableArray<WavFileUnit> originalFiles, bool isSameLength, float r2val)
            : this(originalFiles, isSameLength, r2val, WaveValidation.CalculateRSquared)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="originalFiles">ファイル一覧</param>
        public DefinitionReductor(ImmutableArray<WavFileUnit> originalFiles)
            : this(originalFiles, true, 90, WaveValidation.CalculateRSquared)
        {
        }

        /// <summary>
        /// Create replaced .wav files table.
        /// </summary>
        /// <param name="progress">IProgress</param>
        /// <returns>置換テーブル</returns>
        public List<WavFileUnit> GetReplacedTable(
            IProgress<int> progress)
        {
            progress.Report(0);

            List<WavFileUnit> replacedFiles = new (_originalFiles);

            // Compare wavs
            for (int i = 0; i < _originalFiles.Length; i++)
            {
                ReplaceWav(replacedFiles, i);

                progress.Report((int)((float)i / _originalFiles.Length * 100));
            }

            progress.Report(100);
            return replacedFiles;
        }

        /// <summary>
        /// wav置換テーブルを取得
        /// </summary>
        /// <param name="progress">IProgress</param>
        /// <returns>置換テーブル</returns>
        public List<BmsReplace> GetWavReplaces(IProgress<int> progress)
        {
            List<BmsReplace> replaces = new ();
            List<WavFileUnit> replaced = GetReplacedTable(progress);

            for (int i = 0; i < _originalFiles.Length; i++)
            {
                replaces.Add(new BmsReplace(_originalFiles[i].Wav, replaced[i].Wav));
            }

            return replaces;
        }

        /// <summary>
        /// Preserve WaveFileReader
        /// </summary>
        /// <param name="wavFileUnits">#WAV一覧(絶対パス)</param>
        /// <returns>WaveStream</returns>
        private static WaveStream[] PreserveWavFileReader(ImmutableArray<WavFileUnit> wavFileUnits)
        {
            WaveStream[] readers = new WaveStream[wavFileUnits.Length];
            for (int i = 0; i < wavFileUnits.Length; i++)
            {
                WaveStream? reader = WaveIO.GetWaveStream(wavFileUnits[i].Name);
                if (reader != null)
                {
                    readers[i] = reader;
                }
            }

            return readers;
        }

        /// <summary>
        /// replace wav
        /// </summary>
        /// <param name="replacedFiles">置換リスト</param>
        /// <param name="index">WaveStreamのインデックス</param>
        private void ReplaceWav(List<WavFileUnit> replacedFiles, int index)
        {
            for (int i = index + 1; i < replacedFiles.Count; i++)
            {
                // Ignore replaced wav
                if (replacedFiles[i].Wav.Num < _originalFiles[i].Wav.Num)
                {
                    continue;
                }

                // Ignore same wav
                if (replacedFiles[i].Name.Equals(_originalFiles[i].Name))
                {
                    continue;
                }

                // Replace wav, if match 2 wavs
                if (WaveCompare.IsMatch(_readers[index], _readers[i], _isSameLength, _r2val, _comparator))
                {
                    replacedFiles[i] = _originalFiles[i];
                }
            }
        }
    }
}
