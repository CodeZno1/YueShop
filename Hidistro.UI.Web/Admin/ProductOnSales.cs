using Hidistro.Context;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Commodities;
using Hidistro.Entities.Depot;
using Hidistro.Entities.Store;
using Hidistro.SaleSystem.Commodities;
using Hidistro.SaleSystem.Depot;
using Hidistro.SaleSystem.Store;
using Hidistro.UI.Common.Controls;
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin
{
	[PrivilegeCheck(Privilege.Products)]
	public class ProductOnSales : AdminPage
	{
		private SiteSettings setting;

		protected HtmlInputHidden hidFilter;

		protected HtmlInputText txtSearchText;

		protected HtmlInputText txtSKU;

		protected ProductCategoriesDropDownList dropCategories;

		protected DropDownList ddlProductType;

		protected BrandCategoriesDropDownList dropBrandList;

		protected ProductTagsDropDownList dropTagList;

		protected ShippingTemplatesDropDownList dropShippingTemplateId;

		protected HtmlInputText so_more_input;

		protected HtmlGenericControl so_more_none;

		protected ProductTypeDownList dropType;

		protected HtmlInputCheckBox chkIsWarningStock;

		protected Button btnImportToStore;

		protected PageSizeDropDownList PageSizeDropDownList;

		protected HtmlGenericControl spanstorestock;

		protected HtmlGenericControl spandown;

		protected ProductTagsLiteral litralProductTag;

		protected TextBox txtSubMemberDeduct;

		protected TextBox txtSecondLevelDeduct;

		protected TextBox txtThreeLevelDeduct;

		protected Button btnUpdateProductDeducts;

		protected TrimTextBox txtProductTag;

		protected Button btnSetFreeShip;

		protected Button btnCancelFreeShip;

		protected void Page_Load(object sender, EventArgs e)
		{
			this.setting = SettingsManager.GetMasterSettings();
			this.btnUpdateProductDeducts.Click += this.btnUpdateProductDeducts_Click;
			if (!this.Page.IsPostBack)
			{
				int num = this.Page.Request["CategoryId"].ToInt(0);
				this.dropCategories.DataBind();
				if (num > 0)
				{
					this.dropCategories.SelectedValue = num;
				}
				this.dropBrandList.DataBind();
				this.dropTagList.DataBind();
				this.dropType.DataBind();
				this.dropShippingTemplateId.DataBind();
				SiteSettings siteSettings = HiContext.Current.SiteSettings;
				string value = base.Request.QueryString["ProductName"].ToNullString();
				if (!string.IsNullOrEmpty(value))
				{
					this.txtSearchText.Value = value;
				}
				string value2 = base.Request.QueryString["productCode"].ToNullString();
				if (!string.IsNullOrEmpty(value2))
				{
					this.txtSKU.Value = value2;
				}
				int num2 = base.Request.QueryString["BrandId"].ToInt(0);
				if (num2 > 0)
				{
					this.dropBrandList.SelectedValue = num2;
				}
				int num3 = base.Request.QueryString["TagId"].ToInt(0);
				if (num3 > 0)
				{
					this.dropTagList.SelectedValue = num3;
				}
				int num4 = base.Request.QueryString["TypeId"].ToInt(0);
				if (num4 > 0)
				{
					this.dropType.SelectedValue = num4;
				}
				if (base.Request.QueryString["ShippingTemplateId"] != null)
				{
					int num5 = base.Request.QueryString["ShippingTemplateId"].ToInt(0);
					if (num5 >= 0)
					{
						this.dropShippingTemplateId.SelectedValue = num5;
					}
				}
				if (base.Request.QueryString["isWarning"].ToInt(0) == 1)
				{
					this.chkIsWarningStock.Checked = true;
				}
				if (base.Request.QueryString["ProductType"] != null)
				{
					int num6 = base.Request.QueryString["ProductType"].ToInt(0);
					this.ddlProductType.SelectedValue = num6.ToNullString();
				}
				if (base.Request.QueryString["saleStatus"] != null)
				{
					int num7 = base.Request.QueryString["saleStatus"].ToInt(0);
					if (Enum.IsDefined(typeof(ProductSaleStatus), num7))
					{
						this.hidFilter.Value = num7.ToNullString();
					}
				}
				AttributeCollection attributes = this.txtSubMemberDeduct.Attributes;
				decimal num8 = siteSettings.SubMemberDeduct;
				attributes.Add("placeholder", "?????????????????????" + num8.ToString() + " %");
				AttributeCollection attributes2 = this.txtSecondLevelDeduct.Attributes;
				num8 = siteSettings.SecondLevelDeduct;
				attributes2.Add("placeholder", "?????????????????????" + num8.ToString() + " %");
				AttributeCollection attributes3 = this.txtThreeLevelDeduct.Attributes;
				num8 = siteSettings.ThreeLevelDeduct;
				attributes3.Add("placeholder", "?????????????????????" + num8.ToString() + " %");
			}
			if (base.Request.QueryString["IsMoreSearch"].ToInt(0) == 1)
			{
				this.so_more_none.Style.Add("display", "block");
			}
			else
			{
				this.so_more_none.Style.Add("display", "none");
			}
		}

		private void btnUpdateProductDeducts_Click(object sender, EventArgs e)
		{
			string text = base.Request.Form["CheckBoxGroup"];
			if (string.IsNullOrEmpty(text))
			{
				this.ShowMsg("??????????????????????????????????????????", false);
			}
			else
			{
				decimal secondLevelDeduct = default(decimal);
				decimal subMemberDeduct = default(decimal);
				decimal threeLevelDeduct = default(decimal);
				if (!string.IsNullOrEmpty(this.txtSecondLevelDeduct.Text.Trim()))
				{
					if (!decimal.TryParse(this.txtSecondLevelDeduct.Text.Trim(), out secondLevelDeduct))
					{
						this.ShowMsg("?????????????????????????????????????????????????????????", false);
					}
					else if (!string.IsNullOrEmpty(this.txtSubMemberDeduct.Text.Trim()))
					{
						if (!decimal.TryParse(this.txtSubMemberDeduct.Text, out subMemberDeduct))
						{
							this.ShowMsg("????????????????????????????????????????????????????????????", false);
						}
						else if (!string.IsNullOrEmpty(this.txtThreeLevelDeduct.Text.Trim()))
						{
							if (!decimal.TryParse(this.txtThreeLevelDeduct.Text, out threeLevelDeduct))
							{
								this.ShowMsg("?????????????????????????????????????????????????????????", false);
							}
							else if (ProductHelper.UpdateProductReferralDeduct(text, subMemberDeduct, secondLevelDeduct, threeLevelDeduct))
							{
								this.ShowMsg("???????????????????????????????????????", true);
							}
							else
							{
								this.ShowMsg("????????????????????????????????????", false);
							}
						}
						else
						{
							this.ShowMsg("???????????????????????????????????????", false);
						}
					}
					else
					{
						this.ShowMsg("??????????????????????????????????????????", false);
					}
				}
				else
				{
					this.ShowMsg("???????????????????????????????????????", false);
				}
			}
		}

		protected void btnImportToStore_Click(object sender, EventArgs e)
		{
			StoresQuery storesQuery = new StoresQuery();
			storesQuery.PageIndex = 1;
			storesQuery.PageSize = 2147483647;
			try
			{
				DbQueryResult storeManagers = ManagerHelper.GetStoreManagers(storesQuery);
				DataTable data = storeManagers.Data;
				foreach (DataRow row in data.Rows)
				{
					int storeId = row["StoreId"].ToInt(0);
					StoresHelper.ImportAllProductToAllStores(storeId);
				}
				this.ShowMsg("????????????", true);
			}
			catch (Exception)
			{
				this.ShowMsg("????????????,?????????", true);
			}
		}
	}
}
