using BeMSic.Core.BmsDefinition;

namespace BeMSic.BmsFileOperator
{
    public static class FileList
    {
        /// <summary>
        /// #WAV一覧を取得(wavは絶対パス)
        /// </summary>
        /// <param name="bms"></param>
        /// <param name="bmsDirectory"></param>
        /// <returns></returns>
        public static List<WavFileUnit> GetWavsFullPath(string bms, string bmsDirectory)
        {
            var wavs = GetWavsRelativePath(bms);

            List<WavFileUnit> wavFiles = new List<WavFileUnit>();
            foreach (var wav in wavs)
            {
                wav.Name = bmsDirectory + "\\" + wav.Name;
                wavFiles.Add(wav);
            }
            return wavFiles;
        }

        /// <summary>
        /// #WAV一覧を取得(wavは相対パス)
        /// </summary>
        /// <param name="bms"></param>
        /// <returns></returns>
        public static List<WavFileUnit> GetWavsRelativePath(string bms)
        {
            List<WavFileUnit> wavFiles = new List<WavFileUnit>();

            using (StringReader sr = new StringReader(bms))
            {
                string? line;

                while ((line = sr.ReadLine()) != null)
                {
                    if (BmsCommandSearch.GetLineCommand(line) == BmsCommandSearch.BmsCommand.WAV)
                    {
                        wavFiles.Add(WavLineManager.GetWavData(line));
                    }
                }
            }
            return wavFiles;
        }
    }
}
