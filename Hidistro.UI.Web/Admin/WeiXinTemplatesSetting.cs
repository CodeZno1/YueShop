using Hidistro.Entities.Store;
using Hidistro.Messages;
using Hidistro.SaleSystem.Store;
using Hidistro.UI.Common.Controls;
using System;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin
{
	[PrivilegeCheck(Privilege.WeiXinTemplatesSet)]
	public class WeiXinTemplatesSetting : AdminPage
	{
		protected Repeater grdWxTempletsNew;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!this.Page.IsPostBack)
			{
				this.grdWxTempletsNew.DataSource = MessageTemplateHelper.GetWxMessageTemplates();
				this.grdWxTempletsNew.DataBind();
			}
		}
	}
}
