<%@ Control Language="C#" %>
<%@ Register TagPrefix="Hi" Namespace="Hidistro.UI.Common.Controls" Assembly="Hidistro.UI.Common.Controls" %>
<%@ Register TagPrefix="Hi" Namespace="Hidistro.UI.SaleSystem.Tags" Assembly="Hidistro.UI.SaleSystem.Tags" %>
<%@ Import Namespace="Hidistro.Core" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
    <meta name="renderer" content="webkit" />
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <script type="text/javascript" src="/utility/jquery-1.8.3.min.js"></script>
    <Hi:HeadContainer ID="HeadContainer1" runat="server" />
    <script type="text/javascript">
        //如果是网站首页直接跳转到WAP端首页，如果是其它页面，且该页面对应WAP端有相应的页面对直接中转到对应的WAP页面，如果没有则不跳转
        var pageurl = document.location.href.toLowerCase();
        var sUserAgent = navigator.userAgent.toLowerCase();
        var bIsIpad = sUserAgent.match(/ipad/i) == "ipad";
        var bIsIphoneOs = sUserAgent.match(/iphone os/i) == "iphone os";
        var bIsMidp = sUserAgent.match(/midp/i) == "midp";
        var bIsUc7 = sUserAgent.match(/rv:1.2.3.4/i) == "rv:1.2.3.4";
        var bIsUc = sUserAgent.match(/ucweb/i) == "ucweb";
        var bIsAndroid = sUserAgent.match(/android/i) == "android";
        var bIsCE = sUserAgent.match(/windows ce/i) == "windows ce";
        var bIsWM = sUserAgent.match(/windows mobile/i) == "windows mobile";
        var bIsWX = sUserAgent.match(/micromessenger/i) == "micromessenger";
        //bIsWM = true;

        if ((bIsIpad || bIsIphoneOs || bIsMidp || bIsUc7 || bIsUc || bIsAndroid || bIsCE || bIsWM || bIsWX) && (HasWapRight || HasVshopRight)) {
            //解决分享到微信时，浏览器跳转到网站首页的问题
            DirectUrl = GetWapUrl();
            if (DirectUrl != "") {
                if (bIsWX && HasVshopRight)
                    location.href = "/vShop/" + DirectUrl;
                else if (HasWapRight)
                    location.href = "/Wapshop/" + DirectUrl;
            }
        }
        ///判断是否需要跳转，如果当前页面为首页或者帮助页，文单页则跳转，其它的不需要跳转
        function IsDirect() {

            if (pageurl.indexOf("/default.aspx") > -1) return true;
            if (pageurl.indexOf("/article/") >= -1 && pageurl.indexOf("/help/") >= -1) return false;
            return true;
        }
        ///获取PC端对应Wap端的页面地址，如果没有对应地址则返回空
        function GetWapUrl() {
            var PageKey = "";
            var port = document.location.port;
            var domain = document.domain;
            var param = document.location.search;
            if (port != "80" && port != "") domain = domain + ":" + port;
            domain = document.location.protocol + "//" + domain;

            if ((pageurl.length == domain.length || (pageurl.length - 1) == domain.length) && pageurl.indexOf(domain) == 0) { return "default.aspx"; }
            pageurl = pageurl.replace(domain.toLowerCase(), "");
            if (pageurl.indexOf("/default") > -1) { return "/default"; }
            if (pageurl.indexOf("/groupbuyproductdetails") > -1) { return "groupbuyproductdetails" + param }
            if (pageurl.indexOf("/countdownproductsdetails") > -1) { return "countdownproductsdetails" + param }
            if (pageurl.indexOf("/product_detail") > -1) { return "ProductDetails?productId=" + pageurl.replace("/product_detail/", "").replace(param.toLowerCase(), "") + param.replace("?", "&"); }
            if (pageurl.indexOf("/productdetails") > -1) { return "ProductDetails" + param; }
            if (pageurl.indexOf("/presaleproductdetails") > -1) { return "presaleproductdetails" + pageurl.replace("/presaleproductdetails", ""); }
            if (pageurl.indexOf("/brand_detail") > -1) { return "BrandDetail?brandId=" + pageurl.replace("/brand/brand_detail/", ""); }
            if (pageurl.indexOf("/brand") > -1) { return "brandlist" + pageurl.replace("/brand", ""); }
            if (pageurl.indexOf("/browse/category") > -1) { return "ProductList?categoryId=" + pageurl.replace("/browse/category/", ""); }
            if (pageurl.indexOf("/category") > -1) { return "productsearch"; }
            if (pageurl.indexOf("/subcategory") > -1) { return "ProductList" + pageurl.replace("/subcategory", ""); }
            if (pageurl.indexOf("/groupbuyproducts") > -1) { return "GroupBuyList"; }
            if (pageurl.indexOf("/countdownproducts") > -1) { return "CountDownProducts"; }
            if (pageurl.indexOf("/login") > -1) { return "login"; }


            if (pageurl.indexOf("userdefault") > -1) { return "MemberCenter.aspx"; }
            if (pageurl.indexOf("userorders") > -1) { return "MemberOrders.aspx"; }
            if (pageurl.indexOf("usershippingaddresses") > -1) { return "ShippingAddresses.aspx" }
            if (pageurl.indexOf("user/referralregisteragreement") > -1) { return "ReferralRegisterAgreement.aspx"; }
            if (pageurl.indexOf("user/referralregisterresults") > -1) { return "referralregisterresults"; }
            if (pageurl.indexOf("/user/") > -1) { return "MemberCenter.aspx"; }
            if (pageurl.indexOf("articles") > -1) { return "Articles.aspx" + param; }
            if (pageurl.indexOf("articledetails") > -1) { return "ArticleDetails" + param; }
            if (pageurl.indexOf("appdownload") > -1) { return "appdownload.aspx"; }
            if (pageurl.indexOf("registeredcoupons") > -1) { return "registeredcoupons.aspx"; }//解决扫码登录无法跳转的bug
            if (pageurl.indexOf("referralagreement") > -1) { return "ReferralAgreement" + param; }
            if (pageurl.indexOf("register") > -1) { return "login?action=register"; }

            if (pageurl.indexOf("/un_product_detail") > -1) { return "ProductDetails?productId=" + pageurl.replace("/un_product_detail/", "").replace(param.toLowerCase(), "") + param.replace("?", "&"); }
            return "";
        }

        function Resize() {
            var width = $(window).width(),
                height = $(window).height();
            $("body").css({
                "width": width,
                "height": height,
            });
            if (width <= 1200) {
                $("body").css({
                    "overflow": "auto"
                });
            }
        };

        function GetPageKey(pagepre, IsEnd) {
            if (pageurl.indexOf(pagepre) > -1) {
                var endIndex = 0;
                if (!IsEnd) {
                    endIndex = pageurl.indexOf(".aspx");
                    if (endIndex <= -1) endIndex = pageurl.indexOf(".htm")
                }
                else {
                    endIndex = pageurl.length;
                }
                var startIndex = pageurl.indexOf(pagepre) + pagepre.length;
                if (startIndex >= 0 && endIndex > startIndex)
                    return pageurl.substring(startIndex, endIndex);
            }
            return "0";
        }</script>
    <Hi:PageTitle runat="server" />
    <Hi:MetaTags runat="server" />
    <script type="text/javascript" Src="/utility/jquery-1.8.3.min.js" ></script>
    <script defer="defer" src="/Utility/icheck/icheck.min.js"></script>
    <script type="text/javascript" src="/templates/master/default/script/nav.js"></script>
    <script type="text/javascript" src="/templates/pccommon/script/nav.js"></script>
   <script type="text/javascript" Src="/utility/globals.js" ></script>
   <script type="text/javascript" Src="/utility/region.helper.js"></script>
    <link rel="stylesheet" type="text/css" href="/admin/css/bootstrap.min.css" />
    <link rel="stylesheet" type="text/css" href="/templates/pccommon/style/nav.css" />
    <Hi:TemplateStyle ID="Stylee1" runat="server" Href="/style/home.css"></Hi:TemplateStyle>
     <Hi:TemplateStyle ID="TemplateStyle1" runat="server" Href="/style/style.css"></Hi:TemplateStyle>
    <Hi:TemplateStyle ID="Style1" runat="server" Href="/style/menu.css"></Hi:TemplateStyle>
    <link href="/templates/master/fangetao/style/css/font-awesome.min.css" rel="stylesheet" />
    <link href="/Utility/iconfont/iconfont.css" rel="Stylesheet" />
    <Hi:TemplateStyle ID="RegionStyle" runat="server" Href="/style/region.css"></Hi:TemplateStyle>
    <script defer="defer" src="/Utility/windows.js"></script>
    <link href="/Admin/css/windows.css" rel="stylesheet" />
    <asp:Literal runat="server" ID="MeiQia_OnlineServer"></asp:Literal>
