﻿<%@ Page Language="C#" MasterPageFile="~/smartMISMaster.Master" AutoEventWireup="true" CodeBehind="AreaWisedetail.aspx.cs" Inherits="SmartMIS.Report.AreaWisedetail" Title="Area Wise" %>
<asp:Content ID="Content1" ContentPlaceHolderID="masterContentPlaceHolder" runat="server">

 <style>
        .durationclass
    {
        display:none;
        border: 1px solid #129FEA;
        padding: 6px 0;
        text-align: center;
        -webkit-border-radius: 8px;
        -moz-border-radius: 8px;
        border-radius: 8px;
        -webkit-box-shadow: #666 0px 2px 3px;
        -moz-box-shadow: #666 0px 2px 3px;
        box-shadow: #666 0px 2px 3px;
        position:absolute;
        width:21%;
        background-color: #15497C;
        background: -webkit-linear-gradient(top, #C3D9FF, #8DB3E1); /* For Safari 5.1 to 6.0 */
        background: -o-linear-gradient(bottom, #C3D9FF, #8DB3E1); /* For Opera 11.1 to 12.0 */
        background: -moz-linear-gradient(top, #C3D9FF, #8DB3E1); /* For Firefox 3.6 to 15 */
        background: linear-gradient(top, #C3D9FF, #8DB3E1); /* Standard syntax (must be last) */
    }
.links
{
    text-decoration:none;
    color:#0000EE;
    font-family:Verdana;
	font-weight: bold;
	font-size:12px;
	text-align:left;
	padding:2px;
}
.links:hover
{
      text-decoration:underline;      
}
    .close {
	background-color: #4C4C4C;
    background: -moz-linear-gradient(top, #272727, #4C4C4C);
    background: -webkit-gradient(linear, 0% 0%, 0% 100%, from(#272727), to(#4C4C4C));
	color: #FFFFFF;
	line-height: 25px;
	position: absolute;
	right: -12px;
	text-align: center;
	top: -10px;
	width: 24px;
	text-decoration: none;
	font-weight: bold;
	-webkit-border-radius: 12px;
	-moz-border-radius: 12px;
	border-radius: 12px;
  .box-shadow;
  .transition;
  .transition-delay(.2s);
  &:hover { background: #00d9ff; .transition; }
}
    .close:hover
    {
        background-color: #272727;
    background: -moz-linear-gradient(top, #4C4C4C, #272727);
    background: -webkit-gradient(linear, 0% 0%, 0% 100%, from(#4C4C4C), to(#272727));
        }
.dialogPanelCSS
{
    padding:12px;
    left:10%;
    top:50px;
    z-index:2000;
    position:fixed;
    background-color: #FF9933;
    background: -moz-linear-gradient(top, #C5DEE1, #E8EDFF);
    background: -webkit-gradient(linear, 0% 0%, 0% 100%, from(#C5DEE1), to(#E8EDFF));
    -webkit-border-radius: 5px;
	-moz-border-radius: 5px;
	border-radius: 5px;
    }
    .saveLink {
  padding:5px;
  background-color: #FF9933;
  background: -moz-linear-gradient(top, #FCAE41, #FF9933);
    background: -webkit-gradient(linear, 0% 0%, 0% 100%, from(#FCAE41), to(#FF9933));
  border: 1px solid #666;
  color:#000;
  text-decoration:none;
   font-weight:bold;
}
.alrtPopup
{
    padding:7px;width:30%;max-width:30%;height:auto;
    position:fixed;
    z-index: 1080;top:75px;left: 35%;
    -moz-border-radius: 15px;-webkit-border-radius: 15px;border-radius:15px;
    border:5px solid #888888;box-shadow: 0 0 5px 2px rgba( 0, 0, 0, 0.6 );
    background:#fff8c4 10px 50%;
    border:1px solid #f2c779;
    color:#555;
    font: bold 12px verdana;
}

</style>
<style type="text/css">
        .LabelTextAlignStyle {
            text-align:center;
        }
    </style> 
<script type="text/javascript" language="javascript">
        function ExportToExcel()
         {
            var html = $("#exportArea").html();
            html = $.trim(html);
            html = html.replace(/>/g, '&gt;');
            html = html.replace(/</g, '&lt;');
            $("input[id$='HdnValue']").val(html);
        }
        function showDuration(duration,hideWCID) {
            
            switch (duration) {
                case "Date":
                case "Date":
                    $("#setdate").slideDown();
                    break;
                    case "DateFrom":
                case "DateFrom":
                    $("#setdateFrom").slideDown();
                    break;
                case "Month":
                case "month":
                    $("#setMonth").slideDown();
                    break;
                case "Year":
                case "Year":
                    $("#setYear").slideDown();
                    break;
            }
            if (hideWCID == "hideWCIDDiv")
            $("#dropdownarea").slideUp();
            
        }
        
        function toggleduration() {
            if ($('select[id=ctl00_masterContentPlaceHolder_DropDownListDuration]').val() == "Date") {
                $("#setdate:visible").slideUp(700);
                $("#setdate:hidden").slideDown(700);
            }
            else if ($('select[id=ctl00_masterContentPlaceHolder_DropDownListDuration]').val() == "DateFrom") {
                $("#setdateFrom:visible").slideUp(700);
                $("#setdateFrom:hidden").slideDown(700);
            }
             else if ($('select[id=ctl00_masterContentPlaceHolder_DropDownListDuration]').val() == "Month") {
                $("#setMonth:visible").slideUp(700);
                $("#setMonth:hidden").slideDown(700);
            }
            else if ($('select[id=ctl00_masterContentPlaceHolder_DropDownListDuration]').val() == "Year") {
                $("#setYear:visible").slideUp(700);
                $("#setYear:hidden").slideDown(700);
            }
        }
        function toggletype() {
            $("#ctl00_ctl00_masterContentPlaceHolder_operatorPanel:visible").slideUp(700);
            $("#ctl00_ctl00_masterContentPlaceHolder_operatorPanel:hidden").slideDown(700);
        }
        function setDuration() {

            var element = ["setdate", "setdateFrom","setMonth", "setYear"];
            for (var i = 0; i <= element.length - 1; i++)
                $("#"+element[i]).slideUp(500);

            if (ctl00_masterContentPlaceHolder_DropDownListDuration.value == "Date")
               { $("#setdate").slideDown(500);}
                else if (ctl00_masterContentPlaceHolder_DropDownListDuration.value == "DateFrom")
                {
                $("#setdateFrom").slideDown(500);
                }
            else if (ctl00_masterContentPlaceHolder_DropDownListDuration.value == "Month") {
            $("#setMonth").slideDown(500);
            } 
            else if (ctl00_masterContentPlaceHolder_DropDownListDuration.value == "Year")
               { $("#setYear").slideDown(500);}
            
            //alert(ctl00_ctl00_masterContentPlaceHolder_DropDownListDuration.value);
             
            document.getElementById("ctl00_ctl00_masterContentPlaceHolder_DropDownListDuration").value = ctl00_ctl00_masterContentPlaceHolder_DropDownListDuration.value;
        }
       
        function hidewcDiv() {
            $("#dropdownarea:visible").slideUp(500);
        }
    </script>
    <script type="text/javascript" language="javascript">

        function enableSelection(value) 
        {
            var currentTime = new Date()

            var day = currentTime.getDate();
            var month = currentTime.getMonth() + 1;
            var year = currentTime.getFullYear();

            if (day.toString().length < 2) 
            {
                day = "0" + day;
            }
            if (month.toString().length < 2) 
            {
                month = "0" + month;
            }
            
            currentTime = (day + "-" + month + "-" + year);
            
            if (value == "0")
            {

                document.getElementById('ctl00_ctl00_masterContentPlaceHolder_reportMasterFromDateTextBox_calenderUserControlTextBox').disabled = false;
                document.getElementById('ctl00_ctl00_masterContentPlaceHolder_reportMasterFromDateTextBox_calenderUserControlTextBox').value = currentTime;

                document.getElementById('ctl00_ctl00_masterContentPlaceHolder_reportMasterToDateTextBox_calenderUserControlTextBox').disabled = false;
                document.getElementById('ctl00_ctl00_masterContentPlaceHolder_reportMasterToDateTextBox_calenderUserControlTextBox').value = currentTime;

            }           
            
        }

    </script>
<%@ Register src= "~/UserControl/calenderTextBox.ascx" TagName="calendertextbox" tagprefix="myControl" %>
<%@ Register src="~/UserControl/reportHeader.ascx" TagName="reportHeader" tagprefix="asp" %>
<link href="../Style/reportMaster.css" rel="stylesheet" type="text/css" />
<link href="../Style/master.css" rel="stylesheet" type="text/css" />
<link href="../Style/masterPage.css" rel="stylesheet" type="text/css" />
<table width="100%"><tr><td width="95%" align="center"><h2>Area Wise Scanning Report</h2></td>
<%--<td width="5%" align="right"><div><asp:LinkButton runat="server" ID="ExportExcel" onclick="expToExcel_Click"><img src="../Images/Excel.jpg" alt="Export To Excel" class="imag" /></asp:LinkButton>
--%></div>
</td></tr></table>

<asp:ScriptManager ID= "PCRVIScriptManager" AsyncPostBackTimeout="360000" runat="server"></asp:ScriptManager>
 
 <asp:Label ID="showDownload" runat="server" Text=""></asp:Label>
    <ContentTemplate>
<asp:Label ID="ShowWarning" runat="server" Text="" CssClass="alrtPopup" Visible="false"></asp:Label>
  <table cellpadding="0" cellspacing="0" style="width: 100%; border-style:solid; border-width:thin; border-color: #507CD1; background-color: #FFFFFF;" align="center">
<tr>

 
   
              <td style="font-weight:bold; font-family:Arial; font-size:small; width:15%">          
              Select Type:
               <asp:DropDownList ID="DropDownList1" runat="server"  Width="100" >
                  <asp:ListItem Value="Select">--Select--</asp:ListItem>
                   <asp:ListItem Value="PCR">PCR</asp:ListItem>
                    <asp:ListItem Value="TBR">TBR</asp:ListItem>
               </asp:DropDownList>
                
        </td>          
              
<td width="25%" ><span class="masterLabel" style="padding-left: 5px; cursor:pointer;" onclick="toggleduration();">Duration :</span>
                   <asp:DropDownList ID="DropDownListDuration"  
                   Width="40%" runat="server" 
                   CausesValidation="false" 
                   style="margin-bottom: 0px" onchange="setDuration();">
                   <asp:ListItem Value="Select">--Select--</asp:ListItem>
                   <asp:ListItem Value="Date">Date</asp:ListItem>
                  
                  
             </asp:DropDownList>
                                 
                             
                             <div id="setdate" class="durationclass">
                             <table width="100%"><tr>
                             <td class="masterLabel" width="24%">Date :</td>
                             <td class="masterLabel" width="6%"></td>
                             <td class="masterTextBox" width="60%">
                                
                                 <myControl:calenderTextBox ID="reportMasterFromDateTextBox" runat="server" Width="45%" />
                             </td>
                             </tr>
                             </table>
                             </div>
                            
                          <%--
                             <td style="font-weight:bold; font-family:Arial; font-size:small; width:15%">          
              Size:
               <asp:DropDownList ID="ddlRecipe" runat="server"  Width="100">
                 
                   
               </asp:DropDownList>
                
        </td>
                --%>
             
 

 <td style="font-weight:bold; font-family:Arial; font-size:small; width:10%">  &nbsp;&nbsp;&nbsp;
                <asp:Button ID="ViewButton" runat="server" Text="View Report" onclick="ViewButton_Click" CssClass="button" />

            &nbsp;</td>
    
<td class="tablecolumn"></td>

</tr>

   </table>
<br />
    <asp:Label ID="lbltext" runat="server" Text="NO RECORDS FOUND" Visible="false" BackColor="SkyBlue" Height="30px" Width="100%" Font-Size="Large" ></asp:Label>
<asp:GridView ID="grdView" runat="server" CssClass="TBMTable" 
        HeaderStyle-HorizontalAlign="Center" EmptyDataRowStyle-BackColor="Gray" 
            ShowHeader="true"  EmptyDataRowStyle-Width="100%" EmptyDataRowStyle-HorizontalAlign="Center"
              Width="100%" EmptyDataText="No Records Found"
            ShowFooter="false" HorizontalAlign="Center"   >
     <Columns>
            
                                </Columns>
     <RowStyle HorizontalAlign="Center" />
<EmptyDataRowStyle HorizontalAlign="Center" BackColor="Gray" Width="100%"></EmptyDataRowStyle>

<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
    </asp:GridView>
</asp:Content>
