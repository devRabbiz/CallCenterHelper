<%@ Page Title="" Language="C#" MasterPageFile="~/Statics/Content.Master" AutoEventWireup="true" CodeBehind="agencyList.aspx.cs" Inherits="Tele.Management.Pages.Voice.ivr_sugg.agencyList" %>

<%@ Register Src="~/Statics/Pager.ascx" TagPrefix="uc1" TagName="Pager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/Styles/listStyle.css" rel="stylesheet" />
    <script>
        $(function () {
            $('.item:odd').addClass('aitem');
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="navigator">
        <ul>
            <li>当前位置：</li>
            <li><a href="agencyList.aspx">代理商信息列表</a></li>
        </ul>
        <div style="clear: both;"></div>
    </div>

    <div class="formDiv">
        <h3>查询条件</h3>
        <table class="searchTable">
            <tr>
                <td>代理商名称</td>
                <td>
                    <asp:TextBox ID="txtAGENCY_NAME" runat="server"></asp:TextBox></td>
                <td>产线</td>
                <td>
                    <asp:DropDownList ID="txtPRODUCT_TYPE" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>代理商编号</td>
                <td>
                    <asp:TextBox ID="txtAGENCY_ID" runat="server"></asp:TextBox></td>
                <td>区号</td>
                <td>
                    <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="4" align="right">
                    <asp:Button ID="btnSearch" runat="server" CssClass="btn100" Text="查询" OnClick="btnSearch_Click" />
                    &nbsp;&nbsp;
                    <input type="button" class="btn100" onclick="self.location = 'agencyView.aspx';"
                        value="新增" />
                    &nbsp;&nbsp;
                    <input type="button" class="btn100" onclick="self.location = '../../Public/FileUpload.aspx?docid=XltAgency';"
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
                        <th>代理商编号</th>
                        <th>产线</th>
                        <th>区号</th>
                        <th>代理商名称</th>
                        <th>地址</th>
                        <th>工作类型</th>
                        <th>排序</th>
                        <th>操作</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="item">
                    <td><%#Eval("AGENCY_ID") %></td>
                    <td><%#Eval("PRODUCT_TYPE") %></td>
                    <td><%#Eval("AGENCY_COVER_AREA") %></td>
                    <td><%#Eval("AGENCY_NAME") %></td>
                    <td><%#Eval("AGENCY_INFO") %></td>
                    <td><%#Eval("ADD_TYPE") %></td>
                    <td><%#Eval("CATEGORY") %></td>
                    <td>
                        <asp:LinkButton ID="btnDel" CommandName="Del" CommandArgument='<%#Eval("AGENCY_ID").ToString().Trim()+","+Eval("PRODUCT_TYPE") %>' runat="server" Text="删除" OnClientClick="return deleteConfirm();" />
                        &nbsp;&nbsp;
                            <a href='agencyView.aspx?id=<%#Eval("AGENCY_ID").ToString().Trim() %>,<%#Eval("PRODUCT_TYPE") %>'>编辑</a>
                    </td>
                </tr>
            </ItemTemplate>

            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>

        <uc1:Pager runat="server" ID="pager1" OnPageChanged="pager1_PageChanged" />
    </div>
</asp:Content>
