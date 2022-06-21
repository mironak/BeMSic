using BeMSic.Core.Helpers;
using BeMSic.Core.BmsDefinition;

namespace BeMSic.BmsFileOperator
{
    /// <summary>
    /// BMSテキストを置換したBMSテキストを作成する
    /// </summary>
    public static class BmsDefinitionReplace
    {
        /// <summary>
        /// MAIN行の#WAVをreplacesで置換したBMSテキストを取得
        /// </summary>
        /// <param name="bmsFileText">BMSテキスト</param>
        /// <param name="replaces">置換一覧</param>
        /// <returns>置換後BMSテキスト</returns>
        public static string GetReplacedBmsFile(string bmsFileText, List<BmsReplace> replaces)
        {
            string writeData = "";

            using (StringReader sr = new StringReader(bmsFileText))
            {
                string? readLine;
                while ((readLine = sr.ReadLine()) != null)
                {
                    string line = BmsManager.ReplaceLineDefinition(readLine, replaces);
                    writeData += (line + "\n");
                }
            }
            return writeData;
        }

        /// <summary>
        /// BMSテキストの#WAV番号をoffsetだけ後ろにずらす
        /// </summary>
        /// <param name="bmsFileText">BMSテキスト</param>
        /// <param name="offset">ずらす数</param>
        /// <returns>置換後BMSテキスト</returns>
        public static string GetOffsetedBmsFile(string bmsFileText, int offset)
        {
            var wavs = GetWavIndexes(bmsFileText);
            string writeData = "";

            using (StringReader sr = new StringReader(bmsFileText))
            {
                string? readLine;
                while ((readLine = sr.ReadLine()) != null)
                {
                    string line = BmsManager.OffsettedLineDefinition(readLine, wavs, offset);
                    writeData += (line + "\n");
                }
            }
            return writeData;
        }

        /// <summary>
        /// MAIN行で使用されていない#WAV定義を削除する
        /// </summary>
        /// <param name="bmsFileText">BMSテキスト</param>
        /// <returns>置換後BMSテキスト</returns>
        public static string GetUnusedWavDeletedBmsFile(string bmsFileText)
        {
            string writeData = "";
            string? readLine;
            List<int> wavs = GetUsedWavList(bmsFileText);

            using (StringReader sr = new StringReader(bmsFileText))
            {
                while ((readLine = sr.ReadLine()) != null)
                {
                    if (BmsCommandSearch.GetLineCommand(readLine) == BmsCommandSearch.BmsCommand.WAV)
                    {
                        var index = wavs.IndexOf(RadixConvert.ZZToInt(readLine.Substring(4, 2)));
                        if (index == -1)
                        {
                            continue;
                        }
                    }
                    writeData += (readLine + "\n");
                }
            }
            return writeData;
        }

        /// <summary>
        /// #WAV定義を01から順に詰める
        /// </summary>
        /// <param name="bmsFileText">BMSテキスト</param>
        /// <returns>置換後BMSテキスト</returns>
        public static string GetWavArrangedBmsFile(string bmsFileText)
        {
            var uniqueList = GetUsedWavList(bmsFileText).Distinct().OrderBy(i => i).ToList();

            return GetReplacedArrangedData(bmsFileText, uniqueList);
        }

        /// <summary>
        /// MAIN行で使用されている#WAV一覧を取得する
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        public static List<int> GetUsedWavList(string bmsFileText)
        {
            List<int> wavs = new List<int>();

            using (StringReader sr = new StringReader(bmsFileText))
            {
                // Get list
                string? readLine;
                while ((readLine = sr.ReadLine()) != null)
                {
                    wavs.AddRange(BmsManager.GetLineDefinition(readLine));
                }
            }
            return wavs;
        }

