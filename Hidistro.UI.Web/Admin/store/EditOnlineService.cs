using Hidistro.Core;
using Hidistro.Entities.Store;
using Hidistro.SaleSystem.Store;
using Hidistro.UI.Common.Controls;
using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin.store
{
	public class EditOnlineService : AdminPage
	{
		private int ServiceId;

		protected ServiceTypeDropDownList dropServiceType;

		protected HtmlGenericControl dropServiceTypeTip;

		protected TextBox txtAccount;

		protected HtmlGenericControl txtAccountTip;

		protected TextBox txtNickName;

		protected HtmlGenericControl txtNickNameTip;

		protected TextBox txtOrderId;

		protected HtmlGenericControl txtOrderIdTip;

		protected TextBox txtImageType;

		protected YesNoRadioButtonList radioShowStatus;

		protected HtmlGenericControl radioShowStatusTip;

		protected Button btnSubmit;

		protected void Page_Load(object sender, EventArgs e)
		{
			this.btnSubmit.Click += this.btnSubmit_Click;
			if (!int.TryParse(base.Request.QueryString["ServiceId"], out this.ServiceId))
			{
				base.GotoResourceNotFound();
			}
			else if (!base.IsPostBack)
			{
				OnlineServiceInfo onlineServiceInfo = OnlineServiceHelper.GetOnlineServiceInfo(this.ServiceId);
				if (onlineServiceInfo == null)
				{
					base.GotoResourceNotFound();
				}
				else
				{
					this.txtAccount.Text = onlineServiceInfo.Account;
					TextBox textBox = this.txtImageType;
					int num = onlineServiceInfo.ImageType;
					textBox.Text = num.ToString();
					this.txtNickName.Text = onlineServiceInfo.NickName;
					this.dropServiceType.DataBind();
					this.dropServiceType.SelectedValue = onlineServiceInfo.ServiceType;
					TextBox textBox2 = this.txtOrderId;
					num = onlineServiceInfo.OrderId;
					textBox2.Text = num.ToString();
					this.radioShowStatus.SelectedValue = (onlineServiceInfo.Status == 1 && true);
				}
			}
		}

		private void btnSubmit_Click(object sender, EventArgs e)
		{
			OnlineServiceInfo onlineServiceInfo = OnlineServiceHelper.GetOnlineServiceInfo(this.ServiceId);
			if (onlineServiceInfo == null)
			{
				base.GotoResourceNotFound();
			}
			else if (!this.dropServiceType.SelectedValue.HasValue)
			{
				this.ShowMsg("?????????????????????", false);
			}
			else
			{
				string text = Globals.StripAllTags(this.txtAccount.Text.Trim());
				if (string.IsNullOrEmpty(text) || text.Length > 50)
				{
					this.ShowMsg("?????????????????????????????????????????????50???????????????", false);
				}
				else
				{
					string text2 = Globals.StripAllTags(this.txtNickName.Text.Trim());
					if (string.IsNullOrEmpty(text2) || text2.Length > 50)
					{
						this.ShowMsg("?????????????????????????????????????????????50???????????????", false);
					}
					else
					{
						int imageType = 1;
						int.TryParse(this.txtImageType.Text, out imageType);
						int orderId = 0;
						int.TryParse(this.txtOrderId.Text, out orderId);
						onlineServiceInfo.Id = this.ServiceId;
						onlineServiceInfo.Account = text;
						onlineServiceInfo.ImageType = imageType;
						onlineServiceInfo.NickName = text2;
						onlineServiceInfo.OrderId = orderId;
						onlineServiceInfo.ServiceType = this.dropServiceType.SelectedValue.Value;
						onlineServiceInfo.Status = (this.radioShowStatus.SelectedValue ? 1 : 0);
						if (OnlineServiceHelper.Update(onlineServiceInfo))
						{
							this.ShowMsg("?????????????????????????????????", true);
						}
						else
						{
							this.ShowMsg("????????????", false);
						}
					}
				}
			}
		}
	}
}
