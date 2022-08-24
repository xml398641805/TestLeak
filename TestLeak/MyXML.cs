using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Helper
{
    class MyXML
    {
        #region 私有属性
        XmlDocument _XmlDoc;

        #endregion

        #region 属性
        /// <summary>
        /// 读取或设置配置文件名
        /// </summary>
        public string _ConfigFileName { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 读取配置文件是否存在的状态
        /// </summary>
        /// <returns></returns>
        public bool IsFileExist()
        {
            try
            {
                return System.IO.File.Exists(_ConfigFileName);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 实例化MYXML
        /// </summary>
        /// <param name="mConfigFileName"></param>
        public MyXML(string mConfigFileName)
        {
            _ConfigFileName = mConfigFileName;
        }

        /// <summary>
        /// 打开指定的配置文件
        /// </summary>
        /// <returns></returns>
        public bool OpenConfigFile()
        {
            if (!IsFileExist())
            {
                return false;
            }

            try
            {
                _XmlDoc = new XmlDocument();
                _XmlDoc.Load(_ConfigFileName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 保存文档
        /// </summary>
        /// <returns></returns>
        public bool SaveConfigFile()
        {
            if (_ConfigFileName.Length <= 0) { return false; }

            try
            {
                _XmlDoc.Save(_ConfigFileName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 读取指定节点名称及第一个元素名称的属性值，并返回
        /// </summary>
        /// <param name="ChildNodeName"></param>
        /// <param name="ElmentName"></param>
        /// <param name="AttributeName"></param>
        /// <returns></returns>
        public string ReadAttributeValue(string ChildNodeName,string ElmentName,string AttributeName)
        {
            string r = "";
            try
            {
                XmlNodeList nodeList = _XmlDoc.GetElementsByTagName(ChildNodeName)[0].ChildNodes;
                foreach(XmlNode xn in nodeList)
                {
                    
                    if(xn.Name==ElmentName)
                    {
                        foreach(XmlAttribute xa in xn.Attributes)
                        {
                            if (xa.Name == AttributeName) { r = xa.Value; }
                        }
                        break;
                    }
                }
                return r;
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 读取指定子节点及第一个匹配元素名称的全部属值
        /// </summary>
        /// <param name="ChildNodeName"></param>
        /// <param name="ElmentName"></param>
        /// <returns></returns>
        public List<AttributeList> ReadAttributeValue(string ChildNodeName, string ElmentName)
        {
            List<AttributeList> mAttributeList = new List<AttributeList>();
            try
            {
                XmlNodeList nodeList = _XmlDoc.GetElementsByTagName(ChildNodeName)[0].ChildNodes;
                foreach(XmlNode xn in nodeList)
                {
                    XmlElement xe = (XmlElement)xn;
                    if (xe.Name == ElmentName)
                    {
                        if(xe.HasAttributes)
                        {
                            foreach(XmlAttribute xr in xe.Attributes)
                            {
                                AttributeList al = new AttributeList(xr.Name, xr.Value);
                                mAttributeList.Add(al);
                            }
                            break;
                        }
                    }
                }
                return mAttributeList;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 读取指定子节点中所有元素名称及元素中全部属性信息
        /// </summary>
        /// <param name="ChildNodeName"></param>
        /// <returns></returns>
        public List<ElmentList> ReadAttributeValue(string ChildNodeName)
        {
            List<ElmentList> mElmentList = new List<ElmentList>();
            try
            {
                XmlNodeList nodeList = _XmlDoc.GetElementsByTagName(ChildNodeName)[0].ChildNodes;
                foreach(XmlNode xn in nodeList)
                {
                    XmlElement xe = (XmlElement)xn;
                    ElmentList el = new ElmentList();
                    el.ElmentName = xe.Name;
                    if(xe.HasAttributes)
                    {
                        foreach(XmlAttribute xr in xe.Attributes)
                        {
                            AttributeList al = new AttributeList(xr.Name, xr.Value);
                            el._AttributeList.Add(al);
                        }
                    }
                    mElmentList.Add(el);
                }
                return mElmentList;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 设置具有指定子节点及第一个子元素名称相匹配的其中一个属性名称的属性值
        /// </summary>
        /// <param name="ChildNodeName"></param>
        /// <param name="ElmentName"></param>
        /// <param name="AttributeName"></param>
        /// <param name="AttributeValue"></param>
        /// <returns></returns>
        public bool SetAttributeValue(string ChildNodeName,string ElmentName,string AttributeName,string AttributeValue)
        {
            try
            {
                XmlNodeList nodeList = _XmlDoc.GetElementsByTagName(ChildNodeName);
                foreach(XmlNode xn in nodeList[0].ChildNodes)
                {
                    XmlElement xe = (XmlElement)xn;
                    if(xe.Name == ElmentName)
                    {
                        if (xe.HasAttribute(AttributeName))
                        {
                            xe.SetAttribute(AttributeName, AttributeValue);
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 设置具有指定子节点及第一个子元素名称相匹配的全部属性名称的属性值
        /// </summary>
        /// <param name="ChildNodeName"></param>
        /// <param name="ElmentName"></param>
        /// <param name="mAttributeList"></param>
        /// <returns></returns>
        public bool SetAttributeValue(string ChildNodeName,string ElmentName,List<AttributeList> mAttributeList)
        {
            if (mAttributeList.Count <= 0) { return false; }
            try
            {
                XmlNodeList nodeList = _XmlDoc.GetElementsByTagName(ChildNodeName)[0].ChildNodes;
                foreach(XmlNode xn in nodeList)
                {
                    XmlElement xe = (XmlElement)xn;
                    if(xe.Name == ElmentName)
                    {
                        foreach(AttributeList al in xe.Attributes)
                        {
                            xe.SetAttribute(al.AttributeName, al.AttributeValue);
                        }
                        break;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 删除指定子节点名称下所有元素及属性
        /// </summary>
        /// <param name="ChildNodeName"></param>
        /// <returns></returns>
        public bool DeleteAllElmentFromChildNode(string ChildNodeName)
        {
            try
            {
                XmlNode xn = _XmlDoc.GetElementsByTagName(ChildNodeName)[0];
                xn.RemoveAll();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 删除指定根节点名称下所有子节点及属性
        /// </summary>
        /// <param name="RootNodeName"></param>
        /// <returns></returns>
        public bool DeleteAllElmentFromRootNode(string RootNodeName)
        {
            try
            {
                XmlNode xn = _XmlDoc.SelectSingleNode(RootNodeName);
                xn.RemoveAll();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// 在指定节点中插入子节点
        /// </summary>
        /// <param name="RootNodeName"></param>
        /// <param name="ChildNodeName"></param>
        /// <returns></returns>
        public bool InsertChildNode(string RootNodeName,string ChildNodeName)
        {
            try
            {
                XmlNode xn = _XmlDoc.GetElementsByTagName(RootNodeName)[0];
                XmlElement xe = _XmlDoc.CreateElement(ChildNodeName);
                xn.AppendChild(xn);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 在指定节点中插入子节点并设置属性信息
        /// </summary>
        /// <param name="RootNodeName"></param>
        /// <param name="ChildNodeName"></param>
        /// <param name="AttributeName"></param>
        /// <param name="AttributeValue"></param>
        /// <returns></returns>
        public bool InsertChildNode(string RootNodeName,string ChildNodeName,string AttributeName,string AttributeValue)
        {
            try
            {
                XmlNode rn = _XmlDoc.GetElementsByTagName(RootNodeName)[0];
                XmlElement xe = _XmlDoc.CreateElement(ChildNodeName);
                xe.SetAttribute(AttributeName, AttributeValue);
                rn.AppendChild(xe);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 在指定节点中插入子节点并设置多个属性信息
        /// </summary>
        /// <param name="RootNodeName"></param>
        /// <param name="ChildNodeName"></param>
        /// <param name="mAttributeList"></param>
        /// <returns></returns>
        public bool InsertChildNode(string RootNodeName,string ChildNodeName,List<AttributeList> mAttributeList)
        {
            try
            {
                XmlNode rn = _XmlDoc.GetElementsByTagName(RootNodeName)[0];
                XmlElement xn = _XmlDoc.CreateElement(ChildNodeName);
                foreach(AttributeList al in mAttributeList)
                {
                    xn.SetAttribute(al.AttributeName, al.AttributeValue);
                }
                rn.AppendChild(xn);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        #endregion
    }

    /// <summary>
    /// 用于返回属性名称与值结构
    /// </summary>
    public class AttributeList
    {
        public string AttributeName;
        public string AttributeValue;

        public AttributeList(string attributeName, string attributeValue)
        {
            AttributeName = attributeName;
            AttributeValue = attributeValue;
        }
    }

    /// <summary>
    /// 用于返回指字子节点中所有元素及属性信息
    /// </summary>
    public class ElmentList
    {
        public string ElmentName;
        public List<AttributeList> _AttributeList;

        public ElmentList()
        {
            _AttributeList = new List<AttributeList>();
        }

    }
}
