using System.Text;
using BeMSic.Bmson.Type;
using BeMSic.Core.BmsDefinition;
using BeMSic.Core.Helpers;

namespace BeMSic.Bmson
{
    /// <summary>
    /// BMSファイルを生成
    /// </summary>
    internal class BmsBuilder
    {
        private readonly List<WavFileUnit> _wavs;           // #WAV
        private readonly int _soundIndex;
        private readonly BmsonFormat _bmson;
        private readonly StringBuilder _builder = new　();
        private List<double> _exbpms = new ();              // 拡張BPM
        private bool _isBgmOnly;
        private List<int> _notesWavIndices = new List<int>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="bmson">BMSON</param>
        /// <param name="soundIndex">音声インデックス</param>
        internal BmsBuilder(BmsonFormat bmson, int soundIndex)
        {
            if (bmson.sound_channels.Length <= soundIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(soundIndex), "Invalid index");
            }

            _bmson = bmson;
            _soundIndex = soundIndex;
            _wavs = new List<WavFileUnit>();
        }

        /// <summary>
        /// Append #WAV definition
        /// </summary>
        /// <param name="wav">#WAV番号</param>
        internal void AppendWav(WavFileUnit wav)
        {
            _wavs.Add(wav);
        }

        /// <summary>
        /// BMSテキストを生成する
        /// </summary>
        /// <param name="isBgmOnly">BGMレーンのみに並べる</param>
        /// <returns>BMSテキスト</returns>
        internal string Generate(bool isBgmOnly)
        {
            _isBgmOnly = isBgmOnly;

            SetBmsHeaderField();
            SetWavField();
            SetBpmField();

            SetMainDataField();

            return _builder.ToString();
        }

        /// <summary>
        /// BMSONのレーン定義からBMSのレーン定義を取得する
        /// </summary>
        /// <param name="bmsonLane">BMSONのレーン定義</param>
        /// <returns>BMSのレーン定義</returns>
        private static string GetBmsLaneNum(int? bmsonLane, bool isLongnote)
        {
            return bmsonLane switch
            {
                1 => isLongnote ? "51" : "11", // 1P 1
                2 => isLongnote ? "52" : "12", // 1P 2
                3 => isLongnote ? "53" : "13", // 1P 3
                4 => isLongnote ? "54" : "14", // 1P 4
                5 => isLongnote ? "55" : "15", // 1P 5
                6 => isLongnote ? "58" : "18", // 1P 6
                7 => isLongnote ? "59" : "19", // 1P 7
                8 => isLongnote ? "56" : "16", // 1P SC
                9 => isLongnote ? "61" : "21", // 2P 1
                10 => isLongnote ? "62" : "22", // 2P 2
                11 => isLongnote ? "63" : "23", // 2P 3
                12 => isLongnote ? "64" : "24", // 2P 4
                13 => isLongnote ? "65" : "25", // 2P 5
                14 => isLongnote ? "68" : "28", // 2P 6
                15 => isLongnote ? "69" : "29", // 2P 7
                16 => isLongnote ? "66" : "26", // 2P SC
                _ => "01", // BGM
            };
        }

        /// <summary>
        /// #WAV行テキストを生成する
        /// </summary>
        /// <param name="num">#WAV番号</param>
        /// <param name="wavFileName">wavファイル名</param>
        /// <returns>#WAV行</returns>
        private static string GetWavLineText(int num, string wavFileName)
        {
            return $"#WAV{RadixConvert.IntToZZ(num)} {wavFileName}" + Environment.NewLine;
        }

        /// <summary>
        /// #BPM行テキストを生成する
        /// </summary>
        /// <param name="num">#WAV番号</param>
        /// <param name="wavFileName">wavファイル名</param>
        /// <returns>#BPM行</returns>
        private static string GetBpmLineText(int num, string wavFileName)
        {
            return $"#BPM{RadixConvert.IntToZZ(num)} {wavFileName}" + Environment.NewLine;
        }

        /// <summary>
        /// keyの番号のnoteのみを抜き出したリストを作成
        /// </summary>
        /// <param name="notes">ノート一覧</param>
        /// <param name="key">キー</param>
        /// <returns>keyのノート一覧</returns>
        private static List<RelativeNote> GetPartialNotes(List<RelativeNote> notes, int key)
        {
            List<RelativeNote> ret = new ();

            foreach (RelativeNote note in notes)
            {
                if (note.Note.x == key)
                {
                    ret.Add(note);
                }
            }

            return ret;
        }

