using System.ComponentModel;

namespace CDD.Api.Extensions
{
    /// <summary>
    /// EnumExtensions 擴充方法
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// 擴充方法 取得EnumDescription
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string? GetEnumDescription(this Enum e)
        {
            var descriptionAttribute = e.GetType().GetMember(e.ToString())[0]
                .GetCustomAttributes(typeof(DescriptionAttribute), inherit: false)[0]
                as DescriptionAttribute;

            return descriptionAttribute?.Description;
        }
    }
}
