function ShowDropDown() {
    $("#ddlSubType").show();
    $("#Tburl").hide();
    $("#ddlThridType").hide();
    $("#navigateDesc").hide();
}
function ShowThirdDropDown() {
    $("#ddlSubType").trigger("change");
}
function HiddenAll() {
    $("#Tburl").hide();
    $("#ddlSubType").hide();
    $("#ddlThridType").hide();
    $("#navigateDesc").hide();
}
function ShowTextBox() {
    $("#ddlSubType").hide();
    $("#Tburl").show();
    $("#navigateDesc").hide();
    $("#ddlThridType").hide();
}
function ShowNavigate() {
    $("#ddlSubType").hide();
    $("#ddlThridType").hide();
    $("#Tburl").show();
    $("#navigateDesc").show();
}
function GetTopics() {
    $.ajax({
        url: "../VsiteHandler.ashx",
        type: "POST",
        dataType: "json",
        data: { "actionName": "Topic" },
        success: function (result) {
            $("#ddlSubType").empty();
            if (result != null) {
                $(result).each(
                        function (index, item) {
                            var option = $("<option value=" + item.TopicId + ">" + item.Title + "</option>");
                            $("#ddlSubType").append(option);
                        }
                        );
            }
            else {
                // alert("加载专题错误！");
            }
        },
        error: function (xmlHttpRequest, error) {
            alert(error);
        }
    });
}

function GetCategory() {
    $.ajax({
        url: "../VsiteHandler.ashx",
        type: "POST",
        dataType: "json",
        data: { "actionName": "Category" },
        success: function (result) {
            $("#ddlSubType").empty();
            if (result != null) {
                $(result).each(
                        function (index, item) {
                            var option = $("<option value=" + item.CateId + ">" + item.CateName + "</option>");
                            $("#ddlSubType").append(option);
                        }
                        );
            }
            else {
                // alert("加载专题错误！");
            }
        },
        error: function (xmlHttpRequest, error) {
            alert(error);
        }
    });
}


function GetVotes() {
    $.ajax({
        url: "../VsiteHandler.ashx",
        type: "POST",
        dataType: "json",
        data: { "actionName": "Vote" },
        success: function (result) {
            $("#ddlSubType").empty();
            if (result != null) {
                $(result).each(
                        function (index, item) {
                            var option = $("<option value=" + item.VoteId + ">" + item.VoteName + "</option>");
                            $("#ddlSubType").append(option);
                        }
                        );
            }
            else {
                // alert("加载投票错误！");
            }
        },
        error: function (xmlHttpRequest, error) {
            alert(error);
        }
    });
}

