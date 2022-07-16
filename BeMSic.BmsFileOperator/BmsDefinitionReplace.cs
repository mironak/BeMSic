using BeMSic.BmsFileOperator.LineOperation;
using BeMSic.Core.BmsDefinition;
using BeMSic.Core.Helpers;

namespace BeMSic.BmsFileOperator
{
    /// <summary>
    /// BMSテキストを置換したBMSテキストを作成する
    /// </summary>
    public class BmsDefinitionReplace
    {
        private readonly string _bms;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="bms">BMSテキスト</param>
        public BmsDefinitionReplace(string bms)
        {
            _bms = bms;
        }

        /// <summary>
        /// MAIN行の#WAVをreplacesで置換したBMSテキストを取得
        /// </summary>
        /// <param name="replaces">置換一覧</param>
        /// <returns>置換後BMSテキスト</returns>
        public string GetReplacedBmsFile(List<BmsReplace> replaces)
        {
            string writeData = string.Empty;

            using (StringReader sr = new (_bms))
            {
                string? readLine;
                while ((readLine = sr.ReadLine()) != null)
                {
                    var bmsManager = new BmsManager(readLine);
                    string line = bmsManager.ReplaceLineDefinition(replaces);
                    writeData += line + "\n";
                }
            }

            return writeData;
        }

        /// <summary>
        /// BMSテキストの#WAV番号をoffsetだけ後ろにずらす
        /// </summary>
        /// <param name="offset">ずらす数</param>
        /// <returns>置換後BMSテキスト</returns>
        public string GetOffsetedBmsFile(int offset)
        {
            WavDefinitions wavs = GetWavIndexes();
            string writeData = string.Empty;

            using (StringReader sr = new (_bms))
            {
                string? readLine;
                while ((readLine = sr.ReadLine()) != null)
                {
                    var bmsManager = new BmsManager(readLine);
                    string line = bmsManager.OffsettedLineDefinition(wavs, offset);
                    writeData += line + "\n";
                }
            }

            return writeData;
        }

        /// <summary>
        /// BGMレーンをoffsetの分右にずらす
        /// </summary>
        /// <param name="offset">ずらす数</param>
        /// <returns>置換後BMSテキスト</returns>
        public string GetBgmShiftedBmsFile(int offset)
        {
            string writeData = string.Empty;
            var prevLine = new BmsLine("#99999:00");

            using (StringReader sr = new (_bms))
            {
                string? readLine;
                while ((readLine = sr.ReadLine()) != null)
                {
                    var bmsLine = new BmsLine(readLine);
                    if (!bmsLine.IsBgm())
                    {
                        writeData += readLine + "\n";
                        continue;
                    }

                    if (bmsLine.IsSameBar(prevLine))
                    {
                        writeData += readLine + "\n";
                        continue;
                    }

                    var bmsManager = new BmsManager(readLine);
                    writeData += bmsManager.ShiftBgmLine(offset) + "\n";
                    prevLine = bmsLine;
                }
            }

            return writeData;
        }

        /// <summary>
        /// MAIN行で使用されていない#WAV定義を削除する
        /// </summary>
        /// <returns>置換後BMSテキスト</returns>
        public string GetUnusedWavDeletedBmsFile()
        {
            string writeData = string.Empty;
            string? readLine;
            WavDefinitions wavs = GetUsedWavList();

            using (StringReader sr = new (_bms))
            {
                while ((readLine = sr.ReadLine()) != null)
                {
                    var bmsLine = new BmsLine(readLine);
                    if (bmsLine.IsWav())
                    {
                        if (!wavs.Contains(bmsLine.GetWavNumber()))
                        {
                            continue;
                        }
                    }

                    writeData += readLine + "\n";
                }
            }

            return writeData;
        }

        /// <summary>
        /// #WAV定義を01から順に詰める
        /// </summary>
        /// <returns>置換後BMSテキスト</returns>
        public string GetWavArrangedBmsFile()
        {
            WavDefinitions uniqueList = GetUsedWavList().GetUnique();

            return GetReplacedArrangedData(uniqueList);
        }

