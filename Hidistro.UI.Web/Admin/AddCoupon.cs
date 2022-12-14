using Hidistro.Context;
using Hidistro.Entities.Promotions;
using Hidistro.Entities.Store;
using Hidistro.SaleSystem.Promotions;
using Hidistro.SaleSystem.Store;
using Hidistro.UI.Common.Controls;
using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin
{
	[PrivilegeCheck(Privilege.Coupons)]
	public class AddCoupon : AdminPage
	{
		protected Panel pnlReMark;

		protected TextBox txtCouponName;

		protected TextBox txtPrice;

		protected TextBox txtSendCount;

		protected HtmlSelect ddlUserLimitCount;

		protected RadioButton radNoLimit;

		protected RadioButton radUseFullCut;

		protected TextBox txtFullPrice;

		protected CalendarPanel calendarStartDate;

		protected CalendarPanel calendarEndDate;

		protected RadioButton radAll;

		protected RadioButton radSomeProducts;

		protected Label lblSelectCount;

		protected RadioButton radActiveReceive;

		protected RadioButton radGrant;

		protected RadioButton radExchange;

		protected TextBox txtNeedPoint;

		protected CheckBox chkPanicBuying;

		protected CheckBox chkGroup;

		protected CheckBox chkFireGroup;

		protected Button btnAddCoupons;

		protected HiddenField hidSelectProducts;

		protected HiddenField hidProductIds;

		protected HiddenField hidAllSelectedProducts;

		protected void Page_Load(object sender, EventArgs e)
		{
			this.btnAddCoupons.Click += this.btnAddCoupons_Click;
			Dictionary<string, object> calendarParameter = this.calendarStartDate.CalendarParameter;
			DateTime now = DateTime.Now;
			calendarParameter.Add("startDate ", now.ToString("yyyy-MM-dd"));
			Dictionary<string, object> calendarParameter2 = this.calendarEndDate.CalendarParameter;
			now = DateTime.Now;
			calendarParameter2.Add("startDate ", now.ToString("yyyy-MM-dd"));
			if (!this.Page.IsPostBack)
			{
				this.pnlReMark.Visible = SettingsManager.GetMasterSettings().OpenMultStore;
			}
		}

		private void btnAddCoupons_Click(object sender, EventArgs e)
		{
			int num = 0;
			decimal num2 = default(decimal);
			decimal num3 = default(decimal);
			int num4 = 0;
			if (string.IsNullOrEmpty(this.txtCouponName.Text.Trim()))
			{
				this.ShowMsg("???????????????????????????", false);
			}
			else if (!decimal.TryParse(this.txtPrice.Text, out num2) || num2 > 10000m || num2 < decimal.Zero)
			{
				this.ShowMsg("????????????????????????0.01-10000??????2????????????", false);
			}
			else if (!int.TryParse(this.txtSendCount.Text, out num) || num > 100000000 || num < 0)
			{
				this.ShowMsg("?????????????????????????????????0-100000000?????????", false);
			}
			else if (int.Parse(this.ddlUserLimitCount.Value) > num)
			{
				this.ShowMsg("?????????????????????????????????????????????", false);
			}
			else if (this.radUseFullCut.Checked && (!decimal.TryParse(this.txtFullPrice.Text, out num3) || num3 > 10000000m || num3 < decimal.One))
			{
				this.ShowMsg("???????????????????????????????????????????????????1-10000000?????????", false);
			}
			else if (!this.calendarStartDate.SelectedDate.HasValue)
			{
				this.ShowMsg("????????????????????????????????????", false);
			}
			else if (!this.calendarEndDate.SelectedDate.HasValue)
			{
				this.ShowMsg("????????????????????????????????????", false);
			}
			else
			{
				DateTime dateTime = this.calendarStartDate.SelectedDate.Value;
				if (dateTime.CompareTo(this.calendarEndDate.SelectedDate.Value) > 0)
				{
					this.ShowMsg("???????????????????????????????????????????????????", false);
				}
				else if (this.radSomeProducts.Checked && string.IsNullOrEmpty(this.hidProductIds.Value))
				{
					this.ShowMsg("???????????????????????????", false);
				}
				else if (!this.radActiveReceive.Checked && !this.radGrant.Checked && !this.radExchange.Checked)
				{
					this.ShowMsg("????????????????????????", false);
				}
				else if (this.radExchange.Checked && (!int.TryParse(this.txtNeedPoint.Text, out num4) || num4 > 10000000 || num4 <= 0))
				{
					this.ShowMsg("??????????????????????????????????????????1-10000000?????????", false);
				}
				else
				{
					CouponInfo couponInfo = new CouponInfo();
					couponInfo.CouponName = this.txtCouponName.Text;
					couponInfo.Price = num2;
					couponInfo.SendCount = num;
					couponInfo.UserLimitCount = int.Parse(this.ddlUserLimitCount.Value);
					couponInfo.OrderUseLimit = (this.radNoLimit.Checked ? decimal.Zero : num3);
					couponInfo.StartTime = this.calendarStartDate.SelectedDate.Value;
					CouponInfo couponInfo2 = couponInfo;
					dateTime = this.calendarEndDate.SelectedDate.Value;
					DateTime closingTime;
					if (!dateTime.Equals(this.calendarEndDate.SelectedDate.Value.Date))
					{
						closingTime = this.calendarEndDate.SelectedDate.Value;
					}
					else
					{
						dateTime = this.calendarEndDate.SelectedDate.Value;
						dateTime = dateTime.AddDays(1.0);
						closingTime = dateTime.AddSeconds(-1.0);
					}
					couponInfo2.ClosingTime = closingTime;
					couponInfo.CanUseProducts = this.hidProductIds.Value;
					couponInfo.UseWithFireGroup = this.chkFireGroup.Checked;
					int obtainWay = 0;
					if (this.radActiveReceive.Checked)
					{
						obtainWay = 0;
					}
					else if (this.radGrant.Checked)
					{
						obtainWay = 1;
					}
					else if (this.radExchange.Checked)
					{
						obtainWay = 2;
					}
					couponInfo.ObtainWay = obtainWay;
					if (this.radExchange.Checked)
					{
						couponInfo.NeedPoint = num4;
					}
					couponInfo.UseWithGroup = this.chkGroup.Checked;
					couponInfo.UseWithPanicBuying = this.chkPanicBuying.Checked;
					switch (CouponHelper.AddCoupon(couponInfo))
					{
					case CouponActionStatus.UnknowError:
						this.ShowMsg("????????????", false);
						break;
					case CouponActionStatus.DuplicateName:
						this.ShowMsg("????????????????????????????????????", false);
						break;
					default:
						this.ShowMsg("?????????????????????", true, "NewCoupons.aspx");
						break;
					}
				}
			}
		}
	}
}
