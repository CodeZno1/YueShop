var urlPageIndex = parseInt(getParam("page"));
var urlPageSize = parseInt(getParam("rows"));
if (isNaN(urlPageIndex) || urlPageIndex <= 0) {
    urlPageIndex = 1;
}
else {
    $("#curpageindex").val(urlPageIndex)
}
if (isNaN(urlPageSize) || urlPageSize <= 0) {
    urlPageSize = 10;
}
else {
    $("#pagesize_dropdown").val(urlPageSize);
}
var curpagesize = urlPageSize, curpageindex = urlPageIndex, total = 0;
var showbox, dataurl, showpager, datanullbox, datanulldom;

var DEFAULT_PAGE_INDEX = 1, DEFAULT_PAGESIZE = 10;
var queryParameters;
function getReturnUrl() {
    var listurl = "/admin/member/AccountSummaryList";
    var url = listurl + "?";
    if (queryParameters) {
        for (var item in queryParameters) {
            url += item + "=" + queryParameters[item] + "&";
        }
    }
    if (url.lastIndexOf("&") == url.length - 1) {
        url = url.substring(0, url.length - 1);
    }
    return url;
}
///预付款明细
function ToDetail(userId) {
    location.href = "/admin/member/BalanceDetails.aspx?userId=" + userId + "&returnUrl=" + encodeURIComponent(getReturnUrl());
}

//页面初始
$(function () {
    setQuery();
    InitParameters();
    InitPage();
    //初始数据显示
    ShowListData(showbox, curpageindex, curpagesize, true);

    $.ajax({
        type: "GET",
        url: dataurl,
        data: { action: "GetAdvanceStatistics" },
        dataType: "json",
        success: function (data) {
            var dataJson = data;
            if (dataJson.Status == "Success") {
                $("#lbtotalIncome").html(dataJson.Data.totalIncome);
                $("#lbtotalBalance").html(dataJson.Data.totalBalance);
                $("#lbtodayExpenses").html(dataJson.Data.todayExpenses);
                $("#lbtodayIncome").html(dataJson.Data.todayIncome);
            } else {
                ShowMsg(dataJson.Msg, false);
            }
        },
        error: function (data) {
            ShowMsg("系统内部异常:" + data, false);
        }
    });
});

///参数处理
function InitParameters() {
    showbox = $("#datashow");
    showpager = $("#showpager");
    dataurl = $("#dataurl").val();
    datanullbox = showbox;

    try {
        //curpageindex = parseInt($("#curpageindex").val());
        if (curpageindex <= 0) {
            curpageindex = DEFAULT_PAGE_INDEX;
        }
    } catch (e) {
        curpageindex = DEFAULT_PAGE_INDEX;
    }
    try {
        curpagesize = parseInt($("#pagesize_dropdown").val());
        if (isNaN(curpagesize)) {
            curpagesize = DEFAULT_PAGESIZE;
        }
    } catch (e) {
        curpagesize = DEFAULT_PAGESIZE;
    }
}
//页面初始
function InitPage() {
    //搜索
    $("#btnSearch").on("click", function () {
        ShowListData(showbox, 1, curpagesize, true);
    });
    //每页显示数量
    $("#pagesize_dropdown").on("change", function () {
        var _t = $(this);
        curpagesize = parseInt(_t.val());
        ShowListData(showbox, curpageindex, curpagesize, false);
        showpager.HiPaginator("option", { pageSize: curpagesize });
        showpager.HiPaginator("redraw");
    });
    $("#btnAddBalance").on("click", function () {
        Post_AddMoney();
    });
}

//Tab过滤器
function FilterTab() {
    var tab_status = $(".tab_status");
    var filterdom = $("#hidFilter");
    var FILTER_DATA_NAME = "filter", ACTION_CLASS_NAME = "hover";
    if (filterdom && tab_status.length > 0) {
        tab_status.on("click", function () {
            var _t = $(this);
            var _d = _t.data(FILTER_DATA_NAME);
            filterdom.val(_d);
            tab_status.removeClass(ACTION_CLASS_NAME);
            _t.addClass(ACTION_CLASS_NAME);
            ShowListData(showbox, 1, curpagesize, true);
        });
        var curstatus = filterdom.val();
        if (curstatus && curstatus.length > 0) {
            tab_status.removeClass(ACTION_CLASS_NAME);
            tab_status.each(function () {
                var _t = $(this);
                if (_t.data(FILTER_DATA_NAME) == curstatus) {
                    _t.addClass(ACTION_CLASS_NAME);
                }
            });
        } else {
            tab_status.eq(0).addClass(ACTION_CLASS_NAME);
        }
    }
}
///初始化参数
function setQuery() {
    if (getParam("UserName") != "") {
        $("#txtUserName").val(getParam("UserName"));
    }
    if (getParam("RealName") != "") {
        $("#txtRealName").val(getParam("RealName"));
    }
}
//搜索参数
function initQuery(obj) {
    var UserName = $("#txtUserName").val();
    if (UserName.length > 0) {
        obj.UserName = UserName;
    }
    var RealName = $("#txtRealName").val();
    if (RealName.length > 0) {
        obj.RealName = RealName;
    }
    return obj;
}
//参数检测
function checkQuery(obj) {
    var result = true;

    return result;
}