</head>
<style>
    * {
        box-sizing: content-box;
    }
</style>



<body onResize="Resize()">
    <Hi:Common_OnlineServer ID="Common_OnlineServer1" runat="server"></Hi:Common_OnlineServer>
 <div class="header clearfix" id="header">
	<div class="top-banner">
  	<div class="w1200"><Hi:Common_CustomAd runat="server" AdId="54" /></div>
  </div>
  <div class="header-wrap">
    <div class="header-wrap-nav" id="ty_menu_title">
      <ul class="clearfix">
        <li class="home"><a href="/"><span>首页</span></a></li>
        <Hi:Common_PrimaryClass ID="Common_PrimaryClass1" runat="server" />
        <Hi:Common_HeaderMune ID="Common_HeaderMune1" runat="server" />
        <li class="QRcode" style="display:none;">
          <a href="#">手机一淘
            <i class="icon-qrcode"></i>
            <div class="QRcode-bd"><img src="/templates/master/fangetao/images/etao/QRcode.png" />手机购买</div>
          </a>
        </li>
        <li >
                                <Hi:Common_AppDownloadQRCode ID="Common_AppDownloadQRCode1" runat="server" /></li>
        <li style="display:none;"><Hi:Common_SetDeliveryRegion ID="setDeliverRegion" OnlyShowInDefault="true" runat="server"></Hi:Common_SetDeliveryRegion></li>
        <i class="top-close icon-remove-sign" title="点击关闭"></i>
      </ul>
    </div>
    <div class="header-wrap-bd">
     <div style=" float:left;"> <Hi:Common_Logo ID="Common_Logo1" runat="server" /></div>
      <div class="header-search"> <Hi:Common_Search ID="Common_Search" runat="server" SkinName="/hometags/Common_Search/Skin-Common_Search.ascx"/></div>
      <div class="fr top1_r header-login">
                        <ul>
                            <li>
                                <Hi:Common_MyAccountLink ID="linkMyAccount" runat="server" /></li>                       
                            <li id="liLinkLogin">
                               <Hi:Common_LoginLink ID="Common_Link_Login1" runat="server" /></li>
                            <li class="fore3">
                                 <Hi:Common_VshopQRCode ID="Common_VshopQRCode1" runat="server" />
                            </li>
                        </ul>
                    </div>
    </div>
  </div>
