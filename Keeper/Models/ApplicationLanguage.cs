using System.Globalization;

namespace Keeper.Models
{
    public class ApplicationLanguage
    {
        public string Name { get; set; }

        public CultureInfo CultureInfo { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
