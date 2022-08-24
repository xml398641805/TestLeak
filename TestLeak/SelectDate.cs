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
    public partial class SelectDate : Form
    {
        public string DateValue;

        public SelectDate()
        {
            InitializeComponent();
        }

        private void SelectDate_Load(object sender, EventArgs e)
        {
            DateValue = "";
        }

        private void Calendar_DateChanged(object sender, DateRangeEventArgs e)
        {

        }

        private void Calendar_DateSelected(object sender, DateRangeEventArgs e)
        {
            DateValue = monthCalendar1.SelectionStart.ToShortDateString();
            DialogResult = DialogResult.OK;
            Hide();
        }
    }
}
