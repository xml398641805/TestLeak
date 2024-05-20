using Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using ZXing;
using ZXing.Common;
using ZXing.Datamatrix;
using ZXing.Datamatrix.Encoder;
using ZXing.OneD;

namespace TestLeak
{
    public partial class DesignLabel : Form
    {
        private List<string> TemplateNames;
        private static List<LabelTemplateModel> LabelArray;

        private LabelTemplateModel EditLabelTemplate = new LabelTemplateModel();
        private List<LabelFieldModel> EditLabelFields = new List<LabelFieldModel>();
        private List<LabelFieldModel> CopyLabelFields = new List<LabelFieldModel>();
        private LabelFieldModel ScaleLabelField = new LabelFieldModel();
        private int ScaleHandleIndex = -1;
        private bool InitFinished;

        private ComboBox TempCombobox = new ComboBox();
        private NumericUpDown TempNumericUpDown = new NumericUpDown();
        private TextBox TempTextBox = new TextBox();
        private string EditSign;

        private Point CurrentMousePoint = new Point();
        private Point SelectPoint = new Point();
        private MouseOperation MouseOperationStatus;
        private const int ScreenDpiX = 96;
        private double ScaleDpiX = 96;

        private int RulerPadding = 30;
        private int BoxScrollVertical = 0;
        private int BoxScrollHorizontal = 0;

        public DesignLabel(List<string> templateNames)
        {
            InitializeComponent();
            TemplateNames = templateNames;
            LabelArray = new List<LabelTemplateModel>();
        }

        private void DesignLabel_Load(object sender, EventArgs e)
        {
            try
            {
                SetButtonStatus(false);
                LabelArray = MyLabelTemplate.InitLabelTemplate();
                Template_toolStripComboBox_Name.Items.Clear();
                TemplateNames.ForEach(T => { Template_toolStripComboBox_Name.Items.Add(T); });
                SetButtonStatus(true);
                InitButtonStatus();

                Template_dataGridView.Controls.Add(TempCombobox);
                Template_dataGridView.Controls.Add(TempNumericUpDown);
                Template_dataGridView.Controls.Add(TempTextBox);
                TempTextBox.Visible = false;
                TempCombobox.Visible = false;
                TempCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
                TempNumericUpDown.Visible = false;
                TempNumericUpDown.Minimum = 1;
                TempNumericUpDown.Maximum = 9999;

                comboBox_DpiX.SelectedItem = "96";

                TempCombobox.SelectedValueChanged += TempCombobox_SelectedValueChanged;
                TempNumericUpDown.ValueChanged += TempNumericUpDown_ValueChanged;
                TempTextBox.TextChanged += TempTextBox_TextChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Hide();
            }
        }