        /// <summary>
        /// keyの番号のnoteのみを抜き出したリストを作成(ロングノートなし, ロングノートあり)
        /// </summary>
        /// <param name="notes">ノート一覧</param>
        /// <param name="key">キー</param>
        /// <returns>keyのノート一覧(ロングノートなし, ロングノートあり)</returns>
        private static (List<RelativeNote>, List<RelativeNote>) GetPartialNotesLongNote(List<RelativeNote> notes, int key)
        {
            List<RelativeNote> noLn = new ();
            List<RelativeNote> ln = new ();

            foreach (RelativeNote note in notes)
            {
                if (note.Note.x != key)
                {
                    continue;
                }

                if (note.Note.l == 0)
                {
                    noLn.Add(note);
                }
                else
                {
                    ln.Add(note);
                }
            }

            return (noLn, ln);
        }

        /// <summary>
        /// Set bms header field
        /// </summary>
        private void SetBmsHeaderField()
        {
            _builder.AppendLine("*---------------------- HEADER FIELD");
            _builder.AppendLine(string.Empty);

            string genre = string.Empty;
            if (_bmson.info.genre != null)
            {
                // genre = _bmson.info.genre;
            }

            _builder.AppendLine($"#GENRE {genre}");

            string title = string.Empty;
            if (_bmson.info.title != null)
            {
                // title = _bmson.info.title;
            }

            _builder.AppendLine($"#TITLE {title}");

            string artist = string.Empty;
            if (_bmson.info.artist != null)
            {
                // artist = _bmson.info.artist;
            }

            _builder.AppendLine($"#ARTIST {artist}");

            _builder.AppendLine($"#BPM {_bmson.info.init_bpm}");
            _builder.AppendLine($"#PLAYLEVEL {_bmson.info.level}");
            _builder.AppendLine(string.Empty);
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

            _builder.AppendLine(string.Empty);
        }

        /// <summary>
        /// Set #BPM field
        /// </summary>
        private void SetBpmField()
        {
            _exbpms = GetExBpms();

            // #BPMを追加
            for (int i = 0; i < _exbpms.Count; i++)
            {
                _builder.Append(GetBpmLineText(i + 1, _exbpms[i].ToString()));
            }

            _builder.AppendLine(string.Empty);
        }

        /// <summary>
        /// #BPM一覧を取得
        /// </summary>
        /// <returns>#BPM一覧</returns>
        private List<double> GetExBpms()
        {
            if (_bmson.bpm_events == null)
            {
                return new List<double>();
            }

            HashSet<double> exbpms = new ();

            // bpm_eventsを探索し、0-255以外をまとめる(#BPM用)
            for (int i = 0; i < _bmson.bpm_events.Length; i++)
            {
                if (_bmson.bpm_events[i].bpm != (byte)_bmson.bpm_events[i].bpm)
                {
                    exbpms.Add(_bmson.bpm_events[i].bpm);
                }
            }

            return exbpms.ToList();
        }

        /// <summary>
        /// Set Main data field
        /// </summary>
        private void SetMainDataField()
        {
            _builder.AppendLine("*----------------------MAIN DATA FIELD");
            _builder.AppendLine(string.Empty);

            AppendLines();
        }

        /// <summary>
        /// MAIN行を追加
        /// </summary>
        private void AppendLines()
        {
            if (_bmson.lines == null)
            {
                return;
            }

            // ロングノートを配置する場合、終端ノートをここで配置しておく
            SetLongNotesLast();

            int lineIndex = 0;
            ulong lineHead = 0;
            foreach (Line line in _bmson.lines)
            {
                ulong interval = (line.y - lineHead) / (ulong)_bmson.info.resolution;
                if (interval == 0)
                {
                    continue;
                }

                LineInfo nowLineInfo = new () { Num = lineIndex, Interval = interval, Head = lineHead };

                AppendBar(nowLineInfo);
                AppendNotes(nowLineInfo);
                AppendBpms(nowLineInfo);

                lineHead = line.y;
                lineIndex++;
            }
        }

        /// <summary>
        /// ロングノート(LNTYPE 1)の終端ノートを作成する。
        /// </summary>
        private void SetLongNotesLast()
        {
            _notesWavIndices.Clear();

            int wavIndex = 1;
            List<Note> notes = new List<Note>();

            foreach (var note in _bmson.sound_channels[_soundIndex].notes)
            {
                notes.Add(note);
                _notesWavIndices.Add(wavIndex);

                if (note.l > 0)
                {
                    notes.Add(new Note { c = true, l = note.l, x = note.x, y = note.y + note.l });
                    _notesWavIndices.Add(wavIndex);
                }

                wavIndex++;
            }

            _bmson.sound_channels[_soundIndex].notes = notes.ToArray();
        }

