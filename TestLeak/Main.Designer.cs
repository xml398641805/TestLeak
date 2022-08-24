
namespace TestLeak
{
    partial class Main
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel_Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_Start = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_Config = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_Report = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_Exit = new System.Windows.Forms.ToolStripButton();
            this.serialPort_ReceiveLeakTest = new System.IO.Ports.SerialPort(this.components);
            this.serialPort_ReceiveCheckCode = new System.IO.Ports.SerialPort(this.components);
            this.timer_Main = new System.Windows.Forms.Timer(this.components);
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBox_PartNumber = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBox_PlanDate = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBox_PlanData = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBox_FinishedData = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_ChoosePartNumber = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_ChooseRepeatTestRecord = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_PrintManually = new System.Windows.Forms.ToolStripButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dataGridView_RepeatTest = new System.Windows.Forms.DataGridView();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataGridView_Log = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_RepeatTest)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Log)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel_Status});
            this.statusStrip1.Location = new System.Drawing.Point(0, 636);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1327, 26);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(54, 20);
            this.toolStripStatusLabel1.Text = "状态：";
            // 
            // toolStripStatusLabel_Status
            // 
            this.toolStripStatusLabel_Status.Name = "toolStripStatusLabel_Status";
            this.toolStripStatusLabel_Status.Size = new System.Drawing.Size(90, 20);
            this.toolStripStatusLabel_Status.Text = "Wait............";
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_Start,
            this.toolStripSeparator1,
            this.toolStripButton_Config,
            this.toolStripSeparator2,
            this.toolStripButton_Report,
            this.toolStripSeparator3,
            this.toolStripButton_Exit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1327, 71);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton_Start
            // 
            this.toolStripButton_Start.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Start.Image")));
            this.toolStripButton_Start.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_Start.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Start.Name = "toolStripButton_Start";
            this.toolStripButton_Start.Size = new System.Drawing.Size(73, 68);
            this.toolStripButton_Start.Text = "开始检测";
            this.toolStripButton_Start.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton_Start.Click += new System.EventHandler(this.toolStripButton_Start_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 71);
            // 
            // toolStripButton_Config
            // 
            this.toolStripButton_Config.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Config.Image")));
            this.toolStripButton_Config.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_Config.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Config.Name = "toolStripButton_Config";
            this.toolStripButton_Config.Size = new System.Drawing.Size(73, 68);
            this.toolStripButton_Config.Text = "配置参数";
            this.toolStripButton_Config.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton_Config.Click += new System.EventHandler(this.toolStripButton_Config_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 71);
            // 
            // toolStripButton_Report
            // 
            this.toolStripButton_Report.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Report.Image")));
            this.toolStripButton_Report.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_Report.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Report.Name = "toolStripButton_Report";
            this.toolStripButton_Report.Size = new System.Drawing.Size(73, 68);
            this.toolStripButton_Report.Text = "查看报表";
            this.toolStripButton_Report.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton_Report.ToolTipText = "查看报表";
            this.toolStripButton_Report.Click += new System.EventHandler(this.toolStripButton_Report_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 71);
            // 
            // toolStripButton_Exit
            // 
            this.toolStripButton_Exit.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Exit.Image")));
            this.toolStripButton_Exit.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_Exit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Exit.Name = "toolStripButton_Exit";
            this.toolStripButton_Exit.Size = new System.Drawing.Size(73, 68);
            this.toolStripButton_Exit.Text = "退出程序";
            this.toolStripButton_Exit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton_Exit.Click += new System.EventHandler(this.toolStripButton_Exit_Click);
            // 
            // timer_Main
            // 
            this.timer_Main.Interval = 500;
            this.timer_Main.Tick += new System.EventHandler(this.Timer_Main_Tick);
            // 
            // toolStrip2
            // 
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripTextBox_PartNumber,
            this.toolStripSeparator4,
            this.toolStripLabel2,
            this.toolStripTextBox_PlanDate,
            this.toolStripSeparator5,
            this.toolStripLabel3,
            this.toolStripTextBox_PlanData,
            this.toolStripSeparator6,
            this.toolStripLabel4,
            this.toolStripTextBox_FinishedData,
            this.toolStripSeparator7,
            this.toolStripButton_ChoosePartNumber,
            this.toolStripSeparator9,
            this.toolStripButton_ChooseRepeatTestRecord,
            this.toolStripSeparator8,
            this.toolStripButton_PrintManually});
            this.toolStrip2.Location = new System.Drawing.Point(0, 71);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(1327, 31);
            this.toolStrip2.TabIndex = 3;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(84, 28);
            this.toolStripLabel1.Text = "零件选择：";
            // 
            // toolStripTextBox_PartNumber
            // 
            this.toolStripTextBox_PartNumber.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.toolStripTextBox_PartNumber.Name = "toolStripTextBox_PartNumber";
            this.toolStripTextBox_PartNumber.ReadOnly = true;
            this.toolStripTextBox_PartNumber.Size = new System.Drawing.Size(100, 31);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(84, 28);
            this.toolStripLabel2.Text = "计划日期：";
            // 
            // toolStripTextBox_PlanDate
            // 
            this.toolStripTextBox_PlanDate.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.toolStripTextBox_PlanDate.Name = "toolStripTextBox_PlanDate";
            this.toolStripTextBox_PlanDate.ReadOnly = true;
            this.toolStripTextBox_PlanDate.Size = new System.Drawing.Size(100, 31);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(114, 28);
            this.toolStripLabel3.Text = "计划打印数量：";
            // 
            // toolStripTextBox_PlanData
            // 
            this.toolStripTextBox_PlanData.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.toolStripTextBox_PlanData.Name = "toolStripTextBox_PlanData";
            this.toolStripTextBox_PlanData.ReadOnly = true;
            this.toolStripTextBox_PlanData.Size = new System.Drawing.Size(100, 31);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(69, 28);
            this.toolStripLabel4.Text = "已完成：";
            // 
            // toolStripTextBox_FinishedData
            // 
            this.toolStripTextBox_FinishedData.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.toolStripTextBox_FinishedData.Name = "toolStripTextBox_FinishedData";
            this.toolStripTextBox_FinishedData.ReadOnly = true;
            this.toolStripTextBox_FinishedData.Size = new System.Drawing.Size(100, 31);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripButton_ChoosePartNumber
            // 
            this.toolStripButton_ChoosePartNumber.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_ChoosePartNumber.Image")));
            this.toolStripButton_ChoosePartNumber.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_ChoosePartNumber.Name = "toolStripButton_ChoosePartNumber";
            this.toolStripButton_ChoosePartNumber.Size = new System.Drawing.Size(123, 28);
            this.toolStripButton_ChoosePartNumber.Text = "选择试漏计划";
            this.toolStripButton_ChoosePartNumber.Click += new System.EventHandler(this.toolStripButton_ChoosePartNumber_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripButton_ChooseRepeatTestRecord
            // 
            this.toolStripButton_ChooseRepeatTestRecord.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_ChooseRepeatTestRecord.Image")));
            this.toolStripButton_ChooseRepeatTestRecord.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_ChooseRepeatTestRecord.Name = "toolStripButton_ChooseRepeatTestRecord";
            this.toolStripButton_ChooseRepeatTestRecord.Size = new System.Drawing.Size(123, 28);
            this.toolStripButton_ChooseRepeatTestRecord.Text = "选择复检记录";
            this.toolStripButton_ChooseRepeatTestRecord.Click += new System.EventHandler(this.toolStripButton_ChooseRepeatTestRecord_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripButton_PrintManually
            // 
            this.toolStripButton_PrintManually.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_PrintManually.Image")));
            this.toolStripButton_PrintManually.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_PrintManually.Name = "toolStripButton_PrintManually";
            this.toolStripButton_PrintManually.Size = new System.Drawing.Size(123, 28);
            this.toolStripButton_PrintManually.Text = "标签手动补打";
            this.toolStripButton_PrintManually.Click += new System.EventHandler(this.toolStripButton_PrintManually_Click_1);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dataGridView_RepeatTest);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(724, 102);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(10);
            this.panel2.Size = new System.Drawing.Size(603, 534);
            this.panel2.TabIndex = 7;
            // 
            // dataGridView_RepeatTest
            // 
            this.dataGridView_RepeatTest.AllowUserToAddRows = false;
            this.dataGridView_RepeatTest.AllowUserToDeleteRows = false;
            this.dataGridView_RepeatTest.AllowUserToResizeColumns = false;
            this.dataGridView_RepeatTest.AllowUserToResizeRows = false;
            this.dataGridView_RepeatTest.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_RepeatTest.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column5,
            this.Column6,
            this.Column9,
            this.Column7,
            this.Column8,
            this.Column10,
            this.Column11,
            this.Column12,
            this.Column13,
            this.Column14});
            this.dataGridView_RepeatTest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_RepeatTest.Location = new System.Drawing.Point(10, 10);
            this.dataGridView_RepeatTest.Name = "dataGridView_RepeatTest";
            this.dataGridView_RepeatTest.ReadOnly = true;
            this.dataGridView_RepeatTest.RowHeadersVisible = false;
            this.dataGridView_RepeatTest.RowHeadersWidth = 51;
            this.dataGridView_RepeatTest.RowTemplate.Height = 27;
            this.dataGridView_RepeatTest.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView_RepeatTest.Size = new System.Drawing.Size(583, 514);
            this.dataGridView_RepeatTest.TabIndex = 2;
            // 
            // Column5
            // 
            this.Column5.DataPropertyName = "序号";
            this.Column5.HeaderText = "序号";
            this.Column5.MinimumWidth = 6;
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            this.Column5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column5.Width = 60;
            // 
            // Column6
            // 
            this.Column6.DataPropertyName = "零件号";
            this.Column6.HeaderText = "零件号";
            this.Column6.MinimumWidth = 6;
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            this.Column6.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column6.Width = 110;
            // 
            // Column9
            // 
            this.Column9.DataPropertyName = "编号";
            this.Column9.HeaderText = "编号";
            this.Column9.MinimumWidth = 6;
            this.Column9.Name = "Column9";
            this.Column9.ReadOnly = true;
            this.Column9.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column9.Width = 60;
            // 
            // Column7
            // 
            this.Column7.DataPropertyName = "日期";
            this.Column7.HeaderText = "时间";
            this.Column7.MinimumWidth = 6;
            this.Column7.Name = "Column7";
            this.Column7.ReadOnly = true;
            this.Column7.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column7.Width = 125;
            // 
            // Column8
            // 
            this.Column8.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column8.DataPropertyName = "重新测试标识";
            this.Column8.HeaderText = "状态";
            this.Column8.MinimumWidth = 120;
            this.Column8.Name = "Column8";
            this.Column8.ReadOnly = true;
            this.Column8.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column10
            // 
            this.Column10.DataPropertyName = "计划日期";
            this.Column10.HeaderText = "计划日期";
            this.Column10.MinimumWidth = 6;
            this.Column10.Name = "Column10";
            this.Column10.ReadOnly = true;
            this.Column10.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column10.Visible = false;
            this.Column10.Width = 125;
            // 
            // Column11
            // 
            this.Column11.DataPropertyName = "测试数据";
            this.Column11.HeaderText = "测试数据";
            this.Column11.MinimumWidth = 6;
            this.Column11.Name = "Column11";
            this.Column11.ReadOnly = true;
            this.Column11.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column11.Visible = false;
            this.Column11.Width = 125;
            // 
            // Column12
            // 
            this.Column12.DataPropertyName = "重新测试日期";
            this.Column12.HeaderText = "重新测试日期";
            this.Column12.MinimumWidth = 6;
            this.Column12.Name = "Column12";
            this.Column12.ReadOnly = true;
            this.Column12.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column12.Visible = false;
            this.Column12.Width = 125;
            // 
            // Column13
            // 
            this.Column13.DataPropertyName = "重新测试结果";
            this.Column13.HeaderText = "重新测试结果";
            this.Column13.MinimumWidth = 6;
            this.Column13.Name = "Column13";
            this.Column13.ReadOnly = true;
            this.Column13.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column13.Visible = false;
            this.Column13.Width = 125;
            // 
            // Column14
            // 
            this.Column14.DataPropertyName = "重新测试数据";
            this.Column14.HeaderText = "重新测试数据";
            this.Column14.MinimumWidth = 6;
            this.Column14.Name = "Column14";
            this.Column14.ReadOnly = true;
            this.Column14.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column14.Visible = false;
            this.Column14.Width = 125;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dataGridView_Log);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 102);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(10);
            this.panel1.Size = new System.Drawing.Size(724, 534);
            this.panel1.TabIndex = 8;
            // 
            // dataGridView_Log
            // 
            this.dataGridView_Log.AllowUserToAddRows = false;
            this.dataGridView_Log.AllowUserToDeleteRows = false;
            this.dataGridView_Log.AllowUserToResizeColumns = false;
            this.dataGridView_Log.AllowUserToResizeRows = false;
            this.dataGridView_Log.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridView_Log.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_Log.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4});
            this.dataGridView_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_Log.Location = new System.Drawing.Point(10, 10);
            this.dataGridView_Log.Name = "dataGridView_Log";
            this.dataGridView_Log.ReadOnly = true;
            this.dataGridView_Log.RowHeadersVisible = false;
            this.dataGridView_Log.RowHeadersWidth = 51;
            this.dataGridView_Log.RowTemplate.Height = 27;
            this.dataGridView_Log.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView_Log.Size = new System.Drawing.Size(704, 514);
            this.dataGridView_Log.TabIndex = 9;
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "ID";
            this.Column1.HeaderText = "序号";
            this.Column1.MinimumWidth = 80;
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column1.Width = 80;
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "Date";
            this.Column2.HeaderText = "时间";
            this.Column2.MinimumWidth = 150;
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column2.Width = 150;
            // 
            // Column3
            // 
            this.Column3.DataPropertyName = "Module";
            this.Column3.HeaderText = "模块";
            this.Column3.MinimumWidth = 150;
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column3.Width = 150;
            // 
            // Column4
            // 
            this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column4.DataPropertyName = "Description";
            this.Column4.HeaderText = "描述";
            this.Column4.MinimumWidth = 500;
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1327, 662);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.toolStrip2);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "试漏测试打标";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_RepeatTest)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Log)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton_Start;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton_Config;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_Status;
        private System.Windows.Forms.ToolStripButton toolStripButton_Report;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButton_Exit;
        private System.IO.Ports.SerialPort serialPort_ReceiveLeakTest;
        private System.IO.Ports.SerialPort serialPort_ReceiveCheckCode;
        private System.Windows.Forms.Timer timer_Main;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox_PartNumber;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox_PlanDate;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox_PlanData;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox_FinishedData;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripButton toolStripButton_ChoosePartNumber;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dataGridView_Log;
        private System.Windows.Forms.DataGridView dataGridView_RepeatTest;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripButton toolStripButton_ChooseRepeatTestRecord;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripButton toolStripButton_PrintManually;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column10;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column11;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column12;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column13;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column14;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
    }
}

