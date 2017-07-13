using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.IO;

// 操作数据库类
namespace TkHome
{
    public class DbOperator
    {
        private string _dbName = "TkHome.dat";
        private List<SQLiteTable> _innerTableList = new List<SQLiteTable>();
        private SQLiteConnection _sqlConnection;
        private SQLiteCommand _sqlCommand;
        private SQLiteHelper _helper;
        public DbOperator()
        {
            // 表结构
            SQLiteTable productTable = new SQLiteTable("product");
            productTable.Columns.Add(new SQLiteColumn("id", ColType.Integer, true, true, true, "1"));
            productTable.Columns.Add(new SQLiteColumn("title", ColType.Text, false, false, true, "", true));
            productTable.Columns.Add(new SQLiteColumn("url", ColType.Text));
            productTable.Columns.Add(new SQLiteColumn("price", ColType.Text));
            productTable.Columns.Add(new SQLiteColumn("rate", ColType.Text));
            productTable.Columns.Add(new SQLiteColumn("commfee", ColType.Text));
            productTable.Columns.Add(new SQLiteColumn("sale30", ColType.Text));
            productTable.Columns.Add(new SQLiteColumn("addtime", ColType.Text));
            _innerTableList.Add(productTable);

            SQLiteTable optionTable = new SQLiteTable("option");
            optionTable.Columns.Add(new SQLiteColumn("id", ColType.Integer, true, true, true, "1"));
            optionTable.Columns.Add(new SQLiteColumn("reconnect", ColType.Integer, false, false, true, "0"));
            optionTable.Columns.Add(new SQLiteColumn("reconnect_delayseconds", ColType.Integer, false, false, true, "5"));
            optionTable.Columns.Add(new SQLiteColumn("collect_starttime", ColType.Integer));
            optionTable.Columns.Add(new SQLiteColumn("collect_endtime", ColType.Integer));
            optionTable.Columns.Add(new SQLiteColumn("collect_interval", ColType.Integer));
            optionTable.Columns.Add(new SQLiteColumn("collect_autosync", ColType.Integer, false, false, true, "0"));
            optionTable.Columns.Add(new SQLiteColumn("qunfa_starttime", ColType.Integer));
            optionTable.Columns.Add(new SQLiteColumn("qunfa_endtime", ColType.Integer));
            optionTable.Columns.Add(new SQLiteColumn("qunfa_interval", ColType.Integer));
            _innerTableList.Add(optionTable);
        }

        // 初始化数据库
        public void init()
        {
            try
            {
                bool bGenerateDefaultConfig = !File.Exists(_dbName);
                _sqlConnection = new SQLiteConnection("Data Source=" + _dbName);
                _sqlConnection.Open();
                _sqlCommand = new SQLiteCommand();
                _sqlCommand.Connection = _sqlConnection;
                _helper = new SQLiteHelper(_sqlCommand);

                DataTable dt = _helper.GetTableList(); // 获取表列表
                // 新增表
                foreach (SQLiteTable innerTable in _innerTableList)
                {
                    bool bFound = false;
                    foreach (DataRow row in dt.Rows)
                    {
                        string strTableName = row["Tables"] as string;
                        if (strTableName == innerTable.TableName)
                        {
                            bFound = true;
                            break;
                        }
                    }
                    if (!bFound)
                    {
                        // 表不存在，则创建
                        _helper.CreateTable(innerTable);
                    }
                    else
                    {
                        // 更新表
                        _helper.UpdateTableStructure(innerTable.TableName, innerTable);
                    }
                }

                // 删除表
                foreach (DataRow row in dt.Rows)
                {
                    string strTableName = row["Tables"] as string;
                    bool bFound = false;
                    foreach (SQLiteTable innerTable in _innerTableList)
                    {
                        if (strTableName == innerTable.TableName)
                        {
                            bFound = true;
                            break;
                        }
                    }
                    if (!bFound)
                    {
                        _helper.DropTable(strTableName);
                    }
                }

                if (bGenerateDefaultConfig)
                {
                    generateDefaultConfig(); // 产生默认配置
                }
            }
            catch (Exception)
            {
            }
        }

        // 关闭数据库
        public void deinit()
        {
            if (_sqlConnection != null)
            {
                _sqlConnection.Close();
            }
        }

        // 添加到自选库
        public void addProductList(List<ProductInfo> productList)
        {
            try
            {
                DateTime now = DateTime.Now;
                string addTime = now.ToString();

                _helper.BeginTransaction();
                foreach (ProductInfo item in productList)
                {
                    addProduct(item._title, item._auctionUrl, item._zkPrice, item._tkRate, item._tkCommFee, item._Sale30, addTime);
                }
                _helper.Commit();
            }
            catch (Exception)
            {
                _helper.Rollback();
            }
        }

