namespace Digital.Slovensko.Ekosystem.GeneratorPP.Models
{
    /// <summary>
    /// Stores the progress of processing the request.
    /// </summary>
    public class ProcessingModel
    {
        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        /// <value>
        /// The request identifier.
        /// </value>
        public string RequestId { get; set; }

        /// <summary>
        /// Gets or sets the progress in percents.
        /// </summary>
        /// <value>
        /// The percent.
        /// </value>
        public int Percent { get; set; }
    }
}
