<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Tele.Management.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>联想服务CC电话应用管理系统（CCM）</title>
    <script type="text/javascript">
        window.onload = function () {
            windowWidth = window.screen.availWidth;
            windowHeight = window.screen.availHeight;
            window.moveTo(0, 0);
            window.resizeTo(windowWidth, windowHeight);
        }
    </script>
</head>
<frameset rows="80,*" framespacing="0" border="0">
    <frame name="Header" height="100%" src="Statics/Header.aspx" width="100%" noresize frameborder="0" style="margin:0px" scrolling="no"></frame>
    <frameset cols="215,*" height="100%" border="0">
        <frame name="Tree" src="Statics/Left.aspx" frameborder="0" scrolling="auto" style="overflow:hidden" noresize></frame>
        <frame name="Content" src="Statics/Home.aspx" frameborder="0" scrolling="auto"></frame>
    </frameset> 
</frameset>
</html>
