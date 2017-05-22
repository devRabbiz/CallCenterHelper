<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/Statics/Content.Master"
    CodeBehind="RoleUsers.aspx.cs" Inherits="Tele.Management.Pages.RoleUsers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../../Scripts/globalScript.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery.tree.js" type="text/javascript"></script>
    <link href="../../Styles/formStyle.css" rel="stylesheet" />
    <link href="../../Styles/jquery.tree.css" rel="stylesheet" />

    <script src="../../Scripts/jquery-ui-1.10.1.js"></script>
    <script src="../../Scripts/jquery.multiselect.js"></script>
    <script src="../../Scripts/jquery.multiselect.filter.js"></script>

    <link href="../../Styles/themes/base/jquery.ui.all.css" rel="stylesheet" />
    <link href="../../Styles/themes/jquery.multiselect.css" rel="stylesheet" />
    <link href="../../Styles/themes/jquery.multiselect.filter.css" rel="stylesheet" />


    <script type="text/javascript">
        var sel;
        $(document).ready(function () {
            var multiple = false;
            var selectedList = 4;
            multiple = $('select[id*="listUsers"]').prop('multiple');
            if (multiple) {
                selectedList = 1;
            }
            $('select[id*="listUsers"]')
                    .multiselect({
                        multiple: multiple,
                        selectedList: selectedList,
                        minWidth: 412,
                        height: 300,
                        checkAllText: '全选',
                        uncheckAllText: '取消选中',
                        noneSelectedText: '选择用户',
                        selectedText: '# 个用户选中',
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
    <div>
        <input id="hdTree" type="hidden" value="" runat="server" />
        <input id="hdSelectedOrg" type="hidden" runat="server" />
    </div>
    <div class="navigator">
        <ul>
            <li>当前位置：</li>
            <li><a href="RoleList.aspx">角色列表</a></li>
            <li>-</li>
            <li>角色的用户列表</li>
        </ul>
        <div style="clear: both;"></div>
    </div>
    <div class="formDiv">
        <h3>查询条件</h3>
        <div style="display: none">
            <table cellspacing="0" cellpadding="0" class="searchTable">
                <tr>
                    <td class="colLable">用户ITCode或姓名：</td>
                    <td>
                        <asp:TextBox ID="txtUserID" runat="server"></asp:TextBox>
                    </td>
                    <td colspan="2"></td>
                    <td colspan="2">
                        <asp:Button ID="btnSearch" runat="server" CssClass="btn100" Text="查询" Visible="false" OnClick="btnSearch_Click" />
                        &nbsp;&nbsp;
                    </td>
                </tr>
            </table>
        </div>
    <asp:Button ID="btnSave" runat="server" CssClass="btn100" Text="保存用户" OnClick="btnSave_Click" />
    </div>
    <div class="formDiv">
        <h3>表单元素</h3>
        <table class="searchTable">
            <tr>
                <td class="colLable">角色名称<span>*</span></td>
                <td>
                    <asp:DropDownList ID="ddlRole" runat="server"></asp:DropDownList>
                </td>
                <td></td>
            </tr>
            <tr>
                <td class="colLable">用户列表<span>*</span></td>
                <td>
                    <div id="tree">
                    </div>
                    <asp:ListBox ID="listUsers" runat="server" SelectionMode="Multiple"></asp:ListBox>
                </td>
                <td></td>
            </tr>
        </table>
    </div>
</asp:Content>
