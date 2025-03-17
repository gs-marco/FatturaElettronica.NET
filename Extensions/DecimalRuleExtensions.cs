using FluentValidation;

namespace FatturaElettronica.Extensions
{
    internal static class DecimalRuleExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> ScalePrecision8DecimalType<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.ScalePrecision(8, 19);
        }
        
        public static IRuleBuilderOptions<T, TProperty> ScalePrecision2DecimalType<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.ScalePrecision(2, 13);
        }

        public static void ScalePrecisionPercentualeDecimalType<T>(this IRuleBuilder<T, decimal?> ruleBuilder)
        {
            ruleBuilder.ScalePrecision(2, 6);
        }
    }
}