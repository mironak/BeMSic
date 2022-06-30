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
        internal static void TrimWavFile(string wavFilePath, WaveStream reader, long startPos, long endPos)
        {
            using WaveFileWriter writeSr = new (
                wavFilePath,
                new WaveFormat(
                    reader.WaveFormat.SampleRate,
                    reader.WaveFormat.BitsPerSample,
                    reader.WaveFormat.Channels));
            reader.Position = startPos;
            byte[] buffer = new byte[reader.WaveFormat.SampleRate * reader.WaveFormat.Channels];

            while (true)
            {
                int bytesRequired = (int)(endPos - reader.Position);
                int bytesToRead = Math.Min(bytesRequired, buffer.Length);
                int bytesRead = reader.Read(buffer, 0, (((bytesToRead - 1) / 4) + 1) * 4);
                if (bytesRead <= 0)
                {
                    // End data
                    break;
                }

                writeSr.Write(buffer, 0, bytesRead);
                if (endPos <= reader.Position)
                {
                    break;
                }
            }
        }
    }
}
