using Hidistro.Context;
using Hidistro.Core;
using Hidistro.Entities.Sales;
using Hidistro.SaleSystem.Member;
using Hidistro.UI.Common.Controls;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	[ParseChildren(true)]
	public class AppShippingAddresses : AppshopMemberTemplatedWebControl
	{
		private AppshopTemplatedRepeater rptvShipping;

		private HtmlAnchor aLinkToAdd;

		private SiteSettings setting = SettingsManager.GetMasterSettings();

		protected override void OnInit(EventArgs e)
		{
			if (this.SkinName == null)
			{
				this.SkinName = "Skin-Vshippingaddresses.html";
			}
			base.OnInit(e);
		}

		protected override void AttachChildControls()
		{
			this.rptvShipping = (AppshopTemplatedRepeater)this.FindControl("rptvShipping");
			this.aLinkToAdd = (HtmlAnchor)this.FindControl("aLinkToAdd");
			this.aLinkToAdd.HRef = "/AppShop/AddShippingAddress.aspx";
			if (!string.IsNullOrEmpty(this.Page.Request.QueryString["returnUrl"]))
			{
				HtmlAnchor htmlAnchor = this.aLinkToAdd;
				htmlAnchor.HRef = htmlAnchor.HRef + "?returnUrl=" + Globals.UrlEncode(this.Page.Request.QueryString["returnUrl"]);
			}
			IList<ShippingAddressInfo> shippingAddresses = MemberProcessor.GetShippingAddresses(false);
			if (shippingAddresses != null)
			{
				this.rptvShipping.ItemDataBound += this.rptvShipping_ItemDataBound;
				this.rptvShipping.DataSource = shippingAddresses;
				this.rptvShipping.DataBind();
			}
			PageTitle.AddSiteNameTitle("收货地址");
		}

		protected void rptvShipping_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				Control control = e.Item.Controls[0];
				Literal literal = control.FindControl("ltlUpgrade") as Literal;
				if (this.setting.OpenMultStore)
				{
					if (string.IsNullOrEmpty(literal.Text))
					{
						literal.Text = "<i class=\"icon-update\">需要升级</i>";
					}
					else
					{
						literal.Text = "<i class=\"icon-icon_right2\"></i>";
					}
				}
				else
				{
					literal.Text = "<i class=\"icon-icon_right2\"></i>";
				}
			}
		}
	}
}
