<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Tele.Management.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link href="Styles/globalStyle.css" rel="stylesheet" />
    <title>联想服务CC电话应用管理系统登录</title>
    <script type="text/javascript">
        if (top.location !== self.location)
            top.location = self.location;
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="logo">
            <h1>联想服务CC电话应用管理系统</h1>
        </div>
        <div class="loginBox">
            <table>
                <tr>
                    <td>
                        <label>用户名</label></td>
                    <td>
                        <asp:TextBox ID="txtLogonID" runat="server"></asp:TextBox></td>
                    <td></td>
                </tr>
                <tr>
                    <td>
                        <label>密码</label></td>
                    <td>
                        <asp:TextBox ID="txtPassword" TextMode="Password" runat="server"></asp:TextBox></td>
                    <td>
                        <asp:Button ID="btnLogin" runat="server" Text="登录" CssClass="btn100" OnClick="btnLogin_Click" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div id="divMessage" style="color: red" runat="server">
                            登录失败。
                            原因可能为：
                            1、没有本系统的使用权限。
                            2、用户名，密码不匹配。
                        </div>
                    </td>
                </tr>
            </table>
            <hr />
            <hr style="border-bottom-color: red;" />
        </div>
    </form>
</body>
</html>
