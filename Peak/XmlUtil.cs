using System;
using System.Xml;

namespace Peak
{
    public class XmlUtil
    {
 
        /// <summary>
        /// </summary>
        /// <param name="fliepath"></param>
        /// <param name="rootEle"></param>
        /// <param name="eles"></param>
        public static void CreateXml(string fliepath, string rootEle, string[] eles)
        {
            var doc = new XmlDocument();
            var dec = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(dec);
            //创建一个根节点（一级）
            var root = doc.CreateElement(rootEle);
            doc.AppendChild(root);
            //创建节点（二级）
            foreach (var t in eles)
            {
                XmlNode nodel = doc.CreateElement(t);
                nodel.InnerText = "";
                root.AppendChild(nodel);
            }
            doc.Save(fliepath);
        }

        /// <summary>
        ///     创建XML文件
        /// </summary>
        /// <param name="fliepath">文件路径</param>
        /// <param name="rootEle">根元素</param>
        /// <param name="eles">一级元素</param>
        /// <param name="elesvalue">一级元素的值</param>
        public static void CreateXml(string fliepath, string rootEle, string[] eles, string[] elesvalue)
        {
            if (eles.Length == elesvalue.Length)
            {
                var doc = new XmlDocument();
                var dec = doc.CreateXmlDeclaration("1.0", "utf-8", null);
                doc.AppendChild(dec);
                //创建一个根节点（一级）
                var root = doc.CreateElement(rootEle);
                doc.AppendChild(root);
                //创建节点（二级）
                for (var i = 0; i < eles.Length; i++)
                {
                    XmlNode nodel = doc.CreateElement(eles[i]);
                    nodel.InnerText = elesvalue[i];
                    root.AppendChild(nodel);
                }
                doc.Save(fliepath);
            }
            else
            {
                throw new Exception("eles和elesvalue的长度不一致");
            }
        }

        /// <summary>
        ///     向XML指定元素内添加一组元素
        /// </summary>
        /// <param name="fliepath">文件路径</param>
        /// <param name="elePath">元素路径</param>
        /// <param name="eles">
        ///     元素
        /// </param>
        /// <param name="elesvalue">元素值</param>
        public static void AddXmlElementsToOneElement(string fliepath, string elePath, string[] eles,
            string[] elesvalue)
        {
            var doc = new XmlDocument();
            doc.Load(fliepath);
            var root = doc.DocumentElement;
            var list = root?.SelectNodes(elePath);
            if (list != null)
                for (var i = 0; i < list.Count; i++)
                for (var j = 0; j < eles.Length; j++)
                {
                    XmlNode nodel = doc.CreateElement(eles[j]);
                    nodel.InnerText = elesvalue[j];
                    list[i].AppendChild(nodel);
                }
            doc.Save(fliepath);
        }
    }
}