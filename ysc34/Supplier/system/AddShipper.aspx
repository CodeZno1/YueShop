<%@ Page Language="C#" MasterPageFile="~/Supplier/Admin.Master" AutoEventWireup="true" CodeBehind="AddShipper.aspx.cs" Inherits="Hidistro.UI.Web.Supplier.AddShipper" Title="无标题页" %>

<%@ Register TagPrefix="Hi" Namespace="Hidistro.UI.Common.Controls" Assembly="Hidistro.UI.Common.Controls" %>


<asp:Content ID="Content1" ContentPlaceHolderID="headHolder" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentHolder" runat="server">
    <div class="dataarea mainwidth databody">
        <div class="title">
            <ul class="title-nav">
                <li><a href="Shippers.aspx">管理</a></li>
                <li class="hover"><a href="javascript:void">添加</a></li>
            </ul>
        </div>

        <div class="datafrom">
            <div class="formitem validator1 depot-p">
                <ul>
                    <li class="mb_0"><span class="formitemtitle"><em>*</em>发货点：</span>
                        <asp:TextBox ID="txtShipperTag" CssClass="form_input_l form-control" runat="server" MaxLength="30" placeholder="用来选择从哪个点发货" />
                        <p id="ctl00_contentHolder_txtShipperTagTip"></p>
                    </li>
                    <li class="mb_0"><span class="formitemtitle"><em>*</em>发货人姓名：</span>
                        <asp:TextBox ID="txtShipperName" CssClass="forminput form-control" runat="server" MaxLength="20" placeholder="只能是汉字或字母开头" />
                        <p id="ctl00_contentHolder_txtShipperNameTip"></p>
                    </li>
                    <li><span class="formitemtitle"><em>*</em>发货地区：</span>
                        <abbr class="formselect">
                            <Hi:RegionSelector runat="server" ID="ddlReggion" />
                        </abbr>
                    </li>
                    <li class="mb_0" style="padding-top:10px;"><span class="formitemtitle"><em>*</em>发货详细地址：</span>
                        <asp:TextBox ID="txtAddress" CssClass="form_input_l form-control" runat="server" Width="450px" MaxLength="60" placeholder="长度在1-60个字符之间" />
                        <p id="ctl00_contentHolder_txtAddressTip"></p>
                    </li>
                    <li class="mb_0"><span class="formitemtitle">手机号码：</span>
                        <asp:TextBox ID="txtCellPhone" CssClass="form_input_m form-control" runat="server" MaxLength="20" placeholder="正确的手机号码" />
                        <p id="ctl00_contentHolder_txtCellPhoneTip"></p>
                    </li>
                    <li id="showOpenId" runat="server" class="mb_0" style="display:none"><span class="formitemtitle">微信OpenId：</span>
                        <asp:TextBox ID="txtWxOpenId" CssClass="form_input_l form-control" runat="server" ClientIDMode="Static" />
                        <p></p>
                    </li>
                    <li class="mb_0" id="getOpenId" runat="server" style="display:none"><span class="formitemtitle">获取OpenId：</span>
                        <asp:Image runat="server" ID="OpenIdQrCodeImg" Width="150px" />
                        <br />
                        <p id="ctl00_contentHolder_OpenIdQrCodeImgTip">请使用发货人的微信进行扫码</p>
                        <p style="color: red;">需要配置微信的订单支付消息模板 <%--<a href="/admin/tools/sendmessagetemplets.aspx" class="colorBlue">去配置</a>--%></p>
                    </li>
                    <li class="mb_0"><span class="formitemtitle">电话号码：</span>
                        <asp:TextBox ID="txtTelPhone" CssClass="form_input_m form-control" runat="server" MaxLength="20" placeholder="正确的电话号码" />
                        <p id="ctl00_contentHolder_txtTelPhoneTip"></p>
                    </li>
                    <li class="mb_0" style="display:none;"><span class="formitemtitle">邮政编码：</span>
                        <asp:TextBox ID="txtZipcode" CssClass="form_input_m form-control" runat="server" MaxLength="20" placeholder="长度限制在20个字符以内" />
                        <p id="ctl00_contentHolder_txtZipcodeTip"></p>
                    </li>
                    <%--<li><span class="formitemtitle">设为默认：</span>
                        <Hi:OnOff runat="server" ID="OnOffIsDefault" ClientIDMode="Static"></Hi:OnOff>
                        <p id="ctl00_contentHolder_chkIsDefaultTip"></p>
                    </li>--%>
                    <li class="mb_0"><span class="formitemtitle">备注：</span>
                        <asp:TextBox ID="txtRemark" runat="server" TextMode="MultiLine" CssClass="form_input_l form-control" Height="120" MaxLength="20" placeholder="限制在20个字符以内"></asp:TextBox>
                        <p id="ctl00_contentHolder_txtRemarkTip"></p>
                    </li>
                </ul>
            </div>
            <div class="btntf Pa_198 clear" style="padding-top: 10px;padding-left: 254px;">
                <asp:Button ID="btnAddShipper" OnClientClick="return PageIsValid();" Text="确 定" CssClass="btn btn-primary inbnt" runat="server" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="validateHolder" runat="server">
    <script type="text/javascript" language="javascript">
        function InitValidators() {
            initValid(new InputValidator('ctl00_contentHolder_txtShipperTag', 1, 30, false, null, '发货点不能为空，长度限制在30个字符以内'));
            initValid(new InputValidator('ctl00_contentHolder_txtShipperName', 2, 20, false, '[\u4e00-\u9fa5a-zA-Z]+[\u4e00-\u9fa5_a-zA-Z0-9]*', '发货人姓名不能为空，只能是汉字或字母开头，长度在2-20个字符之间'));
            initValid(new InputValidator('ctl00_contentHolder_txtAddress', 1, 60, false, null, '发货详细地址不能为空，长度限制在60个字符以内'));

            initValid(new InputValidator('ctl00_contentHolder_txtCellPhone', 0, 20, true, '^0?(13|15|18|14|17)[0-9]{9}$', '请输入正确的手机号码'));
            initValid(new InputValidator('ctl00_contentHolder_txtTelPhone', 0, 20, true, '^([0-9]{3,4}-)?[0-9]{7,8}$', '请输入正确的电话号码'));
            initValid(new InputValidator('ctl00_contentHolder_txtZipcode', 0, 20, true, null, '邮政编码的长度限制在20个字符以内'));
            initValid(new InputValidator('ctl00_contentHolder_txtRemark', 0, 20, true, null, '备注的长度限制在20个字符以内'));
        }
        $(document).ready(function () { InitValidators(); });
        var tryTimes = 0;
        $(function () {
            (function getAdminOpenId() {
                $.ajax({
                    url: "/Admin/Admin.ashx",
                    type: 'post',
                    dataType: 'json',
                    data: {
                        action: "GetAdminOpenId"
                    },
                    timeout: 30000,
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        if (tryTimes < 20) {
                            getAdminOpenId();
                            tryTimes += 1;
                        }
                    },
                    success: function (data, textStatus) {
                        if (data.Status == "1") {
                            $("#txtWxOpenId").val(data.OpenId);
                        }
                        else {
                            if (tryTimes < 20) {
                                getAdminOpenId();
                                tryTimes += 1;
                            }
                        }
                    }
                });
            })();

        });
    </script>
</asp:Content>
