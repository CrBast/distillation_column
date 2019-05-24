namespace Arduino_Viewer
{
    /// <summary>
    /// Request : All entry by the Arduino (separate by \r\n) 
    /// </summary>
    public class Request
    {
        #region Request Attribute
        public string Type { get; set; }

        public string Content { get; set; }

        public string FullRequest { get; set; }
        #endregion

        #region Request Type
        public const string TypeInfo = "-i";
        public const string TypeState = "-s";
        public const string TypeHeatingPower = "-p";
        #endregion
    }
}
