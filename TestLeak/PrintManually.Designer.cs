
namespace TestLeak
{
    partial class PrintManually
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label33 = new System.Windows.Forms.Label();
            this.button_Print_TestPrint = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_Label = new System.Windows.Forms.TextBox();
            this.numericUpDown_PrintCount = new System.Windows.Forms.NumericUpDown();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_PrintCount)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.numericUpDown_PrintCount);
            this.panel1.Controls.Add(this.textBox_Label);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label33);
            this.panel1.Controls.Add(this.button_Print_TestPrint);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(545, 188);
            this.panel1.TabIndex = 0;
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(92, 100);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(67, 15);
            this.label33.TabIndex = 39;
            this.label33.Text = "零件号：";
            // 
            // button_Print_TestPrint
            // 
            this.button_Print_TestPrint.Location = new System.Drawing.Point(347, 97);
            this.button_Print_TestPrint.Name = "button_Print_TestPrint";
            this.button_Print_TestPrint.Size = new System.Drawing.Size(119, 27);
            this.button_Print_TestPrint.TabIndex = 37;
            this.button_Print_TestPrint.Text = "打印";
            this.button_Print_TestPrint.UseVisualStyleBackColor = true;
            this.button_Print_TestPrint.Click += new System.EventHandler(this.button_Print_TestPrint_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(77, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 15);
            this.label1.TabIndex = 40;
            this.label1.Text = "标签数量：";
            // 
            // textBox_Label
            // 
            this.textBox_Label.Location = new System.Drawing.Point(165, 97);
            this.textBox_Label.Name = "textBox_Label";
            this.textBox_Label.ReadOnly = true;
            this.textBox_Label.Size = new System.Drawing.Size(165, 25);
            this.textBox_Label.TabIndex = 42;
            // 
            // numericUpDown_PrintCount
            // 
            this.numericUpDown_PrintCount.Location = new System.Drawing.Point(165, 52);
            this.numericUpDown_PrintCount.Name = "numericUpDown_PrintCount";
            this.numericUpDown_PrintCount.Size = new System.Drawing.Size(165, 25);
            this.numericUpDown_PrintCount.TabIndex = 43;
            this.numericUpDown_PrintCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // PrintManually
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(545, 188);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PrintManually";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "手动打印";
            this.Load += new System.EventHandler(this.PrintManually_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_PrintCount)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Button button_Print_TestPrint;
        private System.Windows.Forms.NumericUpDown numericUpDown_PrintCount;
        private System.Windows.Forms.TextBox textBox_Label;
    }
}