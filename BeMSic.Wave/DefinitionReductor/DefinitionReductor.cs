using BeMSic.Core.BmsDefinition;
using BeMSic.Wave.DefinitionReductor.Validators;
using NAudio.Wave;

namespace BeMSic.Wave.DefinitionReductor
{
    public static class DefinitionReductor
    {
        /// <summary>
        /// Create replaced .wav files table.
        /// </summary>
        /// <param name="originalFiles">Original files</param>
        /// <param name="progress">IProgress</param>
        /// <param name="r2val">Match rate threshold</param>
        /// <param name="comparator">Evaluation function</param>
        public static List<WavFileUnit> GetReplacedTable(
            List<WavFileUnit> originalFiles,
            IProgress<int> progress,
            float r2val,
            WaveCompare.ValidComparator comparator)
        {
            progress.Report(0);

            List<WavFileUnit> replacedFiles = GetInitReplacedFiles(originalFiles);
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
        /// Create replaced .wav files table.
        /// </summary>
        /// <param name="originalFiles">Original files</param>
        /// <param name="progress">IProgress</param>
        /// <param name="r2val">Match rate threshold</param>
        public static List<WavFileUnit> GetReplacedTable(List<WavFileUnit> originalFiles, IProgress<int> progress, float r2val)
        {
            return GetReplacedTable(originalFiles, progress, r2val, WaveValidation.CalculateRSquared);
        }

        /// <summary>
        /// Create replaced .wav files table.
        /// </summary>
        /// <param name="originalFiles">Original files</param>
        /// <param name="progress">IProgress</param>
        /// <param name="r2val">Match rate threshold</param>
        public static List<BmsReplace> GetWavReplaces(List<WavFileUnit> originalFiles, IProgress<int> progress, float r2val)
        {
            List<BmsReplace> replaces = new List<BmsReplace>();
            var replaced = GetReplacedTable(originalFiles, progress, r2val, WaveValidation.CalculateRSquared);

            for(int i = 0; i < originalFiles.Count; i++)
            {
                replaces.Add(new BmsReplace { NowDefinition = originalFiles[i].Num, NewDefinition = replaced[i].Num });
            }
            return replaces;
        }

        /// <summary>
        /// Get init replaced files
        /// </summary>
        /// <param name="originalFiles"></param>
        /// <returns></returns>
        private static List<WavFileUnit> GetInitReplacedFiles(List<WavFileUnit> originalFiles)
        {
            List<WavFileUnit> replacedFiles = new List<WavFileUnit>(originalFiles);
            return replacedFiles;
        }

        /// <summary>
        /// replace wav
        /// </summary>
        /// <param name="originalFiles"></param>
        /// <param name="replacedFiles"></param>
        /// <param name="r2val"></param>
        /// <param name="comparator"></param>
        /// <param name="readers"></param>
        /// <param name="index"></param>
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
        /// <param name="originalFiles"></param>
        /// <returns></returns>
        private static WaveStream[] PreserveWavFileReader(List<WavFileUnit> wavFileUnits)
        {
            WaveStream[] readers = new WaveStream[wavFileUnits.Count];
            for (int i = 0; i < wavFileUnits.Count; i++)
            {
                var reader = Wave.WaveManipulator.Wave.GetWaveStream(wavFileUnits[i].Name);
                if(reader != null)
                {
                    readers[i] = reader;
                }
            }
            return readers;
        }
    }
}
