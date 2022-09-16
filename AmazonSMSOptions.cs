namespace _3ai.solutions.AmazonSMS
{
    public record AmazonS3Options()
    {
        public string AccessKey { get; init; } = string.Empty;
        public string SecretAccessKey { get; init; } = string.Empty;
        public string Region { get; init; } = "EUWest1";
    }
}