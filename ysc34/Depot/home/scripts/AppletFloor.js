dataurl = $("#dataurl").val();
function ShowAddProductDiv() {
    DialogFrameClose("/Depot/home/SearchProduct?ProductType=-1&ProductIds=" + getSelectedProductIds(), "选择商品", null, null, function (e) { CloseFrameWindow(); });
}
function CloseFrameWindow() {
    var arr = $("#ctl00_contentHolder_hidSelectProducts").val();
    if (arr == "") return;
    var allProducts = $("#ctl00_contentHolder_hidAllSelectedProducts");
    if (allProducts.val() != "") {
        allProducts.val(allProducts.val() + ",,," + arr);
    }
    else {
        allProducts.val(arr);
    }
    BindProductHtml(arr);
    getProductPager(1);
    currentPage = 1;
    $("#ctl00_contentHolder_hidSelectProducts").val('');
}
function BindProductHtml(arr) {
    if (arr == "Load") {
        arr = $("#ctl00_contentHolder_hidSelectProducts").val();
    }
    if (arr == "" || arr == undefined) return;
    debugger
    var items = arr.split(",,,");
    var content = "";
    for (var i = 0; i < items.length; i++) {
        var item = items[i];
        var record = item.split("|||");
        content += String.format("<tr name='appendlist'><td>{0}</td><td><input type='hidden' value='{1}' id='ctl00_contentHolder_hidProduct_{2}' /><span class='icon_close' onclick='onDelPrduct(this,\"{0}\");'></span></td></tr>", record[1], record[0], record[0]);
    }
    $("#addlist tr:eq(0)").after(content);
    var listCount = $("[name='appendlist']").length;  //总记录数
    $("#ctl00_contentHolder_lblSelectCount").html(listCount);
}

function onDelPrduct(obj, name) {
    if (confirm("确定要删除商品【" + name + "】吗？")) {
        $(obj).parent().parent().remove();
        $("#ctl00_contentHolder_lblSelectCount").html($("[name='appendlist']").length);
        getProductPager(1);
        currentPage = 1;
    }
}
function getSelectedProductIds() {
    var productIds = "";
    $("#addlist input[id^='ctl00_contentHolder_hidProduct_']").each(function (i, obj) {
        if (productIds != "")
            productIds += "," + $(obj).val();
        else
            productIds += $(obj).val();
    });
    return productIds;
}

function setChooseProducts() {
    $("#ctl00_contentHolder_hidProductIds").val(getSelectedProductIds());
}
var pageSize = 5;
var currentPage = 1;
var pageCount = 1;
var listCount;
function getProductPager(pageIndex) {
    var listCount = $("[name='appendlist']").length;  //总记录数
    if (listCount <= 5) {
        $("[name='appendlist']").show();
        $("#divPage").hide();
        return;
    }
    $("#divPage").show();
    pageCount = Math.ceil(listCount / pageSize);  //总页数
    $("#spanPageCount").html(pageIndex + "/" + pageCount);
    var startIndex = pageSize * (pageIndex - 1) + 1;
    var endIndex = startIndex + pageSize;
    $("[name='appendlist']").hide();
    $("[name='appendlist']").slice(startIndex - 1, endIndex - 1).show();
}
function goToPrePage() {
    if (currentPage == 1) return;
    getProductPager(currentPage - 1);
    currentPage = currentPage - 1;
}
function goToNextPage() {
    if (currentPage == pageCount) return;
    getProductPager(currentPage + 1);
    currentPage = currentPage + 1;
}

function doAddSubmit() {
    var FloorName = $("#txtFloorName").val();
    var ProductIds = getSelectedProductIds();

    if (FloorName == "") {
        ShowMsg("楼层名称不能为空!", false);
        return;
    }
    if (ProductIds == "") {
        ShowMsg("请选择关联商品!", false);
        return;
    }
    //if (ImageId == "") {
    //    alert("请选择关联营销图片!");
    //    return;
    //}
    var urldata = {
        action: "AddAppletFloor", FloorName: FloorName, ProductIds: ProductIds
    }
    var loading;
    try {
        loading = showCommonLoading();
    } catch (e) { }
    $.ajax({
        type: "post",
        url: dataurl,
        data: urldata,
        dataType: "json",
        success: function (data) {
            try {
                loading.close();
            } catch (e) { }
            if (data.success) {
                ShowMsg(data.message, true);
                location.href="AppletConfig"
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

function doEditSubmit() {
    var FloorName = $("#txtFloorName").val();
    var ProductIds = getSelectedProductIds();
    var floorId = parseInt($("#txtFloorId").val());
    if (isNaN(floorId)||floorId<=0) {
        ShowMsg("错误的楼层ID", false);
        return;
    }
    if (FloorName == "") {
        ShowMsg("楼层名称不能为空!", false);
        return;
    }
    if (ProductIds == "") {
        ShowMsg("请选择关联商品!", false);
        return;
    }
    //if (ImageId == "") {
    //    alert("请选择关联营销图片!");
    //    return;
    //}
    var urldata = {
        action: "UpdateAppletFloor", FloorId: floorId, FloorName: FloorName, ProductIds: ProductIds
    }
    var loading;
    try {
        loading = showCommonLoading();
    } catch (e) { }
    $.ajax({
        type: "post",
        url: dataurl,
        data: urldata,
        dataType: "json",
        success: function (data) {
            try {
                loading.close();
            } catch (e) { }
            if (data.success) {
                ShowMsg(data.message, true);
                location.href = "AppletConfig"
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