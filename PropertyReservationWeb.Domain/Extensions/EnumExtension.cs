using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace PropertyReservationWeb.Domain.Extensions
{
    public static class EnumExtension
    {
        public static string GetDisplayName(this System.Enum enumValue)
        {
            //var member = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();
            //return member?.GetCustomAttribute<DisplayAttribute>()?.Name ?? enumValue.ToString();
            return enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()
                ?.GetName() ?? "Неопределенный";
        }
    }
}
