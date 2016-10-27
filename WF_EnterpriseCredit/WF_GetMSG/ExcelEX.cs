using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.SS.UserModel;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

namespace WF_GetMSG
{
    public class ExcelEX
    {
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
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
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