        /// <summary>
        /// BMS1にBMS2を追加する
        /// </summary>
        /// <param name="bms1"></param>
        /// <param name="bms2"></param>
        /// <returns></returns>
        public static string GetMargedBms(string bms1, string bms2)
        {
            var bms1WavMax = GetUsedWavList(bms1).Max();
            var bms2WavMax = GetUsedWavList(bms2).Max();

            // 定義数ZZ確認
            if (bms1WavMax + bms2WavMax > 1295)
            {
                throw new ArgumentOutOfRangeException();
            }

            int max = 0;
            string writeBmsData = "";
            var bmsOffsetted = (new BmsConverter(bms2)).Offset(bms1WavMax).Bms;

            using (StringReader sr = new StringReader(bms1))
            {
                string? readLine;
                while ((readLine = sr.ReadLine()) != null)
                {
                    writeBmsData += (readLine + "\n");

                    switch (BmsCommandSearch.GetLineCommand(readLine))
                    {
                        case BmsCommandSearch.BmsCommand.WAV:
                            writeBmsData += AddBmsLine(readLine, bms1WavMax, bmsOffsetted);
                            break;

                        case BmsCommandSearch.BmsCommand.MAIN:
                            var now = GetLineNumber(readLine);
                            // 最終小節番号を保持
                            if (max < now)
                            {
                                max = now;
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
            writeBmsData += AddMainLine(bmsOffsetted, max + 1) + "\n";

            return writeBmsData;
        }

        /// <summary>
        /// #WAVインデックスを前詰めしたBMSデータを取得する
        /// </summary>
        /// <param name="sr"></param>
        /// <param name="uniqueList"></param>
        /// <returns></returns>
        public static string GetReplacedArrangedData(string bmsFileText, List<int> uniqueList)
        {
            string writeData = "";
            string? readLine;

            using (StringReader sr = new StringReader(bmsFileText))
            {
                while ((readLine = sr.ReadLine()) != null)
                {
                    writeData += BmsManager.GetArrangedLine(readLine, uniqueList);
                }
            }
            return writeData;
        }

        private static string AddBmsLine(string line, int finalWav, string bms2)
        {
            int now = RadixConvert.ZZToInt(line.Substring(4, 2));
            if (finalWav != now)
            {
                return "";
            }

            // #WAV最終番号の後ろに追加する
            string writeData = "";
            using (StringReader sr = new StringReader(bms2))
            {
                string? readLine;
                while ((readLine = sr.ReadLine()) != null)
                {
                    switch (BmsCommandSearch.GetLineCommand(readLine))
                    {
                        case BmsCommandSearch.BmsCommand.WAV:
                            writeData += (readLine + "\n");
                            break;

                        default:
                            break;
                    }
                }
            }
            return writeData;
        }

        // MAIN行の小節番号を取得する
        private static int GetLineNumber(string line)
        {
            int now;
            var success = int.TryParse(line.Substring(1, 3), out now);
            if (!success)
            {
                return 0;
            }
            return now;
        }

        // BMS2のMAINレーンをBMS1の最終小節以降の小節番号に変えたものを取得
        private static string AddMainLine(string bms2, int maxLine)
        {
            string writeData = "";
            bool isMainLine = false;

            using (StringReader sr = new StringReader(bms2))
            {
                string? readLine;
                while ((readLine = sr.ReadLine()) != null)
                {
                    switch (BmsCommandSearch.GetLineCommand(readLine))
                    {
                        case BmsCommandSearch.BmsCommand.MAIN:
                        case BmsCommandSearch.BmsCommand.MAIN_NOTOBJ:
                            isMainLine = true;
                            writeData += MainLineManager.OffsetMainLineBar(readLine, maxLine) + "\n";
                            break;

                        default:
                            if (isMainLine)
                            {
                                writeData += readLine + "\n";
                            }
                            break;
                    }
                }
            }
            return writeData;
        }

        /// <summary>
        /// BMSファイル内の#WAVインデックス一覧を取得
        /// </summary>
        /// <param name="bms"></param>
        /// <returns></returns>
        private static List<int> GetWavIndexes(string bms)
        {
            var wavs = FileList.GetWavsRelativePath(bms);

            List<int> wavFiles = new List<int>();
            foreach (var wav in wavs)
            {
                wavFiles.Add(wav.Num);
            }
            return wavFiles;
        }
    }
}
