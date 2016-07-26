using System;
using System.Globalization;

namespace InvoiceGenerator
{
  public static class Extentions
  {
    public static int ToIntOrDefault(this object value)
    {
      int v;
      return int.TryParse(value.ToString(), out v) ? v : 0;
    }

    public static decimal ToDecimalOrDefault(this object value)
    {
      decimal v;
      return decimal.TryParse(value.ToString(), out v) ? v : 0;
    }

    public static DateTime ToDateTimeOrDefault(this object value)
    {
      DateTime d;
      return DateTime.TryParse(value.ToString(), out d) ? d : DateTime.MinValue;
    }

    public static string[] StrSplit(this string value)
    {
      return value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
    }

    public static string DFormat(this DateTime dateTime, string format = "MM-dd-yyyy")
    {
      return dateTime.ToString(format);
    }

    public static string PFormat(this decimal value)
    {
      return value.ToString("P2", CultureInfo.InvariantCulture);
    }

    public static string CFormat(this decimal value, string format = "")
    {
      if (string.IsNullOrWhiteSpace(format))
      {
        format = "C2";
      }
      return value.ToString(format).Replace("$", string.Empty);
    }

    public static string GetCurrency(string currency)
    {
      switch (currency)
      {
        case "inr":
          return "Rupees";
        case "usd":
          return "Dollars";
        default:
          return string.Empty;
      }
    }
  }
}