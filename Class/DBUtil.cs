using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Data;

namespace WebMap.Web
{
    public static class DBUtil
    {
        public enum DBType
        {
            oledb,
            sqldb
        }
        public enum FieldType
        {
            text,
            number,
            oledb_date,
            sqldb_date,
            boolean
        }
        public enum FieldTypeEnum2
        {
            Text,
            Number
        }
        public enum DBConnType
        {
            Oledb,
            Sql
        }

        public static string CheckFeatureSAID_oledb(String connectionName, String tableName, String txtObjectIDText, String saidField)
        {
            string returnSAID = "";
            //Check if the data record as a valie SiteAccess ID
            string ConnStr = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
            OleDbConnection Conn = new OleDbConnection(ConnStr);
            string strSQL = string.Format("SELECT * FROM [{0}] WHERE [{1}] = {2}", tableName, "OBJECTID", txtObjectIDText);
            OleDbCommand pDBC = new OleDbCommand(strSQL, Conn);
            pDBC.Connection.Open();
            OleDbDataReader pDReader = pDBC.ExecuteReader();
            if (pDReader.HasRows)
            {
                pDReader.Read();
                if ((pDReader[saidField] is System.DBNull) || String.IsNullOrEmpty(pDReader[saidField].ToString()))
                {
                    //currently no SAID
                    OleDbConnection Conn2 = new OleDbConnection(ConnStr);

                    //assign temporary SAID
                    string newSAID = txtObjectIDText;

                    //check if feature class already uses this new SAID
                    string strSqlQuerySAID = string.Format("SELECT * FROM [{0}] WHERE [{1}]='{2}'", tableName, saidField, txtObjectIDText);
                    OleDbCommand queryCommand = new OleDbCommand(strSqlQuerySAID, Conn2);
                    queryCommand.Connection.Open();
                    int recordCount = queryCommand.ExecuteNonQuery();
                    queryCommand.Connection.Close();
                    if (recordCount == 0)
                    {
                        //OK, use OBJECTID as new SAID
                        //do nothing
                    }
                    else
                    {
                        //this SAID has already been used, find another one
                        string queryMaxSAID = string.Format("SELECT MAX(INT([{0}])) + 1 FROM [{1}]", saidField, tableName);
                        OleDbCommand maxSAIDCommand = new OleDbCommand(queryMaxSAID, Conn2);
                        maxSAIDCommand.Connection.Open();
                        newSAID = maxSAIDCommand.ExecuteScalar().ToString(); //this will not return null as we can only fall here if SAID already exists
                        maxSAIDCommand.Connection.Close();
                    }

                    //now assign SAID
                    string strSqlUpdate = string.Format("UPDATE [{0}] SET [{1}]='{2}' WHERE [{3}] = {4}", tableName, saidField, newSAID, "OBJECTID", txtObjectIDText);
                    OleDbCommand pUpdateCommand = new OleDbCommand(strSqlUpdate, Conn2);
                    pUpdateCommand.Connection.Open();
                    pUpdateCommand.ExecuteNonQuery();
                    pUpdateCommand.Connection.Close();
                    returnSAID = txtObjectIDText;

                }
                else
                {
                    //SAID exists, use it!!
                    returnSAID = pDReader[saidField].ToString();
                }
            }
            pDBC.Connection.Close();
            return returnSAID;
        }

