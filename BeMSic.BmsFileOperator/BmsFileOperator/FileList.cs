using BeMSic.Core.BmsDefinition;
using BeMSic.Core.Helpers;

namespace BeMSic.BmsFileOperator
{
    public static class FileList
    {
        /// <summary>
        /// Get partial wav file list
        /// </summary>
        /// <param name="files"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static List<WavFileUnit> GetPartialWavList(List<WavFileUnit> files, int start, int end)
        {
            if (!IsInRange(start, end))
            {
                throw new ArgumentOutOfRangeException();
            }

            return GetPartialWavs(files, start, end);
        }

        /// <summary>
        /// Get partial wav file list
        /// </summary>
        /// <param name="fileList"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private static List<WavFileUnit> GetPartialWavs(List<WavFileUnit> fileList, int start, int end)
        {
            List<WavFileUnit> partialWavs = new List<WavFileUnit>();
            foreach (var wav in fileList)
            {
                if (wav.Num < start)
                {
                    continue;
                }
                if (end < wav.Num)
                {
                    break;
                }
                partialWavs.Add(wav);
            }
            return partialWavs;
        }

        /// <summary>
        /// Get all wav list
        /// </summary>
        /// <param name="bmsFilePath"></param>
        /// <param name="bmsDirectory"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static List<WavFileUnit> GetWavFiles(string bms, string bmsDirectory)
        {
            List<WavFileUnit> wavFileList = new List<WavFileUnit>();

            using (StringReader sr = new StringReader(bms))
            {
                string? line;

                while ((line = sr.ReadLine()) != null)
                {
                    if (BmsManager.GetLineCommand(line) == BmsManager.BmsCommand.WAV)
                    {
                        (string def, string fileName) wavData = BmsManager.GetWavData(line);
                        wavFileList.Add(new WavFileUnit(RadixConvert.ZZToInt(wavData.def), GetWavFullPath(bmsDirectory, wavData.fileName)));
                    }
                }
            }
            return wavFileList;
        }

        /// <summary>
        /// Get all #wav list
        /// </summary>
        /// <param name="bmsFilePath"></param>
        /// <param name="bmsDirectory"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static List<int> GetAllWavListFromText(string bms)
        {
            List<int> wavFileList = new List<int>();

            using (StringReader sr = new StringReader(bms))
            {
                string? line;

                while ((line = sr.ReadLine()) != null)
                {
                    if (BmsManager.GetLineCommand(line) == BmsManager.BmsCommand.WAV)
                    {
                        (string def, string fileName) wavData = BmsManager.GetWavData(line);
                        wavFileList.Add(RadixConvert.ZZToInt(wavData.def));
                    }
                }
            }
            return wavFileList;
        }

        /// <summary>
        /// Get wav full path
        /// </summary>
        /// <param name="dirName"></param>
        /// <param name="baseName"></param>
        /// <returns></returns>
        private static string GetWavFullPath(string dirName, string fileName)
        {
            return dirName + "\\" + fileName;
        }

        /// <summary>
        /// #WAV numbers are in range of "01" to "ZZ"
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private static bool IsInRange(int start, int end)
        {
            if (end <= start)
            {
                return false;
            }

            if (start < 1)
            {
                return false;
            }

            if (RadixConvert.ZZToInt("ZZ") < end)
            {
                return false;
            }
            return true;
        }
    }
}