</div>   
    
  

 
<!--my_left_category-->
<script>
    var currenturl = window.location.pathname;
    currenturl = currenturl.toLowerCase();
    if (currenturl != "/" && currenturl != null && currenturl.indexOf("default.aspx") == -1 && currenturl.indexOf("desig_templete.aspx?skintemp=default") == -1) {
        $(".all").hover(function () {
            $(".my_left_category").css({ 'display': 'block' });
        }, function () {
            $(".my_left_category").css({ 'display': 'none' });
        });
    } else {
        $(".my_left_category").css({ 'display': 'block' });
    }
    //$(function(){
    $(".h2_cat").each(function () {
        $(this).hover(function () {
            if ($(this).hasClass('active_cat')) {
                $(this).removeClass("active_cat");
            } else {
                $(this).addClass('active_cat');
            }
        });
    });
    //})
</script>

<!--xinren-->
<script type="text/javascript">
    function Showxinren_tab() {
        document.getElementById("xinren_tab").style.display = "block";
    }
    function Hiddenxinren_tab() {
        document.getElementById("xinren_tab").style.display = "none";
    }
</script>

<script>
    $(document).ready(function () {
        $(".QRcode").hover(function () {
            $(".QRcode-bd").show();
        }, function () {
            $(".QRcode-bd").hide();
        });
        $(".top-banner img").load(function () {
            var HTopBanner = $(".top-banner").height();
            $(".top-close").css("top", -(HTopBanner));
        });
        $(".top-close").toggle(function () {
            $(".top-close").removeClass("icon-remove-sign").addClass("icon-plus-sign").attr("title", "点击展开");
            $(".top-banner").slideToggle();
            $(".top-close").animate({ "top": 5 });
        }, function () {
            $(".top-close").removeClass("icon-plus-sign").addClass("icon-remove-sign").attr("title", "点击关闭");
            $(".top-banner").slideToggle();
            $(".top-close").animate({ "top": -(HTopBanner) });
        });
    });
</script>


    <script>

        $(document).ready(function () {
            $('.dialog_title_r').click(function () {
                $('.login_tan').hide();
                $('.modal_qt').hide();
            })
            var hidIsLogin = $("#hidIsLogin");
            if (hidIsLogin.val() == "1") {
                $("#liLinkLogin").addClass("fore3");
            } else {
                $("#liLinkLogin").removeClass("fore3");
            }
$("#liLinkLogin a").eq(1).css("display","none");  
$("#liLinkLogin a").eq(2).css("display","none");
$("#liLinkLogin a").eq(3).css("display","none");

        });

    </script>