        public static string CheckFeatureSAID_sqldb(String connectionName, String tableName, String txtObjectIDText, String saidField)
        {
            string returnSAID = "";
            //Check if the data record as a valie SiteAccess ID
            string ConnStr = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
            SqlConnection Conn = new SqlConnection(ConnStr);
            string strSQL = string.Format("SELECT * FROM [{0}] WHERE [{1}] = {2}", tableName, "OBJECTID", txtObjectIDText);
            SqlCommand pDBC = new SqlCommand(strSQL, Conn);
            pDBC.Connection.Open();
            SqlDataReader pDReader = pDBC.ExecuteReader();
            if (pDReader.HasRows)
            {
                pDReader.Read();
                if ((pDReader[saidField] is System.DBNull) || String.IsNullOrEmpty(pDReader[saidField].ToString()))
                {
                    //currently no SAID
                    SqlConnection Conn2 = new SqlConnection(ConnStr);

                    //assign temporary SAID
                    string newSAID = txtObjectIDText;

                    //check if feature class already uses this new SAID
                    string strSqlQuerySAID = string.Format("SELECT * FROM [{0}] WHERE [{1}]='{2}'", tableName, saidField, txtObjectIDText);
                    SqlCommand queryCommand = new SqlCommand(strSqlQuerySAID, Conn2);
                    queryCommand.Connection.Open();
                    int recordCount = queryCommand.ExecuteNonQuery();
                    queryCommand.Connection.Close();
                    if (recordCount == 0)
                    {
                        //OK, use OBJECTID as new SAID
                        //do nothing
                    }
                    else
                    {
                        //this SAID has already been used, find another one
                        string queryMaxSAID = string.Format("SELECT MAX(CAST([{0}] AS Int)) + 1 FROM [{1}]", saidField, tableName);
                        SqlCommand maxSAIDCommand = new SqlCommand(queryMaxSAID, Conn2);
                        maxSAIDCommand.Connection.Open();
                        newSAID = maxSAIDCommand.ExecuteScalar().ToString(); //this will not return null as we can only fall here if SAID already exists
                        maxSAIDCommand.Connection.Close();
                    }

                    //now assign SAID
                    string strSqlUpdate = string.Format("UPDATE [{0}] SET [{1}]='{2}' WHERE [{3}] = {4}", tableName, saidField, newSAID, "OBJECTID", txtObjectIDText);
                    SqlCommand pUpdateCommand = new SqlCommand(strSqlUpdate, Conn2);
                    pUpdateCommand.Connection.Open();
                    pUpdateCommand.ExecuteNonQuery();
                    pUpdateCommand.Connection.Close();
                    returnSAID = txtObjectIDText;

                }
                else
                {
                    //SAID exists, use it!!
                    returnSAID = pDReader[saidField].ToString();
                }
            }
            pDBC.Connection.Close();
            return returnSAID;
        }

