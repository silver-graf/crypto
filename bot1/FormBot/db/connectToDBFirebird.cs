using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using System.Web.UI;

namespace FormBot.db
{
    class connectToDBFirebird
    {
        public connectToDBFirebird()
        {
            FbCommand cmd = null;
            FbConnectionStringBuilder fbConnectBild = new FbConnectionStringBuilder();


            //fbConnectBild.DataSource = IDataSource;     //local
            //fbConnectBild.Port = int.Parse(Port);       //"3050";
            //fbConnectBild.Database = Database;          //"kharkov";
            //fbConnectBild.UserID = FBUser;          //"rpt";
            //fbConnectBild.Password = FBPass;            //"rpt";
            //fbConnectBild.Charset = Charset;            //"NONE";
            //fbConnectBild.IsolationLevel = IsolationLevel.ReadCommitted;
            fbConnectBild.Pooling = false;
            FbConnection fbConnect = new FbConnection(fbConnectBild.ToString());
            FbTransaction fbTransact;
            Loger.SetLog("440 " + fbConnectBild.ToString());
            try
            {
                fbConnect.Open();
                FbTransactionOptions option = new FbTransactionOptions();
                option.TransactionBehavior = FbTransactionBehavior.NoWait | FbTransactionBehavior.ReadCommitted | FbTransactionBehavior.RecVersion;
                fbTransact = fbConnect.BeginTransaction(option);
            }
            catch (FbException err)
            {
                Loger.SetLog("449 " + err.Message);
                //MessageBox.Show("Неудачное подключение,проверьте параметр в файле настроек \r\n" + err.Message);
                //Clipboard.SetText("Неудачное подключение \r\n" + err.Message + "\r\nСтрока подключения : " + fbConnectBild.ToString());
                return;
            }

            //string query = String.Format(@"update outcomeinvoice set printflag = 1 where id = {0}", DocId);
            //Loger.SetLog("456 " + query);

            try
            {
                //cmd = new FbCommand(query, fbConnect, fbTransact);
                Loger.SetLog("461 cmd = new FbCommand(query, fbConnect, fbTransact);");
                cmd.ExecuteNonQuery();
                Loger.SetLog("463 cmd.ExecuteNonQuery();");
                cmd.Dispose();
                Loger.SetLog("465 cmd.Dispose();");
                fbTransact.CommitRetaining();
                Loger.SetLog("467 fbTransact.CommitRetaining();");
                fbConnect.Close();
                Loger.SetLog("469 fbConnect.Close();");

            }
            catch (FbException err)
            {
                //string message = String.Format("Не удалось установить признак печати чека \r\nЗапрос : {0} \r\nОшибка : {1}", query, err.Message);
                //MessageBox.Show(message);
                //Clipboard.SetText(message);
                cmd.Dispose();
                fbConnect.Close();
                return;
            }

        }
   
    }
}
