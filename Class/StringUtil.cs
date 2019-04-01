using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;

namespace WebMap.Web
{
    public static class StringUtil
    {
        public static String GetMimetype(String fileExtension)
        {
            String type = String.Empty;
            switch (fileExtension.ToUpper())
            {
                case ".HTM":
                case ".HTML":
                    type = "text/HTML";
                    break;
                case ".TXT":
                case ".LOG":
                    type = "text/plain";
                    break;
                case ".DOC":
                case ".RTF":
                    type = "application/msword";
                    break;
                case ".CSV":
                case ".XLS":
                    type = "application/x-msexcel";
                    break;
                case ".PDF":
                    type = "application/pdf";
                    break;
                case ".DWF":
                    type = "model/vnd.dwf";
                    break;
                case ".TIF":
                case ".TIFF":
                    type = "image/tiff";
                    break;
                case ".JPG":
                case ".JPEG":
                    type = "image/jpeg";
                    break;
                case ".GIF":
                    type = "image/gif";
                    break;
                case ".PNG":
                    type = "image/png";
                    break;
                case ".BMP":
                    type = "image/bmp";
                    break;
                default:
                    type = "application/octet-stream";
                    break;
            }
            return type;

        }

        public static System.Drawing.Imaging.ImageFormat GetImageFormat(String fileExtension)
        {
            switch (fileExtension.ToUpper())
            {
                case ".TIF":
                case ".TIFF":
                    return System.Drawing.Imaging.ImageFormat.Tiff;
                case ".JPG":
                case ".JPEG":
                    return System.Drawing.Imaging.ImageFormat.Jpeg;
                case ".GIF":
                    return System.Drawing.Imaging.ImageFormat.Gif;
                case ".PNG":
                    return System.Drawing.Imaging.ImageFormat.Png;
                case ".BMP":
                    return System.Drawing.Imaging.ImageFormat.Bmp;
                default:
                    return null;
            }
        }

        public static String ToWSSafeString(this Object val, Boolean removeSpace = false, Boolean removeSingleQuote = false, Boolean removeDoubleQuote = false, Boolean removeEqualSign = false, Boolean removeSemicolon = false)
        {
            try
            {
                String result = val.ToString();
                if (removeSpace) result = result.Replace(" ", "");
                if (removeSingleQuote) result = result.Replace("'", "");
                if (removeDoubleQuote) result = result.Replace("\"", "");
                if (removeEqualSign) result = result.Replace("=", "");
                if (removeSemicolon) result = result.Replace(";", "");
                return result;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static String ToWSSafeDateText(this Object val)
        {
            try
            {
                return Convert.ToDateTime(val).ToShortDateString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static Double ToWSSafeDouble(this Object val)
        {
            try
            {
                return Convert.ToDouble(val);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static Boolean ToWSSafeBoolean(this Object val)
        {
            try
            {
                return Convert.ToBoolean(val);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static String ToWSTitleCase(this String str)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            return textInfo.ToTitleCase(str.ToLower());
        }

        public static String CheckAppendEnd(this String val, String StrToAppend)
        {
            if (val.EndsWith(StrToAppend))
            {
                return val;
            }
            else
            {
                return val + StrToAppend;
            }
        }

        public static String ToSqlBitText(this String str)
        {
            if (String.IsNullOrWhiteSpace(str) || str.Trim().ToUpper() == "NO" || str.Trim().ToUpper() == "FALSE")
            {
                return "0";
            }
            else return "1";
        }

        public static String EscapeSingeQuoteSqlParameter(this String str)
        {
            return str.Replace("'", "''");
        }

        public static String AddLeadingText(this String ss, String leadingChar, int charCount)
        {
            while (ss.Length < charCount)
            {
                ss = leadingChar + ss;
            }
            return ss;
        }

        public static String GetZip59(this String ss)
        {
            ss = ss.Trim();
            if (ss.Length < 5)
            {
                return String.Empty;
            }
            else if (ss.Length == 5)
            {
                return ss;
            }
            else if (ss.Length == 9)
            {
                return String.Format("{0}-{1}", ss.Substring(0, 5), ss.Substring(5, 4));
            }
            else if (ss.Length == 10)
            {
                return ss;
            }
            else return String.Empty;
        }

        public static String GetPhoneDisplayText(this String ss)
        {
            ss = ss.Trim();
            if (ss.Length == 10)
            {
                return Regex.Replace(ss, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3");
            }
            else if (ss.Length == 7)
            {
                return Regex.Replace(ss, @"(\d{3})(\d{4})", "$2-$3");
            }
            else
            {
                return ss;
            }
        }

        public static String GetTextMaxLength(this String ss, Int16 maxLength)
        {
            if (ss.Length > maxLength) return ss.Substring(0, maxLength); else return ss;
        }
    }
}