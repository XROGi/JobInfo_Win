using Ji.Services;
using SQLite;
using System;
using System.IO;


namespace Ji.Droid
{
    public static class ChatDataStore //: IChatDataStore
    {
        static SQLiteConnection db;
        static string dbPath = "";
        static ConnectInterface _connectInterface;
        static public ConnectInterface connectInterface
        {
            get
            {
                if (_connectInterface == null)
                {
                    _connectInterface = LoadConnect();
                }
                return _connectInterface;
            }
            set { _connectInterface = value; SaveConncet(); }
        }
        static public bool IsTableExists(string tableName)
        {
            try
            {
                var tableInfo = db.GetTableInfo(tableName);
                if (tableInfo.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        static private ConnectInterface LoadConnect()
        {
            OpenDB();
            TableQuery<Tbl_Connect> table = db.Table<Tbl_Connect>();
            if (table != null)
            {
                try
                {
                    if (IsTableExists("Tbl_Connect"))
                    {
                        foreach (Tbl_Connect s in table.ToList())
                        {
                            _connectInterface = new ConnectInterface();
                            _connectInterface.TokenReqId = s.TokenReqId;
                            _connectInterface.Server_SOAP = s.Server_SOAP;
                            _connectInterface.Server_WS = s.Server_WS;

                            break;
                        }
                    }
                }
                catch (Exception err)
                {

                }

            }
            else

                return null;

            return _connectInterface;
        }

        static private void SaveConncet()
        {
            OpenDB();
            var ConnectList = db.Table<Tbl_Connect>();

            db.CreateTable<Tbl_Connect>();
            if (db.Table<Tbl_Connect>().Count() == 0)
            {
                // only insert the data if it doesn't already exist
                var newStock = new Tbl_Connect()
                {
                    Server_SOAP = _connectInterface.Server_SOAP,
                    Server_WS = _connectInterface.Server_WS,
                    TokenReqId = _connectInterface.TokenReqId
                };
                db.Insert(newStock);
            }

        }

        static private void OpenDB()
        {
            if (db == null)
            {
                string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ji.db3");
                db = new SQLiteConnection(dbPath);
            }
        }
    }
}