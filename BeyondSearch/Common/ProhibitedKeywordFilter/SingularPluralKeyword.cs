using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;

namespace BeyondSearch.Common.ProhibitedKeywordFilter
{
    public class SingularPluralKeyword
    {
        private static readonly PluralizationService pluralizationService = 
            PluralizationService.CreateService(CultureInfo.CurrentCulture);

        public string Singular { get; private set; }
        public string Plural { get; private set; }
        public string OriginalKeyword { get; private set; }
        public bool HasBothSingularAndPlural { get; private set; }

        private SingularPluralKeyword() { }

        public static SingularPluralKeyword CreateInstance(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword) || keyword.Length < 2)
            {
                return new SingularPluralKeyword { OriginalKeyword = keyword };
            }

            if (pluralizationService.IsSingular(keyword))
            {
                return new SingularPluralKeyword
                {
                    HasBothSingularAndPlural = true,
                    OriginalKeyword = keyword,
                    Singular = keyword,
                    Plural = pluralizationService.Pluralize(keyword)
                };
            }

            if (pluralizationService.IsPlural(keyword))
            {
                return new SingularPluralKeyword
                {
                    HasBothSingularAndPlural = true,
                    OriginalKeyword = keyword,
                    Singular = pluralizationService.Singularize(keyword),
                    Plural = keyword
                };
            }

            return new SingularPluralKeyword { OriginalKeyword = keyword };
        }
    }
}
