using Hidistro.Context;
using Hidistro.Core;
using Hidistro.Entities.Store;
using Hidistro.SaleSystem.Store;
using Hidistro.UI.Common.Controls;
using Hidistro.UI.Web.Admin.Ascx;
using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin.Applet
{
	[PrivilegeCheck(Privilege.AppletPayConfig)]
	public class O2OXCXPaySetting : AdminPage
	{
		protected OnOff ooEnableHtmRewrite;

		protected AccountNumbersTextBox txtAppId;

		protected AccountNumbersTextBox txtAppSecret;

		protected AccountNumbersTextBox txtPartnerID;

		protected AccountNumbersTextBox txtPartnerKey;

		protected FileUpload file_UploadCert;

		protected HtmlGenericControl liCertFileName;

		protected TrimTextBox txtCerFileName;

		protected AccountNumbersTextBox txtCertPassword;

		protected Button btnOK;

		protected void Page_Load(object sender, EventArgs e)
		{
			this.ooEnableHtmRewrite.Parameter.Add("onSwitchChange", "fuCheckEnableWXPay");
			this.btnOK.Click += this.btnOK_Click;
			if (!base.IsPostBack)
			{
				SiteSettings masterSettings = SettingsManager.GetMasterSettings();
				this.txtAppId.Text = masterSettings.O2OAppletAppId;
				string o2OAppletKey = masterSettings.O2OAppletKey;
				if (o2OAppletKey.Length > 6)
				{
					this.txtPartnerKey.Text = o2OAppletKey.Substring(0, 3) + Globals.GetHiddenStr(o2OAppletKey.Length - 6, "*") + o2OAppletKey.Substring(o2OAppletKey.Length - 3);
				}
				this.txtPartnerID.Text = masterSettings.O2OAppletMchId;
				this.txtAppSecret.Text = masterSettings.O2OAppletAppSecrect;
				this.ooEnableHtmRewrite.SelectedValue = masterSettings.OpenO2OAppletWxPay;
				if (string.IsNullOrEmpty(masterSettings.O2OAppletPayCert))
				{
					this.liCertFileName.Visible = false;
				}
				else
				{
					this.liCertFileName.Visible = true;
					this.txtCerFileName.Text = masterSettings.O2OAppletPayCert;
				}
				this.txtCertPassword.Text = masterSettings.O2OAppletPayCertPassword;
			}
		}

		protected void btnOK_Click(object sender, EventArgs e)
		{
			SiteSettings masterSettings = SettingsManager.GetMasterSettings();
			if (masterSettings.IsDemoSite)
			{
				this.ShowMsg("????????????????????????????????????", false);
			}
			else
			{
				if (this.ooEnableHtmRewrite.SelectedValue)
				{
					if (string.IsNullOrEmpty(this.txtAppId.Text))
					{
						this.ShowMsg("AppId???????????????", false);
						return;
					}
					if (string.IsNullOrEmpty(this.txtAppSecret.Text))
					{
						this.ShowMsg("AppSecret???????????????", false);
						return;
					}
					if (string.IsNullOrEmpty(this.txtPartnerID.Text))
					{
						this.ShowMsg("mch_id???????????????", false);
						return;
					}
					if (string.IsNullOrEmpty(this.txtPartnerKey.Text))
					{
						this.ShowMsg("Key???????????????", false);
						return;
					}
				}
				string o2OAppletPayCert = this.txtCerFileName.Text;
				if (this.file_UploadCert.HasFile)
				{
					if (!Globals.ValidateCertFile(this.file_UploadCert.PostedFile.FileName))
					{
						this.ShowMsg("?????????????????????", false);
						return;
					}
					string newFileName = this.GetNewFileName(this.file_UploadCert.PostedFile.FileName);
					this.file_UploadCert.PostedFile.SaveAs(base.Server.MapPath("~/pay/cert/") + newFileName);
					o2OAppletPayCert = "/pay/cert/" + newFileName;
				}
				string o2OAppletAppId = masterSettings.O2OAppletAppId;
				masterSettings.O2OAppletAppId = this.txtAppId.Text;
				masterSettings.O2OAppletAppSecrect = this.txtAppSecret.Text;
				masterSettings.O2OAppletMchId = this.txtPartnerID.Text;
				if (this.txtPartnerKey.Text.Replace("*", "").Length != 6)
				{
					masterSettings.O2OAppletKey = this.txtPartnerKey.Text;
				}
				masterSettings.OpenO2OAppletWxPay = this.ooEnableHtmRewrite.SelectedValue;
				masterSettings.O2OAppletPayCert = o2OAppletPayCert;
				masterSettings.O2OAppletPayCertPassword = masterSettings.O2OAppletMchId;
				SettingsManager.Save(masterSettings);
				this.ShowMsg("????????????", true, "XCXPaySetting");
			}
		}

		public string GetNewFileName(string filename)
		{
			string result = filename;
			int num = filename.LastIndexOf(".");
			if (num > -1 && num < filename.Length - 1)
			{
				string str = filename.Substring(num + 1);
				string str2 = filename.Substring(0, num);
				result = str2 + DateTime.Now.ToString("MMddHHmmssfff") + "." + str;
			}
			return result;
		}
	}
}
