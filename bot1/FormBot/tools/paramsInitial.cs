using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.IO;

namespace FormBot.tools
{
    class ParamsInitial             
    {

        private string PathParams = @"bot.xml";
        private static string fBUser = "sysdba";
        private static string fBPass = "masterkey";
        private static string dataSource = null;
        private static string port = null;
        private static string database = null;
        private static string charset = null;

        public static string FBUser { get { return fBUser; } private set { } }
        public static string FBPass { get { return fBPass; } private set { } }
        public static string DataSource { get { return dataSource; } private set { } }
        public static string Database { get { return database; } private set { } }
        public static string Charset { get { return charset; } private set { } }
        public static string Port { get { return port; } private set { } }

        public ParamsInitial()
        {
            LoadParam(PathParams);
                
        }

        private void LoadParam (string XMLFile)
        {
            XDocument setting;
            if (File.Exists(XMLFile))
            {
                setting = XDocument.Load(XMLFile);             
                dataSource = setting.Root.Element("DataSource").Value;
                port = setting.Root.Element("Port").Value;
                database = setting.Root.Element("Database").Value;
                fBUser = setting.Root.Element("FBUser").Value;
                fBPass = setting.Root.Element("FBPass").Value;
                charset = setting.Root.Element("Charset").Value;              
                
            }
            else
            {
                setting = new XDocument(
                    new XElement("Root",
                    new XElement("dbconn", ""),
                    new XElement("DataSource", @"D:\c#\db_firebird\bot.fdb"),                          //<DataSource>192.168.0.8</DataSource>
                    new XElement("Port", "3050"),                                       //<Port>3050</Port>
                    new XElement("Database", "bot"),                                //<Database>kharkov_test</Database>
                    new XElement("FBUser", "sysdba"),                                    //<User>Login</User>
                    new XElement("FBPass", "masterkey"),                                    //<Pass>Login</Pass>
                    new XElement("Charset", "UTF8")					  			//<Charset>WIN1251</Charset>
                   
                ));
                setting.Save(XMLFile);
         
            }

        }






     




    }

}             
