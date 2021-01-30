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
using System.Text;
using System.Globalization;

namespace SmartMIS.Report
{
    public partial class VISummaryReport : System.Web.UI.Page
    {
        #region classes
        smartMISWebService myWebService = new smartMISWebService();
        myConnection myConnection = new myConnection();

        #endregion

        #region globle variable
        public string queryString, rType, rWCID, rChoice, rToDate, rFromDate, rToMonth, rToYear, rFromYear, option, workcentername, wcnamequery, wcIDQuery, machinenamequery;   
        string faultname;
        string recipeCode;
        string faultArea;
        string tableName = "vVisualInspectionReportView";


        #endregion

        #region System Defined Function
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
                else
                {
                    if (FaultTypeDropDownList.SelectedValue == "Select" && FaultAreaDropDownList.SelectedValue=="Select")
                    {
                        VIReportRecipeWiseMainPanel.Visible = true;
                        VIFaultWisePanel.Visible = false;
                        SizeWiseRegionPanel.Visible = false;
                    }
                    else if (FaultTypeDropDownList.SelectedValue != "Select" && FaultAreaDropDownList.SelectedValue=="Select")
                    {
                        VIReportRecipeWiseMainPanel.Visible = false;
                        VIFaultWisePanel.Visible = false;
                        SizeWiseRegionPanel.Visible = true;
                    }
                    else if (FaultTypeDropDownList.SelectedValue != "Select" && FaultAreaDropDownList.SelectedValue != "Select")
                    {

                        VIFaultWisePanel.Visible = true;
                        VIReportRecipeWiseMainPanel.Visible = false;

                        SizeWiseRegionPanel.Visible = false;
                    }

                    backDiv.Visible = false;
                    dialogPanel.Visible = false;

                }
            }
            catch (Exception exp)
            {
                myWebService.writeLogs(exp.Message, System.Reflection.MethodBase.GetCurrentMethod().Name, Path.GetFileName(Request.Url.AbsolutePath));
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


                    if (((GridView)sender).ID == "VIRecipeWiseChildGridView")
                    {
                        e.Row.Attributes.Add("onmouseover", "this.originalcolor=this.style.backgroundColor;" + " this.style.backgroundColor='#9BC8F0';");  //#FDCB0A
                        e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=this.originalcolor;");
                    }
                    if (((GridView)sender).ID == "VIRecipeWiseGridView")
                    {

                        Label recipeCodeLabel = ((Label)e.Row.FindControl("VISizeWiseTyreTypeLabel"));
                        GridView childGridView = ((GridView)e.Row.FindControl("VIRecipeWiseChildGridView"));

                        fillChildInnerGridView("3401","gg" ,childGridView, recipeCodeLabel.Text.Trim(), rToDate,rFromDate, "1");
                    }

                    else if (((GridView)sender).ID == "SizeWiseRegionGridView")
                    {

                        Label recipeCodeLabel = ((Label)e.Row.FindControl("VISizeFaultWiseTyreTypeLabel"));
                        GridView childGridView = ((GridView)e.Row.FindControl("VIRecipeFaultWiseChildGridView"));
                        if (FaultTypeDropDownList.SelectedValue == "MAJOR")
                            faultname = "HOLD";
                        else
                            faultname = FaultTypeDropDownList.SelectedValue;


                        fillChildInnerGridView(faultname,"gg", childGridView, recipeCodeLabel.Text.Trim(), rToDate, rFromDate, "1");
                    }

                    else if (((GridView)sender).ID == "VIFaultWiseGridView")
                    {
                              
                        Label recipeCodeLabel = ((Label)e.Row.FindControl("VIFaultWiseTyreTypeLabel"));
                        GridView childGridView = ((GridView)e.Row.FindControl("VIFaultWiseChildGridView"));
                        if (FaultTypeDropDownList.SelectedValue == "MAJOR")
                            faultname = "HOLD";
                        else
                            faultname = FaultTypeDropDownList.SelectedValue;

                          recipeCode = recipeCodeLabel.Text;
                        fillChildInnerGridView(faultname,"gg", childGridView, recipeCodeLabel.Text.Trim(), rToDate, rFromDate, "1");
                    }

                    else if (((GridView)sender).ID == "VIFaultWiseChildGridView")
                    {

                        Label FaltAreaNameLabel = ((Label)e.Row.FindControl("VIfaltAreaCheckedLabel"));
                        GridView childGridView = ((GridView)e.Row.FindControl("VIFaultNameChildGridView"));
                        if (FaultTypeDropDownList.SelectedValue == "MAJOR")
                            faultname = "HOLD";
                        else
                            faultname = FaultTypeDropDownList.SelectedValue;
                        faultArea = FaltAreaNameLabel.Text.Trim();

                        fillChildInnerGridView(faultname, FaltAreaNameLabel.Text.Trim(), childGridView, recipeCode, rToDate, rFromDate, "1");
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
                if (FaultTypeDropDownList.SelectedValue == "Select" && FaultAreaDropDownList.SelectedValue=="Select")
                {
                    VIRecipeWiseGridView.DataSource = myWebService.fillGridView(query, ConnectionOption.SQL);
                    VIRecipeWiseGridView.DataBind();
                }
                else if(FaultTypeDropDownList.SelectedValue!="Select" && FaultAreaDropDownList.SelectedValue=="Select")
                {
                    SizeWiseRegionGridView.DataSource = myWebService.fillGridView(query, ConnectionOption.SQL);
                    SizeWiseRegionGridView.DataBind();
                }
                else if (FaultTypeDropDownList.SelectedValue != "Select" && FaultAreaDropDownList.SelectedValue != "Select")
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
               

                    if (childGridView.ID == "VIRecipeWiseChildGridView")
                    {
                        if (option == "1")
                        {
                            childGridView.DataSource = fillGridView("sp_Testing_PCRVIReportRecipeWise_Nos", faltType, recipecode, toDate, fromDate, ConnectionOption.SQL);
                            childGridView.DataBind();
                        }
                    }

                    else if (childGridView.ID == "VIRecipeFaultWiseChildGridView")
                    {
                        if (option == "1")
                        {
                            childGridView.DataSource = fillGridView("sp_Testing_PCRVIReportRecipeFaultWise_Nos", faltType, recipecode, toDate, fromDate, ConnectionOption.SQL);
                            childGridView.DataBind();
                        }
                    }
                    else if (childGridView.ID == "VIFaultWiseChildGridView")
                    {
                        if (option == "1")
                        {
                            childGridView.DataSource = fillFaltArea(faltType,faltArea, recipecode, toDate, fromDate, ConnectionOption.SQL);
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

        private DataSet fillFaltArea(string faltType, string faltArea,string recipeCode,string rTodate,string rFromDate,ConnectionOption option)
        {
            DataTable dt = new DataTable();
            
            myConnection.open(ConnectionOption.SQL);
                    myConnection.comm = myConnection.conn.CreateCommand();
            if(FaultAreaDropDownList.SelectedValue=="All")

                myConnection.comm.CommandText = "select distinct FaultAreaName from "+tableName+" where defectstatusName='"+faltType+"' and CuringRecipeName='"+recipeCode+"'  and dtandTime>='"+rTodate+"' and dtandTime<='"+rFromDate+"'";
            else

                myConnection.comm.CommandText = "select distinct FaultAreaName from "+tableName+" where defectstatusName='" + faltType + "' and CuringRecipeName='" + recipeCode + "'  and faultAreaName='"+FaultAreaDropDownList.SelectedValue+"' and  dtandTime>='" + rTodate + "' and dtandTime<='" + rFromDate + "'";

                  
                   System.Data.SqlClient.SqlDataAdapter da  = new System.Data.SqlClient.SqlDataAdapter(myConnection.comm.CommandText,myConnection.conn);
                   DataSet ds = new DataSet();
                   da.Fill(ds,"dt");
                     
                  
                  return ds;

 
        }
        private DataSet fillFaltName(string faltType, string faltArea, string recipeCode, string rTodate, string rFromDate, ConnectionOption option)
        {
            DataTable dt = new DataTable();

            myConnection.open(ConnectionOption.SQL);
            myConnection.comm = myConnection.conn.CreateCommand();
           
            myConnection.comm.CommandText = "select distinct name as faultName from "+tableName+" where defectstatusName='" + faltType + "' and CuringRecipeName='" + recipeCode + "' and faultAreaName='" + faltArea + "'  and dtandTime>='" + rTodate + "' and dtandTime<='" + rFromDate + "'";
            
            System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(myConnection.comm.CommandText, myConnection.conn);
            DataSet ds = new DataSet();
            da.Fill(ds, "dt");


            return ds;


        }

        public int totalcheckedcount = 0, okcount = 0, notokcount = 0, majorcount = 0, minorcount = 0;
        public int reworkcount = 0, treadcount = 0, sidewallcount = 0, beadcount = 0, carcasscount = 0, otherscount = 0;
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

                    System.Data.SqlClient.SqlParameter machineNameParameter = new System.Data.SqlClient.SqlParameter("@faultType", System.Data.SqlDbType.VarChar);
                    machineNameParameter.Direction = System.Data.ParameterDirection.Input;
                    machineNameParameter.Value = faltType;

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

                    myConnection.comm.Parameters.Add(machineNameParameter);
                    myConnection.comm.Parameters.Add(tyreTypeParameter);
                    myConnection.comm.Parameters.Add(toDateParameter);
                    myConnection.comm.Parameters.Add(fromDateParameter);
                    //myConnection.comm.Parameters.Add(reportType);


                    myConnection.reader = myConnection.comm.ExecuteReader(CommandBehavior.CloseConnection);
                    flag.Load(myConnection.reader);

                    myConnection.reader.Close();
                    myConnection.comm.Dispose();
                    myConnection.close(ConnectionOption.SQL);

                    myConnection.open(ConnectionOption.SQL);
                    myConnection.comm = myConnection.conn.CreateCommand();

                    myConnection.comm.CommandText = procedureName;
                    myConnection.comm.CommandType = CommandType.StoredProcedure;

                    System.Data.SqlClient.SqlParameter nmachineNameParameter = new System.Data.SqlClient.SqlParameter("@faultType", System.Data.SqlDbType.VarChar);
                    nmachineNameParameter.Direction = System.Data.ParameterDirection.Input;
                    nmachineNameParameter.Value = faltType;

                    System.Data.SqlClient.SqlParameter ntyreTypeParameter = new System.Data.SqlClient.SqlParameter("@recipecode", System.Data.SqlDbType.VarChar);
                    ntyreTypeParameter.Direction = System.Data.ParameterDirection.Input;
                    ntyreTypeParameter.Value = recipeCode;

                    System.Data.SqlClient.SqlParameter ntoDateParameter = new System.Data.SqlClient.SqlParameter("@toDate", System.Data.SqlDbType.VarChar);
                    ntoDateParameter.Direction = System.Data.ParameterDirection.Input;
                    ntoDateParameter.Value = rToDate;

                    System.Data.SqlClient.SqlParameter nfromDateParameter = new System.Data.SqlClient.SqlParameter("@fromDate", System.Data.SqlDbType.VarChar);
                    nfromDateParameter.Direction = System.Data.ParameterDirection.Input;
                    nfromDateParameter.Value = rFromDate;
                    
                    myConnection.comm.Parameters.Add(nmachineNameParameter);
                    myConnection.comm.Parameters.Add(ntyreTypeParameter);
                    myConnection.comm.Parameters.Add(ntoDateParameter);
                    myConnection.comm.Parameters.Add(nfromDateParameter);
                    
                    myConnection.reader = myConnection.comm.ExecuteReader(CommandBehavior.CloseConnection);
                    if (myConnection.reader.HasRows)
                    {
                        myConnection.reader.Read();

                        totalcheckedcount += Convert.ToInt32(myConnection.reader["TotalChecked"].ToString());

                        if (procedureName == "sp_Testing_PCRVIReportRecipeWise_Nos")
                        {
                            okcount += Convert.ToInt32(myConnection.reader["TotalOK"].ToString());
                            notokcount += Convert.ToInt32(myConnection.reader["TotalNotOK"].ToString());
                            majorcount += Convert.ToInt32(myConnection.reader["TotalMazor"].ToString());
                            minorcount += Convert.ToInt32(myConnection.reader["TotalMinor"].ToString()); 
                        }
                        else if (procedureName == "sp_Testing_PCRVIReportRecipeFaultWise_Nos")
                        {
                            reworkcount += Convert.ToInt32(myConnection.reader["TotalRework"].ToString());
                            treadcount += Convert.ToInt32(myConnection.reader["TreadFault"].ToString());
                            sidewallcount += Convert.ToInt32(myConnection.reader["SideWallFault"].ToString());
                            beadcount += Convert.ToInt32(myConnection.reader["Beadfault"].ToString());
                            carcasscount += Convert.ToInt32(myConnection.reader["Carcassfault"].ToString());
                            otherscount += Convert.ToInt32(myConnection.reader["Othersfault"].ToString());
                        }

                    }
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

     



        public string formattoDate(String date)
        {
            string flag = "";
            if (date != null)
            {
                try
                {
                    DateTime tempDate = Convert.ToDateTime(date);
                    flag = tempDate.ToString("MM-dd-yyyy");
                    flag = flag + " " + "07:00:00";
                }
                catch (Exception exp)
                {
                    myWebService.writeLogs(exp.Message, System.Reflection.MethodBase.GetCurrentMethod().Name, Path.GetFileName(Request.Url.AbsolutePath));
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

        public string fillCuringWCName(Object barcode)
        {
            string flag = "None";
            try
            {
                myConnection.open(ConnectionOption.SQL);
                myConnection.comm = myConnection.conn.CreateCommand();

                myConnection.comm.CommandText = "select wcname from vcuringpcr where gtbarcode = '" + barcode.ToString() + "'";


                myConnection.reader = myConnection.comm.ExecuteReader();
                while (myConnection.reader.Read())
                {
                    if (DBNull.Value != (myConnection.reader[0]))
                        flag = myConnection.reader[0].ToString();
                    else
                        flag = "NOne";
                }
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
            return flag;
        }
        protected void expToExcel_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable gridviewdt = new DataTable();
                DataTable dt = new DataTable();
                DataTable curdt = new DataTable();
                DataTable builddt = new DataTable();
                StringBuilder sb = new StringBuilder();
                    
                rToDate = formatDate(tuoReportMasterFromDateTextBox.Text);
                rFromDate = formatDate(tuoReportMasterToDateTextBox.Text);
                TimeSpan ts = DateTime.Parse(rFromDate) - DateTime.Parse(rToDate);
                int result = (int)ts.TotalDays;

                if ((int)ts.TotalDays < 0)
                {
                    ShowWarning.Visible = true;
                    ShowWarning.Text = "<table style=\"padding:4px;\"><tr><td width=20%><img src='../images/exclamation.png' height=\"30\" /></td><td width=80%><strong> <font color=#9F6000>From Date cannot be greater than To Date!!!</font></strong></td></tr></table>";
                }
                else if ((int)ts.TotalDays > 300)
                {
                    ShowWarning.Visible = true;
                    ShowWarning.Text = "<table style=\"padding:4px;\"><tr><td width=20%><img src='../images/exclamation.png' height=\"30\" /></td><td width=80%><strong> <font color=#9F6000>You cannot select data of more than 300 days!!!</font></strong></td></tr></table>";
                }
                else
                {
                    rFromDate = formatfromDate(rFromDate.Replace(" 07:00:00", ""));

                    myConnection.open(ConnectionOption.SQL);
                    myConnection.comm = myConnection.conn.CreateCommand();
                    myConnection.comm.CommandText = "select wcname, shift=(CASE WHEN convert(char(8), dtandTime, 108) >= '07:00:00 AM' AND convert(char(8), dtandTime, 108) <= '14:59:59.999' THEN 'A' WHEN convert(char(8), dtandTime, 108) >= '15:00:00.000' AND convert(char(8), dtandTime, 108) <= '22:59:59.999' THEN 'B' WHEN ((convert(char(8), dtandTime, 108) >= '23:00:00.000' AND convert(char(8), dtandTime, 108) <= '23:59:59.999') or (convert(char(8), dtandTime, 108) >= '00:00:01.000' AND convert(char(8), dtandTime, 108) <= '06:59:59.999')) THEN 'C' END), description, convert(char(10), dtandTime, 110) AS getdate, convert(char(8), dtandTime, 108) AS gettime, firstName + ' ' + lastName As builderName, gtbarCode, defectName, defectLocationName,statusname,curingRecipeName from vVisualInspectionPCR where status<>'1' AND dtandTime>'" + rToDate + "' and dtandTime<='" + rFromDate + "'";
                    myConnection.reader = myConnection.comm.ExecuteReader(CommandBehavior.CloseConnection);
                    dt.Load(myConnection.reader);

                    myConnection.conn.Close();
                    myConnection.comm.Dispose();
                    myConnection.reader.Close();
                    if (dt.Rows.Count > 0)
                    {
                        sb.Append("<table border=1><tr style=\"background-color:#FFFF00;\"><th>S. No.</th><th>Inspection PressNo</th><th>Shift</th><th>TyreSize</th><th>TBM WCName</th><th>TBM Date</th><th>TBM Time</th><th>TBM Builder Name</th><th>Press Date</th><th>Press Time</th><th>Press No.</th><th>Cavity</th><th>Mould No.</th><th>Inspection Date</th><th>Inspection Time</th><th>Inspector Name</th><th>Barcode</th><th>Defect Location</th><th>Defect</th><th>Status</th><th>Remark</th><th>Responsibility</th></tr>");

                        /*string query = "select wcName, description, TBMwcName, convert(char(10), curingdtandTime, 110) AS getdate, convert(char(8), curingdtandTime, 108) AS gettime, curingwcName, mouldNo, CAST(dtandTime AS DATE) AS getdate, convert(char(8), dtandTime, 108) AS gettime, firstName + ' ' + lastName As builderName, gtbarcode, shift=(CASE WHEN convert(char(8), dtandTime, 108) >= '07:00:00 AM' AND convert(char(8), dtandTime, 108) <= '14:59:59.999' THEN 'A' WHEN convert(char(8), dtandTime, 108) >= '15:00:00.000' AND convert(char(8), dtandTime, 108) <= '22:59:59.999' THEN 'B' WHEN ((convert(char(8), dtandTime, 108) >= '23:00:00.000' AND convert(char(8), dtandTime, 108) <= '23:59:59.999') or (convert(char(8), dtandTime, 108) >= '00:00:01.000' AND convert(char(8), dtandTime, 108) <= '06:59:59.999')) THEN 'C' END) from vPCRVisualInspectionExcelReport where dtandTime>'" + rToDate + "' and dtandTime<='" + rFromDate + "' AND status<>'1'";
                        myConnection.open(ConnectionOption.SQL);
                        myConnection.comm = myConnection.conn.CreateCommand();
                        myConnection.comm.CommandText = query;
                        myConnection.comm.CommandTimeout = 0;
                        myConnection.reader = myConnection.comm.ExecuteReader();
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<table border=1><tr style=\"background-color:#FFFF00;\"><th>S. No.</th><th>Inspection PressNo</th><th>Shift</th><th>TyreSize</th><th>Building Machine Name</th><th>Press Date</th><th>Press Time</th><th>Press No.</th><th>Cavity</th><th>Mould No.</th><th>Building Date</th><th>Building Time</th><th>Builder Name</th><th>Barcode</th><th>Defect Location</th><th>Defect</th><th>Remark</th><th>Responsibility</th></tr>");
                        if (myConnection.reader.HasRows)
                        {
                            int i = 1;
                            while (myConnection.reader.Read())
                            {
                                sb.Append("<tr><td>" + i + "</td><td>" + myConnection.reader[0] + "</td><td>" + myConnection.reader[11] + "</td><td>" + myConnection.reader[1] + "</td><td>" + myConnection.reader[2] + "</td><td>" + myConnection.reader[3] + "</td><td>" + myConnection.reader[4] + "</td><td>" + myConnection.reader[5] + "</td><td></td><td>" + myConnection.reader[6] + "</td><td>" + myConnection.reader[7] + "</td><td>" + myConnection.reader[8] + "</td><td>" + myConnection.reader[9] + "</td><td>" + myConnection.reader[10] + "<td></td></tr>");
                                i++;
                            }
                        }
                        sb.Append("</table>");
                        ExcelLabel.Text = sb.ToString();
                        myConnection.reader.Close();
                        myConnection.comm.Dispose();
                        myConnection.conn.Close();*/

                        DateTime newrDate = DateTime.Parse(rToDate);

                        SqlConnection con = new SqlConnection();
                        con.ConnectionString = ConfigurationManager.ConnectionStrings["mySQLConnection"].ToString();
                        con.Open();
                        SqlCommand cmd = new SqlCommand("SELECT wcName, mouldNo, gtbarCode, convert(char(10), dtandTime, 110) AS getdate, convert(char(8), dtandTime, 108) AS gettime FROM vCuringpcr WHERE dtandTime>'" + newrDate.AddDays(-2) + "' and dtandTime<='" + rFromDate + "'", con);
                        cmd.CommandTimeout = 0;
                        var dread = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        curdt.Load(dread);

                        con.Close();
                        cmd.Dispose();
                        dread.Close();

                        SqlConnection bcon = new SqlConnection();
                        bcon.ConnectionString = ConfigurationManager.ConnectionStrings["mySQLConnection"].ToString();
                        bcon.Open();

                        SqlCommand bcmd = new SqlCommand("SELECT wcName, gtbarCode, CAST(dtandTime AS DATE) AS getdate, convert(char(8), dtandTime, 108) AS gettime, firstname +' '+ lastname as Inspectorname FROM vTbmPCR WHERE dtandTime>'" + newrDate.AddDays(-2) + "' and dtandTime<='" + rFromDate + "'", bcon);
                        bcmd.CommandTimeout = 0;
                        var bdread = bcmd.ExecuteReader(CommandBehavior.CloseConnection);
                        builddt.Load(bdread);

                        bcon.Close();
                        bcmd.Dispose();
                        bdread.Close();

                        /*for (int i = 0; i <= dt.Rows.Count; i++)
                        {
                            string buildwcName = "", curingString = "";

                            try
                            {
                                buildwcName = "<td>" + (from r in builddt.AsEnumerable()
                                                        where r.Field<string>("gtbarCode") == dt.Rows[i][6].ToString()
                                                        select r.Field<string>("wcName")).First<string>() + "</td>";
                            }
                            catch (Exception ex) { }
                            try
                            {
                                var newquery = (from r in curdt.AsEnumerable()
                                                where r.Field<string>("gtbarCode") == dt.Rows[i][6].ToString()
                                                select new
                                                {
                                                    getdate = r.Field<string>("getdate"),
                                                    gettime = r.Field<string>("gettime"),
                                                    wcName = r.Field<string>("wcName"),
                                                    mouldNo = r.Field<string>("mouldNo")
                                                }).ToArray();

                                curingString = "<td>" + newquery[0].getdate.ToString() + "</td><td>" + newquery[0].gettime.ToString() + "</td><td>" + newquery[0].wcName.ToString() + "</td><td></td><td>" + newquery[0].mouldNo.ToString() + "</td>";
                            }
                            catch (Exception ex) { }
                            try
                            {

                                sb.Append("<td>" + (i + 1) + "</td><td>" + dt.Rows[i][0] + "</td><td>" + dt.Rows[i][1] + "</td><td>" + dt.Rows[i][2] + "</td>" + (string.IsNullOrEmpty(buildwcName) ? "<td></td>" : buildwcName) + (string.IsNullOrEmpty(curingString) ? "<td></td><td></td><td></td><td></td><td></td>" : curingString) + "<td>" + dt.Rows[i][3] + "</td><td>" + dt.Rows[i][4] + "</td><td>" + dt.Rows[i][5] + "</td><td>" + dt.Rows[i][6] + "</td><td>" + dt.Rows[i][7] + "</td><td></td><td>" + dt.Rows[i][8] + "</td><td></td></tr>");
                            }
                            catch (Exception ex) { }
                        }*/

                        var query = from v in dt.AsEnumerable()
                                    join c in curdt.AsEnumerable() on v.Field<string>("gtbarCode") equals c.Field<string>("gtbarCode")
                                    join b in builddt.AsEnumerable() on v.Field<string>("gtbarCode") equals b.Field<string>("gtbarCode")
                                    select new { v, c, b };
                        int i = 0;
                        foreach (var x in query)
                        {
                            i++;
                            //dr[0] = x.b[0].ToString();
                            //dr[1] = x.c[0].ToString();
                            //dr[2] = x.c[1].ToString();
                            //dr[3] = x.v[0].ToString();
                            //dr[4] = x.v[1].ToString();
                            //dr[5] = x.v[2].ToString();

                            sb.Append("<tr><td>" + (i) + "</td><td>" + x.v[0].ToString() + "</td><td>" + x.v[1].ToString()
                                + "</td><td>" + x.v[2].ToString() + "</td><td>" + x.b[0].ToString() + "</td><td>" + x.b[2].ToString() + "</td><td>" + x.b[3].ToString() + "</td><td>" + x.b[4].ToString() + "</td> <td>" + x.c[3].ToString() + "</td><td>" + x.c[4].ToString() + "</td><td>"
                                +x.c[0].ToString()+"</td><td></td><td>"+x.c[1].ToString()+"</td><td>" +
                                x.v[3].ToString() + "</td><td>" + x.v[4].ToString() + "</td><td>" + x.v[5].ToString()
                                + "</td><td>" + x.v[6].ToString() + "</td><td>" + x.v[8].ToString() +
                                "</td><td>" + x.v[7].ToString() + "</td><td>" + x.v[9].ToString() + "</td><td></td><td></td></tr>");
                        }

                        sb.Append("</table>");
                        ExcelLabel.Text = sb.ToString();
                        ExcelPanel.Visible = true;
                        Response.Clear();
                        Response.ClearHeaders();
                        Response.ClearContent();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition", "attachment;filename=FirstPCRVisualInspectionReport.xls");
                        Response.ContentType = "application/vnd.ms-excel";

                        StringWriter stringWrite = new StringWriter();
                        HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
                        ExcelPanel.RenderControl(htmlWrite);

                        Response.Write(stringWrite.ToString());

                        Response.Flush();
                        Response.End();
                        ExcelPanel.Visible = false;
                    }
                }
            }
            catch (Exception exp)
            {
                myWebService.writeLogs(exp.Message, System.Reflection.MethodBase.GetCurrentMethod().Name, Path.GetFileName(Request.Url.AbsolutePath));
            }
        }
        protected void ViewButton_Click(object sender, EventArgs e)
        {
            rToDate = formatDate(tuoReportMasterFromDateTextBox.Text);
            rFromDate = formatDate(tuoReportMasterToDateTextBox.Text);
            rFromDate = formatfromDate(rFromDate.Replace(" 07:00:00",""));

            fillRecipeWiseGridView("select distinct description as curingRecipeName from vVisualInspectionPCR where dtandTime>'" + rToDate + "' and dtandTime<='" + rFromDate + "'");
            totalCheckedCountLabel.Text = totalcheckedcount.ToString();
            okcountLabel.Text = okcount.ToString();
            notokcountLabel.Text = notokcount.ToString();
            majorcountLabel.Text = majorcount.ToString();
            minorcountLabel.Text = minorcount.ToString();
        }
        protected void VIRecipeWiseTotalMinor_Click(object sender, EventArgs e)
        {
            backDiv.Visible = true;
            dialogPanel.Visible = true;
            emptyMsg.Visible = false;
            totalcheckedcount = Convert.ToInt32(totalCheckedCountLabel.Text);
            okcount = Convert.ToInt32(okcountLabel.Text);
            notokcount = Convert.ToInt32(notokcountLabel.Text);
            majorcount = Convert.ToInt32(majorcountLabel.Text);
            minorcount = Convert.ToInt32(minorcountLabel.Text);
            if (((LinkButton)sender).ID == "VIRecipeWiseTotalMazor")
            {
                GridViewRow gridViewRow = (GridViewRow)((DataControlFieldCell)((LinkButton)sender).Parent).Parent;
                string recipeCode = (((Label)gridViewRow.Cells[1].FindControl("VIinnerCuringRecipeNameLabel")).Text);

                fillBarCodeDetailGridView(recipeCode.ToString(), "BUFF");
            }
            else if (((LinkButton)sender).ID == "VIRecipeWiseTotalMinor")
            {
                GridViewRow gridViewRow = (GridViewRow)((DataControlFieldCell)((LinkButton)sender).Parent).Parent;
                string recipeCode = (((Label)gridViewRow.Cells[1].FindControl("VIinnerCuringRecipeNameLabel")).Text);

                fillBarCodeDetailGridView(recipeCode.ToString(), "HOLD");
            }
            else if (((LinkButton)sender).ID == "VIRecipeWiseTotalMinorTotal")
            {   
                fillBarCodeDetailGridView("Total", "HOLD");
            }
            else if (((LinkButton)sender).ID == "VIRecipeWiseTotalMajorTotal")
            {                
                fillBarCodeDetailGridView("Total", "BUFF");
            }
        }
        private void fillBarCodeDetailGridView(string recipecode, string status)
        {
            try
            {
                DataTable gridviewdt = new DataTable();
                DataTable dt = new DataTable();
                DataTable curdt = new DataTable();
                DataTable tbmdt = new DataTable();

                curdt.Columns.Add("wcName", typeof(string));
                curdt.Columns.Add("mouldNo", typeof(string));
                curdt.Columns.Add("gtbarCode", typeof(string));
                tbmdt.Columns.Add("wcName", typeof(string));
                tbmdt.Columns.Add("gtbarCode", typeof(string));

                dt.Columns.Add("wcname", typeof(string));
                dt.Columns.Add("description", typeof(string));
                dt.Columns.Add("gtbarcode", typeof(string));

                gridviewdt.Columns.Add("tbmWCName", typeof(string));
                gridviewdt.Columns.Add("curingWCName", typeof(string));
                gridviewdt.Columns.Add("mouldName", typeof(string));
                gridviewdt.Columns.Add("visualWCName", typeof(string));
                gridviewdt.Columns.Add("size", typeof(string));
                gridviewdt.Columns.Add("barcode", typeof(string));

                rToDate = TotaldtformatDate(tuoReportMasterFromDateTextBox.Text, tuoReportMasterToDateTextBox.Text);
                if (recipecode != "Total")
                    recipecode = " AND curingRecipeName='" + recipecode + "'";
                else
                    recipecode = "";
                if (status == "BUFF")
                    status = "status='2' ";
                else if (status == "HOLD")
                    status = "status='3' ";

                myConnection.open(ConnectionOption.SQL);
                myConnection.comm = myConnection.conn.CreateCommand();
                myConnection.comm.CommandText = "select wcname, description, gtbarCode from vVisualInspectionPCR where " + status + " and ((dtandTime>=" + rToDate + recipecode;
                myConnection.reader = myConnection.comm.ExecuteReader(CommandBehavior.CloseConnection);
                dt.Load(myConnection.reader);

                myConnection.conn.Close();
                myConnection.comm.Dispose();
                myConnection.reader.Close();

                if (dt.Rows.Count != 0)
                {
                    string InQuery = "(";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        InQuery += "'" + dt.Select()[i][2].ToString() + "',";
                    }
                    InQuery = InQuery.TrimEnd(',');
                    InQuery += ")";

                    SqlConnection con = new SqlConnection();
                    con.ConnectionString = ConfigurationManager.ConnectionStrings["mySQLConnection"].ToString();
                    con.Open();

                    SqlCommand cmd = new SqlCommand("SELECT wcName, mouldNo, gtbarCode FROM vCuringpcr WHERE gtbarCode IN " + InQuery.ToString(), con);
                    var dread = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    curdt.Load(dread);

                    con.Close();
                    cmd.Dispose();
                    dread.Close();

                    SqlConnection con1 = new SqlConnection();
                    con1.ConnectionString = ConfigurationManager.ConnectionStrings["mySQLConnection"].ToString();
                    con1.Open();

                    SqlCommand cmd1 = new SqlCommand("SELECT wcName, gtbarCode FROM vTbmPCR WHERE gtbarCode IN " + InQuery.ToString(), con1);
                    var dread1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);
                    tbmdt.Load(dread1);

                    con1.Close();
                    cmd1.Dispose();
                    dread1.Close();

                    /*for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = gridviewdt.NewRow();
                        dr[3] = dt.Select()[i][0].ToString();
                        dr[4] = dt.Select()[i][1].ToString();
                        dr[5] = dt.Select()[i][2].ToString();

                        for (int j = 0; j < curdt.Rows.Count; j++)
                        {
                            try
                            {
                                dr[1] = curdt.Select("gtbarCode='" + dt.Rows[i][2].ToString() + "'")[j][0].ToString();
                                dr[2] = curdt.Select("gtbarCode='" + dt.Rows[i][2].ToString() + "'")[j][1].ToString();
                            }
                            catch (Exception e) { }
                        }
                        for (int j = 0; j < tbmdt.Rows.Count; j++)
                        {
                            try
                            {
                                dr[0] = tbmdt.Select("gtbarCode='" + dt.Rows[i][2].ToString() + "'")[j][0].ToString();
                            }
                            catch (Exception e) { }
                        }
                        gridviewdt.Rows.Add(dr);
                    }*/
                    var query =  from v in dt.AsEnumerable()
                    join c in curdt.AsEnumerable() on v.Field<string>("gtbarCode") equals c.Field<string>("gtbarCode")
                    join b in tbmdt.AsEnumerable() on v.Field<string>("gtbarCode") equals b.Field<string>("gtbarCode")
                    select new { v, c, b};

                    foreach(var x in query)
                    {
                        DataRow dr = gridviewdt.NewRow();
                        dr[0] = x.b[0].ToString();
                        dr[1] = x.c[0].ToString();
                        dr[2] = x.c[1].ToString();
                        dr[3] = x.v[0].ToString();
                        dr[4] = x.v[1].ToString();
                        dr[5] = x.v[2].ToString();
                        gridviewdt.Rows.Add(dr);
                    }

                    performanceReportBarcodeDetailGridView.DataSource = gridviewdt;
                    performanceReportBarcodeDetailGridView.DataBind();
                    if (performanceReportBarcodeDetailGridView.Rows.Count == 0)
                        emptyMsg.Visible = true;
                    else
                        emptyMsg.Visible = false;
                }
                totalCheckedCountLabel.Text = totalcheckedcount.ToString();
                okcountLabel.Text = okcount.ToString();
                notokcountLabel.Text = notokcount.ToString();
                majorcountLabel.Text = majorcount.ToString();
                minorcountLabel.Text = minorcount.ToString();
                
            }
            catch (Exception exp)
            {
                myWebService.writeLogs(exp.Message, System.Reflection.MethodBase.GetCurrentMethod().Name, Path.GetFileName(Request.Url.AbsolutePath));
            }
        }
        private string TotaldtformatDate(String fromDate, String toDate)
        {
            string flag = "";
            string flag1 = "";
            string flag2 = "";

            if (fromDate != null)
            {
                string fday, fmonth, fyear;
                string tday, tmonth, tyear;

                string[] ftempDate = fromDate.Split(new char[] { '-' });
                string[] ttempDate = toDate.Split(new char[] { '-' });

                try
                {
                    fday = ftempDate[1].ToString().Trim();
                    fmonth = ftempDate[0].ToString().Trim();
                    fyear = ftempDate[2].ToString().Trim();
                    tday = ttempDate[1].ToString().Trim();
                    tmonth = ttempDate[0].ToString().Trim();
                    tyear = ttempDate[2].ToString().Trim();

                    flag1 = fday + "-" + fmonth + "-" + fyear + " " + "07" + ":" + "00" + ":" + "00";

                    if (Convert.ToInt32(tday)==12 && Convert.ToInt32(tmonth)==31)
                    {
                        flag2 = "01-01" + "-" + (Convert.ToInt32(tyear)+1).ToString() + " " + "07" + ":" + "00" + ":" + "00";
                    } 
                    else if (DateTime.DaysInMonth(Convert.ToInt32(tyear), Convert.ToInt32(tday)) != Convert.ToInt32(tmonth))
                    {
                        flag2 = tday + "-" + (Convert.ToInt32(tmonth) + 1).ToString() + "-" + tyear + " " + "07" + ":" + "00" + ":" + "00";
                    }
                    else
                    {
                        flag2 = (Convert.ToInt32(tday) + 1).ToString() + "-" + "01" + "-" + tyear + " " + "07" + ":" + "00" + ":" + "00";
                    }
                    //flag2 = tday + "-" + tmonth + "-" + tyear + " " + "07" + ":" + "00" + ":" + "00";


                    flag = "'" + flag1 + "' " + "and" + " " + "dtandTime<'" + flag2 + "' ))";
                }
                catch (Exception exp)
                {
                    myWebService.writeLogs(exp.Message, System.Reflection.MethodBase.GetCurrentMethod().Name, Path.GetFileName(Request.Url.AbsolutePath));
                }

            }
            return flag;
        }
        protected void FaultTypeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((DropDownList)sender).ID == "FaultTypeDropDownList")
                FaultAreaDropDownList.SelectedIndex = 0;

               
            if (FaultTypeDropDownList.SelectedValue == "Select" && FaultAreaDropDownList.SelectedValue == "Select")
            {
                VIReportRecipeWiseMainPanel.Visible = true;
                VIFaultWisePanel.Visible = false;
                SizeWiseRegionPanel.Visible = false;
            }
            else if (FaultTypeDropDownList.SelectedValue != "Select" && FaultAreaDropDownList.SelectedValue == "Select")
            {
                VIReportRecipeWiseMainPanel.Visible = false;
                VIFaultWisePanel.Visible = false;
                SizeWiseRegionPanel.Visible = true;
            }
            else if (FaultTypeDropDownList.SelectedValue != "Select" && FaultAreaDropDownList.SelectedValue != "Select")
            {

                VIFaultWisePanel.Visible = true;
                VIReportRecipeWiseMainPanel.Visible = false;

                SizeWiseRegionPanel.Visible = false;
            }
            FaultTypeLabel.Text = "Total"+" "+FaultTypeDropDownList.SelectedValue;
            Label11.Text = FaultTypeDropDownList.SelectedValue + "FaultArea";

            rToDate = formatDate(tuoReportMasterFromDateTextBox.Text);
            rFromDate = formatDate(tuoReportMasterToDateTextBox.Text);
            fillRecipeWiseGridView("select distinct curingRecipeName from " + tableName + " where dtandTime>'" + rToDate + "' and dtandTime<='" + rFromDate + "'");


        }
        public int tyrecount = 0;
        public int TyreQuantity(Object faltName)
        {
            int flag = 0;

            myConnection.open(ConnectionOption.SQL);
            myConnection.comm = myConnection.conn.CreateCommand();

            myConnection.comm.CommandText = "select distinct count(*) from "+tableName+" where defectstatusName='" + faultname + "' and CuringRecipeName='" + recipeCode + "' and faultAreaName='" + faultArea + "' and name='"+faltName+"'  and dtandTime>='" + rToDate + "' and dtandTime<='" + rFromDate + "'";


            myConnection.comm.Parameters.AddWithValue("@todate", formattoDate(rToDate));
            myConnection.comm.Parameters.AddWithValue("@fromdate", formatfromDate(rToDate));

            myConnection.reader = myConnection.comm.ExecuteReader();
            while (myConnection.reader.Read())
            {
                if (DBNull.Value != (myConnection.reader[0]))
                {
                        flag = Convert.ToInt32(myConnection.reader[0]);
                        tyrecount++;
                  
                }
                else
                    flag = 0;
            }

            return flag;
 
        }

    }
}
