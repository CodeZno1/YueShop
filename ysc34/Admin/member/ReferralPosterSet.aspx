<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="ReferralPosterSet.aspx.cs" Inherits="Hidistro.UI.Web.Admin.ReferralPosterSet" %>

<%@ Register TagPrefix="Hi" Namespace="Hidistro.UI.Common.Controls" Assembly="Hidistro.UI.Common.Controls" %>



<%@ Import Namespace="Hidistro.Core" %>
<asp:Content ID="Content3" ContentPlaceHolderID="validateHolder" runat="server">
   <script type="text/javascript" src="/utility/jquery.hishopUpload.js" ></script>
    <script type="text/javascript" src="/utility/jquery.form.js"></script>
    <link id="cssLink" rel="stylesheet" href="../css/style.css" type="text/css" media="screen" />
    <script type="text/javascript" src="/utility/jquery_hashtable.js"></script>
    <script type="text/javascript" src="/utility/jquery-powerFloat-min.js"></script>
    <link href="../css/bootstrap-switch.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/bootstrap-switch.js"></script>
    <script type="text/javascript" src="/admin/js/jquery-json-2.4.js"></script>
    <script type="text/javascript" src="/admin/js/jscolor.min.js"></script>
    <script type="text/javascript" src="../js/zclip/jquery.zclip.min.js"></script>
    <style type="text/css">
        input[type="color"] { width: 40px; border: none; vertical-align: middle; }
        .upFile .adds { width: 100px; height: 100px; color: #FFF; position: absolute; top: 0px; text-align: center; line-height: 100px; font-size: 28px; font-weight: 600; }
        .upFile .bgImg { border: 2px dashed #FF8182; width: 100px; height: 100px; margin-bottom: 5px; margin-right: 10px; cursor: pointer; }
        .upFile img { width: 100%; height: 100%; }
        .upFile .adds input { position: absolute; top: 0px; width: 100px; height: 100px; display: block; overflow: hidden; text-indent: -9999px; opacity: 0; cursor: pointer; }
        .Vjscolor { display: none; }
        .jscolor { width: 40px; font-size: 12px; line-height: 23px; border: none; vertical-align: middle; color: #eee; }
        .resetSize { width: 90px; margin: 0px; padding: 0px; }
    </style>
    <script type="text/javascript">

        var alert_h = function (msg) {
            alert(msg);
        }


        function showResponse(data) {


            if (data == "0") {
                alert_h("背景图片上传图片失败!");
                $("#preservation").attr("disabled", false);
                return;
            } else if (data == "1") {
                alert_h("背景图片大小超过1M，不能上传!");
                $("#preservation").attr("disabled", false);
                return;

            } else if (data == "2") {
                alert_h("上传的背景图片文件格式不正确！上传格式有(.gif、.jpg、.png、.bmp)!");
                $("#preservation").attr("disabled", false);
                return;

            } else if (data == "3" || data == "") {
                data = ybgimg;  //如果没有图片上,使用默认图片
            }

            var jsonArr = [];
            $('.touchmove').each(function () {
                jsonArr.push({
                    left: parseInt($(this).css('left')) / 320,
                    top: parseInt($(this).css('top')),
                    width: $(this).find('img').width() / 320,
                })
            })


            var $color = $(".Vjscolor");

            var postData = {
                posList: jsonArr,
                DefaultHead: $('input:radio[name=UserHeader]:checked').val(),
                myusername: $('#myusername').val(),
                shopname: $('#shopname').val(),
                BgImg: '--BigImagePlaceholder--',
                myusernameSize: $("#myusernameSize").val(),
                shopnameSize: $("#shopnameSize").val(),
                myusernameColor: "#" + $color.eq(0).val(),
                shopnameColor: "#" + $color.eq(2).val(),
                nickNameColor: "#" + $color.eq(1).val(),
                storeNameColor: "#" + $color.eq(3).val()
            }

            if (postData.myusernameColor == "#") {
                postData.myusernameColor = "#0F0F0F";
            }
            if (postData.shopnameColor == "#") {
                postData.shopnameColor = "#C5D234";
            }
            if (postData.nickNameColor == "#") {
                postData.nickNameColor = "#DE6814";
            }
            if (postData.storeNameColor == "#") {
                postData.storeNameColor = "#F8F1FF";
            }




            if (!/^\d+$/.test(postData.myusernameSize) || !/^\d+$/.test(postData.shopnameSize)) {
                alert_h("文字大小请填写整数值！");
                $("#preservation").attr("disabled", false);
                return;
            }




            $.ajax({
                url: "ReferralPosterSet",
                type: 'post', dataType: 'json', timeout: 10000,
                data: { action: "Edit", SotreCardJson: $.toJSON(postData), tempImage: data },
                cache: false,
                success: function (resultData) {
                    if (resultData.success) {

                        alert("保存成功！");
                        $("#preservation").attr("disabled", false);
                    } else {

                        alert_h(resultData.msg);
                        $("#preservation").attr("disabled", false);
                    }
                },
                error: function (data, status, e) {

                    alert_h("系统错误，请重试！");
                    $("#preservation").attr("disabled", false);
                }
            });


        }


        function ReadSetData(SetData) {
            $("#idImg").attr("src", SetData.BgImg);
            $('.exitbusinesscard').css({
                "background": 'url(' + SetData.BgImg + ') no-repeat center',
                'backgroundSize': 'cover'
            });

            $('input:radio[name=UserHeader][value=' + SetData.DefaultHead + ']').attr("checked", true); //选中
            $('input:radio[name=UserHeader][value=' + SetData.DefaultHead + ']').trigger("click");


            //延迟处理一下，等待jscolor执行完，以免颜色不对
            setTimeout(function () {

                $color = $(".Vjscolor");

                $color.eq(0).val(SetData.myusernameColor.replace("#", ""));
                $color.eq(1).val(SetData.nickNameColor.replace("#", ""));
                $color.eq(2).val(SetData.shopnameColor.replace("#", ""));
                $color.eq(3).val(SetData.storeNameColor.replace("#", ""));

                $jscolor = $(".jscolor");
                $jscolor.eq(0).css("background", SetData.myusernameColor);
                $jscolor.eq(1).css("background", SetData.nickNameColor);
                $jscolor.eq(2).css("background", SetData.shopnameColor);
                $jscolor.eq(3).css("background", SetData.storeNameColor);
                $color.trigger("change");

            }, 100);



            $("#myusername").val(SetData.myusername);
            $("#shopname").val(SetData.shopname);

            $("#myusername,#shopname").trigger("keyup");

            $("#myusernameSize").val(SetData.myusernameSize);
            $("#shopnameSize").val(SetData.shopnameSize);
            $("#myusernameSize,#shopnameSize").trigger("change");




            $('.touchmove').each(function (i) {
                $(this).css({ "left": SetData.posList[i].left * 320 + "px", "top": SetData.posList[i].top + "px" });
                $(this).find('img').width(SetData.posList[i].width * 320).height(SetData.posList[i].width * 320);
            })



        }

        var ybgimg = "/Utility/pics/referralposterbg.png"; //默认图片


        //获取上传成功后的图片路径
        function getUploadImages() {
            var srcLogo = $('#logoContainer span[name="siteLogo"]').hishopUpload("getImgSrc");
            $("#<%=hidUploadLogo.ClientID%>").val(srcLogo);
            return srcLogo;
        }
        $(function () {

            $('#btnCopy').zclip({
                path: "../js/zclip/ZeroClipboard.swf",
                copy: function () {
                    return $('#copyContent input').val();
                },
                afterCopy: function () {
                    alert("成功复制到剪切板：" + $('#copyContent input').val());
                }
            });
            var imgSrc = '<%=hidUploadLogo.Value%>';
            $('#logoContainer span[name="siteLogo"]').hishopUpload(
            {
                title: '缩略图',
                url: "/admin/UploadHandler.ashx?action=newupload",
                imageDescript: '',
                imgFieldName: "siteLogo",
                pictureSize: '',
                imagesCount: 1,
                displayImgSrc: imgSrc,
                callback: function (imageUrl) {
                    if (imageUrl != "") {
                        //if (bgurl.indexOf("IE^") == 0) {
                        //    //兼容IE浏览器的方法
                        //    var bgurls = bgurl.split("^");
                        //    $('.exitbusinesscard').css({
                        //        'filter': 'progid:DXImageTransform.Microsoft.AlphaImageLoader(sizingMethod=scale)',
                        //        'width': '320px',
                        //        'height': '490px'
                        //    });
                        //    $('.exitbusinesscard')[0].filters.item("DXImageTransform.Microsoft.AlphaImageLoader").src = bgurls[1];

                        //}
                        //else {
                        $('.exitbusinesscard').css({
                            "background": 'url(' + imageUrl + ') no-repeat center',
                            'backgroundSize': 'cover'
                        });
                        //}
                    }
                }
            });
            ///是否启用头像
            $('input:radio[name=UserHeader]').click(function () {
                if ($(this).val() == 2) {
                    $("#HeadPanel").hide();
                } else {
                    $("#HeadPanel").show();
                };
            });

            //调整字体大小
            $("#myusernameSize,#shopnameSize").change(function () {

                var flag = $(this).attr("flag");

                if (!/^\d+$/.test($(this).val())) {
                    alert_h("请填整数值！");
                    return;
                }


                var resize = parseInt($(this).val());
                if (flag == 1) {
                    $("#UnamePanel").css("fontSize", resize);

                } else {
                    $("#DescPanel").css("fontSize", resize);
                }
            });


            //调整字体大小
            $("#myusername,#shopname").keyup(function () {
                var flag = $(this).attr("flag");
                var ctx = $(this).val();
                var Thtml = ctx.replace("{{昵称}}", "<strong>昵称</strong>").replace("{{商城名称}}", "<strong>商城名称</strong>");
                if (flag == 1) {
                    $("#UnamePanel").html(Thtml);
                } else {
                    $("#DescPanel").html(Thtml);
                }
                $("#DescPanel strong").css("color", "#" + $("#chosen-value4").val());
            });


            //input type="color" 调整颜色

            $(".Vjscolor").change(function () {
                var flag = $(this).attr("flag");
                var recolor = "#" + $(this).val();



                if (flag == 1) {
                    $("#UnamePanel").css("color", recolor);
                }
                else if (flag == 2) {
                    $("#UnamePanel").find("strong").css("color", recolor);
                }
                else if (flag == 3) {
                    $("#DescPanel").css("color", recolor);
                }
                else if (flag == 4) {

                    $("#DescPanel").find("strong").css("color", recolor);
                }

            });

            $('body').on('mousedown', '.touchmove', function (evt) {
                evt.preventDefault();
                var downX = evt.clientX,
                    downY = evt.clientY,
                    topN = downY - parseInt($(this).css('top')),
                    leftN = downX - parseInt($(this).css('left')),
                    winH = $('.exitbusinesscard').outerHeight(),
                    elemH = $(this).height(),
                    winW = $('.exitbusinesscard').width(),
                    elemW = $(this).outerWidth(),
                    _this = $(this);
                _this.css('zIndex', 100);
                $(document).on('mousemove', function (evt) {
                    var moveX = evt.clientX - leftN,
                        moveY = evt.clientY - topN;
                    if (moveY < 0) moveY = 0;
                    if (moveY > winH - elemH) moveY = winH - elemH;
                    if (moveX < 0) moveX = 0;
                    if (moveX > winW - elemW) moveX = winW - elemW;
                    _this.css({
                        'left': moveX,
                        'top': moveY
                    })
                })
                $(document).on('mouseup', function () {
                    $(document).off('mousemove');
                    $(document).off('mouseup');
                    _this.css('zIndex', 0);
                })
            });
            $('.touchmove .left,.touchmove .right').on('mousedown', function (evt) {
                evt.stopPropagation();
                evt.preventDefault();
                var downX = evt.clientX,
                    imgElement = $(this).parent().find('img'),
                    imgW = imgElement.width(),
                    imgH = imgElement.height(),
                    classNa = $(this)[0].className,
                    touchmoveEl = $(this).parent(),
                    moveL = parseInt(touchmoveEl.css('left')),
                    $obj = 320 - $(this).parent().width() - parseInt($(this).parent().css('left'));
                $(document).on('mousemove', function (evt) {
                    var moveX = evt.clientX - downX;

                    if (classNa == 'left') {
                        moveX = -moveX;
                        touchmoveEl.css('left', moveL - moveX / 2 + 'px')
                    }
                    if (moveX > $obj) {

                        return;
                    }
                    imgElement.css({
                        'width': imgW + moveX,
                        'height': imgH + moveX
                    })
                })
                $(document).on('mouseup', function () {
                    $(document).off('mousemove');
                    $(document).off('mouseup');
                })
            })
            $('.touchmove .bottom,.touchmove .top').on('mousedown', function (evt) {
                evt.stopPropagation();
                evt.preventDefault();
                var downY = evt.clientY,
                    imgElement = $(this).parent().find('img'),
                    imgW = imgElement.width(),
                    imgH = imgElement.height(),
                    classNa = $(this)[0].className,
                    touchmoveEl = $(this).parent(),
                    moveT = parseInt(touchmoveEl.css('top'));
                $(document).on('mousemove', function (evt) {
                    var moveY = evt.clientY - downY;
                    if (classNa == 'top') {
                        moveY = -moveY;
                        touchmoveEl.css('top', moveT - moveY / 2 + 'px')
                    }
                    imgElement.css({
                        'width': imgW + moveY,
                        'height': imgH + moveY
                    })
                })
                $(document).on('mouseup', function () {
                    $(document).off('mousemove');
                    $(document).off('mouseup');
                })
            })
            $('.cardqrcode .cardimg,.exitbusinesscard .img').hover(function () {
                $(this).addClass('border').find('span').show();
            }, function () {
                $(this).removeClass('border').find('span').hide();
            })
            $('#preservation').click(function () {
                $("#preservation").attr("disabled", true)
                showResponse(getUploadImages());

            });
            // $('#myusername').keydown(function (){
            //     $('.myusername strong').text($(this).val());
            // })



            //读取原设置数据
            jQuery.getJSON("/Storage/data/Utility/ReferralPoster.js", { random: Math.random() }, function (SetData) {
                if (SetData != null) {
                    ybgimg = SetData.BgImg;
                    $("#<%=hidUploadLogo.ClientID%>").val(ybgimg);
                    ReadSetData(SetData);
                }
            });


        })



    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="contentHolder" runat="server">
    <div class="dataarea mainwidth databody">
        <div class="title">
            <ul class="title-nav">
                <li><a href="DeductSettings.aspx">分佣规则设置</a></li>
                <li><a href="ReferralSettings.aspx">分销员申请设置</a></li>
                <li class="hover"><a href="javascript:void">分销海报设置</a></li>
                <li><a href="ExtendShareSettings.aspx">分销分享设置</a></li>
                <li><a href="RecruitPlanSettings.aspx">招募计划设置</a></li>
            </ul>
        </div>
        <div class="datafrom">
            <div class="edit-text-left">
                <div class="mobile-border">
                    <div class="mobile-d">
                        <div class="mobile-header">
                            <i></i>
                            <div class="mobile-title">分销海报</div>
                        </div>
                        <div class="upshop-view">
                            <div class="exitbusinesscard" style="overflow: hidden; background: url(/Utility/pics/referralposterbg.png) no-repeat center; background-size: 100%;">
                                <div style="z-index: 0; left: 92px; top: 24px;" id="HeadPanel" class="touchmove img">
                                    <img style="width: 133px; height: 133px;" src="/Utility/pics/headLogo.png" width="135">
                                    <span style="display: none;" class="left"></span>
                                    <span style="display: none;" class="top"></span>
                                    <span style="display: none;" class="right"></span>
                                    <span style="display: none;" class="bottom"></span>
                                </div>
                                <h1 class="touchmove myusername" id="UnamePanel">我是<strong>昵称</strong></h1>
                                <h3 class="touchmove shopname" id="DescPanel">邀请您加入<strong>商城名称</strong>分销团队</h3>
                                <div class="cardqrcode" id="CodePanel">
                                    <div class="touchmove cardimg">
                                        <img src="/Utility/pics/yscqrcode.jpg" width="200">
                                        <span style="display: none;" class="left"></span>
                                        <span style="display: none;" class="top"></span>
                                        <span style="display: none;" class="right"></span>
                                        <span style="display: none;" class="bottom"></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="mobile-footer"></div>
                    </div>
                </div>
            </div>

            <div class="edit-text-right">
                <div class="edit-inner" style="border-radius: 5px; width: 560px; margin-top: 70px;">
                    <ul>
                        <li class="content">
                            <br />
                            <span class="formitemtitle">链接地址：</span>
                            <div class="input-group">
                                <span id="copyContent">
                                    <asp:TextBox runat="server" CssClass="form_input form-control" ID="linkUrl" Width="350px" ReadOnly="true"></asp:TextBox></span>
                                <input type="button" class="btn btn-default" value="复制" id="btnCopy" />
                            </div>
                        </li>
                    </ul>
                    <strong style="color: red; font-weight: normal; display: block; margin-bottom: 30px;">注意：可以使用鼠标拖动左图中各元素,进行位置调整</strong>
                    <div class="form-horizontal">
                        <div class="form-group">
                            <label class="col-xs-2 pad resetSize control-label" for="setdate">默认头像：</label>
                            <div class="form-inline journal-query col-xs-9">
                                <div class="form-group pt3">
                                    <label class="middle mr10">
                                        <input type="radio" checked="checked" name="UserHeader" value="0">使用会员头像</label>
                                    <label class="middle mr10">
                                        <input type="radio" name="UserHeader" value="2">不使用头像</label>
                                </div>
                            </div>
                        </div>
                        <div class="form-group setmargin">
                            <label class="col-xs-2 pad resetSize control-label" for="setdate">个人介绍：</label>
                            <div class="form-inline journal-query col-xs-10">
                                <div class="form-group">
                                    <input type="text" class="form-control resetSize inputw200" id="myusername" flag="1" value="我是{{昵称}}">
                                    &nbsp;&nbsp;&nbsp;&nbsp;文字样式：
                                <button class="jscolor {valueElement:'chosen-value1'}">文字</button><input id="chosen-value1" type="text" class="Vjscolor" flag="1">
                                    <button class="jscolor {valueElement:'chosen-value2'}">昵称</button>
                                    <input type="text" class="Vjscolor" id="chosen-value2" flag="2">
                                    &nbsp;&nbsp;<input type="number" id="myusernameSize" flag="1" style="width: 40px; vertical-align: middle" value="14">&nbsp;像素
                                </div>
                                <small style="margin-left: -15px;">“{{昵称}}”为系统参数，实际显示以会员的昵称替代</small>
                            </div>
                        </div>
                        <div class="form-group setmargin">
                            <label class="col-xs-2 pad resetSize control-label" for="setdate">分销口号：</label>
                            <div class="form-inline journal-query col-xs-10">
                                <div class="form-group">

                                    <textarea id="shopname" flag="2" class="form-control resetSize inputw200" style="height: 50px">邀请您加入{{商城名称}}分销团队</textarea>
                                    &nbsp;&nbsp;&nbsp;&nbsp;文字样式：
                                <button class="jscolor {valueElement:'chosen-value3'}">文字</button>
                                    <input type="text" class="Vjscolor" id="chosen-value3" flag="3">
                                    <button class="jscolor {valueElement:'chosen-value4'}">店铺</button>
                                    <input type="text" class="Vjscolor" id="chosen-value4" flag="4">
                                    &nbsp;&nbsp;<input type="number" id="shopnameSize" flag="2" style="width: 40px; vertical-align: middle" value="14">&nbsp;像素
                                </div>
                                <small style="margin-left: -15px;">“{{商城名称}}”为系统参数，实际显示以商城名称替代</small>
                            </div>
                        </div>
                        <div class="form-group setmargin">
                            <label class="col-xs-2 pad resetSize control-label" for="setdate">背景图片：</label>
                            <div class="form-inline journal-query col-xs-9">
                                <div class="form-group pt3">
                                    <div id="logoContainer">
                                        <span name="siteLogo" class="imgbox"></span>
                                        <asp:HiddenField ID="hidUploadLogo" runat="server" />
                                    </div>
                                    <small style="margin-left: -15px;">点击图片上传，建议上传480*735px，小于1M，png、jpg格式图片</small>
                                </div>

                            </div>
                        </div>
                        <div class="form-group pt10">
                            <label class="col-xs-2 pad resetSize control-label" for="setdate">&nbsp;</label>
                            <div class="form-inline journal-query col-xs-9">
                                <button class="btn btn-primary" id="preservation" onclick="return false" style="margin-left: -15px; width: 100px;">保存</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>

