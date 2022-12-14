<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderLogistics.aspx.cs" Inherits="Hidistro.UI.Web.AppDepot.OrderLogistics" %>

<!doctype html>
<html lang="zh-CN">
<head>
    <title>订单物流信息</title>
    <meta charset="UTF-8" />
    <meta content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=1" name="viewport" />
    <meta http-equiv="content-script-type" content="text/javascript">
    <meta name="format-detection" content="telephone=no" />
    <!-- uc强制竖屏 -->
    <meta name="screen-orientation" content="portrait">
    <!-- QQ强制竖屏 -->
    <meta name="x5-orientation" content="portrait">
    <link rel="stylesheet" href="../templates/common/style/fonts/style.css" />
    <link rel="stylesheet" href="../templates/common/style/index.css" />
    <style>
        #LogisticsInfoPanel { padding: 10px; }
        #divlogisticsInfo {  clear: both; }
        .map-box { height: 320px; }
        .expressSpan{ float:left;width:100%}
        .expressSpan li{ font-size: 16px;color: #666;float: left;width: 50%;line-height: 40px;}
    </style>
    <script src="/templates/appshop/script/jquery-1.11.0.min.js" id="jquery"></script>
    <script src="/templates/appshop/script/jquery.slides.min.js"></script>
    <script src="/Utility/bootstrap-validation.min.js"></script>
    <script src="/Utility/common.js?v=3.0" type="text/javascript"></script>
    <script src="/templates/appshop/script/main.js"></script>
    <script src="/templates/appshop/script/iscroll.js"></script>
    <script language="javascript" type="text/javascript">
        $(document).ready(function (e) {
            var loadMessage = "";
            if ($("#LoadMessage").length > 0)
                loadMessage = decodeURI($("#LoadMessage").val().Trim());
            var errorToPage = "";
            if ($("#ErrorToPage").length > 0)
                errorToPage = $("#ErrorToPage").val().Trim();
            if (loadMessage != undefined && loadMessage != "") {
                alert_h(loadMessage, function (e) {
                    if (errorToPage != "") {
                        returnUrl = getParam("returnUrl");
                        referrerUrl = document.referrer;
                        if (errorToPage == "goReturnUrl") {
                            if (returnUrl != undefined && returnUrl != "") {
                                document.location.replace(returnUrl);
                            }
                        }
                        else if (errorToPage == "goReferralUrl") {
                            if (referrerUrl != undefined && referrerUrl != "") {
                                document.location.replace(referrerUrl);
                            }
                        }
                        else {
                            document.location.replace(errorToPage);
                        }
                    }
                });
            }
        });
    </script>


</head>
<body>
    <div class="pbox" id="LogisticsInfoPanel" runat="server" clientidmode="static">
        <input type="hidden" id="hidOrderId" runat="server" clientidmode="static" value="" />
        <input type="hidden" id="hidReplaceType" runat="server" clientidmode="static" />
        <div class="logistics_1">
            <span class="icon_delivery icon-icon_express"></span><span id="logisticeinfo">
                <asp:Literal ID="ltlExpressCompanyName" runat="server"></asp:Literal><asp:Literal ID="ltlShipOrderNumber" runat="server"></asp:Literal></span>
        </div>
        <div class="step1 fl" style="padding-right: 15px;">
            <div class="step1_left">
                <span class="icon_adress icon-icon_address"></span>
            </div>
            <div class="m step1-in ">
                <a href="javascript:void(0)">
                    <div class="s1-name">
                        <span>
                            <asp:Literal ID="ltlReceiveName" runat="server"></asp:Literal>，<asp:Literal ID="ltlTel" runat="server"></asp:Literal></span>
                    </div>
                    <div class="color_6">
                        <asp:Literal ID="ltlShipAddress" runat="server"></asp:Literal>
                    </div>
                </a>
            </div>
            <b class="s1-borderB"></b>
        </div>
        <ul  class="expressSpan" id="ulExpress" runat="server">
             <li>物流查询</li>
             <li style="text-align:right"><asp:HyperLink runat="server" ID="hylExpress100Search" Target="_blank">快递100查询></asp:HyperLink></li>
        </ul>       
        <div class="logistics_2 mt_15" id="divlogisticsInfo" runat="server" clientidmode="static">
        </div>
    </div>
    <input type="hidden" id="hidIsShowDadaGIS" runat="server" clientidmode="static" />
    <input type="hidden" id="hidUserLatlng" runat="server" clientidmode="static" />
    <input type="hidden" id="hidStoreLatlng" runat="server" clientidmode="static" />
    <input type="hidden" id="hidBaseURL" runat="server" clientidmode="static" />
    <div class="qqmapbox">
        <div class="map-box" id="qqmapcontainer"></div>
    </div>
    <script type="text/javascript">

        $(document).ready(function () {
            var canShowMap = $("#hidIsShowDadaGIS").val() == "1";
            if (!canShowMap) {
                var returnsId = parseInt(getParam("returnsId"));
                var replaceId = parseInt(getParam("replaceId"));
                if (!isNaN(replaceId) && replaceId > 0) {
                    getReturnOrReplaceExpressData(replaceId, "return");
                }
                else if (!isNaN(returnsId) && returnsId > 0) {
                    getReturnExpressData(returnsId);
                }
                else {
                    loadExpressDataInfo();
                }
            }
            else {
                $(".expressSpan").hide();
            }
        });

        function loadExpressDataInfo() {
            var url = '/API/VshopProcess.ashx';
            var html = "<ul>";
            $.ajax({
                type: "get",
                url: url,
                data: { action: 'Logistic', orderId: $("#hidOrderId").val() },
                dataType: "json",
                success: function (data) {
                    if (data != undefined && data.success == true && data.traces != "") {
                        var expressData = data.traces;
                        $(expressData).each(function () {
                            html += "<li>" +
                                "<div class=\"logistics_info\">" +
                                    "<span class=\"color_6\">" + this.acceptTime + "</span>" +
                                    "<span class=\"color_9\">" + this.acceptStation + "</span>" +
                                "</div>" +
                            "</li>";
                        });

                        html += "</ul>";
                        $("#divlogisticsInfo").html(html);
                    } else {
                        $("#divlogisticsInfo").html("暂无物流信息，请稍后再试！<br> 如有信息推送不及时，请到物流公司官网查询！");
                    }
                }
            });
        }

        ///获取退货的物流信息
        function getReturnExpressData(objId) {
            var url = '/API/VshopProcess.ashx';
            var expressData;
            var html = "<ul>";
            $.ajax({
                type: "get",
                url: url,
                data: { action: "ReturnLogistic", OperId: objId },
                dataType: "json",
                async: false,
                success: function (data) {
                    if (data != undefined && data.success == true && data.traces != "") {
                        var expressData = data.traces;
                        $(expressData).each(function () {
                            html += "<li>" +
                                "<div class=\"logistics_info\">" +
                                    "<span class=\"color_6\">" + this.acceptTime + "</span>" +
                                    "<span class=\"color_9\">" + this.acceptStation + "</span>" +
                                "</div>" +
                            "</li>";
                        });
                        html += "</ul>";
                        $("#divlogisticsInfo").html(html);
                    } else {
                        $("#divlogisticsInfo").html("暂无物流信息，请稍后再试！<br>本物流信息服务由快递鸟提供，如有信息推送不及时，请到物流公司官网查询！");
                    }
                }
            });
        }

        ///获取换货的物流信息
        function getReplaceExpressData(objId) {
            var url = '/API/VshopProcess.ashx';
            var expressData;
            var html = "<ul>";
            $.ajax({
                type: "get",
                url: url,
                data: { action: "ReplaceLogistic", OperId: objId, InfoType: $("#hidReplaceType").val() },
                dataType: "json",
                async: false,
                success: function (data) {
                    if (data != undefined && data.success == true && data.traces != "") {
                        var expressData = data.traces;
                        $(expressData).each(function () {
                            html += "<li>" +
                                "<div class=\"logistics_info\">" +
                                    "<span class=\"color_6\">" + this.acceptTime + "</span>" +
                                    "<span class=\"color_9\">" + this.acceptStation + "</span>" +
                                "</div>" +
                            "</li>";
                        });
                        html += "</ul>";
                        $("#divlogisticsInfo").html(html);
                    } else {
                        $("#divlogisticsInfo").html("暂无物流信息，请稍后再试！<br>本物流信息服务由快递鸟提供，如有信息推送不及时，请到物流公司官网查询！");
                    }
                }
            });
        }
    </script>

    <script type="text/javascript" charset="utf-8" src="https://map.qq.com/api/js?v=2.exp"></script>
    <script>
        var baseurl = $("#hidBaseURL").val();
        var map, dadaStatus, riderTips = "", riderDistance = "", targetLatlng;
        var userlatlng = $("#hidUserLatlng").val(), storelatlng = $("#hidStoreLatlng").val();
        var canShowMap = $("#hidIsShowDadaGIS").val() == "1";
        var userlat, userlng, storelat, storelng, riderlat, riderlng;
        if (userlatlng.length > 0 && userlatlng.indexOf(",") >= 0) {
            userlat = parseFloat(userlatlng.split(",")[0]);
            userlng = parseFloat(userlatlng.split(",")[1]);
        }
        if (storelatlng.length > 0 && storelatlng.indexOf(",") >= 0) {
            storelat = parseFloat(storelatlng.split(",")[0]);
            storelng = parseFloat(storelatlng.split(",")[1]);
        }
        canShowMap = canShowMap && !isNaN(userlat) && !isNaN(userlng) && !isNaN(storelat) && !isNaN(storelng);

        if (canShowMap) {
            initShowMap();
        }

        function initShowMap() {
            //处理骑手信息
            $.ajax({
                type: "get",
                url: "/Api/LogisticsHandler.ashx?",
                data: { action: 'orderStatusQuery', order_id: $("#hidOrderId").val() },
                dataType: "json",
                success: function (data) {
                    if (data && data.status == "success" && data.result) {
                        dadaStatus = data.result.statusCode;
                        var loginfo = data.result.statusMsg;
                        if (dadaStatus == 2) {
                            canShowMap = canShowMap && true;
                            riderTips = "距商家";
                            targetLatlng = storelatlng;
                        }
                        if (dadaStatus == 3) {
                            canShowMap = canShowMap && true;
                            riderTips = "距客户";
                            targetLatlng = userlatlng;
                        }
                        if (dadaStatus == 2 || dadaStatus == 3 || dadaStatus == 4) {
                            loginfo += "<br>骑手：" + data.result.transporterName + "," + data.result.transporterPhone;
                        }
                        $("#logisticeinfo").html(loginfo);
                        if (canShowMap) {
                            riderlat = parseFloat(data.result.transporterLat);
                            riderlng = parseFloat(data.result.transporterLng);
                            if (dadaStatus == 4) {
                                riderlat = userlat;
                                riderlng = userlng;
                            }
                            if (!isNaN(riderlat) && !isNaN(riderlng)) {
                                $.ajax({
                                    type: "get",
                                    url: "/Api/LogisticsHandler.ashx?",
                                    data: { action: 'getDistance', onelatlng: targetLatlng, twolatlng: riderlat + "," + riderlng },
                                    dataType: "json",
                                    success: function (data) {
                                        if (data) {
                                            if (data.result > 1000) {
                                                riderDistance = (data.result / 1000).toFixed(2) + "KM";
                                            } else {
                                                riderDistance = data.result.toFixed(2) + "M"
                                            }
                                            $("#divlogisticsInfo").hide();
                                            initMap(getMiddelNumber(userlat, riderlat), getMiddelNumber(userlng, riderlng), data.result);
                                        }
                                    }
                                });
                            }
                        }
                    }
                    else {
                        if (data.status == "fail") {
                            ShowMsg(data.msg);
                        }
                    }
                }
            });
        }

        function initMap(lat, lng, distance) {
            var zoom = 14;
            if (distance > 4000) {
                zoom = 13;
            }
            if (distance > 40000) {
                zoom = 11;
            }
            if (distance < 1000) {
                zoom = 15;
            }
            var center = new qq.maps.LatLng(lat, lng);
            map = new qq.maps.Map(document.getElementById('qqmapcontainer'), {
                center: center,
                zoom: zoom
            });
            addMapMarker(map, new qq.maps.LatLng(userlat, userlng), "客户", baseurl + "/templates/pccommon/images/icon_map_my.png", 32, 40);
            addMapMarker(map, new qq.maps.LatLng(storelat, storelng), "门店", baseurl + "/templates/pccommon/images/icon_map_store.png", 32, 40);
            if (dadaStatus == 2 || dadaStatus == 3) {
                addMapMarker(map, new qq.maps.LatLng(riderlat, riderlng), riderTips + riderDistance, baseurl + "/templates/pccommon/images/icon_map_rider.png", 40, 50);
            }
            addReloadBtn(map);
        }
        function addReloadBtn(map) {
            var controlDiv=document.createElement("div");
            controlDiv.style.padding = "3px";
            controlDiv.style.backgroundColor = "#FFFFFF";
            controlDiv.style.border = "1px solid #666";
            controlDiv.style.margin = "0 10px 30px 0";

            controlDiv.index = 1;//设置在当前布局中的位置
            controlDiv.innerHTML = "<img src='/templates/common/images/main/posi-icon1.png' width='16'>&nbsp;重新加载";
            controlDiv.onclick = function () {
                initShowMap();
            };
            map.controls[qq.maps.ControlPosition.BOTTOM_RIGHT].push(controlDiv);
        }
        function addMapMarker(map, latlng, tips, tipsicon, w, h) {
            var marker = new qq.maps.Marker({
                title: tips,
                position: latlng,
                map: map,
            });
            var anchor = new qq.maps.Point(w / 2, h),
                 size = new qq.maps.Size(w, h),
                 origin = new qq.maps.Point(0, 0),
                 markerIcon = new qq.maps.MarkerImage(
             tipsicon,
             size,
             origin,
             anchor
           );
            var label = new qq.maps.Label({
                position: latlng,
                map: map,
                content: tips,
                style: { color: "#f00", fontSize: "12px", fontWeight: "bold", boxshadow: "3px 4px 3px #aaa" },
                offset: new qq.maps.Size(-16, h > 40 ? -73 : -62)
            });
            marker.setIcon(markerIcon);
        }
        function getMiddelNumber(one, two) {
            var _tmp;
            if (one > two) {
                _tmp = two;
                two = one;
                one = _tmp;
            }
            _tmp = (two - one) / 2;
            return one + _tmp;
        }
    </script>
</body>
</html>
