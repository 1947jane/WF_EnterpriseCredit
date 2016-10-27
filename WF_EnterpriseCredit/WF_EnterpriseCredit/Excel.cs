using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Data.OleDb;
//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Spreadsheet;
using System.Xml;
using System.Diagnostics;
//using DocumentFormat.OpenXml;
using System.Reflection;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Office.Interop.Excel;
using ExcelApplication = Microsoft.Office.Interop.Excel.Application;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System.Configuration;

namespace WF_EnterpriseCredit
{
    public class Excel
    {

        #region OleDb读取excel文件返回dateset
        /// <summary>
        /// 读取excel文件
        /// </summary>
        /// <param name="filePath">文件地址</param>
        /// <param name="i">表号0则为NI的数据，1则为TN的数据</param>
        /// <returns></returns>
        public static DataSet LoadDataFromExcel(string filePath, int i, string code, int num, int num1)
        {
            DataSet myDataSet = new DataSet();
            //连接Excel文件，读入myDstaSet，serverpath是Excel文件有服务器中路径
            try
            {
                String strConnectionString = string.Format("Provider=Microsoft.{0}.OLEDB.{3}.0;Data Source={1};Extended Properties='Excel {2}.0;HDR=No;IMEX=1'", code, filePath, num, num1);
                OleDbConnection Excel_conn = new OleDbConnection(strConnectionString);
                Excel_conn.Open();
                System.Data.DataTable dtExcelSchema = Excel_conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });//建立连接Excel的数据表
                string SheetName = "";
                SheetName = dtExcelSchema.Rows[i]["TABLE_NAME"].ToString();//取出第一个工作表名称
                Excel_conn.Close();
                string query = "SELECT * FROM " + "[" + SheetName + "]";//查询Excel字符串
                OleDbDataAdapter oleAdapter = new OleDbDataAdapter(query, Excel_conn);
                Excel_conn.Open();
                oleAdapter.Fill(myDataSet, "Excel_Sheet1");
                Excel_conn.Close();
                return myDataSet;

            }
            catch (Exception e) { }
            return myDataSet;

        }
        #endregion

        #region 利用NPOI导入EXCEL
        #region Excel2003
        /// <summary>
        /// 将Excel文件中的数据读出到DataTable中(xls)
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static System.Data.DataTable ExcelToTableForXLS(string file)
        {
            try
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    HSSFWorkbook hssfworkbook = new HSSFWorkbook(fs);
                    ISheet sheet = hssfworkbook.GetSheetAt(0);

                    //表头
                    IRow header = sheet.GetRow(sheet.FirstRowNum);
                    List<int> columns = new List<int>();
                    for (int i = 0; i < header.LastCellNum; i++)
                    {
                        object obj = GetValueTypeForXLS(header.GetCell(i) as HSSFCell);
                        if (obj == null || obj.ToString() == string.Empty)
                        {
                            dt.Columns.Add(new DataColumn("Columns" + i.ToString()));
                            //continue;
                        }
                        else
                            dt.Columns.Add(new DataColumn(obj.ToString()));
                        columns.Add(i);
                    }
                    //数据
                    for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                    {
                        DataRow dr = dt.NewRow();
                        bool hasValue = false;
                        foreach (int j in columns)
                        {
                            dr[j] = GetValueTypeForXLS(sheet.GetRow(i).GetCell(j) as HSSFCell);
                            if (dr[j] != null && dr[j].ToString() != string.Empty)
                            {
                                hasValue = true;
                            }
                        }
                        if (hasValue)
                        {
                            dt.Rows.Add(dr);
                        }
                    }
                }
                return dt;
            }
            catch (Exception exp)
            {
                return null;
            }
        }


        /// <summary>
        /// 获取单元格类型(xls)
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private static object GetValueTypeForXLS(HSSFCell cell)
        {
            if (cell == null)
                return null;
            switch (cell.CellType)
            {
                case CellType.Blank: //BLANK:
                    return null;
                case CellType.Boolean: //BOOLEAN:
                    return cell.BooleanCellValue;
                case CellType.Numeric: //NUMERIC:
                    return cell.NumericCellValue;
                case CellType.String: //STRING:
                    return cell.StringCellValue;
                case CellType.Error: //ERROR:
                    return cell.ErrorCellValue;
                case CellType.Formula: //FORMULA:
                default:
                    return "=" + cell.CellFormula;
            }
        }
        #endregion

        #region Excel2007
        /// <summary>
        /// 将Excel文件中的数据读出到DataTable中(xlsx)
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static System.Data.DataTable ExcelToTableForXLSX(string file)
        {
            try
            {

                System.Data.DataTable dt = new System.Data.DataTable();
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    XSSFWorkbook xssfworkbook = new XSSFWorkbook(fs);
                    ISheet sheet = xssfworkbook.GetSheetAt(0);

                    //表头
                    IRow header = sheet.GetRow(sheet.FirstRowNum);
                    List<int> columns = new List<int>();
                    for (int i = 0; i < header.LastCellNum; i++)
                    {
                        object obj = GetValueTypeForXLSX(header.GetCell(i) as XSSFCell);
                        if (obj == null || obj.ToString() == string.Empty)
                        {
                            dt.Columns.Add(new DataColumn("Columns" + i.ToString()));
                            //continue;
                        }
                        else
                            dt.Columns.Add(new DataColumn(obj.ToString()));
                        columns.Add(i);
                    }
                    //数据
                    for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                    {
                        DataRow dr = dt.NewRow();
                        bool hasValue = false;
                        foreach (int j in columns)
                        {
                            dr[j] = GetValueTypeForXLSX(sheet.GetRow(i).GetCell(j) as XSSFCell);
                            if (dr[j] != null && dr[j].ToString() != string.Empty)
                            {
                                hasValue = true;
                            }
                        }
                        if (hasValue)
                        {
                            dt.Rows.Add(dr);
                        }
                    }
                }
                return dt;
            }
            catch (Exception exp)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取单元格类型(xlsx)
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private static object GetValueTypeForXLSX(XSSFCell cell)
        {
            if (cell == null)
                return null;
            switch (cell.CellType)
            {
                case CellType.Blank: //BLANK:
                    return null;
                case CellType.Boolean: //BOOLEAN:
                    return cell.BooleanCellValue;
                case CellType.Numeric: //NUMERIC:
                    return cell.NumericCellValue;
                case CellType.String: //STRING:
                    return cell.StringCellValue;
                case CellType.Error: //ERROR:
                    return cell.ErrorCellValue;
                case CellType.Formula: //FORMULA:
                default:
                    return "=" + cell.CellFormula;
            }
        }
        #endregion
        #endregion

        #region 利用NPOI导出EXCEL
        /// <summary>
        /// ListView反向填充DataTable数据项
        /// </summary>
        /// <param name="lv">ListView控件</param>
        /// <param name="dt">DataTable</param>
        public static System.Data.DataTable listViewToDataTable(ListView lv)
        {
            try
            {

                System.Data.DataTable dt = new System.Data.DataTable();
                DataRow dr;
                dt.Clear();
                dt.Columns.Clear();
                for (int k = 0; k < lv.Columns.Count; k++)
                {
                    dt.Columns.Add(lv.Columns[k].Text.Trim().ToString());//生成DataTable列头
                }
                for (int i = 0; i < lv.Items.Count; i++)
                {
                    dr = dt.NewRow();
                    for (int j = 0; j < lv.Columns.Count; j++)
                    {
                        dr[j] = lv.Items[i].SubItems[j].Text.Trim();
                    }
                    dt.Rows.Add(dr);
                }
                return dt;
            }
            catch (Exception exp)
            {
                return null;
            }
        }

        /// <summary>
        /// 将DataTable数据导出到Excel文件中(xls)
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="file"></param>
        public static bool TableToExcelForXLS(System.Data.DataTable dt, string saveFileName, int intcount)
        {
            try
            {
                //string saveFileName = "";
                //SaveFileDialog saveDialog = new SaveFileDialog();
                //saveDialog.DefaultExt = "xls";
                //saveDialog.Filter = "Excel文件|*.xls";
                //saveDialog.FileName = DateTime.Now.ToString("yyyy-MM-dd");
                //DialogResult a = saveDialog.ShowDialog();
                //saveFileName = saveDialog.FileName;
                HSSFWorkbook hssfworkbook = new HSSFWorkbook();
                ISheet sheet = hssfworkbook.CreateSheet("天眼查");

                //表头
                if (intcount == 1)
                {
                    IRow row = sheet.CreateRow(0);
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        ICell cell = row.CreateCell(i);
                        cell.SetCellValue(dt.Columns[i].ColumnName);
                    }
                }
                //数据
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row1 = sheet.CreateRow(i + 1);
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        ICell cell = row1.CreateCell(j);
                        cell.SetCellValue(dt.Rows[i][j].ToString());
                    }
                }

                //转为字节数组
                MemoryStream stream = new MemoryStream();
                hssfworkbook.Write(stream);
                var buf = stream.ToArray();

                //保存为Excel文件
                using (FileStream fs = new FileStream(saveFileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(buf, 0, buf.Length);
                    fs.Flush();
                }
                return true;
                //if (a == DialogResult.OK)
                //{
                //    return true;
                //}
                //else
                //{
                //    return false;
                //}
            }
            catch (Exception exp)
            {
                return false;
            }
        }


        /// <summary>
        /// 将DataTable数据导出到Excel文件中(xlsx)
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="file"></param>
        public static bool TableToExcelForXLSX(System.Data.DataTable dt, string saveFileName, string keyword, int intcount)
        {
            try
            {
                //保存为Excel文件
                using (FileStream fs = new FileStream(saveFileName, FileMode.OpenOrCreate, FileAccess.Write))
                {

                    XSSFWorkbook xssfworkbook = new XSSFWorkbook();
                    ISheet sheet = xssfworkbook.CreateSheet(keyword);

                    //表头
                    IRow row = sheet.CreateRow(0);
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        ICell cell = row.CreateCell(i);
                        cell.SetCellValue(dt.Columns[i].ColumnName);
                    }

                    //数据
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        IRow row1 = sheet.CreateRow(i + 1);
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            ICell cell = row1.CreateCell(j);
                            cell.SetCellValue(dt.Rows[i][j].ToString());
                        }
                    }

                    //转为字节数组
                    MemoryStream stream = new MemoryStream();
                    xssfworkbook.Write(stream);
                    var buf = stream.ToArray();
                    fs.Write(buf, 0, buf.Length);
                    fs.Flush();
                    fs.Close();
                }
                return true;
            }
            catch (Exception exp)
            {
                return false;
            }

        }
        #endregion

        /// <summary>
        /// 将DataTable数据导入到excel中
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="data">要导入的数据</param>
        /// <param name="sheetName">要导入的excel的sheet的名称</param>
        /// <param name="blnAppled">是否是追加模式</param>
        /// <param name="isColumnWritten">DataTable的列名是否要导入</param>
        /// <returns>导入数据行数(包含列名那一行)</returns>
        public static int DataTableToExcel(string fileName, System.Data.DataTable data, bool blnAppled = false, string sheetName = "sheet1", bool isColumnWritten = true)
        {
            int i = 0;
            int j = 0;
            int count = 0;
            ISheet sheet = null;
            IWorkbook workbook = null;
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Read))
            {
                if (!blnAppled)
                {
                    if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                        workbook = new XSSFWorkbook();
                    else if (fileName.IndexOf(".xls") > 0) // 2003版本
                        workbook = new HSSFWorkbook();
                }
                else
                {
                    if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                        workbook = new XSSFWorkbook(fs);
                    else if (fileName.IndexOf(".xls") > 0) // 2003版本
                        workbook = new HSSFWorkbook(fs);
                }

                try
                {
                    if (!blnAppled && !string.IsNullOrEmpty(sheetName))
                    {
                        if (workbook != null)
                        {
                            sheet = workbook.CreateSheet(sheetName);
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    else
                    {
                        sheet = workbook.GetSheetAt(0);
                    }

                    if (!blnAppled)
                    {
                        if (isColumnWritten == true) //写入DataTable的列名
                        {
                            IRow row = sheet.CreateRow(0);
                            for (j = 0; j < data.Columns.Count; ++j)
                            {
                                row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                            }
                            count = 1;
                        }
                        else
                        {
                            count = 0;
                        }
                    }

                    count = sheet.LastRowNum + 1;

                    for (i = 0; i < data.Rows.Count; ++i)
                    {
                        IRow row = sheet.CreateRow(count);
                        for (j = 0; j < data.Columns.Count; ++j)
                        {
                            row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                        }
                        ++count;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message);
                    return -1;
                }
            }

            FileStream outFs = new FileStream(fileName, FileMode.Open, FileAccess.Write);
            workbook.Write(outFs);
            outFs.Close();
            return count;
        }
    }
}