        // 从自选库中加载商品信息
        public List<ProductInfo> loadProductList(int startRow, int count, bool bRandom = false)
        {
            List<ProductInfo> productList = new List<ProductInfo>();
            try
            {
                DataTable dt;
                if (bRandom)
                {
                    dt = _helper.Select("select * from product order by random() limit " + count.ToString()); // 随机取               
                }
                else
                {
                    dt = _helper.Select("select * from product limit " + startRow.ToString() + "," + count.ToString());
                }
                foreach (DataRow row in dt.Rows)
                {
                    int id = Convert.ToInt32(row["id"]);
                    string title = row["title"] as string;
                    string url = row["url"] as string;
                    string price = row["price"] as string;
                    string rate = row["rate"] as string;
                    string commfee = row["commfee"] as string;
                    string sale30 = row["sale30"] as string;
                    string addtime = row["addtime"] as string;
                    productList.Add(new ProductInfo(id, title, sale30, rate, commfee, price, url, addtime));
                }
            }
            catch (Exception)
            {
                
            }
            return productList;
        }

        // 根据产品ID列表来删除产品
        public void delProductIdList(List<int> productIdList)
        {
            try
            {
                _helper.BeginTransaction();
                foreach (int id in productIdList)
                {
                    _helper.Execute("delete from product where id = " + id.ToString());
                }
                _helper.Commit();
            }
            catch (Exception)
            {
                _helper.Rollback();
            }
        }

        public Configure loadConfigre()
        {
            Configure conf = new Configure();

            try
            {
                DataTable dt = _helper.Select("select * from option");
                conf.Reconnect = Convert.ToBoolean(dt.Rows[0]["reconnect"]);
                conf.ReconnectDelaySeconds = Convert.ToInt32(dt.Rows[0]["reconnect_delayseconds"]);

                conf.CollectStartTime = Convert.ToInt32(dt.Rows[0]["collect_starttime"]);
                conf.CollectEndTime = Convert.ToInt32(dt.Rows[0]["collect_endtime"]);
                conf.CollectInterval = Convert.ToInt32(dt.Rows[0]["collect_interval"]);
                conf.CollectAutoSync = Convert.ToBoolean(dt.Rows[0]["collect_autosync"]);

                conf.QunfaStartTime = Convert.ToInt32(dt.Rows[0]["qunfa_starttime"]);
                conf.QunfaEndTime = Convert.ToInt32(dt.Rows[0]["qunfa_endtime"]);
                conf.QunfaInterval = Convert.ToInt32(dt.Rows[0]["qunfa_interval"]);
            }
            catch (Exception)
            {
            }

            return conf;
        }

        public void saveCofigure(Configure conf)
        {
            string sql = "update option set reconnect = " + Convert.ToInt32(conf.Reconnect) + ", ";
            sql += "reconnect_delayseconds = " + conf.ReconnectDelaySeconds + ", ";
            sql += "collect_starttime = " + conf.CollectStartTime + ", ";
            sql += "collect_endtime = " + conf.CollectEndTime + ", ";
            sql += "collect_interval = " + conf.CollectInterval + ", ";
            sql += "collect_autosync = " + Convert.ToInt32(conf.CollectAutoSync) + ", ";
            sql += "qunfa_starttime = " + conf.QunfaStartTime + ", ";
            sql += "qunfa_endtime = " + conf.QunfaEndTime + ", ";
            sql += "qunfa_interval = " + conf.QunfaInterval;
            try
            {
                _helper.Execute(sql);
            }
            catch (Exception)
            {
            }
        }
        
        // 添加商品
        public void addProduct(string title, string url, string price, string rate, string commfee, string sale30, string addtime)
        {
            try
            {
                string enURL = EncryptDES.Encrypt(url); // URL进行加密
                string sql = "insert into product(title, url, price, rate, commfee, sale30, addtime) values ('" + title + "', '" + enURL + "', '"
                    + price + "', '" + rate + "', '" + commfee + "', '" + sale30 + "', '" + addtime + "')";
                _helper.Execute(sql);
            }
            catch (Exception)
            {
            }
        }
        // 生成默认配置
        private void generateDefaultConfig()
        {
            _helper.Execute("insert into option(reconnect, reconnect_delayseconds, collect_starttime, collect_endtime, collect_interval, collect_autosync, qunfa_starttime, qunfa_endtime, qunfa_interval) values(0, 5, 9, 21, 5, 0, 9, 21, 5)");
        }
    }
}
