using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Helper;
using ZXing;
using ZXing.Common;

namespace TestLeak
{
    public partial class Config : Form
    {
        private DataTable PartDt;
        private bool PrintProgress;
        private string TestPrinter;
        private DataTable PlanDt;
        private AccessHelper accessHelper;
        private ComboBox TempCombobox = new ComboBox();
        private Parameter.PartModel TextPart;
        private Parameter parameter;

        public Config()
        {
            InitializeComponent();
        }

        private void Config_Load(object sender, EventArgs e)
        {
            try
            {
                toolStripButton_Save.Enabled = false;
                dateTimePicker_PlanDate.Enabled = false;
                button_SavePlanData.Enabled = false;

                if(!InitComList())
                {
                    MessageBox.Show("初始化COM列表失败，请查看系统配置！");
                    return;
                }

                if(!InitPrinterList())
                {
                    MessageBox.Show("初始化打印机列表失败，请查看系统配置！");
                    return;
                }

                parameter = MyConfig.GetParameter();
                if (parameter == null)
                {
                    MessageBox.Show("初始化程序参数失败，请检查配置文件是否存在！");
                    return;
                }

                comboBox_Leak_ComPort.Text = parameter.LeakTestSettings.ComConfig.ComPort;
                comboBox_Leak_Speed.Text = parameter.LeakTestSettings.ComConfig.Speed.ToString();
                comboBox_Leak_DataBit.Text = parameter.LeakTestSettings.ComConfig.DataBit.ToString();
                comboBox_Leak_StopBit.Text = parameter.LeakTestSettings.ComConfig.StopBit.ToString();
                comboBox_Leak_CheckType.Text = parameter.LeakTestSettings.ComConfig.CheckType;
                textBox_Leak_SuccessSign.Text = parameter.LeakTestSettings.SuccessSign;
                textBox_Leak_ErrorSign.Text = parameter.LeakTestSettings.ErrorSign;
                numericUpDown_Leak_GapAlert.Value = parameter.LeakTestSettings.GapAlertValue;
                checkBox_Leak_EnabledGapAlert.Checked = parameter.LeakTestSettings.EnabledGapAlert;

                checkBox_PrintCheck_Enabled.Checked = parameter.PrintCheckSettings.EnabledPrintCheck;
                checkBox_PrintCheck_EnabledStrictCheck.Checked = parameter.PrintCheckSettings.EnabledStrictCheck;
                comboBox_PrintCheck_ComPort.Text = parameter.PrintCheckSettings.ComConfig.ComPort;
                comboBox_PrintCheck_DataBit.Text = parameter.PrintCheckSettings.ComConfig.DataBit.ToString();
                comboBox_PrintCheck_Speed.Text = parameter.PrintCheckSettings.ComConfig.Speed.ToString();
                comboBox_PrintCheck_StopBit.Text = parameter.PrintCheckSettings.ComConfig.StopBit.ToString();
                comboBox_PrintCheck_CheckType.Text = parameter.PrintCheckSettings.ComConfig.CheckType;

                comboBox_Print_PrintName.Text = parameter.PrintSettings.PrintName;
                comboBox_Print_DpiX.Text = parameter.PrintSettings.DpiX.ToString();

                PartDt = new DataTable();
                PartDt.Columns.Add("Item");
                PartDt.Columns.Add("PrintCount");
                foreach(Parameter.PartModel s in parameter.PartList)
                {
                    DataRow dr = PartDt.NewRow();
                    dr["Item"] = s.Item;
                    dr["PrintCount"] = s.PrintCount;
                    PartDt.Rows.Add(dr);
                    TempCombobox.Items.Add(s.Item);
                }
                dataGridView_PartList.DataSource = PartDt;

                if (!InitAccessFile())
                {
                    MessageBox.Show("初始化数据库文件失败！");
                    return;
                }

                if (!InitPlanData())
                {
                    MessageBox.Show("初始化试漏计划数据失败！");
                    return;
                }

                if (!string.IsNullOrEmpty(parameter.Password))
                {
                    //textBox_Password.Text = Helper.Convert.RSADecrypt(parameter.Password);
                }

                checkBox_EnabledAlertSound.Checked = parameter.EnabledAlertSound;

                toolStripButton_Save.Enabled = true;
                dateTimePicker_PlanDate.Enabled = true;
                button_SavePlanData.Enabled = true;
            }
            catch (Exception ex )
            {
                MessageBox.Show("初始化窗体失败，返回错误：" + ex.ToString());
            }
        }

