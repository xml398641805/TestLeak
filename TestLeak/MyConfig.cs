using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    class MyConfig
    {
        private static string _ConfigFile = Directory.GetCurrentDirectory().ToString() + @"\Config.xml";
        /// <summary>
        /// 读取配置文件名
        /// </summary>
        /// <returns></returns>
        public string ConfigFile() { return _ConfigFile; }

        public static Parameter GetParameter()
        {
            if (!File.Exists(_ConfigFile)) { return null; }
            try
            {
                MyXML myConfig = new MyXML(_ConfigFile);
                if (!myConfig.OpenConfigFile()) { return null; }

                Parameter parameter = new Parameter();
                parameter.LeakTestSettings.ComConfig.ComPort = myConfig.ReadAttributeValue("appsettings", "LeakTestSettings", "Com");
                parameter.LeakTestSettings.ComConfig.Speed = int.Parse(myConfig.ReadAttributeValue("appsettings", "LeakTestSettings", "Speed"));
                parameter.LeakTestSettings.ComConfig.DataBit = int.Parse(myConfig.ReadAttributeValue("appsettings", "LeakTestSettings", "Data"));
                parameter.LeakTestSettings.ComConfig.StopBit = int.Parse(myConfig.ReadAttributeValue("appsettings", "LeakTestSettings", "Stop"));
                parameter.LeakTestSettings.ComConfig.CheckType = myConfig.ReadAttributeValue("appsettings", "LeakTestSettings", "CheckType");
                parameter.LeakTestSettings.SuccessSign = myConfig.ReadAttributeValue("appsettings", "LeakTestSettings", "SuccessSign");
                parameter.LeakTestSettings.ErrorSign = myConfig.ReadAttributeValue("appsettings", "LeakTestSettings", "ErrorSign");
                parameter.LeakTestSettings.GapAlertValue = int.Parse(myConfig.ReadAttributeValue("appsettings", "LeakTestSettings", "GapAlertValue"));
                parameter.LeakTestSettings.EnabledGapAlert = bool.Parse(myConfig.ReadAttributeValue("appsettings", "LeakTestSettings", "EnabledGapAlert"));


                parameter.PrintCheckSettings.ComConfig.ComPort = myConfig.ReadAttributeValue("appsettings", "PrintCheckSettings", "Com");
                parameter.PrintCheckSettings.ComConfig.Speed = int.Parse(myConfig.ReadAttributeValue("appsettings", "PrintCheckSettings", "Speed"));
                parameter.PrintCheckSettings.ComConfig.DataBit = int.Parse(myConfig.ReadAttributeValue("appsettings", "PrintCheckSettings", "Data"));
                parameter.PrintCheckSettings.ComConfig.StopBit = int.Parse(myConfig.ReadAttributeValue("appsettings", "PrintCheckSettings", "Stop"));
                parameter.PrintCheckSettings.ComConfig.CheckType = myConfig.ReadAttributeValue("appsettings", "PrintCheckSettings", "CheckType");
                parameter.PrintCheckSettings.EnabledPrintCheck = bool.Parse(myConfig.ReadAttributeValue("appsettings", "PrintCheckSettings", "EnabledCheck"));
                parameter.PrintCheckSettings.EnabledStrictCheck = bool.Parse(myConfig.ReadAttributeValue("appsettings", "PrintCheckSettings", "EnabledStrictCheck"));

                parameter.PrintSettings.PrintName = myConfig.ReadAttributeValue("appsettings", "PrintSettings", "PrintName");
                parameter.PrintSettings.DpiX = int.Parse(myConfig.ReadAttributeValue("appsettings", "PrintSettings", "DpiX"));


                List<ElmentList> elments = myConfig.ReadAttributeValue("PartList");
                foreach(ElmentList el in elments)
                {
                    if (el.ElmentName.Equals("Item"))
                    {
                        Parameter.PartModel Item = new Parameter.PartModel();
                        Item.Item = el._AttributeList[0].AttributeValue;
                        Item.PrintCount = int.Parse(el._AttributeList[1].AttributeValue);
                        parameter.PartList.Add(Item);
                    }
                }

                parameter.Password = myConfig.ReadAttributeValue("appsettings", "Password", "Value");
                parameter.EnabledAlertSound = bool.Parse(myConfig.ReadAttributeValue("appsettings", "EnabledAlertSound", "Value"));

                parameter.PrintSerial.Serial = int.Parse(myConfig.ReadAttributeValue("appsettings", "PrintSerial", "Value"));
                parameter.PrintSerial.ResetSign = DateTime.Parse(myConfig.ReadAttributeValue("appsettings", "PrintSerial", "ResetSign"));

                return parameter;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool SaveParameter(Parameter parameter)
        {
            if (!File.Exists(_ConfigFile)) { return false; }

            try
            {
                MyXML myConfig = new MyXML(_ConfigFile);
                if (!myConfig.OpenConfigFile()) { return false; }

                if (!myConfig.SetAttributeValue("appsettings", "LeakTestSettings", "Com", parameter.LeakTestSettings.ComConfig.ComPort)) throw new Exception("error");
                if (!myConfig.SetAttributeValue("appsettings", "LeakTestSettings", "Speed", parameter.LeakTestSettings.ComConfig.Speed.ToString())) throw new Exception("error");
                if (!myConfig.SetAttributeValue("appsettings", "LeakTestSettings", "Data", parameter.LeakTestSettings.ComConfig.DataBit.ToString())) throw new Exception("error");
                if (!myConfig.SetAttributeValue("appsettings", "LeakTestSettings", "Stop", parameter.LeakTestSettings.ComConfig.StopBit.ToString())) throw new Exception("error");
                if (!myConfig.SetAttributeValue("appsettings", "LeakTestSettings", "CheckType", parameter.LeakTestSettings.ComConfig.CheckType)) throw new Exception("error");
                if (!myConfig.SetAttributeValue("appsettings", "LeakTestSettings", "SuccessSign", parameter.LeakTestSettings.SuccessSign)) throw new Exception("error");
                if (!myConfig.SetAttributeValue("appsettings", "LeakTestSettings", "ErrorSign", parameter.LeakTestSettings.ErrorSign)) throw new Exception("error");
                if (!myConfig.SetAttributeValue("appsettings", "LeakTestSettings", "GapAlertValue", parameter.LeakTestSettings.GapAlertValue.ToString())) throw new Exception("error");
                if (!myConfig.SetAttributeValue("appsettings", "LeakTestSettings", "EnabledGapAlert", parameter.LeakTestSettings.EnabledGapAlert.ToString())) throw new Exception("error");

                if (!myConfig.SetAttributeValue("appsettings", "PrintCheckSettings", "Com", parameter.PrintCheckSettings.ComConfig.ComPort)) throw new Exception("error");
                if (!myConfig.SetAttributeValue("appsettings", "PrintCheckSettings", "Speed", parameter.PrintCheckSettings.ComConfig.Speed.ToString())) throw new Exception("error");
                if (!myConfig.SetAttributeValue("appsettings", "PrintCheckSettings", "Data", parameter.PrintCheckSettings.ComConfig.DataBit.ToString())) throw new Exception("error");
                if (!myConfig.SetAttributeValue("appsettings", "PrintCheckSettings", "Stop", parameter.PrintCheckSettings.ComConfig.StopBit.ToString())) throw new Exception("error");
                if (!myConfig.SetAttributeValue("appsettings", "PrintCheckSettings", "CheckType", parameter.PrintCheckSettings.ComConfig.CheckType)) throw new Exception("error");
                if (!myConfig.SetAttributeValue("appsettings", "PrintCheckSettings", "EnabledCheck", parameter.PrintCheckSettings.EnabledPrintCheck.ToString())) throw new Exception("error");
                if (!myConfig.SetAttributeValue("appsettings", "PrintCheckSettings", "EnabledStrictCheck", parameter.PrintCheckSettings.EnabledStrictCheck.ToString())) throw new Exception("error");

                if (!myConfig.SetAttributeValue("appsettings", "PrintSettings", "PrintName", parameter.PrintSettings.PrintName)) throw new Exception("error");
                if (!myConfig.SetAttributeValue("appsettings", "PrintSettings", "DpiX", parameter.PrintSettings.DpiX.ToString())) throw new Exception("error");

                if (!myConfig.DeleteAllElmentFromChildNode("PartList")) throw new Exception("error");
                foreach(Parameter.PartModel s in parameter.PartList)
                {
                    List<AttributeList> arrList = new List<AttributeList>();
                    AttributeList Attr1 = new AttributeList("Value", s.Item);
                    AttributeList Attr2 = new AttributeList("PrintCount", s.PrintCount.ToString());
                    arrList.Add(Attr1);
                    arrList.Add(Attr2);
                    myConfig.InsertChildNode("PartList", "Item", arrList);
                }

                if (!myConfig.SetAttributeValue("appsettings", "Password", "Value", parameter.Password)) throw new Exception("error");
                if (!myConfig.SetAttributeValue("appsettings", "EnabledAlertSound", "Value", parameter.EnabledAlertSound.ToString())) throw new Exception("error");

                if (!myConfig.SaveConfigFile()) throw new Exception("error");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool SavePrintSerial(Parameter parameter)
        {
            try
            {
                if (!File.Exists(_ConfigFile)) { return false; }
                MyXML myConfig = new MyXML(_ConfigFile);
                if (!myConfig.OpenConfigFile()) { return false; }

                if (!myConfig.SetAttributeValue("appsettings", "PrintSerial", "Value", parameter.PrintSerial.Serial.ToString())) throw new Exception("error");
                if (!myConfig.SetAttributeValue("appsettings", "PrintSerial", "ResetSign", parameter.PrintSerial.ResetSign.ToShortDateString())) throw new Exception("error");

                if (!myConfig.SaveConfigFile()) throw new Exception("error");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public class Parameter
    {
        public class ComPortConfig
        {
            public string ComPort { get; set; }
            public int DataBit { get; set; }
            public int StopBit { get; set; }
            public int Speed { get; set; }
            public string CheckType { get; set; }
        }

        public class LeakTestConfig
        {
            public ComPortConfig ComConfig { get; set; }
            public string SuccessSign { get; set; }
            public string ErrorSign { get; set; }
            public int GapAlertValue { get; set; }
            public bool EnabledGapAlert { get; set; }
            public DateTime LastLeakTime { get; set; }

            public LeakTestConfig() { ComConfig = new ComPortConfig(); }
        }

        public class PrintCheckConfig
        {
            public ComPortConfig ComConfig { get; set; }
            public bool EnabledPrintCheck { get; set; }
            public bool EnabledStrictCheck { get; set; }

            public PrintCheckConfig() { ComConfig = new ComPortConfig(); }
        }

        public class PrintConfig
        {
            public string PrintName { get; set; }
            public int DpiX { get; set; }
        }

        public class PartModel
        {
            public string Item { get; set; }
            public int PrintCount { get; set; }
        }
        public List<PartModel> PartList { get; set; }

        public class PlanDataModel
        {
            public long ID { get; set; }
            public PartModel PartItem { get; set; }
            public string PlanDate { get; set; }
            public int PlanData { get; set; }
            public int FinishedData { get; set; }
            public int ErrorData { get; set; }
            public string PrintDate { get; set; }

            public PlanDataModel() { PartItem = new PartModel(); }
        }

        public LeakTestConfig LeakTestSettings { get; set; }
        public PrintCheckConfig PrintCheckSettings { get; set; }
        public PrintConfig PrintSettings { get; set; }

        public PlanDataModel PlanData { get; set; }

        public string Password { get; set; }
        public bool EnabledAlertSound { get; set; }

        public class SerialModel
        {
            public int Serial { get; set; }
            public DateTime ResetSign { get; set; }
        }
        public SerialModel PrintSerial { get; set; }

        public Parameter()
        {
            LeakTestSettings = new LeakTestConfig();
            PrintCheckSettings = new PrintCheckConfig();
            PrintSettings = new PrintConfig();
            PartList = new List<PartModel>();
            PrintSerial = new SerialModel();
        }
    }
}
