//var config_TRANSFERDN = {
//    '报工号': '7632400',
//    '转满意度': '7632201',
//    '转队列': '7632201',
//    '电话支付': '7632410'
//}

//'FromBJ','FromNJ'
var CallCenterCode = 'FromBJ';

//SPI AVAYA
var VOICE_SUPPORT_TYPE = 'Avaya';

//是否启用调试模式，调试模式下会打印日志
var LENOVODEBUG = false;

//分机配置（失效，转移到web.config)
var config_TRANSFERDN = {
    '报工号': '',
    '转满意度': '',
    '转队列': '',
    '电话支付': ''
}

var IPDNSET = {
    //是否报工号
    IsSayEmpNO: 1,
    //是否是真实分机
    IsRealDN: 1,
    //扩展1-未使用
    Ext1: '',
    //扩展2-未使用
    Ext2: '',
    //扩展3-未使用
    Ext3: ''
}

//授权码（失效，转移到web.config)
var lenovo_authorizedNumber = '96050904#';