using Hidistro.Core;
using Hidistro.Entities;
using Hidistro.Entities.Orders;
using Hidistro.Entities.Promotions;
using Hidistro.Entities.VShop;
using Hidistro.SaleSystem.Catalog;
using Hidistro.SaleSystem.Member;
using Hidistro.SaleSystem.Store;
using Hidistro.UI.Common.Controls;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	[ParseChildren(true)]
	public class AppApplyRefund : AppshopMemberTemplatedWebControl
	{
		private string OrderId = "";

		private string SkuId = "";

		private IButton btnRefund;

		private HtmlInputHidden txtOrderId;

		private HtmlInputHidden txtSkuId;

		private HtmlInputHidden hidErrorMsg;

		private TextBox txtRemark;

		private WapRefundTypeDropDownList dropRefundType;

		private WapAfterSalesReasonDropDownList DropRefundReason;

		private Literal litOrderIds;

		private Literal litRefundMoney;

		private OrderInfo order = null;

		private LineItemInfo RefundItem = null;

		private HtmlGenericControl groupbuyPanel;

		protected override void OnInit(EventArgs e)
		{
			if (this.SkinName == null)
			{
				this.SkinName = "/skin-ApplyRefund.html";
			}
			this.OrderId = ((HttpContext.Current.Request.QueryString["OrderId"] == null) ? "" : Globals.StripAllTags(HttpContext.Current.Request.QueryString["OrderId"]));
			this.SkuId = ((HttpContext.Current.Request.QueryString["SkuId"] == null) ? "" : Globals.StripAllTags(HttpContext.Current.Request.QueryString["SkuId"]));
			base.OnInit(e);
		}

		protected override void AttachChildControls()
		{
			this.hidErrorMsg = (HtmlInputHidden)this.FindControl("hidErrorMsg");
			this.txtOrderId = (HtmlInputHidden)this.FindControl("txtOrderId");
			this.litOrderIds = (Literal)this.FindControl("litOrderIds");
			this.txtSkuId = (HtmlInputHidden)this.FindControl("txtSkuId");
			this.txtRemark = (TextBox)this.FindControl("txtRemark");
			this.litRefundMoney = (Literal)this.FindControl("litRefundMoney");
			this.dropRefundType = (WapRefundTypeDropDownList)this.FindControl("dropRefundType");
			this.DropRefundReason = (WapAfterSalesReasonDropDownList)this.FindControl("RefundReasonDropDownList");
			this.btnRefund = ButtonManager.Create(this.FindControl("btnRefund"));
			this.groupbuyPanel = (HtmlGenericControl)this.FindControl("groupbuyPanel");
			this.txtOrderId.Value = this.OrderId;
			this.litOrderIds.Text = this.OrderId;
			this.txtSkuId.Value = this.SkuId;
			this.order = TradeHelper.GetOrderInfo(this.OrderId);
			if (this.order == null)
			{
				this.ShowError("?????????????????????!");
			}
			else
			{
				if (this.order.FightGroupId > 0)
				{
					FightGroupInfo fightGroup = VShopHelper.GetFightGroup(this.order.FightGroupId);
					if (fightGroup.Status != FightGroupStatus.FightGroupSuccess)
					{
						this.ShowError("???????????????????????????????????????????????????!");
						return;
					}
				}
				if (TradeHelper.IsOnlyOneSku(this.order))
				{
					this.SkuId = "";
				}
				if (!string.IsNullOrEmpty(this.SkuId) && !this.order.LineItems.ContainsKey(this.SkuId))
				{
					this.ShowError("?????????????????????!");
				}
				else if (!TradeHelper.CanRefund(this.order, this.SkuId))
				{
					this.ShowError("?????????????????????????????????????????????????????????/???????????????");
				}
				else
				{
					if (this.order.LineItems.ContainsKey(this.SkuId))
					{
						this.RefundItem = this.order.LineItems[this.SkuId];
					}
					GroupBuyInfo groupBuyInfo = null;
					if (this.order.GroupBuyId > 0)
					{
						groupBuyInfo = ProductBrowser.GetGroupBuy(this.order.GroupBuyId);
						if (groupBuyInfo != null)
						{
							if (this.groupbuyPanel != null && groupBuyInfo.Status != GroupBuyStatus.Failed)
							{
								this.groupbuyPanel.Visible = true;
							}
							if (groupBuyInfo.NeedPrice > this.order.GetTotal(false) && groupBuyInfo.Status != GroupBuyStatus.Failed)
							{
								this.ShowError("????????????????????????????????????,????????????????????????!");
								return;
							}
						}
					}
					decimal canRefundAmount = this.order.GetCanRefundAmount(this.SkuId, groupBuyInfo, 0);
					if (canRefundAmount < decimal.Zero)
					{
						this.ShowError("??????????????????????????????????????????????????????!");
					}
					else
					{
						this.litRefundMoney.Text = canRefundAmount.F2ToString("f2");
						PageTitle.AddSiteNameTitle("????????????");
						if (!this.Page.IsPostBack)
						{
							this.DropRefundReason.IsRefund = true;
							this.dropRefundType.preSaleId = this.order.PreSaleId;
							string enumDescription = EnumDescription.GetEnumDescription((Enum)(object)EnumPaymentType.AdvancePay, 1);
							this.dropRefundType.BalanceAmount = this.order.BalanceAmount;
							this.dropRefundType.OrderGateWay = ((this.order.PreSaleId > 0 && this.order.DepositGatewayOrderId.ToNullString().ToLower() == enumDescription) ? enumDescription : this.order.Gateway);
						}
					}
				}
			}
		}

		public void ShowError(string errorMsg)
		{
			this.hidErrorMsg.Value = errorMsg;
		}
	}
}
