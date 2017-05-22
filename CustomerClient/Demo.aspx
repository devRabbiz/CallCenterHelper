<%@ Page Language="C#" AutoEventWireup="true" %>

<!DOCTYPE html>
<script runat="server">

    protected void btnPost_Click(object sender, EventArgs e)
    {
        var file = this.Request.Files[0];

        Response.Write(" file.ContentType:" + file.ContentType + "<br/>");
        Response.Write(" file.ContentType:" + System.IO.Path.GetExtension(file.FileName) + "<br/>");
    }
</script>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:FileUpload ID="File1" runat="server" />
            <asp:Button ID="btnPost" runat="server" OnClick="btnPost_Click" />
        </div>
    </form>
</body>
</html>
