﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Text;
using System.Globalization;

namespace SmartMIS.Report

{
    public partial class PCRVI3SummaryReport : System.Web.UI.Page
    {
        #region classes
        smartMISWebService myWebService = new smartMISWebService();
        myConnection myConnection = new myConnection();

        #endregion

        #region globle variable
        public string queryString, rType, rWCID, rChoice, rToDate, rFromDate, rToMonth, rToYear, rFromYear, option, workcentername, wcnamequery, wcIDQuery, machinenamequery;
        string faultname, recipeCode;
        public int tyrecount = 0;
        int pid = -1;

        Excel.Application xlApp;
        Excel.Workbook xlWorkBook;
        Excel.Worksheet xlWorkSheet;
        object misValue = System.Reflection.Missing.Value;

        string fileName = DateTime.Now.ToString("yyyy_MM_ddThh_mm_ss_") + "SecondPCRVisualInspectionReport.xlsx";
        string filepath; 

        #endregion

        #region System Defined Function

        public PCRVI3SummaryReport()
        {
            filepath = myWebService.getExcelPath();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(tuoReportMasterFromDateTextBox.Text))  // If Textbox already null, then show current Date
                {
                    tuoReportMasterFromDateTextBox.Text = DateTime.Now.ToString("dd-MM-yyyy");
                    string showToDate = "";
                    int month = DateTime.Now.Month, year = DateTime.Now.Year;

                    if (DateTime.Now.Month == 12 && DateTime.Now.Day == 31)
                        showToDate = "01-01" + "-" + (DateTime.Now.Year + 1);
                    else if (DateTime.Now.Day == 31 && (DateTime.Now.Month == 1 || DateTime.Now.Month == 3 || DateTime.Now.Month == 5 || DateTime.Now.Month == 7 || DateTime.Now.Month == 8 || DateTime.Now.Month == 10))
                        showToDate = "01-" + checkDigit((DateTime.Now.Month + 1)) + "-" + DateTime.Now.Year.ToString();
                    else if (DateTime.Now.Day == 30 && (DateTime.Now.Month == 4 || DateTime.Now.Month == 6 || DateTime.Now.Month == 9 || DateTime.Now.Month == 11))
                        showToDate = "01-" + (checkDigit(DateTime.Now.Month + 1)) + "-" + DateTime.Now.Year.ToString();
                    else if (DateTime.Now.Month == 2)
                        showToDate = "01-" + checkDigit((DateTime.Now.Month + 1)) + "-" + DateTime.Now.Year.ToString();
                    else
                        showToDate = checkDigit((DateTime.Now.Day + 1)) + "-" + checkDigit(DateTime.Now.Month) + "-" + DateTime.Now.Year;

                    tuoReportMasterToDateTextBox.Text = showToDate.ToString();

                }

                if (Session["userID"].ToString().Trim() == "")
                {
                    Response.Redirect("/SmartMIS/Default.aspx", true);
                }
                PCRVI3SummaryReportPanel.Visible = false;
                VI3FaultWisePanel.Visible = false;

