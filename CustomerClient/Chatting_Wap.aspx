<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Chatting_Wap.aspx.cs" Inherits="CustomerClient.Chatting_Wap" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>

    <form id="form2" action="Chat/Chatting_wap">
        <table align="center">
            <tr>
                <td>UserID</td>
                <td>
                    <input type="text" id="UserID" name="UserID" value="1" /></td>
            </tr>
            <tr>
                <td>TargetSkill</td>
                <td>
                    <input type="text" id="TargetSkill" value="12" name="TargetSkill" /></td>
            </tr>
            <tr>
                <td>UserName</td>
                <td>
                    <input type="text" id="UserName" name="UserName" value="测试" /></td>
            </tr>
            <tr>
                <td>EnterID</td>
                <td>
                    <input type="text" id="EnterID" name="EnterID" value="9999" /></td>
            </tr>
            <tr>
                <td>strServiceCardNo</td>
                <td>
                    <input type="text" id="strServiceCardNo" name="strServiceCardNo" /></td>
            </tr>
            <tr>
                <td>MachineNo</td>
                <td>
                    <input type="text" id="MachineNo" name="MachineNo" /></td>
            </tr>
            <tr>
                <td>RegisterNumber</td>
                <td>
                    <input type="text" id="RegisterNumber" name="RegisterNumber" /></td>
            </tr>


            <tr>
                <td>Queue</td>
                <td>
                    <input type="text" id="Queue" name="Queue" /></td>
            </tr>

            <tr>
                <td>LAStatID</td>
                <td>
                    <input type="text" id="LAStatID" name="LAStatID" /></td>
            </tr>
            <tr>
                <td>emailClient</td>
                <td>
                    <input type="text" id="emailClient" value="3333@163.com" name="emailClient" /></td>
            </tr>
            <tr>
                <td>WSISID</td>
                <td>
                    <input type="text" id="WSISID" name="WSISID" /></td>
            </tr>
            <tr>
                <td>PreText</td>
                <td>
                    <textarea id="PreText" name="PreText" rows="10" cols="60"></textarea></td>
            </tr>
            <tr>
                <td>
                    <input type="submit" id="btnRequest" value="聊天申请" style="width: 100px; height: 50px;" />
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
