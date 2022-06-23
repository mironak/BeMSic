using BeMSic.Core.BmsDefinition;
using BeMSic.Core.Helpers;
using System.Text;

namespace BeMSic.Bmson
{
    public class BmsBuilder
    {
        /// <summary>
        /// 小節線情報
        /// </summary>
        class LineInfo
        {
            /// <summary>
            /// 小節番号
            /// </summary>
            public int Num { get; set; }

            /// <summary>
            /// 小節長さ
            /// </summary>
            public ulong Interval { get; set; }

            /// <summary>
            /// 小節線先頭位置
            /// </summary>
            public ulong Head { get; set; }
        }

        /// <summary>
        /// ノート相対位置
        /// </summary>
        class RelativeNote
        {
            /// <summary>
            /// 小節先頭からの位置
            /// </summary>
            public ulong Position { get; set; }

            /// <summary>
            /// #WAV番号
            /// </summary>
            public int Wav { get; set; }

            /// <summary>
            /// 鍵盤番号
            /// </summary>
            public int Key { get; set; }
        }

        List<WavFileUnit> _wavs;      // #WAV
        List<double> _exbpms = new List<double>();   // BPM(その他)
        Bmson _bmson;
        StringBuilder _builder = new StringBuilder();
        int _soundIndex;

        /// <summary>
        /// Constructor
        /// </summary>
        public BmsBuilder(Bmson bmson, int soundIndex)
        {
            _bmson = bmson;
            _soundIndex = soundIndex;
            _wavs = new List<WavFileUnit>();
        }

        /// <summary>
        /// Append #WAV definition
        /// </summary>
        /// <param name="wav"></param>
        public void AppendWav(WavFileUnit wav)
        {
            _wavs.Add(wav);
        }

        /// <summary>
        /// BMSテキストを生成する
        /// </summary>
        public string Generate()
        {
            SetBmsHeaderField();
            SetWavField();
            SetBpmField();

            SetMainDataField();

            return _builder.ToString();
        }

        /// <summary>
        /// Set bms header field
        /// </summary>
        private void SetBmsHeaderField()
        {
            _builder.AppendLine("*---------------------- HEADER FIELD");
            _builder.AppendLine("");
            _builder.AppendLine($"#GENRE {_bmson.info.genre}");
            _builder.AppendLine($"#TITLE {_bmson.info.title}");
            _builder.AppendLine($"#ARTIST {_bmson.info.artist}");
            _builder.AppendLine($"#BPM {_bmson.info.init_bpm}");
            _builder.AppendLine($"#PLAYLEVEL {_bmson.info.level}");
            _builder.AppendLine("");
        }

        /// <summary>
        /// Set #WAV field
        /// </summary>
        private void SetWavField()
        {
            // Set each #WAV
            for (int i = 0; i < _wavs.Count; i++)
            {
                _builder.Append(GetWavLineText(i + 1, _wavs[i].Name));
            }

            _builder.AppendLine("");
        }

        /// <summary>
        /// Set #BPM field
        /// </summary>
        private void SetBpmField()
        {
            var exbpms = new HashSet<double>();

            // bpm_eventsを探索し、0-255以外をまとめる(#BPM用)
            for (int i = 0; i < _bmson.bpm_events.Length; i++)
            {
                if (_bmson.bpm_events[i].bpm != (byte)_bmson.bpm_events[i].bpm)
                {
                    exbpms.Add(_bmson.bpm_events[i].bpm);
                }
            }
            _exbpms = exbpms.ToList();

            // #BPMを追加
            for (int i = 0; i < _exbpms.Count; i++)
            {
                _builder.Append(GetBpmLineText(i + 1, _exbpms[i].ToString()));

            }

            _builder.AppendLine("");
        }

        /// <summary>
        /// Set Main data field
        /// </summary>
        private void SetMainDataField()
        {
            _builder.AppendLine("*----------------------MAIN DATA FIELD");
            _builder.AppendLine("");

            AppendLines();
        }

        private void AppendLines()
        {
            int lineIndex = 0;
            ulong lineHead = 0;
            foreach (var line in _bmson.lines)
            {
                var interval = (line.y - lineHead) / (ulong)_bmson.info.resolution;
                if (interval == 0)
                {
                    continue;
                }
                LineInfo nowLineInfo = new LineInfo { Num = lineIndex, Interval = interval, Head = lineHead};

                AppendBar(nowLineInfo);
                AppendNotes(nowLineInfo);
                AppendBpms(nowLineInfo);

                lineHead = line.y;
                lineIndex++;
            }
        }

        /// <summary>
        /// 小節長さ(4/4拍子は出力しない)
        /// </summary>
        /// <param name="nowLineInfo"></param>
        private void AppendBar(LineInfo nowLineInfo)
        {
            if ((nowLineInfo.Interval != 0) && (nowLineInfo.Interval != 4))
            {
                _builder.AppendLine($"#{nowLineInfo.Num.ToString("D3")}02:{nowLineInfo.Interval / 2d}");
            }
        }

