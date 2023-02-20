using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Gratti.App.Marking.Core.Extensions
{
    public static class EnumExtensions
    {
        public class ValueDisplayName
        {
            public object Value { get; set; }
            public object DisplayName { get; set; }
        }


        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType().GetMember(enumValue.ToString()).First()
                .GetCustomAttribute<DisplayAttribute>()?.GetName();
        }

        public static string GetDescription(this Enum value)
        {
            var attributes = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Any())
                return (attributes.First() as DescriptionAttribute).Description;

            // If no description is found, the least we can do is replace underscores with spaces
            // You can add your own custom default formatting logic here
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            return ti.ToTitleCase(ti.ToLower(value.ToString().Replace("_", " ")));
        }

        public static IEnumerable<ValueDisplayName> GetAllValuesDisplayName(Type t)
        {
            if (!t.IsEnum)
                throw new ArgumentException($"{nameof(t)} must be an enum type");

            Enum.GetValues(t).Cast<Enum>();
            return Enum.GetValues(t).Cast<Enum>().Select((e) => new ValueDisplayName() { Value = e, DisplayName = e.GetDisplayName() }).ToList();
        }
    }

}
