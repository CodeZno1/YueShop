using Hidistro.Context;
using Hidistro.Core;
using Hidistro.Entities.Orders;
using Hidistro.SaleSystem.Member;
using Hidistro.SaleSystem.Sales;
using Hidistro.UI.Common.Controls;
using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin.sales
{
	public class RealNameCertification : AdminPage
	{
		private string orderId;

		protected HtmlInputHidden hidCertificationModel;

		protected Label lblShipTo;

		protected Label lblRealName;

		protected Label lblIDNumber;

		protected HtmlImage IDImageJust;

		protected HtmlImage IDImageAnti;

		protected TextBox txtIDRemark;

		protected HtmlGenericControl txtIDRemarkTip;

		protected HtmlImage IDImageJust_zoom;

		protected HtmlImage IDImageAnti_zoom;

		protected Button btnPass;

		protected Button btnRefuse;

		protected void Page_Load(object sender, EventArgs e)
		{
			this.orderId = this.Page.Request.QueryString["OrderId"];
			if (!this.Page.IsPostBack)
			{
				OrderInfo orderInfo = OrderHelper.GetOrderInfo(this.orderId);
				if (orderInfo != null)
				{
					this.BindRealName(orderInfo);
				}
			}
			this.btnPass.Click += this.btnPass_Click;
			this.btnRefuse.Click += this.btnRefuse_Click;
		}

		private void BindRealName(OrderInfo order)
		{
			this.hidCertificationModel.Value = HiContext.Current.SiteSettings.CertificationModel.ToString();
			this.lblShipTo.Text = order.ShipTo;
			this.lblRealName.Text = order.ShipTo;
			this.lblIDNumber.Text = HiCryptographer.Decrypt(order.IDNumber);
			this.txtIDRemark.Text = order.IDRemark;
			if (!string.IsNullOrWhiteSpace(order.IDImage1))
			{
				this.IDImageJust.Src = order.IDImage1;
				this.IDImageJust.Visible = true;
			}
			else
			{
				this.IDImageJust.Visible = false;
			}
			if (!string.IsNullOrWhiteSpace(order.IDImage2))
			{
				this.IDImageAnti.Src = order.IDImage2;
				this.IDImageAnti.Visible = true;
			}
			else
			{
				this.IDImageAnti.Visible = false;
			}
			if (order.IDStatus == 1)
			{
				this.btnPass.Visible = false;
			}
		}

		private void btnPass_Click(object sender, EventArgs e)
		{
			OrderInfo orderInfo = OrderHelper.GetOrderInfo(this.orderId);
			if (orderInfo != null)
			{
				orderInfo.IDStatus = 1;
				orderInfo.IDRemark = this.txtIDRemark.Text.Trim();
				if (TradeHelper.UpdateOrderInfo(orderInfo))
				{
					this.ShowMsgCloseWindow("????????????", true);
				}
				else
				{
					this.ShowMsgCloseWindow("??????????????????", false);
				}
			}
			this.ShowMsgCloseWindow("??????????????????", false);
		}

		private void btnRefuse_Click(object sender, EventArgs e)
		{
			this.VerifyInfo();
			OrderInfo orderInfo = OrderHelper.GetOrderInfo(this.orderId);
			if (orderInfo != null)
			{
				orderInfo.IDStatus = 0;
				orderInfo.IDRemark = this.txtIDRemark.Text.Trim();
				if (TradeHelper.UpdateOrderInfo(orderInfo))
				{
					this.ShowMsgCloseWindow("????????????", true);
				}
				else
				{
					this.ShowMsgCloseWindow("??????????????????", false);
				}
			}
			this.ShowMsgCloseWindow("??????????????????", false);
		}

		private void VerifyInfo()
		{
			if (this.txtIDRemark.Text != "" && this.txtIDRemark.Text.Length > 100)
			{
				this.ShowMsg("?????????????????????1-100?????????", false);
			}
		}
	}
}
