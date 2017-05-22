<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Management.aspx.cs" Inherits="SoftPhoneService.Management" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>

            <asp:DataGrid ID="list" runat="server" EnableViewState="false">
            </asp:DataGrid>

            <%--<asp:Button ID="btnStart" runat="server" Text="Start" OnClick="btnStart_Click" />
            <asp:Button ID="btnStop" runat="server" Text="Stop" OnClick="btnStop_Click" />
            |
            <asp:Button ID="btnOpenSkill" runat="server" Text="OpenSkill" OnClick="btnOpenSkill_Click" />
            <asp:Button ID="btnCloseSkill" runat="server" Text="CloseSkill" OnClick="btnCloseSkill_Click" />
            |
            <asp:Button ID="btnReInit" runat="server" Text="ReInit" OnClick="btnReInit_Click" />
            |
            <asp:TextBox ID="txtReferenceId" runat="server"></asp:TextBox>
            <asp:Button ID="btnCloseOne" runat="server" Text="关闭单个订阅" OnClick="btnCloseOne_Click" />--%>
        </div>
    </form>
</body>
</html>
