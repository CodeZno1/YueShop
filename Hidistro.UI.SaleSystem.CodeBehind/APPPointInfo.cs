using Hidistro.Context;
using Hidistro.Core;
using Hidistro.Entities.Promotions;
using Hidistro.Entities.Sales;
using Hidistro.SaleSystem.Catalog;
using Hidistro.SaleSystem.Shopping;
using Hidistro.UI.Common.Controls;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	[ParseChildren(true)]
	public class APPPointInfo : AppshopMemberTemplatedWebControl
	{
		private int giftId;

		private bool isExemptionPostage;

		private HiImage imgGift;

		private Literal litName;

		private Literal litMarketPrice;

		private Literal litNeedPoints;

		private Literal litHasPoints;

		private Literal litMeta_Description;

		private ImageLinkButton btnClearCart;

		private HtmlInputHidden hidGiftId;

		private HiddenField hidHasPoints;

		private HiddenField hidNeedPoints;

		protected override void OnInit(EventArgs e)
		{
			if (this.SkinName == null)
			{
				this.SkinName = "skin-PointInfo.html";
			}
			base.OnInit(e);
		}

		protected override void AttachChildControls()
		{
			if (!int.TryParse(this.Page.Request.QueryString["GiftId"], out this.giftId))
			{
				base.GotoResourceNotFound("");
			}
			this.imgGift = (HiImage)this.FindControl("imgGift");
			this.litName = (Literal)this.FindControl("litName");
			this.litMarketPrice = (Literal)this.FindControl("litMarketPrice");
			this.litNeedPoints = (Literal)this.FindControl("litNeedPoints");
			this.litHasPoints = (Literal)this.FindControl("litHasPoints");
			this.hidHasPoints = (HiddenField)this.FindControl("hidHasPoints");
			this.hidNeedPoints = (HiddenField)this.FindControl("hidNeedPoints");
			this.litMeta_Description = (Literal)this.FindControl("litMeta_Description");
			this.hidGiftId = (HtmlInputHidden)this.FindControl("hidGiftId");
			this.btnClearCart = (ImageLinkButton)this.FindControl("btnClearCart");
			this.btnClearCart.Click += this.btnClearCart_Click;
			GiftInfo giftDetails = ProductBrowser.GetGiftDetails(this.giftId);
			if (giftDetails == null)
			{
				this.Page.Response.Redirect("ResourceNotFound?errorMsg=" + Globals.UrlEncode("??????????????????????????????????????????????????????????????????"));
			}
			else
			{
				this.isExemptionPostage = giftDetails.IsExemptionPostage;
				this.imgGift.ImageUrl = giftDetails.ImageUrl;
				this.litName.Text = giftDetails.Name;
				this.litMarketPrice.Text = Math.Round((!giftDetails.MarketPrice.HasValue) ? decimal.Zero : giftDetails.MarketPrice.Value, 2).ToString();
				Literal literal = this.litNeedPoints;
				int num = giftDetails.NeedPoint;
				literal.Text = num.ToString();
				this.litMeta_Description.Text = giftDetails.LongDescription;
				this.hidGiftId.Value = this.giftId.ToString();
				if (HiContext.Current.UserId != 0 && giftDetails.NeedPoint > 0)
				{
					if (HiContext.Current.User.Points < giftDetails.NeedPoint)
					{
						this.btnClearCart.Enabled = false;
						this.btnClearCart.Style.Add("disable", "true");
						this.btnClearCart.Text = "????????????";
					}
					else
					{
						this.btnClearCart.Enabled = true;
						this.btnClearCart.Text = "????????????";
					}
					Literal literal2 = this.litHasPoints;
					num = HiContext.Current.User.Points;
					literal2.Text = num.ToString();
					HiddenField hiddenField = this.hidHasPoints;
					num = HiContext.Current.User.Points;
					hiddenField.Value = num.ToString();
					HiddenField hiddenField2 = this.hidNeedPoints;
					num = giftDetails.NeedPoint;
					hiddenField2.Value = num.ToString();
				}
				else if (giftDetails.NeedPoint <= 0)
				{
					this.btnClearCart.Enabled = false;
					this.btnClearCart.Text = "?????????????????????";
				}
				else
				{
					this.btnClearCart.Text = "?????????????????????";
				}
				if (!giftDetails.IsPointExchange)
				{
					this.btnClearCart.Enabled = false;
					this.btnClearCart.Text = "????????????????????????????????????";
				}
				PageTitle.AddSiteNameTitle("????????????");
			}
		}

		protected void btnClearCart_Click(object sender, EventArgs e)
		{
			if (this.btnClearCart.Text == "?????????????????????" || HiContext.Current.User == null)
			{
				this.Page.Response.Redirect("Login.aspx?ReturnUrl=" + HttpContext.Current.Request.RawUrl, true);
			}
			else if (int.Parse(this.litNeedPoints.Text) <= int.Parse(this.litHasPoints.Text))
			{
				ShoppingCartInfo shoppingCart = ShoppingCartProcessor.GetShoppingCart(null, false, false, -1);
				if (shoppingCart != null && shoppingCart.LineGifts != null && shoppingCart.LineGifts.Count > 0)
				{
					foreach (ShoppingCartGiftInfo lineGift in shoppingCart.LineGifts)
					{
						if (lineGift.GiftId == this.giftId)
						{
							this.Page.ClientScript.RegisterStartupScript(base.GetType(), "myscript", "<script>$(function(){alert_h(\"??????????????????????????????????????????????????????????????????????????????????????????\")});</script>");
							return;
						}
					}
				}
				if (ShoppingCartProcessor.AddGiftItem(this.giftId, 1, PromoteType.NotSet, this.isExemptionPostage))
				{
					this.Page.ClientScript.RegisterStartupScript(base.GetType(), "myscript", "<script> var type = GetAgentType(); if (type == 2) window.HiCmd.webGoHome(2); else if (type == 1) loadIframeURL('hishop://webGoHome/null/2');</script>");
				}
				else
				{
					this.Page.ClientScript.RegisterStartupScript(base.GetType(), "myscript", "<script>$(function(){alert_h(\"???????????????????????????\")});;</script>");
				}
			}
			else
			{
				this.Page.ClientScript.RegisterStartupScript(base.GetType(), "myscript", "<script>$(function(){alert_h(\"???????????????\")});</script>");
			}
		}
	}
}
