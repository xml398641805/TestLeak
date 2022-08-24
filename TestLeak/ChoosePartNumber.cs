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
    public partial class ChoosePartNumber : Form
    {
        private DataTable PlanDt;
        private AccessHelper accessHelper;
        public Parameter.PlanDataModel PlanData;
        public Parameter MyParameter;

        public ChoosePartNumber()
        {
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            toolStripButton_Confirm.Enabled = false;
            dateTimePicker_PlanDate.Enabled = false;

            if (!InitParameter())
            {
                MessageBox.Show("初始化程序参数失败！");
                return;
            }

            if (!InitAccessFile())
            {
                MessageBox.Show("初始化数据库链接失败！");
                return;
            }

            if (!InitPlanData())
            {
                MessageBox.Show("初始化打印计划数据失败！");
                return;
            }

            toolStripButton_Confirm.Enabled = true;
            //dateTimePicker_PlanDate.Enabled = true;
        }

        private bool InitParameter()
        {
            try
            {
                MyParameter = MyConfig.GetParameter();
                if (MyParameter == null) return false; else return true;
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
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void PlanDate_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!InitPlanData())
                {
                    MessageBox.Show("读取打印计划数据失败！");
                    return;
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
                accessHelper.CloseAccessHelper();
                accessHelper = null;
            }
            catch (Exception)
            {
            }
            finally
            {
                Hide();
            }
        }

        private void toolStripButton_Confirm_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView_PlanData.SelectedRows.Count <= 0) { MessageBox.Show("未选择任何打印计划！"); return; }
                long ID = long.Parse(dataGridView_PlanData.SelectedRows[0].Cells[0].Value.ToString());
                DataRow[] drFind = PlanDt.Select("ID=" + ID.ToString());
                if (drFind == null || drFind.Length <= 0) { MessageBox.Show("数据提取错误，请尝试重新打开!"); return; }
                PlanData = new Parameter.PlanDataModel();
                PlanData.ID = ID;
                PlanData.PartItem = MyParameter.PartList.Find(c => c.Item == Helper.Convert.ConvertString(drFind[0], "零件号"));
                if (PlanData.PartItem == null) { MessageBox.Show("零件详细信息提取错误，请尝试重新打开!"); return; }
                PlanData.PlanDate = Helper.Convert.ConvertString(drFind[0], "计划日期");
                PlanData.PlanData = Helper.Convert.ConvertInt(drFind[0], "计划数量");
                PlanData.FinishedData = Helper.Convert.ConvertInt(drFind[0], "已完成数量");
                PlanData.ErrorData = Helper.Convert.ConvertInt(drFind[0], "故障数量");

                PlanDt.Clear();
                PlanDt = null;
                accessHelper.CloseAccessHelper();
                accessHelper = null;

                DialogResult = DialogResult.OK;
                Hide();
            }
            catch (Exception)
            {

            }
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
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
    }
}
