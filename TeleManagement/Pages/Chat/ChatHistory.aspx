<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChatHistory.aspx.cs" Inherits="Tele.Management.Pages.Chat.ChatHistory" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>聊天记录详细信息</title>
    <script type="text/javascript" src="../../Scripts/jquery-1.7.1.min.js"></script>
    <link href="~/Styles/globalStyle.css" rel="stylesheet" />
    <link href="../../Styles/listStyle.css" rel="stylesheet" />
</head>
<body style="overflow-x: hidden; overflow-y: scroll;">
    <form id="form1" runat="server">
        <div class="historyBox">
            <asp:Literal ID="ltlContent" runat="server" Text="请至少选择一条记录。"></asp:Literal>
        </div>
    </form>
</body>
</html>
