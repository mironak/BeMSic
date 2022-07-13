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
            var wavs = GetWavsRelativePath(bms);
            WavFileUnitUtility wavFiles = new ();

            foreach (var wav in wavs.Files)
            {
                wavFiles.Add(wav.Wav.Num, bmsDirectory + "\\" + wav.Name);
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
                    var bmsLine = new BmsLine(line);

                    if (bmsLine.IsWav())
                    {
                        var wavData = new WavLine(line);
                        wavFiles.Add(wavData.GetWavData());
                    }
                }
            }

            return wavFiles;
        }
    }
}
