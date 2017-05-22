<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FileUpload.aspx.cs" Inherits="Tele.Management.Pages.FileUpload" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <base target="_self" />
    <title>批量导入</title>
    <link href="../../Styles/globalStyle.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="formDiv">
            <h3>批量导入</h3>
            <div style="line-height: 30px;">
                第一步：如果您还没有下载模板，请先
                <asp:Button ID="btnDownloadTemplate" runat="server" CssClass="btn100"
                    Text="下载模板" OnClick="btnDownloadTemplate_Click" />
            </div>
            <div style="line-height: 30px;">
                第二步：在下载的模板中填写数据后保存。
            </div>
            <div style="line-height: 30px;">
                第三步：选择保存的文件，并上传<input type="file" id="file" runat="server" style="width: 300px" /> &nbsp;<asp:Button ID="btnUpload" runat="server" CssClass="btn100"
                    Text="上传" OnClick="btnUpload_Click" />
            </div>
        </div>
        <div style="margin: 3px;">
            <asp:Literal ID="ltlError" runat="server"></asp:Literal>
        </div>
    </form>
</body>
</html>
