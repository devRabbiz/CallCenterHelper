<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/Statics/Content.Master"
    CodeBehind="HolidayView.aspx.cs" Inherits="Tele.Management.Pages.HolidayView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../../Scripts/jquery-ui-1.10.1.js"></script>
    <script src="../../Scripts/jquery.multiselect.js"></script>
    <script src="../../Scripts/jquery.multiselect.filter.js"></script>
    <link href="../../Styles/formStyle.css" rel="stylesheet" />
    <link href="../../Styles/themes/base/jquery.ui.all.css" rel="stylesheet" />
    <link href="../../Styles/themes/jquery.multiselect.css" rel="stylesheet" />
    <link href="../../Styles/themes/jquery.multiselect.filter.css" rel="stylesheet" />
    <script>
        $(function () {
            var multiple = false;
            var selectedList = 3;
            multiple = $('select[id*="listSkills"]').prop('multiple');
            if (multiple) {
                selectedList = 1;
            }
            $('select[id*="listSkills"]')
                    .multiselect({
                        multiple: multiple,
                        selectedList: selectedList,
                        minWidth: 412,
                        height: 300,
                        checkAllText: '全选',
                        uncheckAllText: '取消选中',
                        noneSelectedText: '选择队列',
                        selectedText: '# 个队列选中',
                        position: {
                            my: 'left bottom',
                            at: 'left top'
                        }
                    })
                    .multiselectfilter({
                        label: '筛选',
                        placeholder: '请输入关键字',
                        width: 200
                    });

        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="navigator">
        <ul>
            <li>当前位置：</li>
            <li><a href="HolidayList.aspx">工作时间配置列表</a></li>
            <li>-</li>
            <li>
                <asp:Literal ID="ltlCurrentPageType" runat="server"></asp:Literal></li>
        </ul>
        <div style="clear: both;"></div>
    </div>
    <div class="formDiv">
        <h3>表单元素</h3>
        <table class="searchTable">
            <tr>
                <td class="colLable">类型：<span>*</span></td>
                <td>
                    <asp:DropDownList ID="ddlType" runat="server">
                        <asp:ListItem Text="语音" Value="1"></asp:ListItem>
                        <asp:ListItem Text="文本" Value="2"></asp:ListItem>
                    </asp:DropDownList></td>
                <td></td>
            </tr>
            <tr>
                <td class="colLable">假期规则<span>*</span></td>
                <td>
                    <asp:DropDownList ID="ddlHolidayRole" runat="server">
                    </asp:DropDownList>
                </td>
                <td></td>
            </tr>
            <tr>
                <td class="colLable">号码</td>
                <td>
                    <asp:TextBox ID="txtDNIS" runat="server" TextMode="MultiLine" Height="200" Width="400"></asp:TextBox>
                    <div>
                        多个号码使用逗号分开
                    </div>
                </td>
                <td></td>
            </tr>
            <tr>
                <td class="colLable">IVR队列<span>*</span></td>
                <td>
                    <asp:HiddenField ID="hdQueueName" runat="server" />

                    <asp:ListBox ID="listSkills" runat="server" AppendDataBoundItems="true" DataTextField="name" Height="80" SelectionMode="Multiple">
                    </asp:ListBox>
                </td>
                <td></td>
            </tr>

            <tr>
                <td></td>
                <td>
                    <asp:Button ID="btnSave" runat="server" CssClass="btn100" Text="保存" OnClick="btnSave_Click" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