        private void TempTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (TempTextBox.Visible)
                {
                    if (EditLabelFields.Count != 1 || string.IsNullOrEmpty(EditSign)) throw new Exception("所选字段状态不正确！");
                    switch (EditSign)
                    {
                        case "打印值":
                            EditLabelFields[0].PrintValue = TempTextBox.Text;
                            break;
                        default:
                            throw new Exception("遇到意外的编辑标志！");
                    }
                    Redraw();
                    TempTextBox.Focus();
                }
            }
            catch (Exception ex)
            {
                EditSign = "";
                EditLabelFields.Clear();
                MouseOperationStatus = MouseOperation.Idel;
                TempTextBox.Visible = false;
                MessageBox.Show("执行失败，返回错误：" + ex.Message);
            }
        }

        private void TempNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (TempNumericUpDown.Visible)
                {
                    if (EditLabelFields.Count != 1 || string.IsNullOrEmpty(EditSign)) throw new Exception("所选字段状态不正确！");
                    EditLabelFields[0].FontSize = (float)(TempNumericUpDown.Value <= 0 ? 1 : TempNumericUpDown.Value);
                    if (EditLabelFields[0].ObjectType == DrawObjectType.Text || EditLabelFields[0].ObjectType == DrawObjectType.Date || EditLabelFields[0].ObjectType == DrawObjectType.Number)
                    {
                        EditLabelFields[0].ScaleWidth = 0;
                        EditLabelFields[0].ScaleHeight = 0;
                    }
                    else if (EditLabelFields[0].ObjectType == DrawObjectType.Matrix || EditLabelFields[0].ObjectType == DrawObjectType.QRCode)
                    {
                        EditLabelFields[0].ScaleHeight = 0;
                        EditLabelFields[0].ScaleWidth = 0;
                        EditLabelFields[0].Width = (float)(TempNumericUpDown.Value <= 0 ? 1 : TempNumericUpDown.Value);
                        EditLabelFields[0].Length = (float)(TempNumericUpDown.Value <= 0 ? 1 : TempNumericUpDown.Value);
                    }
                    else if (EditLabelFields[0].ObjectType == DrawObjectType.Barcode)
                    {
                        EditLabelFields[0].ScaleHeight = 0;
                        EditLabelFields[0].ScaleWidth = 0;
                        EditLabelFields[0].Width = (float)(TempNumericUpDown.Value <= 0 ? 1 : TempNumericUpDown.Value);
                    }
                    Redraw();
                    TempNumericUpDown.Focus();
                }
            }
            catch (Exception ex)
            {
                EditSign = "";
                EditLabelFields.Clear();
                MouseOperationStatus = MouseOperation.Idel;
                TempNumericUpDown.Value = 1;
                TempNumericUpDown.Visible = false;
                MessageBox.Show("执行失败，返回错误：" + ex.Message);
            }
        }

        private void TempCombobox_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (TempCombobox.Visible)
                {
                    if (EditLabelFields.Count != 1 || string.IsNullOrEmpty(EditSign)) throw new Exception("所选字段状态不正确！");
                    switch (EditSign)
                    {
                        case "值类型":
                            EditLabelFields[0].FieldObjectValueType = TempCombobox.Text;
                            EditLabelFields[0].ObjectValueType = (DrawObjectValueType)Enum.Parse(typeof(DrawObjectValueType), TempCombobox.Text);
                            break;
                        case "打印值":
                            if (EditLabelFields[0].ObjectType == DrawObjectType.Date && EditLabelFields[0].ObjectValueType == DrawObjectValueType.Regular)
                            {
                                EditLabelFields[0].PrintValue = TempCombobox.Text;
                            }
                            break;
                        case "旋转":
                            EditLabelFields[0].Angle = float.Parse(TempCombobox.Text);
                            break;
                        case "字体":
                            EditLabelFields[0].FontName = TempCombobox.Text;
                            break;
                        default:
                            throw new Exception("遇到意外的编辑标志！");
                    }
                    Redraw();
                    TempCombobox.Focus();
                }
            }
            catch (Exception ex)
            {
                EditSign = "";
                EditLabelFields.Clear();
                MouseOperationStatus = MouseOperation.Idel;
                TempCombobox.Visible = false;
                MessageBox.Show("执行失败，返回错误：" + ex.Message);
            }
        }

        private void SetButtonStatus(bool Sign)
        {
            try
            {
                toolStripButton_Save.Enabled = Sign;
                Template_button_Redraw.Enabled = Sign;
                foreach (ToolStripItem C in toolStrip_LabelPanel.Items) { if (C.Tag != null && C.Tag.Equals("Button")) C.Enabled = Sign; }
                foreach(ToolStripItem C in toolStrip_DrawPanel.Items) { if (C.Tag != null && C.Tag.Equals("Button")) C.Enabled = Sign; }
            }
            catch (Exception ex)
            {
                throw new Exception("设置程序状态失败！");
            }
        }

        private void InitButtonStatus()
        {
            try
            {
                EditLabelFields.Clear();
                EditLabelTemplate = new LabelTemplateModel();
                CopyLabelFields.Clear();
                MouseOperationStatus = MouseOperation.Idel;
                Template_toolStripComboBox_Name.Text = "";
                Template_toolStripButton_New.Enabled = true;
                Template_toolStripComboBox_Name.Enabled = true;
                Template_toolStripButton_Save.Enabled = false;
                Template_toolStripButton_Cancel.Enabled = false;
                Template_toolStripButton_DeleteTemplate.Enabled = false;
                Template_toolStripButton_TestPrint.Enabled = false;
                Template_dataGridView.DataSource = new BindingList<LabelFieldModel>(EditLabelTemplate.LabelFields);
                foreach (ToolStripItem C in toolStrip_DrawPanel.Items) { if (C.Tag != null && C.Tag.Equals("Button")) C.Enabled = false; }
            }
            catch (Exception)
            {
                throw new Exception("初始程序状态失败！");
            }
            {
                Redraw();
            }
        }

        private void SetEditButtonStatus()
        {
            try
            {
                InitFinished = true;
                Template_toolStripButton_New.Enabled = false;
                Template_toolStripComboBox_Name.Enabled = false;
                Template_toolStripButton_Save.Enabled = true;
                Template_toolStripButton_Cancel.Enabled = true;
                Template_toolStripButton_DeleteTemplate.Enabled = true;
                Template_toolStripButton_TestPrint.Enabled = true;
                comboBox_DpiX.SelectedIndex = 0;
                foreach (ToolStripItem C in toolStrip_DrawPanel.Items) { if (C.Tag != null && C.Tag.Equals("Button")) C.Enabled = true; }

                MouseOperationStatus = MouseOperation.Idel;
                EditLabelFields.Clear();
                CopyLabelFields.Clear();
                LabelTemplateModel TempTemplate = LabelArray.Find(T => { return T.LabelName.Equals(Template_toolStripComboBox_Name.Text); });
                if (TempTemplate == null)
                {
                    EditLabelTemplate = new LabelTemplateModel();
                    EditLabelTemplate.LabelName = Template_toolStripComboBox_Name.Text;
                    EditLabelTemplate.LabelWidth = 10;
                    EditLabelTemplate.LabelLength = 10;
                    Template_numericUpDown_LabelWidth.Value = 10;
                    Template_numericUpDown_LabelLength.Value = 10;
                    Template_pictureBox_Preview.Width = (int)Helper.Convert.MillimetersToPixelsWidth(ScreenDpiX, 10 * 10);
                    Template_pictureBox_Preview.Height = (int)Helper.Convert.MillimetersToPixelsWidth(ScreenDpiX, 10 * 10);
                }
                else
                {
                    EditLabelTemplate.LabelName = TempTemplate.LabelName;
                    EditLabelTemplate.LabelWidth = TempTemplate.LabelWidth;
                    EditLabelTemplate.LabelLength = TempTemplate.LabelLength;
                    TempTemplate.LabelFields.ForEach(T =>
                    {
                        var T_Properties = T.GetType().GetProperties();
                        LabelFieldModel TempField = new LabelFieldModel();
                        var TempField_Properties = TempField.GetType().GetProperties();
                        for (int Col = 0; Col < T_Properties.Length; Col++)
                        {
                            TempField_Properties[Col].SetValue(TempField, T_Properties[Col].GetValue(T));
                        }
                        TempField.ObjectType = T.ObjectType;
                        TempField.ObjectValueType = T.ObjectValueType;
                        EditLabelTemplate.LabelFields.Add(TempField);
                    });

                    Template_numericUpDown_LabelWidth.Value = (decimal)EditLabelTemplate.LabelWidth;
                    Template_numericUpDown_LabelLength.Value = (decimal)EditLabelTemplate.LabelLength;
                    Template_pictureBox_Preview.Width = (int)Helper.Convert.MillimetersToPixelsWidth(ScreenDpiX, EditLabelTemplate.LabelWidth * 10);
                    Template_pictureBox_Preview.Height = (int)Helper.Convert.MillimetersToPixelsWidth(ScreenDpiX, EditLabelTemplate.LabelLength * 10);
                }
                Template_dataGridView.DataSource = new BindingList<LabelFieldModel>(EditLabelTemplate.LabelFields);
            }
            catch (Exception)
            {
                throw new Exception("进入程序编辑状态失败！");
            }
            finally
            {
                Redraw();
                InitFinished = false;
            }
        }

        private void toolStripButton_Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (MyLabelTemplate.WriteLabelTemplate()) MessageBox.Show("保存成功！");
                DialogResult = DialogResult.OK;
                Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存失败，返回错误：" + ex.Message);
            }
        }

        private void toolStripButton_Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        private void Template_toolStripComboBox_Name_selectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Template_toolStripComboBox_Name.Text))
                {
                    SetEditButtonStatus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("执行失败，返回错误：" + ex.Message);
                InitButtonStatus();
            }
        }

        private void Template_toolStripButton_New_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(Template_toolStripComboBox_Name.Text)) throw new Exception("模版名称不能为空！");
                if (LabelArray.FindAll(T => { return T.LabelName.Equals(Template_toolStripComboBox_Name.Text); }).Count > 0) throw new Exception("当前模版本名称已经存在，请重新命名！");
                SetEditButtonStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("执行失败，返回错误：" + ex.Message);
                InitButtonStatus();
            }
        }

        private void Template_toolStripButton_Cancel_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show("执行错误，返回信息：" + ex.Message);
            }
            finally
            {
                InitButtonStatus();
            }
        }

        private void Template_toolStripButton_Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(Template_toolStripComboBox_Name.Text)) throw new Exception("模块名称不能为空！");
                if (EditLabelTemplate.LabelFields.Count <= 0) throw new Exception("图像队列中没有任何图形存在！");

                EditLabelTemplate.LabelName = Template_toolStripComboBox_Name.Text;
                LabelTemplateModel TempTemplate = new LabelTemplateModel();
                TempTemplate.LabelWidth = EditLabelTemplate.LabelWidth;
                TempTemplate.LabelLength = EditLabelTemplate.LabelLength;
                TempTemplate.LabelName = EditLabelTemplate.LabelName;
                EditLabelTemplate.LabelFields.ForEach(T =>
                {
                    var T_Properties = T.GetType().GetProperties();
                    LabelFieldModel TempField = new LabelFieldModel();
                    var TempField_Properties = TempField.GetType().GetProperties();
                    for (int Col = 0; Col < T_Properties.Length; Col++)
                    {
                        TempField_Properties[Col].SetValue(TempField, T_Properties[Col].GetValue(T));
                    }
                    TempField.ObjectType = T.ObjectType;
                    TempField.ObjectValueType = T.ObjectValueType;
                    TempTemplate.LabelFields.Add(TempField);
                });

                int Index = LabelArray.FindIndex(T => { return T.LabelName.Equals(EditLabelTemplate.LabelName); });
                if (Index<0)
                {
                    LabelArray.Add(TempTemplate);
                    Template_toolStripComboBox_Name.Items.Add(TempTemplate.LabelName);
                    TemplateNames.Add(TempTemplate.LabelName);
                }
                else
                {
                    LabelArray[Index] = TempTemplate;
                }
                MessageBox.Show("保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show("执行错误，返回信息：" + ex.Message);
            }
        }

        private void Template_toolStripButton_DeleteTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(Template_toolStripComboBox_Name.Text)) throw new Exception("模版名称不能为空！");
                LabelTemplateModel TempTemplate = LabelArray.Find(T => { return T.LabelName.Equals(Template_toolStripComboBox_Name.Text); });
                if (TempTemplate != null)
                {
                    LabelArray.Remove(TempTemplate);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("执行错误，返回信息：" + ex.Message);
            }
            finally
            {
                InitButtonStatus();
            }
        }

        private void Template_toolStripButton_TestPrint_Click(object sender, EventArgs e)
        {
            try
            {
                /*StringBuilder PrintDoc = new StringBuilder();
                PrintDoc.AppendLine("CT~~CD,~CC^~CT~");
                PrintDoc.AppendLine("^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR4,4~SD25^JUS^LRN^CI0^XZ");
                PrintDoc.AppendLine("^XA");
                PrintDoc.AppendLine("*InsertLine*");
                PrintDoc.AppendLine("^PQ*PrintCount*,0,1,Y^XZ");

                Bitmap b = InitDrawBitmap();

                int TotalBytes = 0, RowBytes = 0;
                string Temp = ZebraUnity.BitmapToHex(b, out TotalBytes, out RowBytes);
                Temp = string.Format("~DGR:Temp0.GRF,{0},{1},{2}", TotalBytes, RowBytes, Temp);
                Temp += "^FO0,0^XGR:Temp0.GRF,1,1^FS";
                Temp = PrintDoc.ToString().Replace("*InsertLine*", Temp);
                Temp = Temp.Replace("*PrintCount*", "1");
                if (!MyZebra.PrintWithDrv(Temp, "ZDesigner 105SL 203DPI")) throw new Exception("error");
*/
                if (printDialog1.ShowDialog() == DialogResult.OK)
                {
                    string Printname = printDialog1.PrinterSettings.PrinterName;
                    if (Printname.IndexOf("ZDesigner") >= 0)
                    {
                        StringBuilder PrintDoc = new StringBuilder();
                        PrintDoc.AppendLine("CT~~CD,~CC^~CT~");
                        PrintDoc.AppendLine("^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR4,4~SD25^JUS^LRN^CI0^XZ");
                        PrintDoc.AppendLine("^XA");
                        PrintDoc.AppendLine("*InsertLine*");
                        PrintDoc.AppendLine("^PQ*PrintCount*,0,1,Y^XZ");

                        float Scalex = (comboBox_DpiX.SelectedItem == null ? 96 : int.Parse(comboBox_DpiX.SelectedItem.ToString())) / 96;
                        int LabelWidth = (int)(Helper.Convert.MillimetersToPixelsWidth(ScreenDpiX, EditLabelTemplate.LabelWidth * 10) * Scalex);
                        int LabelHeight = (int)(Helper.Convert.MillimetersToPixelsWidth(ScreenDpiX, EditLabelTemplate.LabelLength * 10) * Scalex);
                        Bitmap b = EditLabelTemplate.InitPrintBitmap(LabelWidth, LabelHeight, Scalex);

                        int TotalBytes = 0, RowBytes = 0;
                        string Temp = ZebraUnity.BitmapToHex(b, out TotalBytes, out RowBytes);
                        Temp = string.Format("~DGR:Temp0.GRF,{0},{1},{2}", TotalBytes, RowBytes, Temp);
                        Temp += "^FO0,0^XGR:Temp0.GRF,1,1^FS";
                        Temp = PrintDoc.ToString().Replace("*InsertLine*", Temp);
                        Temp = Temp.Replace("*PrintCount*", "1");
                        if (!MyZebra.PrintWithDrv(Temp, Printname)) throw new Exception("打印失败！");
                    }
                    else
                    {
                        printDocument1.PrinterSettings = printDialog1.PrinterSettings;
                        printDocument1.Print();
                    }
                    MessageBox.Show("打印完成！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("执行失败，返回错误：" + ex.Message);
            }
        }

        private void Template_numericUpDown_LabelSize_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (InitFinished) return;
                EditLabelTemplate.LabelWidth = (double)Template_numericUpDown_LabelWidth.Value;
                EditLabelTemplate.LabelLength = (double)Template_numericUpDown_LabelLength.Value;
                Template_pictureBox_Preview.Width = (int)(Helper.Convert.MillimetersToPixelsWidth(comboBox_DpiX.SelectedItem == null ? 96 : int.Parse(comboBox_DpiX.SelectedItem.ToString()), EditLabelTemplate.LabelWidth * 10));
                Template_pictureBox_Preview.Height = (int)(Helper.Convert.MillimetersToPixelsWidth(comboBox_DpiX.SelectedItem == null ? 96 : int.Parse(comboBox_DpiX.SelectedItem.ToString()), EditLabelTemplate.LabelLength * 10));
            }
            catch (Exception ex)
            {
                MessageBox.Show("执行失败，返回错误：" + ex.Message);
                InitButtonStatus();
            }
        }

        #region 画图

        private void Template_button_Redraw_Click(object sender, EventArgs e)
        {
            if (EditLabelTemplate != null) Redraw();
        }

        private void Template_toolStripButton_AddObject_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripButton ToolButton = (ToolStripButton)sender;
                LabelFieldModel LabelField = new LabelFieldModel();
                LabelField.Line = EditLabelTemplate.LabelFields.Count + 1;
                LabelField.FieldObjectType = ToolButton.Text;
                LabelField.ObjectType = (DrawObjectType)Enum.Parse(typeof(DrawObjectType), ToolButton.Text);
                LabelField.ObjectValueType = DrawObjectValueType.Static;
                LabelField.FieldObjectValueType = DrawObjectValueType.Static.ToString();
                LabelField.FontName = "宋体";
                LabelField.FontSize = 20;
                LabelField.Angle = 0;
                switch (LabelField.ObjectType)
                {
                    case DrawObjectType.Text:
                        LabelField.PrintValue = "TEST";
                        break;
                    case DrawObjectType.Date:
                        LabelField.PrintValue = DateTime.Now.ToString();
                        break;
                    case DrawObjectType.Number:
                        LabelField.PrintValue = "9999";
                        break;
                    case DrawObjectType.Matrix:
                    case DrawObjectType.Barcode:
                    case DrawObjectType.QRCode:
                        LabelField.PrintValue = "TEST";
                        LabelField.Width = (int)Helper.Convert.MillimetersToPixelsWidth(ScreenDpiX, 5 * 10); //5厘米
                        LabelField.Length = (int)Helper.Convert.MillimetersToPixelsWidth(ScreenDpiX, 5 * 10); //5厘米
                        LabelField.ScaleHeight = (int)Helper.Convert.MillimetersToPixelsWidth(ScreenDpiX, 5 * 10); //5厘米
                        LabelField.ScaleWidth = (int)Helper.Convert.MillimetersToPixelsWidth(ScreenDpiX, 5 * 10); //5厘米
                        LabelField.FontSize = (int)Helper.Convert.MillimetersToPixelsWidth(ScreenDpiX, 5 * 10); //5厘米
                        break;
                    default:
                        throw new Exception("选择的图形对象出现异常！");
                }
                CurrentMousePoint = Point.Empty;
                EditLabelTemplate.LabelFields.Add(LabelField);
                Template_dataGridView.DataSource = new BindingList<LabelFieldModel>(EditLabelTemplate.LabelFields);
                for(int Index = 0; Index < Template_dataGridView.Rows.Count; Index++) { Template_dataGridView.Rows[Index].Selected = false; }
                Template_dataGridView.Rows[LabelField.Line - 1].Selected = true;
                MouseOperationStatus = MouseOperation.Paint;
                EditLabelFields.Clear();
                EditLabelFields.Add(LabelField);
            }
            catch (Exception ex)
            {
                MessageBox.Show("执行失败，返回错误：" + ex.Message);
            }
        }


        private void Template_pictureBox_Preview_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                Template_toolStripTextBox_MouseX.Text = e.X.ToString();
                Template_toolStripTextBox_MouseY.Text = e.Y.ToString();
                if (EditLabelTemplate != null)
                {
                    if (MouseOperationStatus == MouseOperation.Idel)
                    {
                        CurrentMousePoint = new Point(e.X, e.Y);
                    }
                    else if (MouseOperationStatus == MouseOperation.Paint)
                    {
                        if (EditLabelFields.Count != 1) throw new Exception("等待绘制的图像队列数量不能大于1！");
                        if (CurrentMousePoint == Point.Empty)
                        {
                            EditLabelFields[0].Left = e.X;
                            EditLabelFields[0].Top = e.Y;
                        }
                        else
                        {
                            EditLabelFields[0].Left += e.X - CurrentMousePoint.X;
                            EditLabelFields[0].Top += e.Y - CurrentMousePoint.Y;
                        }
                        CurrentMousePoint = new Point(e.X, e.Y);
                    }
                    else if (MouseOperationStatus == MouseOperation.Drag)
                    {
                        if (EditLabelFields.Count <= 0) throw new Exception("当前编辑队列为空，不正确！");
                        EditLabelFields.ForEach(T =>
                        {
                            if (CurrentMousePoint == Point.Empty)
                            {
                                T.Left = e.X;
                                T.Top = e.Y;
                            }
                            else
                            {
                                T.Left += e.X - CurrentMousePoint.X;
                                T.Top += e.Y - CurrentMousePoint.Y;
                            }
                            //T.Left = e.X - T.Width/2;
                            //T.Top = e.Y - T.Length/2;
                        });
                        CurrentMousePoint = new Point(e.X, e.Y);
                    }
                    else if (MouseOperationStatus == MouseOperation.Selecting)
                    {
                        SelectPoint = new Point(e.X, e.Y);
                    }
                    else if (MouseOperationStatus == MouseOperation.Selected)
                    {
                        bool Sign = false;
                        EditLabelFields.ForEach(T =>
                        {
                            for (int Index = 0; Index < T.HandleRectangle.Count; Index++)
                            {
                                if (T.HandleRectangle[Index].Contains(e.X, e.Y))
                                {
                                    if (Index == 0 || Index == 1) this.Cursor = Cursors.SizeNS;
                                    else this.Cursor = Cursors.SizeWE;
                                    Sign = true;
                                    return;
                                }
                            }
                        });
                        if (!Sign) this.Cursor = Cursors.Default;
                    }
                    else if (MouseOperationStatus == MouseOperation.Scale)
                    {
                        if (ScaleLabelField != null && ScaleHandleIndex >= 0)
                        {
                            int Index = ScaleHandleIndex;
                            Rectangle T = ScaleLabelField.HandleRectangle[Index];
                            if (Index == 0) //上
                            {
                                ScaleLabelField.HandleRectangle[Index] = new Rectangle(new Point(T.X, T.Y + e.Y - CurrentMousePoint.Y), new Size(LabelTemplateModel.HandleLength, LabelTemplateModel.HandleLength));
                                ScaleLabelField.Top += e.Y - CurrentMousePoint.Y;
                                ScaleLabelField.ScaleHeight += -(e.Y - CurrentMousePoint.Y);
                                if (ScaleLabelField.ObjectType == DrawObjectType.Matrix || ScaleLabelField.ObjectType == DrawObjectType.QRCode) ScaleLabelField.ScaleWidth = ScaleLabelField.ScaleHeight;
                            }
                            else if (Index == 1) //下
                            {
                                ScaleLabelField.HandleRectangle[Index] = new Rectangle(new Point(T.X, T.Y + e.Y - CurrentMousePoint.Y), new Size(LabelTemplateModel.HandleLength, LabelTemplateModel.HandleLength));
                                //ScaleLabelField.Top += e.Y - CurrentMousePoint.Y;
                                ScaleLabelField.ScaleHeight += e.Y - CurrentMousePoint.Y;
                                if (ScaleLabelField.ObjectType == DrawObjectType.Matrix || ScaleLabelField.ObjectType == DrawObjectType.QRCode) ScaleLabelField.ScaleWidth = ScaleLabelField.ScaleHeight;
                            }
                            else if (Index == 2) //左
                            {
                                ScaleLabelField.HandleRectangle[Index] = new Rectangle(new Point(T.X + e.X - CurrentMousePoint.X, T.Y), new Size(LabelTemplateModel.HandleLength, LabelTemplateModel.HandleLength));
                                ScaleLabelField.Left += e.X - CurrentMousePoint.X;
                                ScaleLabelField.ScaleWidth += -(e.X - CurrentMousePoint.X);
                                if (ScaleLabelField.ObjectType == DrawObjectType.Matrix || ScaleLabelField.ObjectType == DrawObjectType.QRCode) ScaleLabelField.ScaleHeight = ScaleLabelField.ScaleWidth;
                            }
                            else if (Index == 3) //右
                            {
                                ScaleLabelField.HandleRectangle[Index] = new Rectangle(new Point(T.X + e.X - CurrentMousePoint.X, T.Y), new Size(LabelTemplateModel.HandleLength, LabelTemplateModel.HandleLength));
                                ScaleLabelField.ScaleWidth += e.X - CurrentMousePoint.X;
                                if (ScaleLabelField.ObjectType == DrawObjectType.Matrix || ScaleLabelField.ObjectType == DrawObjectType.QRCode) ScaleLabelField.ScaleHeight = ScaleLabelField.ScaleWidth;
                            }
                            //if (ScaleLabelField.ObjectType == DrawObjectType.Matrix || ScaleLabelField.ObjectType == DrawObjectType.QRCode) ScaleLabelField.ScaleWidth = ScaleLabelField.ScaleHeight;
                        }
                        CurrentMousePoint = new Point(e.X, e.Y);
                    }
                }
            }
            catch (Exception ex)
            {
                EditLabelFields.Clear();
                CurrentMousePoint = Point.Empty;
                MouseOperationStatus = MouseOperation.Idel;
                MessageBox.Show("执行失败，返回错误：" + ex.Message);
            }
            finally
            {
                Redraw();
                EditLabelTemplate.LabelFields.ForEach(T =>
                { if (EditLabelFields.Find(L => { return L.Line == T.Line; }) != null) { Template_dataGridView.Rows[T.Line - 1].Selected = true; } else { Template_dataGridView.Rows[T.Line - 1].Selected = false; } });
            }
        }

        private void Template_pictureBox_Preview_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (MouseOperationStatus == MouseOperation.Idel)
                    {
                        EditLabelFields.Clear();
                        EditLabelTemplate.LabelFields.ForEach(T =>
                        {
                            if (T.Rectangle.Contains(e.X, e.Y)) EditLabelFields.Add(T);
                        });
                        if (EditLabelFields.Count > 0)
                        {
                            CurrentMousePoint = new Point(e.X, e.Y);
                            MouseOperationStatus = MouseOperation.Drag;
                        }
                        else if (EditLabelFields.Count <= 0)
                        {
                            CurrentMousePoint = new Point(e.X, e.Y);
                            SelectPoint = new Point(e.X, e.Y);
                            MouseOperationStatus = MouseOperation.Selecting;
                        }
                    }
                    else if (MouseOperationStatus == MouseOperation.Paint)
                    {

                    }
                    else if (MouseOperationStatus == MouseOperation.Selecting)
                    {
                    }
                    else if (MouseOperationStatus == MouseOperation.Selected)
                    {
                        if (EditLabelFields.Find(T => { return T.Rectangle.Contains(e.X, e.Y); }) != null)
                        {
                            MouseOperationStatus = MouseOperation.Drag;
                            CurrentMousePoint = new Point(e.X, e.Y);
                        }
                        else
                        {
                            bool Sign = false;
                            EditLabelFields.ForEach(T =>
                            {
                                for (int Index = 0; Index < T.HandleRectangle.Count; Index++)
                                {
                                    if (T.HandleRectangle[Index].Contains(e.X, e.Y))
                                    {
                                        ScaleLabelField = T;
                                        MouseOperationStatus = MouseOperation.Scale;
                                        ScaleHandleIndex = Index;
                                        CurrentMousePoint = new Point(e.X, e.Y);
                                        Sign = true;
                                        return;
                                    }
                                }
                            });
                            if (!Sign)
                            {
                                MouseOperationStatus = MouseOperation.Idel;
                                CurrentMousePoint = Point.Empty;
                                EditLabelFields.Clear();
                            }
                        }
                    }
                    else if (MouseOperationStatus == MouseOperation.ReadyDrag)
                    {
                        if (EditLabelFields.Find(T => { return T.Rectangle.Contains(e.X, e.Y); }) != null)
                        {
                            MouseOperationStatus = MouseOperation.Drag; 
                            CurrentMousePoint = new Point(e.X, e.Y);
                        }
                        else
                        {
                            EditLabelFields.Clear();
                            CurrentMousePoint = Point.Empty;
                            MouseOperationStatus = MouseOperation.Idel;
                        }
                    }
                    else if (MouseOperationStatus == MouseOperation.Drag)
                    {

                    }
                    else if(MouseOperationStatus==MouseOperation.Copy)
                    {

                    }
                }
                else if (e.Button == MouseButtons.Right)
                {

                }
            }
            catch (Exception ex)
            {
                EditLabelFields.Clear();
                CurrentMousePoint = Point.Empty;
                MouseOperationStatus = MouseOperation.Idel;
                MessageBox.Show("执行失败，返回错误：" + ex.Message);
            }
            finally
            {
                Redraw();
                EditLabelTemplate.LabelFields.ForEach(T =>
                { if (EditLabelFields.Find(L => { return L.Line == T.Line; }) != null) { Template_dataGridView.Rows[T.Line - 1].Selected = true; } else { Template_dataGridView.Rows[T.Line - 1].Selected = false; } });
            }
        }

        private void Template_pictureBox_Preview_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if(e.Button == MouseButtons.Left)
                {
                    if (MouseOperationStatus == MouseOperation.Idel)
                    {
                        if (EditLabelFields.Count > 0) MouseOperationStatus = MouseOperation.Selected;
                    }
                    else if (MouseOperationStatus == MouseOperation.Paint)
                    {
                        MouseOperationStatus = MouseOperation.Idel;
                    }
                    else if (MouseOperationStatus == MouseOperation.Drag)
                    {

                    }
                    else if (MouseOperationStatus == MouseOperation.Selecting)
                    {
                        EditLabelFields.Clear();
                        EditLabelTemplate.LabelFields.ForEach(T => {
                            if (new Rectangle(CurrentMousePoint.X, CurrentMousePoint.Y, e.X - CurrentMousePoint.X, e.Y - CurrentMousePoint.Y).Contains(T.Rectangle)) EditLabelFields.Add(T);
                        });
                        if (EditLabelFields.Count > 0) MouseOperationStatus = MouseOperation.Selected; else MouseOperationStatus = MouseOperation.Idel;
                    }
                    else if (MouseOperationStatus == MouseOperation.Scale)
                    {
                        ScaleLabelField = null;
                        ScaleHandleIndex = -1;
                        MouseOperationStatus = MouseOperation.Selected;
                    }
                }
            }
            catch (Exception ex)
            {
                EditLabelFields.Clear();
                CurrentMousePoint = Point.Empty;
                MouseOperationStatus = MouseOperation.Idel;
                MessageBox.Show("执行失败，返回错误：" + ex.Message);
            }
            finally
            {
                Redraw();
                EditLabelTemplate.LabelFields.ForEach(T =>
                { if (EditLabelFields.Find(L => { return L.Line == T.Line; }) != null) { Template_dataGridView.Rows[T.Line - 1].Selected = true; } else { Template_dataGridView.Rows[T.Line - 1].Selected = false; } });
            }
        }

        private void Template_pictureBox_Preview_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (MouseOperationStatus == MouseOperation.Drag)
                    {
                        //EditLabelFields.Clear();
                        CurrentMousePoint = Point.Empty;
                        MouseOperationStatus = MouseOperation.Idel;
                    }
                    else if (MouseOperationStatus == MouseOperation.Idel)
                    {
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (MouseOperationStatus == MouseOperation.Copy)
                    {
                        CurrentMousePoint = new Point(e.X, e.Y);
                        contextMenuStrip1.Show(new Point(MousePosition.X, MousePosition.Y));
                    }
                    else
                    {
                        LabelFieldModel LabelField = EditLabelTemplate.LabelFields.Find(T => { return T.Rectangle.Contains(e.X, e.Y); });
                        if (LabelField != null)
                        {
                            if (EditLabelFields.Find(T => { return T.Line == LabelField.Line; }) == null) EditLabelFields.Add(LabelField);
                            foreach(ToolStripMenuItem Item in Template_ContextMenu_toolStripMenuItem_Angel.DropDownItems)
                            {
                                if (Item.Text.IndexOf("旋转" + LabelField.Angle.ToString() + "度") >= 0) { Item.CheckState = CheckState.Checked; Item.Enabled = false; } else { Item.CheckState = CheckState.Unchecked; Item.Enabled = true; }
                            }
                            contextMenuStrip1.Show(new Point(MousePosition.X, MousePosition.Y));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EditLabelFields.Clear();
                CurrentMousePoint = Point.Empty;
                MouseOperationStatus = MouseOperation.Idel;
                MessageBox.Show("执行失败，返回错误：" + ex.Message);
            }
            finally
            {
                Redraw();
                EditLabelTemplate.LabelFields.ForEach(T =>
                { if (EditLabelFields.Find(L => { return L.Line == T.Line; }) != null) { Template_dataGridView.Rows[T.Line - 1].Selected = true; } else { Template_dataGridView.Rows[T.Line - 1].Selected = false; } });
            }
        }

        private void Redraw()
        {
            try
            {
                Bitmap b = InitDrawBitmap();
                Graphics g = Template_pictureBox_Preview.CreateGraphics();
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(b, 0, 0);
                b.Dispose();
                g.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("执行失败，返回错误：" + ex.Message);
            }
        }

        private Bitmap InitDrawBitmap()
        {
            try
            {
                int LabelWidth= (int)Helper.Convert.MillimetersToPixelsWidth(comboBox_DpiX.SelectedItem == null ? 96 : int.Parse(comboBox_DpiX.SelectedItem.ToString()), EditLabelTemplate.LabelWidth * 10 + 1);
                int LabelHeight= (int)Helper.Convert.MillimetersToPixelsWidth(comboBox_DpiX.SelectedItem == null ? 96 : int.Parse(comboBox_DpiX.SelectedItem.ToString()), EditLabelTemplate.LabelLength * 10 + 1);
                Bitmap b = new Bitmap(LabelWidth, LabelHeight);
                Graphics theGraphics = Graphics.FromImage((System.Drawing.Image)b);
                theGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                theGraphics.Clear(Color.White);
                Brush bush = new SolidBrush(System.Drawing.Color.Black);
                theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                Pen HoverPen = new Pen(System.Drawing.Color.Red, 1);
                HoverPen.DashStyle = DashStyle.Custom;
                HoverPen.DashPattern = new float[] { 4f, 6f };
                Pen DragPen = new Pen(new SolidBrush(System.Drawing.Color.Red));
                Pen HandlePen = new Pen(new SolidBrush(System.Drawing.Color.Black));
                foreach (LabelFieldModel LabelField in EditLabelTemplate.LabelFields)
                {
                    string PrintValue = LabelField.PrintValue;
                    Font myFont = new System.Drawing.Font(LabelField.FontName, LabelField.FontSize<=0?1:LabelField.FontSize, FontStyle.Regular);
                    switch (LabelField.ObjectType)
                    {
                        case DrawObjectType.Text:
                        case DrawObjectType.Date:
                        case DrawObjectType.Number:
                            if (LabelField.ObjectType == DrawObjectType.Date)
                            {
                                if (LabelField.ObjectValueType == DrawObjectValueType.Dynamic) try { PrintValue = DateTime.Now.ToString(LabelField.PrintValue); } catch (Exception) { PrintValue = "ERROR"; }
                                else if (LabelField.ObjectValueType == DrawObjectValueType.Regular) try { PrintValue = DateTime.Now.ToString(LabelField.PrintValue); } catch (Exception) { PrintValue = "ERROR"; }
                            }
                            else if (LabelField.ObjectType == DrawObjectType.Text && LabelField.ObjectValueType == DrawObjectValueType.Regular)
                            {

                            }
                            SizeF sf = theGraphics.MeasureString(PrintValue, myFont);
                            LabelTemplateModel.ConvertCoordinate(LabelField, (int)sf.Width, (int)sf.Height);
                            if (LabelField.Angle > 0)
                            {
                                Matrix matrix = theGraphics.Transform;
                                matrix.RotateAt(LabelField.Angle, new PointF(LabelField.Center.X, LabelField.Center.Y));
                                theGraphics.Transform = matrix;
                                LabelTemplateModel.DrawStringToImage(theGraphics, LabelField, PrintValue, myFont, bush, sf);
                                theGraphics.ResetTransform();
                                //if (LabelField.Angle == 90||LabelField.Angle==270) LabelField.Rectangle = new Rectangle((int)(LabelField.Center.X - sf.Height / 2), (int)(LabelField.Center.Y - sf.Width / 2), (int)sf.Height, (int)sf.Width);
                            }
                            else LabelTemplateModel.DrawStringToImage(theGraphics, LabelField, PrintValue, myFont, bush, sf);
                            break;
                        case DrawObjectType.Matrix:
                        case DrawObjectType.Barcode:
                        case DrawObjectType.QRCode:
                            Bitmap CodePic = null;
                            if (string.IsNullOrEmpty(PrintValue)) PrintValue = "ERROR";
                            if (LabelField.ObjectType == DrawObjectType.Matrix) CodePic = LabelTemplateModel.GenerateMatrixCode(PrintValue, (int)LabelField.Length, (int)LabelField.Width);
                            else if (LabelField.ObjectType == DrawObjectType.Barcode) CodePic = LabelTemplateModel.Generate128Code(PrintValue, (int)LabelField.Length, (int)LabelField.Width);
                            else if (LabelField.ObjectType == DrawObjectType.QRCode) CodePic = LabelTemplateModel.GenerateQRCode(PrintValue, (int)LabelField.Length, (int)LabelField.Width);
                            if (CodePic == null) throw new Exception("图像代码生成失败！");
                            LabelTemplateModel.ConvertCoordinate(LabelField, (int)CodePic.Width, (int)CodePic.Height);
                            if (LabelField.Angle > 0)
                            {
                                Matrix matrix = theGraphics.Transform;
                                matrix.RotateAt(LabelField.Angle, new PointF(LabelField.Center.X, LabelField.Center.Y));
                                theGraphics.Transform = matrix;
                                theGraphics.DrawImage(CodePic, LabelField.Rectangle);
                                theGraphics.ResetTransform();
                                //if (LabelField.Angle == 90 || LabelField.Angle == 270) LabelField.Rectangle = new Rectangle((int)(LabelField.Center.X - LabelField.ScaleHeight / 2), (int)(LabelField.Center.Y - LabelField.ScaleWidth / 2), (int)LabelField.ScaleHeight, (int)LabelField.ScaleWidth);
                            }
                            else theGraphics.DrawImage(CodePic, LabelField.Rectangle);
                            break;
                        default:
                            throw new Exception("未知图形对象！");
                    }
                    if (EditLabelFields.Find(T => { return T.Line == LabelField.Line; }) != null)
                    {
                        theGraphics.DrawRectangle(DragPen, LabelField.Rectangle);
                        if (EditLabelFields.Count == 1)
                        {
                            theGraphics.DrawRectangles(HandlePen, EditLabelFields[0].HandleRectangle.ToArray());
                        }
                    }
                    if (MouseOperationStatus == MouseOperation.Drag) { }
                    else if (MouseOperationStatus == MouseOperation.Idel)
                    {
                        if (LabelField.Rectangle.Contains(CurrentMousePoint)) theGraphics.DrawRectangle(HoverPen, LabelField.Rectangle);
                    }
                }
                if (MouseOperationStatus == MouseOperation.Selecting)
                {
                    theGraphics.DrawRectangle(DragPen, CurrentMousePoint.X > SelectPoint.X ? SelectPoint.X : CurrentMousePoint.X, CurrentMousePoint.Y > SelectPoint.Y ? SelectPoint.Y : CurrentMousePoint.Y, Math.Abs(SelectPoint.X - CurrentMousePoint.X), Math.Abs(SelectPoint.Y - CurrentMousePoint.Y));
                }
                theGraphics.Dispose();
                return b;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Template_pictureBox_Preview_Paint(object sender, PaintEventArgs e)
        {
            if (EditLabelTemplate != null) Redraw();
        }

        #endregion

        private void Template_dataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                EditLabelFields.Clear();
                MouseOperationStatus = MouseOperation.Idel;
                DataGridViewCell TemplateID = Template_dataGridView.Rows[e.RowIndex].Cells["Template_ID"];
                DataGridViewCell cell = Template_dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (cell == null || TemplateID == null) throw new Exception("定位单元格错误！");
                string TempValue = cell == null || cell.Value == null ? "" : cell.Value.ToString();
                Rectangle rect = Template_dataGridView.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                
                EditLabelFields.Add(EditLabelTemplate.LabelFields.Find(T => { return T.Line == int.Parse(TemplateID.Value.ToString()); }));
                if (EditLabelFields.Count != 1) throw new Exception("定位图形对象失败！");

                EditSign = Template_dataGridView.Columns[e.ColumnIndex].HeaderText;
                if (!string.IsNullOrEmpty(EditSign))
                {
                    switch (EditSign)
                    {
                        case "类型":
                            break;
                        case "标识":
                            break;
                        case "值类型":
                            TempCombobox.Visible = true;
                            TempCombobox.Items.Clear();
                            TempCombobox.Location = rect.Location; TempCombobox.Size = rect.Size; TempCombobox.SelectedIndex = -1;
                            if (EditLabelFields[0].ObjectType == DrawObjectType.Date) TempCombobox.Items.AddRange(new object[] { "Dynamic", "Static", "Regular" });
                            else TempCombobox.Items.AddRange(new object[] { "Static", "Variable" });
                            if (string.IsNullOrEmpty(TempValue))
                                TempCombobox.Text = "静态";
                            else
                                TempCombobox.Text = TempValue;
                            break;
                        case "打印值":
                            switch (EditLabelFields[0].ObjectType)
                            {
                                case DrawObjectType.Text:
                                    TempTextBox.Location = rect.Location; TempTextBox.Size = rect.Size; TempTextBox.Text = TempValue; TempTextBox.Visible = true;
                                    break;
                                case DrawObjectType.Date:
                                    if (EditLabelFields[0].ObjectValueType == DrawObjectValueType.Static || EditLabelFields[0].ObjectValueType == DrawObjectValueType.Dynamic) { TempTextBox.Location = rect.Location; TempTextBox.Size = rect.Size; TempTextBox.Text = TempValue; TempTextBox.Visible = true; }
                                    else
                                    {
                                        TempCombobox.Visible = true;
                                        TempCombobox.Items.Clear();
                                        TempCombobox.Location = rect.Location; TempCombobox.Size = rect.Size; TempCombobox.SelectedIndex = -1;
                                        TempCombobox.Items.AddRange(new object[] { "yyyy-MM-dd", "yyyy/MM/dd", "yyyyMMdd", "yyMMdd", "yy-M-d", "yy/M/d", "hh:mm:ss", "hhmmss", "HH:mm:ss", "HHmmss", "h:m:s", "H:m:s" });
                                        if (string.IsNullOrEmpty(TempValue)) TempCombobox.Text = "yyyy-MM-dd"; else TempCombobox.Text = TempValue;
                                    }
                                    break;
                                case DrawObjectType.Number:
                                    TempNumericUpDown.Location = rect.Location; TempNumericUpDown.Size = rect.Size; int Temp2 = 0; int.TryParse(String.IsNullOrEmpty(TempValue) ? "0" : TempValue, out Temp2);
                                    TempNumericUpDown.Value = Temp2; TempNumericUpDown.Visible = true;
                                    break;
                                case DrawObjectType.Matrix:
                                case DrawObjectType.Barcode:
                                case DrawObjectType.QRCode:
                                    TempTextBox.Location = rect.Location; TempTextBox.Size = rect.Size; TempTextBox.Text = TempValue; TempTextBox.Visible = true;
                                    break;
                            }
                            break;
                        case "旋转":
                            TempCombobox.Items.Clear();
                            TempCombobox.Location = rect.Location; TempCombobox.Size = rect.Size; TempCombobox.SelectedIndex = -1;
                            TempCombobox.Items.AddRange(new object[] { 0, 90, 180, 270 });
                            if (string.IsNullOrEmpty(TempValue)) TempCombobox.Text = "0"; else TempCombobox.Text = TempValue;
                            TempCombobox.Visible = true;
                            break;
                        case "字体":
                            if (EditLabelFields[0].ObjectType == DrawObjectType.Matrix || EditLabelFields[0].ObjectType == DrawObjectType.Barcode || EditLabelFields[0].ObjectType == DrawObjectType.QRCode) throw new Exception("图型对像无法修改字体参数！");
                            else
                            {
                                TempCombobox.Items.Clear();
                                TempCombobox.Location = rect.Location; TempCombobox.Size = rect.Size; TempCombobox.SelectedIndex = -1;
                                TempCombobox.Items.AddRange(new object[] { "宋体","黑体" });
                                if (string.IsNullOrEmpty(TempValue)) TempCombobox.Text = "宋体"; else TempCombobox.Text = TempValue;
                                TempCombobox.Visible = true;
                            }
                            break;
                        case "字体大小":
                            //if (EditLabelFields[0].ObjectType == DrawObjectType.Matrix || EditLabelFields[0].ObjectType == DrawObjectType.Barcode || EditLabelFields[0].ObjectType == DrawObjectType.QRCode) throw new Exception("无权修改此项数据！");
                            TempNumericUpDown.Location = rect.Location; TempNumericUpDown.Size = rect.Size; int Temp = 0; int.TryParse(String.IsNullOrEmpty(TempValue) ? "0" : TempValue, out Temp);
                            TempNumericUpDown.Value = Temp; TempNumericUpDown.Visible = true;
                            break;
                        default:
                            throw new Exception("无权修改此项数据！");
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("执行失败，返回错误：" + ex.Message);
                EditSign = "";
                EditLabelFields.Clear();
                CurrentMousePoint = Point.Empty;
                MouseOperationStatus = MouseOperation.Idel;
            }
        }

        private void Template_dataGridView_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string HeaderText = Template_dataGridView.Columns[e.ColumnIndex].HeaderText;
                DataGridViewCell cell = Template_dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (TempCombobox.Visible) { cell.Value = TempCombobox.Text; TempCombobox.Visible = false; }
                if (TempNumericUpDown.Visible) { cell.Value = TempNumericUpDown.Value; TempNumericUpDown.Visible = false; }
                if (TempTextBox.Visible) { cell.Value = TempTextBox.Text; TempTextBox.Visible = false; }
            }
            catch (Exception ex)
            {
                MessageBox.Show("执行失败，返回错误：" + ex.Message);
            }
            finally
            {
                EditSign = "";
                EditLabelFields.Clear();
                CurrentMousePoint = Point.Empty;
                MouseOperationStatus = MouseOperation.Idel;
            }
        }

        private void Template_DataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                EditLabelFields.Clear();
                CurrentMousePoint = Point.Empty;
                DataGridViewCell TemplateID = Template_dataGridView.Rows[e.RowIndex].Cells["Template_ID"];
                LabelFieldModel LabelField = EditLabelTemplate.LabelFields.Find(T => { return T.Line == int.Parse(TemplateID.Value.ToString()); });
                if (LabelField == null) throw new Exception("定位记录失败！");
                EditLabelFields.Add(LabelField);
                MouseOperationStatus = MouseOperation.Drag;
            }
            catch (Exception ex)
            {
                EditSign = "";
                EditLabelFields.Clear();
                CurrentMousePoint = Point.Empty;
                MouseOperationStatus = MouseOperation.Idel;
                MessageBox.Show("执行失败，返回错误：" + ex.Message);
            }
            finally { Redraw(); }
        }

        private void Template_dataGridView_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                if (TempTextBox.Visible) TempTextBox.Visible = false;
                if (TempCombobox.Visible) TempCombobox.Visible = false;
                if (TempNumericUpDown.Visible) TempNumericUpDown.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("执行失败，返回错误：" + ex.Message);
            }
            finally
            {
                EditSign = "";
                EditLabelFields.Clear();
                CurrentMousePoint = Point.Empty;
                MouseOperationStatus = MouseOperation.Idel;
            }
        }

        private void Template_ContextMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TempCombobox.Visible = false;
                TempTextBox.Visible = false;
                TempNumericUpDown.Visible = false;
                ToolStripMenuItem Item = (ToolStripMenuItem)sender;
                switch (Item.Text)
                {
                    case "旋转0度":
                        EditLabelFields.ForEach(T => { T.Angle = 0; });
                        break;
                    case "旋转90度":
                        EditLabelFields.ForEach(T => { T.Angle = 90; });
                        break;
                    case "旋转180度":
                        EditLabelFields.ForEach(T => { T.Angle = 180; });
                        break;
                    case "复制":
                        MouseOperationStatus = MouseOperation.Copy;
                        CopyLabelFields.Clear();
                        EditLabelFields.ForEach(T => { CopyLabelFields.Add(T); });
                        EditLabelFields.Clear();
                        break;
                    case "删除":
                        EditLabelTemplate.LabelFields.RemoveAll(T => { return EditLabelFields.Find(F => { return F.Line == T.Line; }) != null; });
                        int Line = 1;
                        EditLabelTemplate.LabelFields.ForEach(T => { T.Line = Line; Line++; });
                        EditLabelFields.Clear();
                        CurrentMousePoint = Point.Empty;
                        MouseOperationStatus = MouseOperation.Idel;
                        Template_dataGridView.DataSource = new BindingList<LabelFieldModel>(EditLabelTemplate.LabelFields);
                        break;
                    case "粘贴":
                        if (MouseOperationStatus == MouseOperation.Copy && Item.Text.Equals("粘贴"))
                        {
                            EditLabelFields.Clear();
                            for (int Index = 0; Index < CopyLabelFields.Count; Index++)
                            {
                                LabelFieldModel T = CopyLabelFields[Index];
                                LabelFieldModel LabelField = new LabelFieldModel();
                                var T_Properties = T.GetType().GetProperties();
                                var Label_Properties = LabelField.GetType().GetProperties();
                                for (int col = 0; col < T_Properties.Length; col++)
                                {
                                    var T_Property = T_Properties[col];
                                    var Label_Property = Label_Properties[col];
                                    Label_Property.SetValue(LabelField, T_Property.GetValue(T));
                                }
                                LabelField.Line = EditLabelTemplate.LabelFields.Count + 1;
                                if (CurrentMousePoint != Point.Empty)
                                {
                                    if (Index == 0)
                                    {
                                        LabelField.Left = CurrentMousePoint.X;
                                        LabelField.Top = CurrentMousePoint.Y;
                                    }
                                    else if (Index > 0)
                                    {
                                        LabelField.Left = CurrentMousePoint.X + (T.Left - CopyLabelFields[0].Left);
                                        LabelField.Top = CurrentMousePoint.Y + (T.Top - CopyLabelFields[0].Top);
                                    }
                                }
                                LabelField.ObjectType = (DrawObjectType)Enum.Parse(typeof(DrawObjectType), LabelField.FieldObjectType);
                                LabelField.ObjectValueType = (DrawObjectValueType)Enum.Parse(typeof(DrawObjectValueType), LabelField.FieldObjectValueType);
                                EditLabelFields.Add(LabelField);
                                EditLabelTemplate.LabelFields.Add(LabelField);
                            }
                            CurrentMousePoint = Point.Empty;
                            MouseOperationStatus = MouseOperation.Drag;
                            CopyLabelFields.Clear();
                            Template_dataGridView.DataSource = new BindingList<LabelFieldModel>(EditLabelTemplate.LabelFields);
                        }
                        break;
                    case "重置":
                        if (EditLabelFields.Count > 0)
                        {
                            EditLabelFields.ForEach(T =>
                            {
                                T.ScaleHeight = 0;
                                T.ScaleWidth = 0;
                            });
                        }
                        break;
                    case "水平对齐":
                        if(EditLabelFields.Count > 0)
                        {
                            EditLabelFields.ForEach(T =>
                            {
                                T.Top = EditLabelFields[0].Top;
                            });
                        }
                        break;
                    case "垂直对齐":
                        if (EditLabelFields.Count > 0)
                        {
                            EditLabelFields.ForEach(T =>
                            {
                                T.Left = EditLabelFields[0].Left;
                            });
                        }
                        break;
                    case "最上面":
                        if (EditLabelFields.Count > 0)
                        {
                            EditLabelTemplate.LabelFields.RemoveAll(T => { return EditLabelFields.Find(F => { return F.Line == T.Line; }) != null; });
                            EditLabelTemplate.LabelFields.AddRange(EditLabelFields);
                            Line = 1;
                            EditLabelTemplate.LabelFields.ForEach(T => { T.Line = Line; Line++; });
                        }

                        break;
                    case "最下面":
                        if (EditLabelFields.Count > 0)
                        {
                            EditLabelTemplate.LabelFields.RemoveAll(T => { return EditLabelFields.Find(F => { return F.Line == T.Line; }) != null; });
                            EditLabelFields.ForEach(T =>
                            {
                                EditLabelTemplate.LabelFields.Insert(0, T);
                            });
                            Line = 1;
                            EditLabelTemplate.LabelFields.ForEach(T => { T.Line = Line; Line++; });
                        }
                        break;
                }

            }
            catch (Exception ex)
            {
                EditSign = "";
                EditLabelFields.Clear();
                CurrentMousePoint = Point.Empty;
                MouseOperationStatus = MouseOperation.Idel;
                MessageBox.Show("执行失败，返回错误：" + ex.Message);
            }
        }

        private void Template_dataGridView_RowRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            try
            {
                int Index = 0;
                EditLabelTemplate.LabelFields.ForEach(T => { T.Line = ++Index; });
                if (TempTextBox.Visible) TempTextBox.Visible = false;
                if (TempCombobox.Visible) TempCombobox.Visible = false;
                if (TempNumericUpDown.Visible) TempNumericUpDown.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("执行失败，返回错误：" + ex.Message);
            }
            finally
            {
                EditSign = "";
                EditLabelFields.Clear();
                CurrentMousePoint = Point.Empty;
                MouseOperationStatus = MouseOperation.Idel;
                Redraw();
            }
        }

        private void PrintDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            try
            {
                float Scalex = (comboBox_DpiX.SelectedItem == null ? 96 : int.Parse(comboBox_DpiX.SelectedItem.ToString())) / 96;
                int LabelWidth = (int)(Helper.Convert.MillimetersToPixelsWidth(ScreenDpiX, EditLabelTemplate.LabelWidth * 10) * Scalex);
                int LabelHeight = (int)(Helper.Convert.MillimetersToPixelsWidth(ScreenDpiX, EditLabelTemplate.LabelLength * 10) * Scalex);
                Bitmap b = EditLabelTemplate.InitPrintBitmap(LabelWidth, LabelHeight, Scalex);
                e.Graphics.DrawImage(b, 0, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("打印时出现错误，返回：" + ex.Message);
            }
        }

        private void comboBox_DpiX_SelectedIndexChanged(object sender, EventArgs e)
        {
            //float Scalex = (comboBox_DpiX.SelectedItem == null ? 96 : int.Parse(comboBox_DpiX.SelectedItem.ToString())) / 96;
            Template_pictureBox_Preview.Width = (int)(Helper.Convert.MillimetersToPixelsWidth(comboBox_DpiX.SelectedItem == null ? 96 : int.Parse(comboBox_DpiX.SelectedItem.ToString()), EditLabelTemplate.LabelWidth * 10));
            Template_pictureBox_Preview.Height = (int)(Helper.Convert.MillimetersToPixelsWidth(comboBox_DpiX.SelectedItem == null ? 96 : int.Parse(comboBox_DpiX.SelectedItem.ToString()), EditLabelTemplate.LabelLength * 10));
            panel_Ruler_Vertical.Invalidate();
            panel_Ruler_Horizontal.Invalidate();
            Template_pictureBox_Preview.Invalidate();
        }

        private void Panel_Box_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
                {
                    BoxScrollVertical = e.NewValue;
                    panel_Ruler_Vertical.Invalidate();
                }
                else if(e.ScrollOrientation==ScrollOrientation.HorizontalScroll)
                {
                    BoxScrollHorizontal = e.NewValue;
                    panel_Ruler_Horizontal.Invalidate();
                }
                Redraw();
            }
            catch (Exception)
            {

            }
        }

        private void Panel_Ruler_Vertical_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Rectangle clientRect = panel_Ruler_Vertical.ClientRectangle;
                Bitmap bufferimage = new Bitmap(clientRect.Width,clientRect.Height);
                Graphics g = Graphics.FromImage(bufferimage);
                //g.SmoothingMode = SmoothingMode.HighQuality; //高质量
                //g.PixelOffsetMode = PixelOffsetMode.HighQuality; //高像素偏移质量
                SolidBrush br = new SolidBrush(Color.LightGoldenrodYellow);
                g.FillRectangle(br, clientRect);
                Pen pen = Pens.Black;

                var width = clientRect.Width;
                var height = clientRect.Height;
                var unitHeight = RulerPadding - 15; ;
                var halfUnitHeght = RulerPadding - 20; ;
                var packUnitHeight = RulerPadding - 5;
                var hDc = Helper.Convert.GetDC(this.Handle);

                int Millimeter = 1;
                double MM = Helper.Convert.PixelsToMillimetersWidth(comboBox_DpiX.SelectedItem == null ? 96 : int.Parse(comboBox_DpiX.SelectedItem.ToString()), (double)BoxScrollVertical);
                Millimeter = (int)MM;

                for (int i = RulerPadding; i < height; i++)
                {
                    MM = Helper.Convert.PixelsToMillimetersWidth(comboBox_DpiX.SelectedItem == null ? 96 : int.Parse(comboBox_DpiX.SelectedItem.ToString()), (double)(i - RulerPadding + BoxScrollVertical));
                    if (MM >= Millimeter)
                    {
                        g.DrawLine(pen, 0, i, halfUnitHeght, i);
                        if (Millimeter % 5 == 0)
                        {
                            g.DrawLine(pen, 0, i, unitHeight, i);
                        }
                        if (Millimeter % 10 == 0)
                        {
                            g.DrawLine(pen, 0, i, packUnitHeight, i);
                            g.DrawString((Millimeter / 10).ToString(), this.Font, Brushes.Black, packUnitHeight - 10, i);
                        }
                        Millimeter++;
                    }
                    else if (MM == 0)
                    {
                        g.DrawLine(pen, 0, i, packUnitHeight, i);
                    }
                }
                e.Graphics.DrawImage(bufferimage,0, 0);
            }
            catch (Exception ex)
            {

            }
        }

        private void Panel_Ruler_Horizontal_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Rectangle clientRect = panel_Ruler_Horizontal.ClientRectangle;
                Bitmap bufferimage = new Bitmap(clientRect.Width, clientRect.Height);
                Graphics g = Graphics.FromImage(bufferimage);
                //g.SmoothingMode = SmoothingMode.HighQuality; //高质量
                //g.PixelOffsetMode = PixelOffsetMode.HighQuality; //高像素偏移质量
                SolidBrush br = new SolidBrush(Color.LightGoldenrodYellow);
                g.FillRectangle(br, clientRect);
                Pen pen = Pens.Black;

                var width = clientRect.Width;
                var height = clientRect.Height;
                var unitHeight = RulerPadding - 15; ;
                var halfUnitHeght = RulerPadding - 20;
                var packUnitHeight = RulerPadding - 5;
                var hDc = Helper.Convert.GetDC(this.Handle);

                int Millimeter = 1;
                double MM = Helper.Convert.PixelsToMillimetersWidth(comboBox_DpiX.SelectedItem == null ? 96 : int.Parse(comboBox_DpiX.SelectedItem.ToString()), (double)BoxScrollHorizontal);
                Millimeter = (int)MM;

                for (int i = 0; i < width; i++)
                {
                    MM = Helper.Convert.PixelsToMillimetersWidth(comboBox_DpiX.SelectedItem == null ? 96 : int.Parse(comboBox_DpiX.SelectedItem.ToString()), (double)(i + BoxScrollHorizontal));
                    if (MM >= Millimeter)
                    {
                        g.DrawLine(pen, i, 0, i, halfUnitHeght);
                        if (Millimeter % 5 == 0)
                        {
                            g.DrawLine(pen, i, 0, i, unitHeight);
                        }
                        if (Millimeter % 10 == 0)
                        {
                            g.DrawLine(pen, i, 0, i, packUnitHeight);
                            g.DrawString((Millimeter / 10).ToString(), this.Font, Brushes.Black, i, packUnitHeight - 8);
                        }
                        Millimeter++;
                    }
                    else if (MM == 0)
                    {
                        g.DrawLine(pen, i, 0, i, packUnitHeight);
                    }
                }
                e.Graphics.DrawImage(bufferimage, 0, 0);
            }
            catch (Exception ex)
            {

            }
        }
    }

    public static class MyLabelTemplate
    {
        private static string _ConfigFile = Directory.GetCurrentDirectory().ToString() + @"\DesignLabel.xml";
        public static List<LabelTemplateModel> LabelArray { get; set; }
       
        public static List<LabelTemplateModel> InitLabelTemplate()
        {
            try
            {
                if (!File.Exists(_ConfigFile)) throw new Exception("标签配置文件不存在，请检查程序文件夹位置！");
                MyXML myConfig = new MyXML(_ConfigFile);
                if (!myConfig.OpenConfigFile()) { throw new Exception("标签配置文件打开失败，请联系管理员处理！"); }
                LabelArray = new List<LabelTemplateModel>();

                string[] Temps = myConfig.ReadAttributeValue("appsettings", "LabelTemplate", "Value").Split('|');
                foreach(string Name in Temps)
                {
                    if (string.IsNullOrEmpty(Name)) continue;
                    LabelTemplateModel Template = new LabelTemplateModel();
                    Template.LabelName = Name;
                    Template.LabelWidth = double.Parse(myConfig.ReadAttributeValue("LabelTemplate", "Template_" + Name, "LabelWidth"));
                    Template.LabelLength = double.Parse(myConfig.ReadAttributeValue("LabelTemplate", "Template_" + Name, "LabelLength"));
                    List<ElmentList> elments = myConfig.ReadAttributeValue("Template_" + Name);
                    for (int Index = 0; Index < elments.Count; Index++)
                    {
                        ElmentList el = elments[Index];
                        LabelFieldModel LabelField = new LabelFieldModel();
                        var LabelObjectProperties = LabelField.GetType().GetProperties();
                        for (int Col = 0; Col < LabelObjectProperties.Length; Col++)
                        {
                            var Item = LabelObjectProperties[Col];
                            string Temp = Col==0?"":el._AttributeList[Col-1].AttributeValue;
                            if (Item.Name.Equals("Line")) Item.SetValue(LabelField, Index+1, null);
                            else
                            {
                                if (Item.Name.Equals("FieldObjectType")) { LabelField.ObjectType = (DrawObjectType)Enum.Parse(typeof(DrawObjectType), Temp); }
                                else if (Item.Name.Equals("FieldObjectValueType")) { LabelField.ObjectValueType = (DrawObjectValueType)Enum.Parse(typeof(DrawObjectValueType), Temp); }

                                object ObjValue = (object)Item.GetValue(LabelField);
                                if (ObjValue==null || ObjValue.GetType() == typeof(string)) Item.SetValue(LabelField, Temp, null);
                                else if (ObjValue.GetType() == typeof(float)) Item.SetValue(LabelField, float.Parse(Temp), null);
                            }
                        }
                        Template.LabelFields.Add(LabelField);
                    }
                    LabelArray.Add(Template);
                }
                return LabelArray;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
                //return null;
            }
        }

        public static LabelTemplateModel GetLabelTemplate(string TemplateName)
        {
            try
            {
                return LabelArray.Find(T => { return T.LabelName.Equals(TemplateName); });
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool WriteLabelTemplate()
        {
            try
            {
                if (LabelArray == null) throw new Exception("标签数据信息为空！");
                if (!File.Exists(_ConfigFile)) throw new Exception("标签配置文件不存在，请检查程序文件夹位置！");
                try { File.Copy(_ConfigFile, Directory.GetCurrentDirectory().ToString() + @"\DesignLabel" + DateTime.Now.ToString("yyyyMMdd HHmm") + ".bak"); }
                catch(Exception ex) { throw new Exception("备份标签配置文件失败，返回错误信息：" + ex.Message); }

                MyXML myConfig = new MyXML(_ConfigFile);
                if (!myConfig.OpenConfigFile()) { throw new Exception("标签配置文件打开失败，请联系管理员处理！"); }

                string Temp = "";
                LabelArray.ForEach(T => { Temp += string.IsNullOrEmpty(Temp) ? T.LabelName : ("|" + T.LabelName); });
                if (!myConfig.DeleteAllElmentFromChildNode("appsettings")) throw new Exception("清除标签配置根节点失败！");
                //if (!myConfig.SetAttributeValue("appsettings", "LabelTemplate", "Value", Temp)) throw new Exception("写入标签配置信息根节点失败！");
                if(!myConfig.InsertChildNode("appsettings", "LabelTemplate", "Value", Temp)) throw new Exception("写入标签配置信息根节点失败！");

                foreach (LabelTemplateModel Template in LabelArray)
                {
                    AttributeList Att1 = new AttributeList("LabelWidth", Template.LabelWidth.ToString());
                    AttributeList Att2 = new AttributeList("LabelLength", Template.LabelLength.ToString());
                    if (!myConfig.InsertChildNode("LabelTemplate", "Template_" + Template.LabelName, new List<AttributeList>() { Att1, Att2 })) throw new Exception("写入标签：" + Template.LabelName + "节点失败！");

                    foreach(LabelFieldModel LabelField in Template.LabelFields)
                    {
                        var LabelObjectProperties = LabelField.GetType().GetProperties();
                        List<AttributeList> AttArry = new List<AttributeList>();
                        for (int Col = 0; Col < LabelObjectProperties.Length; Col++)
                        {
                            var Item = LabelObjectProperties[Col];
                            if (Item.Name.Equals("Line")) continue;
                            else if (Item.Name.Equals("FieldObjectType")) { AttArry.Add(new AttributeList("FieldObjectType", LabelField.ObjectType.ToString())); }
                            else if (Item.Name.Equals("FieldObjectValueType")) { AttArry.Add(new AttributeList("FieldObjectValueType", LabelField.ObjectValueType.ToString())); }
                            else { AttArry.Add(new AttributeList(Item.Name, Item.GetValue(LabelField, null)!=null?Item.GetValue(LabelField, null).ToString():"")); }
                        }
                        if (!myConfig.InsertChildNode("Template_" + Template.LabelName, "Item", AttArry)) throw new Exception("写入标签：" + Template.LabelName + "字段记录信息不正确！");
                    }
                }
                if (!myConfig.SaveConfigFile()) return false; else return true;
            }
            catch (Exception ex)
            {
                throw ex;
                //return false;
            }
        }

        public static LabelTemplateModel CloneTempalte(LabelTemplateModel Template)
        {
            try
            {
                LabelTemplateModel Temp = new LabelTemplateModel();
                Temp.LabelName = Template.LabelName;
                Temp.LabelWidth = Template.LabelWidth;
                Temp.LabelLength = Template.LabelLength;
                Template.LabelFields.ForEach(T =>
                {
                    var T_Properties = T.GetType().GetProperties();
                    LabelFieldModel TempField = new LabelFieldModel();
                    var TempField_Properties = TempField.GetType().GetProperties();
                    for (int Col = 0; Col < T_Properties.Length; Col++)
                    {
                        TempField_Properties[Col].SetValue(TempField, T_Properties[Col].GetValue(T));
                    }
                    TempField.ObjectType = T.ObjectType;
                    TempField.ObjectValueType = T.ObjectValueType;
                    Temp.LabelFields.Add(TempField);
                });
                return Temp;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public class LabelTemplateModel
    {
        public static int HandleLength = 10;
        public string LabelName { get; set; }
        public double LabelWidth { get; set; }
        public double LabelLength { get; set; }
        public List<LabelFieldModel> LabelFields { get; set; }
        public LabelTemplateModel() { LabelFields = new List<LabelFieldModel>(); }

        public static Bitmap GenerateMatrixCode(string Code, int height, int width)
        {
            try
            {
                DatamatrixEncodingOptions options = new DatamatrixEncodingOptions();
                options.DefaultEncodation = Encodation.ASCII;/*指定默认编码确保内容符合编码值，否则将抛出异常。 标准值：Encodation.ASCII*/
                //options.MinSize = new Dimension(100, 100);  //指定最小条形码大小 当SymbolShapeHint.FORCE_NONE时 有效
                //options.MaxSize = new Dimension(150, 120); //指定最大条形码大小 当SymbolShapeHint.FORCE_NONE时 有效
                options.SymbolShape = SymbolShapeHint.FORCE_SQUARE; //条码形状
                options.GS1Format = true;    //是否符合GS1 不会用
                options.Width = width;    //图片宽度
                options.Height = height;    //图片高度
                BarcodeWriter writer = new BarcodeWriter();
                writer.Format = BarcodeFormat.DATA_MATRIX;
                writer.Options = options;
                Bitmap bmp = writer.Write(Code);
                return bmp;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static Bitmap Generate128Code(string code, int height, int width)
        {
            try
            {
                BarcodeWriter writer = new BarcodeWriter();
                writer.Format = BarcodeFormat.CODE_128;
                Code128EncodingOptions options = new Code128EncodingOptions();
                options.ForceCodeset = Code128EncodingOptions.Codesets.A;
                options.PureBarcode = true;
                options.GS1Format = false;
                options.Width = width;
                options.Height = height;
                options.Margin = 6;
                /*EncodingOptions options = new EncodingOptions()
                {
                    PureBarcode = true,
                    Width = width,
                    Height = height,
                    Margin = 3
                };*/
                writer.Options = options;
                Bitmap map = writer.Write(code);
                return map;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static Bitmap GenerateQRCode(string code, int height, int width)
        {
            try
            {
                BarcodeWriter writer = new BarcodeWriter();
                writer.Format = BarcodeFormat.QR_CODE;
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
        public static void ConvertCoordinate(LabelFieldModel LabelField, int Width, int Height)
        {
            try
            {
                LabelField.Width = Width;
                LabelField.Length = Height;
                if (LabelField.ScaleHeight == 0 || LabelField.ScaleWidth == 0)
                {
                    LabelField.ScaleWidth = Width;
                    LabelField.ScaleHeight = Height;
                }
                LabelField.Center = new PointF((float)(LabelField.Left + LabelField.ScaleWidth / 2), (float)(LabelField.Top + LabelField.ScaleHeight / 2));
                if (LabelField.Angle == 90 || LabelField.Angle == 270)
                {
                    LabelField.Rectangle = new Rectangle((int)(LabelField.Center.X - LabelField.ScaleHeight / 2), (int)(LabelField.Center.Y - LabelField.ScaleWidth / 2), (int)LabelField.ScaleHeight, (int)LabelField.ScaleWidth);
                }
                else
                {
                    LabelField.Rectangle = new Rectangle((int)LabelField.Left, (int)LabelField.Top, (int)LabelField.ScaleWidth, (int)LabelField.ScaleHeight);
                }

                if (true/*LabelField.HandleRectangle.Count <= 0 || (LabelField.ScaleWidth == LabelField.Width && LabelField.Length == LabelField.ScaleHeight)*/)
                {
                    LabelField.HandleRectangle.Clear();
                    //LabelField.HandleRectangle.Add(new Rectangle(new Point((int)(LabelField.Left + LabelField.ScaleWidth / 2 - HandleLength / 2), (int)(LabelField.Top - HandleLength)), new Size(HandleLength, HandleLength)));
                    //LabelField.HandleRectangle.Add(new Rectangle(new Point((int)(LabelField.Left + LabelField.ScaleWidth / 2 - HandleLength / 2), (int)(LabelField.Top + LabelField.ScaleHeight)), new Size(HandleLength, HandleLength)));
                    //LabelField.HandleRectangle.Add(new Rectangle(new Point((int)(LabelField.Left - HandleLength), (int)(LabelField.Top + LabelField.ScaleHeight / 2 - HandleLength / 2)), new Size(HandleLength, HandleLength)));
                    //LabelField.HandleRectangle.Add(new Rectangle(new Point((int)(LabelField.Left + LabelField.ScaleWidth), (int)(LabelField.Top + LabelField.ScaleHeight / 2 - HandleLength / 2)), new Size(HandleLength, HandleLength)));

                    LabelField.HandleRectangle.Add(new Rectangle(new Point((int)(LabelField.Rectangle.X + LabelField.Rectangle.Width / 2 - HandleLength / 2), (int)(LabelField.Rectangle.Y - HandleLength)), new Size(HandleLength, HandleLength)));
                    LabelField.HandleRectangle.Add(new Rectangle(new Point((int)(LabelField.Rectangle.X + LabelField.Rectangle.Width / 2 - HandleLength / 2), (int)(LabelField.Rectangle.Y + LabelField.Rectangle.Height)), new Size(HandleLength, HandleLength)));
                    LabelField.HandleRectangle.Add(new Rectangle(new Point((int)(LabelField.Rectangle.X - HandleLength), (int)(LabelField.Rectangle.Y + LabelField.Rectangle.Height / 2 - HandleLength / 2)), new Size(HandleLength, HandleLength)));
                    LabelField.HandleRectangle.Add(new Rectangle(new Point((int)(LabelField.Rectangle.X + LabelField.Rectangle.Width), (int)(LabelField.Rectangle.Y + LabelField.Rectangle.Height / 2 - HandleLength / 2)), new Size(HandleLength, HandleLength)));
                }


            }
            catch (Exception)
            {
                throw new Exception("转换图形对象坐标失败！");
            }
        }
        public static void DrawStringToImage(Graphics G, LabelFieldModel LabelField, string PrintValue, Font myFont, Brush bush, SizeF sf)
        {
            try
            {
                if (LabelField.Width != LabelField.ScaleWidth || LabelField.Length != LabelField.ScaleHeight)
                {
                    Bitmap TempBit = new Bitmap((int)(sf.Width + 1), (int)(sf.Height + 1));
                    Graphics TempG = Graphics.FromImage(TempBit);
                    TempG = Graphics.FromImage(TempBit);
                    TempG.Clear(Color.Transparent);
                    TempG.DrawString(PrintValue, myFont, bush, 0, 0);
                    TempG.Save();
                    G.DrawImage(TempBit, LabelField.Left, LabelField.Top, LabelField.ScaleWidth, LabelField.ScaleHeight);
                    TempG.Dispose();
                    TempBit.Dispose();
                }
                else
                {
                    G.DrawString(PrintValue, myFont, bush, LabelField.Left, LabelField.Top);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void DrawStringToScaleImage(Graphics G, LabelFieldModel LabelField, string PrintValue, Font myFont, Brush bush, SizeF sf, float ScaleX)
        {
            try
            {
                if (LabelField.Width != LabelField.ScaleWidth || LabelField.Length != LabelField.ScaleHeight)
                {
                    Bitmap TempBit = new Bitmap((int)(sf.Width + 1), (int)(sf.Height + 1));
                    Graphics TempG = Graphics.FromImage(TempBit);
                    TempG = Graphics.FromImage(TempBit);
                    TempG.Clear(Color.Transparent);
                    TempG.DrawString(PrintValue, myFont, bush, 0, 0);
                    TempG.Save();
                    G.DrawImage(TempBit, LabelField.Left * ScaleX, LabelField.Top * ScaleX, LabelField.ScaleWidth * ScaleX, LabelField.ScaleHeight * ScaleX);
                    TempG.Dispose();
                    TempBit.Dispose();
                }
                else
                {
                    G.DrawString(PrintValue, myFont, bush, LabelField.Left * ScaleX, LabelField.Top * ScaleX);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Bitmap InitPrintBitmap(int LabelWidth, int LabelHeight, float ScaleX = 1)
        {
            try
            {
                Bitmap b = new Bitmap(LabelWidth, LabelHeight);
                Graphics theGraphics = Graphics.FromImage((System.Drawing.Image)b);
                theGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                theGraphics.Clear(Color.White);
                Brush bush = new SolidBrush(System.Drawing.Color.Black);
                foreach (LabelFieldModel LabelField in LabelFields)
                {
                    string PrintValue = LabelField.PrintValue;
                    Font myFont = new System.Drawing.Font(LabelField.FontName, LabelField.FontSize <= 0 ? 1 : LabelField.FontSize, FontStyle.Regular);
                    switch (LabelField.ObjectType)
                    {
                        case DrawObjectType.Text:
                        case DrawObjectType.Date:
                        case DrawObjectType.Number:
                            if (LabelField.ObjectType == DrawObjectType.Date)
                            {
                                if (LabelField.ObjectValueType == DrawObjectValueType.Dynamic || LabelField.ObjectValueType == DrawObjectValueType.Regular)
                                {
                                    try { PrintValue = DateTime.Now.ToString(LabelField.PrintValue); }
                                    catch(Exception ex) { PrintValue = "XXXXXXXX"; }
                                }
                            }
                            SizeF sf = theGraphics.MeasureString(PrintValue, myFont);
                            ConvertCoordinate(LabelField, (int)sf.Width, (int)sf.Height);
                            if (LabelField.Angle > 0)
                            {
                                Matrix matrix = theGraphics.Transform;
                                matrix.RotateAt(LabelField.Angle, new PointF(LabelField.Center.X * ScaleX, LabelField.Center.Y * ScaleX));
                                theGraphics.Transform = matrix;
                                DrawStringToScaleImage(theGraphics, LabelField, PrintValue, myFont, bush, sf, ScaleX);
                                theGraphics.ResetTransform();
                                //if (LabelField.Angle == 90||LabelField.Angle==270) LabelField.Rectangle = new Rectangle((int)(LabelField.Center.X - sf.Height / 2), (int)(LabelField.Center.Y - sf.Width / 2), (int)sf.Height, (int)sf.Width);
                            }
                            else DrawStringToScaleImage(theGraphics, LabelField, PrintValue, myFont, bush, sf, ScaleX);
                            break;
                        case DrawObjectType.Matrix:
                        case DrawObjectType.Barcode:
                        case DrawObjectType.QRCode:
                            Bitmap CodePic = null;
                            if (string.IsNullOrEmpty(PrintValue)) PrintValue = "ERROR";
                            if (LabelField.ObjectType == DrawObjectType.Matrix) CodePic = GenerateMatrixCode(PrintValue, (int)LabelField.Length, (int)LabelField.Width);
                            else if (LabelField.ObjectType == DrawObjectType.Barcode) CodePic = Generate128Code(PrintValue, (int)LabelField.Length, (int)LabelField.Width);
                            else if (LabelField.ObjectType == DrawObjectType.QRCode) CodePic = GenerateQRCode(PrintValue, (int)LabelField.Length, (int)LabelField.Width);
                            if (CodePic == null) throw new Exception("图像代码生成失败！");
                            ConvertCoordinate(LabelField, (int)CodePic.Width, (int)CodePic.Height);
                            LabelField.Rectangle = new Rectangle((int)(LabelField.Left * ScaleX), (int)(LabelField.Top * ScaleX), (int)(LabelField.ScaleWidth * ScaleX), (int)(LabelField.ScaleHeight * ScaleX));
                            if (LabelField.Angle > 0)
                            {
                                LabelField.Rectangle = new Rectangle((int)(LabelField.Center.X * ScaleX - LabelField.ScaleHeight * ScaleX / 2), (int)(LabelField.Center.Y * ScaleX - LabelField.ScaleWidth * ScaleX / 2), (int)(LabelField.ScaleHeight * ScaleX), (int)(LabelField.ScaleWidth * ScaleX));
                                Matrix matrix = theGraphics.Transform;
                                matrix.RotateAt(LabelField.Angle, new PointF(LabelField.Center.X * ScaleX, LabelField.Center.Y * ScaleX));
                                theGraphics.Transform = matrix;
                                theGraphics.DrawImage(CodePic, LabelField.Rectangle);
                                theGraphics.ResetTransform();
                                //if (LabelField.Angle == 90 || LabelField.Angle == 270) LabelField.Rectangle = new Rectangle((int)(LabelField.Center.X - LabelField.ScaleHeight / 2), (int)(LabelField.Center.Y - LabelField.ScaleWidth / 2), (int)LabelField.ScaleHeight, (int)LabelField.ScaleWidth);
                            }
                            else theGraphics.DrawImage(CodePic, LabelField.Rectangle);
                            break;
                        default:
                            throw new Exception("未知图形对象！");
                    }
                }
                theGraphics.Dispose();
                return b;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class LabelFieldModel
    {
        public int Line { get; set; }
        public string FieldObjectType { get; set; }
        public string FieldObjectValueType { get; set; }

        public DrawObjectType ObjectType;
        public DrawObjectValueType ObjectValueType;
        public string Sign { get; set; }
        public string PrintValue { get; set; }
        public float Left { get; set; }
        public float Top { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public float ScaleHeight { get; set; }
        public float ScaleWidth { get; set; }
        public float Angle { get; set; }
        public string FontName { get; set; }
        public float FontSize { get; set; }
        public PointF Center;
        public Rectangle Rectangle;
        public List<Rectangle> HandleRectangle;

        public LabelFieldModel() { HandleRectangle = new List<Rectangle>(); }
    }

    public enum DrawObjectType { Text,Date, Number, Matrix, Barcode, QRCode,None }
    public enum DrawObjectValueType { Dynamic, Static, Regular, Variable }
    public enum MouseOperation { Idel, Paint, Selecting, Selected, ReadyDrag, Drag, Copy, Scale }

    public class DataTemplate
    {
        public DrawObjectType ObjectType { get; set; }
        public DrawObjectValueType ValueType { get; set; }
    }
}
