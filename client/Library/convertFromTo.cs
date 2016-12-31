using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Reflection;

namespace client.Library
{
    public static class ConvertFromTo
    {
        public static IEnumerable<string> ExcelSheetList(string pathName)
        {
            string strConn =
                $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={pathName};Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;'";
            var file = new FileInfo(pathName);
            var cnnxls = new OleDbConnection(strConn);
            cnnxls.Open();
            var datatb = cnnxls.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            if (datatb == null)
            {
                return null;
            }
            var excelSheets = new string[datatb.Rows.Count];
            var c = 0;
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
            var strConn =
                string.Format(
                    "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;'",
                    pathName);
            if (string.IsNullOrEmpty(sheetName))
            {
                sheetName = "Sheet1";
            }
            var file = new FileInfo(pathName);
            if (!file.Exists)
            {
                throw new Exception("Error, file doesn't exists!");
            }

            var cnnxls = new OleDbConnection(strConn);

            OleDbDataAdapter oda = new OleDbDataAdapter($"select * from [{sheetName}$]", cnnxls);
            DataSet ds = new DataSet();
            oda.Fill(tbContainer);
            return tbContainer;
        }

        public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            var dataTable = new DataTable(typeof (T).Name);

            //Get all the properties
            var props = typeof (T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }
}