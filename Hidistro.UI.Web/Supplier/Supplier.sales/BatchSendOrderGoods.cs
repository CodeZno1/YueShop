using Hidistro.Context;
using Hidistro.Core;
using Hidistro.Entities.Members;
using Hidistro.Entities.Orders;
using Hidistro.Entities.Promotions;
using Hidistro.Entities.Sales;
using Hidistro.Messages;
using Hidistro.SaleSystem.Sales;
using Hidistro.SaleSystem.Store;
using Hidistro.UI.Common.Controls;
using Hishop.Plugins;
using System;
using System.Data;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Supplier.sales
{
	public class BatchSendOrderGoods : SupplierAdminPage
	{
		private string strIds;

		protected DropDownList dropExpressComputerpe;

		protected TextBox txtStartShipOrderNumber;

		protected Button btnSetShipOrderNumber;

		protected Repeater repOrderGoods;

		protected Repeater grdOrderGoods;

		protected Button btnBatchSendGoods;

		protected void Page_Load(object sender, EventArgs e)
		{
			this.strIds = base.Request.QueryString["OrderIds"];
			this.btnSetShipOrderNumber.Click += this.btnSetShipOrderNumber_Click;
			this.grdOrderGoods.ItemDataBound += this.grdOrderGoods_RowDataBound;
			this.btnBatchSendGoods.Click += this.btnSendGoods_Click;
			if (!this.Page.IsPostBack)
			{
				this.dropExpressComputerpe.DataSource = ExpressHelper.GetAllExpress(false);
				this.dropExpressComputerpe.DataTextField = "Name";
				this.dropExpressComputerpe.DataValueField = "Name";
				this.dropExpressComputerpe.DataBind();
				this.dropExpressComputerpe.Items.Insert(0, new ListItem("", ""));
				this.BindData();
			}
		}

		private void grdOrderGoods_RowDataBound(object sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				HiddenField hiddenField = (HiddenField)e.Item.FindControl("hidorderId");
				string value = hiddenField.Value;
				ExpressDropDownList expressDropDownList = e.Item.FindControl("expressList1") as ExpressDropDownList;
				OrderInfo orderInfo = OrderHelper.GetOrderInfo(value);
				if (orderInfo != null && (orderInfo.OrderStatus == OrderStatus.BuyerAlreadyPaid || (orderInfo.OrderStatus == OrderStatus.WaitBuyerPay && orderInfo.Gateway == "hishop.plugins.payment.podrequest")) && orderInfo.ItemStatus == OrderItemStatus.Nomarl)
				{
					expressDropDownList.DataBind();
					expressDropDownList.SelectedValue = orderInfo.ExpressCompanyName;
				}
			}
		}

		protected string GetOrderIds()
		{
			string text = string.Empty;
			for (int i = 0; i < this.grdOrderGoods.Items.Count; i++)
			{
				RepeaterItem repeaterItem = this.grdOrderGoods.Items[i];
				HiddenField hiddenField = (HiddenField)this.grdOrderGoods.Items[i].FindControl("hidorderId");
				string value = hiddenField.Value;
				text = text + value + ",";
			}
			return text.TrimEnd(',');
		}

		private void btnSetShipOrderNumber_Click(object sender, EventArgs e)
		{
			string orderIds = this.GetOrderIds();
			string[] orderIds2 = orderIds.Split(',');
			long num = 0L;
			string orderIds3 = "'" + orderIds.Replace(",", "','") + "'";
			if (!string.IsNullOrEmpty(this.dropExpressComputerpe.SelectedValue))
			{
				OrderHelper.SetOrderExpressComputerpe(orderIds3, this.dropExpressComputerpe.SelectedItem.Text, this.dropExpressComputerpe.SelectedValue);
			}
			if (!string.IsNullOrEmpty(this.txtStartShipOrderNumber.Text.Trim()) && long.TryParse(this.txtStartShipOrderNumber.Text.Trim(), out num))
			{
				try
				{
					OrderHelper.SetOrderShipNumber(orderIds2, this.txtStartShipOrderNumber.Text.Trim(), this.dropExpressComputerpe.SelectedValue);
					this.BindData();
				}
				catch (Exception)
				{
					this.ShowMsg("?????????????????????????????????", false);
				}
			}
			else
			{
				this.ShowMsg("??????????????????????????????????????????????????????", false);
			}
		}

		private void btnSendGoods_Click(object sender, EventArgs e)
		{
			if (this.grdOrderGoods.Items.Count <= 0)
			{
				this.ShowMsg("?????????????????????????????????", false);
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				SiteSettings masterSettings = SettingsManager.GetMasterSettings();
				int num = 0;
				for (int i = 0; i < this.grdOrderGoods.Items.Count; i++)
				{
					HiddenField hiddenField = (HiddenField)this.grdOrderGoods.Items[i].FindControl("hidorderId");
					string value = hiddenField.Value;
					TextBox textBox = (TextBox)this.grdOrderGoods.Items[i].FindControl("txtShippOrderNumber");
					ExpressDropDownList expressDropDownList = this.grdOrderGoods.Items[i].FindControl("expressList1") as ExpressDropDownList;
					OrderInfo orderInfo = OrderHelper.GetOrderInfo(value);
					if ((orderInfo.GroupBuyId <= 0 || orderInfo.GroupBuyStatus == GroupBuyStatus.Success) && ((orderInfo.OrderStatus == OrderStatus.WaitBuyerPay && orderInfo.Gateway == "hishop.plugins.payment.podrequest") || orderInfo.OrderStatus == OrderStatus.BuyerAlreadyPaid))
					{
						ExpressCompanyInfo expressCompanyInfo = null;
						if (!string.IsNullOrEmpty(expressDropDownList.SelectedValue))
						{
							expressCompanyInfo = ExpressHelper.FindNode(expressDropDownList.SelectedValue);
						}
						if (expressCompanyInfo != null)
						{
							if (!string.IsNullOrEmpty(orderInfo.OuterOrderId) && orderInfo.OuterOrderId.StartsWith("jd_") && string.IsNullOrWhiteSpace(expressCompanyInfo.JDCode))
							{
								continue;
							}
							orderInfo.ExpressCompanyName = expressCompanyInfo.Name;
							orderInfo.ExpressCompanyAbb = expressCompanyInfo.Kuaidi100Code;
							orderInfo.ShipOrderNumber = textBox.Text;
						}
						if (OrderHelper.SendGoods(orderInfo) && expressCompanyInfo != null && !string.IsNullOrEmpty(orderInfo.ExpressCompanyAbb) && orderInfo.ExpressCompanyAbb.ToUpper() == "HTKY")
						{
							ExpressHelper.GetDataByKuaidi100(orderInfo.ExpressCompanyAbb, orderInfo.ShipOrderNumber);
						}
						if (!string.IsNullOrEmpty(orderInfo.GatewayOrderId) && orderInfo.GatewayOrderId.Trim().Length > 0)
						{
							PaymentModeInfo paymentMode = SalesHelper.GetPaymentMode(orderInfo.Gateway);
							if (paymentMode != null)
							{
								string hIGW = paymentMode.Gateway.Replace(".", "_");
								PaymentRequest paymentRequest = PaymentRequest.CreateInstance(paymentMode.Gateway, HiCryptographer.Decrypt(paymentMode.Settings), orderInfo.PayOrderId, orderInfo.GetTotal(false), "????????????", "?????????-" + orderInfo.PayOrderId, orderInfo.EmailAddress, orderInfo.OrderDate, Globals.FullPath(""), Globals.FullPath(base.GetRouteUrl("PaymentReturn_url", new
								{
									HIGW = hIGW
								})), Globals.FullPath(base.GetRouteUrl("PaymentNotify_url", new
								{
									HIGW = hIGW
								})), "");
								paymentRequest.SendGoods(orderInfo.GatewayOrderId, orderInfo.RealModeName, orderInfo.ShipOrderNumber, "EXPRESS");
							}
						}
						if (!string.IsNullOrEmpty(orderInfo.OuterOrderId) && expressCompanyInfo != null)
						{
							if (orderInfo.OuterOrderId.StartsWith("tb_"))
							{
								string text = orderInfo.OuterOrderId.Replace("tb_", "");
								try
								{
									string requestUriString = $"http://order2.kuaidiangtong.com/UpdateShipping.ashx?tid={text}&companycode={expressCompanyInfo.TaobaoCode}&outsid={orderInfo.ShipOrderNumber}&Host={HiContext.Current.SiteUrl}";
									WebRequest webRequest = WebRequest.Create(requestUriString);
									webRequest.GetResponse();
								}
								catch
								{
								}
							}
							else if (orderInfo.OuterOrderId.StartsWith("jd_") && expressCompanyInfo != null)
							{
								string text = orderInfo.OuterOrderId.Replace("jd_", "");
								try
								{
									JDHelper.JDOrderOutStorage(masterSettings.JDAppKey, masterSettings.JDAppSecret, masterSettings.JDAccessToken, expressCompanyInfo.JDCode, orderInfo.ShipOrderNumber, text);
								}
								catch (Exception ex)
								{
									stringBuilder.Append($"?????????{orderInfo.OrderId}?????????????????????????????????????????????{text}???{ex.Message}\r\n");
								}
							}
						}
						int num2 = orderInfo.UserId;
						if (num2 == 1100)
						{
							num2 = 0;
						}
						MemberInfo user = Users.GetUser(num2);
						Messenger.OrderShipping(orderInfo, user);
						orderInfo.OnDeliver();
						num++;
					}
				}
				if (num == 0)
				{
					this.ShowMsg("??????????????????,??????????????????????????????", false);
				}
				else if (num > 0)
				{
					this.ShowMsgCloseWindow(string.Format("?????????????????????????????????{0}??????{1}", num, (stringBuilder.Length > 0) ? stringBuilder.ToString() : ""), true);
				}
			}
		}

		private void BindData()
		{
			SiteSettings masterSettings = SettingsManager.GetMasterSettings();
			string orderIds = "'" + this.strIds.Replace(",", "','") + "'";
			DataTable sendGoodsOrders = OrderHelper.GetSendGoodsOrders(orderIds, masterSettings.OpenMultStore, false);
			DataView defaultView = sendGoodsOrders.DefaultView;
			defaultView.RowFilter = "SupplierId=" + HiContext.Current.Manager.StoreId;
			this.grdOrderGoods.DataSource = defaultView;
			this.grdOrderGoods.DataBind();
		}
	}
}