        /// <summary>
        /// 小節長さ(4/4拍子は出力しない)
        /// </summary>
        /// <param name="nowLineInfo">現在の小節線情報</param>
        private void AppendBar(LineInfo nowLineInfo)
        {
            if (nowLineInfo.Interval != 0 && nowLineInfo.Interval != 4)
            {
                _builder.AppendLine($"#{nowLineInfo.Num.ToString("D3")}02:{nowLineInfo.Interval / 2d}");
            }
        }

        /// <summary>
        /// ノート
        /// </summary>
        /// <param name="nowLineInfo">現在の小節線情報</param>
        private void AppendNotes(LineInfo nowLineInfo)
        {
            List<RelativeNote> notes = new ();

            for (int i = 0; i < _bmson.sound_channels[_soundIndex].notes.Length; i++)
            {
                // ノートの小節番号を取得する
                LineInfo noteLine = GetNoteLine(_bmson.sound_channels[_soundIndex].notes[i].y);
                if (noteLine.Num != nowLineInfo.Num)
                {
                    continue;
                }

                // 前のノートと同じ小節の時、一時領域に追加
                notes.Add(new RelativeNote
                {
                    Position = _bmson.sound_channels[_soundIndex].notes[i].y - noteLine.Head,
                    Wav = _notesWavIndices[i],
                    Note = _bmson.sound_channels[_soundIndex].notes[i],
                });
            }

            // ノートが保持されていれば出力する。
            if (notes.Count > 0)
            {
                // 前のノートから小節が変わった場合、noteの位置を計算し、割り当てる。
               // SetLinesNotes(nowLineInfo, notes);
                SetLinesNotesLongnotes(nowLineInfo, notes);
                _builder.AppendLine(string.Empty);
            }
        }

