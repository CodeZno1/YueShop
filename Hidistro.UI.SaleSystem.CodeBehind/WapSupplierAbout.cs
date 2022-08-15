using Hidistro.Core;
using Hidistro.Entities.Supplier;
using Hidistro.SaleSystem.Supplier;
using Hidistro.UI.Common.Controls;
using System;
using System.Web.UI.WebControls;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	public class WapSupplierAbout : WAPTemplatedWebControl
	{
		private Literal litSupplierName;

		private Literal litSupplierAbout;

		protected override void OnInit(EventArgs e)
		{
			if (this.SkinName == null)
			{
				this.SkinName = "Skin-SupplierAbout.html";
			}
			base.OnInit(e);
		}

		protected override void AttachChildControls()
		{
			this.litSupplierName = (Literal)this.FindControl("litSupplierName");
			this.litSupplierAbout = (Literal)this.FindControl("litSupplierAbout");
			int supplierid = base.GetParameter("SupplierId").ToInt(0);
			SupplierInfo supplierById = SupplierHelper.GetSupplierById(supplierid);
			if (supplierById == null)
			{
				base.GotoResourceNotFound("");
			}
			else
			{
				this.litSupplierName.SetWhenIsNotNull(supplierById.SupplierName);
				this.litSupplierAbout.SetWhenIsNotNull(supplierById.Introduce);
			}
		}
	}
}
