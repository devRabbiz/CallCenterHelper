<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Header.aspx.cs" Inherits="Tele.Management.Statics.Header" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="../Styles/globalStyle.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="logo">
            <h1>联想服务CC电话应用管理系统</h1>
            <ul>
                <li><%= DateTime.Now.ToString("yyyy年M月d日") %></li>
                <li>当前用户：<asp:Literal ID="ltlUser" runat="server"></asp:Literal></li>
                <li>
                    <asp:LinkButton ID="btnLogout" runat="server" Text="注销" OnClick="btnLogout_Click"></asp:LinkButton></li>
            </ul>
        </div>
    </form>
</body>
</html>
