using Hidistro.Context;
using Hidistro.Core;
using Hidistro.Entities.Orders;
using Hidistro.Entities.Store;
using Hidistro.SaleSystem.Sales;
using Hidistro.SaleSystem.Store;
using Hidistro.UI.Common.Controls;
using Hishop.Components.Validation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin
{
	[PrivilegeCheck(Privilege.EditOrders)]
	public class EditOrder : AdminPage
	{
		private string orderId;

		private OrderInfo order;

		protected Repeater grdProducts;

		protected Repeater grdOrderGift;

		protected FormatedMoneyLabel lblAllPrice;

		protected HtmlTableRow trDeopt;

		protected FormatedMoneyLabel FormatedMoneyDeopt;

		protected HtmlTableRow trFinal;

		protected FormatedMoneyLabel FormatedMoneyFinal;

		protected HtmlGenericControl tdDiscount;

		protected FormatedMoneyLabel fullDiscountAmount;

		protected HyperLink lkbtnFullDiscount;

		protected TextBox txtAdjustedFreight;

		protected HtmlTableRow trFullFree;

		protected HyperLink lkbtnFullFree;

		protected HtmlTableRow trCouponAmount;

		protected FormatedMoneyLabel couponAmount;

		protected TextBox txtAdjustedDiscount;

		protected Literal litTotal;

		protected Literal litTax;

		protected Literal litInvoiceTitle;

		protected HtmlGenericControl tdPoint;

		protected Literal litPoint;

		protected HyperLink hlkSentTimesPointPromotion;

		protected Literal litBalance;

		protected Button btnUpdateOrderAmount;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(this.Page.Request.QueryString["OrderId"]))
			{
				base.GotoResourceNotFound();
			}
			else
			{
				this.orderId = this.Page.Request.QueryString["OrderId"];
				this.btnUpdateOrderAmount.Click += this.btnUpdateOrderAmount_Click;
				this.grdOrderGift.ItemCommand += this.grdOrderGift_ItemCommand;
				this.grdProducts.ItemCommand += this.grdProducts_ItemCommand;
				this.order = OrderHelper.GetOrderInfo(this.orderId);
				if (!this.Page.IsPostBack)
				{
					if (this.order == null)
					{
						base.GotoResourceNotFound();
					}
					else
					{
						this.BindProductList(this.order);
						this.BindOtherAmount(this.order);
						this.BindTatolAmount(this.order);
						if (this.order.LineItems.Count == 0)
						{
							this.grdProducts.Visible = false;
						}
					}
				}
			}
		}

		public int GetPoint(OrderInfo order)
		{
			int num = 0;
			SiteSettings masterSettings = SettingsManager.GetMasterSettings();
			if ((order.GetTotal(false) - order.AdjustedFreight) * order.TimesPoint / masterSettings.PointsRate > 2147483647m)
			{
				num = 2147483647;
			}
			else if (masterSettings.PointsRate != decimal.Zero)
			{
				num = (int)((order.GetTotal(false) - order.AdjustedFreight) * order.TimesPoint / masterSettings.PointsRate);
			}
			if (num < 0)
			{
				num = 0;
			}
			return num;
		}

		private void btnUpdateOrderAmount_Click(object sender, EventArgs e)
		{
			decimal num = default(decimal);
			decimal num2 = default(decimal);
			if (!this.order.CheckAction(OrderActions.SELLER_MODIFY_TRADE))
			{
				this.ShowMsg("????????????????????????????????????????????????????????????", false);
			}
			else if (this.ValidateValues(out num, out num2))
			{
				string empty = string.Empty;
				this.order.AdjustedFreight = num;
				this.order.AdjustedDiscount = num2;
				if (this.order.PreSaleId > 0 && this.order.FinalPayment + num + num2 < decimal.Zero)
				{
					this.ShowMsg("???????????????????????????????????????", false);
				}
				else
				{
					decimal d = this.order.GetPayTotal() - this.order.BalanceAmount;
					this.order.Points = this.GetPoint(this.order);
					if (this.order.PreSaleId > 0)
					{
						this.order.FinalPayment = this.order.GetFinalPayment();
					}
					ValidationResults validationResults = Validation.Validate(this.order, "ValOrder");
					if (!validationResults.IsValid)
					{
						using (IEnumerator<ValidationResult> enumerator = ((IEnumerable<ValidationResult>)validationResults).GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								ValidationResult current = enumerator.Current;
								empty += Formatter.FormatErrorMessage(current.Message);
								this.ShowMsg(empty, false);
								return;
							}
						}
					}
					if (d > decimal.Zero)
					{
						if (OrderHelper.UpdateOrderAmount(this.order))
						{
							int num3 = this.order.PayRandCode.ToInt(0);
							num3 = ((num3 >= 100) ? (num3 + 1) : 100);
							this.order.PayRandCode = num3.ToString();
							OrderHelper.UpdateOrderPaymentType(this.order);
							this.BindTatolAmount(this.order);
							this.ShowMsg("??????????????????????????????", true);
						}
						else
						{
							this.ShowMsg("????????????????????????", false);
						}
					}
					else
					{
						this.ShowMsg("????????????????????????????????????0?????????,???????????????????????????", false);
					}
				}
			}
		}

		private void BindProductList(OrderInfo order)
		{
			this.grdProducts.DataSource = order.LineItems.Values;
			this.grdProducts.DataBind();
			if (order.Gifts.Count > 0)
			{
				if (order.UserAwardRecordsId > 0)
				{
					foreach (OrderGiftInfo gift in order.Gifts)
					{
						gift.PromoteType = -1;
					}
				}
				this.grdOrderGift.DataSource = order.Gifts;
				this.grdOrderGift.DataBind();
			}
			else
			{
				this.grdOrderGift.Visible = false;
			}
		}

		private void grdOrderGift_ItemCommand(object sender, RepeaterCommandEventArgs e)
		{
			if (e.CommandName == "Delete")
			{
				if (!this.order.CheckAction(OrderActions.SELLER_MODIFY_TRADE))
				{
					this.ShowMsg("????????????????????????????????????????????????????????????", false);
				}
				else
				{
					int giftId = e.CommandArgument.ToInt(0);
					if (OrderHelper.DeleteOrderGift(this.order, giftId))
					{
						this.order = OrderHelper.GetOrderInfo(this.orderId);
						this.BindProductList(this.order);
						this.BindTatolAmount(this.order);
						this.ShowMsg("???????????????????????????", true);
					}
					else
					{
						this.ShowMsg("??????????????????", false);
					}
				}
			}
		}

		private void grdProducts_ItemCommand(object sender, RepeaterCommandEventArgs e)
		{
			if (e.CommandName == "Delete")
			{
				if (!this.order.CheckAction(OrderActions.SELLER_MODIFY_TRADE))
				{
					this.ShowMsg("????????????????????????????????????????????????????????????", false);
					return;
				}
				if (this.order.LineItems.Values.Count <= 1)
				{
					this.ShowMsg("??????????????????????????????????????????", false);
					return;
				}
				string sku = e.CommandArgument.ToString();
				if (OrderHelper.DeleteLineItem(sku, this.order))
				{
					this.order = OrderHelper.GetOrderInfo(this.orderId);
					this.BindProductList(this.order);
					this.BindTatolAmount(this.order);
					this.ShowMsg("???????????????????????????", true);
				}
				else
				{
					this.ShowMsg("??????????????????", false);
				}
			}
			if (e.CommandName == "setQuantity")
			{
				OrderInfo orderInfo = this.order;
				if (!this.order.CheckAction(OrderActions.SELLER_MODIFY_TRADE))
				{
					this.ShowMsg("????????????????????????????????????????????????????????????", false);
				}
				else
				{
					string text = e.CommandArgument.ToString();
					TextBox textBox = e.Item.FindControl("txtQuantity") as TextBox;
					int quantity = orderInfo.LineItems[text].Quantity;
					int num = default(int);
					if (!int.TryParse(textBox.Text.Trim(), out num))
					{
						this.ShowMsg("????????????????????????", false);
					}
					else if (num > OrderHelper.GetSkuStock(text))
					{
						this.ShowMsg("?????????????????????", false);
					}
					else if (num <= 0)
					{
						this.ShowMsg("???????????????????????????0", false);
					}
					else
					{
						orderInfo.LineItems[text].Quantity = num;
						orderInfo.LineItems[text].ShipmentQuantity = num;
						orderInfo.LineItems[text].ItemAdjustedPrice = orderInfo.LineItems[text].ItemListPrice;
						if (this.order.PreSaleId > 0)
						{
							if (orderInfo.AdjustedDiscount < decimal.Zero && orderInfo.FinalPayment == decimal.Zero && num < quantity)
							{
								this.ShowMsg("????????????????????????????????????,????????????????????????", false);
								return;
							}
						}
						else if (orderInfo.GetTotal(false) <= decimal.Zero)
						{
							this.ShowMsg("???????????????????????????0", false);
							return;
						}
						if (quantity != num)
						{
							this.order.Points = this.GetPoint(orderInfo);
							if (OrderHelper.UpdateLineItem(text, this.order, num, quantity))
							{
								this.BindProductList(this.order);
								this.BindTatolAmount(this.order);
								this.ShowMsg("??????????????????????????????", true);
							}
							else
							{
								this.ShowMsg("??????????????????????????????", false);
							}
						}
					}
				}
			}
		}

		private void grdProducts_RowCommand(object sender, GridViewCommandEventArgs e)
		{
		}

		private void BindTatolAmount(OrderInfo order)
		{
			decimal amount = order.GetAmount(false);
			this.lblAllPrice.Money = amount;
			if (order.Points > 0)
			{
				this.litPoint.Text = order.Points.ToString(CultureInfo.InvariantCulture);
			}
			else
			{
				this.tdPoint.InnerHtml = "";
				this.litPoint.Visible = false;
				this.hlkSentTimesPointPromotion.Visible = false;
			}
			this.litBalance.Text = Globals.FormatMoney(order.BalanceAmount);
			this.litTotal.Text = Globals.FormatMoney(order.GetPayTotal() - order.BalanceAmount);
			if (order.PreSaleId > 0)
			{
				this.FormatedMoneyDeopt.Text = order.Deposit.F2ToString("f2");
				this.FormatedMoneyFinal.Text = order.FinalPayment.F2ToString("f2");
				this.trFinal.Visible = true;
				this.trDeopt.Visible = true;
			}
		}

		private void BindOtherAmount(OrderInfo order)
		{
			if (order.IsReduced)
			{
				this.fullDiscountAmount.Money = order.ReducedPromotionAmount;
				this.lkbtnFullDiscount.NavigateUrl = base.GetRouteUrl("FavourableDetails", new
				{
					activityId = order.ReducedPromotionId
				});
				this.lkbtnFullDiscount.Text = order.ReducedPromotionName;
			}
			else
			{
				this.tdDiscount.InnerHtml = "";
				this.fullDiscountAmount.Visible = false;
			}
			if (order.IsFreightFree)
			{
				this.lkbtnFullFree.NavigateUrl = base.GetRouteUrl("FavourableDetails", new
				{
					activityId = order.FreightFreePromotionId
				});
				this.lkbtnFullFree.Text = order.FreightFreePromotionName;
			}
			else
			{
				this.trFullFree.Visible = false;
			}
			if (order.IsSendTimesPoint)
			{
				this.hlkSentTimesPointPromotion.Text = order.SentTimesPointPromotionName;
				this.hlkSentTimesPointPromotion.NavigateUrl = base.GetRouteUrl("FavourableDetails", new
				{
					activityId = order.SentTimesPointPromotionId
				});
			}
			TextBox textBox = this.txtAdjustedFreight;
			decimal num = order.AdjustedFreight;
			textBox.Text = num.ToString("F", CultureInfo.InvariantCulture);
			TextBox textBox2 = this.txtAdjustedDiscount;
			num = order.AdjustedDiscount;
			textBox2.Text = num.ToString("F", CultureInfo.InvariantCulture);
			if (!string.IsNullOrEmpty(order.CouponName))
			{
				this.couponAmount.Text = "[" + order.CouponName + "]-" + Globals.FormatMoney(order.CouponValue);
			}
			else
			{
				this.couponAmount.Text = "-" + Globals.FormatMoney(order.CouponValue);
			}
			if (order.CouponValue <= decimal.Zero)
			{
				this.trCouponAmount.Visible = false;
			}
			if (order.Tax > decimal.Zero && order.SupplierId == 0)
			{
				this.litTax.Text = "<tr class=\"bg\"><td align=\"right\">??????(???)???</td><td colspan=\"2\"><span class='Name'>" + Globals.FormatMoney(order.Tax);
				Literal literal = this.litTax;
				literal.Text += "</span></td></tr>";
			}
			if (order.InvoiceTitle.Length > 0 && order.SupplierId == 0)
			{
				this.litInvoiceTitle.Text = "<tr class=\"bg\"><td align=\"right\">???????????????</td><td colspan=\"2\"><span class='Name'>" + order.InvoiceTitle;
				Literal literal2 = this.litInvoiceTitle;
				literal2.Text += "</span></td></tr>";
				if (!string.IsNullOrWhiteSpace(order.InvoiceTaxpayerNumber))
				{
					Literal literal3 = this.litInvoiceTitle;
					literal3.Text = literal3.Text + "<tr class=\"bg\"><td align=\"right\">?????????????????????</td><td colspan=\"2\"><span class='Name'>" + order.InvoiceTaxpayerNumber;
					Literal literal4 = this.litInvoiceTitle;
					literal4.Text += "</span></td></tr>";
				}
			}
		}

		private bool ValidateValues(out decimal adjustedFreight, out decimal discountAmout)
		{
			string text = string.Empty;
			if (!decimal.TryParse(this.txtAdjustedFreight.Text.Trim(), out adjustedFreight))
			{
				text += Formatter.FormatErrorMessage("???????????????0-1000?????????");
			}
			int num = 0;
			if (this.txtAdjustedDiscount.Text.Trim().IndexOf(".") > 0)
			{
				num = this.txtAdjustedDiscount.Text.Trim().Substring(this.txtAdjustedDiscount.Text.Trim().IndexOf(".") + 1).Length;
			}
			if (!decimal.TryParse(this.txtAdjustedDiscount.Text.Trim(), out discountAmout) || num > 2)
			{
				text += Formatter.FormatErrorMessage("????????????????????????,?????????????????????????????????????????????2?????????");
			}
			if (!string.IsNullOrEmpty(text))
			{
				this.ShowMsg(text, false);
				return false;
			}
			return true;
		}
	}
}