        private bool InitComList()
        {
            try
            {
                comboBox_Leak_ComPort.Items.Clear();
                comboBox_PrintCheck_ComPort.Items.Clear();
                foreach (string vPortName in SerialPort.GetPortNames())
                {
                    comboBox_Leak_ComPort.Items.Add(vPortName);
                    comboBox_PrintCheck_ComPort.Items.Add(vPortName);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool InitPrinterList()
        {
            try
            {
                PrintDocument PrintDoc = new PrintDocument();
                string DefaultPrinter = PrintDoc.PrinterSettings.PrinterName;
                comboBox_Print_PrintName.Items.Clear();
                foreach (string Printer in PrinterSettings.InstalledPrinters)
                {
                    comboBox_Print_PrintName.Items.Add(Printer);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool InitAccessFile()
        {
            try
            {
                accessHelper = new AccessHelper();
                return accessHelper.InitAccessHelper();
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool InitPlanData()
        {
            try
            {
                string SQL = "select * from Plan Where 计划日期=#" + dateTimePicker_PlanDate.Value.ToShortDateString() + " 00:00:00#";
                PlanDt = accessHelper.ExecuteDatatable(SQL);
                if (PlanDt == null) return false;

                dataGridView_PlanData.DataSource = PlanDt;
                dataGridView_PlanData.Controls.Add(TempCombobox);
                TempCombobox.Visible = false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void toolStripButton_Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(comboBox_Leak_ComPort.Text)) { MessageBox.Show("试漏仪COM口未设置！"); return; }
                if (checkBox_PrintCheck_Enabled.Checked && string.IsNullOrEmpty(comboBox_PrintCheck_ComPort.Text)) { MessageBox.Show("标签打印验证接收COM口未设置！"); return; }
                if (string.IsNullOrEmpty(comboBox_Print_PrintName.Text)) { MessageBox.Show("标签打印机未设置！"); return; }

                parameter = new Parameter();
                parameter.LeakTestSettings.ComConfig.ComPort = comboBox_Leak_ComPort.Text;
                parameter.LeakTestSettings.ComConfig.Speed = int.Parse(comboBox_Leak_Speed.Text);
                parameter.LeakTestSettings.ComConfig.DataBit = int.Parse(comboBox_Leak_DataBit.Text);
                parameter.LeakTestSettings.ComConfig.StopBit = int.Parse(comboBox_Leak_StopBit.Text);
                parameter.LeakTestSettings.ComConfig.CheckType = comboBox_Leak_CheckType.Text;
                parameter.LeakTestSettings.SuccessSign = textBox_Leak_SuccessSign.Text;
                parameter.LeakTestSettings.ErrorSign = textBox_Leak_ErrorSign.Text;
                parameter.LeakTestSettings.GapAlertValue = (int)numericUpDown_Leak_GapAlert.Value;
                parameter.LeakTestSettings.EnabledGapAlert = checkBox_Leak_EnabledGapAlert.Checked;

                parameter.PrintCheckSettings.ComConfig.ComPort = comboBox_PrintCheck_ComPort.Text;
                parameter.PrintCheckSettings.ComConfig.Speed = int.Parse(comboBox_PrintCheck_Speed.Text);
                parameter.PrintCheckSettings.ComConfig.DataBit = int.Parse(comboBox_PrintCheck_DataBit.Text);
                parameter.PrintCheckSettings.ComConfig.StopBit = int.Parse(comboBox_PrintCheck_StopBit.Text);
                parameter.PrintCheckSettings.ComConfig.CheckType = comboBox_PrintCheck_CheckType.Text;
                parameter.PrintCheckSettings.EnabledPrintCheck = checkBox_PrintCheck_Enabled.Checked;
                parameter.PrintCheckSettings.EnabledStrictCheck = checkBox_PrintCheck_EnabledStrictCheck.Checked;

                parameter.PrintSettings.PrintName = comboBox_Print_PrintName.Text;
                parameter.PrintSettings.DpiX=int.Parse(comboBox_Print_DpiX.Text);

                PartDt.AcceptChanges();
                parameter.PartList.Clear();
                foreach(DataRow dr in PartDt.Rows)
                {
                    Parameter.PartModel Item = new Parameter.PartModel();
                    Item.Item = dr["item"].ToString();
                    Item.PrintCount = int.Parse(dr["PrintCount"].ToString());
                    parameter.PartList.Add(Item);
                }

                //parameter.Password = Helper.Convert.RSAEncryption(textBox_Password.Text);

                parameter.EnabledAlertSound = checkBox_EnabledAlertSound.Checked;

                if (MyConfig.SaveParameter(parameter))
                {
                    MessageBox.Show("保存成功！");
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("保存失败！");
                    DialogResult = DialogResult.Cancel;
                }

                accessHelper.CloseAccessHelper();
                accessHelper = null;
                Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存时失败，返回的错误：" + ex.ToString());
            }
        }

        private void toolStripButton_Exit_Click(object sender, EventArgs e)
        {
            accessHelper.CloseAccessHelper();
            accessHelper = null;
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        private void button_SavePlanData_Click(object sender, EventArgs e)
        {
            try
            {
                PlanDt.AcceptChanges();
                foreach(DataRow dr in PlanDt.Rows)
                {
                    string ID = Helper.Convert.ConvertString(dr, "ID");
                    string SQL = "";
                    if (string.IsNullOrEmpty(ID)) SQL = "insert into Plan(计划日期,零件号,计划数量,已完成数量,故障数量) values('" + dateTimePicker_PlanDate.Value.ToShortDateString() + " 00:00:00','" + Helper.Convert.ConvertString(dr, "零件号") + "'," + Helper.Convert.ConvertString(dr, "计划数量") + ",0,0)";
                    else SQL = "UPDATE Plan set 零件号='" + Helper.Convert.ConvertString(dr, "零件号") + "', 计划数量=" + Helper.Convert.ConvertString(dr, "计划数量") + " Where ID=" + ID;

                    if (accessHelper.ExecuteCommand(SQL) <= 0) throw new Exception("写入数据库失败！");
                }
                MessageBox.Show("计划数据保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show("更新标签打印计划数据失败，返回错误：" + ex.ToString());
            }
        }

        private void PlanData_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dataGridView_PlanData.Columns[e.ColumnIndex].Name.Equals("PartName"))
                {
                    DataGridViewCell cell = dataGridView_PlanData.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    Rectangle rect = dataGridView_PlanData.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                    TempCombobox.Location = rect.Location;
                    TempCombobox.Size = rect.Size;
                    ConfirmComboboxValue(TempCombobox, cell.Value.ToString());
                    TempCombobox.Visible = true;
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel_Status.Text = ex.ToString();
            }
        }

        private void PlanData_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dataGridView_PlanData.Columns[e.ColumnIndex].Name.Equals("PartName"))
                {
                    DataGridViewCell cell = dataGridView_PlanData.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    cell.Value = TempCombobox.Text;
                    TempCombobox.Visible = false;
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel_Status.Text = ex.ToString();
            }
        }

        private void ConfirmComboboxValue(ComboBox comboBox, string CellValue)
        {
            try
            {
                comboBox.SelectedIndex = -1;
                if (string.IsNullOrEmpty(CellValue))
                {
                    comboBox.Text = "";
                    return;
                }
                comboBox.Text = CellValue;
                foreach (Object Item in comboBox.Items)
                {
                    if (Item.ToString().Equals(CellValue))
                    {
                        comboBox.SelectedItem = Item;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel_Status.Text = ex.ToString();
            }
        }

        private void PlanDate_ValueChanged(object sender, EventArgs e)
        {
            if (!InitPlanData())
            {
                MessageBox.Show("读取标签打印计划失败！");
            }
        }

        private void toolStripButton_DesignLabel_Click(object sender, EventArgs e)
        {
            List<string> Templates = new List<string>();
            parameter.PartList.ForEach(T => { Templates.Add(T.Item); });
            DesignLabel Design = new DesignLabel(Templates);
            Design.ShowDialog();
        }
    }
}
