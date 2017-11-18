namespace Digital.Slovensko.Ekosystem.GeneratorPP.Models
{
    /// <summary>
    /// Model for displaying errors.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Gets a value indicating whether [show request identifier].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show request identifier]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowRequestId
        {
            get { return !string.IsNullOrEmpty(RequestId); }
        }
    }
}