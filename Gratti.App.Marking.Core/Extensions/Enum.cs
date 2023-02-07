using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace Gratti.App.Marking.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType().GetMember(enumValue.ToString()).First()
                .GetCustomAttribute<DisplayAttribute>()?.GetName();
        }
    }
}
