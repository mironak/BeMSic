using NAudio.Wave;

namespace BeMSic.Wave.WaveManipulator
{
    public class WaveIO
    {
        WaveFileReader _wfr;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="readWavFilePath"></param>
        public WaveIO(string readWavFilePath)
        {
            _wfr = new WaveFileReader(readWavFilePath);
        }

        /// <summary>
        /// wav出力
        /// </summary>
        /// <param name="wavFilePath"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        public void Write(string wavFilePath, long startPos, long endPos)
        {
            Wave.TrimWavFile(wavFilePath, _wfr, startPos, endPos);
        }

        /// <summary>
        /// サンプルサイズ取得
        /// </summary>
        /// <returns></returns>
        public long GetWaveSampleLength()
        {
            return _wfr.Length;
        }

        /// <summary>
        /// 1秒当たりのサンプル数を取得
        /// </summary>
        /// <returns></returns>
        public double GetSamplePerSeccond()
        {
            return _wfr.WaveFormat.SampleRate * (_wfr.WaveFormat.BitsPerSample / 8) * _wfr.WaveFormat.Channels;
        }
    }
}
