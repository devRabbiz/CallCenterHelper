<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Left.aspx.cs" Inherits="Tele.Management.Statics.Left" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>导航菜单</title>
    <link href="../Styles/globalStyle.css" rel="stylesheet" />
    <script src="../Scripts/jquery-1.7.1.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $(".menu>ul>li>a").click(function () {
                var id = this.id;
                reqLink(id, true);
            });
            reqLink(null, false);

            $('[class*="act"]').hide();

            $('[class*="nav"]').each(function (i, e) {
                $(e).css('cursor', 'pointer');
                $(e).click(function () {
                    var s = $(this).attr('class').substr(3);
                    $('.act' + s).toggle();
                });
            });

            $('.nav1').click();
            $('.nav2').click();
        });
        //默认打开第一个链接
        function reqLink(id, needRefresh) {
            var as = $(".menu>ul>li>a");
            if (as && as.length > 0) {
                $(as).each(function () { $(this).parent().removeClass("menu_sel"); });
                if (!id) {
                    var lk = $(".menu>ul>li>a:first");
                    if (lk && lk[0]) id = lk[0].id;
                }
            }
            if (!id) id = 1;
            var a = $(".menu>ul>li>a[id=" + id + "]");
            if (a) {
                var target = a.attr("target")
                if (!target) target = "Content";
                if (!needRefresh) {
                    try {
                        window.top.frames[target].location.href = a.attr("href");
                    }
                    catch (e) { }
                }
                a.parent().addClass("menu_sel");
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="divMenu" runat="server" class="menu">
        </div>
    </form>
</body>
</html>
