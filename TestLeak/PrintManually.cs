using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Helper;
using ZXing;
using ZXing.Common;

namespace TestLeak
{
    public partial class PrintManually : Form
    {
        public Parameter.PlanDataModel PlanData;
        public LabelTemplateModel CurrentLabelTemplate;

        public PrintManually()
        {
            InitializeComponent();
        }

        private Bitmap Generate128Code(string code, int height, int width)
        {
            try
            {
                BarcodeWriter writer = new BarcodeWriter();
                writer.Format = BarcodeFormat.CODE_128;
                EncodingOptions options = new EncodingOptions()
                {
                    PureBarcode = true,
                    Width = width,
                    Height = height,
                    Margin = 0
                };
                writer.Options = options;
                Bitmap map = writer.Write(code);
                return map;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static double MillimetersToPixelsWidth(double length)
        {
            System.Windows.Forms.Panel p = new System.Windows.Forms.Panel();
            System.Drawing.Graphics g = System.Drawing.Graphics.FromHwnd(p.Handle);
            IntPtr hdc = g.GetHdc();
            int width = GetDeviceCaps(hdc, 4); //HORZRES
            int pixels = GetDeviceCaps(hdc, 8);// BITSPIXEL
            g.ReleaseHdc(hdc);
            return (((double)pixels / (double)width) * (double)length);
        }
        [DllImport("gdi32.dll")] private static extern int GetDeviceCaps(IntPtr hdc, int Index);

        private void button_Print_TestPrint_Click(object sender, EventArgs e)
        {
            try
            {
                Parameter MyParameter = MyConfig.GetParameter();
                if (!MyParameter.PrintSerial.ResetSign.ToShortDateString().Equals(DateTime.Now.ToShortDateString()))
                {
                    MyParameter.PrintSerial.ResetSign = DateTime.Now.Date;
                    MyParameter.PrintSerial.Serial = 0;
                }
                MyParameter.PrintSerial.Serial++;
                if (!MyConfig.SavePrintSerial(MyParameter))
                {
                    throw new Exception("更新打印序列号失败！");
                }

                DateTime PrintTime = DateTime.Parse(PlanData.PrintDate);
                StringBuilder PrintDoc = new StringBuilder();
                PrintDoc.AppendLine("CT~~CD,~CC^~CT~");
                PrintDoc.AppendLine("^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR4,4~SD25^JUS^LRN^CI0^XZ");
                PrintDoc.AppendLine("^XA");
                PrintDoc.AppendLine("*InsertLine*");
                PrintDoc.AppendLine("^PQ*PrintCount*,0,1,Y^XZ");

                CurrentLabelTemplate.LabelFields.ForEach(T =>
                {
                    if (T.ObjectType == DrawObjectType.Text && T.ObjectValueType == DrawObjectValueType.Variable)
                    {
                        switch (T.Sign.ToUpper())
                        {
                            case "DATE":
                                T.PrintValue = PrintTime.ToString("yyMMdd");
                                break;
                            case "TIME":
                                T.PrintValue = PrintTime.ToString("HH:mm:ss");
                                break;
                            case "SERIAL":
                                T.PrintValue = MyParameter.PrintSerial.Serial.ToString().PadLeft(3, '0');
                                break;
                            default:
                                MessageBox.Show("动态标识无法识别！");
                                throw new Exception("error");
                        }
                    }
                });

                float Scalex = MyParameter.PrintSettings.DpiX / 96;
                int LabelWidth = (int)(Helper.Convert.MillimetersToPixelsWidth(96, CurrentLabelTemplate.LabelWidth * 10) * Scalex);
                int LabelHeight = (int)(Helper.Convert.MillimetersToPixelsWidth(96, CurrentLabelTemplate.LabelLength * 10) * Scalex);
                Bitmap b = CurrentLabelTemplate.InitPrintBitmap(LabelWidth, LabelHeight*2, Scalex);

                int TotalBytes = 0, RowBytes = 0;
                string Temp = ZebraUnity.BitmapToHex(b, out TotalBytes, out RowBytes);
                Temp = string.Format("~DGR:Temp0.GRF,{0},{1},{2}", TotalBytes, RowBytes, Temp);
                Temp += "^FO0,0^XGR:Temp0.GRF,1,1^FS";
                Temp = PrintDoc.ToString().Replace("*InsertLine*", Temp);
                Temp = Temp.Replace("*PrintCount*", ((int)numericUpDown_PrintCount.Value).ToString());
                if (!MyZebra.PrintWithDrv(Temp, MyParameter.PrintSettings.PrintName)) throw new Exception("打印失败！");
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                DialogResult = DialogResult.Cancel;
            }
            finally
            {
                Hide();
            }

        }

        private void PrintManually_Load(object sender, EventArgs e)
        {
            try
            {
                button_Print_TestPrint.Enabled = false;
                if (PlanData != null)
                {
                    textBox_Label.Text = PlanData.PartItem.Item;
                    PlanData.PrintDate = DateTime.Now.ToString();
                    button_Print_TestPrint.Enabled = true;
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
