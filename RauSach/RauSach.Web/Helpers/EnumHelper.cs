using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.Reflection;

namespace RauSach.Web.Helpers
{
    public static class EnumHelper
    {
        public static string GetDescriptionEnum<T>(T item) where T : struct, IConvertible
        {
            FieldInfo fi = typeof(T).GetField(item.ToString());
            DescriptionAttribute dna = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
            return dna != null ? dna.Description : item.ToString();
        }

        public static IEnumerable<SelectListItem> EnumToListItems<T>() where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }
            var ls = Enum.GetValues(typeof(T)).Cast<T>().ToList();
            List<SelectListItem> lsSelects = new List<SelectListItem>() { };
            foreach (T item in ls)
            {
                string txtSelectListItem = GetDescriptionEnum<T>(item);
                lsSelects.Add(new SelectListItem() { Text = txtSelectListItem, Value = item.GetHashCode().ToString() });
            }
            lsSelects = lsSelects.OrderBy(x => int.Parse(x.Value)).ToList();
            return lsSelects;
        }
    }
}
