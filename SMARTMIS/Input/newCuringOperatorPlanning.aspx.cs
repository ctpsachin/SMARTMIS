﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data;
using System.IO;

namespace SmartMIS.Input
{
    public partial class newCuringOperatorPlanning : System.Web.UI.Page
    {

        smartMISWebService myWebService = new smartMISWebService();
        myConnection myConnection = new myConnection();
        int processID = 0;
        string shift = "", wcGroupID = "", WcGroup = "";
        string operatornameA = "", operatorsapcodeA = "";
        string operatornameB = "", operatorsapcodeB = "";
        string operatornameC = "", operatorsapcodeC = "";

        string areaname = "";


        protected void Page_Load(object sender, EventArgs e)
        {
            alertUser.Visible = false;
            if (PCRCuringRedioButton.Checked)
            {
                processID = 8;
                areaname = "PCRCuring";
            }
            else if (TBRCuringRedioButton.Checked)
            {
                processID = 5;
                areaname = "TBRCuring";
            }
           


                if (!IsPostBack)
                {
                fillWCGroupdropdownlist();
                fillmanningdropdownlist();
                fillgridview();
                                    
                }

        }

        protected void PCRCuringRedioButton_CheckedChanged(object sender, EventArgs e)
        {
            processID = 8;
            fillWCGroupdropdownlist();
            fillmanningdropdownlist();
            WcNameLabel.Text = "";
            groupIDlabel.Text = "";
            manningIDlabel.Text = "";
            OperatornameLabel.Text = "";

            fillgridview();



        }

        protected void TBRCuringRedioButton_CheckedChanged(object sender, EventArgs e)
        {
            processID = 5;
            fillWCGroupdropdownlist();
            fillmanningdropdownlist();
            WcNameLabel.Text = "";
            groupIDlabel.Text = "";
            manningIDlabel.Text = "";
            OperatornameLabel.Text = "";

            fillgridview();

            
          

        }

