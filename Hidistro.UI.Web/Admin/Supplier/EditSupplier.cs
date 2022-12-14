using Hidistro.Context;
using Hidistro.Core;
using Hidistro.Core.Enums;
using Hidistro.Entities.Store;
using Hidistro.Entities.Supplier;
using Hidistro.SaleSystem.Store;
using Hidistro.SaleSystem.Supplier;
using Hidistro.UI.Common.Controls;
using Hidistro.UI.Web.Admin.Ascx;
using System;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin.Supplier
{
	[PrivilegeCheck(Privilege.EditSupplier)]
	public class EditSupplier : AdminPage
	{
		private int supplierId = 0;

		protected Literal lblManageId;

		protected Literal lblUserName;

		protected Literal lblStatus;

		protected TrimTextBox txtSupplierName;

		protected HiddenField hidUploadImages;

		protected HiddenField hidOldImages;

		protected TrimTextBox txtContactMan;

		protected TrimTextBox txtTel;

		protected RegionSelector dropRegion;

		protected TrimTextBox txtAddress;

		protected Ueditor editDescription;

		protected Button btnAdd;

		protected Literal lblUserName2;

		protected Literal lblSupplierName;

		protected ImageList ImageList;

		[AdministerCheck(true)]
		protected void Page_Load(object sender, EventArgs e)
		{
			this.supplierId = base.Request.QueryString["SupplierId"].ToInt(0);
			if (this.supplierId == 0)
			{
				base.Response.Redirect("SuppliersList.aspx");
			}
			this.btnAdd.Click += this.btnAdd_Click;
			if (!this.Page.IsPostBack)
			{
				this.BindData();
			}
		}

		public void BindData()
		{
			SupplierInfo supplierById = SupplierHelper.GetSupplierById(this.supplierId);
			ManagerInfo managerInfo = ManagerHelper.FindManagerByStoreIdAndRoleId(this.supplierId, -2);
			if (supplierById == null || managerInfo == null)
			{
				base.Response.Redirect("SupplierList.aspx");
			}
			else
			{
				this.lblManageId.Text = managerInfo.ManagerId.ToString();
				Literal literal = this.lblUserName;
				Literal literal2 = this.lblUserName2;
				string text2 = literal.Text = (literal2.Text = managerInfo.UserName);
				TrimTextBox trimTextBox = this.txtSupplierName;
				Literal literal3 = this.lblSupplierName;
				text2 = (trimTextBox.Text = (literal3.Text = supplierById.SupplierName));
				string text4 = "?????? &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href=\"javascript:void(0);\" onclick=\"updatesupperstate(2)\">??????</a>";
				if (supplierById.Status != 1)
				{
					text4 = "?????? &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href=\"javascript:void(0);\" onclick=\"updatesupperstate(1)\">??????</a>";
				}
				this.lblStatus.Text = text4;
				this.hidOldImages.Value = supplierById.Picture;
				this.txtContactMan.Text = supplierById.ContactMan;
				this.txtTel.Text = supplierById.Tel;
				this.dropRegion.SetSelectedRegionId(supplierById.RegionId);
				this.txtAddress.Text = supplierById.Address;
				this.editDescription.Text = supplierById.Introduce;
			}
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			this.UpdateSupplier();
			this.ShowMsg("????????????", true, "SupplierList.aspx");
		}

		private void UpdateSupplier()
		{
			SupplierInfo supplierById = SupplierHelper.GetSupplierById(this.supplierId);
			ManagerInfo managerInfo = ManagerHelper.FindManagerByStoreId(this.supplierId, SystemRoles.SupplierAdmin);
			if (managerInfo == null || supplierById == null)
			{
				base.Response.Redirect("SupplierList.aspx");
			}
			string supplierName = Globals.StripAllTags(this.txtSupplierName.Text.Trim());
			string text = Globals.StripAllTags(this.txtAddress.Text);
			string text2 = this.txtTel.Text;
			string text3 = Globals.StripAllTags(this.txtContactMan.Text);
			if (SupplierHelper.ExistSupplierName(this.supplierId, supplierName))
			{
				this.ShowMsg("???????????????????????????,??????????????????", false);
			}
			else if (!this.dropRegion.GetSelectedRegionId().HasValue)
			{
				this.ShowMsg("?????????????????????????????????", false);
			}
			else if (text3.Length > 8 || text3.Length < 2)
			{
				this.ShowMsg("??????????????????,????????????????????????2-8??????", false);
			}
			else if (text.Length > 50 || text.Length < 2)
			{
				this.ShowMsg("???????????????,???????????????2-50????????????", false);
			}
			else if (text2 == "" || !DataHelper.IsTel(text2))
			{
				this.ShowMsg("?????????????????????????????????????????????????????????)???", false);
			}
			else
			{
				int value = this.dropRegion.GetSelectedRegionId().Value;
				supplierById.SupplierName = supplierName;
				supplierById.Picture = this.UploadImage();
				supplierById.Tel = text2;
				supplierById.Address = text;
				supplierById.ContactMan = text3;
				supplierById.RegionId = value;
				supplierById.FullRegionPath = RegionHelper.GetFullPath(value, true);
				supplierById.Introduce = this.editDescription.Text;
				SupplierHelper.UpdateSupplier(supplierById);
			}
		}

		private string UploadImage()
		{
			string str = Globals.GetStoragePath() + "/temp/";
			string text = HttpContext.Current.Server.MapPath(Globals.GetStoragePath() + "/Supplier/");
			if (!Globals.PathExist(text, false))
			{
				Globals.CreatePath(text);
			}
			string value = this.hidUploadImages.Value;
			if (value.Trim().Length == 0)
			{
				return string.Empty;
			}
			value = value.Replace("//", "/");
			string text2 = (value.Split('/').Length == 6) ? value.Split('/')[5] : value.Split('/')[4];
			if (File.Exists(text + text2))
			{
				return Globals.GetStoragePath() + "/Supplier/" + text2;
			}
			File.Copy(HttpContext.Current.Server.MapPath(this.hidUploadImages.Value), text + text2);
			string path = HttpContext.Current.Server.MapPath(str + text2);
			if (File.Exists(path))
			{
				File.Delete(path);
			}
			return Globals.GetStoragePath() + "/Supplier/" + text2;
		}
	}
}
