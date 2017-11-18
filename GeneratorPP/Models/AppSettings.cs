namespace Digital.Slovensko.Ekosystem.GeneratorPP.Models
{
    /// <summary>
    /// Application settings.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Gets or sets the support mail.
        /// </summary>
        public string SupportMail { get; set; }

        /// <summary>
        /// Gets or sets the generator to use.
        /// </summary>
        public string UseGenerator { get; set; }

        /// <summary>
        /// Gets or sets the external generator.
        /// </summary>
        public ExternalGeneratorSettings ExternalGenerator { get; set; }
    }
}
