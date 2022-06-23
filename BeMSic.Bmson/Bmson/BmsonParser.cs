using BeMSic.Core.BmsDefinition;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace BeMSic.Bmson
{
    public class BmsonMain
    {
        /// <summary>
        /// BMSON text
        /// </summary>
        private Bmson? _bmson;
        public Bmson Bmson { get { return _bmson!; }}

        double _coef;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bmson">BMSONテキスト</param>
        public BmsonMain(string bmson)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };
            _bmson = JsonSerializer.Deserialize<Bmson>(bmson, options);
        }

        /// <summary>
        /// WAVファイルを切断し、切断後のWAVを書き込んだBMSテキストを返す
        /// </summary>
        /// <param name="saveDirectory">保存先ディレクトリ</param>
        /// <param name="readWavFilePath">切断対象WAVファイルパス</param>
        /// <param name="chIndex">切断対象WAVインデックス</param>
        /// <returns>BMSテキスト</returns>
        public string CutWav(string saveDirectory, string readWavFilePath, int chIndex)
        {
            var waveIO = new Wave.WaveManipulator.WaveIO(readWavFilePath);

            _coef = CalculateCoefficient(_bmson!.info.resolution, waveIO.GetSamplePerSeccond());

            BmsBuilder bmsBuilder = new BmsBuilder(_bmson!, chIndex);

            long sampleStart = (long)((double)_bmson!.sound_channels[chIndex].notes[0].y * _coef / _bmson.info.init_bpm);
            for (int i = 0; i < _bmson!.sound_channels[chIndex].notes.Length; i++)
            {
                long sampleEnd = Math.Min(GetSampleEnd(sampleStart, chIndex, i), waveIO.GetWaveSampleLength());

                string wavFilePath = GetSaveFilePath(saveDirectory, _bmson.sound_channels[chIndex].name, sampleStart, sampleEnd);
                waveIO.Write(wavFilePath, sampleStart, sampleEnd);
                bmsBuilder.AppendWav(new WavFileUnit(i, Path.GetFileName(wavFilePath)));
                sampleStart = sampleEnd;
            }

            return bmsBuilder.Generate();
        }

        /// <summary>
        /// 係数を計算する
        /// </summary>
        /// <param name="bmsonResolution">BMSON解像度</param>
        /// <param name="samplePerSeccond"></param>
        private double CalculateCoefficient(int bmsonResolution, double samplePerSeccond)
        {
            return 240.0 / (bmsonResolution * 4) * samplePerSeccond;
        }

        /// <summary>
        /// サンプルの終了位置を取得
        /// </summary>
        /// <param name="chIndex"></param>
        /// <param name="noteIndex"></param>
        /// <param name="lastSample"></param>
        /// <returns></returns>
        private long GetSampleEnd(long sampleStart, int chIndex, int index)
        {
            if (_bmson!.sound_channels[chIndex].notes.Length <= (index + 1))
            {
                return long.MaxValue;
            }
            // sampleStartから次のノートまでの間のbpm変化を取得
            var bpmChanges = new List<BpmEvents>();
            var startBpm = _bmson.info.init_bpm;
            bool startFlag = true;
            var startPosition = _bmson.sound_channels[chIndex].notes[index].y;
            var endPosition = _bmson.sound_channels[chIndex].notes[index+1].y;
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

            // sampleStartからbpm変化ごとのサンプル数を足し合わせる
            var sampleEnd = sampleStart;
            var prevSample = startPosition;
            var nowBpm = startBpm;
            foreach (var bpm in bpmChanges)
            {
                var length = bpm.y - prevSample;
                sampleEnd += (long)((double)length * _coef / nowBpm);

                prevSample = bpm.y;
                nowBpm = bpm.bpm;
            }
            var lastLength = endPosition - prevSample;
            sampleEnd += (long)((double)lastLength * _coef / nowBpm);

            return sampleEnd;
        }

        /// <summary>
        /// 保存するWAVファイル名を作成する
        /// </summary>
        /// <param name="readFilePath"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private string GetSaveFilePath(string saveDirectory, string fileName, long start, long end)
        {
            return Path.ChangeExtension(
                saveDirectory + "\\" +
                Path.GetFileNameWithoutExtension(fileName) + $"_{start}_{end}",
                ".wav");
        }
    }
}
