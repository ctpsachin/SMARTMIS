﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="SmartMIS.Error" Title="Unhandled Exception Occurred" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<link rel="stylesheet" href="../Style/masterPage.css" type="text/css" charset="utf-8" />
<link rel="stylesheet" href="../Style/CalendarControl.css"  type="text/css" charset="utf-8" />
<link rel="SHORTCUT ICON" href="../Images/favicon.ico" />
    

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="masterPageHead" runat="server">
    <title>Smart MIS</title>
    
 <script language="javascript" type="text/javascript" src="../Script/jquery-1.4.1.js"></script>    
    <script type="text/javascript" language="javascript" src="../Script/ddaccordion.js">

        /***********************************************
        * Accordion Content script
        ***********************************************/

    </script>
    
      
    <script type="text/javascript" language="javascript">


        ddaccordion.init({
            headerclass: "submenuheader", //Shared CSS class name of headers group
            contentclass: "submenu", //Shared CSS class name of contents group
            revealtype: "click", //Reveal content when user clicks or onmouseover the header? Valid value: "click", "clickgo", or "mouseover"
            mouseoverdelay: 200, //if revealtype="mouseover", set delay in milliseconds before header expands onMouseover
            collapseprev: true, //Collapse previous content (so only one open at any time)? true/false 
            defaultexpanded: [], //index of content(s) open by default [index1, index2, etc] [] denotes no content
            onemustopen: false, //Specify whether at least one header should be open always (so never all headers closed)
            animatedefault: false, //Should contents open by default be animated into view?
            persiststate: true, //persist state of opened contents within browser session?
            toggleclass: ["", ""], //Two CSS classes to be applied to the header when it's collapsed and expanded, respectively ["class1", "class2"]
            togglehtml: ["suffix", "<img src='../Images/plus.gif' class='statusicon' />", "<img src='../Images/minus.gif' class='statusicon' />"], //Additional HTML added to the header when it's collapsed and expanded, respectively  ["position", "html1", "html2"] (see docs)
            animatespeed: "fast", //speed of animation: integer in milliseconds (ie: 200), or keywords "fast", "normal", or "slow"
            oninit: function(headers, expandedindices) { //custom code to run when headers have initalized
                //do nothing
            },
            onopenclose: function(header, index, state, isuseractivated) { //custom code to run whenever a header is opened or closed
                //do nothing
            }
        })

    </script>
    
    <script language="javascript" type="text/javascript">
        function revealModal(divID)
        {
            window.onscroll = function() { document.getElementById(divID).style.top = document.body.scrollTop; };
            document.getElementById(divID).style.display = "block";
            document.getElementById(divID).style.top = document.body.scrollTop;
        }
        function hideModal(divID) {
            document.getElementById(divID).style.display = "none";
        }
        function getKeyCode(e)
        {
            if (window.event)
                return window.event.keyCode;
            else if (e)
                return e.which;
            else
                return null;
        }
        function keyRestrict(e, validchars)
        {
            var key = '', keychar = '';
            key = getKeyCode(e);
            if (key == null) return true;
            keychar = String.fromCharCode(key);
            keychar = keychar.toLowerCase();
            validchars = validchars.toLowerCase();
            if (validchars.indexOf(keychar) != -1)
                return true;
            if (key == null || key == 0 || key == 8 || key == 9 || key == 13 || key == 27)
                return true;
            return false;
        }
        function validateTextBox(textBoxID) 
        {
            if (textBoxID.value == '') 
            {
                textBoxID.value = "0";
            }
        }
            
    </script>

</head>
<body>
    <form id="masterPageForm" runat="server">
     
    <table align="center" class="masterPageTable">
        <tr>
            <td class="masterPageFirstCol" rowspan="2">
               <img alt="Smart MIS" src="../Images/logo.png" class="logoImage" />
            </td>
            <td class="masterPageSecondCol" colspan="4">
                <div style="height: 50px; text-align: right">                    
                <img alt="JK Tyres" class="ceatImg" src="../Images/logo_jk.jpg" /></div>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <div class="masterHeader" style="height: 25px; vertical-align: bottom">
                 </div>
            </td>
        </tr>
        <tr>
            <td colspan="3" width="50%">
                <asp:SiteMapPath ID="masterSiteMapPath" runat="server" Font-Names="Verdana" 
                    Font-Size="0.8em" PathSeparator=" : ">
                    <PathSeparatorStyle Font-Bold="True" ForeColor="#507CD1" />
                    <CurrentNodeStyle ForeColor="#333333" />
                    <NodeStyle Font-Bold="True" ForeColor="#284E98" />
                    <RootNodeStyle Font-Bold="True" ForeColor="#507CD1" />
                </asp:SiteMapPath>
            </td>
            <td  width="20%">
                
            </td>
             
            <td align="right" width="33%">
                <div>
                    <asp:Label ID="masterWelcomeLabel" runat="server" CssClass="masterWelcomeLabel" Text="Welcome User!"></asp:Label>&nbsp;
                    <a href="/SmartMIS/Home/home.aspx" class="navigation">[Home]</a>
                </div>
            </td>
        </tr>
        <tr>
            <td valign="top">
                <div class="glossymenu">
                

            </div>
            </td>
            
            <td valign="top" colspan="4">
                <h3>An Error has occurred.</h3>
                The application has encountered an unknown error!<br />
                We've logged the error.
Please try again or contact <a href="mailto:support@smartcontrols.in?Subject=Error%20Notification" target="_top">support@smartcontrols.in</a>. Help us improve your experience by sending an error report
            </td>
        </tr>
        <tr>
            <td colspan="5">
                <div class="masterFooter">
                    <p class="masterFooterTagline">© 2011 Developed By SmartControls India Ltd.</p>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;</td>
            <td colspan="4">
                &nbsp;</td>
        </tr>
    </table>
    </form>
</body>
</html>