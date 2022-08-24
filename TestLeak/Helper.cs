using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Helper
{
    public static class Convert
    {
        public static string ConvertReader(SqlDataReader dr, string sign)
        {
            try
            {
                if (dr[sign] == System.DBNull.Value) return "";

                return dr[sign].ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string ConvertDateReader(SqlDataReader dr, string sign)
        {
            try
            {
                if (dr[sign] == System.DBNull.Value) return "";

                return DateTime.Parse(dr[sign].ToString()).ToString("yyyy/M/dd");
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string ConvertDateYearReader(SqlDataReader dr, string sign)
        {
            try
            {
                if (dr[sign] == System.DBNull.Value) return "";

                return DateTime.Parse(dr[sign].ToString()).Year.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string ConvertShortDateReader(SqlDataReader dr, string sign)
        {
            try
            {
                if (dr[sign] == System.DBNull.Value) return "";

                return DateTime.Parse(dr[sign].ToString()).ToString("M月d日");
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static int ConvertIntReader(SqlDataReader dr, string sign)
        {
            try
            {
                if (dr[sign] == System.DBNull.Value) return 0;

                return int.Parse(dr[sign].ToString());
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static long ConvertLongReader(SqlDataReader dr, string sign)
        {
            try
            {
                if (dr[sign] == System.DBNull.Value) return 0;

                return long.Parse(dr[sign].ToString());
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static string ConvertString(DataRow dr, string sign)
        {
            try
            {
                if (dr[sign] == System.DBNull.Value) return "";

                return dr[sign].ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static double ConvertDouble(DataRow dr, string sign)
        {
            try
            {
                if (dr[sign] == System.DBNull.Value) return 0;
                return double.Parse(dr[sign].ToString());
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static double ConvertDoubleReader(SqlDataReader dr, string sign)
        {
            try
            {
                if (dr[sign] == System.DBNull.Value) return 0;
                return double.Parse(dr[sign].ToString());
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static int ConvertInt(DataRow dr, string sign)
        {
            try
            {
                if (dr[sign] == System.DBNull.Value) return 0;
                return int.Parse(dr[sign].ToString());
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static long ConvertLong(DataRow dr, string sign)
        {
            try
            {
                if (dr[sign] == System.DBNull.Value) return 0;
                return long.Parse(dr[sign].ToString());
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static string ConvertDate(DataRow dr, string sign)
        {
            try
            {
                if (dr[sign] == System.DBNull.Value) return "";

                return DateTime.Parse(dr[sign].ToString()).ToString("yyyy/M/dd");
                //return dr[sign].ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static bool ConvertBool(DataRow dr, string sign)
        {
            try
            {
                if (dr[sign] == System.DBNull.Value) return false;
                return bool.Parse(dr[sign].ToString());
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool ConvertBoolReader(SqlDataReader dr, string sign)
        {
            try
            {
                if (dr[sign] == System.DBNull.Value) return false;
                return bool.Parse(dr[sign].ToString());
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string ConvertColumnString(DataRow dr, int ColumnIndex)
        {
            try
            {
                if (dr[ColumnIndex] == System.DBNull.Value) return "";

                return dr[ColumnIndex].ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static bool IsEqualDate(string value1, string value2)
        {
            try
            {
                if (String.IsNullOrEmpty(value1) != String.IsNullOrEmpty(value2)) return false;
                if (String.IsNullOrEmpty(value1) && String.IsNullOrEmpty(value2)) return true;
                string d1= DateTime.Parse(value1).ToString();
                string d2 = DateTime.Parse(value2).ToString(); ;
                if (d1.Equals(d2)) return true; else return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string GetFileType(string Filename)
        {
            try
            {
                string result = "";
                string fileExt = Filename.Substring(Filename.LastIndexOf('.') + 1);
                switch (fileExt.ToUpper())
                {
                    case "JPG":
                    case "PNG":
                    case "BMP":
                        result = "图片文档";
                        break;
                    case "XLS":
                    case "XLSX":
                        result = "Excel表格";
                        break;
                    case "DOC":
                    case "DOCX":
                        result = "Word文档";
                        break;
                    case "PPT":
                    case "PPTX":
                        result = "PPT文档";
                        break;
                    case "PDF":
                        result = "PDF文档";
                        break;
                    default:
                        result = "";
                        break;
                }
                return result;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static double ConvertDoubleCelling(double arg1,double arg2)
        {
            try
            {
                double v= Math.Ceiling(arg1 / arg2 * 100);
                if (double.IsNaN(v)) return 0; else return v;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static double ConvertDouble(double arg1, double arg2)
        {
            try
            {
                double v = Math.Round(arg1 / arg2,2);
                if (double.IsNaN(v)) return 0; else return v;
            }
            catch (Exception)
            {
                return 0;
            }
        }


        public static bool CheckInsertSQL(ref string FieldList, ref string ValueList, string Field, string Value, string Type)
        {
            try
            {
                if (string.IsNullOrEmpty(Value)) return true;
                switch (Type.ToUpper())
                {
                    case "INT":
                    case "LONG":
                        if (FieldList.Length <= 0) { FieldList += Field; ValueList += Value; } else { FieldList += "," + Field; ValueList += "," + Value; }
                        break;
                    case "STRING":
                        if (FieldList.Length <= 0) { FieldList += Field; ValueList += "'" + Value + "'"; } else { FieldList += "," + Field; ValueList += ",'" + Value + "'"; }
                        break;
                    case "DATE":
                        if (FieldList.Length <= 0) 
                        {
                            FieldList += Field;
                            if (Value.ToUpper().Equals("GETDATE()"))
                            { ValueList +=Value; }
                            else if (!string.IsNullOrEmpty(Value))
                            { ValueList += "'" + Value + "'"; } 
                        }
                        else if(Value.ToUpper().Equals("GETDATE()")) 
                        {
                            FieldList += "," + Field;
                            ValueList += "," + Value;
                        }
                        else if(!string.IsNullOrEmpty(Value))
                        {
                            FieldList += "," + Field;
                            ValueList += ",'" + Value + "'";
                        }
                        break;
                    case "BOOL":
                        if (FieldList.Length <= 0) { FieldList += Field; ValueList += "'" + Value + "'"; } else { FieldList += "," + Field; ValueList += ",'" + Value + "'"; }
                        break;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool CheckUpdateSQL(ref string sqlList, string Field, string Value, string Type)
        {
            try
            {
                if (string.IsNullOrEmpty(Value)&&!Type.Equals("DATE")&&!Type.Equals("STRING")) return true;
                switch (Type.ToUpper())
                {
                    case "INT":
                    case "LONG":
                    case "DOUBLE":
                        if (sqlList.Length <= 0) { sqlList += Field + "=" + Value; } else { sqlList += "," + Field + "=" + Value; }
                        break;
                    case "DATE":
                        if (string.IsNullOrEmpty(Value))
                        {
                            if (sqlList.Length <= 0) { sqlList += Field + "=null"; } else { sqlList += "," + Field + "=null"; }
                        }
                        else
                        {
                            if (sqlList.Length <= 0) { sqlList += Field + "='" + Value + "'"; } else { sqlList += "," + Field + "='" + Value + "'"; }
                        }
                        break;
                    case "STRING":
                        if (sqlList.Length <= 0) { sqlList += Field + "='" + Value + "'"; } else { sqlList += "," + Field + "='" + Value + "'"; }
                        break;
                    case "BOOL":
                        if (sqlList.Length <= 0) { sqlList += Field + "='" + Value + "'"; } else { sqlList += "," + Field + "='" + Value + "'"; }
                        break;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string ConvertToShortDate(string value)
        {
            try
            {
                return DateTime.Parse(value).ToShortDateString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static bool GetWeekLyStartDate(int year, int weekNum, out DateTime weekStart)
        {
            try
            {
                weekNum -= 1;
                var dateTime = new DateTime(year, 1, 1);
                dateTime = dateTime.AddDays(7 * weekNum);
                weekStart = dateTime.AddDays(-(int)dateTime.DayOfWeek + (int)DayOfWeek.Monday);
                return true;
            }
            catch (Exception)
            {
                weekStart = DateTime.Now;
                return false;
            }
        }

        public static bool GetWeekLyEndDate(int year, int weekNum, out DateTime weekeEnd)
        {
            try
            {
                weekNum -= 1;
                var dateTime = new DateTime(year, 1, 1);
                dateTime = dateTime.AddDays(7 * weekNum);
                weekeEnd = dateTime.AddDays((int)DayOfWeek.Saturday - (int)dateTime.DayOfWeek + 1);
                return true;
            }
            catch (Exception)
            {
                weekeEnd = DateTime.Now;
                return false;
            }
        }

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int Index);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        public static double MillimetersToPixelsWidth(double DpiX, double length) //length是毫米，1厘米=10毫米
        {
            System.Windows.Forms.Panel p = new System.Windows.Forms.Panel();
            System.Drawing.Graphics g = System.Drawing.Graphics.FromHwnd(p.Handle);
            IntPtr hdc = g.GetHdc();
            int width = GetDeviceCaps(hdc, 4);     // HORZRES 
            int pixels = GetDeviceCaps(hdc, 8);     // BITSPIXEL
            g.ReleaseHdc(hdc);
            //return (((double)pixels / (double)width) * (double)length);
            return (((double)DpiX / (double)25.4) * (double)length);
        }

        public static double PixelsToMillimetersWidth(double DpiX, double length)
        {
            return (double)length * (double)25.4 / (double)DpiX;
        }


        public static double MillimetersToPixelsWidth(IntPtr hDc, double length)
        {
            int width = GetDeviceCaps(hDc, CapIndex.HORZSIZE);
            int pixels = GetDeviceCaps(hDc, CapIndex.HORZRES);
            return (((double)pixels / (double)width) * (double)length);
        }

        public static double PixelsToMillimetersWidth(IntPtr hDc, double length)
        {
            int width = GetDeviceCaps(hDc, CapIndex.HORZSIZE);
            int pixels = GetDeviceCaps(hDc, CapIndex.HORZRES);
            return (double)width / (double)pixels * (double)length;
        }

        public class CapIndex
        {
            public static readonly int DRIVERVERSION = 0;
            public static readonly int TECHNOLOGY = 2;
            public static readonly int HORZSIZE = 4;
            public static readonly int VERTSIZE = 6;
            public static readonly int HORZRES = 8;
            public static readonly int VERTRES = 10;
            public static readonly int BITSPIXEL = 12;
            public static readonly int PLANES = 14;
            public static readonly int NUMBRUSHES = 16;
            public static readonly int NUMPENS = 18;
            public static readonly int NUMMARKERS = 20;
            public static readonly int NUMFONTS = 22;
            public static readonly int NUMCOLORS = 24;
            public static readonly int PDEVICESIZE = 26;
            public static readonly int CURVECAPS = 28;
            public static readonly int LINECAPS = 30;
            public static readonly int POLYGONALCAPS = 32;
            public static readonly int TEXTCAPS = 34;
            public static readonly int CLIPCAPS = 36;
            public static readonly int RASTERCAPS = 38;
            public static readonly int ASPECTX = 40;
            public static readonly int ASPECTY = 42;
            public static readonly int ASPECTXY = 44;
            public static readonly int SHADEBLENDCAPS = 45;
            public static readonly int LOGPIXELSX = 88;
            public static readonly int LOGPIXELSY = 90;
            public static readonly int SIZEPALETTE = 104;
            public static readonly int NUMRESERVED = 106;
            public static readonly int COLORRES = 108;
            public static readonly int PHYSICALWIDTH = 110;
            public static readonly int PHYSICALHEIGHT = 111;
            public static readonly int PHYSICALOFFSETX = 112;
            public static readonly int PHYSICALOFFSETY = 113;
            public static readonly int SCALINGFACTORX = 114;
            public static readonly int SCALINGFACTORY = 115;
            public static readonly int VREFRESH = 116;
            public static readonly int DESKTOPVERTRES = 117;
            public static readonly int DESKTOPHORZRES = 118;
            public static readonly int BLTALIGNMENT = 119;
        }

        /// <summary> 
        /// RSA加密数据 
        /// </summary> 
        /// <param name="express">要加密数据</param> 
        /// <param name="KeyContainerName">密匙容器的名称</param> 
        /// <returns></returns> 
        public static string RSAEncryption(string express, string KeyContainerName = null)
        {

            System.Security.Cryptography.CspParameters param = new System.Security.Cryptography.CspParameters();
            param.KeyContainerName = KeyContainerName ?? "zhiqiang"; //密匙容器的名称，保持加密解密一致才能解密成功
            using (System.Security.Cryptography.RSACryptoServiceProvider rsa = new System.Security.Cryptography.RSACryptoServiceProvider(param))
            {
                byte[] plaindata = System.Text.Encoding.Default.GetBytes(express);//将要加密的字符串转换为字节数组
                byte[] encryptdata = rsa.Encrypt(plaindata, false);//将加密后的字节数据转换为新的加密字节数组
                return System.Convert.ToBase64String(encryptdata);//将加密后的字节数组转换为字符串
            }
        }
        /// <summary> 
        /// RSA解密数据 
        /// </summary> 
        /// <param name="express">要解密数据</param> 
        /// <param name="KeyContainerName">密匙容器的名称</param> 
        /// <returns></returns> 
        public static string RSADecrypt(string ciphertext, string KeyContainerName = null)
        {
            System.Security.Cryptography.CspParameters param = new System.Security.Cryptography.CspParameters();
            param.KeyContainerName = KeyContainerName ?? "zhiqiang"; //密匙容器的名称，保持加密解密一致才能解密成功
            using (System.Security.Cryptography.RSACryptoServiceProvider rsa = new System.Security.Cryptography.RSACryptoServiceProvider(param))
            {
                byte[] encryptdata = System.Convert.FromBase64String(ciphertext);
                byte[] decryptdata = rsa.Decrypt(encryptdata, false);
                return System.Text.Encoding.Default.GetString(decryptdata);
            }
        }
    }
}