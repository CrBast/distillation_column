namespace Arduino_Viewer
{
    public class Request
    {
        public string Type { get; set; }

        public string Content { get; set; }

        public string FullRequest { get; set; }

        public const string TypeInfo = "-i";
        public const string TypeState = "-s";
        public const string TypeHeatingPower = "-p";
    }
}
