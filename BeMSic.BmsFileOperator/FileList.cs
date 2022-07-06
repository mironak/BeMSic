using BeMSic.BmsFileOperator.LineOperation;
using BeMSic.Core.BmsDefinition;

namespace BeMSic.BmsFileOperator
{
    /// <summary>
    /// ファイル一覧
    /// </summary>
    public static class FileList
    {
        /// <summary>
        /// #WAV一覧を取得(wavは絶対パス)
        /// </summary>
        /// <param name="bms">BMSテキスト</param>
        /// <param name="bmsDirectory">BMSファイルのあるディレクトリ</param>
        /// <returns>#WAV絶対パス</returns>
        public static WavFileUnitUtility GetWavsFullPath(string bms, string bmsDirectory)
        {
            var wavs = GetWavsRelativePath(bms).Get();

            WavFileUnitUtility wavFiles = new ();
            foreach (WavFileUnit wav in wavs)
            {
                wav.Name = bmsDirectory + "\\" + wav.Name;
                wavFiles.Add(wav);
            }

            return wavFiles;
        }

        /// <summary>
        /// #WAV一覧を取得(wavは相対パス)
        /// </summary>
        /// <param name="bms">BMSテキスト</param>
        /// <returns>#WAV相対パス</returns>
        public static WavFileUnitUtility GetWavsRelativePath(string bms)
        {
            WavFileUnitUtility wavFiles = new ();

            using (StringReader sr = new (bms))
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
