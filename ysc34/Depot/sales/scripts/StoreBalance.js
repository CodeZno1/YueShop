var curpagesize = 10, curpageindex = 1, total = 0;
var showbox, dataurl, showpager, datanullbox, datanulldom;

var returnStatus = "";

$(function () {
    showbox = $("#datashow");
    showpager = $("#showpager");
    dataurl = $("#dataurl").val();
    datanullbox = showbox;

    InitPage();

    //初始数据显示
    ShowListData(showbox, curpageindex, curpagesize, true);
    //搜索
    $("#btnSearch").on("click", function () {
        ReloadPageData(1);
    });
});

function InitPage() {
    $('#startDate').datetimepicker({
        minView: 2, format: 'yyyy-mm-dd', language: 'zh-CN', weekStart: 1,
        todayBtn: 1, autoclose: 1, todayHighlight: 1, startView: 2,
        forceParse: 0, showMeridian: 1,
    });
    $('#endDate').datetimepicker({
        minView: 2, format: 'yyyy-mm-dd',
        language: 'zh-CN', weekStart: 1, todayBtn: 1,
        autoclose: 1, todayHighlight: 1, startView: 2,
        forceParse: 0, showMeridian: 1,
    });
}

function ExportToExcel() {
    var url = dataurl + "?";
    var urldata = {
        action: "exporttoexcel"
    }
    initQuery(urldata);
    for (var item in urldata) {
        url += item + "=" + urldata[item] + "&";
    }
    window.open(url);
}

//搜索参数
function initQuery(obj) {
    var startDate = $("#startDate").val();
    if (startDate.length > 0) {
        obj.StartDate = startDate;
    }
    var endDate = $("#endDate").val();
    if (endDate.length > 0) {
        obj.EndDate = endDate;
    }
    var ddlReceivables = $("#ddlReceivables").val();
    if (ddlReceivables.toString().length > 0) {
        obj.IsStoreCollect = ddlReceivables;
    }
    return obj;
}

function CheckQuery(obj) {
    if (!obj.StartDate) {
        ShowMsg("请选择开始时间", false);
        return false;
    }
    if (!obj.EndDate) {
        ShowMsg("请选择结束时间", false);
        return false;
    }
    return true;
}



function ShowListData(target, page, pagesize, initpagination) {
    page = page || 1;
    pagesize = pagesize || 10;
    if (typeof (initpagination) == "undefined") { initpagination = true; }
    var urldata = {
        page: page, rows: pagesize, action: "getlist"
    }
    initQuery(urldata);
    if (!CheckQuery(urldata)) {
        return;
    }
    var loadingobj = showLoading(showbox);
    target.empty();
    DataNullHelper.hide(datanullbox);

    $.ajax({
        type: "GET",
        url: dataurl,
        data: urldata,
        dataType: "json",
        success: function (data) {
            var isnodata = true;
            var isshowpage = true;
            var databox = $(target);
            loadingobj.close();
            databox.empty(); DataNullHelper.hide(datanullbox);
            $("#lblOrderSummaryTotal").html("0.00");
            if (data) {
                total = data.total;
                if (total == 0) {
                    isshowpage = false;
                }
                if (data.rows.length > 0) {
                    isnodata = false;
                    databox.html(template('datatmpl', data));
                    $('[data-toggle="tooltip"]', databox).tooltip();
                }
                $("#lblOrderSummaryTotal").html(data.totalAmount.toFixed(2));
            }
            if (isnodata) {
                //total = 0;
                DataNullHelper.show(datanullbox);
            }
            $("#spanTotalCount").html(total);
            curpageindex = page;
            if (initpagination) {
                //初始分页组件
                if (isshowpage) {
                    showpager.HiPaginator({
                        totalCounts: total,
                        pageSize: curpagesize,
                        currentPage: curpageindex,
                        prev: '<a href="javascript:;" class="page-prev">上一页</a>',
                        next: '<a href="javascript:;" class="page-next">下一页</a>',
                        page: '<a href="javascript:;">{{page}}</a>',
                        first: '',
                        last: '',
                        visiblePages: 10,
                        disableClass: 'hide',
                        activeClass: 'page-cur',
                        onPageChange: function (pageindex, type) {
                            //分页回调
                            curpagesize = $("#showpager").HiPaginator("getPagesize");
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
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            loadingobj.close();
            ShowMsg("系统内部异常", false);
        },
    });
}


function ReloadPageData(pageindex) {    /*复位*/    try { var chkall = $("#checkall"); if (chkall) { if (chkall.iCheck) { chkall.iCheck('uncheck'); } chkall.prop("checked", false); } } catch (e) { }
    var _page = showpager;
    var hasPaginator = showpager.HiPaginator("isExist");
    if (hasPaginator) {
        var opts = _page.HiPaginator("getOption");
        curpagesize = opts.pageSize;
        curpageindex = pageindex || opts.currentPage;
    } else {
        curpagesize = 10;
        curpageindex = 1;
    }
    ShowListData(showbox, curpageindex, curpagesize, true);
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