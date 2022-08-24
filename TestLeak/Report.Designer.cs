
namespace TestLeak
{
    partial class Report
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Report));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel_Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_Export = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_Cancel = new System.Windows.Forms.ToolStripButton();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBox_ReportType = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBox_StartDate = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripButton_StartDate = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBox_EndDate = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripButton_EndDate = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_Find = new System.Windows.Forms.ToolStripButton();
            this.reoGridControl_Excel = new unvell.ReoGrid.ReoGridControl();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1,
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel_Status});
            this.statusStrip1.Location = new System.Drawing.Point(0, 584);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1013, 26);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(88, 20);
            this.toolStripStatusLabel1.Text = "Progress：";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(200, 18);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(54, 20);
            this.toolStripStatusLabel2.Text = "状态：";
            // 
            // toolStripStatusLabel_Status
            // 
            this.toolStripStatusLabel_Status.Name = "toolStripStatusLabel_Status";
            this.toolStripStatusLabel_Status.Size = new System.Drawing.Size(118, 20);
            this.toolStripStatusLabel_Status.Text = "Wait...................";
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_Export,
            this.toolStripSeparator1,
            this.toolStripButton_Cancel});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1013, 59);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton_Export
            // 
            this.toolStripButton_Export.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Export.Image")));
            this.toolStripButton_Export.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_Export.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Export.Name = "toolStripButton_Export";
            this.toolStripButton_Export.Size = new System.Drawing.Size(73, 56);
            this.toolStripButton_Export.Text = "导出报表";
            this.toolStripButton_Export.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton_Export.Click += new System.EventHandler(this.toolStripButton_Export_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 59);
            // 
            // toolStripButton_Cancel
            // 
            this.toolStripButton_Cancel.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Cancel.Image")));
            this.toolStripButton_Cancel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_Cancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Cancel.Name = "toolStripButton_Cancel";
            this.toolStripButton_Cancel.Size = new System.Drawing.Size(73, 56);
            this.toolStripButton_Cancel.Text = "取消退出";
            this.toolStripButton_Cancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton_Cancel.Click += new System.EventHandler(this.toolStripButton_Cancel_Click);
            // 
            // toolStrip2
            // 
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripComboBox_ReportType,
            this.toolStripSeparator2,
            this.toolStripLabel2,
            this.toolStripTextBox_StartDate,
            this.toolStripButton_StartDate,
            this.toolStripSeparator4,
            this.toolStripLabel3,
            this.toolStripTextBox_EndDate,
            this.toolStripButton_EndDate,
            this.toolStripSeparator3,
            this.toolStripButton_Find});
            this.toolStrip2.Location = new System.Drawing.Point(0, 59);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(1013, 28);
            this.toolStrip2.TabIndex = 2;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(84, 25);
            this.toolStripLabel1.Text = "报表选择：";
            // 
            // toolStripComboBox_ReportType
            // 
            this.toolStripComboBox_ReportType.Items.AddRange(new object[] {
            "程序日志",
            "试漏日志",
            "打印计划",
            "复检日志"});
            this.toolStripComboBox_ReportType.Name = "toolStripComboBox_ReportType";
            this.toolStripComboBox_ReportType.Size = new System.Drawing.Size(121, 28);
            this.toolStripComboBox_ReportType.TextChanged += new System.EventHandler(this.ReportType_TextChanged);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 28);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(84, 25);
            this.toolStripLabel2.Text = "开始日期：";
            // 
            // toolStripTextBox_StartDate
            // 
            this.toolStripTextBox_StartDate.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.toolStripTextBox_StartDate.Name = "toolStripTextBox_StartDate";
            this.toolStripTextBox_StartDate.ReadOnly = true;
            this.toolStripTextBox_StartDate.Size = new System.Drawing.Size(100, 28);
            // 
            // toolStripButton_StartDate
            // 
            this.toolStripButton_StartDate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_StartDate.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_StartDate.Image")));
            this.toolStripButton_StartDate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_StartDate.Name = "toolStripButton_StartDate";
            this.toolStripButton_StartDate.Size = new System.Drawing.Size(29, 25);
            this.toolStripButton_StartDate.Text = "toolStripButton3";
            this.toolStripButton_StartDate.Click += new System.EventHandler(this.toolStripButton_StartDate_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 28);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(84, 25);
            this.toolStripLabel3.Text = "结束日期：";
            // 
            // toolStripTextBox_EndDate
            // 
            this.toolStripTextBox_EndDate.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.toolStripTextBox_EndDate.Name = "toolStripTextBox_EndDate";
            this.toolStripTextBox_EndDate.ReadOnly = true;
            this.toolStripTextBox_EndDate.Size = new System.Drawing.Size(100, 28);
            // 
            // toolStripButton_EndDate
            // 
            this.toolStripButton_EndDate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_EndDate.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_EndDate.Image")));
            this.toolStripButton_EndDate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_EndDate.Name = "toolStripButton_EndDate";
            this.toolStripButton_EndDate.Size = new System.Drawing.Size(29, 25);
            this.toolStripButton_EndDate.Text = "toolStripButton4";
            this.toolStripButton_EndDate.Click += new System.EventHandler(this.toolStripButton_EndDate_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 28);
            // 
            // toolStripButton_Find
            // 
            this.toolStripButton_Find.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Find.Image")));
            this.toolStripButton_Find.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Find.Name = "toolStripButton_Find";
            this.toolStripButton_Find.Size = new System.Drawing.Size(63, 25);
            this.toolStripButton_Find.Text = "查询";
            this.toolStripButton_Find.Click += new System.EventHandler(this.toolStripButton_Find_Click);
            // 
            // reoGridControl_Excel
            // 
            this.reoGridControl_Excel.BackColor = System.Drawing.Color.White;
            this.reoGridControl_Excel.ColumnHeaderContextMenuStrip = null;
            this.reoGridControl_Excel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reoGridControl_Excel.LeadHeaderContextMenuStrip = null;
            this.reoGridControl_Excel.Location = new System.Drawing.Point(0, 87);
            this.reoGridControl_Excel.Name = "reoGridControl_Excel";
            this.reoGridControl_Excel.RowHeaderContextMenuStrip = null;
            this.reoGridControl_Excel.Script = null;
            this.reoGridControl_Excel.SheetTabContextMenuStrip = null;
            this.reoGridControl_Excel.SheetTabNewButtonVisible = false;
            this.reoGridControl_Excel.SheetTabVisible = false;
            this.reoGridControl_Excel.SheetTabWidth = 60;
            this.reoGridControl_Excel.ShowScrollEndSpacing = true;
            this.reoGridControl_Excel.Size = new System.Drawing.Size(1013, 497);
            this.reoGridControl_Excel.TabIndex = 3;
            this.reoGridControl_Excel.Text = "reoGridControl1";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "xlsx";
            this.saveFileDialog1.Filter = "Excel|*.xlsx";
            // 
            // Report
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1013, 610);
            this.Controls.Add(this.reoGridControl_Excel);
            this.Controls.Add(this.toolStrip2);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Report";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "查看报表";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Report_FormClosing);
            this.Load += new System.EventHandler(this.Report_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_Status;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton_Export;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton_Cancel;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox_ReportType;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox_StartDate;
        private System.Windows.Forms.ToolStripButton toolStripButton_StartDate;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox_EndDate;
        private System.Windows.Forms.ToolStripButton toolStripButton_EndDate;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButton_Find;
        private unvell.ReoGrid.ReoGridControl reoGridControl_Excel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}