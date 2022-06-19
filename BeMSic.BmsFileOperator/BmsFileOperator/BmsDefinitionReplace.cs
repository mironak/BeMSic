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
            var wavList = FileList.GetAllWavListFromText(bmsFileText);
            string writeData = "";

            using (StringReader sr = new StringReader(bmsFileText))
            {
                string? readLine;
                while ((readLine = sr.ReadLine()) != null)
                {
                    string line = BmsManager.OffsettedLineDefinition(readLine, wavList, offset);
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
            List<int> wavTable = GetUsedWavList(bmsFileText);

            using (StringReader sr = new StringReader(bmsFileText))
            {
                while ((readLine = sr.ReadLine()) != null)
                {
                    if (BmsManager.GetLineCommand(readLine) == BmsManager.BmsCommand.WAV)
                    {
                        var index = wavTable.IndexOf(RadixConvert.ZZToInt(readLine.Substring(4, 2)));
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
        /// Get used #WAV list from MAIN line
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        public static List<int> GetUsedWavList(string bmsFileText)
        {
            List<int> wavTable = new List<int>();

            using (StringReader sr = new StringReader(bmsFileText))
            {
                // Get list
                string? readLine;
                while ((readLine = sr.ReadLine()) != null)
                {
                    wavTable.AddRange(BmsManager.GetLineDefinition(readLine));
                }
            }
            return wavTable;
        }

        /// <summary>
        /// BMS1にBMS2を追加する
        /// </summary>
        /// <param name="bms1"></param>
        /// <param name="bms2"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static string GetMargedBms(string bms1, string bms2, int offset)
        {
            int max = 0;
            string writeBmsData = "";
            var bmsOffsetted = (new BmsConverter(bms2)).Offset(offset).Bms;

            using (StringReader sr = new StringReader(bms1))
            {
                string? readLine;
                while ((readLine = sr.ReadLine()) != null)
                {
                    writeBmsData += (readLine + "\n");

                    switch (BmsManager.GetLineCommand(readLine))
                    {
                        case BmsManager.BmsCommand.WAV:
                            writeBmsData += AddBmsLine(readLine, offset, bmsOffsetted);
                            break;

                        case BmsManager.BmsCommand.MAIN:
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
                    switch (BmsManager.GetLineCommand(readLine))
                    {
                        case BmsManager.BmsCommand.WAV:
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
                    switch (BmsManager.GetLineCommand(readLine))
                    {
                        case BmsManager.BmsCommand.MAIN:
                        case BmsManager.BmsCommand.MAIN_NOTOBJ:
                            isMainLine = true;
                            writeData += BmsManager.ReplaceMainLineNumber(readLine, maxLine) + "\n";
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
        /// Get replaced line 
        /// </summary>
        /// <param name="readLine"></param>
        /// <param name="uniqueList"></param>
        /// <returns></returns>
        private static string GetReplacedLineArrangedWav(string readLine, List<int> uniqueList)
        {
            return BmsManager.ReductLineDefinition(readLine, uniqueList);
        }

        /// <summary>
        /// Get replaced arranged data
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
                    writeData += GetReplacedLineArrangedWav(readLine, uniqueList);
                }
            }
            return writeData;
        }
    }
}
