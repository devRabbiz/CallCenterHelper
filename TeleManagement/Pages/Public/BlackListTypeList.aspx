<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/Statics/Content.Master"
    CodeBehind="BlackListTypeList.aspx.cs" Inherits="Tele.Management.Pages.BlackListTypeList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Styles/listStyle.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="navigator">
        <ul>
            <li>当前位置：</li>
            <li><a href="BlackListTypeList.aspx">BlackListType列表</a></li>
        </ul>
        <div style="clear: both;"></div>
    </div>
    <div class="formDiv">
        <h3>查询条件</h3>
        <table cellspacing="0" cellpadding="0" class="searchTable">
            <tr>
                <td class="colLable">黑名单类型：</td>
                <td>
                    <asp:TextBox ID="txtTypeName" runat="server"></asp:TextBox>

                </td>
                <td colspan="2"></td>
                <td colspan="2">
                    <asp:Button ID="btnSearch" runat="server" CssClass="btn100" Text="查询" OnClick="btnSearch_Click" />
                    &nbsp;&nbsp;
                    <input type="button" class="btn100" onclick="self.location = 'BlackListTypeView.aspx';"
                        value="新增" />
                </td>
            </tr>
        </table>
    </div>
    <div class="formDiv">
        <h3>数据列表</h3>
        <asp:Repeater ID="rptList" runat="server">
            <HeaderTemplate>
                <table class="formTable">
                    <tr class="header">
                        <th>黑名单类型</th>
                        <th>创建人</th>
                        <th>创建时间</th>
                        <th>修改人</th>
                        <th>修改时间</th>
                        <th>操作</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="item">
                    <td><%#Eval("TypeName")%></td>
                    <td><%#Eval("CreateBy")%></td>
                    <td><%#Eval("CreateTime")%></td>
                    <td><%#Eval("UpdateBy")%></td>
                    <td><%#Eval("UpdateTime")%></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</asp:Content>
