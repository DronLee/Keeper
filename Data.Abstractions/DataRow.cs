namespace Data.Abstractions
{
    /// <summary>
    /// Model of row with data for keeping
    /// </summary>
    public class DataRow
    {
        /// <summary>
        /// Name of keeping information
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A login for authentication
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// A password for authentication
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Name of section for data row
        /// </summary>
        public string Section { get; set; }

        /// <summary>
        /// True - row doesn't contains valuable information
        /// </summary>
        public bool IsEmpty => string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(Login) && string.IsNullOrEmpty(Password);
    }
}
