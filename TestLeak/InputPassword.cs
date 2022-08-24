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
    public partial class InputPassword : Form
    {
        private string Password="13579";

        public InputPassword()
        {
            InitializeComponent();
        }

        private void Password_Load(object sender, EventArgs e)
        {
            textBox_Password.Enabled = false;
            button_Confirm.Enabled = false;
            Parameter MyParamter = MyConfig.GetParameter();
            if (MyParamter == null)
            {
                MessageBox.Show("参数初始化错误！");
                return;
            }
            if(!string.IsNullOrEmpty(MyParamter.Password)) Password = Helper.Convert.RSADecrypt(MyParamter.Password);

            textBox_Password.Enabled = true;
            button_Confirm.Enabled = true;
        }
        
        
        
        private void Password_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void Password_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (Password.Equals(textBox_Password.Text))
                    {
                        DialogResult = DialogResult.OK;
                        Hide();
                    }
                    else
                    {
                        MessageBox.Show("密码错误！");
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("程序密码检测错误！");
            }
        }

        private void button_Confirm_Click(object sender, EventArgs e)
        {
            try
            {
                if (Password.Equals(textBox_Password.Text))
                {
                    DialogResult = DialogResult.OK;
                    Hide();
                }
                else
                {
                    MessageBox.Show("密码错误!");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("程序密码检测错误！");
            }
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
