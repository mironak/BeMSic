using System.Text.Encodings.Web;
using System.Text.Json;
using BeMSic.Bmson.Type;
using BeMSic.Core.BmsDefinition;

namespace BeMSic.Bmson
{
    /// <summary>
    /// BMSON操作
    /// </summary>
    public class BmsonParser
    {
        /// <summary>
        /// BMSON text
        /// </summary>
        private readonly BmsonFormat? _bmson;
        private double _coef;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bmson">BMSONテキスト</param>
        public BmsonParser(string bmson)
        {
            JsonSerializerOptions options = new ()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            };

            _bmson = JsonSerializer.Deserialize<BmsonFormat>(bmson, options);
        }

        /// <summary>
        /// BMSON
        /// </summary>
        public BmsonFormat Bmson
        {
            get { return _bmson!; }
        }

        /// <summary>
        /// WAVファイルを切断し、切断後のWAVを書き込んだBMSテキストを返す
        /// </summary>
        /// <param name="saveDirectory">保存先ディレクトリ</param>
        /// <param name="readWavFilePath">切断対象WAVファイルパス</param>
        /// <param name="chIndex">切断対象WAVインデックス</param>
        /// <returns>BMSテキスト</returns>
        public string CutWav(string saveDirectory, string readWavFilePath, int chIndex, bool isBgmOnly)
        {
            Wave.WaveManipulator waveIO = new (readWavFilePath);

            _coef = CalculateCoefficient(_bmson!.info.resolution, waveIO.GetSamplePerSeccond());

            BmsBuilder bmsBuilder = new (_bmson!, chIndex);

            if (_bmson.sound_channels[chIndex].notes != null)
            {
                long sampleStart = (long)(_bmson!.sound_channels[chIndex].notes[0].y * _coef / _bmson.info.init_bpm);
                long prevContinuous = 0;

                for (int i = 0; i < _bmson!.sound_channels[chIndex].notes.Length; i++)
                {
                    if (!_bmson.sound_channels[chIndex].notes[i].c)
                    {
                        prevContinuous = sampleStart;
                    }

                    long sampleNext = Math.Min(GetSampleEnd(sampleStart, chIndex, i), waveIO.GetWaveSampleLength() + prevContinuous);

                    string wavFilePath = GetSaveFilePath(
                        saveDirectory,
                        _bmson.sound_channels[chIndex].name,
                        sampleStart,
                        sampleNext);
                    waveIO.Trim(wavFilePath, sampleStart - prevContinuous, sampleNext - prevContinuous);
                    bmsBuilder.AppendWav(new WavFileUnit(i, Path.GetFileName(wavFilePath)));
                    sampleStart = sampleNext;
                }
            }

            return bmsBuilder.Generate(isBgmOnly);
        }

        /// <summary>
        /// 係数を計算する
        /// </summary>
        /// <param name="bmsonResolution">BMSON解像度</param>
        /// <param name="samplePerSeccond">1秒あたりのサンプル数</param>
        private static double CalculateCoefficient(int bmsonResolution, double samplePerSeccond)
        {
            return 240.0 / (bmsonResolution * 4) * samplePerSeccond;
        }

        /// <summary>
        /// 保存するWAVファイル名を作成する
        /// </summary>
        /// <param name="saveDirectory">保存先ディレクトリ</param>
        /// <param name="fileName">ファイル名</param>
        /// <param name="start">サンプル開始位置</param>
        /// <param name="end">サンプル終了位置</param>
        /// <returns>保存wavファイルパス</returns>
        private static string GetSaveFilePath(string saveDirectory, string fileName, long start, long end)
        {
            return Path.ChangeExtension(
                saveDirectory + "\\" +
                Path.GetFileNameWithoutExtension(fileName) + $"_{start}_{end}",
                ".wav");
        }

        /// <summary>
        /// サンプルの終了位置を取得
        /// </summary>
        /// <param name="sampleStart">サンプル開始位置</param>
        /// <param name="chIndex">音声インデックス</param>
        /// <param name="index">ノートインデックス</param>
        /// <returns>サンプル終了位置</returns>
        private long GetSampleEnd(long sampleStart, int chIndex, int index)
        {
            if (_bmson!.sound_channels[chIndex].notes.Length <= (index + 1))
            {
                return long.MaxValue;
            }

            // sampleStartから次のノートまでの間のbpm変化を取得
            (List<BpmEvents> bpmChanges, double startBpm) = GetBpmChanges(chIndex, index);

            // sampleStartからbpm変化ごとのサンプル数を足し合わせる
            long sampleEnd = sampleStart;
            ulong prevSample = _bmson.sound_channels[chIndex].notes[index].y;
            double nowBpm = startBpm;

            foreach (BpmEvents bpm in bpmChanges)
            {
                ulong length = bpm.y - prevSample;
                sampleEnd += (long)(length * _coef / nowBpm);

                prevSample = bpm.y;
                nowBpm = bpm.bpm;
            }

            ulong lastLength = _bmson.sound_channels[chIndex].notes[index + 1].y - prevSample;
            sampleEnd += (long)(lastLength * _coef / nowBpm);

            return sampleEnd;
        }

        /// <summary>
        /// BPM変更一覧取得
        /// </summary>
        /// <param name="chIndex">音声インデックス</param>
        /// <param name="index">ノートインデックス</param>
        /// <returns>BPM変更一覧</returns>
        private (List<BpmEvents>, double) GetBpmChanges(int chIndex, int index)
        {
            List<BpmEvents> bpmChanges = new ();
            double startBpm = _bmson!.info.init_bpm;
            bool startFlag = true;
            ulong startPosition = _bmson.sound_channels[chIndex].notes[index].y;
            ulong endPosition = _bmson.sound_channels[chIndex].notes[index + 1].y;

            if (_bmson.bpm_events != null)
            {
                for (int i = 0; i < _bmson.bpm_events.Length; i++)
                {
                    if (startFlag)
                    {
                        startBpm = _bmson.bpm_events[i].bpm;
                    }

                    BpmEvents? bpm = _bmson.bpm_events[i];
                    if ((bpm.y >= startPosition) && (bpm.y < endPosition))
                    {
                        bpmChanges.Add(bpm);
                        startFlag = false;
                    }
                }
            }

            return (bpmChanges, startBpm);
        }
    }
}