        /// <summary>
        /// ノート
        /// </summary>
        /// <param name="nowLineInfo"></param>
        private void AppendNotes(LineInfo nowLineInfo)
        {
            List<RelativeNote> notes = new List<RelativeNote>();

            for (int i = 0; i < _bmson.sound_channels[_soundIndex].notes.Count(); i++)
            {
                // ノートの小節番号を取得する
                var noteLine = GetNoteLine(_bmson.sound_channels[_soundIndex].notes[i].y);
                if (noteLine.Num != nowLineInfo.Num)
                {
                    continue;
                }

                // 前のノートと同じ小節の時、一時領域に追加
                notes.Add(new RelativeNote
                {
                    Position = _bmson.sound_channels[_soundIndex].notes[i].y - noteLine.Head,
                    Wav = i + 1,
                    Key = _bmson.sound_channels[_soundIndex].notes[i].x,
                });
            }

            // ノートが保持されていれば出力する。
            if (notes.Count > 0)
            {
                // 前のノートから小節が変わった場合、noteの位置を計算し、割り当てる。
                SetLinesNotes(nowLineInfo, notes);
                _builder.AppendLine("");
            }
        }

        /// <summary>
        /// BPM
        /// </summary>
        /// <param name="nowLineInfo"></param>
        private void AppendBpms(LineInfo nowLineInfo)
        { 
            List<RelativeNote> bpms = new List<RelativeNote>();
            List<RelativeNote> exbpms = new List<RelativeNote>();

            for (int i = 0; i < _bmson.bpm_events.Count(); i++)
            {
                // ノートの小節番号を取得する
                var noteLine = GetNoteLine(_bmson.bpm_events[i].y);
                if (noteLine.Num != nowLineInfo.Num)
                {
                    continue;
                }

                if (_exbpms.Contains(_bmson.bpm_events[i].bpm))
                {
                    exbpms.Add(new RelativeNote
                    {
                        Position = _bmson.bpm_events[i].y - noteLine.Head,
                        Wav = _exbpms.IndexOf(_bmson.bpm_events[i].bpm) + 1,
                        Key = 0,
                    });
                }
                else
                {
                    bpms.Add(new RelativeNote
                    {
                        Position = _bmson.bpm_events[i].y - noteLine.Head,
                        Wav = (byte)_bmson.bpm_events[i].bpm,
                        Key = 0,
                    });
                }
            }

            // ノートが保持されていれば出力する。
            if (bpms.Count > 0)
            {
                SetLinesBpms(nowLineInfo, bpms);
            }
            if (exbpms.Count > 0)
            {
                SetLinesExbpms(nowLineInfo, exbpms);
            }
            _builder.AppendLine("");
        }

        /// <summary>
        /// note行
        /// </summary>
        /// <param name="nowLine"></param>
        /// <param name="notes"></param>
        private void SetLinesNotes(LineInfo nowLine, List<RelativeNote> notes)
        {
            //レーンごとに探索し、notesの部分リストを作成する。
            for(int i = 0; i < 17; i++)
            {
                var parts = GetPartialNotes(notes, i);
                if(parts.Count == 0)
                {
                    continue;
                }
                _builder.AppendLine($"#{nowLine.Num.ToString("D3")}{GetBmsLaneNum(i)}:{SetLineNotes(parts, nowLine.Interval)}");
            }
        }

        /// <summary>
        /// BPM行
        /// </summary>
        /// <param name="nowLine"></param>
        /// <param name="bpms"></param>
        private void SetLinesBpms(LineInfo nowLine, List<RelativeNote> bpms)
        {
            _builder.AppendLine($"#{nowLine.Num.ToString("D3")}03:{SetLineBpms(bpms, nowLine.Interval)}");
        }

        /// <summary>
        /// 拡張BPM行
        /// </summary>
        /// <param name="nowLine"></param>
        /// <param name="exbpms"></param>
        private void SetLinesExbpms(LineInfo nowLine, List<RelativeNote> exbpms)
        {
            _builder.AppendLine($"#{nowLine.Num.ToString("D3")}08:{SetLineNotes(exbpms, nowLine.Interval)}");
        }

        /// <summary>
        /// keyの番号のnoteのみを抜き出したリストを作成
        /// </summary>
        /// <param name="notes"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private List<RelativeNote> GetPartialNotes(List<RelativeNote> notes, int key)
        {
            List<RelativeNote> ret = new List<RelativeNote>();

            foreach(var note in notes)
            {
                if(note.Key == key)
                {
                    ret.Add(note);
                }
            }
            return ret;
        }

