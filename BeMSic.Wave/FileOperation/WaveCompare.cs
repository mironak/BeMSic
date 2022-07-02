using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace BeMSic.Wave.FileOperation
{
    /// <summary>
    /// 音声比較
    /// </summary>
    public static class WaveCompare
    {
        /// <summary>
        /// Delegate for compare 2 .wav files
        /// </summary>
        /// <param name="wav1">wav data 1</param>
        /// <param name="wav2">wav data 2</param>
        /// <returns>一致度</returns>
        public delegate float ValidComparator(float[] wav1, float[] wav2);

        /// <summary>
        /// 2つのwavの一致度がr2val以上であればtrue
        /// </summary>
        /// <param name="reader1">reader1</param>
        /// <param name="reader2">reader2</param>
        /// <param name="isSameLength">同じ長さのみ比較する場合はtrue</param>
        /// <param name="r2val">一致度</param>
        /// <param name="comparator">評価関数</param>
        /// <returns>If 2 wav datas are match, return true</returns>
        public static bool IsMatch(WaveStream reader1, WaveStream reader2, bool isSameLength, float r2val, ValidComparator comparator)
        {
            if (!IsSameSettings(reader1, reader2))
            {
                return false;
            }

            if (isSameLength && (reader1.Length != reader2.Length))
            {
                return false;
            }

            return CalculateAllMatchRate(reader1, reader2, r2val, comparator);
        }

        /// <summary>
        /// 一致度がr2val以上ならtrue
        /// </summary>
        /// <param name="reader1">reader1</param>
        /// <param name="reader2">reader2</param>
        /// <param name="r2val">一致度</param>
        /// <param name="comparator">評価関数</param>
        /// <returns>データ終了ならtrue</returns>
        private static bool CalculateAllMatchRate(WaveStream reader1, WaveStream reader2, float r2val, ValidComparator comparator)
        {
            reader1.Position = 0;
            reader2.Position = 0;

            long length = Math.Max(reader1.Length, reader2.Length);

            SampleChannel sampleChannel1 = new (reader1, false);
            SampleChannel sampleChannel2 = new (reader2, false);
            float[] readBufferA = new float[length];
            float[] readBufferB = new float[length];

            int bufferAResidual = sampleChannel1.Read(readBufferA, 0, readBufferA.Length);
            int bufferBResidual = sampleChannel2.Read(readBufferB, 0, readBufferB.Length);

            if (bufferAResidual <= 0 && bufferBResidual <= 0)
            {
                // End data
                return true;
            }

            if (CalculateMatchRate(readBufferA, readBufferB, reader1.WaveFormat.Channels, comparator) >= r2val)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determine if the settings of the 2 wav files are the same.
        /// </summary>
        /// <param name="r1">.wav file 1</param>
        /// <param name="r2">.wav file 2</param>
        /// <returns>If the settings of the 2 wav files are same, return true</returns>
        private static bool IsSameSettings(WaveStream r1, WaveStream r2)
        {
            if (r1.WaveFormat.SampleRate != r2.WaveFormat.SampleRate)
            {
                return false;
            }

            if (r1.WaveFormat.BitsPerSample != r2.WaveFormat.BitsPerSample)
            {
                return false;
            }

            if (r1.WaveFormat.Channels != r2.WaveFormat.Channels)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Calculate match rate of 2 wav datas
        /// </summary>
        /// <param name="wav1">wav data 1</param>
        /// <param name="wav2">wav data 2</param>
        /// <param name="channelNum">Channel number of wav</param>
        /// <param name="comparator">Evaluation function</param>
        /// <returns>Match Rate</returns>
        private static float CalculateMatchRate(float[] wav1, float[] wav2, int channelNum, ValidComparator comparator)
        {
            int length = Math.Min(wav1.Length, wav2.Length) / channelNum;
            float[] wav1Ch = new float[length];
            float[] wav2Ch = new float[length];
            float minimumMatchRate = 1.0F;

            for (int i = 0; i < channelNum; i++)
            {
                for (int j = i; j < length; j += channelNum)
                {
                    wav1Ch[j] = wav1[j];
                    wav2Ch[j] = wav2[j];
                }

                float matchRate = comparator(wav1Ch, wav2Ch);
                if (matchRate < minimumMatchRate)
                {
                    minimumMatchRate = matchRate;
                }
            }

            return minimumMatchRate;
        }
    }
}
