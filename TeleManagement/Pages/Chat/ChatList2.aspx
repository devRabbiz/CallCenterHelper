<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Statics/Content.Master" CodeBehind="ChatList.aspx.cs" Inherits="Tele.Management.Pages.ChatList" %>

<%@ Register Src="~/Statics/Pager.ascx" TagPrefix="uc1" TagName="Pager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="../../Scripts/globalScript.js"></script>
    <link href="../../Styles/listStyle.css" rel="stylesheet" />
    <script type="text/javascript">
        $(function () {
            $(".cbChatID").click(function () { changeIDs(); });
            $("#cbAll").click(function () {
                var checked = this.checked;
                $(".cbChatID").each(function () {
                    if (this.checked != checked) this.click();
                });
            });
        });

        function checkIDs() {
            var len = $(".cbChatID").filter(":checked").length;
            if (len == 0) {
                alert('请至少选择一条记录！');
                return false;
            }
            return true;
        }

        function viewHistory() {
            if (checkIDs()) {
                var ids = $("#ContentPlaceHolder1_hdChatIDs").val();
                var url = 'ChatHistory.aspx?ids=' + ids;
                showDialog(url, 700, 550);
            }
        }

        function changeIDs() {
            var values = '';
            $(".cbChatID").each(function () {
                if (this.checked)
                    values = values + this.value + ',';
            });
            $("#ContentPlaceHolder1_hdChatIDs").val(values);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="navigator">
        <ul>
            <li>当前位置：</li>
            <li><a href="ChatList.aspx">文本历史记录列表</a></li>
        </ul>
        <div style="clear: both;"></div>
    </div>
    <div class="formDiv">
        <h3>查询条件</h3>
        <table cellspacing="0" cellpadding="0" class="searchTable">
            <tr>
                <td class="colLable">开始日期：</td>
                <td>
                    <asp:TextBox ID="txtChatBeginTime" runat="server" CssClass="qqcalendar"></asp:TextBox>
                </td>
                <td class="colLable">截止日期：</td>
                <td>
                    <asp:TextBox ID="txtChatEndTime" runat="server" CssClass="qqcalendar"></asp:TextBox>
                </td>
                <td class="colLable"></td>
                <td></td>
            </tr>
            <tr>
                <td class="colLable">ChatID：</td>
                <td>
                    <asp:TextBox ID="txtChatID" runat="server"></asp:TextBox>

                </td>
                <td class="colLable">员工ID：</td>
                <td>
                    <asp:TextBox ID="txtEmployeeID" runat="server"></asp:TextBox>

                </td>
                <td class="colLable">坐席IP：</td>
                <td>
                    <asp:TextBox ID="txtPlaceIP" runat="server"></asp:TextBox>

                </td>
            </tr>
            <tr>
                <td class="colLable">队列名称：</td>
                <td>
                    <asp:TextBox ID="txtQueueName" runat="server"></asp:TextBox>

                </td>
                <td class="colLable">客户ID：</td>
                <td>
                    <asp:TextBox ID="txtCustomerID" runat="server"></asp:TextBox>

                </td>
                <td class="colLable">主机编号：</td>
                <td>
                    <asp:TextBox ID="txtMachineNo" runat="server"></asp:TextBox>

                </td>
            </tr>
            <tr>
                <td colspan="5"></td>
                <td>
                    <asp:Button ID="btnSearch" runat="server" CssClass="btn100" Text="查询" OnClick="btnSearch_Click" />
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
                        <th>
                            <input type="checkbox" id="cbAll" /><label for="cbAll">全选</label></th>
                        <th>客户ID</th>
                        <th>客户姓名</th>
                        <th>ChatID</th>
                        <th>开始时间</th>
                        <th>结束时间</th>
                        <th>主机编号</th>
                        <th>服务卡号</th>
                        <th>队列名称</th>
                        <th>坐席ID</th>
                        <th>坐席IP</th>
                        <th>客户邮箱</th>
                        <th>RTO</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="item">
                    <td>
                        <input class="cbChatID" type="checkbox" value='<%#Eval("ChatID")%>' /></td>
                    <td><%#Eval("CustomerID")%></td>
                    <td><%#Eval("CustomerName")%></td>
                    <td><%#Eval("ChatID")%></td>
                    <td><%#Eval("ChatBeginTime")%></td>
                    <td><%#Eval("ChatEndTime")%></td>
                    <td><%#Eval("MachineNo")%></td>
                    <td><%#Eval("ServicecardNo")%></td>
                    <td><%#Eval("CurrentQueueName")%></td>
                    <td><%#Eval("EmployeeID")%></td>
                    <td><%#Eval("PlaceIP")%></td>
                    <td><%#Eval("MailAddress") %></td>
                    <td><%# (Eval("IsRTO").ToString() == "1") ? "是" : "否" %></td>
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr class="aitem">
                    <td>
                        <input class="cbChatID" type="checkbox" value='<%#Eval("ChatID")%>' /></td>
                    <td><%#Eval("CustomerID")%></td>
                    <td><%#Eval("CustomerName")%></td>
                    <td><%#Eval("ChatID")%></td>
                    <td><%#Eval("ChatBeginTime")%></td>
                    <td><%#Eval("ChatEndTime")%></td>
                    <td><%#Eval("MachineNo")%></td>
                    <td><%#Eval("ServicecardNo")%></td>
                    <td><%#Eval("CurrentQueueName")%></td>
                    <td><%#Eval("EmployeeID")%></td>
                    <td><%#Eval("PlaceIP")%></td>
                    <td><%#Eval("MailAddress") %></td>
                    <td><%# (Eval("IsRTO").ToString() == "1") ? "是" : "否" %></td>
                </tr>
            </AlternatingItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
        <uc1:Pager runat="server" ID="pager1" OnPageChanged="pager1_OnPageChanged" />
        <div>
            <asp:HiddenField ID="hdChatIDs" runat="server" />
            <input type="button" class="btn100" onclick="viewHistory();" value="查看选中" />
            &nbsp;&nbsp;
            <asp:Button ID="btnExport" CssClass="btn100" Text="导出选中" runat="server" OnClientClick="return checkIDs();" OnClick="btnExport_Click" />
            &nbsp;&nbsp;
            <asp:Button ID="btnExportAll" CssClass="btn100" Text="导出所有" runat="server" OnClick="btnExportAll_Click" />
        </div>

    </div>

</asp:Content>
