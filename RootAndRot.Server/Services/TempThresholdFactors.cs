namespace RootAndRot.Server.Services
{
    public class TempThresholdFactors
    {
        public bool placeholder1 { get; set; }
        public bool placeholder2 { get; set; }
        public bool placeholder3 { get; set; }

        public float CalculateTempThreshold()
        {
            if (placeholder1)
            {
                return 50.0f;
            }
            else if (placeholder2)
            {
                return 60.0f;
            }
            else if (placeholder3)
            {
                return 70.0f;
            }
            else
            {
                return 40.0f;
            }
        }
    }
}