function ShowListData(target, page, pagesize, initpagination) {
    page = page || 1;
    pagesize = pagesize || 10;
    if (typeof (initpagination) == "undefined") { initpagination = true; }
    var DATA_TEMPLATE_DOM_NAME = "datatmpl";
    var urldata = {
        page: page, rows: pagesize, action: "getlist"
    }
    initQuery(urldata);
    queryParameters = urldata;
    if (!checkQuery(urldata)) {
        return;
    }
    var loadingobj;
    try {
        target.empty();
        DataNullHelper.hide(datanullbox);
        loadingobj = showLoading(target);
    } catch (e) { }

    $.ajax({
        type: "GET",
        url: dataurl,
        data: urldata,
        dataType: "json",
        success: function (data) {
            var isnodata = true;
            var isshowpage = true;
            var databox = $(target);
            try {
                loadingobj.close();
            } catch (e) { }
            databox.empty(); DataNullHelper.hide(datanullbox);
            if (data) {
                total = data.total;
                if (total == 0) {
                    isshowpage = false;
                }
                if (data.rows.length > 0) {
                    isnodata = false;
                    databox.html(template(DATA_TEMPLATE_DOM_NAME, data));
                    DataRedraw(databox);
                }
            }
            if (isnodata) {
                //total = 0;
                DataNullHelper.show(datanullbox);
            }
            curpageindex = page;
            if (initpagination) {
                //初始分页组件
                if (isshowpage) {
                    showpager.HiPaginator({
                        totalCounts: total,
                        pageSize: curpagesize,
                        currentPage: curpageindex,
                        first: '',
                        prev: '<a href="javascript:;" class="page-prev">上一页</a>',
                        next: '<a href="javascript:;" class="page-next">下一页</a>',
                        page: '<a href="javascript:;">{{page}}</a>',
                        last: '',
                        visiblePages: 10,
                        disableClass: 'hide',
                        activeClass: 'page-cur',
                        onPageChange: function (pageindex, type) {
                            //分页回调
                            curpagesize = showpager.HiPaginator("getPagesize");
                            if (type != "init") {
                                ShowListData(showbox, pageindex, curpagesize, false);
                            }
                        }
                    });
                } else {
                    if (showpager.HiPaginator("isExist")) {
                        showpager.HiPaginator("destroy");
                    }
                }
            }
        },
        error: function () {
            try {
                loadingobj.close();
            } catch (e) { }
            ShowMsg("系统内部异常", false);
        }
    });
}
//后续处理
function DataRedraw(databox) {
    try {
        $('[data-toggle="tooltip"]').tooltip();
        $('.icheck', databox).iCheck({ handle: 'checkbox', checkboxClass: 'icheckbox_square-red', });
    } catch (e) { }
}

function ReloadPageData(pageindex) {
    ResetCheckAll();
    var _page = showpager;
    var opts = _page.HiPaginator("getOption");
    curpagesize = opts.pageSize;
    curpageindex = pageindex || opts.currentPage;
    ShowListData(showbox, curpageindex, curpagesize, true);
}

//复位全选
function ResetCheckAll() {
    try {
        var chkall = $("#checkall");
        if (chkall) {
            if (chkall.iCheck) {
                chkall.iCheck('uncheck');
            }
            chkall.prop("checked", false);
        }
    } catch (e) { }
}

//关闭并刷新页面数据
function CloseDialogAndReloadData(id) {
    if (id) {
        art.dialog({ id: id }).close();
    } else {
        CloseDialogFrame("", false);
    }
    ReloadPageData();
}

//获取选中的ID
function GetSelectId() {
    var v_str = "";
    $("input[type='checkbox'][name='CheckBoxGroup']:checked").each(function (rowIndex, rowItem) {
        if (rowIndex > 0) {
            v_str += ",";
        }
        v_str += $(rowItem).attr("value");
    });
    return v_str;
}

function Post_AddMoney() {
    var userid = $("#currentUserId").val();
    var income = $("#txtReCharge").val();
    var remark = $("#txtRemark").val();
    if (userid.length < 1) {
        alert("错误的用户编号");
        return;
    }
    if (!income) {
        alert("加款金额不允许为空！");
        return false;
    }

    if (!parseFloat(income)) {
        alert("请输入正确的金额");
        return false;
    }
    var canoper = true;
    if (canoper) {
        var pdata = {
            userid: userid, income: income, remark: remark, action: "addmoney"
        }
        var loading;
        try {
            loading = showCommonLoading();
        } catch (e) { }
        $.ajax({
            type: "post",
            url: dataurl,
            data: pdata,
            dataType: "json",
            success: function (data) {
                $("#txtRemark").val("");
                $("#txtReCharge").val("");
                try {
                    loading.close();
                } catch (e) { }
                if (data.success) {
                    ShowMsg(data.message, true);
                    ReloadPageData();
                } else {
                    ShowMsg(data.message, false);
                }
            },
            error: function () {
                try {
                    loading.close();
                } catch (e) { }
            }
        });
    }
}


function ExportToExcel() {

    var urldata = {
        action: "ExportToExcel"
    }

    initQuery(urldata);


    var url = dataurl + "?";
    for (var item in urldata) {
        url += item + "=" + urldata[item] + "&";
    }

    window.open(url);
}