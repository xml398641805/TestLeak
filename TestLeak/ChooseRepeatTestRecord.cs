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
    public partial class ChooseRepeatTestRecord : Form
    {
        private DataTable RepeatTestDT;
        private AccessHelper accessHelper;
        public Parameter.PlanDataModel PlanData;
        public Parameter MyParameter;

        public ChooseRepeatTestRecord()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            toolStripButton_Confirm.Enabled = false;

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

            if (!InitRepeatTestData())
            {
                MessageBox.Show("初始化打印计划数据失败！");
                return;
            }

            toolStripButton_Confirm.Enabled = true;
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

        private bool InitRepeatTestData()
        {
            try
            {
                string SQL = "select * from RepeatTest Where 重新测试标识='等待复检'";
                RepeatTestDT = accessHelper.ExecuteDatatable(SQL);
                if (RepeatTestDT == null) return false;

                dataGridView_RepeatTest.DataSource = RepeatTestDT;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void toolStripButton_Confirm_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView_RepeatTest.SelectedRows.Count <= 0) { MessageBox.Show("未选择任何打印计划！"); return; }
                long ID = long.Parse(dataGridView_RepeatTest.SelectedRows[0].Cells[0].Value.ToString());
                DataRow[] drFind = RepeatTestDT.Select("序号=" + ID.ToString());
                if (drFind == null || drFind.Length <= 0) { MessageBox.Show("数据提取错误，请尝试重新打开!"); return; }
                PlanData = new Parameter.PlanDataModel();
                PlanData.ID = ID;
                PlanData.PartItem = MyParameter.PartList.Find(c => c.Item == Helper.Convert.ConvertString(drFind[0], "零件号"));
                if (PlanData.PartItem == null) { MessageBox.Show("零件详细信息提取错误，请尝试重新打开!"); return; }
                PlanData.PlanDate = "";
                PlanData.PlanData = 1;
                PlanData.FinishedData = 0;
                PlanData.ErrorData = 0;

                RepeatTestDT.Clear();
                RepeatTestDT = null;
                accessHelper.CloseAccessHelper();
                accessHelper = null;

                DialogResult = DialogResult.OK;
                Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作出现意外情况！");
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
    }
}
