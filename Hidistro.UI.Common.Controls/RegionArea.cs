using Hidistro.Context;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Hidistro.UI.Common.Controls
{
	[ParseChildren(true)]
	public class RegionArea : TemplatedWebControl
	{
		private HtmlGenericControl contents;

		private HtmlGenericControl contentRegion;

		protected override void AttachChildControls()
		{
			this.contents = (HtmlGenericControl)this.FindControl("contents");
			this.contentRegion = (HtmlGenericControl)this.FindControl("contentRegion");
			if (!this.Page.IsPostBack)
			{
				this.BindAreasHtml();
				this.BindRegionHtml();
			}
		}

		private void BindAreasHtml()
		{
			Dictionary<int, string> regions = RegionHelper.GetRegions();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<ul>");
			foreach (int key in regions.Keys)
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				string arg = string.Empty;
				Dictionary<int, string> provinces = RegionHelper.GetProvinces(key, false);
				foreach (int key2 in provinces.Keys)
				{
					stringBuilder2.Append(key2.ToString() + ",");
				}
				if (!string.IsNullOrEmpty(stringBuilder2.ToString()))
				{
					arg = stringBuilder2.ToString().Substring(0, stringBuilder2.ToString().Length - 1);
				}
				stringBuilder.AppendFormat("<li> <input id=\"areas_{0}\" onclick=\"checkRansack(this.value,this.checked)\" type=\"checkbox\" value=\"{1}\" class=\"icheck kc-danger\"/><label for=\"areas_{0}\">{2}</label> </li>", key, arg, regions[key]);
			}
			stringBuilder.Append("</ul>");
			this.contents.InnerHtml = stringBuilder.ToString();
		}

		private void BindRegionHtml()
		{
			Dictionary<int, string> regions = RegionHelper.GetRegions();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<ul>");
			foreach (int key in regions.Keys)
			{
				Dictionary<int, string> provinces = RegionHelper.GetProvinces(key, false);
				foreach (int key2 in provinces.Keys)
				{
					stringBuilder.AppendFormat("<li> <input id=\"{0}\" type=\"checkbox\"  value=\"{1}\"  class=\"icheck kc-danger\"/><label for=\"{0}\">{1}</label></li> ", key2, provinces[key2]);
				}
			}
			stringBuilder.Append("</ul>");
			this.contentRegion.InnerHtml = stringBuilder.ToString();
		}
	}
}
