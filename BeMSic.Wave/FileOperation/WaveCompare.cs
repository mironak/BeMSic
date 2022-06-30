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
        /// Match judgment of 2 wavs
        /// </summary>
        /// <param name="reader1">reader1</param>
        /// <param name="reader2">reader2</param>
        /// <param name="r2val">一致度</param>
        /// <param name="comparator">評価関数</param>
        /// <returns>If 2 wav datas are match, return true</returns>
        public static bool IsMatch(WaveStream reader1, WaveStream reader2, float r2val, ValidComparator comparator)
        {
            if (!IsSameSetting(reader1, reader2))
            {
                return false;
            }

            return CalculateAllMatchRate(reader1, reader2, r2val, comparator);
        }

        /// <summary>
        /// Calculate all match rate
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

            SampleChannel sampleChannel1 = new (reader1, false);
            SampleChannel sampleChannel2 = new (reader2, false);
            float[] readBufferA = new float[reader1.WaveFormat.SampleRate * reader1.WaveFormat.Channels];
            float[] readBufferB = new float[reader2.WaveFormat.SampleRate * reader2.WaveFormat.Channels];

            while (true)
            {
                int bufferAResidual = sampleChannel1.Read(readBufferA, 0, readBufferA.Length);
                int bufferBResidual = sampleChannel2.Read(readBufferB, 0, readBufferB.Length);

                if (bufferAResidual <= 0 && bufferBResidual <= 0)
                {
                    // End data
                    return true;
                }

                if (CalculateMatchRate(readBufferA, readBufferB, reader1.WaveFormat.Channels, comparator) < r2val)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Determine if the settings of the 2 wav files are the same.
        /// </summary>
        /// <param name="r1">.wav file 1</param>
        /// <param name="r2">.wav file 2</param>
        /// <returns>If the settings of the 2 wav files are same, return true</returns>
        private static bool IsSameSetting(WaveStream r1, WaveStream r2)
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
            float[] wav1Ch = new float[wav1.Length / channelNum];
            float[] wav2Ch = new float[wav2.Length / channelNum];
            float minimumMatchRate = 1.0F;

            for (int i = 0; i < channelNum; i++)
            {
                for (int j = i; j < wav1.Length; j += channelNum)
                {
                    wav1Ch[j / channelNum] = wav1[j];
                    wav2Ch[j / channelNum] = wav2[j];
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