                showDownload.Text = "";
            }
            catch (Exception ex)
            {

            }
        }

        protected string checkDigit(int digit)
        {
            string str = "";
            if (digit.ToString().Length == 1)
                str = "0" + digit;
            else
                str = digit.ToString();
            return str;
        }
        protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            try
            {

                if (e.Row.RowType == DataControlRowType.DataRow)
                {



                    if (((GridView)sender).ID == "VIRecipeWiseGridView")
                    {

                        Label recipeCodeLabel = ((Label)e.Row.FindControl("VISizeWiseTyreTypeLabel"));
                        GridView childGridView = ((GridView)e.Row.FindControl("VIRecipeWiseChildGridView"));

                        fillChildInnerGridView("3401", "gg", childGridView, recipeCodeLabel.Text.Trim(), rToDate, rFromDate, "1");
                    }

                    else if (((GridView)sender).ID == "SizeWiseRegionGridView")
                    {

                        Label recipeCodeLabel = ((Label)e.Row.FindControl("VISizeFaultWiseTyreTypeLabel"));
                        GridView childGridView = ((GridView)e.Row.FindControl("VIRecipeFaultWiseChildGridView"));

                        fillChildInnerGridView(faultname, "gg", childGridView, recipeCodeLabel.Text.Trim(), rToDate, rFromDate, "1");
                    }




                    else if (((GridView)sender).ID == "VIFaultWiseGridView")
                    {

                        Label recipeCodeLabel = ((Label)e.Row.FindControl("VIFaultWiseTyreTypeLabel"));
                        GridView childGridView = ((GridView)e.Row.FindControl("VIFaultWiseChildGridView"));

                        recipeCode = recipeCodeLabel.Text;

                        fillChildInnerGridView(faultname, "gg", childGridView, recipeCodeLabel.Text.Trim(), rToDate, rFromDate, "1");
                    }

                    else if (((GridView)sender).ID == "VIFaultWiseChildGridView")
                    {

                        Label FaltAreaNameLabel = ((Label)e.Row.FindControl("VIfaltAreaCheckedLabel"));
                        GridView childGridView = ((GridView)e.Row.FindControl("VIFaultNameChildGridView"));


                        fillChildInnerGridView(faultname, FaltAreaNameLabel.Text.Trim(), childGridView, recipeCode, rToDate, rFromDate, "1");
                    }

                    if (((GridView)sender).ID == "VIRecipeFaultWiseChildGridView")
                    {
                        e.Row.Attributes.Add("onmouseover", "this.originalcolor=this.style.backgroundColor;" + " this.style.backgroundColor='#9BC8F0';");  //#FDCB0A
                        e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=this.originalcolor;");
                    }

                }

            }

            catch (Exception exp)
            {
                myWebService.writeLogs(exp.Message, System.Reflection.MethodBase.GetCurrentMethod().Name, Path.GetFileName(Request.Url.AbsolutePath));
            }
        }

        #endregion

        #region User Defined Function

        private void fillRecipeWiseGridView(string query)
        {

            //Description   : Function for filling performanceReportSizeWiseGridView TyreType
            //Author        : Brajesh kumar
            //Date Created  : 22 June 2012
            //Date Updated  : 22 June 2012
            //Revision No.  : 01
            try
            {
                
                if ( FaultAreaDropDownList.SelectedValue == "Select")
                {
                    SizeWiseRegionGridView.DataSource = myWebService.fillGridView(query, ConnectionOption.SQL);
                    SizeWiseRegionGridView.DataBind();
                }
                else if (FaultAreaDropDownList.SelectedValue != "Select")
                {
                    VIFaultWiseGridView.DataSource = myWebService.fillGridView(query, ConnectionOption.SQL);
                    VIFaultWiseGridView.DataBind();
                }
            }
            catch (Exception exp)
            {
                myWebService.writeLogs(exp.Message, System.Reflection.MethodBase.GetCurrentMethod().Name, Path.GetFileName(Request.Url.AbsolutePath));
            }
        }

        private void fillChildGridView(GridView childgridview, string query)
        {
            childgridview.DataSource = myWebService.fillGridView(query, ConnectionOption.SQL);
            childgridview.DataBind();
        }

        private void fillChildInnerGridView(string faltType, string faltArea, GridView childGridView, string recipecode, String toDate, String fromDate, String option)
        {
            //Description   : Function for filling ChildGridView
            //Author        : Brajesh kumar
            //Date Created  : 23 June 2012
            //Date Updated  : 23 June 2012
            //Revision No.  : 01
            //Description   :
            try
            {


               

                 if (childGridView.ID == "VIRecipeFaultWiseChildGridView")
                {
                    if (option == "1")
                    {
                        childGridView.DataSource = fillGridView("sp_Testing_PCRVIReportRecipe3FaultWise_Nos", faltType, recipecode, toDate, fromDate, ConnectionOption.SQL);
                        childGridView.DataBind();
                    }
                }


                else if (childGridView.ID == "VIFaultWiseChildGridView")
                {
                    if (option == "1")
                    {
                        childGridView.DataSource = fillFaltArea(faltType, faltArea, recipecode, toDate, fromDate, ConnectionOption.SQL);
                        childGridView.DataBind();
                    }
                }
                else if (childGridView.ID == "VIFaultNameChildGridView")
                {
                    if (option == "1")
                    {
                        childGridView.DataSource = fillFaltName(faltType, faltArea, recipecode, toDate, fromDate, ConnectionOption.SQL);
                        childGridView.DataBind();
                    }
                }




            }

            catch (Exception exp)
            {
                myWebService.writeLogs(exp.Message, System.Reflection.MethodBase.GetCurrentMethod().Name, Path.GetFileName(Request.Url.AbsolutePath));
            }
        }

        private DataTable fillFaltArea(string faltType, string faltArea, string recipeCode, string rTodate, string rFromDate, ConnectionOption option)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("defectAreaID", typeof(int));
            dt.Columns.Add("FaultAreaName", typeof(string));

        string defectArea = FaultAreaDropDownList.SelectedValue.ToString();

        int defectID = 0;

        switch (defectArea)
        {
            case "Tread":
                defectID = 1;
                break;
            case "SideWall":
                defectID = 2;
                break;
            case "Bead":
                defectID = 3;
                break;
            case "Carcass":
                defectID = 4;
                break;
            case "Others":
                defectID = 5;
                break;

        }

             //myConnection.open(ConnectionOption.SQL);
             //myConnection.comm = myConnection.conn.CreateCommand();
        if (FaultAreaDropDownList.SelectedValue == "All")
        {
            dt.Rows.Add(1, "Tread");
            dt.Rows.Add(2, "SideWall");
            dt.Rows.Add(3, "Bead");
            dt.Rows.Add(4, "Carcass");
            dt.Rows.Add(5, "Others");
        }

          // myConnection.comm.CommandText = "select distinct defectLocationName from vVisualInspectionPCR where statusName='" + faltType + "' and description='" + recipeCode + "'  and dtandTime>='" + rTodate + "' and dtandTime<='" + rFromDate + "'";
        else
        {
            dt.Rows.Add(defectID, defectArea);

        }

            // myConnection.comm.CommandText = "select distinct defectLocationName from vVisualInspectionPCR where statusName='" + faltType + "' and description='" + recipeCode + "'  and defectLocationName='" + FaultAreaDropDownList.SelectedValue + "' and  dtandTime>='" + rTodate + "' and dtandTime<='" + rFromDate + "'";


            //System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(myConnection.comm.CommandText, myConnection.conn);
            //DataSet ds = new DataSet();
            //da.Fill(ds, "dt");


            return dt;


        }

        private DataTable fillFaltName(string faltType, string faltArea, string recipeCode, string rTodate, string rFromDate, ConnectionOption option)
        {
            ArrayList defectCode = new ArrayList();

            DataTable dt = new DataTable();
            dt.Columns.Add("faultName", typeof(string));
            dt.Columns.Add("quantity", typeof(string));

            myConnection.open(ConnectionOption.SQL);
            myConnection.comm = myConnection.conn.CreateCommand();

            myConnection.comm.CommandText = "select distinct defectName as faultName from vVisualInspectionPCR where statusName='Hold' and description='" + recipeCode + "' and defectLocationName='" + faltArea + "'  and dtandTime>='" + rTodate + "' and dtandTime<='" + rFromDate + "'";

            myConnection.reader = myConnection.comm.ExecuteReader();
            while (myConnection.reader.Read())
            {
                defectCode.Add(myConnection.reader[0].ToString());
            }

            myConnection.reader.Close();
            myConnection.comm.Dispose();


            //System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(myConnection.comm.CommandText, myConnection.conn);
            //DataSet ds = new DataSet();
            //da.Fill(ds, "dt");
            for (int i = 0; i < defectCode.Count; i++)
            {


                myConnection.open(ConnectionOption.SQL);
                myConnection.comm = myConnection.conn.CreateCommand();

                myConnection.comm.CommandText = "select count(*)  from vVisualInspectionPCR where statusName='Hold' and description='" + recipeCode + "' and defectLocationName='" + faltArea + "'  and defectName='" + defectCode[i].ToString() + "' and dtandTime>='" + rTodate + "' and dtandTime<='" + rFromDate + "'";

                myConnection.reader = myConnection.comm.ExecuteReader();

                while (myConnection.reader.Read())
                {

                    dt.Rows.Add(defectCode[i], myConnection.reader[0].ToString());

                    tyrecount = tyrecount + Convert.ToInt32(myConnection.reader[0]);
                }

                myConnection.reader.Close();
                myConnection.comm.Dispose();

            }


            return dt;


        }

        public int totalcheckedcount = 0, totalBuff = 0, TotalRepair = 0;

        public int totalcheckedcount2 = 0, reworkcount = 0, treadcount = 0, sidewallcount = 0, beadcount = 0, carcasscount = 0, otherscount = 0;

        public DataTable fillGridView(string procedureName, string faltType, string recipeCode, string rToDate, string rFromDate, ConnectionOption option)
        {
            DataTable flag = new DataTable();

            //Description   : Function for returning Datatable on the basis of SQL Query
            //Author        : Brajesh kumar
            //Date Created  : 04 April 2011
            //Date Updated  : 04 April 2011
            //Revision No.  : 01

            if (option == ConnectionOption.SQL)
            {
                try
                {
                    myConnection.open(ConnectionOption.SQL);
                    myConnection.comm = myConnection.conn.CreateCommand();

                    myConnection.comm.CommandText = procedureName;
                    myConnection.comm.CommandType = CommandType.StoredProcedure;

                    System.Data.SqlClient.SqlParameter tyreTypeParameter = new System.Data.SqlClient.SqlParameter("@recipecode", System.Data.SqlDbType.VarChar);
                    tyreTypeParameter.Direction = System.Data.ParameterDirection.Input;
                    tyreTypeParameter.Value = recipeCode;

                    System.Data.SqlClient.SqlParameter toDateParameter = new System.Data.SqlClient.SqlParameter("@toDate", System.Data.SqlDbType.VarChar);
                    toDateParameter.Direction = System.Data.ParameterDirection.Input;
                    toDateParameter.Value = rToDate;

                    System.Data.SqlClient.SqlParameter fromDateParameter = new System.Data.SqlClient.SqlParameter("@fromDate", System.Data.SqlDbType.VarChar);
                    fromDateParameter.Direction = System.Data.ParameterDirection.Input;
                    fromDateParameter.Value = rFromDate;

                    //System.Data.SqlClient.SqlParameter reportType = new System.Data.SqlClient.SqlParameter("@reportType", System.Data.SqlDbType.VarChar);
                    //reportType.Direction = System.Data.ParameterDirection.Input;
                    //reportType.Value = type;

                    myConnection.comm.Parameters.Add(tyreTypeParameter);
                    myConnection.comm.Parameters.Add(toDateParameter);
                    myConnection.comm.Parameters.Add(fromDateParameter);
                    //myConnection.comm.Parameters.Add(reportType);


                    myConnection.reader = myConnection.comm.ExecuteReader(CommandBehavior.CloseConnection);
                    flag.Load(myConnection.reader);

                    myConnection.reader.Close();
                    myConnection.comm.Dispose();
                    myConnection.close(ConnectionOption.SQL);

                   
                            totalcheckedcount2 += Convert.ToInt32(flag.Rows[0][0]);
                            treadcount += Convert.ToInt32(flag.Rows[0][1]);
                            sidewallcount += Convert.ToInt32(flag.Rows[0][2]);
                            beadcount += Convert.ToInt32(flag.Rows[0][3]);
                            carcasscount += Convert.ToInt32(flag.Rows[0][4]);
                            otherscount += Convert.ToInt32(flag.Rows[0][5]);
                        

                    
                }
                catch (Exception exp)
                {
                    myWebService.writeLogs(exp.Message, System.Reflection.MethodBase.GetCurrentMethod().Name, Path.GetFileName(Request.Url.AbsolutePath));
                }
                finally
                {

                    myConnection.reader.Close();
                    myConnection.comm.Dispose();
                    myConnection.close(ConnectionOption.SQL);
                }
            }

            return flag;
        }

       
        public string formatfromDate(String date)
        {
            string flag = "";

            string day, month, year;
            if (date != null)
            {
                string[] tempDate = date.Split(new char[] { '-' });
                try
                {
                    day = tempDate[1].ToString().Trim();
                    month = tempDate[0].ToString().Trim();
                    year = tempDate[2].ToString().Trim();
                    // DateTime tempDate1 = Convert.ToDateTime(date);
                    if (Convert.ToInt32(month) == 12 && Convert.ToInt32(day) == 31)
                    {
                        flag = "01-01-" + (Convert.ToInt32(year) + 1).ToString() + " 07" + ":" + "00" + ":" + "00";
                    }
                    else if (DateTime.DaysInMonth(Convert.ToInt32(year), Convert.ToInt32(month)) != Convert.ToInt32(day))
                    {
                        flag = month + "-" + (Convert.ToInt32(day) + 1).ToString() + "-" + year + " " + "07" + ":" + "00" + ":" + "00";
                    }
                    else
                    {
                        flag = (Convert.ToInt32(month) + 1).ToString() + "-" + "01" + "-" + year + " " + "07" + ":" + "00" + ":" + "00";
                    }
                }
                catch (Exception exp)
                {
                    myWebService.writeLogs(exp.Message, System.Reflection.MethodBase.GetCurrentMethod().Name, Path.GetFileName(Request.Url.AbsolutePath));
                }
            }
            return flag;
        }
        public string formatDate(String date)
        {
            string flag = "";

            string day, month, year;
            if (date != null)
            {
                string[] tempDate = date.Split(new char[] { '-' });
                try
                {
                    day = tempDate[1].ToString().Trim();
                    month = tempDate[0].ToString().Trim();
                    year = tempDate[2].ToString().Trim();
                    flag = day + "-" + month + "-" + year + " " + "07" + ":" + "00" + ":" + "00";

                }
                catch (Exception exp)
                {
                    myWebService.writeLogs(exp.Message, System.Reflection.MethodBase.GetCurrentMethod().Name, Path.GetFileName(Request.Url.AbsolutePath));
                }
            }
            return flag;
        }
        #endregion

        protected void ViewButton_Click(object sender, EventArgs e)
        {

            rToDate = formatDate(tuoReportMasterFromDateTextBox.Text);
            rFromDate = formatDate(tuoReportMasterToDateTextBox.Text);
            rFromDate = formatfromDate(rFromDate.Replace(" 07:00:00", ""));

            if (FaultAreaDropDownList.SelectedValue == "Select")
            {

                PCRVI3SummaryReportPanel.Visible = true;
                VI3FaultWisePanel.Visible = false;
            }
            else if (FaultAreaDropDownList.SelectedValue != "Select")
            {
                VI3FaultWisePanel.Visible = true;
                PCRVI3SummaryReportPanel.Visible = false;
            }

            fillRecipeWiseGridView("select distinct description as curingRecipeName from vVisualInspectionPCR where status=31 and  dtandTime>'" + rToDate + "' and dtandTime<='" + rFromDate + "'");

        }

        protected void FaultTypeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (((DropDownList)sender).ID == "FaultTypeDropDownList")
                FaultAreaDropDownList.SelectedIndex = 0;


          
            if ( FaultAreaDropDownList.SelectedValue == "Select")
            {
                
                PCRVI3SummaryReportPanel.Visible = true;
                VI3FaultWisePanel.Visible = false;
            }
            else if (FaultAreaDropDownList.SelectedValue != "Select")
            {
                VI3FaultWisePanel.Visible = true;
                PCRVI3SummaryReportPanel.Visible = false;
            }
          //  Label11.Text =FaultAreaDropDownList.SelectedValue;

            rToDate = formatDate(tuoReportMasterFromDateTextBox.Text);
            rFromDate = formatDate(tuoReportMasterToDateTextBox.Text);
            rFromDate = formatfromDate(rFromDate.Replace(" 07:00:00", ""));
            fillRecipeWiseGridView("select distinct description as curingRecipeName from  vVisualInspectionPCR where dtandTime>'" + rToDate + "' and dtandTime<='" + rFromDate + "'");


        }
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowThreadProcessId(HandleRef handle, out int processId);
        protected void expToExcel_Click(object sender, EventArgs e)
        {
                DataTable gridviewdt = new DataTable();
                DataTable dt = new DataTable();
                DataTable curdt = new DataTable();
                DataTable builddt = new DataTable();
                StringBuilder sb = new StringBuilder();

                DateTime rToDate = Convert.ToDateTime(tuoReportMasterFromDateTextBox.Text); ;
                var toDate = rToDate.ToString("yyyy-MM-dd");
                DateTime rFromDate = Convert.ToDateTime(tuoReportMasterToDateTextBox.Text);
                var fromDate = rFromDate.ToString("yyyy-MM-dd");
                //rToDate = formatDate(tuoReportMasterFromDateTextBox.Text);
                //rFromDate = formatDate(tuoReportMasterToDateTextBox.Text);
                //TimeSpan ts = DateTime.Parse(rFromDate) - DateTime.Parse(rToDate);

                //TimeSpan ts = (fromDate) - (toDate);
                //int result = (int)ts.TotalDays;

                //if ((int)ts.TotalDays < 0)
                //{
                //    ShowWarning.Visible = true;
                //    ShowWarning.Text = "<table style=\"padding:4px;\"><tr><td width=20%><img src='../images/exclamation.png' height=\"30\" /></td><td width=80%><strong> <font color=#9F6000>From Date cannot be greater than To Date!!!</font></strong></td></tr></table>";
                //}
                //else if ((int)ts.TotalDays > 300)
                //{
                //    ShowWarning.Visible = true;
                //    ShowWarning.Text = "<table style=\"padding:4px;\"><tr><td width=20%><img src='../images/exclamation.png' height=\"30\" /></td><td width=80%><strong> <font color=#9F6000>You cannot select data of more than 300 days!!!</font></strong></td></tr></table>";
                //}
                //else
                {
                    //rFromDate = formatfromDate(rFromDate.Replace(" 07:00:00", ""));

                    myConnection.open(ConnectionOption.SQL);
                    myConnection.comm = myConnection.conn.CreateCommand();
                    myConnection.comm.CommandText = "select wcname, shift=(CASE WHEN convert(char(8), dtandTime, 108) >= '07:00:00 AM' AND convert(char(8), dtandTime, 108) <= '14:59:59.999' THEN 'A' WHEN convert(char(8), dtandTime, 108) >= '15:00:00.000' AND convert(char(8), dtandTime, 108) <= '22:59:59.999' THEN 'B' WHEN ((convert(char(8), dtandTime, 108) >= '23:00:00.000' AND convert(char(8), dtandTime, 108) <= '23:59:59.999') or (convert(char(8), dtandTime, 108) >= '00:00:01.000' AND convert(char(8), dtandTime, 108) <= '06:59:59.999')) THEN 'C' END), description, convert(char(10), dtandTime, 110) AS getdate, convert(char(8), dtandTime, 108) AS gettime, firstName + ' ' + lastName As builderName, gtbarCode, defectName, defectLocationName,statusname,curingRecipeName from vVisualInspectionPCR where status in('31','32','33') AND dtandTime>'" + toDate + "' and dtandTime<='" + fromDate + "'";
                    myConnection.reader = myConnection.comm.ExecuteReader(CommandBehavior.CloseConnection);
                    dt.Load(myConnection.reader);

                    myConnection.conn.Close();
                    myConnection.comm.Dispose();
                    myConnection.reader.Close();
                    if (dt.Rows.Count > 0)
                    {
                        sb.Append("<table border=1><tr style=\"background-color:#FFFF00;\"><th>S. No.</th><th>Inspection PressNo</th><th>Shift</th><th>TyreSize</th><th>TBM WCName</th><th>TBM Date</th><th>TBM Time</th><th>TBM Builder Name</th><th>Press Date</th><th>Press Time</th><th>Press No.</th><th>Cavity</th><th>Mould No.</th><th>Inspection Date</th><th>Inspection Time</th><th>Inspector Name</th><th>Barcode</th><th>Defect Location</th><th>Defect</th><th>Status</th><th>Remark</th><th>Responsibility</th></tr>");

                        //DateTime newrDate = DateTime.Parse(rToDate);

                        SqlConnection con = new SqlConnection();
                        con.ConnectionString = ConfigurationManager.ConnectionStrings["mySQLConnection"].ToString();
                        con.Open();
                        SqlCommand cmd = new SqlCommand("SELECT wcName, mouldNo, gtbarCode, convert(char(10), dtandTime, 110) AS getdate, convert(char(8), dtandTime, 108) AS gettime FROM vCuringpcr WHERE dtandTime>'" + toDate + "' and dtandTime<='" + fromDate + "'", con);


                        cmd.CommandTimeout = 0;
                        var dread = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        curdt.Load(dread);

                        con.Close();
                        cmd.Dispose();
                        dread.Close();

                        SqlConnection bcon = new SqlConnection();
                        bcon.ConnectionString = ConfigurationManager.ConnectionStrings["mySQLConnection"].ToString();
                        bcon.Open();

                        SqlCommand bcmd = new SqlCommand("SELECT wcName, gtbarCode, CAST(dtandTime AS DATE) AS getdate, convert(char(8), dtandTime, 108) AS gettime, firstname +' '+ lastname as Inspectorname FROM vTbmPCR WHERE dtandTime >'" + toDate + "' and dtandTime <='" + fromDate + "'", bcon);
                        //SqlCommand bcmd = new SqlCommand("SELECT wcName, gtbarCode, CAST(dtandTime AS DATE) AS getdate, convert(char(8), dtandTime, 108) AS gettime, firstname +' '+ lastname as Inspectorname FROM vTbmPCR  ", bcon);


                        bcmd.CommandTimeout = 0;
                        var bdread = bcmd.ExecuteReader(CommandBehavior.CloseConnection);
                        builddt.Load(bdread);

                        bcon.Close();
                        bcmd.Dispose();
                        bdread.Close();
                        var query = from v in dt.AsEnumerable()
                                    join c in curdt.AsEnumerable() on v.Field<string>("gtbarCode") equals c.Field<string>("gtbarCode")
                                    join b in builddt.AsEnumerable() on v.Field<string>("gtbarCode") equals b.Field<string>("gtbarCode")
                                    select new { v, c, b };


                        int i = 0;
                        foreach (var x in query)
                        {
                            i++;
                            sb.Append("<tr><td>" + (i) + "</td><td>" + x.v[0].ToString() + "</td><td>" + x.v[1].ToString()
                                + "</td><td>" + x.v[2].ToString() + "</td><td>" + x.b[0].ToString() + "</td><td>" + x.b[2].ToString() + "</td><td>" + x.b[3].ToString() + "</td><td>" + x.b[4].ToString() + "</td> <td>" + x.c[3].ToString() + "</td><td>" + x.c[4].ToString() + "</td><td>"
                                + x.c[0].ToString() + "</td><td></td><td>" + x.c[1].ToString() + "</td><td>" +
                                x.v[3].ToString() + "</td><td>" + x.v[4].ToString() + "</td><td>" + x.v[5].ToString()
                                + "</td><td>" + x.v[6].ToString() + "</td><td>" + x.v[8].ToString() +
                                "</td><td>" + x.v[7].ToString() + "</td><td>" + x.v[9].ToString() + "</td><td></td><td></td></tr>");
                        }

                        sb.Append("</table>");
                        ExcelLabel.Text = sb.ToString();
                        ExcelPanel.Visible = true;
                        Response.Clear();
                        Response.Buffer = true;
                        Response.ClearHeaders();
                        Response.ClearContent();
                        string filename = "ThirdPCRVisualInspectionReport_" + DateTime.Now.ToString("yyyy_MM_ddThh_mm_ss_") + ".xls";
                        Response.AddHeader("content-disposition", "attachment;filename=" + filename);
                     
                        Response.Charset = "";
                        Response.ContentType = "application/vnd.ms-excel";
                        System.IO.StringWriter stringWrite = new System.IO.StringWriter();
                        System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);


                        ExcelPanel.RenderControl(htmlWrite);

                        Response.Write(stringWrite.ToString());
                        //Response.Write(ExcelLabel);
                        Response.Flush();
                        //HttpContext.Current.ApplicationInstance.CompleteRequest();
                        Response.End();
                        ExcelPanel.Visible = false;


                    }
                }

        }
        //protected void expToExcel_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        DataTable dt = new DataTable();
        //        DataTable curdt = new DataTable();
        //        rToDate = formatDate(tuoReportMasterFromDateTextBox.Text);
        //        rFromDate = formatDate(tuoReportMasterToDateTextBox.Text);
        //        rFromDate = formatfromDate(rFromDate.Replace(" 07:00:00", ""));

        //        xlApp = new Excel.ApplicationClass();
        //        xlWorkBook = xlApp.Workbooks.Add(misValue);
        //        xlWorkBook.CheckCompatibility = false;
        //        xlWorkBook.DoNotPromptForConvert = true;

        //        //Get PID
        //        HandleRef hwnd = new HandleRef(xlApp, (IntPtr)xlApp.Hwnd);
        //        GetWindowThreadProcessId(hwnd, out pid);

        //        xlApp.Visible = true; // ensure that the excel app is visible.
        //        xlWorkSheet = (Excel.Worksheet)xlApp.ActiveSheet; // Get the current active worksheet.
        //        Microsoft.Office.Interop.Excel.Worksheet worksheet2 = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2); //Get more work sheet if neccessary

        //        xlWorkSheet.get_Range("B1", "E1").Merge(misValue); // Heading
        //        xlWorkSheet.Cells[1, 2] = "Third PCR Visual Inspection Report";
        //        xlWorkSheet.get_Range("B1", "E1").Font.Size = 16;
        //        xlWorkSheet.Cells[1, 6] = DateTime.Now.ToString();
        //        ((Excel.Range)xlWorkSheet.Cells[1, 6]).EntireColumn.ColumnWidth = "20";
        //        ((Excel.Range)xlWorkSheet.Cells[3, 2]).EntireColumn.ColumnWidth = "20";
        //        ((Excel.Range)xlWorkSheet.Cells[3, 3]).EntireColumn.ColumnWidth = "20";
        //        ((Excel.Range)xlWorkSheet.Cells[3, 8]).EntireColumn.ColumnWidth = "20";
        //        ((Excel.Range)xlWorkSheet.Cells[3, 9]).EntireColumn.ColumnWidth = "20";
        //        ((Excel.Range)xlWorkSheet.Cells[3, 10]).EntireColumn.ColumnWidth = "20";
        //        ((Excel.Range)xlWorkSheet.Cells[3, 11]).EntireColumn.ColumnWidth = "20";
        //        ((Excel.Range)xlWorkSheet.Cells[3, 12]).EntireColumn.ColumnWidth = "20";
        //        ((Excel.Range)xlWorkSheet.Cells[3, 15]).EntireColumn.ColumnWidth = "20";
        //        xlWorkSheet.get_Range("B1", "E1").Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

        //        xlWorkSheet.get_Range("A2", "B2").Merge(misValue);
        //        xlWorkSheet.get_Range("C2", "D2").Merge(misValue);
        //        xlWorkSheet.get_Range("C3", "C4").Cells.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
        //        xlWorkSheet.get_Range("A3", "A4").Cells.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
        //        xlWorkSheet.get_Range("B3", "B4").Cells.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
        //        xlWorkSheet.get_Range("A3", "H3").Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

        //        xlWorkSheet.Cells[2, 1] = "From : " + formattoDate(rToDate);
        //        xlWorkSheet.Cells[2, 3] = "To : " + formattoDate(rFromDate);
        //        xlWorkSheet.get_Range("A2", "D2").Font.Bold = true;

        //        xlWorkSheet.Cells[3, 1] = "S. No.";
        //        xlWorkSheet.Cells[3, 2] = "Shift";
        //        xlWorkSheet.Cells[3, 3] = "TyreSize";
        //        xlWorkSheet.Cells[3, 4] = "Press No.";
        //        xlWorkSheet.Cells[3, 5] = "Cavity";
        //        xlWorkSheet.Cells[3, 6] = "Mould No.";
        //        xlWorkSheet.Cells[3, 7] = "Side";
        //        xlWorkSheet.Cells[3, 8] = "Building Date";
        //        xlWorkSheet.Cells[3, 9] = "Building Time";
        //        xlWorkSheet.Cells[3, 10] = "Builder Name";
        //        xlWorkSheet.Cells[3, 11] = "Barcode";
        //        xlWorkSheet.Cells[3, 12] = "Defect";
        //        xlWorkSheet.Cells[3, 13] = "Disposal";
        //        xlWorkSheet.Cells[3, 14] = "Remark";
        //        xlWorkSheet.Cells[3, 15] = "Responsibility";
        //        xlWorkSheet.get_Range("A3", "O3").Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Yellow);
        //        xlWorkSheet.get_Range("A3", "O3").Font.Bold = true;

        //        string query = "select description, wcName, CAST(dtandTime AS DATE) AS getdate, convert(char(8), dtandTime, 108) AS gettime, firstName + ' ' + lastName As builderName, gtbarcode, defectName, shift=(CASE WHEN convert(char(8), dtandTime, 108) >= '07:00:00 AM' AND convert(char(8), dtandTime, 108) <= '14:59:59.999' THEN 'A' WHEN convert(char(8), dtandTime, 108) >= '15:00:00.000' AND convert(char(8), dtandTime, 108) <= '22:59:59.999' THEN 'B' WHEN ((convert(char(8), dtandTime, 108) >= '23:00:00.000' AND convert(char(8), dtandTime, 108) <= '23:59:59.999') or (convert(char(8), dtandTime, 108) >= '00:00:01.000' AND convert(char(8), dtandTime, 108) <= '06:59:59.999')) THEN 'C' END) from vVisualInspectionPCR where status=31 and dtandTime>'" + rToDate + "' and dtandTime<='" + rFromDate + "'";

        //        myConnection.open(ConnectionOption.SQL);
        //        myConnection.comm = myConnection.conn.CreateCommand();
        //        myConnection.comm.CommandText = query;

        //        myConnection.reader = myConnection.comm.ExecuteReader();
        //        dt.Load(myConnection.reader);
        //        myConnection.reader.Close();
        //        myConnection.comm.Dispose();
        //        myConnection.conn.Close();

        //        if (dt.Rows.Count != 0)
        //        {
        //            string InQuery = "(";
        //            for (int i = 0; i < dt.Rows.Count; i++)
        //            {
        //                InQuery += "'" + dt.Select()[i][5].ToString() + "',";
        //            }
        //            InQuery = InQuery.TrimEnd(',');
        //            InQuery += ")";

        //            SqlConnection con = new SqlConnection();
        //            con.ConnectionString = ConfigurationManager.ConnectionStrings["mySQLConnection"].ToString();
        //            con.Open();

        //            SqlCommand cmd = new SqlCommand("SELECT wcName, mouldNo, gtbarCode FROM vCuringpcr WHERE gtbarCode IN " + InQuery.ToString(), con);
        //            var dread = cmd.ExecuteReader(CommandBehavior.CloseConnection);
        //            curdt.Load(dread);

        //            con.Close();
        //            cmd.Dispose();
        //            dread.Close();

        //            for (int i = 0; i < dt.Rows.Count; i++)
        //            {
        //                xlWorkSheet.Cells[i + 4, 1] = i + 1;
        //                xlWorkSheet.Cells[i + 4, 2] = dt.Rows[i][7];
        //                xlWorkSheet.Cells[i + 4, 3] = dt.Rows[i][0];
        //                xlWorkSheet.Cells[i + 4, 4] = dt.Rows[i][1];

        //                for (int j = 0; j < curdt.Rows.Count; j++)
        //                {
        //                    try
        //                    {
        //                        xlWorkSheet.Cells[i + 4, 6] = curdt.Select("gtbarCode='" + dt.Rows[i][5].ToString() + "'")[j][1].ToString();
        //                    }
        //                    catch (Exception ex) { }
        //                }

        //                xlWorkSheet.Cells[i + 4, 8] = dt.Rows[i][2];
        //                xlWorkSheet.Cells[i + 4, 9] = dt.Rows[i][3];
        //                xlWorkSheet.Cells[i + 4, 10] = dt.Rows[i][4];
        //                xlWorkSheet.Cells[i + 4, 11] = dt.Rows[i][5];
        //                xlWorkSheet.Cells[i + 4, 12] = dt.Rows[i][6];
        //            }
        //            xlWorkSheet.get_Range("A1", "O" + (dt.Rows.Count + 3)).Font.Bold = true;
        //            xlWorkSheet.get_Range("A1", "O" + (dt.Rows.Count + 3)).Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
        //        }


        //        xlWorkBook.SaveAs(filepath + fileName, 51, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlShared, misValue, misValue, misValue, misValue, misValue);  //For excel 2007 and above
        //        xlWorkBook.Close(true, misValue, misValue);
        //        xlApp.Quit();

        //        showDownload.Text = "<div id=\"backdiv\" style=\"position: fixed;top: 0;right: 0;bottom: 0;left: 0;z-index: 1040;background: rgba(0, 0, 0, 0.7);margin: 0px;padding: 0px;display: block;cursor: auto;\"><div id=\"innerdiv\" align=\"center\" style=\"width:25%;max-width:25%;height:150px;background: rgba(255, 255, 255, 1);background-color:#A9E2F3;position:fixed;z-index: 1050;top:45px;left: 500px;-moz-border-radius: 10px;-webkit-border-radius: 10px;border-radius: 5px;border:5px solid #888888;box-shadow: 0 0 5px 2px rgba( 0, 0, 0, 0.6 );\"><h3>Third PCR Visual Inspection Report</h3><BR><a href=javascript:void() onClick=\"downloadFile('../Excel/" + fileName + "')\">Click Here</a> to download Excel file  <a href=javascript:void(); title=\"Close\" onClick=\"closebox()\" class=\"close\">X</a></div></div>";
        //    }
        //    catch (Exception exp)
        //    {
        //        myWebService.writeLogs(exp.Message, System.Reflection.MethodBase.GetCurrentMethod().Name, Path.GetFileName(Request.Url.AbsolutePath));
        //        KillProcess(pid, "EXCEL");
        //    }
        //}
        private void KillProcess(int pid, string processName)
        {
            // to kill current process of excel
            System.Diagnostics.Process[] AllProcesses = System.Diagnostics.Process.GetProcessesByName(processName);
            foreach (System.Diagnostics.Process process in AllProcesses)
            {
                if (process.Id == pid)
                {
                    process.Kill();
                }
            }
            AllProcesses = null;
        }
    }
}