        /// <summary>
        /// BPM
        /// </summary>
        /// <param name="nowLineInfo">現在の小節線情報</param>
        private void AppendBpms(LineInfo nowLineInfo)
        {
            if (_bmson.bpm_events == null)
            {
                return;
            }

            List<RelativeNote> bpms = new ();
            List<RelativeNote> exbpms = new ();

            for (int i = 0; i < _bmson.bpm_events.Length; i++)
            {
                // ノートの小節番号を取得する
                LineInfo noteLine = GetNoteLine(_bmson.bpm_events[i].y);
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
                        Note = new Note(),
                    });
                }
                else
                {
                    bpms.Add(new RelativeNote
                    {
                        Position = _bmson.bpm_events[i].y - noteLine.Head,
                        Wav = (byte)_bmson.bpm_events[i].bpm,
                        Note = new Note(),
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

            _builder.AppendLine(string.Empty);
        }

        /// <summary>
        /// note行
        /// </summary>
        /// <param name="nowLineInfo">現在の小節線情報</param>
        /// <param name="notes">ノート一覧</param>
        private void SetLinesNotes(LineInfo nowLineInfo, List<RelativeNote> notes)
        {
            if (!_isBgmOnly)
            {
                // レーンごとに探索し、notesの部分リストを作成する。
                for (int i = 0; i < 17; i++)
                {
                    List<RelativeNote> parts = GetPartialNotes(notes, i);
                    if (parts.Count == 0)
                    {
                        continue;
                    }

                    SetLinesNote(nowLineInfo, parts, GetBmsLaneNum(i, false));
                }
            }
            else
            {
                SetLinesNote(nowLineInfo, notes, GetBmsLaneNum(0, false));
            }
        }

        /// <summary>
        /// note行
        /// </summary>
        /// <param name="nowLineInfo">現在の小節線情報</param>
        /// <param name="notes">ノート一覧</param>
        private void SetLinesNotesLongnotes(LineInfo nowLineInfo, List<RelativeNote> notes)
        {
            if (!_isBgmOnly)
            {
                // レーンごとに探索し、notesの部分リストを作成する。
                for (int i = 0; i < 17; i++)
                {
                    (var noLns, var lns) = GetPartialNotesLongNote(notes, i);
                    if (noLns.Count > 0)
                    {
                        SetLinesNote(nowLineInfo, noLns, GetBmsLaneNum(i, false));
                    }

                    if (lns.Count > 0)
                    {
                        SetLinesNote(nowLineInfo, lns, GetBmsLaneNum(i, true));
                    }
                }
            }
            else
            {
                SetLinesNote(nowLineInfo, notes, GetBmsLaneNum(0, false));
            }
        }

        /// <summary>
        /// note行1行
        /// </summary>
        /// <param name="nowLineInfo">現在の小節線情報</param>
        /// <param name="notes">ノート一覧</param>
        private void SetLinesNote(LineInfo nowLineInfo, List<RelativeNote> notes, string lane)
        {
            _builder.AppendLine($"#{nowLineInfo.Num.ToString("D3")}{lane}:{SetLineNotes(notes, nowLineInfo.Interval)}");
        }

        /// <summary>
        /// BPM行
        /// </summary>
        /// <param name="nowLineInfo">現在の小節線情報</param>
        /// <param name="bpms">BPM一覧(0-255)</param>
        private void SetLinesBpms(LineInfo nowLineInfo, List<RelativeNote> bpms)
        {
            _builder.AppendLine($"#{nowLineInfo.Num.ToString("D3")}03:{SetLineBpms(bpms, nowLineInfo.Interval)}");
        }

        /// <summary>
        /// 拡張BPM行
        /// </summary>
        /// <param name="nowLineInfo">現在の小節線情報</param>
        /// <param name="exbpms">拡張BPM一覧</param>
        private void SetLinesExbpms(LineInfo nowLineInfo, List<RelativeNote> exbpms)
        {
            _builder.AppendLine($"#{nowLineInfo.Num.ToString("D3")}08:{SetLineNotes(exbpms, nowLineInfo.Interval)}");
        }

        /// <summary>
        /// ノート
        /// </summary>
        /// <param name="notes">ノート一覧</param>
        /// <param name="interval">小節長さ</param>
        /// <returns>定義行</returns>
        private string SetLineNotes(List<RelativeNote> notes, ulong interval)
        {
            ulong gcd = GetNotesGcd(notes, interval);
            if (gcd == (ulong)_bmson.info.resolution * interval)
            {
                return RadixConvert.IntToZZ(notes[0].Wav);
            }

            string ret = string.Empty;
            int noteIndex = 0;
            ulong num = (ulong)_bmson.info.resolution * interval / gcd;
            for (ulong i = 0; i < num; i++)
            {
                // 各ノートを配置
                if (noteIndex < notes.Count)
                {
                    RelativeNote note = notes[noteIndex];
                    ulong pos = note.Position / gcd;

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

        /// <summary>
        /// BPM(FFまで)
        /// </summary>
        /// <param name="notes">ノート一覧</param>
        /// <param name="interval">小節長さ</param>
        /// <returns>BPM定義行</returns>
        private string SetLineBpms(List<RelativeNote> notes, ulong interval)
        {
            ulong gcd = GetNotesGcd(notes, interval);
            if (gcd == (ulong)_bmson.info.resolution * interval)
            {
                return $"{notes[0].Wav:X}";
            }

            string ret = string.Empty;
            int noteIndex = 0;
            ulong num = (ulong)_bmson.info.resolution * interval / gcd;
            for (ulong i = 0; i < num; i++)
            {
                // 各ノートを配置
                if (noteIndex < notes.Count)
                {
                    RelativeNote note = notes[noteIndex];
                    ulong pos = note.Position / gcd;

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

        /// <summary>
        /// ノート一覧の位置の最小公倍数を取得
        /// </summary>
        /// <param name="notes">ノート一覧</param>
        /// <param name="interval">小節長さ</param>
        /// <returns>ノート一覧の位置の最小公倍数</returns>
        private ulong GetNotesGcd(List<RelativeNote> notes, ulong interval)
        {
            ulong gcd = 0;
            foreach (RelativeNote note in notes)
            {
                // note退避、各note位置(小節開始からの相対位置)の最大公約数を計算。(noteの情報は書き出し時に使う)。
                gcd = CalcurateEx.Gcd(gcd, note.Position);
            }

            return CalcurateEx.Gcd(gcd, (ulong)_bmson.info.resolution * interval);
        }

        /// <summary>
        /// ノートのある小節を取得
        /// </summary>
        /// <param name="notePosition">ノート位置</param>
        /// <returns>小節情報</returns>
        private LineInfo GetNoteLine(ulong notePosition)
        {
            LineInfo lineInfo = new ();
            for (int i = 1; i < _bmson.lines!.Length; i++)
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
        /// 小節線情報
        /// </summary>
        private class LineInfo
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
        private class RelativeNote
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
            public Note Note { get; set; }
        }
    }
}
