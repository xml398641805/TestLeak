using Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestLeak
{
    public partial class Report : Form
    {
        private AccessHelper accessHelper;
        private string CurrentReport;

        public Report()
        {
            InitializeComponent();
        }

        private void Report_Load(object sender, EventArgs e)
        {
            toolStripButton_Export.Enabled = false;
            toolStripButton_StartDate.Enabled = false;
            toolStripButton_EndDate.Enabled = false;
            toolStripButton_Find.Enabled = false;
            toolStripComboBox_ReportType.Enabled = false;

            if (!InitAccessFile())
            {
                MessageBox.Show("初始化数据库连接失败，请尝试重新打开程序！");
                return;
            }

            if (!InitExcel())
            {
                MessageBox.Show("初始化数据表格失败，请尝试重新打开程序！");
                return;
            }

            toolStripTextBox_StartDate.Text = DateTime.Now.ToShortDateString();
            toolStripTextBox_EndDate.Text = DateTime.Now.ToShortDateString();
            toolStripButton_Export.Enabled = true;
            toolStripButton_StartDate.Enabled = true;
            toolStripButton_EndDate.Enabled = true;
            toolStripButton_Find.Enabled = true;
            toolStripComboBox_ReportType.Enabled = true;
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

        private bool InitExcel()
        {
            try
            {
                reoGridControl_Excel.Worksheets.Clear();
                unvell.ReoGrid.Worksheet worksheet = reoGridControl_Excel.CreateWorksheet();
                reoGridControl_Excel.Worksheets.Add(worksheet);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void Report_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (accessHelper != null)
                {
                    accessHelper.CloseAccessHelper();
                    accessHelper = null;
                }
            }
            catch (Exception)
            {
            }
        }

        private void toolStripButton_Cancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (accessHelper != null)
                {
                    accessHelper.CloseAccessHelper();
                    accessHelper = null;
                }
                Hide();
            }
            catch (Exception)
            {
            }
        }

        private void toolStripButton_Export_Click(object sender, EventArgs e)
        {
            try
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string FileName = saveFileDialog1.FileName;
                    reoGridControl_Excel.Save(FileName, unvell.ReoGrid.IO.FileFormat.Excel2007);
                    MessageBox.Show("报表导出成功！");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("报表导出失败，请联系管理员查看！");
            }
        }

        private void ReportType_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(toolStripComboBox_ReportType.Text))
                {
                    switch (toolStripComboBox_ReportType.Text)
                    {
                        case "程序日志":
                            CurrentReport = "Log";
                            break;
                        case "试漏日志":
                            CurrentReport = "LeakTestLog";
                            break;
                        case "打印计划":
                            CurrentReport = "PlanData";
                            break;
                        case "复检日志":
                            CurrentReport = "RepeatTest";
                            break;
                        default:
                            CurrentReport = "";
                            MessageBox.Show("当前报表类型错误，请重新选择！");
                            throw new Exception("error");
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                InitExcel();
            }
        }

        private void toolStripButton_StartDate_Click(object sender, EventArgs e)
        {
            try
            {
                SelectDate select = new SelectDate();
                select.ShowDialog();
                if (select.DialogResult == DialogResult.OK)
                {
                    toolStripTextBox_StartDate.Text = select.DateValue;
                }
                else
                {
                    toolStripTextBox_StartDate.Text = "";
                }
                select.Dispose();
                select = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("出现意外错误，返回：" + ex.ToString());
            }
        }

        private void toolStripButton_EndDate_Click(object sender, EventArgs e)
        {
            try
            {
                SelectDate select = new SelectDate();
                select.ShowDialog();
                if (select.DialogResult == DialogResult.OK)
                {
                    toolStripTextBox_EndDate.Text = select.DateValue;
                }
                else
                {
                    toolStripTextBox_EndDate.Text = "";
                }
                select.Dispose();
                select = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("出现意外错误，返回：" + ex.ToString());
            }
        }

        private void toolStripButton_Find_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(CurrentReport)) { MessageBox.Show("当前报表类型未正确选择！"); return; }
                if (string.IsNullOrEmpty(toolStripTextBox_StartDate.Text)||string.IsNullOrEmpty(toolStripTextBox_EndDate.Text)) { MessageBox.Show("当前日期参数未正确选择！"); return; }

                string SQL = "";
                switch (CurrentReport)
                {
                    case "Log":
                        SQL = "select * from Log Where OperationTime>=#" + toolStripTextBox_StartDate.Text + " 00:00:00# and OperationTime<=#" + toolStripTextBox_EndDate.Text + " 23:59:59#";
                        break;
                    case "LeakTestLog":
                        SQL = "select * from LeakTestLog Where 写入日期>=#" + toolStripTextBox_StartDate.Text + " 00:00:00# and 写入日期<=#" + toolStripTextBox_EndDate.Text + " 23:59:59#";
                        break;
                    case "PlanData":
                        SQL = "select * from Plan Where 计划日期>=#" + toolStripTextBox_StartDate.Text + " 00:00:00# and 计划日期<=#" + toolStripTextBox_EndDate.Text + " 23:59:59#";
                        break;
                    case "RepeatTest":
                        SQL= "select * from RepeatTest Where 日期>=#" + toolStripTextBox_StartDate.Text + " 00:00:00# and 日期<=#" + toolStripTextBox_EndDate.Text + " 23:59:59#";
                        break;
                }

                reoGridControl_Excel.Worksheets.Clear();
                unvell.ReoGrid.Worksheet SSheet = reoGridControl_Excel.CreateWorksheet();
                reoGridControl_Excel.Worksheets.Add(SSheet);

                DataTable Data = accessHelper.ExecuteDatatable(SQL);
                if (Data != null)
                {
                    SSheet.SetCols(Data.Columns.Count);
                    SSheet.SetRows(Data.Rows.Count + 10);

                    int Line = 0;
                    for(int i = 0; i < Data.Columns.Count; i++) { SSheet[Line, i] = Data.Columns[i].ColumnName; if (Data.Columns[i].ColumnName.Equals("Description")) { SSheet.SetColumnsWidth(i, 1, 600); } }

                    Line++;
                    foreach(DataRow dr in Data.Rows)
                    {
                        for(int i = 0; i < Data.Columns.Count; i++)
                        {
                            Type dataType = Data.Columns[i].DataType;
                            try
                            {
                                if (dataType.Name.Equals("DateTime")) SSheet[Line, i] = "`" + dr[i].ToString();
                                else SSheet[Line, i] = dr[i].ToString();
                            }
                            catch (Exception){ }
                        }
                        Line++;
                    }

                    Data.Clear();
                    Data = null;
                    MessageBox.Show("查询成功！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询数据库出错，返回错误：" + ex.ToString());
            }
        }
    }
}
