
(function ($, undefined) {
    if (window.jqeditor) return false;
    var index = 0, bShowPanel = false, bookmark;
    var tools = { FontName: { t: '字体', h: 1 }, FontSize: { t: '字体大小', h: 1 }, Bold: { t: '加粗 (Ctrl+B)', s: 'Ctrl+B' }, Italic: { t: '斜体 (Ctrl+I)', s: 'Ctrl+I' }, Underline: { t: '下划线 (Ctrl+U)', s: 'Ctrl+U' }, FontColor: { t: '字体颜色', h: 1 }, InsertUnorderedList: { t: '符号列表' }, InsertOrderedList: { t: '数字列表' }, JustifyLeft: { t: '文本左对齐 (Ctrl+L)', s: 'Ctrl+L' }, JustifyCenter: { t: '居中(Ctrl+E)', s: 'Ctrl+E' }, Outdent: { t: '减少缩进' }, Indent: { t: '增加缩进' }, Link: { t: '超链接', h: 1 }, Img: { t: '图片', h: 1 }, Emote: { t: '表情', s: 'ctrl+e', h: 1 }, Table: { t: '表格', h: 1 }, QuickReply: { t: '常用语', h: 1 }, Upload: { t: '文件上传', h: 1 }, HotKey: { t: '发送快捷键', h: 1 } };
    var fontFamilies = [{ n: '宋体', c: 'SimSun' }, { n: '仿宋', c: 'FangSong_GB2312' }, { n: '黑体', c: 'SimHei' }, { n: '楷体', c: 'KaiTi_GB2312' }, { n: '微软雅黑', c: 'Microsoft YaHei' }, { n: 'Arial' }, { n: 'Arial Black' }, { n: 'Comic Sans MS' }, { n: 'Courier New' }, { n: 'Times New Roman' }, { n: 'Tahoma' }, { n: 'Verdana' }];
    var fontSizes = [7, 10, 12, 14, 18, 24, 36];
    var colors = ['#FFFFFF', '#CCCCCC', '#C0C0C0', '#808080', '#000000', '#FF0000', '#FF00FF', '#FF7C80', '#FFC03A', '#800000', '#FFFF00', '#808000', '#00FF00', '#66CC80', '#008000', '#0000FF', '#000080', '#00FFFF', '#008080', '#800080'];

    var emotes = { name: '默认', width: 24, height: 24, col: 7, list: { 'smile': '微笑', 'titter': '偷笑', 'laugh': '呲牙', 'tongue': '调皮', 'envy': '色', 'proud': '得意', 'shy': '害羞', 'sad': '难过', 'wronged': '委屈', 'fastcry': '快哭了', 'wail': '大哭', 'ch': '擦汗', 'baiy': '白眼', 'knock': '敲打', 'yun': '晕', 'crazy': '抓狂', 'angry': '发怒', 'ohmy': '惊讶', 'awkward': '尴尬', 'panic': '惊恐', 'cute': '可怜', 'struggle': '奋斗', 'quiet': '嘘', 'shutup': '闭嘴', 'doubt': '疑问', 'despise': '鄙视', 'sleep': '睡', 'bye': '再见', 'baituo': '拜托', 'baoquan': '抱拳', 'guzhang': '鼓掌', 'haixiu': '害羞', 'jingli': '敬礼', 'keai': '可爱', 'kelian': '可怜', 'kuaikule': '快哭了', 'meigui': '玫瑰', 'OK': 'OK', 'qiang': '强', 'shengli': '胜利', 'weiqu': '委屈', 'woshou': '握手', 'yongbao': '拥抱' } };
    var toolsGroups = {
        client: 'FontName,FontSize,Bold,Italic,Undercol,FontColor,|,Emote,Upload,|,HotKey',
        agent: 'FontName,FontSize,Bold,Italic,Undercol,FontColor,|,Link,Emote,QuickReply,HotKey'
    };
    // 1.实例化 编辑器
    var browser = $.browser, browerVer = 7, isIE = true;
    if (browser) {
        browerVer = parseFloat(browser.version);
        isIE = browser.msie;
    }

    var jqeditor = function (textarea, options) {
        //初始化工具弹出层
        $(document.body).append('<div id="lyPanel"></div><div id="lyShadow"></div>');
        var $panel = $("#lyPanel"), $box, $bar, $iframeArea;
        var _this = this, ctrl = textarea, $ctrl = $(ctrl), doc, $doc;
        var emotePath = '/Content/Images/emote/', lastFontName = 'SimSun', lastFontSize = 2;
        var ev = null, timer, disableHover = false, hoverExec = true, lastPoint = null, lastAngle = null;
        var sendHotKeyType = { enter: 0, ctrlEnter: 1, shiftEnter: 2, altS: 3 }, currentHotKey = sendHotKeyType.enter;
        var settings = _this.settings = $.extend({}, jqeditor.settings, options);
        if (settings.tools.match(/^\s*(m?client|agent)\s*$/i))
            settings.tools = toolsGroups[$.trim(settings.tools)];
        settings.tools = settings.tools.split(',');

        //初始化编辑器
        this.init = function () {
            var ctrlWidth = $ctrl.outerWidth(), ctrlHeight = $ctrl.outerHeight();
            if (!ctrlWidth) ctrlWidth = $ctrl.css("width");
            if (!ctrlHeight) ctrlHeight = $ctrl.css("height");
            var editorWidth = settings.width || (ctrlWidth > 10 ? ctrlWidth : 500);
            var editorHeight = settings.height || (ctrlHeight > 10 ? ctrlHeight : 160);
            if (!ctrl.id) ctrl.id = index;
            var boxID = 'box_' + ctrl.id, barID = 'bar_' + ctrl.id, iframeID = 'iframe_' + ctrl.id, iframeAreaID = 'iframearea_' + ctrl.id, cursorID = 'cursor_' + ctrl.id;

            // box html
            $ctrl.after($('<input type="text" id="' + cursorID + '" style="position:absolute;display:none;" /><span id="' + boxID
                + '" class="jqeditor" style="display:none"><table cellspacing="0" cellpadding="0" class="eLayout" style="width:' + editorWidth
                + 'px;height:' + editorHeight + 'px;" role="presentation"><tr><td id="' + barID
                + '" class="eTool" unselectable="on" role="presentation"></td></tr><tr><td id="' + iframeAreaID
                + '" class="eIframeArea" role="presentation"><iframe frameborder="0" id="' + iframeID
                + '" src="javascript:;"></iframe><input class="icon_btn" id="btnSend" type="button" value="发送"/></td></tr></table></span>'));

            //隐藏textarea,同时显示编辑器
            $ctrl.hide();
            $box = $('#' + boxID).show();
            $bar = $('#' + barID);
            //设置编辑区高度
            $iframeArea = $('#' + iframeAreaID);
            $iframeArea.css('height', editorHeight - 32);
            if (isIE & browerVer < 8) setTimeout(function () { $iframeArea.css('height', editorHeight - 32); }, 1);
            try {
                $iframeArea.focusout(function () {
                    chat.updateTypingStatus(0);
                }).focusin(function () {
                    chat.updateTypingStatus(1);
                });
            } catch (e) { }
            //激活焦点
            $ctrl.focus(_this.focus);
            _this.win = $('#' + iframeID)[0].contentWindow;
            //鼠标事件：隐藏弹出层
            $iframeArea.mousedown(function () { _this.hidePanel(); })
            $(document).mousedown(function () { _this.hidePanel(); });
            //禁止冒泡
            $panel.mousedown(function (ev) { ev.stopPropagation() });

            //初始化文本器工具栏
            var barHtml = ['<span class="tools"/>'], tool, css, regSeparator = /\||\//i;
            $.each(settings.tools, function (i, n) {
                if (n.match(regSeparator)) barHtml.push('<span class="eSeparator"/>');
                else {
                    tool = tools[n];
                    if (!tool) return;
                    if (tool.c) css = tool.c;
                    else css = 'eIcon eBtn' + n;
                    barHtml.push('<span><a href="javascript:;" title="' + tool.t + '" cmd="' + n
                        + '" class="eButton eEnabled" tabindex="-1" role="button"><span class="' + css
                        + '" unselectable="on" style="font-size:0;color:transparent;text-indent:-999px;">' + tool.t + '</span></a></span>');
                }
            });
            barHtml.push('<span class="tools"/><br />');
            //工具栏按钮点击事件
            $bar.append(barHtml.join('')).bind('mousedown contextmenu', returnFalse)
                .click(function (event) {
                    var $btn = $(event.target).closest('a');
                    if ($btn.is('.eEnabled')) {
                        clearTimeout(timer);
                        $bar.find('a').attr('tabindex', '-1');
                        ev = event;
                        _this.exec($btn.attr('cmd'));
                    }
                    return false;
                });
            //工具栏按钮悬停事件
            $bar.find('.eButton').hover(function (event) {
                var $btn = $(this);
                var tAngle = lastAngle; lastAngle = null;
                if (disableHover || !$btn.is('.eEnabled')) return false;
                var cmd = $btn.attr('cmd');
                if (tools[cmd].h !== 1) {
                    _this.hidePanel();
                    return false;
                }
                if (hoverExec)
                    timer = setTimeout(function () {
                        ev = event;
                        lastPoint = { x: ev.clientX, y: ev.clientY };
                        _this.exec(cmd);
                    }, 200);
            }, function (event) {
                lastPoint = null;
                if (timer) clearTimeout(timer);
            }).mousemove(function (event) {
                if (lastPoint) {
                    var diff = { x: event.clientX - lastPoint.x, y: event.clientY - lastPoint.y };
                    if (Math.abs(diff.x) > 1 || Math.abs(diff.y) > 1) {
                        if (diff.x > 0 && diff.y > 0) {
                            var tAngle = Math.round(Math.atan(diff.y / diff.x) / 0.017453293);
                            if (lastAngle) lastAngle = (lastAngle + tAngle) / 2
                            else lastAngle = tAngle;
                        }
                        else lastAngle = null;
                        lastPoint = { x: event.clientX, y: event.clientY };
                    }
                }
            });
            // iframe textarea
            try {
                this.doc = doc = _this.win.document; $doc = $(doc);
                doc.open();
                doc.write('<html><head><meta http-equiv="Content-Type" content="text/html; charset=utf-8" />'
                    + '<title>可视化文本编辑器</title><style type="text/css">*{margin:0;padding:0;}</style></head>'
                    + '<body spellcheck="0" class="editMode"></body></html>');
                doc.close();
                doc.body.innerHTML = '<font size=\"' + lastFontSize + '\" face=\"' + lastFontName + '\"> </font>';
                if (isIE) doc.body.contentEditable = 'true';
                else doc.designMode = 'On';
            } catch (e) { }
            //
            $doc.keydown(checkHotKey).bind('mousedown click', function (ev) { $ctrl.trigger(ev.type); });
            if (isIE) {
                $doc.bind('controlselect', function (ev) {
                    ev = ev.target; if (!$.nodeName(ev, 'IMG')) return;
                    $(ev).unbind('resizeend', fixResize).bind('resizeend', fixResize);
                });
                //修正IE拖动img大小不更新width和height属性值的问题
                function fixResize(ev) {
                    var jImg = $(ev.target), v;
                    if (v = jImg.css('width')) jImg.css('width', '').attr('width', v.replace(/[^0-9%]+/g, ''));
                    if (v = jImg.css('height')) jImg.css('height', '').attr('height', v.replace(/[^0-9%]+/g, ''));
                }
            }
            //自动清理粘贴内容
            $(doc.documentElement).bind(isIE ? 'beforepaste' : 'paste', function () {
                if (window.clipboardData) {
                    var text = window.clipboardData.getData('Text', true);
                    if (text) {
                        var dict = { '<': '&lt;', '>': '&gt;', '®': '&reg;', '©': '&copy;' };
                        var charArray = /[<>®©]/g;
                        text = text.replace(charArray, function (c) { return dict[c]; });
                        text = text.replace(/\r?\n/g, '<br />');
                        text = _this.cleanHTML(text);
                        window.clipboardData.setData('Text', text);
                    }
                }
            });
            index++;
            //_this.getDuration();
            return true;
        }
        this.focus = function () {
            _this.win.focus();
            if (isIE) {
                var rng = _this.getRng();
                if (rng.parentElement && rng.parentElement().ownerDocument !== doc) _this.setTextCursor(); //修正IE初始焦点问题
            }
            return false;
        }

        this.exec = function (cmd) {
            _this.hidePanel();
            var text = doc.body.innerHTML;
            if (text)
                $ctrl.attr('value', text);
            var tool = tools[cmd];
            if (!tool) return false;
            if (ev === null)//非鼠标点击
            {
                ev = {};
                var btn = $bar.find('.eButton[cmd=' + cmd + ']');
                if (btn.length === 1) ev.target = btn; //设置当前事件焦点
            }
            cmd = cmd.toLowerCase();
            switch (cmd) {
                case 'fontname':
                    var menuFontname = [];
                    $.each(fontFamilies, function (i, item) { item.c = item.c ? item.c : item.n; menuFontname.push({ s: '<span style="font-family:' + item.c + '">' + item.n + '</span>', v: item.c, t: item.n }); });
                    _this.showMenu(menuFontname, function (i) {
                        lastFontName = i;
                        _this._exec('fontname', lastFontName);
                    });
                    break;
                case 'fontsize':
                    var menuFontsize = [];
                    $.each(fontSizes, function (i, item) { menuFontsize.push({ s: item + 'px', v: i + 1 }); });
                    _this.showMenu(menuFontsize, function (i) {
                        lastFontSize = i;
                        _this._exec('fontsize', lastFontSize);
                    });
                    break;
                case 'fontcolor':
                    _this.showColor(function (i) { _this._exec('forecolor', i); });
                    break;
                case 'emote':
                    _this.showEmote();
                    break;
                case 'quickreply':
                    _this.showQuickReply();
                    break;
                case 'upload':
                    _this.showUpload();
                    break;
                case 'hotkey':
                    _this.showHotKey();
                    break;
                default:
                    _this._exec(cmd);
                    break;
            }
            ev = null;
        }
        this._exec = function (cmd, param, noFocus) {
            if (!noFocus) _this.focus();
            var state;
            // 执行命令标识符
            if (param !== undefined) state = doc.execCommand(cmd, false, param);
            else state = doc.execCommand(cmd, false, null);
            return state;
        }

        // 显示列表
        this.showMenu = function (datas, callback) {
            var $list = $('<div class="eMenu"></div>'), items = [];
            $.each(datas, function (n, v) {
                items.push('<a href="javascript:void(\'' + v.v + '\')" title="' + (v.t ? v.t : v.s) + '" v="' + v.v
                    + '" role="option" aria-setsize="' + datas.length + '" aria-posinset="' + (n + 1) + '" tabindex="0">' + v.s + '</a>');
            });
            $list.append(items.join(''));
            $list.click(function (ev) {
                ev = ev.target;
                if ($.nodeName(ev, 'DIV')) return;
                _this.loadBookmark();
                callback($(ev).closest('a').attr('v'));
                _this.hidePanel();
                return false;
            }).mousedown(returnFalse);
            _this.saveBookmark();
            _this.showPanel($list);
        }

        // 显示色板
        this.showColor = function (callback) {
            var $list = $('<div class="eColor"></div>'), items = [];
            $.each(colors, function (i, v) {
                if (i % 5 === 0) items.push((i > 0 ? '</div>' : '') + '<div>');
                items.push('<a href="javascript:void(\'' + v + '\')" value="' + v + '" title="' + v + '" style="background:' + v + '" role="option" aria-setsize="' + colors.length + '" aria-posinset="' + (i + 1) + '"></a>');
            });
            items.push('</div>');
            $list.append(items.join(''));
            $list.click(function (e) {
                ev = e.target;
                if (!$.nodeName(ev, 'A')) return;
                _this.loadBookmark();
                callback($(ev).attr('value'));
                _this.hidePanel();
                return false;
            }).mousedown(returnFalse);
            _this.saveBookmark();
            _this.showPanel($list);
        }

        // 显示表情
        this.showEmote = function () {
            var imgList = [], col = emotes.col + 1;
            $.each(emotes.list, function (id, title) {
                if (imgList.length % col === 0) imgList.push(imgList.length > 0 ? '<br />' : '');
                imgList.push('<a href="javascript:void(\'' + title + '\')" style="background-image:url(' + emotePath + id + '.gif);" id="' + id + '" title="' + title + '" ev="' + title + '" role="option">&nbsp;</a>');
            });
            $emoteDialog = $('<style> .eEmote div a{width:' + emotes.width + 'px;height:' + emotes.height + 'px;}</style><div class="eEmote"><div>' + imgList.join('') + '</div></div>')
                .click(function (ev) {
                    ev = ev.target;
                    if (!$.nodeName(ev, 'A')) return;
                    _this.loadBookmark();
                    _this.pasteHTML('<img src="' + emotePath + $(ev).attr('id') + '.gif" alt="' + $(ev).attr('ev') + '">');
                    _this.hidePanel(); return false;
                }).mousedown(returnFalse);
            _this.saveBookmark();
            _this.showPanel($emoteDialog);
        }

        // 文件上传
        this.showUpload = function () {
            var idIO = 'jUploadFrame' + new Date().getTime(), _this = this;
            var jIO = $('<iframe name="' + idIO + '" class="eHideArea" />').appendTo('body');
            var jUpload = $('<span class="eUpload"><form id="upform1" ><div>[注]:仅支持上传jpg,png,bmp文件(小于2M)</div><div><input type="file" class="eFile" size="13" name="fileData" tabindex="-1" /><input type="button" value="确定" class="eBtn" tabindex="-1" /></div></form></span>');
            var jUpBtn = $('.eBtn', jUpload), jFile = $('.eFile', jUpload), jForm = $("#upform1", jUpload);

            jUpBtn.click(function () {
                var limitExt = ".jpg|.png|.bmp", limitSize = 2;
                if (!checkFileLimit(jFile[0].value, limitExt, limitSize)) return;
                var upload = new _this.html4Upload(jFile[0], "/Chat/FileUpload", uploadCallback);
                upload.start();
            });

            function checkFileLimit(filename, limitExt, limitSize) {
                var matchExt = (limitExt === '*' || filename.match(new RegExp('\.(' + limitExt.replace(/,/g, '|') + ')$', 'i')))
                if (!matchExt) {
                    alert('上传文件仅支持的文件类型: ' + limitExt);
                    return false;
                }
                try {
                    //var fso = new ActiveXObject('Scripting.FileSystemObject');
                    //var file = fso.GetFile(filename);
                    //if (parseInt(file.Size) > (limitSize * 1024 * 1024)) {
                    //    alert("上传的文件过大[最大为" + limitSize + "M],请重新选择。");
                    //    return false;
                    //}
                }
                catch (e) { }
                return true;
            }
            function uploadCallback(data) {
                if (data != 'undefined') {
                    data = eval('(' + data + ')');
                    var text = '';
                    if (data.ReturnFlag)
                        text = '<div>【文件】：<a href="' + data.FileUrl + '" target="_blank">'
                            + data.FileName + '</a>&nbsp;&nbsp;[' + data.FileSize + ']</div>';
                    else if (data.ReturnFlag == false)
                        text = '<div>【文件上传失败】：' + data.Message + '</div>';
                    sendMessage(text);
                }
                _this.hidePanel();
            }
            _this.showDialog(jUpload);
        }
        this.html4Upload = function (inputfile, toUrl, callback) {
            var uid = new Date().getTime(), idIO = 'jUploadFrame' + uid, _this = this;
            var jIO = $('<iframe name="' + idIO + '" class="eHideArea" />').appendTo('body');
            var jForm = $('<form action="' + toUrl + '" target="' + idIO + '" method="post" enctype="multipart/form-data" class="xheHideArea"></form>').appendTo('body');
            $(inputfile).appendTo(jForm);
            this.remove = function () {
                if (_this !== null) {
                    $(inputfile).remove();
                    jIO.remove(); jForm.remove();
                    _this = null;
                }
            }
            this.onLoad = function () {
                var ifmDoc = jIO[0].contentWindow.document, result = $(ifmDoc.body).text();
                ifmDoc.write('');
                _this.remove();
                callback(result);
            }
            this.start = function () { jForm.submit(); jIO.load(_this.onLoad); }
            return this;
        }

        this.showHotKey = function () {
            var $hotKey = $('<div class="eHotKey">'
                + '<div><input id="hk1" type="radio" value="0" name="hotkey" /><label for="hk1">按 Enter 键发送消息</label></div>'
                + '<div><input id="hk2" type="radio" value="1" name="hotkey" /><label for="hk2">按 Ctrl + Enter 键发送消息</label></div>'
                + '<div><input id="hk3" type="radio" value="2" name="hotkey" /><label for="hk3">按 Shift + Enter 键发送消息</label></div>'
                + '<div><input id="hk4" type="radio" value="3" name="hotkey" /><label for="hk4">按 Alt + S 键发送消息</label></div>'
                + '</div>');
            $("input[type=radio]", $hotKey).each(function () {
                if (this.value == currentHotKey.toString())
                    this.checked = true;
            });
            $("input[type=radio]", $hotKey).click(function () {
                currentHotKey = parseInt(this.value);
                var keys = ["Enter", "Ctrl + Enter", "Shift + Enter", "Alt + S"];
                $("#liHotKey").html("当前发送快捷键 " + keys[currentHotKey]);
                _this.hidePanel();
            });
            _this.showDialog($hotKey);
        }

        this.showDialog = function (content) {
            var $dialog = $('<div class="eDialog"></div>'), $content = $(content), btnSave = $('#btnSave', $content);
            if (btnSave.length === 1) {
                $content.find('input[type=text],select').keypress(function (ev) { if (ev.which === 13) { btnSave.click(); return false; } });
                $content.find('textarea').keydown(function (ev) { if (ev.ctrlKey && ev.which === 13) { btnSave.click(); return false; } });
            }
            $dialog.append($content);
            _this.showPanel($dialog);
        }
        this.showPanel = function (content) {
            if (!ev.target) return false;
            $panel.html('').append(content).css('left', -999).css('top', -999);
            $activeBar = $(ev.target).closest('a').addClass('eActive');
            var btnXY = $activeBar.offset(), btnWidth = $activeBar.outerWidth(), btnHeight = $activeBar.outerHeight(), panelWidth = $panel.outerWidth(), panelHeight = $panel.outerHeight();
            var x = btnXY.left, y = btnXY.top - panelHeight, maxX = $box.offset().left + $box.outerWidth();
            if ((x + panelWidth) > maxX) x -= (panelWidth - btnWidth);
            var shadowBorder = 3;
            if (settings.disableShadow) shadowBorder = 0;
            $("#lyShadow").css({ 'left': x + shadowBorder, 'top': y + shadowBorder, 'width': panelWidth, 'height': panelHeight }).show();
            $panel.css({ 'left': x, 'top': y }).show();
            bShowPanel = true;
        }
        this.hidePanel = function () {
            if (bShowPanel) {
                $activeBar.removeClass('eActive');
                $('#lyShadow').hide();
                $panel.hide();
                bShowPanel = false;
                lastAngle = null;
                _this.focus();
                _this.loadBookmark();
            }
        }

        //检查粘贴的HTML
        this.pasteHTML = function (sHtml) {
            _this.focus();
            var sel = _this.getSel(), rng = _this.getRng();
            if (rng.insertNode) {
                if ($(rng.startContainer).closest('style,script').length > 0) return false;
                rng.deleteContents();
                rng.insertNode(rng.createContextualFragment(sHtml));
            }
            else {
                if (sel.type.toLowerCase() === 'control') { sel.clear(); rng = _this.getRng(); };
                rng.pasteHTML(sHtml);
            }
        }
        this.cleanHTML = function (sHtml) {
            sHtml = sHtml.replace(/<!?\/?(DOCTYPE|html|body|meta)(\s+[^>]*?)?>/ig, '');
            sHtml = sHtml.replace(/<head(?:\s+[^>]*?)?>([\s\S]*?)<\/head>/i, '');
            sHtml = sHtml.replace(/<\??xml(:\w+)?(\s+[^>]*?)?>([\s\S]*?<\/xml>)?/ig, '');
            sHtml = sHtml.replace(/<script(\s+[^>]*?)?>[\s\S]*?<\/script>/ig, '');
            sHtml = sHtml.replace(/<style(\s+[^>]*?)?>[\s\S]*?<\/style>/ig, '');
            sHtml = sHtml.replace(/(<(\w+))((?:\s+[\w-]+\s*=\s*(?:"[^"]*"|'[^']*'|[^>\s]+))*)\s*(\/?>)/ig,
                function (all, left, tag, attr, right) {
                    if (!settings.linkTag && tag.toLowerCase() === 'link') return '';
                    if (!settings.incolScript) attr = attr.replace(/\s+on(?:click|dblclick|mouse(down|up|move|over|out|enter|leave|wheel)|key(down|press|up)|change|select|submit|reset|blur|focus|load|unload)\s*=\s*("[^"]*"|'[^']*'|[^>\s]+)/ig, '');
                    if (!settings.incolStyle) attr = attr.replace(/\s+(style|class)\s*=\s*("[^"]*"|'[^']*'|[^>\s]+)/ig, '');
                    return left + attr + right;
                });
            sHtml = sHtml.replace(/<\/(strong|b|u|strike|em|i)>((?:\s|<br\/?>|&nbsp;)*?)<\1(\s+[^>]*?)?>/ig, '$2');
            sHtml = sHtml.replace(/(<br\/?>|<hr\/?>|&nbsp;)*/ig, '');
            return sHtml;
        }
        this.saveBookmark = function () {
            _this.focus();
            var rng = _this.getRng();
            rng = rng.cloneRange ? rng.cloneRange() : rng;
            bookmark = { 'rng': rng };
        }
        this.loadBookmark = function () {
            if (!bookmark) return;
            _this.focus();
            var rng = bookmark.rng;
            if (isIE) rng.select();
            else {
                var sel = _this.getSel();
                sel.removeAllRanges();
                sel.addRange(rng);
            }
            bookmark = null;
        }

        this.getParent = function (tag) {
            var rng = _this.getRng(), p;
            if (isIE) p = rng.item ? rng.item(0) : rng.parentElement();
            else {
                p = rng.commonAncestorContainer;
                if (!rng.collapsed) if (rng.startContainer === rng.endContainer && rng.startOffset - rng.endOffset < 2 && rng.startContainer.hasChildNodes()) p = rng.startContainer.childNodes[rng.startOffset];
            }
            tag = tag ? tag : '*'; p = $(p);
            if (!p.is(tag)) p = $(p).closest(tag);
            return p;
        }

        this.getRng = function (bNew) {
            var sel, rng;
            try {
                if (!bNew) {
                    sel = _this.getSel();
                    rng = sel.createRange ? sel.createRange() : sel.rangeCount > 0 ? sel.getRangeAt(0) : null;
                }
                if (!rng) rng = doc.body.createTextRange ? doc.body.createTextRange() : doc.createRange();
            } catch (ex) { }
            return rng;
        }
        this.getSel = function () { return doc.selection ? doc.selection : _this.win.getSelection(); }


        this.getContentAndClear = function () {
            var text = '';
            $text = $("<div></div>");
            $text.append(doc.body.innerHTML);
            if ($text.length > 0) {
                $('a', $text).each(function () { $(this).attr("target", "_blank"); });
                text = $text.html();
            }
            if ($($text[0]).text().length > 350) {
                text = '$$OVERFLOW$$';
                alert('发送消息内容超长，请分条发送。');
            }
            else {
                doc.body.innerHTML = '<font size=\"' + lastFontSize + '\" face=\"' + lastFontName + '\"></font>';
                _this.saveBookmark();
                _this.loadBookmark();
            }
            return text;
        }

        var pauseSeconds = 0, th, enableAlert = true;
        // 空闲计时
        this.getDuration = function () {
            if ($("#btnSend").attr('disabled') != 'disabled')
                pauseSeconds++;
            if (pauseSeconds >= 360) {
                if (chat) chat.leftChat();
                clearTimeout(th);
            }
            if (pauseSeconds >= 180 && enableAlert) {
                alert("您已经3分钟没有输入信息了！");
                enableAlert = false;
            }
            th = setTimeout(_this.getDuration, 1000);
        }


        function checkHotKey(ev) {
            pauseSeconds = 0;
            enableAlert = true

            var enable = true;
            if (isIE) {
                //禁止Backspace页面后退
                var rng = _this.getRng();
                if (ev.which === 8 && rng.item) {
                    $(rng.item(0)).remove();
                    return enable;
                }
                if (ev.ctrlKey && (ev.keyCode == 90 || ev.keyCode == 122)) {
                    doc.body.innerHTML = '<font size=\"' + lastFontSize + '\" face=\"' + lastFontName + '\"></font>';
                    return false;
                }
            }
            if (currentHotKey == sendHotKeyType.enter && ev.keyCode == 13) enable = false;
            else if (ev.ctrlKey || ev.shiftKey || ev.altKey) {
                if (ev.ctrlKey && ev.keyCode == 13) enable = !(currentHotKey == sendHotKeyType.ctrlEnter);
                else if (ev.shiftKey && ev.keyCode == 13) enable = !(currentHotKey == sendHotKeyType.shiftEnter);
                else if (ev.altKey && (ev.keyCode == 83 || ev.keyCode == 115))
                    enable = !(currentHotKey == sendHotKeyType.altS);
            } else enable = true;
            if (!enable) sendMessage();
            return enable;
        }
        function sendMessage(text) {
            if (!text)
                text = window.jqeditor.getEditorContent('txtMessage');
            text = text.replace(new RegExp('src="http://' + document.location.host, "ig"), 'src="');
            text = text.replace(/<\/?(script)[^>]*>/ig, "");
            var ntext = text.replace(/<\/?(p|font|strong|b|em)[^>]*>/ig, "");
            if (ntext.length == 0) {
                alert("请先输入内容");
            }
            else if (chat && text != '$$OVERFLOW$$' && $("#btnSend").attr("disabled") != 'disabled') {
                text = encodeURIComponent(text);
                chat.sendMessage(text);
            }
        }

        // 远程提交
        function sendJsonpRequest(url, params, callback) {
            $.ajax({
                url: url,
                data: params,
                dataType: "jsonp",
                jsonp: 'jsoncallback',
                success: function (data) {
                    callback(data);
                }
            });
        }

    }
    function returnFalse() { return false; }

    jqeditor.settings = { tools: 'client', linkTag: false, disableShadow: false, cleanPaste: 1, appServer: '', queueName: '' };
    jqeditor.editors = [];
    jqeditor.getEditorContent = function (id) {
        var editor = null;
        for (var index = 0; index <= jqeditor.editors.length - 1; index++) {
            var item = jqeditor.editors[index];
            if (item.id == id) {
                editor = item.editor;
                break;
            }
        }
        if (editor)
            return editor.getContentAndClear();
        return '';
    }
    window.jqeditor = jqeditor;

    $(function () {
        var editors = [], editorsItem = function () { var id, editor; };
        $('textarea').each(function () {
            var $this = $(this), css = $this.attr('class');
            if (css && (css = css.match(/(?:^|\s)jqeditor(?:\-(m?client|agent))?(?:\s|$)/i))) {
                var editor = new jqeditor(this, css[1] ? { tools: css[1] } : null);
                editor.init();
                addEditor($this.attr('id'), editor);
            }
        });
        jqeditor.editors = editors;

        function addEditor(id, editor) {
            var item = new editorsItem();
            item.id = id;
            item.editor = editor;
            editors.push(item);
        };

    });

})(jQuery);