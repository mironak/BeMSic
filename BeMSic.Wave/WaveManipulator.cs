﻿using NAudio.Wave;

namespace BeMSic.Wave
{
    /// <summary>
    /// 音声操作
    /// </summary>
    public class WaveManipulator
    {
        private WaveFileReader _wfr;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="readWavFilePath">wavファイルパス</param>
        public WaveManipulator(string readWavFilePath)
        {
            _wfr = new WaveFileReader(readWavFilePath);
        }

        /// <summary>
        /// wav出力
        /// </summary>
        /// <param name="wavFilePath">wavファイルパス</param>
        /// <param name="startPos">開始サンプル</param>
        /// <param name="endPos">終了サンプル</param>
        public void Trim(string wavFilePath, long startPos, long endPos)
        {
            FileOperation.WaveIO.TrimWavFile(wavFilePath, _wfr, startPos, endPos);
        }

        /// <summary>
        /// サンプルサイズ取得
        /// </summary>
        /// <returns>サンプルサイズ</returns>
        public long GetWaveSampleLength()
        {
            return _wfr.Length;
        }

        /// <summary>
        /// 1秒当たりのサンプル数を取得
        /// </summary>
        /// <returns>1秒当たりのサンプル数</returns>
        public double GetSamplePerSeccond()
        {
            return _wfr.WaveFormat.SampleRate * (_wfr.WaveFormat.BitsPerSample / 8) * _wfr.WaveFormat.Channels;
        }
    }
}
