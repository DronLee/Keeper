using Data.Abstractions;
using System.Text;
using System.Text.RegularExpressions;

namespace Data.Encrypter
{
    /// <summary>
    /// Converter for getting data rows from string and for getting string from data rows
    /// </summary>
    internal static class DataRowConverter
    {
        /// <summary>
        /// Full string with login can be like this: +section+name---password, login
        /// </summary>
        private readonly static Regex rowRegex = new Regex(
            @"^(\+(?<section>.*)\+)?(?<name>.+)---(?<password>[^\t]+)(,\s(?<login>[^\t]+))",
            RegexOptions.Compiled);

        /// <summary>
        /// String without login can be kind of this:  +section+name---password
        /// </summary>
        private readonly static Regex rowRegexWithoutLogin = new Regex(
            @"^(\+(?<section>.*)\+)?(?<name>.+)---(?<password>[^\t]+)",
            RegexOptions.Compiled);

        public static DataRow DataRowFromString(string row)
        {
            var match = rowRegex.Match(row);
            if (!match.Success)
                match = rowRegexWithoutLogin.Match(row);
            if (match.Success)
            {
                return new DataRow
                {
                    Login = match.Groups["login"].Success ? match.Groups["login"].Value : null,
                    Password = match.Groups["password"].Value,
                    Name = match.Groups["name"].Value,
                    Section = match.Groups["section"].Value
                };
            }
            else
            {
                return new DataRow();
            }
        }

        public static string DataRowToString(DataRow dataRow)
        {
            var stringBuilder = new StringBuilder(dataRow.Section == null ? string.Empty : $"+{dataRow.Section}+");
            stringBuilder.Append(string.Format("{0}---{1}", dataRow.Name, dataRow.Password));
            if (dataRow.Login != null)
                stringBuilder.Append(", ").Append(dataRow.Login);
            return stringBuilder.ToString();
        }
    }
}
