using Hidistro.Context;
using Hidistro.Core;
using Hidistro.Entities;
using Hidistro.Entities.Supplier;
using Hidistro.SaleSystem.Supplier;
using Hidistro.UI.Common.Controls;
using System;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Supplier.Balance
{
	public class RequestBalanceDraw : SupplierAdminPage
	{
		protected Literal litUserName;

		protected FormatedMoneyLabel lblBanlance;

		protected TextBox txtAmount;

		protected Literal lblminDraws;

		protected RadioButton IsDefault;

		protected RadioButton IsAlipay;

		protected TextBox txtAlipayRealName;

		protected TextBox txtAlipayCode;

		protected TextBox txtBankName;

		protected TextBox txtAccountName;

		protected TextBox txtMerchantCode;

		protected TextBox txtRemark;

		protected TextBox txtTradePassword;

		protected Button btnDrawNext;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!this.Page.IsPostBack)
			{
				this.BindData();
			}
		}

		private void BindData()
		{
			this.litUserName.Text = HiContext.Current.Manager.UserName;
			SiteSettings masterSettings = SettingsManager.GetMasterSettings();
			this.lblminDraws.Text = masterSettings.MinimumSingleShot.F2ToString("f2");
			this.IsDefault.Checked = true;
			decimal num = BalanceHelper.BalanceLeft(HiContext.Current.Manager.StoreId);
			this.lblBanlance.Money = num;
			this.BindLastInfo();
		}

		private void BindLastInfo()
		{
			SupplierBalanceDrawRequestInfo lastDrawRequest = BalanceHelper.GetLastDrawRequest(HiContext.Current.Manager.StoreId);
			if (lastDrawRequest != null)
			{
				this.txtAlipayRealName.Text = lastDrawRequest.AlipayRealName;
				this.txtAlipayCode.Text = lastDrawRequest.AlipayCode;
				this.txtAccountName.Text = lastDrawRequest.AccountName;
				this.txtBankName.Text = lastDrawRequest.BankName;
				this.txtMerchantCode.Text = lastDrawRequest.MerchantCode;
			}
		}

		protected void btnDrawNext_Click(object sender, EventArgs e)
		{
			SiteSettings masterSettings = SettingsManager.GetMasterSettings();
			decimal num = default(decimal);
			if (!decimal.TryParse(this.txtAmount.Text.Trim(), out num))
			{
				this.ShowMsg("????????????????????????,???????????????????????????", false);
			}
			else
			{
				SupplierInfo supplierById = SupplierHelper.GetSupplierById(HiContext.Current.Manager.StoreId);
				if (supplierById == null || num > supplierById.Balance)
				{
					this.ShowMsg("????????????,???????????????????????????", false);
				}
				else if (num < masterSettings.MinimumSingleShot || num < decimal.One)
				{
					this.ShowMsg("???????????????????????????????????????????????????????????????" + masterSettings.MinimumSingleShot + "???", false);
				}
				else if (string.IsNullOrEmpty(this.txtTradePassword.Text))
				{
					this.ShowMsg("?????????????????????", false);
				}
				else
				{
					switch (SupplierHelper.ValidTradePassword(HiContext.Current.Manager.StoreId, this.txtTradePassword.Text))
					{
					case -1:
						this.ShowMsg("???????????????????????????", false);
						break;
					case 0:
						this.ShowMsg("?????????????????????,??????????????????", false);
						break;
					default:
						if (!string.IsNullOrEmpty(this.txtRemark.Text) && this.txtRemark.Text.Length > 100)
						{
							this.ShowMsg("??????????????????100??????", false);
						}
						else
						{
							SupplierBalanceDrawRequestInfo supplierBalanceDrawRequestInfo = new SupplierBalanceDrawRequestInfo();
							supplierBalanceDrawRequestInfo.BankName = this.txtBankName.Text.Trim();
							supplierBalanceDrawRequestInfo.AccountName = this.txtAccountName.Text.Trim();
							supplierBalanceDrawRequestInfo.MerchantCode = this.txtMerchantCode.Text.Trim();
							supplierBalanceDrawRequestInfo.Amount = num;
							supplierBalanceDrawRequestInfo.Remark = this.txtRemark.Text.Trim();
							supplierBalanceDrawRequestInfo.SupplierId = HiContext.Current.Manager.StoreId;
							supplierBalanceDrawRequestInfo.IsAlipay = this.IsAlipay.Checked;
							supplierBalanceDrawRequestInfo.IsWeixin = false;
							supplierBalanceDrawRequestInfo.AlipayCode = this.txtAlipayCode.Text.Trim();
							supplierBalanceDrawRequestInfo.AlipayRealName = this.txtAlipayRealName.Text;
							if (this.IsAlipay.Checked)
							{
								supplierBalanceDrawRequestInfo.BankName = "0";
								supplierBalanceDrawRequestInfo.AccountName = "0";
								supplierBalanceDrawRequestInfo.MerchantCode = "0";
							}
							else
							{
								supplierBalanceDrawRequestInfo.AlipayCode = "0";
								supplierBalanceDrawRequestInfo.AlipayRealName = "0";
							}
							this.Page.Response.Redirect($"RequestConfirm.aspx?bankName={Globals.UrlEncode(Globals.HtmlEncode(supplierBalanceDrawRequestInfo.BankName))}&accountName={Globals.UrlEncode(Globals.HtmlEncode(supplierBalanceDrawRequestInfo.AccountName))}&merchantCode={Globals.UrlEncode(Globals.HtmlEncode(supplierBalanceDrawRequestInfo.MerchantCode))}&amount={supplierBalanceDrawRequestInfo.Amount}&remark={Globals.UrlEncode(Globals.HtmlEncode(supplierBalanceDrawRequestInfo.Remark))}&isalipay={supplierBalanceDrawRequestInfo.IsAlipay}&alipaycode={Globals.UrlEncode(Globals.HtmlEncode(supplierBalanceDrawRequestInfo.AlipayCode))}&alipayrealname={Globals.UrlEncode(Globals.HtmlEncode(supplierBalanceDrawRequestInfo.AlipayRealName))}");
						}
						break;
					}
				}
			}
		}
	}
}