        /// <summary>
        /// MAIN行で使用されている#WAV一覧を取得する
        /// </summary>
        /// <returns>#WAV番号一覧</returns>
        public WavDefinitions GetUsedWavList()
        {
            WavDefinitions wavs = new ();

            using (StringReader sr = new (_bms))
            {
                // Get list
                string? readLine;
                while ((readLine = sr.ReadLine()) != null)
                {
                    var bmsManager = new BmsManager(readLine);
                    wavs.AddRange(bmsManager.GetLineDefinition());
                }
            }

            return wavs;
        }

        /// <summary>
        /// BMS2の#WAV定義を追加する
        /// </summary>
        /// <param name="bms">BMSテキスト</param>
        /// <returns>合体後BMS</returns>
        public string GetWavMargedBmsFile(string bms)
        {
            var bms1WavMax = GetUsedWavList().GetMax();
            var rep = new BmsDefinitionReplace(bms);
            var bms2WavMax = rep.GetUsedWavList().GetMax();

            // 定義数ZZ確認
            if (bms1WavMax.Num + bms2WavMax.Num > 1295)
            {
                throw new ArgumentOutOfRangeException(nameof(_bms) + nameof(bms), "Definition over");
            }

            string writeBmsData = string.Empty;
            string bmsOffsetted = new BmsConverter(bms).Offset(bms1WavMax.Num).Bms;

            using (StringReader sr = new (_bms))
            {
                string? readLine;
                while ((readLine = sr.ReadLine()) != null)
                {
                    writeBmsData += readLine + "\n";

                    var bmsLine = new BmsLine(readLine);
                    if (bmsLine.IsWav())
                    {
                        writeBmsData += AddBmsLine(readLine, bms1WavMax.Num, bmsOffsetted);
                    }

                    writeBmsData += readLine + "\n";
                }
            }

            return writeBmsData;
        }

        /// <summary>
        /// #WAVインデックスを前詰めしたBMSデータを取得する
        /// </summary>
        /// <param name="uniqueList">#WAV番号一覧(重複なし)</param>
        /// <returns>前詰め後BMSテキスト</returns>
        public string GetReplacedArrangedData(WavDefinitions uniqueList)
        {
            string writeData = string.Empty;
            string? readLine;

            using (StringReader sr = new (_bms))
            {
                while ((readLine = sr.ReadLine()) != null)
                {
                    var bmsManager = new BmsManager(readLine);
                    writeData += bmsManager.GetArrangedLine(uniqueList);
                }
            }

            return writeData;
        }

        /// <summary>
        /// 行が#WAV定義の最終行のとき、その後ろに#WAV定義を追加する
        /// </summary>
        /// <param name="line">行</param>
        /// <param name="finalWav">#WAV最終番号</param>
        /// <param name="bms2">BMS</param>
        /// <returns>#WAV追加後行</returns>
        private static string AddBmsLine(string line, int finalWav, string bms2)
        {
            int now = RadixConvert.ZZToInt(line.Substring(4, 2));
            if (finalWav != now)
            {
                return string.Empty;
            }

            // #WAV最終番号の後ろに追加する
            string writeData = string.Empty;
            using (StringReader sr = new (bms2))
            {
                string? readLine;
                while ((readLine = sr.ReadLine()) != null)
                {
                    var bmsLine = new BmsLine(readLine);

                    if (bmsLine.IsWav())
                    {
                        writeData += readLine + "\n";
                    }
                }
            }

            return writeData;
        }

        /// <summary>
        /// BMSファイル内の#WAVインデックス一覧を取得
        /// </summary>
        /// <param name="bms">BMSテキスト</param>
        /// <returns>#WAV番号一覧</returns>
        private WavDefinitions GetWavIndexes()
        {
            var wavs = FileList.GetWavsRelativePath(_bms);

            var wavFiles = new WavDefinitions();
            foreach (WavFileUnit wav in wavs.Files)
            {
                wavFiles.Add(wav.Wav);
            }

            return wavFiles;
        }
    }
}
