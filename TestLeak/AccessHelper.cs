using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;

namespace Helper
{
    class AccessHelper
    {
        private OleDbConnection conn = null;
        private OleDbCommand cmd = null;
        private OleDbDataReader sdr;
        private string conString;
        private string _DataFile = Directory.GetCurrentDirectory().ToString() + @"\Result.mdb";

        public bool InitAccessHelper()
        {
            try
            {
                conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + _DataFile + ";Persist Security Info=False";
                conn = new OleDbConnection(conString);
                conn.Open();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public int ExecuteCommand(string SQL)
        {
            try
            {
                cmd = new OleDbCommand(SQL, conn);
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return - 1;
            }
        }

        public DataTable ExecuteDatatable(string SQL)
        {
            try
            {
                DataTable dt = new DataTable();
                cmd = new OleDbCommand(SQL, conn);
                sdr = cmd.ExecuteReader();
                dt.Load(sdr);
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool CloseAccessHelper()
        {
            try
            {
                if (sdr != null)
                {
                    sdr.Close();
                    sdr = null;
                }
                if (cmd != null)
                {
                    cmd = null;
                }
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn = null;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
