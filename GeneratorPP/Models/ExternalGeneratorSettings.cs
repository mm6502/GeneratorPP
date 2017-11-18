namespace Digital.Slovensko.Ekosystem.GeneratorPP.Models
{
    /// <summary>
    /// External QR string generator settings.
    /// </summary>
    public class ExternalGeneratorSettings
    {
        /// <summary>
        /// Gets or sets URL of the web service.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the service identifier.
        /// </summary>
        public string ServiceId { get; set; }

        /// <summary>
        /// Gets or sets the service user identifier.
        /// </summary>
        public string ServiceUserId { get; set; }
    }
}