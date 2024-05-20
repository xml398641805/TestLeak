using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Helper;
using ZXing;
using ZXing.Common;

namespace TestLeak
{
    public partial class Main : Form
    {
        private Parameter MyParameter;
        private DataTable LogDT;
        private DataTable RepeatTestDT;
        private AccessHelper accessHelper;
        private enum WorkType { InitWork, WaitReceiveLeakTest, WaitPrintPartCode, WaitReceivePrintCheckCode, PrintCheckErrorCode, StopWork }
        private WorkType CurrentWorkType;
        private bool ThreadProgram;
        private int CurrentPrintLabelCount;
        private enum AlertType { Init, MeetPlanLimit, PrintError, CheckLabelError, LeakError, RepeatTestFinished }
        private string CurrentDirectory = Directory.GetCurrentDirectory().ToString();
        private bool RepeatTestModel;

        private List<LabelTemplateModel> labelTemplateModels;
        private LabelTemplateModel CurrentLabelTemplate;

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            toolStripButton_Start.Enabled = false;
            toolStripButton_Config.Enabled = false;
            toolStripButton_Report.Enabled = false;
            toolStripButton_ChoosePartNumber.Enabled = false;
            toolStripButton_ChooseRepeatTestRecord.Enabled = false;
            toolStripButton_PrintManually.Enabled = false;

            if (!InitLog())
            {
                MessageBox.Show("初始化日志列表失败，请联系管理员处理！");
                return;
            }

            if(!InitParameter())
            {
                SetLog("初始化", "初始程序参数错误，请查看配置文件是否存在！");
                return;
            }

            if (!InitAccessFile())
            {
                SetLog("初始化", "初始化数据库文件失败，请查看文件是否存在！");
                return;
            }

            if (!InitRepeatTestDT())
            {
                SetLog("初始化","初始化等待复检数据失败，请联系管理员处理！");
                return;
            }

            try { labelTemplateModels = MyLabelTemplate.InitLabelTemplate(); }
            catch(Exception ex)
            {
                SetLog("初始化", "初始化打印模版失败，请联系管理员处理！");
                return;
            }

            MyParameter.PlanData = null;

            CurrentWorkType = WorkType.StopWork;
            //toolStripButton_Start.Enabled = true;
            toolStripButton_Config.Enabled = true;
            toolStripButton_Report.Enabled = true;
            toolStripButton_ChoosePartNumber.Enabled = true;
            toolStripButton_ChooseRepeatTestRecord.Enabled = true;

            SetLog("初始化", "程序初始化完成，准备开始操作!");
            Invoke(new CallbackPlayAlert(PlayAlert), new object[] { AlertType.Init });
        }

