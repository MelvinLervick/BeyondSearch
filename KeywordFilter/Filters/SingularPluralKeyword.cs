using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;

namespace KeywordFilter.Filters
{
    public class SingularPluralKeyword
    {
        private static readonly PluralizationService PluralizationService = 
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

            if (PluralizationService.IsSingular(keyword))
            {
                return new SingularPluralKeyword
                {
                    HasBothSingularAndPlural = true,
                    OriginalKeyword = keyword,
                    Singular = keyword,
                    Plural = PluralizationService.Pluralize(keyword)
                };
            }

            if (PluralizationService.IsPlural(keyword))
            {
                return new SingularPluralKeyword
                {
                    HasBothSingularAndPlural = true,
                    OriginalKeyword = keyword,
                    Singular = PluralizationService.Singularize(keyword),
                    Plural = keyword
                };
            }

            return new SingularPluralKeyword { OriginalKeyword = keyword };
        }
    }
}