        public static string DoLookup(DBType dbType, SqlDataSource pSqlDataSource, string strKeyField, string strKey, FieldType KeyType, string strDisplayField, string strSQL = "")
        {
            string functionReturnValue = null;
            functionReturnValue = "";
            try
            {
                string ConnStr = pSqlDataSource.ConnectionString;
                if (string.IsNullOrEmpty(strSQL.Trim()))
                    strSQL = pSqlDataSource.SelectCommand;
                string strFilter = string.Empty;
                switch (KeyType)
                {
                    case FieldType.number:
                        strFilter = string.Format("[{0}] = {1}", strKeyField, strKey);
                        break;
                    case FieldType.text:
                        strFilter = string.Format("[{0}] = '{1}'", strKeyField, strKey);
                        break;
                    case FieldType.oledb_date:
                        strFilter = string.Format("[{0}] = #{1}#", strKeyField, strKey);
                        break;
                    case FieldType.sqldb_date:
                        strFilter = string.Format("[{0}] = '{1}'", strKeyField, strKey);
                        break;
                    default:
                        strFilter = string.Format("[{0}] = '{1}'", strKeyField, strKey);
                        break;
                }
                if (strSQL.IndexOf("ORDER BY", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    strSQL = strSQL.Substring(0, strSQL.IndexOf("ORDER BY", StringComparison.CurrentCultureIgnoreCase));
                }
                if (strSQL.ToUpper().Contains("WHERE"))
                {
                    strFilter = string.Format(" AND {0}", strFilter);
                }
                else
                {
                    strFilter = string.Format(" WHERE {0}", strFilter);
                }
                strSQL += strFilter;

                DataSet pDS = new DataSet();
                switch (dbType)
                {
                    case DBType.sqldb:
                        SqlConnection Conn = new SqlConnection(ConnStr);
                        SqlDataAdapter pDA = new SqlDataAdapter(strSQL, Conn);
                        pDA.Fill(pDS, "MyLookupTable");
                        break;
                    case DBType.oledb:
                        OleDbConnection Conn2 = new OleDbConnection(ConnStr);
                        OleDbDataAdapter pDA2 = new OleDbDataAdapter(strSQL, Conn2);
                        pDA2.Fill(pDS, "MyLookupTable");
                        break;
                }
                DataTable pTable = pDS.Tables["MyLookupTable"];
                DataRow[] pDR = pTable.Select();
                functionReturnValue = pDR[0][strDisplayField].ToWSSafeString();

            }
            catch (Exception ex)
            {
            }
            return functionReturnValue;
        }

        public static string DoLookup(DBType dbType, String webconfigConnName, string strKeyField, string strKey, FieldType KeyType, string strDisplayField, string strInitialSQL = "")
        {
            string functionReturnValue = null;
            functionReturnValue = "";
            String strSQL = strInitialSQL;
            try
            {
                string ConnStr = ConfigurationManager.ConnectionStrings[webconfigConnName].ConnectionString;
                string strFilter = string.Empty;
                switch (KeyType)
                {
                    case FieldType.number:
                        strFilter = string.Format("[{0}] = {1}", strKeyField, strKey);
                        break;
                    case FieldType.text:
                        strFilter = string.Format("[{0}] = '{1}'", strKeyField, strKey);
                        break;
                    case FieldType.oledb_date:
                        strFilter = string.Format("[{0}] = #{1}#", strKeyField, strKey);
                        break;
                    case FieldType.sqldb_date:
                        strFilter = string.Format("[{0}] = '{1}'", strKeyField, strKey);
                        break;
                    default:
                        strFilter = string.Format("[{0}] = '{1}'", strKeyField, strKey);
                        break;
                }
                if (strSQL.IndexOf("ORDER BY", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    strSQL = strSQL.Substring(0, strSQL.IndexOf("ORDER BY", StringComparison.CurrentCultureIgnoreCase));
                }
                if (strSQL.ToUpper().Contains("WHERE"))
                {
                    strFilter = string.Format(" AND {0}", strFilter);
                }
                else
                {
                    strFilter = string.Format(" WHERE {0}", strFilter);
                }
                strSQL += strFilter;

                DataSet pDS = new DataSet();
                switch (dbType)
                {
                    case DBType.oledb:
                        OleDbConnection Conn2 = new OleDbConnection(ConnStr);
                        OleDbDataAdapter pDA2 = new OleDbDataAdapter(strSQL, Conn2);
                        pDA2.Fill(pDS, "MyLookupTable");
                        break;
                    case DBType.sqldb:
                        SqlConnection Conn = new SqlConnection(ConnStr);
                        SqlDataAdapter pDA = new SqlDataAdapter(strSQL, Conn);
                        pDA.Fill(pDS, "MyLookupTable");
                        break;
                }
                DataTable pTable = pDS.Tables["MyLookupTable"];
                DataRow[] pDR = pTable.Select();
                functionReturnValue = pDR[0][strDisplayField].ToWSSafeString();

            }
            catch (Exception ex)
            {
            }
            return functionReturnValue;
        }

        //function with old signature for backward compatibility
        public static string DoLookup(string sDBTYPE, SqlDataSource pSqlDataSource, string strKeyField, string strKey, string KeyType, string strDisplayField, string strSQL = "")
        {
            string functionReturnValue = null;
            functionReturnValue = "";
            try
            {
                string ConnStr = pSqlDataSource.ConnectionString;
                if (string.IsNullOrEmpty(strSQL.Trim()))
                    strSQL = pSqlDataSource.SelectCommand;
                string strFilter = string.Empty;
                switch (KeyType.Trim().ToUpper())
                {
                    case "NUM":
                    case "NUMBER":
                    case "INTEGER":
                    case "INT16":
                    case "INT32":
                    case "DOUBLE":
                    case "SINGLE":
                        strFilter = string.Format("[{0}] = {1}", strKeyField, strKey);
                        break;
                    case "STRING":
                    case "TEXT":
                        strFilter = string.Format("[{0}] = '{1}'", strKeyField, strKey);
                        break;
                    case "OLEDATE":
                        strFilter = string.Format("[{0}] = #{1}#", strKeyField, strKey);
                        break;
                    case "SQLDATE":
                        strFilter = string.Format("[{0}] = '{1}'", strKeyField, strKey);
                        break;
                    default:
                        strFilter = string.Format("[{0}] = '{1}'", strKeyField, strKey);
                        break;
                }
                if (strSQL.IndexOf("ORDER BY", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    strSQL = strSQL.Substring(0, strSQL.IndexOf("ORDER BY", StringComparison.CurrentCultureIgnoreCase));
                }
                if (strSQL.ToUpper().Contains("WHERE"))
                {
                    strFilter = string.Format(" AND {0}", strFilter);
                }
                else
                {
                    strFilter = string.Format(" WHERE {0}", strFilter);
                }
                strSQL += strFilter;

                DataSet pDS = new DataSet();
                switch (sDBTYPE.ToUpper().Trim())
                {
                    case "SQL":
                        SqlConnection Conn = new SqlConnection(ConnStr);
                        SqlDataAdapter pDA = new SqlDataAdapter(strSQL, Conn);
                        pDA.Fill(pDS, "MyLookupTable");
                        break;
                    case "OLEDB":
                        OleDbConnection Conn2 = new OleDbConnection(ConnStr);
                        OleDbDataAdapter pDA2 = new OleDbDataAdapter(strSQL, Conn2);
                        pDA2.Fill(pDS, "MyLookupTable");
                        break;
                }
                DataTable pTable = pDS.Tables["MyLookupTable"];
                DataRow[] pDR = pTable.Select();
                functionReturnValue = pDR[0][strDisplayField].ToWSSafeString();

            }
            catch (Exception ex)
            {
            }
            return functionReturnValue;
        }

        //new function lookup multiple field
        public static List<String> DoLookupSQL(String connectionName, String tableName, String Keyfield, FieldTypeEnum2 KeyType, String KeyValue, List<String> FieldToReturn)
        {
            string ConnStr = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
            SqlConnection Conn = new SqlConnection(ConnStr);
            String enclosure = (KeyType == FieldTypeEnum2.Text) ? "'" : "";
            String strSQL = string.Format("SELECT * FROM [{0}] WHERE [{1}] = {3}{2}{3}", tableName, Keyfield, KeyValue, enclosure);
            SqlCommand pDBC = new SqlCommand(strSQL, Conn);
            pDBC.Connection.Open();
            SqlDataReader pDReader = pDBC.ExecuteReader();
            List<String> returnval = new List<String>();
            if (pDReader.HasRows)
            {
                pDReader.Read();
                foreach (String f2r in FieldToReturn)
                {
                    returnval.Add(pDReader[f2r].ToWSSafeString());
                }
            }
            pDBC.Connection.Close();
            return returnval;
        }

        public static List<String> DoLookupOLEDB(String connectionName, String tableName, String Keyfield, FieldTypeEnum2 KeyType, String KeyValue, List<String> FieldToReturn)
        {
            string ConnStr = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
            OleDbConnection Conn = new OleDbConnection(ConnStr);
            String enclosure = (KeyType == FieldTypeEnum2.Text) ? "'" : "";
            String strSQL = string.Format("SELECT * FROM [{0}] WHERE [{1}] = {3}{2}{3}", tableName, Keyfield, KeyValue, enclosure);
            OleDbCommand pDBC = new OleDbCommand(strSQL, Conn);
            pDBC.Connection.Open();
            OleDbDataReader pDReader = pDBC.ExecuteReader();
            List<String> returnval = new List<String>();
            if (pDReader.HasRows)
            {
                pDReader.Read();
                foreach (String f2r in FieldToReturn)
                {
                    returnval.Add(pDReader[f2r].ToWSSafeString());
                }
            }
            pDBC.Connection.Close();
            return returnval;
        }
        
        public static DataTable GetOLEDataTable(string sFeatureClassDBConnection, SqlDataSource sqlDataSourceControl, string sTableName)
        {
            string ConnStr = ConfigurationManager.ConnectionStrings[sFeatureClassDBConnection].ConnectionString;
            OleDbConnection Conn = new OleDbConnection(ConnStr);
            string strSQL = sqlDataSourceControl.SelectCommand;
            OleDbDataAdapter pDA = new OleDbDataAdapter(strSQL, ConnStr);
            DataSet pDS = new DataSet();
            pDA.Fill(pDS, sTableName);
            System.Web.HttpContext.Current.Session[sTableName] = pDS.Tables[sTableName];  //this is used in form dd routine for caching lookup table
            return pDS.Tables[sTableName];
        }
        public static DataTable GetOLEDataTable(string sDBConnection, string sQuery, string sTableName)
        {
            string ConnStr = ConfigurationManager.ConnectionStrings[sDBConnection].ConnectionString;
            OleDbConnection Conn = new OleDbConnection(ConnStr);
            OleDbDataAdapter pDA = new OleDbDataAdapter(sQuery, Conn);
            DataSet pDS = new DataSet();
            pDA.Fill(pDS, sTableName);
            return pDS.Tables[sTableName];
        }
        public static DataTable GetSQLDataTable(string sFeatureClassDBConnection, SqlDataSource sqlDataSourceControl, string sTableName)
        {
            string ConnStr = ConfigurationManager.ConnectionStrings[sFeatureClassDBConnection].ConnectionString;
            SqlConnection Conn = new SqlConnection(ConnStr);
            string strSQL = sqlDataSourceControl.SelectCommand;
            SqlDataAdapter pDA = new SqlDataAdapter(strSQL, ConnStr);
            DataSet pDS = new DataSet();
            pDA.Fill(pDS, sTableName);
            System.Web.HttpContext.Current.Session[sTableName] = pDS.Tables[sTableName];  //this is used in form dd routine for caching lookup table
            return pDS.Tables[sTableName];
        }
        public static DataTable GetSQLDataTable(string sDBConnection, string sQuery, string sTableName)
        {
            string ConnStr = ConfigurationManager.ConnectionStrings[sDBConnection].ConnectionString;
            SqlConnection Conn = new SqlConnection(ConnStr);
            SqlDataAdapter pDA = new SqlDataAdapter(sQuery, Conn);
            DataSet pDS = new DataSet();
            pDA.Fill(pDS, sTableName);
            return pDS.Tables[sTableName];
        }

        public static Int32 ConnExecute(DBConnType conntype, String connname, String sql)
        {
            string ConnStr = ConfigurationManager.ConnectionStrings[connname].ConnectionString;
            switch (conntype)
            {
                case DBConnType.Oledb:
                    OleDbConnection Conn = new OleDbConnection(ConnStr);
                    OleDbCommand cmd = new OleDbCommand(sql, Conn);
                    cmd.Connection.Open();
                    Int32 rowAffected = cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                    return rowAffected;
                case DBConnType.Sql:
                    SqlConnection Conn2 = new SqlConnection(ConnStr);
                    SqlCommand cmd2 = new SqlCommand(sql, Conn2);
                    cmd2.Connection.Open();
                    Int32 rowAffected2 = cmd2.ExecuteNonQuery();
                    cmd2.Connection.Close();
                    return rowAffected2;
                default:
                    return -1;
            }
        }

    }
}