function GetActivity() {
    $.ajax({
        url: "../VsiteHandler.ashx",
        type: "POST",
        async: false,
        dataType: "json",
        data: { "actionName": "Activity" },
        success: function (result) {
            $("#ddlSubType").empty();
            if (result != null) {
                $("#ddlSubType").append('<option value="">请选择活动</option>');
                $(result).each(
                        function (index, item) {
                            var option = $("<option value=" + item.Value + ">" + item.Name + "</option>");
                            $("#ddlSubType").append(option);
                        }
                        );
            }
            else {
                // alert("加载活动错误！");
            }
        },
        error: function (xmlHttpRequest, error) {
            alert(error);
        }
    });
}
//加载文章分类
function GetArticleCategory() {
    $.ajax({
        url: "../VsiteHandler.ashx",
        type: "POST",
        async: false,
        dataType: "json",
        data: { "actionName": "ArticleCategory" },
        success: function (result) {
            $("#ddlSubType").empty();
            if (result != null) {
                $("#ddlSubType").append('<option value="">请选择文章分类</option>');
                $(result).each(
                        function (index, item) {
                            if (index == 0)
                                var option = $("<option value=" + item.Value + " selected=\"selected\">" + item.Name + "</option>");
                            else
                                var option = $("<option value=" + item.Value + ">" + item.Name + "</option>");
                            $("#ddlSubType").append(option);
                        }
                        );
            }
            else {
                // alert("加载活动错误！");
            }
        },
        error: function (xmlHttpRequest, error) {
            alert(error);
        }
    });
}
///加载文章列表
function GetArticleList(type) {
    $.ajax({
        url: "../VsiteHandler.ashx",
        type: "POST",
        dataType: "json",
        data: { "actionName": "ArticleList", "categoryId": type },
        success: function (result) {
            $("#ddlThridType").empty();
            if (result != null && result.length > 0) {
                $(result).each(
                        function (index, item) {
                            var option = $("<option value=" + item.Value + ">" + item.Name + "</option>");
                            $("#ddlThridType").append(option);
                        }
                        );
            }
            else {
                alert("加载文章列表错误,或者你没有添加该栏目下的文章请先添加！");
            }
        },
        error: function (xmlHttpRequest, error) {
            alert(xmlHttpRequest.toString());
        }
    });
}
function GetLoctionUrl() {
    var typeval = $("#ddlType").val();
    var result;
    debugger;
    switch (typeval) {
        case "Topic":
            result = $("#ddlSubType").val();
            break;
        case "Vote":
            result = "Vote.aspx?voteId=" + $("#ddlSubType").val();
            break;
        case "Activity":
            var thirdtype = $("#ddlThridType").val();
            result = $("#ddlSubType").val() + "," + thirdtype;
            if (thirdtype == "" || thirdtype == null) {
                alert("请选择一个活动");
                return false;
            }
            break;
        case "Home":
            result = "Default.aspx";
            break;
        case "Category":
            // result = $("#ddlSubType").val();
            result = "ProductSearch.aspx";
            break;
        case "ShoppingCart":
            result = "ShoppingCart.aspx";
            break;
        case "OrderCenter":
            result = "MemberCenter.aspx"
        case "VipCard":
            result = "MemberCard.aspx";
            break;
        case "Link":
            result = $("#Tburl").val();
            break;
        case "Phone":
            result = $("#Tburl").val();
            break;
        case "Address":
            result = $("#Tburl").val();
            break;
        case "GroupBuy":
            result = "GroupBuyList.aspx";
            break;
        case "Brand":
            result = "BrandList.aspx";
            break;
        case "Article":
            var thirdtype = $("#ddlThridType").val();
            if (thirdtype == null || thirdtype == "") { alert("请选择一篇文章"); return false; }
            result = "ArticleDetails.aspx?articleId=" + thirdtype;
            break;
        case "CountDownBuy":
            result = "CountDownProcuts.aspx";
            break;
        case "PointMall":
            result = "PointMall.aspx";
            break;
    }
    $("#locationUrl").val(result);
    return true;
}

function showThird(type) {

    $.ajax({
        url: "../VsiteHandler.ashx",
        type: "POST",
        dataType: "json",
        data: { "actionName": "ActivityList", "acttype": type },
        success: function (result) {
            $("#ddlThridType").empty();
            if (result != null && result.length > 0) {
                $(result).each(
                        function (index, item) {
                            var option = $("<option value=" + item.ActivityId + ">" + item.ActivityName + "</option>");
                            $("#ddlThridType").append(option);
                        }
                        );
            }
            else {
                alert("加载活动列表错误,或者你没有添加该栏目下的活动请先添加！");
            }
        },
        error: function (xmlHttpRequest, error) {
            // alert(xmlHttpRequest.toString());
        }
    });
}

function BindSubType() {
    $("#ddlSubType").bind("change", function () {

        var typeval = $(this).val();
        if ($("#ddlType").val() == "Activity") {
            showThird(typeval);
        }
        if ($("#ddlType").val() == "Article") {
            GetArticleList(typeval);
        }
    });

}

function BindType() {
    BindSubType();
    $("#ddlType").bind("change", function () {
        var typeval = $(this).val();
        switch (typeval) {
            case "Topic":
                ShowDropDown();
                GetTopics();
                break;
            case "Vote":
                ShowDropDown();
                GetVotes();
                break;
            case "Activity":
                ShowDropDown();
                GetActivity();
                showThird($("#ddlSubType").val());
                $("#ddlThridType").show();
                break;
            case "Home":
                HiddenAll();
                break;
            case "OrderCenter":
                HiddenAll();
                break;
            case "Category":
                //  ShowDropDown();
                //GetCategory();
                HiddenAll();
                break;
            case "ShoppingCart":
                HiddenAll();
                break;
            case "VipCard":
                HiddenAll();
                break;
            case "Link":
                ShowTextBox();
                break;
            case "Phone":
                ShowTextBox();
                break;
            case "Address":
                ShowNavigate();
                break;
            case "GroupBuy":
            case "Brand":
            case "CountDownBuy":
                HiddenAll();
                break;
            case "Article":
                ShowDropDown();
                GetArticleCategory();
                GetArticleList($("#ddlSubType").val());
                $("#ddlThridType").show();
                break;
        }
    }
);

}