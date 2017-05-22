function closeWindow() {
    var isIE = navigator.appName == "Microsoft Internet Explorer";
    if (!isIE) window.close();
    else {
        window.opener = "";
        window.open("", "_self");
        window.close();
    }
}

function deleteConfirm() {
    return confirm('确认要删除本条记录吗？');
}

function showDialog(url, width, height) {
    if (!width) width = 900;
    if (!height) height = 500;
    var size = 'dialogHeight:' + height + 'px;dialogWidth:' + width + 'px;edge:raised;resizable:yes;scroll:yes;status:no;center:yes;help:no;minimize:no;maximize:yes;';
    // Make sure you have set a value to window.returnValue before using it.
    return window.showModalDialog(url, 'window', size);
}

String.prototype.format = function (args) {
    var result = this;
    if (arguments.length > 0) {
        if (arguments.length == 1 && typeof (args) == "object") {
            for (var key in args) {
                if (args[key] != undefined) {
                    var reg = new RegExp("({" + key + "})", "g");
                    result = result.replace(reg, args[key]);
                }
            }
        }
        else {
            for (var i = 0; i < arguments.length; i++) {
                if (arguments[i] != undefined) {
                    var reg = new RegExp("({[" + i + "]})", "g");
                    result = result.replace(reg, arguments[i]);
                }
            }
        }
    }
    return result;
}

function refreshNavByID(id) {
    window.top.frames["Tree"].reqLink(id);
}

//禁止输入负号
function validateKeyCode() {
    var k = window.event.keyCode;
    if ((k == 189) || (k == 109)) {
        window.event.returnValue = false;
    }
}

function resetWindowSize() {
    var docHeight = document.documentElement.clientHeight;
    var bodyHeight = document.body.scrollHeight;
    var docWidth = document.documentElement.clientWidth;
    var bodyWidth = document.body.scrollHeight;
    var height = (docHeight > bodyHeight) ? docHeight : bodyHeight;
    var width = (docWidth > bodyWidth) ? docWidth : bodyWidth;
    $("#divFoot").css({ "top": (height - 186) + "px", "width": width + "px" });
}
