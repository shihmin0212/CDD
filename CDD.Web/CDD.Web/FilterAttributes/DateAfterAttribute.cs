using System.ComponentModel.DataAnnotations;

namespace CDD.Web.FilterAttributes
{
    public class DateAfterAttribute : ValidationAttribute
    {
        private DateTime start;

        public DateAfterAttribute(string dateString, string format = "yyyy/MM/dd")
        {
            start = DateTime.ParseExact(dateString, format, null);
        }


        public override bool IsValid(object value)
        {
            var date = (DateTime)value;

            if (date.Ticks > start.Ticks)
            {
                return true;
            }
            return false;
        }
    }
}
