using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Net.Sockets;

namespace Helper
{
    class MyZebra
    {
        /// <summary>
        /// 线程锁，防止多线程
        /// </summary>
        private static object SyncRoot;

        /// <summary>
        ///  定义设备类型枚举
        /// </summary>
        public enum DeviceType
        {
            LPT=0,
            DRV=1,
            TCP=2
        }

        /// <summary>
        /// 定义打印机指令类型枚举
        /// </summary>
        public enum ProgrammingLanguage
        {
            ZPL=0,
            EPL=1,
            CPCL=2
        }

        public static string PrintName { get; set; }
        public static DeviceType PrintType { get; set; }
        public static ProgrammingLanguage PrinterProgrammingLanguage { get; set; }
        public static string TcpIpAddress { get; set; }
        public static int TcpPort { get; set; }
        public static int LptPort { get; set; }

        static MyZebra()
        {
            PrintType = DeviceType.DRV;
            PrinterProgrammingLanguage = ProgrammingLanguage.ZPL;
        }

        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }

        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public extern static bool OpenPrinter(string szPrinter, ref IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public extern static bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public extern static bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public extern static bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public extern static bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public extern static bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public extern static bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, ref Int32 dwWritten);

        /// <summary>
        /// 发送byte值给打印机
        /// </summary>
        /// <param name="szPrinterName">打印机名称</param>
        /// <param name="pBytes"></param>
        /// <param name="dwCount">字符长度</param>
        /// <returns>成功标记(true为成功)</returns>
        public static bool SendBytesToPrinter(String szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0;
            Int32 dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false;
            di.pDocName = "My C#.NET RAW Document";
            di.pDataType = "RAW";
            try
            {
                //Open the printer.
                if (OpenPrinter(szPrinterName.Normalize(), ref hPrinter, IntPtr.Zero))
                {
                    //Start a document.
                    if (StartDocPrinter(hPrinter, 1, di))
                    {
                        //Start a page
                        if (StartPagePrinter(hPrinter))
                        {
                            bSuccess = WritePrinter(hPrinter, pBytes, dwCount, ref dwWritten);
                            EndPagePrinter(hPrinter);
                        }
                        EndDocPrinter(hPrinter);
                    }
                    ClosePrinter(hPrinter);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            if (!bSuccess)
            {
                dwError = Marshal.GetLastWin32Error();
            }
            return bSuccess;
        }

        public static bool SendFileToPrinter(string szPrinterName, string szFileName)
        {
            FileStream fs = new FileStream(szFileName, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            byte[] bytes = new byte[fs.Length - 1];
            bool bSuccess = false;
            IntPtr pUnmanagedBytes = new IntPtr(0);
            int nLength;
            nLength = System.Convert.ToInt32(fs.Length);
            bytes = br.ReadBytes(nLength);
            pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
            Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
            bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);
            Marshal.FreeCoTaskMem(pUnmanagedBytes);
            return bSuccess;
        }

        public static bool SendStringToPrinter(string szPrinterName, string szString)
        {
            IntPtr pBytes;
            Int32 dwCount;
            bool bSuccess = false;
            dwCount = szString.Length;
            pBytes = Marshal.StringToCoTaskMemAnsi(szString);
            bSuccess = SendBytesToPrinter(szPrinterName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
            return bSuccess;
        }

        private static bool DrvPrint(string szString)
        {
            bool result = false;
            try
            {
                if (!string.IsNullOrEmpty(PrintName))
                {
                    result = SendStringToPrinter(PrintName, szString);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return result;
        }

        private static bool TcpPrint(byte[] cmdBytes)
        {
            bool result = false;
            TcpClient tcp = null;
            try
            {
                tcp = new TcpClient(TcpIpAddress, TcpPort);
                tcp.ReceiveTimeout = 1000;
                tcp.SendTimeout = 1000;
                if (tcp.Connected)
                {
                    tcp.Client.Send(cmdBytes);
                    result = true;
                }
            }
            catch (Exception)
            {
                result = false;
            }
            finally
            {
                if (tcp != null)
                {
                    if (tcp.Client != null)
                    {
                        tcp.Client.Close();
                        tcp.Client = null;
                    }
                    tcp.Close();
                    tcp = null;
                }
            }
            return result;
        }

        private static bool LptPrint(byte[] cmdBytes)
        {
            bool result = false;
            try
            {

            }
            catch (Exception)
            {
                
            }
            return result;
        }

        private static bool PrintCommand(string cmd)
        {
            bool result = false;
            try
            {
                switch (PrintType)
                {
                    case DeviceType.DRV:
                        result = DrvPrint(cmd);
                        break;
                    case DeviceType.LPT:
                        break;
                    case DeviceType.TCP:
                        result = TcpPrint(Encoding.Default.GetBytes(cmd));
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return result;
        }

        public static bool PrintWithDrv(string cmd, string printName)
        {
            PrintType = DeviceType.DRV;
            PrintName = printName;
            return PrintCommand(cmd);
        }

        public static bool PrintWithTcp(string cmd)
        {
            PrintType = DeviceType.TCP;
            return PrintCommand(cmd);
        }

        public static bool PrintWithLPT(string cmd, int Port)
        {
            PrintType = DeviceType.LPT;
            LptPort = Port;
            return PrintCommand(cmd);
        }
    }
}
