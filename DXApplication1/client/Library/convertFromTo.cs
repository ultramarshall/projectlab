using System;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace client.Library
{
    public static class convertFromTo
    {
        public static string[] ExcelSheetList(string pathName)
        {

            string strConn = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;'", pathName);
            FileInfo file = new FileInfo(pathName);
            OleDbConnection cnnxls = new OleDbConnection(strConn);
            cnnxls.Open();
            DataTable datatb = cnnxls.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            if (datatb == null)
            {
                return null;
            }
            string[] excelSheets = new String[datatb.Rows.Count];
            int c = 0;
            foreach (DataRow row in datatb.Rows)
            {
                excelSheets[c] = row["TABLE_NAME"].ToString().Replace("$", "");
                c++;
            }
            cnnxls.Close();
            return excelSheets;
        }
        public static DataTable ExcelToDataTable(string pathName, string sheetName)
        {
            DataTable tbContainer = new DataTable();
            string strConn = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;'", pathName);
            if (string.IsNullOrEmpty(sheetName)) { sheetName = "Sheet1"; }
            FileInfo file = new FileInfo(pathName);
            if (!file.Exists) { throw new Exception("Error, file doesn't exists!"); }

            OleDbConnection cnnxls = new OleDbConnection(strConn);

            OleDbDataAdapter oda = new OleDbDataAdapter(string.Format("select * from [{0}$]", sheetName), cnnxls);
            DataSet ds = new DataSet();
            oda.Fill(tbContainer);
            return tbContainer;
        }
        public static byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }
    }
}
