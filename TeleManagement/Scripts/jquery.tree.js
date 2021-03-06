/// <reference path="../intellisense/jquery-1.2.6-vsdoc-cn.js" />
/****************************************
data:[{
id:1, //ID只能包含英文数字下划线中划线
text:"node 1",
value:"1",
showcheck:false,
checkstate:0,         //0,1,2
hc:true,
isexpand:false,
cp:false, 是否已加载子节点
cn:[] // child nodes
},
..........
]
author:xuanye.wan@gmail.com
***************************************/
(function ($) {
    $.fn.swapClass = function (c1, c2) {
        return this.removeClass(c1).addClass(c2);
    };
    $.fn.switchClass = function (c1, c2) {
        if (this.hasClass(c1)) {
            return this.swapClass(c1, c2);
        }
        else {
            return this.swapClass(c2, c1);
        }
    };
    $.fn.treeview = function (settings) {
        var lastId = -1;
        var dfop =
            {
                method: "POST",
                datatype: "json",
                url: false,
                cbiconpath: "/Styles/tree/",
                icons: ["cb_0.gif", "cb_1.gif", "cb_2.gif"],
                emptyiconpath: "../Styles/tree/s.gif",
                showcheck: false, //是否显示选择            
                oncheckboxclick: false, //当checkstate状态变化时所触发的事件，但是不会触发因级联选择而引起的变化
                onnodeclick: false,
                cascadecheck: true,
                data: null,
                clicktoggle: true, //点击节点展开和收缩子节点
                theme: "bbit-tree-lines", //bbit-tree-lines ,bbit-tree-no-lines,bbit-tree-arrows,
                property: -1,
                pngicon: true,
                menubackcolor: '#FFFFFF',
                version: 20, //.net version
                scrollwidth: 0,
                width: 225,
                makebold: true
            };

        $.extend(dfop, settings);
        var treenodes = dfop.data;
        var me = $(this);
        var id = me.attr("id");
        if (id == null || id == "") {
            id = "bbtree" + new Date().getTime();
            me.attr("id", id);
        }

        var bgImage = "";
        var lastScrollHeight = -1;
        var html = [];
        buildtree(dfop.data, html);
        me.addClass("bbit-tree").html(html.join(""));
        InitEvent(me);
        html = null;
        //预加载图片
        if (dfop.showcheck) {
            for (var i = 0; i < 3; i++) {
                var im = new Image();
                im.src = dfop.cbiconpath + dfop.icons[i];
            }
        }

        //region 
        function buildtree(data, ht) {
            ht.push("<div class='bbit-tree-bwrap'>"); // Wrap ;
            ht.push("<div class='bbit-tree-body' >"); // body ;
            ht.push("<ul class='bbit-tree-root ", dfop.theme, "'>"); //root
            if (data && data.length > 0) {
                var l = data.length;
                for (var i = 0; i < l; i++) {
                    buildnode(data[i], ht, 0, i, i == l - 1);
                }
            }
            else {
                asnyloadc(null, false, function (data) {
                    if (data && data.length > 0) {
                        treenodes = data;
                        dfop.data = data;
                        var l = data.length;
                        for (var i = 0; i < l; i++) {
                            buildnode(data[i], ht, 0, i, i == l - 1);
                        }
                    }
                });
            }
            ht.push("</ul>"); // root and;
            ht.push("</div>"); // body end;
            ht.push("</div>"); // Wrap end;
        }
        //endregion
        function buildnode(nd, ht, deep, path, isend) {
            if (!nd) return;
            var nid = nd.d.toString().replace(/[^\w]/gi, "_");
            ht.push("<li  class='bbit-tree-node'>");
            ht.push("<div style='width: " + (dfop.width - dfop.scrollwidth).toString() + "px;overflow: hidden;white-space: nowrap;-o-text-overflow: ellipsis;text-overflow: ellipsis;color:black;margin:0px;background:transparent;' id='", id, "_", nid, "' tpath='", path, "' unselectable='on' title='", nd.t, "'");

            var cs = [];
            cs.push("bbit-tree-node-el");
            if (nd.h) {
                cs.push(nd.isexpand ? "bbit-tree-node-expanded" : "bbit-tree-node-collapsed");
            }
            else {
                cs.push("bbit-tree-node-leaf");
            }
            if (nd.classes) { cs.push(nd.classes); }

            ht.push(" class='", cs.join(" "), "'>");

            //span indent
            ht.push("<span class='bbit-tree-node-indent'>");

            if (deep == 1) {
                ht.push("<img  class='bbit-tree-icon'  src='", dfop.emptyiconpath, "'/>");
            }
            else if (deep > 1) {
                ht.push("<img class='bbit-tree-icon' src='", dfop.emptyiconpath, "'/>");
                for (var j = 1; j < deep; j++) {
                    ht.push("<img class='bbit-tree-elbow-line' src='", dfop.emptyiconpath, "'/>");
                }
            }
            ht.push("</span>");
            //img
            cs.length = 0;
            if (nd.h) {
                if (nd.isexpand) {
                    cs.push(isend ? "bbit-tree-elbow-end-minus" : "bbit-tree-elbow-minus");
                }
                else {
                    cs.push(isend ? "bbit-tree-elbow-end-plus" : "bbit-tree-elbow-plus");
                }
            }
            else {
                cs.push(isend ? "bbit-tree-elbow-end" : "bbit-tree-elbow");
            }
            ht.push("<img class='bbit-tree-ec-icon ", cs.join(" "), "' src='", dfop.emptyiconpath, "'/>");

            var nodeImageCss = "class='bbit-tree-node-icon' ";
            ht.push("<img " + nodeImageCss + " src='", dfop.emptyiconpath, "'/>");
            //checkbox
            if (dfop.showcheck && nd.c) {
                if (nd.cs == null || nd.cs == undefined) {
                    nd.cs = 0;
                }
                ht.push("<img  id='", id, "_", nid, "_cb' class='bbit-tree-node-cb' src='", dfop.cbiconpath, dfop.icons[nd.cs], "'/>");
            }
            //a
            ht.push("<a hideFocus class='bbit-tree-node-anchor' tabIndex=1 href='javascript:void(0);'>");
            ht.push("<span   unselectable='on'>", nd.t, "</span>");
            ht.push("</a>");
            ht.push("</div>");
            //Child
            if (nd.h) {
                if (nd.isexpand) {
                    ht.push("<ul  class='bbit-tree-node-ct'  style='z-index: 0; position: static; visibility: visible; top: auto; left: auto;'>");
                    if (nd.n) {
                        var l = nd.n.length;
                        for (var k = 0; k < l; k++) {
                            if (!nd.n[k]) continue;
                            nd.n[k].parent = nd;
                            buildnode(nd.n[k], ht, deep + 1, path + "." + k, k == l - 1);
                        }
                    }
                    ht.push("</ul>");
                }
                else {
                    ht.push("<ul style='display:none;'></ul>");
                }
            }
            ht.push("</li>");
            nd.render = true;
        }
        function getItem(path) {
            var ap = path.split(".");

            var t = treenodes;
            for (var i = 0; i < ap.length; i++) {
                if (i == 0) {
                    t = t[ap[i]];
                }
                else {
                    t = t.n[ap[i]];
                }
            }

            return t;
        }
        function check(item, state, type) {
            if (!item) return;
            var pstate = item.cs;
            if (type == 1) {
                item.cs = state;
            }
            else {// 上溯
                var cs = item.n;
                var l = cs.length;
                var ch = true;
                for (var i = 0; i < l; i++) {
                    if (!cs[i]) break;
                    if ((state == 1 && cs[i].cs != 1) || state == 0 && cs[i].cs != 0) {
                        ch = false;
                        break;
                    }
                }
                if (ch) {
                    item.cs = state;
                }
                else {
                    item.cs = 2;
                }
            }
            //change show
            if (item.render && pstate != item.cs) {
                var nid = item.d.toString().replace(/[^\w]/gi, "_");
                var et = $("#" + id + "_" + nid + "_cb");
                if (et.length == 1) {
                    et.attr("src", dfop.cbiconpath + dfop.icons[item.cs]);
                }
            }
        }
        //遍历子节点
        function cascade(fn, item, args) {
            if (fn(item, args, 1) != false) {
                if (item.n != null && item.n.length > 0) {
                    var cs = item.n;
                    for (var i = 0, len = cs.length; i < len; i++) {
                        if (!cs[i]) continue;
                        cascade(fn, cs[i], args);
                    }
                }
            }
        }
        //冒泡的祖先
        function bubble(fn, item, args) {
            var p = item.parent;
            while (p) {
                if (fn(p, args, 0) === false) {
                    break;
                }
                p = p.parent;
            }
        }
        function nodeclick(e) {
            var path = $(this).attr("tpath");
            var et = e.target || e.srcElement;
            var item = getItem(path);

            var action = -1;

            if (et.tagName == "IMG") {
                // +号需要展开
                if ($(et).hasClass("bbit-tree-elbow-plus") || $(et).hasClass("bbit-tree-elbow-end-plus")) {

                    var ul = $(this).next(); //"bbit-tree-node-ct"

                    if (ul.hasClass("bbit-tree-node-ct")) {
                        ul.show();
                    }
                    else {
                        var deep = path.split(".").length;
                        if (item.n == null)//显示文件夹但没有真实子节点
                        {
                            return;
                        }

                        if (item.p) {
                            item.n != null && asnybuild(item.n, deep, path, ul, item);

                        }
                        else {
                            $(this).addClass("bbit-tree-node-loading");
                            asnyloadc(item, true, function (data) {
                                item.p = true;
                                item.n = data;
                                asnybuild(data, deep, path, ul, item);
                            });
                        }
                    }
                    if ($(et).hasClass("bbit-tree-elbow-plus")) {
                        $(et).swapClass("bbit-tree-elbow-plus", "bbit-tree-elbow-minus");
                    }
                    else {
                        $(et).swapClass("bbit-tree-elbow-end-plus", "bbit-tree-elbow-end-minus");
                    }
                    $(this).swapClass("bbit-tree-node-collapsed", "bbit-tree-node-expanded");

                    action = 1;
                    item.isexpand = true;
                }
                else if ($(et).hasClass("bbit-tree-elbow-minus") || $(et).hasClass("bbit-tree-elbow-end-minus")) {  //- 号需要收缩                    
                    $(this).next().hide();
                    if ($(et).hasClass("bbit-tree-elbow-minus")) {
                        $(et).swapClass("bbit-tree-elbow-minus", "bbit-tree-elbow-plus");
                    }
                    else {
                        $(et).swapClass("bbit-tree-elbow-end-minus", "bbit-tree-elbow-end-plus");
                    }
                    $(this).swapClass("bbit-tree-node-expanded", "bbit-tree-node-collapsed");

                    action = 2;
                    item.isexpand = false;
                }
                else if ($(et).hasClass("bbit-tree-node-cb")) // 点击了Checkbox
                {
                    var s = item.cs != 1 ? 1 : 0;
                    if (item.cc != undefined && item.cc == 1)//取消单击操作
                    {
                        s = 0;
                    }

                    var r = true;
                    if (dfop.oncheckboxclick) {
                        r = dfop.oncheckboxclick.call(et, item, s);
                    }
                    if (r != false) {
                        if (dfop.cascadecheck) {
                            //遍历
                            cascade(check, item, s);
                            //上溯
                            bubble(check, item, s);
                        }
                        else {
                            check(item, s, 1);
                        }
                    }
                }
            }
            else {
                if (dfop.citem) {
                    var nid = dfop.citem.d.toString().replace(/[^\w]/gi, "_");
                    $("#" + id + "_" + nid).removeClass("bbit-tree-selected");
                }
                dfop.citem = item;
                $(this).addClass("bbit-tree-selected");

                if (dfop.onnodeclick) {
                    if (dfop.makebold) {
                        if (lastId.toString() != "-1") {
                            $("#" + lastId).css("text-decoration", "none");
                            $("#" + lastId).css("font-weight", "normal");
                        }
                        $(this).css("text-decoration", "underline");
                        $(this).css("font-weight", "bold");

                        lastId = $(this).attr('id');
                    }
                    if (!item.expand) {
                        item.expand = function () { expandnode.call(item); };
                    }
                    dfop.onnodeclick.call(this, item, dfop.property, id, dfop.version);
                }
            }

            //展开时，滚动条自动定位
            var curScrollHeight = $("#" + id).parent()[0].scrollHeight;
            var menuHeight = $(this).css("height").toLowerCase().replace("px", "");
            var treeHeight = $("#" + id).parent().css("height").toLowerCase().replace("px", "");
            switch (action) {
                case 1:
                    if (lastScrollHeight == -1) {
                        lastScrollHeight = curScrollHeight;
                    }
                    else if (lastScrollHeight < curScrollHeight) {
                        offset = curScrollHeight - lastScrollHeight;
                        curTop = $(this)[0].offsetTop - $("#" + id).parent().scrollTop(); //当前元素相对垂直位置
                        if (curTop >= offset && offset > treeHeight - curTop - menuHeight) {
                            $("#" + id).parent().scrollTop($("#" + id).parent().scrollTop() + (offset - (treeHeight - curTop - menuHeight)));
                        }
                        lastScrollHeight = curScrollHeight;
                    }
                    else {
                        lastScrollHeight = curScrollHeight;
                    }
                    break;
                case 2:
                    lastScrollHeight = curScrollHeight;
                    break;
                default:
                    break;
            }
            return false;
        }
        function expandnode() {
            var item = this;
            var nid = item.d.toString().replace(/[^\w]/gi, "_");
            var img = $("#" + id + "_" + nid + " img.bbit-tree-ec-icon");
            if (img.length > 0) {
                img.click();
            }
        }
        function asnybuild(nodes, deep, path, ul, pnode) {
            var l = nodes.length;
            if (l > 0) {
                var ht = [];
                for (var i = 0; i < l; i++) {
                    if (!nodes[i]) continue;
                    nodes[i].parent = pnode;
                    buildnode(nodes[i], ht, deep, path + "." + i, i == l - 1);
                }
                ul.html(ht.join(""));
                ht = null;
                InitEvent(ul);
            }
            ul.addClass("bbit-tree-node-ct").css({ "z-index": 0, position: "static", visibility: "visible", top: "auto", left: "auto", display: "" });
            ul.prev().removeClass("bbit-tree-node-loading");
        }
        function asnyloadc(pnode, isAsync, callback) {
            if (dfop.url) {
                if (pnode && pnode != null)
                    var param = builparam(pnode);
                $.ajax({
                    type: dfop.method,
                    url: dfop.url,
                    data: param,
                    async: isAsync,
                    dataType: dfop.datatype,
                    success: callback,
                    error: callback
                });
            }
        }
        function builparam(node) {
            var p = [{ name: "id", value: encodeURIComponent(node.d.toString()) }
                    , { name: "text", value: encodeURIComponent(node.t) }
            //, { name: "value", value: encodeURIComponent(node.v) }
                    , { name: "checkstate", value: node.cs}];
            return p;
        }
        function bindevent() {
            $(this).click(nodeclick);
        }
        function getNodes(items, s) {
            $.each(items, function () {
                var item = $(this)[0];
                if (item.n != null && item.n.length > 0) {
                    getNodes(item.n, s);
                }
                if (!item.h || item.h == undefined) {
                    s.push(item.v + "," + item.cs);
                }
            });
        }
        function InitEvent(parent) {
            var nodes = $("li.bbit-tree-node>div", parent);
            nodes.each(bindevent);
        }
        function BindEvent(ul) {
            var nodes = $("div", ul);
            alert(nodes.length);
            nodes.each(bindevent);
        }
        function GetItemByPath(path) {
            var item = getItem(path);
            if (item) {
                return item;
            }
            return null;
        }
        function refreshExt(itemId, moduleID) {
            var nid = itemId.replace(/[^\w]/gi, "_");
            var node = $("#" + id + "_" + nid);
            if (node.length > 0) {
                var isend = node.hasClass("bbit-tree-elbow-end") || node.hasClass("bbit-tree-elbow-end-plus") || node.hasClass("bbit-tree-elbow-end-minus");
                var path = node.attr("tpath");
                var deep = path.split(".").length;
                var item = getItem(path);
                if (item) {
                    asnyloadc(item, true, function (data) {
                        if (!data || data.readyState != 4 || data.status != 200) return;
                        if ($.trim(data) == "") {
                            $(node).parent().children("ul").remove();
                            item.n = null;
                            $(node).children("img").click();
                        }
                        else {
                            data = data.responseText;
                            data = eval("(" + data.replace(/!/g, '\'') + ")");
                            item.p = true;
                            item.n = data;
                            item.isexpand = true;
                            item.h = (data && data.length > 0);
                            var ht = [];
                            buildnode(item, ht, deep - 1, path, isend);
                            ht.shift();
                            ht.pop();
                            var li = node.parent();
                            li.html(ht.join(""));
                            ht = null;
                            InitEvent(li);
                            bindevent.call(li.find(">div"));
                        }
                        if (moduleID) {
                            moduleID = moduleID.replace(/[^\w]/gi, "_");
                            nid = "tree_" + moduleID.toString();
                        }
                        $("#" + nid).css("font-weight", "bold");
                    });
                }
            }
            else {
                alert("该节点还没有生成");
            }
        }

        function getck(items, c, fn) {
            for (var i = 0, l = items.length; i < l; i++) {
                if (!items[i]) continue;
                (items[i].c == true && items[i].cs == 1) && c.push(fn(items[i]));
                if (items[i].n != null && items[i].n.length > 0) {
                    getck(items[i].n, c, fn);
                }
            }
        }
        function getckOC(items, c, fn) {
            for (var i = 0, l = items.length; i < l; i++) {
                if (!items[i]) continue;
                if (items[i].n != null && items[i].n.length > 0) {
                    getck(items[i].n, c, fn);
                    continue;
                }

                (items[i].c == true && items[i].cs == 1) && c.push(fn(items[i]));
            }
        }
        function getCkAndHalfCk(items, c, fn) {
            for (var i = 0, l = items.length; i < l; i++) {
                if (!items[i]) continue;
                (items[i].c == true && (items[i].cs == 1 || items[i].cs == 2)) && c.push(fn(items[i]));
                if (items[i].n != null && items[i].n.length > 0) {
                    getCkAndHalfCk(items[i].n, c, fn);
                }
            }
        }
        me[0].t = {
            getNodes: function () {
                var s = [];
                getNodes(treenodes, s);

                var ret = s.join(';');
                s = null;
                return ret;
            },
            getSelectedNodes: function (gethalfchecknode) {
                var s = [];
                if (gethalfchecknode) {
                    getCkAndHalfCk(treenodes, s, function (item) { return item; });
                }
                else {
                    getck(treenodes, s, function (item) { return item; });
                }
                return s;
            },
            getSelectedValues: function () {
                var s = [];
                getck(treenodes, s, function (item) { return item.v; });
                return s;
            },
            getSelectedValuesOC: function () {
                var s = [];
                getckOC(treenodes, s, function (item) { return item.v; });
                return s;
            },
            getCurrentItem: function () {
                return dfop.citem;
            },
            reflash: function (itemOrItemId) {
                var id;
                if (typeof (itemOrItemId) == "string") {
                    id = itemOrItemId;
                }
                else {
                    id = itemOrItemId.d.toString();
                }
                reflash(id);
            },
            refreshExt: function (itemOrItemId, moduleID, doWhat) {
                var id;
                if (typeof (itemOrItemId) == "string") {
                    id = itemOrItemId;
                }
                else {
                    id = itemOrItemId.d.toString();
                }
                refreshExt(id, moduleID, doWhat);
            },
            BindEvent: function (ul) {
                return BindEvent(ul);
            },
            GetItemByPath: function (id) {
                return GetItemByPath(id);
            }
        };
        return me;
    };
    //获取所有子节点的Value
    $.fn.getNodes = function () {
        if (this[0].t) {
            return this[0].t.getNodes();
        }
        return null;
    };
    //获取所有选中的节点的Value数组
    $.fn.getTSVs = function () {
        if (this[0].t) {
            return this[0].t.getSelectedValues();
        }
        return null;
    };
    //获取所有选中的节点的Value数组(父节点除外)
    $.fn.getTSVsOC = function () {
        if (this[0].t) {
            return this[0].t.getSelectedValuesOC();
        }
        return null;
    };
    //获取所有选中的节点的Item数组
    $.fn.getTSNs = function (gethalfchecknode) {
        if (this[0].t) {
            return this[0].t.getSelectedNodes(gethalfchecknode);
        }
        return null;
    };
    //获取所有选中的节点的Item数组
    $.fn.getTSVsAll = function () {
        if (this[0].t) {
            var s = [];
            var objs = this[0].t.getSelectedNodes(1);
            for (var i = 0, l = objs.length; i < l; i++) {
                if (!objs[i]) continue;
                s.push(objs[i].v + '_' + objs[i].cs)
            }
            return s;
        }
        return null;
    };
    $.fn.getTCT = function () {
        if (this[0].t) {
            return this[0].t.getCurrentItem();
        }
        return null;
    };
    $.fn.refreshExt = function (ItemOrItemId, moduleID, doWhat) {
        if (this[0].t) {
            return this[0].t.refreshExt(ItemOrItemId, moduleID, doWhat);
        }
    };
    $.fn.BindEvent = function (ul) {
        if (this[0].t) {
            return this[0].t.BindEvent(ul);
        }
    };
    $.fn.GetItemByPath = function (id) {
        if (this[0].t) {
            return this[0].t.GetItemByPath(id);
        }
    };
})(jQuery);