        private bool InitLog()
        {
            try
            {
                LogDT = new DataTable();
                LogDT.Columns.Add("ID");
                LogDT.Columns.Add("Date");
                LogDT.Columns.Add("Module");
                LogDT.Columns.Add("Description");
                dataGridView_Log.DataSource = LogDT;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
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

        private bool InitRepeatTestDT()
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

        private delegate void CallbackSetLog(string Module, string val, string WriteSign = "Yes");
        private void SetLog(string Module, string val, string WriteSign="Yes")
        {
            try
            {
                if (LogDT.Rows.Count > 300) LogDT.Rows.Clear();

                DataRow dr = LogDT.NewRow();
                dr["ID"] = (LogDT.Rows.Count + 1).ToString().PadLeft(3, '0');
                dr["Date"] = DateTime.Now.ToString();
                dr["Module"] = Module;
                dr["Description"] = val;
                LogDT.Rows.InsertAt(dr, 0);
                dataGridView_Log.CurrentCell = dataGridView_Log.Rows[0].Cells[0];

                if (WriteSign.Equals("Yes")) WriteLog(Module, val);
            }
            catch (Exception)
            {
                toolStripStatusLabel_Status.Text = "日志写入错误！";
            }
        }
        
        private void WriteLog(string Module, string Val)
        {
            try
            {
                if (accessHelper == null) SetLog("错误提示", "数据库状态不正确，请联系管理员查看！", "No");
                string SQL = "insert into Log(Operation,OperationTime,Description) values('" + Module + "','" + DateTime.Now.ToString() + "','" + Val.Replace(",", "，") + "')";
                if(accessHelper.ExecuteCommand(SQL)<=0) SetLog("错误提示", "数据库写入不正确，请联系管理员查看！", "No");
            }
            catch (Exception)
            {
                SetLog("错误提示", "数据库操作出错，请联系管理员查看！", "No");
            }
        }

        private void toolStripButton_Start_Click(object sender, EventArgs e)
        {
            StartCheck();
        }

        private void StartCheck()
        {
            try
            {
                if(MyParameter.PlanData==null||string.IsNullOrEmpty(MyParameter.PlanData.PartItem.Item)) { MessageBox.Show("当前未选择正确的零件号！"); return; }
                if(toolStripButton_Start.Text.Equals("开始检测")&&MyParameter.PlanData.FinishedData>=MyParameter.PlanData.PlanData) { MessageBox.Show("当前零件打印计划已经完成！"); return; }

                if (toolStripButton_Start.Text.Equals("开始检测"))
                {
                    if (CurrentWorkType != WorkType.StopWork) { MessageBox.Show("当前程序状态不正确，请关闭后重新运行！"); return; }
                    toolStripButton_Start.Text = "停止检测";
                    CurrentWorkType = WorkType.InitWork;
                    if (!InitComPort())
                    {
                        toolStripButton_Start.Text = "开始检测";
                        CurrentWorkType = WorkType.StopWork;
                        ReleaseComPort();
                        return;
                    }
                    
                    ThreadProgram = false;
                    timer_Main.Enabled = true;
                    timer_Main.Start();
                    SetLog("开始检测", "启动定时器，开始接收试漏数据，并等待打印标签！");
                    CurrentWorkType = WorkType.WaitReceiveLeakTest;

                    toolStripButton_Config.Enabled = false;
                    toolStripButton_Report.Enabled = false;
                    toolStripButton_ChoosePartNumber.Enabled = false;
                    toolStripButton_ChooseRepeatTestRecord.Enabled = false;
                    toolStripButton_PrintManually.Enabled = false;
                }
                else
                {
                    CurrentWorkType = WorkType.StopWork;
                    if (timer_Main.Enabled)
                    {
                        timer_Main.Stop();
                        timer_Main.Enabled = false;
                    }
                    ReleaseComPort();
                    SetLog("结束检测", "已经结束本次检测工作！");

                    Invoke(new CallbackSetButtonEnabled(SetButtonEnabled));
                }
            }
            catch (Exception)
            {

            }
        }

        private delegate void CallbackSetButtonEnabled();
        private void SetButtonEnabled()
        {
            toolStripButton_Start.Text = "开始检测";

            toolStripButton_Config.Enabled = true;
            toolStripButton_Report.Enabled = true;
            toolStripButton_ChoosePartNumber.Enabled = true;
            toolStripButton_ChooseRepeatTestRecord.Enabled = true;
        }

        private bool InitComPort()
        {
            try
            {
                serialPort_ReceiveLeakTest.PortName = MyParameter.LeakTestSettings.ComConfig.ComPort;
                serialPort_ReceiveLeakTest.BaudRate = MyParameter.LeakTestSettings.ComConfig.Speed;
                serialPort_ReceiveLeakTest.DataBits = MyParameter.LeakTestSettings.ComConfig.DataBit;
                serialPort_ReceiveLeakTest.StopBits = MyParameter.LeakTestSettings.ComConfig.StopBit == 1 ? System.IO.Ports.StopBits.One : System.IO.Ports.StopBits.Two;
                serialPort_ReceiveLeakTest.Parity = MyParameter.LeakTestSettings.ComConfig.CheckType == "Even" ? System.IO.Ports.Parity.Even : System.IO.Ports.Parity.Odd;
                try { serialPort_ReceiveLeakTest.Open(); } 
                catch(Exception ex)
                {
                    SetLog("开始检测", "无法初始化试漏设备接口设置，返回错误：初始化试漏COM口错误，请检查配置参数！");
                    throw new Exception(ex.ToString());
                }
                SetLog("开始检测", "完成初始化试漏设备接口设置！");

                if (MyParameter.PrintCheckSettings.EnabledPrintCheck)
                {
                    serialPort_ReceiveCheckCode.PortName = MyParameter.PrintCheckSettings.ComConfig.ComPort;
                    serialPort_ReceiveCheckCode.BaudRate = MyParameter.PrintCheckSettings.ComConfig.Speed;
                    serialPort_ReceiveCheckCode.DataBits = MyParameter.PrintCheckSettings.ComConfig.DataBit;
                    serialPort_ReceiveCheckCode.StopBits = MyParameter.PrintCheckSettings.ComConfig.StopBit == 1 ? System.IO.Ports.StopBits.One : System.IO.Ports.StopBits.Two;
                    if (MyParameter.PrintCheckSettings.ComConfig.CheckType.Equals("Even")) serialPort_ReceiveCheckCode.Parity = System.IO.Ports.Parity.Even;
                    else if (MyParameter.PrintCheckSettings.ComConfig.CheckType.Equals("Odd")) serialPort_ReceiveCheckCode.Parity = System.IO.Ports.Parity.Odd;
                    else if (MyParameter.PrintCheckSettings.ComConfig.CheckType.Equals("None")) serialPort_ReceiveCheckCode.Parity = System.IO.Ports.Parity.None;
                    else
                    {
                        SetLog("开始检测", "无法初始化打印验证接收接口设置，返回错误：初始化打印校验COM口错误，端口校验位参数不正确！");
                        throw new Exception("无法初始化打印验证接收接口设置，返回错误：初始化打印校验COM口错误，端口校验位参数不正确！");
                    }
                    try { serialPort_ReceiveCheckCode.Open(); } 
                    catch(Exception ex)
                    {
                        SetLog("开始检测", "无法初始化打印验证接收接口设置，返回错误：初始化打印校验COM口错误，请检查配置参数！");
                        throw new Exception(ex.ToString());
                    }
                    SetLog("开始检测", "完成初始化打印验证接收接口设置！");
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool ReleaseComPort()
        {
            try
            {
                if (serialPort_ReceiveLeakTest.IsOpen)
                {
                    try { serialPort_ReceiveLeakTest.Close(); }
                    catch(Exception ex)
                    {
                        SetLog("结束检测", "关闭试漏设备接口失败，返回错误：" + ex.ToString());
                        throw new Exception("error");
                    }
                    SetLog("结束检测", "成功关闭试漏设备接口！");
                }

                if (MyParameter.PrintCheckSettings.EnabledPrintCheck)
                {
                    if (serialPort_ReceiveCheckCode.IsOpen)
                    {
                        try { serialPort_ReceiveCheckCode.Close(); }
                        catch (Exception ex)
                        {
                            SetLog("结束检测", "关闭打印验证接收接口失败，返回错误：" + ex.ToString());
                            throw new Exception("error");
                        }
                        SetLog("结束检测", "成功关闭打印验证接收接口！");
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void toolStripButton_Config_Click(object sender, EventArgs e)
        {
            try
            {
                if (!toolStripButton_Start.Text.Equals("开始检测"))
                {
                    MessageBox.Show("请先暂时检测功能，然后配置程序参数！");
                    return;
                }

                /*InputPassword Password = new InputPassword();
                if (Password.ShowDialog() != DialogResult.OK)
                {
                    Password.Dispose();
                    return;
                }*/

                //toolStripButton_Start.Enabled = false;
                toolStripButton_Config.Enabled = false;
                toolStripButton_Report.Enabled = false;

                Config config = new Config();
                config.ShowDialog();
                if (config.DialogResult == DialogResult.OK)
                {
                    if (!InitParameter())
                    {
                        SetLog("初始化", "初始程序参数错误，请查看配置文件是否存在！");
                        return;
                    }
                    else
                    {
                        toolStripTextBox_PartNumber.Text = "";
                        toolStripTextBox_PlanData.Text = "";
                        toolStripTextBox_PlanDate.Text = "";
                        toolStripTextBox_FinishedData.Text = "";

                        SetLog("初始化", "重新配置程序参数，新参数已经生效！");
                    }
                    MyLabelTemplate.InitLabelTemplate();
                }
                //toolStripButton_Start.Enabled = true;
                toolStripButton_Config.Enabled = true;
                toolStripButton_Report.Enabled = true;

            }
            catch (Exception)
            {
                MessageBox.Show("运行参数配置过程出现意外情况，请联系管理员处理！");
            }
        }

        private void toolStripButton_Report_Click(object sender, EventArgs e)
        {
            try
            {
                if (!toolStripButton_Start.Text.Equals("开始检测"))
                {
                    MessageBox.Show("程序运行过程中无法查看报表，请先停止检测！");
                    return;
                }
                Report report = new Report();
                report.ShowDialog();
            }
            catch (Exception)
            {

            }
        }

        private void toolStripButton_Exit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!toolStripButton_Start.Text.Equals("开始检测"))
                {
                    MessageBox.Show("请先停止当前的检测操作，然后再关闭程序！");
                    return;
                }
                else if (accessHelper != null)
                {
                    accessHelper.CloseAccessHelper();
                    accessHelper = null;
                }
                Application.Exit();
            }
            catch (Exception)
            {

            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (!toolStripButton_Start.Text.Equals("开始检测"))
                {
                    MessageBox.Show("请先停止当前的检测操作，然后再关闭程序！");
                    e.Cancel = true;
                    return;
                }
                else if (accessHelper != null)
                {
                    accessHelper.CloseAccessHelper();
                    accessHelper = null;
                }
            }
            catch (Exception)
            {

            }
        }

        private void Timer_Main_Tick(object sender, EventArgs e)
        {
            if (CurrentWorkType == WorkType.WaitReceiveLeakTest && !ThreadProgram)
            {
                new Thread(new ThreadStart(StartReceiveLeakTestThread)).Start();
            }
            else if (CurrentWorkType == WorkType.WaitReceivePrintCheckCode && !ThreadProgram)
            {
                new Thread(new ThreadStart(StartReceivePrintCheckThread)).Start();
            }
        }

        private void StartReceiveLeakTestThread()
        {
            try
            {
                ThreadProgram = true;
                int ByteCount = serialPort_ReceiveLeakTest.BytesToRead;
                if (ByteCount > 0)
                {
                    string ScanByte = "";
                    try
                    {
                        ScanByte = serialPort_ReceiveLeakTest.ReadExisting();
                        serialPort_ReceiveLeakTest.DiscardInBuffer();
                    }
                    catch (Exception)
                    {
                        Invoke(new CallbackSetLog(SetLog), new object[] { "错误提示", "处理试漏设备输出的信息失败，返回的错误：读取COM口缓冲区数据失败！", "Yes" });
                        throw new Exception("error");
                    }
                    Invoke(new CallbackSetLog(SetLog), new object[] {"开始检测", "接收到一条试漏数据", "Yes" });
                    Invoke(new CallbackSetLog(SetLog), new object[] { "开始检测", "试漏数据：" + ScanByte, "Yes" });

                    //保存试漏接收时间点，用于判断试漏时间间隔报警
                    if (MyParameter.LeakTestSettings.EnabledGapAlert)
                    {
                        if (DateTime.Now.Subtract(MyParameter.LeakTestSettings.LastLeakTime).TotalSeconds < MyParameter.LeakTestSettings.GapAlertValue) { }
                    }
                    MyParameter.LeakTestSettings.LastLeakTime = DateTime.Now;

                    if (!AnalyseLeakData(ScanByte)) throw new Exception("分析数据错误！");
                }
            }
            catch (Exception ex)
            {
                Invoke(new CallbackSetLog(SetLog), new object[] { "错误提示", "检测过程已经结束，请检查后再重新开始！", "Yes" });
                StartCheck();
            }
            finally
            {
                ThreadProgram = false;
            }
        }

        private bool AnalyseLeakData(string Code)
        {
            try
            {
                string EquipmentDate = "", PressValue = "", LeakData = "", LeakSign = "";
                Regex regexDate = new Regex(@"\d{1,2}[/-]\d{1,2}[/-]\d{2,4}\s{1,}\d{1,2}:\d{1,2}:\d{1,2}");
                Regex regexPress = new Regex(@"\d*.\d*\s*bar");
                Regex regexLeak = new Regex(@"\d*.\d*\s*ml/min");
                string SuccessSign = MyParameter.LeakTestSettings.SuccessSign.Replace("(", @"\(").Replace(")", @"\)");
                //string ErrorSign = MyParameter.LeakTestSettings.ErrorSign.Replace("(", @"\(").Replace(")", @"\(");
                Regex regexSign = new Regex(SuccessSign);
                MyParameter.PlanData.PrintDate = DateTime.Now.ToString();
                try
                {
                    if (regexDate.Match(Code).Success) EquipmentDate = regexDate.Match(Code).Value;
                    if (regexSign.Match(Code).Success) LeakSign = regexSign.Match(Code).Value;
                    if (regexPress.Match(Code).Success) PressValue = regexPress.Match(Code).Value;
                    if (regexLeak.Match(Code).Success) LeakData = regexLeak.Match(Code).Value;

                    /*if (string.IsNullOrEmpty(EquipmentDate)) 
                    {
                        Invoke(new CallbackPlayAlert(PlayAlert), new object[] { AlertType.LeakError });
                        throw new Exception("试漏数据解析失败，缺少设备时间！");
                    }
                    else if (string.IsNullOrEmpty(LeakSign))
                    {
                        Invoke(new CallbackPlayAlert(PlayAlert), new object[] { AlertType.LeakError });
                        throw new Exception("试漏数据解析失败，缺少试漏标志！");
                    }
                    else if (string.IsNullOrEmpty(PressValue))
                    {
                        Invoke(new CallbackPlayAlert(PlayAlert), new object[] { AlertType.LeakError });
                        throw new Exception("试漏数据解析失败，缺少压力数值！");
                    }
                    else if (string.IsNullOrEmpty(LeakData))
                    {
                        Invoke(new CallbackPlayAlert(PlayAlert), new object[] { AlertType.LeakError });
                        throw new Exception("试漏数据解析失败，缺少泄漏量数值！");
                    }*/
                }
                catch (Exception ex)
                {
                    Invoke(new CallbackSetLog(SetLog), new object[] { "错误提示", "分析试漏数据失败，返回错误：匹配数据标志失败!", "Yes" });
                    throw new Exception(ex.ToString());
                }

                try
                {
                    string FieldList = "", ValueList = "", TempSign = "";
                    if (string.IsNullOrEmpty(LeakSign)) TempSign = "失败"; else TempSign = LeakSign;
                    Helper.Convert.CheckInsertSQL(ref FieldList, ref ValueList, "写入日期", DateTime.Now.ToString(), "DATE");
                    Helper.Convert.CheckInsertSQL(ref FieldList, ref ValueList, "零件号", MyParameter.PlanData.PartItem.Item, "STRING");
                    Helper.Convert.CheckInsertSQL(ref FieldList, ref ValueList, "计划日期", MyParameter.PlanData.PlanDate, "DATE");
                    Helper.Convert.CheckInsertSQL(ref FieldList, ref ValueList, "计划数量", MyParameter.PlanData.PlanData.ToString(), "LONG");
                    Helper.Convert.CheckInsertSQL(ref FieldList, ref ValueList, "已完成数量", MyParameter.PlanData.FinishedData.ToString(), "LONG");
                    Helper.Convert.CheckInsertSQL(ref FieldList, ref ValueList, "打印标签日期", MyParameter.PlanData.PrintDate, "DATE");
                    Helper.Convert.CheckInsertSQL(ref FieldList, ref ValueList, "测试日期", EquipmentDate, "DATE");
                    Helper.Convert.CheckInsertSQL(ref FieldList, ref ValueList, "测试压强", PressValue, "STRING");
                    Helper.Convert.CheckInsertSQL(ref FieldList, ref ValueList, "测试结果", TempSign, "STRING");
                    Helper.Convert.CheckInsertSQL(ref FieldList, ref ValueList, "泄漏量", LeakData, "STRING");
                    Helper.Convert.CheckInsertSQL(ref FieldList, ref ValueList, "接收数据", Code, "STRING");
                    string SQL = "insert into LeakTestLog(" + FieldList + ") values(" + ValueList + ")";
                    if (accessHelper.ExecuteCommand(SQL) <= 0) throw new Exception("写入数据库失败！");
                }
                catch (Exception ex)
                {
                    Invoke(new CallbackSetLog(SetLog), new object[] { "错误提示", "分析试漏数据失败，返回错误：将试漏数据写入数据库失败!", "Yes" });
                    throw new Exception(ex.ToString());
                }

                if (MyParameter.LeakTestSettings.SuccessSign.Equals(LeakSign))
                {
                    Invoke(new CallbackSetLog(SetLog), new object[] { "开始检测", "试漏成功，开始打印标签", "Yes" });
                    CurrentWorkType = WorkType.WaitPrintPartCode;
                    CurrentPrintLabelCount = 1;
                    int Result = NewPrintLabel(Code);
                    if (Result < 0)
                    {
                        Invoke(new CallbackSetLog(SetLog), new object[] { "错误提示", "分析试漏数据失败，返回错误：打印出现意外情况!", "Yes" });
                        throw new Exception("打印出现意外情况!");
                    }
                    else if (Result == 0)
                    {
                        ThreadProgram = false;
                        StartCheck();
                        return true;
                    }
                }
                else
                {
                    if (!RepeatTestModel) 
                    {
                        if (!WriteRepeatTestRecord(Code)) 
                        {
                            Invoke(new CallbackSetLog(SetLog), new object[] { "错误提示", "分析试漏数据失败，返回错误：复检记录更新失败!", "Yes" });
                            throw new Exception("复检记录更新失败！"); 
                        }
                    }
                    Invoke(new CallbackPlayAlert(PlayAlert), new object[] { AlertType.LeakError });
                    Invoke(new CallbackSetLog(SetLog), new object[] { "错误提示", "试漏失败", "Yes" });
                    ThreadProgram = false;
                    StartCheck();
                    return true;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void StartReceivePrintCheckThread()
        {
            try
            {
                ThreadProgram = true;
                int ByteCount = serialPort_ReceiveCheckCode.BytesToRead;
                if (ByteCount > 0)
                {
                    string ScanByte = "";
                    try
                    {
                        ScanByte = serialPort_ReceiveCheckCode.ReadExisting();
                        serialPort_ReceiveCheckCode.DiscardInBuffer();
                    }
                    catch (Exception)
                    {
                        Invoke(new CallbackSetLog(SetLog), new object[] { "错误提示", "处理标签校验数据失败，返回的错误：读取打印校验COM口缓冲区数据失败！", "Yes" });
                        throw new Exception("读取打印校验COM口缓冲区数据失败!");
                    }
                    Invoke(new CallbackSetLog(SetLog), new object[] { "开始检测", "接收到一条标签校验数据", "Yes" });
                    Invoke(new CallbackSetLog(SetLog), new object[] { "开始检测", "校验数据：" + ScanByte, "Yes" });

                    if (MyParameter.PlanData.PartItem.Item.Equals(ScanByte) || !MyParameter.PrintCheckSettings.EnabledStrictCheck)
                    {
                        int Result = NewPrintLabel("");
                        if (Result<0)
                        {
                            throw new Exception("error");
                        }else if (Result == 0)
                        {
                            ThreadProgram = false;
                            StartCheck();
                            return;
                        }
                    }
                    else
                    {
                        Invoke(new CallbackPlayAlert(PlayAlert), new object[] { AlertType.CheckLabelError });
                        Invoke(new CallbackSetLog(SetLog), new object[] { "错误提示", "处理标签校验数据失败，返回的错误：当前接收到的标签校验数据不正确!", "Yes" });
                        throw new Exception("当前接收到的标签校验数据不正确！");
                    }
                }
            }
            catch (Exception ex)
            {
                Invoke(new CallbackSetLog(SetLog), new object[] { "错误提示", "检测过程已经结束，请检查后再重新开始！", "Yes" });
                StartCheck();
            }
            finally
            {
                ThreadProgram = false;
            }
        }

        /// <summary>
        /// 大于1代表True，等于0代表计划完成，小于0代表错误
        /// </summary>
        /// <returns></returns>
        /*private int PrintLabel(string Code)
        {
            try
            {
                if (!MyParameter.PrintSerial.ResetSign.ToShortDateString().Equals(DateTime.Now.ToShortDateString()))
                {
                    MyParameter.PrintSerial.ResetSign = DateTime.Now.Date;
                    MyParameter.PrintSerial.Serial = 0;
                }
                MyParameter.PrintSerial.Serial++;
                if (!MyConfig.SavePrintSerial(MyParameter))
                {
                    Invoke(new CallbackSetLog(SetLog), new object[] { "错误提示", "更新打印序列号失败，请检查配置文件！", "Yes" });
                    throw new Exception("更新打印序列号失败！");
                }

                DateTime PrintTime = DateTime.Parse(MyParameter.PlanData.PrintDate);
                StringBuilder PrintDoc = new StringBuilder();
                PrintDoc.AppendLine("CT~~CD,~CC^~CT~");
                PrintDoc.AppendLine("^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR4,4~SD25^JUS^LRN^CI0^XZ");
                PrintDoc.AppendLine("^XA");
                PrintDoc.AppendLine("*InsertLine*");
                PrintDoc.AppendLine("^PQ*PrintCount*,0,1,Y^XZ");

                Bitmap CodePic = Generate128Code(MyParameter.PlanData.PartItem.Item,MyParameter.PrintSettings.CodeHeight, MyParameter.PrintSettings.CodeWidth);
                if (CodePic == null) { throw new Exception("生成条码图像失败！"); }

                Bitmap bmp = new Bitmap((int)MillimetersToPixelsWidth(MyParameter.PrintSettings.LabelWidth), (int)MillimetersToPixelsWidth(MyParameter.PrintSettings.LabelHeight));
                Graphics theGraphics = Graphics.FromImage(bmp);
                theGraphics.Clear(Color.White);
                Font myFont = new System.Drawing.Font("宋体", 20, FontStyle.Bold);
                Brush bush = new SolidBrush(System.Drawing.Color.Black);
                theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                Matrix matrix = new Matrix();
                matrix.Rotate(180);

                theGraphics.DrawString(PrintTime.ToString("yyMMdd"), myFont, bush, MyParameter.PrintSettings.DateUpPoint.X, MyParameter.PrintSettings.DateUpPoint.Y);
                theGraphics.DrawString(PrintTime.ToString("HH:mm:ss"), myFont, bush, MyParameter.PrintSettings.DateUpPoint.X - 45, MyParameter.PrintSettings.DateUpPoint.Y + 25);
                theGraphics.DrawString(MyParameter.PrintSerial.Serial.ToString().PadLeft(3, '0'), myFont, bush, MyParameter.PrintSettings.DateUpPoint.X + 90, MyParameter.PrintSettings.DateUpPoint.Y + 25);
                theGraphics.DrawString(MyParameter.PlanData.PartItem.Prefix + " " + MyParameter.PlanData.PartItem.Item + " " + MyParameter.PlanData.PartItem.Suffix, myFont, bush, MyParameter.PrintSettings.PartUpPoint.X, MyParameter.PrintSettings.PartUpPoint.Y);
                theGraphics.DrawImage(CodePic, MyParameter.PrintSettings.CodePoint.X, MyParameter.PrintSettings.CodePoint.Y);
                theGraphics.Transform = matrix;
                theGraphics.DrawString(MyParameter.PlanData.PartItem.Prefix + " " + MyParameter.PlanData.PartItem.Item + " " + MyParameter.PlanData.PartItem.Suffix, myFont, bush, MyParameter.PrintSettings.PartDownPoint.X, MyParameter.PrintSettings.PartDownPoint.Y);
                theGraphics.DrawString(MyParameter.PrintSerial.Serial.ToString().PadLeft(3, '0'), myFont, bush, MyParameter.PrintSettings.DateDownPoint.X + 90, MyParameter.PrintSettings.DateDownPoint.Y + 25);
                theGraphics.DrawString(PrintTime.ToString("HH:mm:ss"), myFont, bush, MyParameter.PrintSettings.DateDownPoint.X - 45, MyParameter.PrintSettings.DateDownPoint.Y + 25);
                theGraphics.DrawString(PrintTime.ToString("yyMMdd"), myFont, bush, MyParameter.PrintSettings.DateDownPoint.X, MyParameter.PrintSettings.DateDownPoint.Y);
                theGraphics.Save();
                theGraphics.Dispose();

                int TotalBytes = 0, RowBytes = 0;
                string Temp = ZebraUnity.BitmapToHex(bmp, out TotalBytes, out RowBytes);
                Temp = string.Format("~DGR:Temp0.GRF,{0},{1},{2}", TotalBytes, RowBytes, Temp);
                Temp += "^FO0,0^XGR:Temp0.GRF,1,1^FS";
                Temp = PrintDoc.ToString().Replace("*InsertLine*", Temp);
                Temp = Temp.Replace("*PrintCount*", "1");
                if (!MyZebra.PrintWithDrv(Temp, MyParameter.PrintSettings.PrintName))
                {
                    MyParameter.PlanData.ErrorData++;
                    if (!RepeatTestModel) 
                    {
                        if (!UpdatePlanData()) 
                        {
                            Invoke(new CallbackSetLog(SetLog), new object[] { "错误提示", "处理试漏设备输出的信息失败，返回的错误：更新错误打印记录统计数据失败！", "Yes" });
                            throw new Exception("更新打印计划数据失败！"); 
                        }
                    }
                    Invoke(new CallbackSetLog(SetLog), new object[] { "错误提示", "处理试漏设备输出的信息失败，返回的错误：打印出现意外情况，请检查打印机！", "Yes" });
                    throw new Exception("error");
                }

                MyParameter.PlanData.FinishedData++;
                Invoke(new CallbackSetFinishedPrintData(SetFinishedPrintData));
                if (!RepeatTestModel)
                {
                    if (!UpdatePlanData())
                    {
                        Invoke(new CallbackSetLog(SetLog), new object[] { "错误提示", "处理试漏设备输出的信息失败，返回的错误：更新成功打印记录统计数据失败！", "Yes" });
                        throw new Exception("更新打印计划数据失败！");
                    }
                }
                else
                {
                    if (!UpdateRepeatTestData(Code)) 
                    {
                        Invoke(new CallbackSetLog(SetLog), new object[] { "错误提示", "处理试漏设备输出的信息失败，返回的错误：更新复检记录信息失败！", "Yes" });
                        throw new Exception("更新复检记录信息失败！");
                    }
                }
                Invoke(new CallbackSetLog(SetLog), new object[] { "开始检测", "标签："+MyParameter.PlanData.PartItem.Item+"，已经打印，当前打印标签序号："+CurrentPrintLabelCount.ToString(), "Yes" });

                if (MyParameter.PlanData.FinishedData >= MyParameter.PlanData.PlanData)
                {
                    Invoke(new CallbackSetLog(SetLog), new object[] { "开始检测", "当前打印标签数量已经达到计划限额，程序自动停止检测功能！", "Yes" });
                    Invoke(new CallbackPlayAlert(PlayAlert), new object[] { RepeatTestModel?AlertType.RepeatTestFinished:AlertType.MeetPlanLimit });
                    return 0;
                }

                CurrentPrintLabelCount++;
                if (CurrentPrintLabelCount <= MyParameter.PlanData.PartItem.PrintCount)
                {
                    if (MyParameter.PrintCheckSettings.EnabledPrintCheck)
                    {
                        CurrentWorkType = WorkType.WaitReceivePrintCheckCode;
                        serialPort_ReceiveCheckCode.DiscardInBuffer();
                        Invoke(new CallbackSetLog(SetLog), new object[] { "开始检测", "等待接收标签打印校验数据！", "Yes" });
                    }
                    else
                    {
                        return PrintLabel(Code);
                    }
                }
                else
                {
                    CurrentWorkType = WorkType.WaitReceiveLeakTest;
                    serialPort_ReceiveLeakTest.DiscardInBuffer();
                    Invoke(new CallbackSetLog(SetLog), new object[] { "开始检测", "等待接收试漏输出数据！", "Yes" });
                }
                return 1;
            }
            catch (Exception ex)
            {
                Invoke(new CallbackPlayAlert(PlayAlert), new object[] { AlertType.PrintError });
                return -1;
            }
        }*/

        private int NewPrintLabel(string Code)
        {
            try
            {
                if (!MyParameter.PrintSerial.ResetSign.ToShortDateString().Equals(DateTime.Now.ToShortDateString()))
                {
                    MyParameter.PrintSerial.ResetSign = DateTime.Now.Date;
                    MyParameter.PrintSerial.Serial = 0;
                }
                MyParameter.PrintSerial.Serial++;
                if (!MyConfig.SavePrintSerial(MyParameter))
                {
                    Invoke(new CallbackSetLog(SetLog), new object[] { "错误提示", "更新打印序列号失败，请检查配置文件！", "Yes" });
                    throw new Exception("更新打印序列号失败！");
                }

                DateTime PrintTime = DateTime.Parse(MyParameter.PlanData.PrintDate);
                StringBuilder PrintDoc = new StringBuilder();
                PrintDoc.AppendLine("CT~~CD,~CC^~CT~");
                PrintDoc.AppendLine("^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR4,4~SD25^JUS^LRN^CI0^XZ");
                PrintDoc.AppendLine("^XA");
                PrintDoc.AppendLine("*InsertLine*");
                PrintDoc.AppendLine("^PQ*PrintCount*,0,1,Y^XZ");

                CurrentLabelTemplate.LabelFields.ForEach(T =>
                {
                    if (T.ObjectType == DrawObjectType.Text && T.ObjectValueType == DrawObjectValueType.Variable)
                    {
                        switch (T.Sign.ToUpper())
                        {
                            case "DATE":
                                T.PrintValue = PrintTime.ToString("yyMMdd");
                                break;
                            case "TIME":
                                T.PrintValue = PrintTime.ToString("HH:mm:ss");
                                break;
                            case "SERIAL":
                                if (MyParameter.PrintSerial.Serial.ToString().Length > 3) throw new Exception("序列号错误，超出位数！");
                                Invoke(new CallbackSetLog(SetLog), new object[] { "错误提示", "序列号错误，超出位数！", "Yes" });
                                T.PrintValue = MyParameter.PrintSerial.Serial.ToString().PadLeft(3, '0');
                                break;
                            default:
                                MessageBox.Show("动态标识无法识别！");
                                throw new Exception("error");
                        }
                    }
                });

                float Scalex = MyParameter.PrintSettings.DpiX / 96;
                int LabelWidth = (int)(Helper.Convert.MillimetersToPixelsWidth(96, CurrentLabelTemplate.LabelWidth * 10) * Scalex);
                int LabelHeight = (int)(Helper.Convert.MillimetersToPixelsWidth(96, CurrentLabelTemplate.LabelLength * 10) * Scalex);
                Bitmap b = CurrentLabelTemplate.InitPrintBitmap(LabelWidth, LabelHeight*2, Scalex);

                int TotalBytes = 0, RowBytes = 0;
                string Temp = ZebraUnity.BitmapToHex(b, out TotalBytes, out RowBytes);
                Temp = string.Format("~DGR:Temp0.GRF,{0},{1},{2}", TotalBytes, RowBytes, Temp);
                Temp += "^FO0,0^XGR:Temp0.GRF,1,1^FS";
                Temp = PrintDoc.ToString().Replace("*InsertLine*", Temp);
                Temp = Temp.Replace("*PrintCount*", "1"); 
                if (!MyZebra.PrintWithDrv(Temp, MyParameter.PrintSettings.PrintName))
                {
                    MyParameter.PlanData.ErrorData++;
                    if (!RepeatTestModel)
                    {
                        if (!UpdatePlanData())
                        {
                            Invoke(new CallbackSetLog(SetLog), new object[] { "错误提示", "处理试漏设备输出的信息失败，返回的错误：更新错误打印记录统计数据失败！", "Yes" });
                            throw new Exception("更新打印计划数据失败！");
                        }
                    }
                    Invoke(new CallbackSetLog(SetLog), new object[] { "错误提示", "处理试漏设备输出的信息失败，返回的错误：打印出现意外情况，请检查打印机！", "Yes" });
                    throw new Exception("error");
                }

                MyParameter.PlanData.FinishedData++;
                Invoke(new CallbackSetFinishedPrintData(SetFinishedPrintData));
                if (!RepeatTestModel)
                {
                    if (!UpdatePlanData())
                    {
                        Invoke(new CallbackSetLog(SetLog), new object[] { "错误提示", "处理试漏设备输出的信息失败，返回的错误：更新成功打印记录统计数据失败！", "Yes" });
                        throw new Exception("更新打印计划数据失败！");
                    }
                }
                else
                {
                    if (!UpdateRepeatTestData(Code))
                    {
                        Invoke(new CallbackSetLog(SetLog), new object[] { "错误提示", "处理试漏设备输出的信息失败，返回的错误：更新复检记录信息失败！", "Yes" });
                        throw new Exception("更新复检记录信息失败！");
                    }
                }
                Invoke(new CallbackSetLog(SetLog), new object[] { "开始检测", "标签：" + MyParameter.PlanData.PartItem.Item + "，已经打印，当前打印标签序号：" + CurrentPrintLabelCount.ToString(), "Yes" });

                if (MyParameter.PlanData.FinishedData >= MyParameter.PlanData.PlanData)
                {
                    Invoke(new CallbackSetLog(SetLog), new object[] { "开始检测", "当前打印标签数量已经达到计划限额，程序自动停止检测功能！", "Yes" });
                    Invoke(new CallbackPlayAlert(PlayAlert), new object[] { RepeatTestModel ? AlertType.RepeatTestFinished : AlertType.MeetPlanLimit });
                    return 0;
                }

                CurrentPrintLabelCount++;
                if (CurrentPrintLabelCount <= MyParameter.PlanData.PartItem.PrintCount)
                {
                    if (MyParameter.PrintCheckSettings.EnabledPrintCheck)
                    {
                        CurrentWorkType = WorkType.WaitReceivePrintCheckCode;
                        serialPort_ReceiveCheckCode.DiscardInBuffer();
                        Invoke(new CallbackSetLog(SetLog), new object[] { "开始检测", "等待接收标签打印校验数据！", "Yes" });
                    }
                    else
                    {
                        return NewPrintLabel(Code);
                    }
                }
                else
                {
                    CurrentWorkType = WorkType.WaitReceiveLeakTest;
                    serialPort_ReceiveLeakTest.DiscardInBuffer();
                    Invoke(new CallbackSetLog(SetLog), new object[] { "开始检测", "等待接收试漏输出数据！", "Yes" });
                }
                return 1;
            }
            catch (Exception ex)
            {
                Invoke(new CallbackPlayAlert(PlayAlert), new object[] { AlertType.PrintError });
                return -1;
            }
        }

        private Bitmap Generate128Code(string code, int height, int width)
        {
            try
            {
                BarcodeWriter writer = new BarcodeWriter();
                writer.Format = BarcodeFormat.CODE_128;
                EncodingOptions options = new EncodingOptions()
                {
                    PureBarcode = true,
                    Width = width,
                    Height = height,
                    Margin = 0
                };
                writer.Options = options;
                Bitmap map = writer.Write(code);
                return map;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static double MillimetersToPixelsWidth(double length)
        {
            System.Windows.Forms.Panel p = new System.Windows.Forms.Panel();
            System.Drawing.Graphics g = System.Drawing.Graphics.FromHwnd(p.Handle);
            IntPtr hdc = g.GetHdc();
            int width = GetDeviceCaps(hdc, 4); //HORZRES
            int pixels = GetDeviceCaps(hdc, 8);// BITSPIXEL
            g.ReleaseHdc(hdc);
            return (((double)pixels / (double)width) * (double)length);
        }
        [DllImport("gdi32.dll")] private static extern int GetDeviceCaps(IntPtr hdc, int Index);

        /// <summary>
        /// 选择试漏计划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton_ChoosePartNumber_Click(object sender, EventArgs e)
        {
            try
            {
                if (!toolStripButton_Start.Text.Equals("开始检测"))
                {
                    MessageBox.Show("请先暂时停止检测功能，然后选择零件计划！");
                    return;
                }
                MyParameter.PlanData = null;
                RepeatTestModel = false;
                toolStripButton_Start.Enabled = false;
                toolStripTextBox_PartNumber.Text = "";
                toolStripTextBox_PlanData.Text = "";
                toolStripTextBox_PlanDate.Text = "";
                toolStripTextBox_FinishedData.Text = "";
                ChoosePartNumber choose = new ChoosePartNumber();
                choose.ShowDialog();
                if (choose.DialogResult == DialogResult.OK)
                {
                    MyParameter.PlanData = choose.PlanData;
                    CurrentLabelTemplate = MyLabelTemplate.GetLabelTemplate(MyParameter.PlanData.PartItem.Item);
                    if (CurrentLabelTemplate == null) throw new Exception("当前零件打印模版不存在！");

                    toolStripTextBox_PartNumber.Text = MyParameter.PlanData.PartItem.Item;
                    toolStripTextBox_PlanData.Text = MyParameter.PlanData.PlanData.ToString();
                    toolStripTextBox_PlanDate.Text = MyParameter.PlanData.PlanDate;
                    toolStripTextBox_FinishedData.Text = MyParameter.PlanData.FinishedData.ToString();
                    toolStripButton_Start.Enabled = true;
                    toolStripButton_PrintManually.Enabled = true;
                }
                choose.Dispose();
                choose = null;
            }
            catch (Exception ex)
            {
                MyParameter.PlanData = null;
                RepeatTestModel = false;
                toolStripButton_Start.Enabled = false;
                MessageBox.Show("选择零件出现意外情况，返回错误："+ex.Message);
            }
        }

        private void toolStripButton_ChooseRepeatTestRecord_Click(object sender, EventArgs e)
        {
            try
            {
                if (!toolStripButton_Start.Text.Equals("开始检测"))
                {
                    MessageBox.Show("请先暂时停止检测功能，然后选择需要复检的记录！");
                    return;
                }
                MyParameter.PlanData = null;
                RepeatTestModel = true;
                toolStripButton_Start.Enabled = false;
                toolStripTextBox_PartNumber.Text = "";
                toolStripTextBox_PlanData.Text = "";
                toolStripTextBox_PlanDate.Text = "";
                toolStripTextBox_FinishedData.Text = "";
                ChooseRepeatTestRecord choose = new ChooseRepeatTestRecord();
                choose.ShowDialog();
                if (choose.DialogResult == DialogResult.OK)
                {
                    MyParameter.PlanData = choose.PlanData;
                    CurrentLabelTemplate = MyLabelTemplate.GetLabelTemplate(MyParameter.PlanData.PartItem.Item);
                    if (CurrentLabelTemplate == null) throw new Exception("当前零件打印模版不存在！");

                    toolStripTextBox_PartNumber.Text = MyParameter.PlanData.PartItem.Item;
                    toolStripTextBox_PlanData.Text = MyParameter.PlanData.PlanData.ToString();
                    toolStripTextBox_PlanDate.Text = MyParameter.PlanData.PlanDate;
                    toolStripTextBox_FinishedData.Text = MyParameter.PlanData.FinishedData.ToString();
                    toolStripButton_Start.Enabled = true;
                    toolStripButton_PrintManually.Enabled = true;
                }
                choose.Dispose();
                choose = null;
            }
            catch (Exception ex)
            {
                MyParameter.PlanData = null;
                RepeatTestModel = true;
                toolStripButton_Start.Enabled = false;
                MessageBox.Show("选择零件出现意外情况，返回错误：" + ex.Message);
            }
        }

        private void toolStripButton_PrintManually_Click_1(object sender, EventArgs e)
        {
            try
            {
                if(string.IsNullOrEmpty(toolStripTextBox_PartNumber.Text)||MyParameter.PlanData==null)
                {
                    MessageBox.Show("当前未正确选择计划打印的零件号！");
                    return;
                }
                InputPassword password = new InputPassword();
                password.ShowDialog();
                if (password.DialogResult == DialogResult.OK)
                {
                    PrintManually print = new PrintManually();
                    print.PlanData = MyParameter.PlanData;
                    print.CurrentLabelTemplate = CurrentLabelTemplate;
                    print.ShowDialog();
                    if (print.DialogResult == DialogResult.OK)
                    {
                        Invoke(new CallbackSetLog(SetLog), new object[] { "操作提示", "当前零件号:"+MyParameter.PlanData.PartItem.Item+", 标签补打印已经完成！", "Yes" });
                    }
                    else
                    {
                        Invoke(new CallbackSetLog(SetLog), new object[] { "错误提示", "当前零件号:" + MyParameter.PlanData.PartItem.Item + ", 标签补打印失败！", "Yes" });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private bool UpdatePlanData()
        {
            try
            {
                string SQL = "update Plan set 已完成数量=" + MyParameter.PlanData.FinishedData.ToString() + ", 故障数量=" + MyParameter.PlanData.ErrorData.ToString() + " Where ID=" + MyParameter.PlanData.ID.ToString();
                if (accessHelper.ExecuteCommand(SQL) <= 0) throw new Exception("更新打印计划完成信息失败！");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool UpdateRepeatTestData(string Code)
        {
            try
            {
                if (MyParameter.PlanData == null || !RepeatTestModel) return false;
                string SQL = "update RepeatTest set 重新测试标识='完成', 重新测试日期='" + DateTime.Now.ToString() + "',重新测试结果='成功',重新测试数据='" + Code + "' Where 序号=" + MyParameter.PlanData.ID.ToString();
                if (accessHelper.ExecuteCommand(SQL) <= 0) throw new Exception("更新复检记录信息失败！");

                if (RepeatTestDT != null && RepeatTestDT.Rows.Count > 0)
                {
                    DataRow[] drFind = RepeatTestDT.Select("序号=" + MyParameter.PlanData.ID.ToString());
                    if (drFind.Length > 0)
                    {
                        drFind[0]["重新测试标识"] = "完成";
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private delegate void CallbackSetFinishedPrintData();
        private void SetFinishedPrintData()
        {
            try
            {
                toolStripTextBox_FinishedData.Text = MyParameter.PlanData.FinishedData.ToString();
            }
            catch (Exception)
            {
            }
        }

        private delegate void CallbackPlayAlert(AlertType type);
        private void PlayAlert(AlertType type)
        {
            try
            {
                if (!MyParameter.EnabledAlertSound) return;
                string url = "";
                switch (type)
                {
                    case AlertType.Init:
                        url = CurrentDirectory + @"\Alert-InitiativeFinished.mp3";
                        break;
                    case AlertType.MeetPlanLimit:
                        url = CurrentDirectory + @"\Alert-meetPlanLimit.mp3";
                        break;
                    case AlertType.PrintError:
                        url = CurrentDirectory + @"\Alert-PrintError.mp3";
                        break;
                    case AlertType.LeakError:
                        url = CurrentDirectory + @"\Alert-LeakError.mp3";
                        break;
                    case AlertType.CheckLabelError:
                        url = CurrentDirectory + @"\Alert-CheckLabelError.mp3";
                        break;
                    case AlertType.RepeatTestFinished:
                        url = CurrentDirectory + @"\Alert-RepeatTestFinished.mp3";
                        break;
                }
                if (!string.IsNullOrEmpty(url))
                {
                    //axWindowsMediaPlayer1.URL = url;
                    //axWindowsMediaPlayer1.Ctlcontrols.play();
                }
            }
            catch (Exception)
            {
                Invoke(new CallbackSetLog(SetLog), new object[] { "错误提示", "播放声音提示出错！", "No" });
            }
        }

        private bool WriteRepeatTestRecord(String Code)
        {
            try
            {
                string Time = DateTime.Now.ToString();
                for(int Index = 1; Index <= MyParameter.PlanData.PartItem.PrintCount; Index++)
                {
                    string FieldList = "", ValueList = "";
                    Helper.Convert.CheckInsertSQL(ref FieldList, ref ValueList, "日期", Time, "DATE");
                    Helper.Convert.CheckInsertSQL(ref FieldList, ref ValueList, "零件号", MyParameter.PlanData.PartItem.Item, "STRING");
                    Helper.Convert.CheckInsertSQL(ref FieldList, ref ValueList, "编号", Index.ToString(), "LONG");
                    Helper.Convert.CheckInsertSQL(ref FieldList, ref ValueList, "计划日期", MyParameter.PlanData.PlanDate, "DATE");
                    Helper.Convert.CheckInsertSQL(ref FieldList, ref ValueList, "测试数据", Code, "STRING");
                    Helper.Convert.CheckInsertSQL(ref FieldList, ref ValueList, "重新测试标识", "等待复检", "STRING");
                    string SQL = "insert into RepeatTest(" + FieldList + ") values(" + ValueList + ")";
                    if (accessHelper.ExecuteCommand(SQL) <= 0) return false;
                    Invoke(new CallbackSetRepeatTestView(SetRepeatTestView), new object[] { MyParameter.PlanData.PartItem.Item,Index.ToString(),Time });
                }
                return true;
            }
            catch (Exception ex)
            {
                Invoke(new CallbackSetLog(SetLog), new object[] { "错误提示", "写入复检等待队列失败，返回错误：" + ex.ToString(), "Yes" });
                return false;
            }
        }

        private delegate void CallbackSetRepeatTestView(string Part, string Serial, string Time);
        private void SetRepeatTestView(string Part, string Serial, string Time)
        {
            try
            {
                DataRow dr = RepeatTestDT.NewRow();
                dr["序号"] = RepeatTestDT.Rows.Count + 1;
                dr["零件号"] = Part;
                dr["编号"] = Serial;
                dr["日期"] = Time;
                dr["重新测试标识"] = "等待复检";

                RepeatTestDT.Rows.Add(dr);
            }
            catch (Exception)
            {
            }
        }
    }
}