        protected void WcGroupDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string wcnames = "";
            try
            {
                myConnection.open(ConnectionOption.SQL);
                myConnection.comm = myConnection.conn.CreateCommand();


                myConnection.comm.CommandText = "select wcGroupID,workcenterName from vworkcenter where wcGroupName='" + WcGroupDropDownList.SelectedItem.Text+ "'";

                myConnection.reader = myConnection.comm.ExecuteReader();
                while (myConnection.reader.Read())
                {
                    wcnames = wcnames +","+ myConnection.reader[1].ToString();
                    groupIDlabel.Text = myConnection.reader[0].ToString();
                }

                WcNameLabel.Text = wcnames;
            }
            catch (Exception exp)
            {

            }
            finally
            {

                myConnection.reader.Close();
                myConnection.comm.Dispose();
                myConnection.close(ConnectionOption.SQL);
            }

        }
        protected void shiftDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            shift = shiftDropDownList.SelectedItem.Text;
        }
        protected void WcGroupDropDownList0_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                myConnection.open(ConnectionOption.SQL);
                myConnection.comm = myConnection.conn.CreateCommand();


                myConnection.comm.CommandText = "select firstName,lastname,iD from vmanning where sapcode='" + manningDropDownList.SelectedItem.Text+ "'";

                myConnection.reader = myConnection.comm.ExecuteReader();
                while (myConnection.reader.Read())
                {
                    OperatornameLabel.Text = myConnection.reader[0].ToString() +"  "+ myConnection.reader[1].ToString();
                    manningIDlabel.Text = myConnection.reader[2].ToString();
                }
            }
            catch (Exception exp)
            {

            }
            finally
            {

                myConnection.reader.Close();
                myConnection.comm.Dispose();
                myConnection.close(ConnectionOption.SQL);
            }
        
        }
                
        protected string getUser(string sapCode)
        {
            string name = "";
            try
            {
                myConnection.open(ConnectionOption.SQL);
                myConnection.comm = myConnection.conn.CreateCommand();
                myConnection.comm.CommandText = "select firstname,lastname from vmanning where sapCode='" + sapCode+"'";
                myConnection.reader = myConnection.comm.ExecuteReader();

                if (myConnection.reader.HasRows)
                {
                    if (myConnection.reader.Read())
                    {
                        name = myConnection.reader[0].ToString() + " " + myConnection.reader[1].ToString();
                    }
                }
                myConnection.close(ConnectionOption.SQL);
                myConnection.comm.Dispose();
            }
            catch (Exception exp)
            {
                myWebService.writeLogs(exp.Message, System.Reflection.MethodBase.GetCurrentMethod().Name, Path.GetFileName(Request.Url.AbsolutePath));
            }
            return name;

        }
        
        protected void AddButton_Click(object sender, EventArgs e)
        {
            save();
            clear();
            fillgridview();
        }
        protected void showWCNames_Click(object sender, EventArgs e)
        {
            try
            {
                string wcnames = "<OL>", WcGroup = "";
                if (((LinkButton)sender).ID == "wcNames")
                {
                    GridViewRow gridViewRow = (GridViewRow)((DataControlFieldCell)((LinkButton)sender).Parent).Parent;
                    WcGroup = (((Label)gridViewRow.Cells[1].FindControl("wcGroupName")).Text);
                }
                myConnection.open(ConnectionOption.SQL);
                myConnection.comm = myConnection.conn.CreateCommand();

                myConnection.comm.CommandText = "select wcGroupID,workcenterName from vworkcenter where wcGroupName='" + WcGroup + "'";
                myConnection.reader = myConnection.comm.ExecuteReader();
                while (myConnection.reader.Read())
                {
                    wcnames += "<LI>"+myConnection.reader[1].ToString() + "</LI>";
                }
                alertUser.Text = "WorkCenters List for " + WcGroup + "<BR>" + wcnames.TrimEnd(',') + "</OL><BR><Center><input type=\"button\" onClick=\"closePopup()\" value=\"  OK  \" class=\"popupBut\" /></Center>";
                alertUser.Visible = true;
            }
            catch (Exception exp)
            {
                myWebService.writeLogs(exp.Message, System.Reflection.MethodBase.GetCurrentMethod().Name, Path.GetFileName(Request.Url.AbsolutePath));
            }
            myConnection.close(ConnectionOption.SQL);
            myConnection.comm.Dispose();
        }
        private void fillWCGroupdropdownlist()
        {
            WcGroupDropDownList.DataSource = null;
            WcGroupDropDownList.DataSource = FillDropDownList("vworkcenter", "wcGroupName","where processID="+processID+"");
            WcGroupDropDownList.DataBind();
        }

        private void fillmanningdropdownlist()
        {
            manningDropDownList.DataSource = null;
            manningDropDownList.DataSource = FillDropDownList("vmanning","sapCode","where areaname='"+areaname+"' ORDER BY sapCode DESC");
            manningDropDownList.DataBind();
        }

        private ArrayList FillDropDownList(string tableName, string coloumnName, string whereClause)
        {
            ArrayList flag = new ArrayList();
            string sqlQuery = "";

            //Description   : Function for returning values of coloums of a table in an ArrayList
            //Author        : Brajesh kumar
            //Date Created  : 19 AUG 2014
            //Date Updated  : 19 AUG 2014
            //Revision No.  : 01

            flag.Add("Select");
            try
            {
                myConnection.open(ConnectionOption.SQL);
                myConnection.comm = myConnection.conn.CreateCommand();

                sqlQuery = "Select DISTINCT " + coloumnName + " from " + tableName + " " + whereClause + "";

                myConnection.comm.CommandText = sqlQuery;

                myConnection.reader = myConnection.comm.ExecuteReader();
                while (myConnection.reader.Read())
                {
                    if (myConnection.reader[0].ToString() == "" || myConnection.reader[0].ToString() == "NULL")
                    { 
                    }
                    else
                    flag.Add(myConnection.reader[0].ToString());
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
            flag.Add("None");
            return flag;
        }

        private void save()
        {
            try
            {
                shift = shiftDropDownList.SelectedItem.Text;
                if (!iswcgroupExist())
                {
                    if (WcGroupDropDownList.SelectedValue != "Select" && manningDropDownList.SelectedValue != "Select" && shiftDropDownList.SelectedValue != "Select")
                    {
                        myConnection.open(ConnectionOption.SQL);
                        myConnection.comm = myConnection.conn.CreateCommand();

                        myConnection.comm.CommandText = "Insert into curingOperatorPlanning (processID, wcGroupID, manningID,shift,dtandTime) values (@processID, @wcGroupID, @manningID,@shift,@dtandTime)";
                        myConnection.comm.Parameters.AddWithValue("@processID", processID);
                        myConnection.comm.Parameters.AddWithValue("@wcGroupID", Convert.ToInt32(groupIDlabel.Text));
                        myConnection.comm.Parameters.AddWithValue("@manningID", Convert.ToInt32(manningIDlabel.Text));
                        myConnection.comm.Parameters.AddWithValue("@shift", shift);
                        myConnection.comm.Parameters.AddWithValue("@dtandTime", DateTime.Now);

                        myConnection.comm.ExecuteNonQuery();

                        myConnection.comm.Dispose();
                        myConnection.close(ConnectionOption.SQL);
                    }
                    else
                    {
                        alertUser.Text = "<center><b>All fields are necessary</b><BR><BR><input type=\"button\" onClick=\"closePopup()\" value=\"  OK  \" class=\"popupBut\" /></center>";
                        alertUser.Visible = true;
                    }
                }

                else
                {
                    alertUser.Text = "<center><b>Work Group Already Exist. Try Other WorkGroup</b><BR><BR><input type=\"button\" onClick=\"closePopup()\" value=\"  OK  \" class=\"popupBut\" /></center>";
                    alertUser.Visible = true;
                }
            }
            catch (Exception exp)
            {
                myConnection.comm.Dispose();
                myConnection.close(ConnectionOption.SQL);
                myWebService.writeLogs(exp.Message, System.Reflection.MethodBase.GetCurrentMethod().Name, Path.GetFileName(Request.Url.AbsolutePath));

            }
          
        }

        private bool iswcgroupExist()
       {
           bool flag=false;

           try
           {
               myConnection.open(ConnectionOption.SQL);
               myConnection.comm = myConnection.conn.CreateCommand();
               myConnection.comm.CommandText = "select * from vcuringOperatorPlanning  where wcGroupName='" + WcGroupDropDownList.SelectedValue + "' and shift='"+shift+"'";
               if (myConnection.comm.ExecuteReader().HasRows)
               {
                   flag = true;
               }
               else
                   flag = false;

             //  myConnection.reader.Close();
               myConnection.close(ConnectionOption.SQL);
               myConnection.comm.Dispose();
           }
           catch (Exception exc)
           {
               myConnection.reader.Close();
               myConnection.close(ConnectionOption.SQL);
               myConnection.comm.Dispose();
               flag = false;
           }

           return flag;

        }

        private void clear()
        {

            fillWCGroupdropdownlist();
            fillmanningdropdownlist();
            shiftDropDownList.SelectedIndex = 0;
            WcNameLabel.Text = "";
            groupIDlabel.Text = "";
            manningIDlabel.Text = "";
            OperatornameLabel.Text = "";
        }
        private static DataTable GetDistinctRecords(DataTable dt, string Columns)
        {
            DataTable dtUniqRecords = new DataTable();
            dtUniqRecords = dt.DefaultView.ToTable(true, Columns);
            return dtUniqRecords;
        }
        private void fillgridview()
        {
            DataTable gridviewdt = new DataTable();
            DataTable maindt = new DataTable();

            maindt.Columns.Add("sapCode", typeof(string));
            maindt.Columns.Add("firstname", typeof(string));
            maindt.Columns.Add("lastname", typeof(string));
            gridviewdt.Columns.Add("wcGroupID", typeof(string));
            gridviewdt.Columns.Add("wcGroupName", typeof(string));
            gridviewdt.Columns.Add("shiftAoperatorsapCode", typeof(string));
            gridviewdt.Columns.Add("ShiftAoperatorCode", typeof(string));
            gridviewdt.Columns.Add("shiftBoperatorsapCode", typeof(string));
            gridviewdt.Columns.Add("ShiftBoperatorCode", typeof(string));
            gridviewdt.Columns.Add("shiftCoperatorsapCode", typeof(string));
            gridviewdt.Columns.Add("ShiftCoperatorCode", typeof(string));
            
            myConnection.open(ConnectionOption.SQL);
            myConnection.comm = myConnection.conn.CreateCommand();
            if(TBRCuringRedioButton.Checked)
                myConnection.comm.CommandText = "select wcGroupID,wcGroupName, sapCode,firstname,lastname, shift from vcuringOperatorPlanning where processID=5 order by wcGroupID desc";
            else if(PCRCuringRedioButton.Checked)
                myConnection.comm.CommandText = "select wcGroupID,wcGroupName, sapCode,firstname,lastname, shift from vcuringOperatorPlanning where processID=8 order by wcGroupID desc";

            myConnection.reader = myConnection.comm.ExecuteReader();            
            maindt.Load(myConnection.reader);

            DataTable uniqwcGroupdt = new DataTable();
            uniqwcGroupdt = GetDistinctRecords(maindt, "wcGroupID");
            DataRow dr = gridviewdt.NewRow();
            
            for (int i = 0; i < uniqwcGroupdt.Rows.Count; i++)
            {
                string wgname = "";
                try
                {
                    operatornameA = maindt.Select("wcGroupID ='" + uniqwcGroupdt.Rows[i][0].ToString() + "' AND shift='A'")[0][1].ToString() + " " + maindt.Select("wcGroupID ='" + uniqwcGroupdt.Rows[i][0].ToString() + "' AND shift='A'")[0][2].ToString();
                    operatorsapcodeA = maindt.Select("wcGroupID ='" + uniqwcGroupdt.Rows[i][0].ToString() + "' AND shift='A'")[0][0].ToString();
                    wgname = maindt.Select("wcGroupID ='" + uniqwcGroupdt.Rows[i][0].ToString() + "' AND shift='A'")[0][4].ToString();
                }
                catch (Exception exp)
                {
                    operatornameA = ""; operatorsapcodeA = "";
                }
                try
                {
                   operatornameB = maindt.Select("wcGroupID ='" + uniqwcGroupdt.Rows[i][0].ToString() + "' AND shift='B'")[0][1].ToString() + " " + maindt.Select("wcGroupID ='" + uniqwcGroupdt.Rows[i][0].ToString() + "' AND shift='B'")[0][2].ToString();
                   operatorsapcodeB = maindt.Select("wcGroupID ='" + uniqwcGroupdt.Rows[i][0].ToString() + "' AND shift='B'")[0][0].ToString();
                   wgname = maindt.Select("wcGroupID ='" + uniqwcGroupdt.Rows[i][0].ToString() + "' AND shift='B'")[0][4].ToString();
                
                }
                catch (Exception exp)
                { 
                    operatornameB = ""; operatorsapcodeB = ""; 
                }
                
                try
                {
                    operatornameC = maindt.Select("wcGroupID ='" + uniqwcGroupdt.Rows[i][0].ToString() + "' AND shift='C'")[0][1].ToString() + " " + maindt.Select("wcGroupID ='" + uniqwcGroupdt.Rows[i][0].ToString() + "' AND shift='C'")[0][2].ToString();
                    operatorsapcodeC = maindt.Select("wcGroupID ='" + uniqwcGroupdt.Rows[i][0].ToString() + "' AND shift='C'")[0][0].ToString();
                    wgname = maindt.Select("wcGroupID ='" + uniqwcGroupdt.Rows[i][0].ToString() + "' AND shift='C'")[0][4].ToString();
                }
                catch (Exception exp)
                {
                    operatornameC = ""; operatorsapcodeC = "";
                }
                DataRow drow = gridviewdt.NewRow();
                drow[0] = uniqwcGroupdt.Rows[i]["wcGroupID"];
                drow[1] = wgname;
                drow[2] = operatorsapcodeA;
                drow[3] = operatornameA;
                drow[4] = operatorsapcodeB;
                drow[5] = operatornameB;
                drow[6] = operatorsapcodeC;
                drow[7] = operatornameC;
                gridviewdt.Rows.Add(drow);
            }

            operatorPlanningMainGridView.DataSource = gridviewdt;
            operatorPlanningMainGridView.DataBind();
                         
        }

        protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (PCRCuringRedioButton.Checked)
                areaname = "PCRCuring";
            else if (TBRCuringRedioButton.Checked)
                areaname = "TBRCuring";

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                
                if (((GridView)sender).ID == "operatorPlanningMainGridView")
                {

                    Label wcGroupIDLabel = ((Label)e.Row.FindControl("wcGroupID"));
                    wcGroupID = wcGroupIDLabel.Text;             
                                    
                    DropDownList DropDownListA = (DropDownList)e.Row.FindControl("DropDownListA");
                    DropDownListA.DataSource = FillDropDownList("vmanning", "sapCode", "where areaname='"+areaname+"'");
                    DropDownListA.DataBind();
                    Label shiftAoperatorsapCodeLabel = ((Label)e.Row.FindControl("shiftAoperatorsapCode"));
                    DropDownListA.SelectedValue = shiftAoperatorsapCodeLabel.Text.ToString();

                    DropDownList DropDownListB = (DropDownList)e.Row.FindControl("DropDownListB");
                    DropDownListB.DataSource = FillDropDownList("vmanning", "sapCode", "where areaname='" + areaname + "'");
                    DropDownListB.DataBind();
                    Label shiftBoperatorsapCodeLabel = ((Label)e.Row.FindControl("shiftBoperatorsapCode"));
                    DropDownListB.SelectedValue = shiftBoperatorsapCodeLabel.Text.ToString();

                    DropDownList DropDownListC = (DropDownList)e.Row.FindControl("DropDownListC");
                    DropDownListC.DataSource = FillDropDownList("vmanning", "sapCode", "where areaname='" + areaname + "'");
                    DropDownListC.DataBind();
                    Label shiftCoperatorsapCodeLabel = ((Label)e.Row.FindControl("shiftCoperatorsapCode"));
                    DropDownListC.SelectedValue = shiftCoperatorsapCodeLabel.Text.ToString();

                }

            }

        }
               
        protected void remove_Click(object sender, EventArgs e)
        {
            string shiftA = "", shiftB = "", shiftC = "";
            if (((Button)sender).ID == "remove")
            {
                GridViewRow gridViewRow = (GridViewRow)((DataControlFieldCell)((Button)sender).Parent).Parent;
                shiftA = (((Label)gridViewRow.Cells[1].FindControl("ShiftALabel")).Text);
                shiftB = (((Label)gridViewRow.Cells[1].FindControl("ShiftBLabel")).Text);
                shiftC = (((Label)gridViewRow.Cells[1].FindControl("ShiftCLabel")).Text);
            }
        }

        protected string getManningId(string opSAPID)
        {
            string ret = "";
            myConnection.open(ConnectionOption.SQL);
            myConnection.comm = myConnection.conn.CreateCommand();
            myConnection.comm.CommandText = "select  iD from manningMaster  where sapCode=" + opSAPID;
            myConnection.reader = myConnection.comm.ExecuteReader();

            if (myConnection.reader.HasRows)
            {
                if (myConnection.reader.Read())
                    ret = myConnection.reader["iD"].ToString();
                
            }
            myConnection.close(ConnectionOption.SQL);
            myConnection.comm.Dispose();
            return ret;
        }
        /*protected string validateOperator(string opSAPID)
        {
            myConnection.open(ConnectionOption.SQL);
            myConnection.comm = myConnection.conn.CreateCommand();
            myConnection.comm.CommandText = "select  sapCode,firstname,lastname, shift, wcGroupName from vcuringOperatorPlanning  where wcgroupName!='" + selectedWcGroup.Text + "' and   sapCode='" + opSAPID + "'";
            myConnection.reader = myConnection.comm.ExecuteReader();
            string str = "";
            if (myConnection.reader.HasRows)
            {
                while (myConnection.reader.Read())
                    str = wcGroupID + "<center><b>Operator " + myConnection.reader["firstname"] + " " + myConnection.reader["lastname"] + " is already assigned to " + myConnection.reader["wcGroupName"] + " for shift " + myConnection.reader["shift"] + "</b><BR></center>";
                myConnection.close(ConnectionOption.SQL);
                myConnection.comm.Dispose();
                return str;
            }
            else
            {
                myConnection.close(ConnectionOption.SQL);
                myConnection.comm.Dispose();
                return str;
            }
        }*/
        protected void DropDownListB_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow gridViewRow = (GridViewRow)((DataControlFieldCell)((DropDownList)sender).Parent).Parent;
            string opSAPID = (((DropDownList)gridViewRow.Cells[1].FindControl("DropDownListB")).Text);
            Label shiftBoperatorCodeLabel = (((Label)gridViewRow.Cells[1].FindControl("shiftBoperatorCodeLabel")));

            shiftBoperatorCodeLabel.Text = getUser(opSAPID);
        }
        protected void DropDownListC_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow gridViewRow = (GridViewRow)((DataControlFieldCell)((DropDownList)sender).Parent).Parent;
            string opSAPID = (((DropDownList)gridViewRow.Cells[1].FindControl("DropDownListC")).Text);
            Label shiftCoperatorCodeLabel = (((Label)gridViewRow.Cells[1].FindControl("shiftCoperatorCodeLabel")));

            shiftCoperatorCodeLabel.Text = getUser(opSAPID);
        }
        protected void DropDownListA_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow gridViewRow = (GridViewRow)((DataControlFieldCell)((DropDownList)sender).Parent).Parent;
            string opSAPID = (((DropDownList)gridViewRow.Cells[1].FindControl("DropDownListA")).Text);
            Label shiftAoperatorCodeLabel = (((Label)gridViewRow.Cells[1].FindControl("shiftAoperatorCodeLabel")));

            shiftAoperatorCodeLabel.Text = getUser(opSAPID);
            
        }
        protected bool saveinfo(string sapCode, string wcGID, string shift)
        {
            try
            {
                string manID = "";
                if (sapCode == "None")
                {
                    myConnection.open(ConnectionOption.SQL);
                    myConnection.comm = myConnection.conn.CreateCommand();
                    myConnection.comm.CommandText = "DELETE FROM curingOperatorPlanning where wcGroupID='" + wcGID + "' AND shift='" + shift + "'";
                    myConnection.comm.ExecuteNonQuery();
                }
                else if (sapCode != "Select" && sapCode != null)
                {
                    manID = getManningId(sapCode);
                    myConnection.open(ConnectionOption.SQL);
                    myConnection.comm = myConnection.conn.CreateCommand();
                    myConnection.comm.CommandText = "UPDATE curingOperatorPlanning SET manningID='" + manID + "' where wcGroupID='" + wcGID + "' AND shift='" + shift + "'";
                    int cnt = myConnection.comm.ExecuteNonQuery();
                    myConnection.close(ConnectionOption.SQL);
                    myConnection.comm.Dispose();
                    if (cnt == 0)
                    {
                        myConnection.open(ConnectionOption.SQL);
                        myConnection.comm = myConnection.conn.CreateCommand();
                        myConnection.comm.CommandText = "INSERT INTO curingOperatorPlanning(processID,manningID,wcGroupID,shift,dtandTime) VALUES('8','" + manID + "','" + wcGID + "','" + shift + "',GETDATE())";
                        myConnection.comm.ExecuteNonQuery();

                    }
                }
                myConnection.close(ConnectionOption.SQL);
                myConnection.comm.Dispose();
                myConnection.reader.Close();
                return true;
            }
            catch (Exception exp)
            {
                myWebService.writeLogs(exp.Message, System.Reflection.MethodBase.GetCurrentMethod().Name, Path.GetFileName(Request.Url.AbsolutePath));
                return false;
            }
        }
        protected void saveAll_Click(object sender, EventArgs e)
        {
            string[,] opA = new string[20,10];
            string[,] opB = new string[20,10];
            string[,] opC = new string[20,10];
            
            //Looping through each Gridview row to find exact Row of the Grid from where the SelectedIndex change event is fired.
            string msg = "";
            int i = 0;
            bool flag = false;
            foreach (GridViewRow row in operatorPlanningMainGridView.Rows)
            {
                Control wcGroupID = row.FindControl("wcgroupID") as Label;
                Control wcGroupName = row.FindControl("wcgroupName") as Label;
                Control DropDownListA = row.FindControl("DropDownListA") as DropDownList;
                Control DropDownListB = row.FindControl("DropDownListB") as DropDownList;
                Control DropDownListC = row.FindControl("DropDownListC") as DropDownList;
                string shA = ((DropDownList)DropDownListA).SelectedValue;
                string shB = ((DropDownList)DropDownListB).SelectedValue;
                string shC = ((DropDownList)DropDownListC).SelectedValue;
                string wcGID = ((Label)wcGroupID).Text;
                string wcGName = ((Label)wcGroupName).Text;
                if (DropDownListA != null)
                {
                    for (int j = 0; j <= i; j++)
                    {
                        if (opA[j, 0] == shA && shA != "Select" && shA != "None")
                        {
                            msg += shA + " is duplicate entry in Shift A for WorkGroup " + wcGName + "<BR>";
                            flag = true;
                            break;
                        }
                    }
                    if (flag == false)
                    {
                        opA[i, 0] = shA;
                        opA[i, 1] = wcGID;
                    }
                }
                if (DropDownListB != null)
                {
                    for (int j = 0; j <= i; j++)
                    {
                        if (opB[j, 0] == shB && shB != "Select" && shB != "None")
                        {
                            msg += shB + " is duplicate entry in Shift B for WorkGroup " + wcGName + "<BR>";
                            flag = true;
                        }
                    }
                    if (flag == false)
                    {
                        opB[i, 0]= shB;
                        opB[i, 1] = wcGID;
                    }
                }
                if (DropDownListC != null)
                {
                    for (int j = 0; j <= i; j++)
                    {
                        if (opC[j, 0] == shC && shC != "Select" && shC != "None")
                        {
                            msg += shC + " is duplicate entry in Shift C for WorkGroup " + wcGName + "<BR>";
                            flag = true;
                        }
                    }
                    if (flag == false)
                    {
                        opC[i, 0] = shC;
                        opC[i, 1] = wcGID;
                    }
                }
                i++;
            }
            if (flag == true)
            {
                alertUser.Text = msg.ToString() + "<center><BR><BR><input type=\"button\" onClick=\"closePopup()\" value=\"  OK  \" class=\"popupBut\" /></center>";
                alertUser.Visible = true;
            }
            else 
            {
                for (int x = 0; x <= 5; x++)
                {
                    if (saveinfo(opA[x, 0], opA[x, 1], "A"))
                        alertUser.Text = "<center>Information Saved Successfully<BR><BR><input type=\"button\" onClick=\"closePopup()\" value=\"  OK  \" class=\"popupBut\" /></center>";
                }
                for (int x = 0; x <= 5; x++)
                {
                    if (saveinfo(opB[x, 0], opB[x, 1], "B"))
                       alertUser.Text = "<center>Information Saved Successfully<BR><BR><input type=\"button\" onClick=\"closePopup()\" value=\"  OK  \" class=\"popupBut\" /></center>";
                }
                for (int x = 0; x <= 5; x++)
                {
                    if (saveinfo(opC[x, 0], opC[x, 1], "C")) alertUser.Text = "<center>Information Saved Successfully<BR><BR><input type=\"button\" onClick=\"closePopup()\" value=\"  OK  \" class=\"popupBut\" /></center>";
                }
                alertUser.Visible = true;
            }
        }

        protected void workGroupChangeType_CheckedChanged(object sender, EventArgs e)
        {
            if(workGroupChangeType.Checked)
            {
                addNewWorkGroup.Visible = true;                
            }
            else
            {
                addNewWorkGroup.Visible = false;
            }
        }
    }
}
