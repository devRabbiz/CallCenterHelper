<%@ Page Title="" Language="C#" MasterPageFile="~/Statics/Content.Master" AutoEventWireup="true" CodeBehind="ReportList.aspx.cs" Inherits="Tele.Management.Pages.Report.ReportList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Styles/formStyle.css" rel="stylesheet" />
    <link href="../../Styles/listStyle.css" rel="stylesheet" />
    <script>
        $(function () {
            $('.formTable tbody tr:even').addClass('item');
            $('.formTable tbody tr:odd').addClass('aitem');
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="navigator">
        <ul>
            <li>当前位置：</li>
            <li><a href="ReportList.aspx">话务报表</a></li>
        </ul>
        <div style="clear: both;"></div>
    </div>

    <div class="formDiv">
        <h3>话务报表</h3>
        <table class="formTable">
            <thead>
                <tr class="header">
                    <th>报表名称</th>
                    <th>查看</th>
                </tr>
            </thead>
            <tbody>
                <%
                    foreach (var item in list)
                    {
                %>
                <tr>
                    <td>
                        <%:item.ReportName %>
                    </td>
                    <td>
                        <a href="Detail.aspx?guid=<%:item.GUID %>" target="_blank">查看</a></td>
                </tr>
                <%
                    }
                %>
            </tbody>
        </table>
    </div>
</asp:Content>
