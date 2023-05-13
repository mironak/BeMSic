using NAudio.Wave;

namespace BeMSic.Wave.FileOperation
{
    /// <summary>
    /// 音声入出力
    /// </summary>
    internal static class WaveIO
    {
        /// <summary>
        /// Get wave stream
        /// </summary>
        /// <param name="fileName">読み込みファイル名</param>
        /// <returns>wavリーダー</returns>
        internal static WaveStream? GetWaveStream(string fileName)
        {
            string wavFilePath = Path.ChangeExtension(fileName, ".wav");
            if (File.Exists(wavFilePath))
            {
                return new WaveFileReader(wavFilePath);
            }

            string oggFilePath = Path.ChangeExtension(fileName, ".ogg");
            if (File.Exists(oggFilePath))
            {
                return new NAudio.Vorbis.VorbisWaveReader(oggFilePath);
            }

            return null;
        }

        /// <summary>
        /// Trim wav file
        /// </summary>
        /// <param name="wavFilePath">wavファイルパス</param>
        /// <param name="reader">wavリーダー</param>
        /// <param name="startPos">開始サンプル</param>
        /// <param name="endPos">終了サンプル</param>
        /// <param name="feedinSample">フェードインサンプル数</param>
        /// <param name="feedoutSample">フェードアウトサンプル数</param>
        internal static void TrimWavFile(string wavFilePath, WaveStream reader, long startPos, long endPos, int feedinSample, int feedoutSample)
        {
            using WaveFileWriter writeSr = new (
                wavFilePath,
                new WaveFormat(
                    reader.WaveFormat.SampleRate,
                    reader.WaveFormat.BitsPerSample,
                    reader.WaveFormat.Channels));
            reader.Position = startPos;

            int bytesRequired = (int)(endPos - reader.Position);
            int count = (((bytesRequired - 1) / reader.WaveFormat.BlockAlign) + 1) * reader.WaveFormat.BlockAlign;
            byte[] buffer = new byte[count];
            int bytesRead = reader.Read(buffer, 0, count);

            // feedin
            for (int i = 0; i < feedinSample; i++)
            {
                if (4 * i >= count)
                {
                    return;
                }

                short val1 = (short)(BitConverter.ToInt16(buffer, 4 * i) * i / feedinSample);
                short val2 = (short)(BitConverter.ToInt16(buffer, (4 * i) + 2) * i / feedinSample);
                byte[] bytes1 = BitConverter.GetBytes(val1);
                byte[] bytes2 = BitConverter.GetBytes(val2);

                byte[] newArray = new byte[bytes1.Length + bytes2.Length];
                bytes1.CopyTo(newArray, 0);
                bytes2.CopyTo(newArray, bytes1.Length);

                writeSr.Write(newArray, 0, 4);
            }

            if (bytesRead - (2 * 4 * feedinSample) < 0)
            {
                return;
            }

            writeSr.Write(buffer, 4 * feedinSample, bytesRead - (2 * 4 * feedinSample));

            // feedout
            for (int i = 0; i < feedoutSample; i++)
            {
                if (4 * i >= count)
                {
                    return;
                }

                short val1 = (short)(BitConverter.ToInt16(buffer, bytesRead + (4 * (i - feedoutSample))) * (feedoutSample - 1 - i) / feedoutSample);
                short val2 = (short)(BitConverter.ToInt16(buffer, bytesRead + (4 * (i - feedoutSample)) + 2) * (feedoutSample - 1 - i) / feedoutSample);
                byte[] bytes1 = BitConverter.GetBytes(val1);
                byte[] bytes2 = BitConverter.GetBytes(val2);

                byte[] newArray = new byte[bytes1.Length + bytes2.Length];
                bytes1.CopyTo(newArray, 0);
                bytes2.CopyTo(newArray, bytes1.Length);

                writeSr.Write(newArray, 0, 4);
            }
        }
    }
}
