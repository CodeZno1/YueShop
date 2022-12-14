using Hidistro.Context;
using Hidistro.Core;
using Hidistro.Entities;
using Hidistro.Entities.Members;
using Hidistro.Entities.Promotions;
using Hidistro.SaleSystem.Promotions;
using Hidistro.UI.Common.Controls;
using Hidistro.UI.SaleSystem.Tags;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	[ParseChildren(true)]
	public class WAPScratch : WAPMemberTemplatedWebControl
	{
		private int activityid;

		private HtmlImage bgimg;

		private Literal litActivityDesc;

		private Common_PrizeNames litPrizeNames;

		private Common_PrizeUsers litPrizeUsers;

		private Literal litStartDate;

		private Literal litEndDate;

		private HtmlInputHidden hidIsUsePoint;

		private HtmlInputHidden hidFreeTimes;

		private HtmlInputHidden hdTitle;

		private HtmlInputHidden hdDesc;

		private HtmlInputHidden hdAppId;

		private HtmlInputHidden hdImgUrl;

		private HtmlInputHidden hdLink;

		protected override void OnInit(EventArgs e)
		{
			if (this.SkinName == null)
			{
				this.SkinName = "Skin-VScratch.html";
			}
			base.OnInit(e);
		}

		protected override void AttachChildControls()
		{
			if (!int.TryParse(this.Page.Request.QueryString["activityid"], out this.activityid))
			{
				base.GotoResourceNotFound("");
			}
			MemberInfo user = HiContext.Current.User;
			if (user.UserId == 0)
			{
				HttpContext.Current.Response.Redirect("login.aspx?ReturnUrl=Scratch.aspx?activityid=" + this.activityid);
			}
			else
			{
				this.bgimg = (HtmlImage)this.FindControl("bgimg");
				this.litActivityDesc = (Literal)this.FindControl("litActivityDesc");
				this.litPrizeNames = (Common_PrizeNames)this.FindControl("litPrizeNames");
				this.litPrizeUsers = (Common_PrizeUsers)this.FindControl("litPrizeUsers");
				this.litStartDate = (Literal)this.FindControl("litStartDate");
				this.litEndDate = (Literal)this.FindControl("litEndDate");
				this.hidIsUsePoint = (HtmlInputHidden)this.FindControl("hidIsUsePoint");
				this.hidFreeTimes = (HtmlInputHidden)this.FindControl("hidFreeTimes");
				PageTitle.AddSiteNameTitle("?????????");
				this.hdTitle = (HtmlInputHidden)this.FindControl("hdTitle");
				this.hdAppId = (HtmlInputHidden)this.FindControl("hdAppId");
				this.hdDesc = (HtmlInputHidden)this.FindControl("hdDesc");
				this.hdImgUrl = (HtmlInputHidden)this.FindControl("hdImgUrl");
				this.hdLink = (HtmlInputHidden)this.FindControl("hdLink");
				ActivityInfo activityInfo = ActivityHelper.GetActivityInfo(this.activityid);
				if (activityInfo == null || activityInfo.ActivityType != 2)
				{
					base.GotoResourceNotFound("??????????????????");
				}
				else
				{
					Literal literal = this.litStartDate;
					DateTime dateTime = activityInfo.StartDate;
					literal.Text = dateTime.ToString("yyyy???MM???dd??? HH:mm:ss");
					Literal literal2 = this.litEndDate;
					dateTime = activityInfo.EndDate;
					literal2.Text = dateTime.ToString("yyyy???MM???dd??? HH:mm:ss");
					this.litActivityDesc.Text = activityInfo.Description;
					this.litPrizeNames.ActivityId = this.activityid;
					this.litPrizeUsers.ActivityId = this.activityid;
					if (activityInfo.StartDate < DateTime.Now && DateTime.Now < activityInfo.EndDate)
					{
						string arg = "????????????";
						int num = activityInfo.FreeTimes;
						ActivityJoinStatisticsInfo currUserActivityStatisticsInfo = ActivityHelper.GetCurrUserActivityStatisticsInfo(user.UserId, this.activityid);
						if (currUserActivityStatisticsInfo == null)
						{
							num = activityInfo.FreeTimes;
						}
						else if (activityInfo.ResetType == 2)
						{
							arg = "????????????";
							dateTime = DateTime.Now;
							DateTime date = dateTime.Date;
							dateTime = currUserActivityStatisticsInfo.LastJoinDate;
							num = ((!(date == dateTime.Date)) ? activityInfo.FreeTimes : (activityInfo.FreeTimes - currUserActivityStatisticsInfo.FreeNum));
						}
						else
						{
							num = activityInfo.FreeTimes - currUserActivityStatisticsInfo.FreeNum;
						}
						this.hidFreeTimes.Value = num.ToString();
						if (num <= 0)
						{
							this.hidIsUsePoint.Value = "1";
							Literal literal3 = this.litActivityDesc;
							literal3.Text += $"{arg}{activityInfo.FreeTimes}????????????????????????????????????0???,?????????????????????????????????{activityInfo.ConsumptionIntegral}????????????";
						}
						else
						{
							this.hidIsUsePoint.Value = "0";
							Literal literal4 = this.litActivityDesc;
							literal4.Text += $"{arg}{activityInfo.FreeTimes}????????????????????????????????????{num}??????";
						}
					}
					else
					{
						this.ShowMessage("???????????????????????????????????????", false, "", 1);
					}
					if (base.ClientType == ClientType.VShop)
					{
						SiteSettings masterSettings = SettingsManager.GetMasterSettings();
						string local = (!string.IsNullOrWhiteSpace(activityInfo.SharePic)) ? activityInfo.SharePic : "/Templates/common/images/process/ggk.jpg";
						this.hdImgUrl.Value = Globals.FullPath(local);
						this.hdTitle.Value = activityInfo.ActivityName;
						this.hdDesc.Value = activityInfo.ShareDetail;
						this.hdLink.Value = Globals.FullPath($"/vshop/Scratch.aspx?ActivityId={this.activityid}");
						this.hdAppId.Value = masterSettings.WeixinAppId;
					}
				}
			}
		}
	}
}
