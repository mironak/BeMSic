using NAudio.Wave;

namespace BeMSic.Wave.WaveManipulator
{
    public static class Wave
    {
        /// <summary>
        /// Get wave stream
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static WaveStream? GetWaveStream(string fileName)
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
        /// <param name="reader"></param>
        /// <param name="writer"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        public static void TrimWavFile(WaveStream reader, WaveFileWriter writer, long startPos, long endPos)
        {
            reader.Position = startPos;
            var buffer = new byte[reader.WaveFormat.SampleRate * reader.WaveFormat.Channels];

            while (true)
            {
                int bytesRequired = (int)(endPos - reader.Position);
                int bytesToRead = Math.Min(bytesRequired, buffer.Length);
                int bytesRead = reader.Read(buffer, 0, ((bytesToRead - 1) / 4 + 1) * 4);
                if (bytesRead <= 0)
                {
                    // End data
                    break;
                }
                writer.Write(buffer, 0, bytesRead);
                if (endPos <= reader.Position)
                {
                    break;
                }
            }
        }
    }
}
