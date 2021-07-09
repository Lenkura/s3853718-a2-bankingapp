using System;

namespace DataValidator
{
    public static class UserInput

    {
        private static readonly int __accountNumberLength = 4;
        private static readonly int _loginNumberLength = 8;
        private static readonly int _maxCommentLength = 30;
        public static bool IsAccountNumberFormat(this string s)
        {
            if (int.TryParse(s, out _) && s.Length == __accountNumberLength)
                return true;
            else
                return false;
        }
        public static bool IsAccountNumberFormat(this int i)
        {
            if (i.ToString().Length == __accountNumberLength)
                return true;
            else
                return false;
        }
        public static bool IsDollarAmount(this string s)
        {
            if (s.Contains("."))
            {
                if (s.Substring(s.IndexOf(".") + 1).Length < 3)
                    return true;
                else return false;
            }

            if (double.TryParse(s, out _) && Convert.ToDouble(s) > 0)
                return true;
            else
                return false;
        }
        public static bool IsValidComment(this string s)
        {
            if (s == null)
                return true;
            if (s.Length > _maxCommentLength)
                return false;
            else
                return true;
        }
        public static bool IsCustomerIDFormat(this string s) => s.IsAccountNumberFormat();
        public static bool IsCustomerIDFormat(this int i) => i.IsAccountNumberFormat();
        public static bool IsLoginIDFormat(this string s)
        {
            {
                if (int.TryParse(s, out _) && s.Length == _loginNumberLength)
                    return true;
                else
                    return false;
            }
        }
    }
}
