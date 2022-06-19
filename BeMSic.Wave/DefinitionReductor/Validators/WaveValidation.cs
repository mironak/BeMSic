namespace BeMSic.Wave.DefinitionReductor.Validators
{
    static public class WaveValidation
    {
        /// <summary>
        /// Calculate coefficient of determination
        /// https://en.wikipedia.org/wiki/Coefficient_of_determination
        /// </summary>
        /// <param name="wav1">wav data 1</param>
        /// <param name="wav2">wav data 2</param>
        /// <returns>Match Rate</returns>
        static public float CalculateRSquared(float[] wav1, float[] wav2)
        {
            float wav1Average = wav1.Average();

            float rss = 0;
            float dss = 0;
            for (int i = 0; i < wav1.Length; i++)
            {
                float wav1RssTemp = wav1[i] - wav2[i];
                float wav2DssTemp = wav1[i] - wav1Average;
                rss += wav1RssTemp * wav1RssTemp;
                dss += wav2DssTemp * wav2DssTemp;
            }
            return 1.0F - (rss / dss);
        }
    }
}
