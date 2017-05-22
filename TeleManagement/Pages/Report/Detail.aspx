<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="Tele.Management.Pages.Report.Detail" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=EmulateIE7" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>ReportViewer</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <rsweb:ReportViewer ID="ReportViewer1" runat="server" Height="510px" Width="100%"
                ShowPageNavigationControls="true" ShowBackButton="False" ProcessingMode="Remote"
                ShowParameterPrompts="True" Font-Names="Verdana" Font-Size="8pt" InteractiveDeviceInfos="(集合)"
                WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" ShowRefreshButton="True"
                ShowPrintButton="True" AsyncRendering="true">
            </rsweb:ReportViewer>
        </div>
    </form>
</body>
</html>
