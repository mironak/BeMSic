using BeMSic.Core.BmsDefinition;
using BeMSic.Wave.FileOperation;
using BeMSic.Wave.Validators;
using NAudio.Wave;

namespace BeMSic.Wave
{
    /// <summary>
    /// 定義削減
    /// </summary>
    public static class DefinitionReductor
    {
        /// <summary>
        /// Create replaced .wav files table.
        /// </summary>
        /// <param name="originalFiles">Original files</param>
        /// <param name="progress">IProgress</param>
        /// <param name="r2val">Match rate threshold</param>
        /// <param name="comparator">Evaluation function</param>
        /// <returns>置換テーブル</returns>
        public static List<WavFileUnit> GetReplacedTable(
            List<WavFileUnit> originalFiles,
            IProgress<int> progress,
            float r2val,
            WaveCompare.ValidComparator comparator)
        {
            progress.Report(0);

            List<WavFileUnit> replacedFiles = new (originalFiles);
            WaveStream[] readers = PreserveWavFileReader(originalFiles);

            // Compare wavs
            for (int i = 0; i < originalFiles.Count; i++)
            {
                ReplaceWav(originalFiles[i], replacedFiles, r2val, comparator, readers, i);

                progress.Report((int)((float)i / originalFiles.Count * 100));
            }

            progress.Report(100);
            return replacedFiles;
        }

        /// <summary>
        /// wav置換テーブルを取得(デフォルト比較関数使用)
        /// </summary>
        /// <param name="originalFiles">Original files</param>
        /// <param name="progress">IProgress</param>
        /// <param name="r2val">Match rate threshold</param>
        /// <returns>置換テーブル</returns>
        public static List<WavFileUnit> GetReplacedTable(List<WavFileUnit> originalFiles, IProgress<int> progress, float r2val)
        {
            return GetReplacedTable(originalFiles, progress, r2val, WaveValidation.CalculateRSquared);
        }

        /// <summary>
        /// wav置換テーブルを取得
        /// </summary>
        /// <param name="originalFiles">Original files</param>
        /// <param name="progress">IProgress</param>
        /// <param name="r2val">Match rate threshold</param>
        /// <returns>置換テーブル</returns>
        public static List<BmsReplace> GetWavReplaces(List<WavFileUnit> originalFiles, IProgress<int> progress, float r2val)
        {
            List<BmsReplace> replaces = new ();
            List<WavFileUnit> replaced = GetReplacedTable(originalFiles, progress, r2val, WaveValidation.CalculateRSquared);

            for (int i = 0; i < originalFiles.Count; i++)
            {
                replaces.Add(new BmsReplace(originalFiles[i].Num, replaced[i].Num));
            }

            return replaces;
        }

        /// <summary>
        /// replace wav
        /// </summary>
        /// <param name="originalFiles">置換対象</param>
        /// <param name="replacedFiles">置換リスト</param>
        /// <param name="r2val">相関係数</param>
        /// <param name="comparator">比較関数</param>
        /// <param name="readers">WaveStream</param>
        /// <param name="index">WaveStreamのインデックス</param>
        private static void ReplaceWav(
            WavFileUnit originalFiles,
            List<WavFileUnit> replacedFiles,
            float r2val,
            WaveCompare.ValidComparator comparator,
            WaveStream[] readers,
            int index)
        {
            for (int i = index + 1; i < replacedFiles.Count; i++)
            {
                // Ignore replaced wav
                if (replacedFiles[i].Num < originalFiles.Num)
                {
                    continue;
                }

                // Ignore same wav
                if (replacedFiles[i].Name.Equals(originalFiles.Name))
                {
                    continue;
                }

                // Replace wav, if match 2 wavs
                if (WaveCompare.IsMatch(readers[index], readers[i], r2val, comparator))
                {
                    replacedFiles[i] = originalFiles;
                }
            }
        }

        /// <summary>
        /// Preserve WaveFileReader
        /// </summary>
        /// <param name="wavFileUnits">#WAV一覧(絶対パス)</param>
        /// <returns>WaveStream</returns>
        private static WaveStream[] PreserveWavFileReader(List<WavFileUnit> wavFileUnits)
        {
            WaveStream[] readers = new WaveStream[wavFileUnits.Count];
            for (int i = 0; i < wavFileUnits.Count; i++)
            {
                WaveStream? reader = WaveIO.GetWaveStream(wavFileUnits[i].Name);
                if (reader != null)
                {
                    readers[i] = reader;
                }
            }

            return readers;
        }
    }
}
