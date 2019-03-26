using System.Text;
using Tomlyn.Syntax;

namespace Ergon.Core
{
    internal static class TomlynHelper
    {
        public static TableSyntaxBase FindByName(this SyntaxList<TableSyntaxBase> tables, string name)
        {
            foreach (var tableSyntaxBase in tables)
            {
                var tableName = AsText(tableSyntaxBase.Name.Key);
                if (tableName == name)
                {
                    return tableSyntaxBase;
                }
            }
            return null;
        }

        public static string AsText(this KeySyntax key)
        {
            var bareKey = AsText(key.Key);
            if (key.DotKeys.ChildrenCount == 0)
            {
                return bareKey;
            }
            var builder = new StringBuilder();
            builder.Append(bareKey);
            foreach (var dottedKeyItemSyntax in key.DotKeys)
            {
                builder.Append('.');
                builder.Append(AsText(dottedKeyItemSyntax.Key));
            }
            return builder.ToString();
        }

        public static string AsText(this BareKeyOrStringValueSyntax bareKeyOrString)
        {
            if (bareKeyOrString is BareKeySyntax bareKey)
            {
                return AsText(bareKey);
            }
            return AsText((StringValueSyntax) bareKeyOrString);
        }

        public static string AsText(this BareKeySyntax bareKey)
        {
            return bareKey.Key?.Text;
        }

        public static string AsText(this StringValueSyntax stringKey)
        {
            return stringKey.Token?.Text;
        }

        public static object AsObject(this ValueSyntax value)
        {
            switch (value)
            {
                case BooleanValueSyntax booleanValueSyntax:
                    return booleanValueSyntax.Value;
                case DateTimeValueSyntax dateTimeValueSyntax:
                    return dateTimeValueSyntax.Value;
                case FloatValueSyntax floatValueSyntax:
                    return floatValueSyntax.Value;
                case IntegerValueSyntax integerValueSyntax:
                    return integerValueSyntax.Value;
                case StringValueSyntax stringValueSyntax:
                    return stringValueSyntax.Value;
            }
            return null;
        }

    }
}
