<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="domain.aspx.cs" Inherits="SoftPhoneToolBar.domain" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <script>
        try {
            <%  if (Request.Cookies["softphone"] != null && Request.Cookies["softphone"].Values["isemergency"] == "0")
                {%>
            document.domain = '<%:System.Web.Configuration.WebConfigurationManager.AppSettings["document.domain"]%>';
            <%}%>
        }
        catch (e) { }
    </script>
</head>
<body>
</body>
</html>