        // ノート
        private string SetLineNotes(List<RelativeNote> notes, ulong interval)
        {
            ulong gcd = GetNotesGcd(notes, interval);
            if (gcd == (ulong)_bmson.info.resolution * interval)
            {
                return RadixConvert.IntToZZ(notes[0].Wav);
            }


            string ret = "";
            int noteIndex = 0;
            var num = (ulong)_bmson.info.resolution * interval / gcd;
            for (ulong i = 0; i < num; i++)
            {
                // 各ノートを配置
                if (noteIndex < notes.Count)
                {
                    var note = notes[noteIndex];
                    var pos = note.Position / gcd;

                    if (pos == i)
                    {
                        ret += RadixConvert.IntToZZ(note.Wav);
                        noteIndex++;
                        continue;
                    }
                }

                ret += "00";
            }

            return ret;
        }

        // BPM(FFまで)
        private string SetLineBpms(List<RelativeNote> notes, ulong interval)
        {
            ulong gcd = GetNotesGcd(notes, interval);
            if (gcd == (ulong)_bmson.info.resolution * interval)
            {
                return $"{notes[0].Wav:X}";
            }


            string ret = "";
            int noteIndex = 0;
            var num = (ulong)_bmson.info.resolution * interval / gcd;
            for (ulong i = 0; i < num; i++)
            {
                // 各ノートを配置
                if (noteIndex < notes.Count)
                {
                    var note = notes[noteIndex];
                    var pos = note.Position / gcd;

                    if (pos == i)
                    {
                        ret += $"{note.Wav:X}";
                        noteIndex++;
                        continue;
                    }
                }

                ret += "00";
            }

            return ret;
        }

        private ulong GetNotesGcd(List<RelativeNote> notes, ulong interval)
        {
            ulong gcd = 0;
            foreach (var note in notes)
            {
                // note退避、各note位置(小節開始からの相対位置)の最大公約数を計算。(noteの情報は書き出し時に使う)。
                gcd = CalcurateEx.Gcd(gcd, note.Position);
            }
            return CalcurateEx.Gcd(gcd, (ulong)_bmson.info.resolution * interval);
        }

        /// <summary>
        /// #WAV行テキストを生成する
        /// </summary>
        /// <param name="num">#WAV番号</param>
        /// <param name="wavFileName">wavファイル名</param>
        /// <returns></returns>
        private string GetWavLineText(int num, string wavFileName)
        {
            return $"#WAV{RadixConvert.IntToZZ(num)} {wavFileName}" + Environment.NewLine;
        }

        /// <summary>
        /// #BPM行テキストを生成する
        /// </summary>
        /// <param name="num">#WAV番号</param>
        /// <param name="wavFileName">wavファイル名</param>
        /// <returns></returns>
        private string GetBpmLineText(int num, string wavFileName)
        {
            return $"#BPM{RadixConvert.IntToZZ(num)} {wavFileName}" + Environment.NewLine;
        }

        /// <summary>
        /// ノートのある小節を取得
        /// </summary>
        /// <param name="soundIndex"></param>
        /// <param name="noteIndex"></param>
        /// <returns></returns>
        private LineInfo GetNoteLine(ulong notePosition)
        {
            LineInfo lineInfo = new LineInfo();
            for (int i = 1; i < _bmson.lines!.Count(); i++)
            {
                if (notePosition < _bmson.lines![i].y)
                {
                    lineInfo.Num = i - 1;
                    lineInfo.Head = _bmson.lines[lineInfo.Num].y;
                    lineInfo.Interval = (_bmson.lines[i].y - lineInfo.Head) / (ulong)_bmson.info.resolution;
                    break;
                }
            }
            return lineInfo;
        }

        /// <summary>
        /// BMSONのレーン定義からBMSのレーン定義を取得する
        /// </summary>
        /// <param name="bmsonLane">BMSONのレーン定義</param>
        /// <returns>BMSのレーン定義</returns>
        private static string GetBmsLaneNum(int? bmsonLane)
        {
            switch (bmsonLane)
            {
                case 1: return "11";  // 1P 1
                case 2: return "12";  // 1P 2
                case 3: return "13";  // 1P 3
                case 4: return "14";  // 1P 4
                case 5: return "15";  // 1P 5
                case 6: return "18";  // 1P 6
                case 7: return "19";  // 1P 7
                case 8: return "16";  // 1P SC
                case 9: return "21";  // 2P 1
                case 10: return "22";  // 2P 2
                case 11: return "23";  // 2P 3
                case 12: return "24";  // 2P 4
                case 13: return "25";  // 2P 5
                case 14: return "28";  // 2P 6
                case 15: return "29";  // 2P 7
                case 16: return "26";  // 2P SC

                case 0:
                case null:
                default:
                    return "01";   // BGM
            }
        }
    }
}
