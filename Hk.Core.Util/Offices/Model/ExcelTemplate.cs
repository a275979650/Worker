using System;
using System.IO;
using Hk.Core.Util.Helper;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Hk.Core.Util.Offices.Model
{
    public class ExcelTemplate
    {
        private string templatePath;

        private string newFileName;
        private string templdateName;
        private string sheetName;
        public string SheetName
        {
            get { return sheetName; }
            set { sheetName = value; }
        }
        public ExcelTemplate(string templdateName, string newFileName)
        {
            this.sheetName = "sheet1";
            templatePath = WebHelper.RootPath + "/Resource/ExcelTemplate/";
            this.templdateName = string.Format("{0}{1}", templatePath, templdateName);
            this.newFileName = newFileName;
        }
        public void ExportDataToExcel(Action<ISheet> actionMethod)
        {
            using (MemoryStream ms = SetDataToExcel(actionMethod))
            {
                byte[] data = ms.ToArray();

                #region response to the client

                HttpResponse response = WebHelper.HttpContext.Response;
                response.Clear();
                response.ContentType = "application/vnd-excel";//"application/vnd.ms-excel";
                response.Headers.Add("Content-Disposition", string.Format("attachment; filename=" + newFileName));
                response.Body.Write(data,0,data.Length);

                #endregion
            }
        }
        private MemoryStream SetDataToExcel(Action<ISheet> actionMethod)
        {
            //Load template file
            FileStream file = new FileStream(templdateName, FileMode.Open, FileAccess.Read);
            XSSFWorkbook workbook = new XSSFWorkbook(file);
            ISheet sheet = workbook.GetSheet(SheetName);

            if (actionMethod != null) actionMethod(sheet);

            sheet.ForceFormulaRecalculation = true;
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                //ms.Position = 0;
                return ms;
            }
        }
    }
}