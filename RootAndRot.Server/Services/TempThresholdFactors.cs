namespace RootAndRot.Server.Services
{
    public class TempThresholdFactors
    {
        public bool placeholder1 { get; set; }
        public bool placeholder2 { get; set; }
        public bool placeholder3 { get; set; }

        public Task<float> CalculateTempThreshold()
        {
            if (placeholder1)
            {
                return Task.FromResult(50.0f);
            }
            else if (placeholder2)
            {
                return Task.FromResult(60.0f);
            }
            else if (placeholder3)
            {
                return Task.FromResult(70.0f);
            }
            else
            {
                return Task.FromResult(40.0f);
            }
        }
    }
}
