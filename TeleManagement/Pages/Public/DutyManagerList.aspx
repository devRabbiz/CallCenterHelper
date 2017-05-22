<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/Statics/Content.Master"
    CodeBehind="DutyManagerList.aspx.cs" Inherits="Tele.Management.Pages.DutyManagerList" %>
<%@ Register Src="~/Statics/Pager.ascx" TagPrefix="uc1" TagName="Pager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Styles/listStyle.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="navigator">
        <ul>
            <li>当前位置：</li>
            <li><a href="DutyManagerList.aspx">值班经理维护列表</a></li>
        </ul>
        <div style="clear: both;"></div>
    </div>
    <div class="formDiv">
        <h3>查询条件</h3>
        <table cellspacing="0" cellpadding="0" class="searchTable">
            <tr>
                <td class="colLable">姓名：</td>
                <td>
                    <asp:TextBox ID="txtManagerName" runat="server"></asp:TextBox>

                </td>
                <td class="colLable">电话：</td>
                <td>
                    <asp:TextBox ID="txtPhoneNo" runat="server"></asp:TextBox>

                </td>
                <td class="colLable">值班日期：</td>
                <td>
                    <asp:TextBox ID="txtBeginDate" runat="server" CssClass="qqcalendar"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="4"></td>
                <td colspan="2">
                    <asp:Button ID="btnSearch" runat="server" CssClass="btn100" Text="查询" OnClick="btnSearch_Click" />
                    &nbsp;&nbsp;
                    <input type="button" class="btn100" onclick="self.location = 'DutyManagerView.aspx';"
                        value="新增" />
                    &nbsp;&nbsp;
                    <input type="button" class="btn100" onclick="self.location = 'FileUpload.aspx?docid=Xlt002';"
                        value="导入" />
                </td>
            </tr>
        </table>
    </div>
    <div class="formDiv">
        <h3>数据列表</h3>
        <asp:Repeater ID="rptList" runat="server" OnItemCommand="rptList_ItemCommand">
            <HeaderTemplate>
                <table class="formTable">
                    <tr class="header">
                        <th>姓名</th>
                        <th>电话</th>
                        <th>值班日期</th>
                        <th>操作</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="item">
                    <td><%#Eval("ManagerName")%></td>
                    <td><%#Eval("PhoneNo")%></td>
                    <td align="center"><%#Eval("DutyDate","{0:yyyy-MM-dd}")%></td>
                    <td align="center">
                        <asp:LinkButton ID="btnStatus" OnClientClick="return deleteConfirm();" CommandName="Status" CommandArgument='<%#Eval("DutyManagerID") %>' runat="server" Text="删除" />
                        &nbsp;&nbsp;
                            <a href='DutyManagerView.aspx?id=<%#Eval("DutyManagerID") %>'>编辑</a></td>
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr class="aitem">
                    <td><%#Eval("ManagerName")%></td>
                    <td><%#Eval("PhoneNo")%></td>
                    <td align="center"><%#Eval("DutyDate","{0:yyyy-MM-dd}")%></td>
                    <td align="center">
                        <asp:LinkButton ID="btnStatus" OnClientClick="return deleteConfirm();" CommandName="Status" CommandArgument='<%#Eval("DutyManagerID") %>' runat="server" Text="删除" />
                        &nbsp;&nbsp;
                            <a href='DutyManagerView.aspx?id=<%#Eval("DutyManagerID") %>'>编辑</a></td>
                </tr>
            </AlternatingItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
        <uc1:Pager runat="server" ID="pager1" OnPageChanged="pager1_OnPageChanged" />
    </div>
</asp:Content>
