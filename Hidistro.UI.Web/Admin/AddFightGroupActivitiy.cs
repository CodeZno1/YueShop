using Hidistro.Core;
using Hidistro.Entities;
using Hidistro.Entities.Commodities;
using Hidistro.SaleSystem.Commodities;
using Hidistro.SaleSystem.Store;
using Hidistro.UI.Common.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin
{
	public class AddFightGroupActivitiy : AdminPage
	{
		public class DataForm
		{
			public string Price
			{
				get;
				set;
			}

			public string TotalCount
			{
				get;
				set;
			}

			public string StartTime
			{
				get;
				set;
			}

			public string EndTime
			{
				get;
				set;
			}

			public string Icon
			{
				get;
				set;
			}

			public string MaxCount
			{
				get;
				set;
			}

			public string JoinNumber
			{
				get;
				set;
			}

			public string LimitedHour
			{
				get;
				set;
			}
		}

		public static object ObjLock = new object();

		private string formData;

		private int productId;

		public string filterProductIds;

		protected Literal ltProductName;

		protected HtmlGenericControl liSkus;

		protected Repeater rptProductSkus;

		protected HiddenField hidUploadLogo;

		protected RadioButtonList rbtlTitle;

		protected TextBox txtFightGroupShareTitle;

		protected TextBox txtFightGroupShareDetails;

		protected HtmlGenericControl liSalePrice;

		protected Label lblPrice;

		protected HtmlGenericControl liDefaultPrice;

		protected TextBox txtPrice;

		protected HtmlGenericControl liDefaultStock;

		protected Literal ltStock;

		protected HtmlGenericControl liDefaultTotalCount;

		protected TextBox txtTotalCount;

		protected CalendarPanel CPStartTime;

		protected CalendarPanel CPEndDate;

		protected TextBox txtJoinNumber;

		protected TextBox txtLimitedHour;

		protected TextBox txtMaxCount;

		protected Button btnAddFightGroupActivitiy;

		protected HiddenField hfProductId;

		protected void Page_Load(object sender, EventArgs e)
		{
			this.formData = this.Page.Request["formData"].ToNullString();
			this.btnAddFightGroupActivitiy.Click += this.btnAddFightGroupActivitiy_Click;
			this.productId = this.Page.Request["productId"].ToInt(0);
			this.hfProductId.Value = this.productId.ToString();
			this.SetDateControl();
			if (!this.Page.IsPostBack)
			{
				this.BindProduct();
				this.ShowSKUOrDefault();
				this.BindFormData();
			}
			this.filterProductIds = VShopHelper.GetFightGroupActivitiyActiveProducts();
			if (this.productId > 0 && this.filterProductIds.Length > 0)
			{
				this.filterProductIds = this.filterProductIds + "," + this.productId;
			}
			else if (this.filterProductIds.Length == 0 && this.productId > 0)
			{
				this.filterProductIds = this.productId.ToString();
			}
		}

		private void SetDateControl()
		{
			Dictionary<string, object> calendarParameter = this.CPStartTime.CalendarParameter;
			DateTime now = DateTime.Now;
			calendarParameter.Add("startDate ", now.ToString("yyyy-MM-dd"));
			Dictionary<string, object> calendarParameter2 = this.CPEndDate.CalendarParameter;
			now = DateTime.Now;
			calendarParameter2.Add("startDate ", now.ToString("yyyy-MM-dd"));
			this.CPStartTime.FunctionNameForChangeDate = "fuChangeStartDate";
			this.CPEndDate.FunctionNameForChangeDate = "fuChangeEndDate";
			this.CPStartTime.CalendarParameter["format"] = "yyyy-mm-dd hh:ii";
			this.CPStartTime.CalendarParameter["minView"] = "0";
			this.CPEndDate.CalendarParameter["format"] = "yyyy-mm-dd hh:ii";
			this.CPEndDate.CalendarParameter["minView"] = "0";
		}

		private void ShowSKUOrDefault()
		{
			if (this.rptProductSkus.Items.Count == 0)
			{
				this.liSkus.Style.Add("display", "none");
				if (this.productId > 0)
				{
					this.liSalePrice.Style.Add("display", "block");
					this.liDefaultStock.Style.Add("display", "block");
				}
			}
			else
			{
				this.liDefaultPrice.Style.Add("display", "none");
				this.liDefaultStock.Style.Add("display", "none");
				this.liDefaultTotalCount.Style.Add("display", "none");
			}
		}

		private void BindProduct()
		{
			if (this.productId != 0)
			{
				this.rptProductSkus.DataSource = ProductHelper.GetSkusByProductIdNew(this.productId);
				this.rptProductSkus.DataBind();
				IList<int> list = null;
				Dictionary<int, IList<int>> dictionary = default(Dictionary<int, IList<int>>);
				ProductInfo productDetails = ProductHelper.GetProductDetails(this.productId, out dictionary, out list);
				if (productDetails != null)
				{
					this.ltProductName.Text = productDetails.ProductName;
					this.ltStock.Text = productDetails.Stock.ToString();
					this.lblPrice.Text = productDetails.MinSalePrice.F2ToString("f2");
				}
			}
		}

		private void btnAddFightGroupActivitiy_Click(object sender, EventArgs e)
		{
			lock (AddFightGroupActivitiy.ObjLock)
			{
				FightGroupActivityInfo fightGroupActivityInfo = new FightGroupActivityInfo();
				List<FightGroupSkuInfo> list = new List<FightGroupSkuInfo>();
				if (this.productId == 0)
				{
					this.ShowMsg(Formatter.FormatErrorMessage("???????????????"), false);
					goto end_IL_0009;
				}
				fightGroupActivityInfo.ProductId = this.productId;
				string text = Globals.StripAllTags(this.txtFightGroupShareTitle.Text.Trim());
				string text2 = Globals.StripAllTags(this.txtFightGroupShareDetails.Text.Trim());
				int num;
				if ((this.rbtlTitle.SelectedIndex != 1 || (!string.IsNullOrEmpty(text) && text.Length <= 60)) && !string.IsNullOrEmpty(text2))
				{
					num = ((text2.Length > 60) ? 1 : 0);
					goto IL_00bb;
				}
				num = 1;
				goto IL_00bb;
				IL_00bb:
				if (num != 0)
				{
					this.ShowMsg("???????????????????????????????????????", false);
				}
				else
				{
					if (this.rbtlTitle.SelectedIndex == 0)
					{
						fightGroupActivityInfo.ShareTitle = "";
					}
					else
					{
						fightGroupActivityInfo.ShareTitle = text;
					}
					fightGroupActivityInfo.ShareContent = text2;
					for (int i = 0; i < this.rptProductSkus.Items.Count; i++)
					{
						RepeaterItem repeaterItem = this.rptProductSkus.Items[i];
						HiddenField hiddenField = repeaterItem.FindControl("hfSkuId") as HiddenField;
						TextBox textBox = repeaterItem.FindControl("txtActivityStock") as TextBox;
						TextBox textBox2 = repeaterItem.FindControl("txtActivitySalePrice") as TextBox;
						if (textBox2.Text.Trim().ToDecimal(0) == decimal.Zero)
						{
							this.ShowMsg("???????????????????????????", false);
							return;
						}
						FightGroupSkuInfo item = new FightGroupSkuInfo
						{
							SalePrice = textBox2.Text.Trim().ToDecimal(0),
							TotalCount = textBox.Text.Trim().ToInt(0),
							SkuId = hiddenField.Value
						};
						list.Add(item);
					}
					if (this.rptProductSkus.Items.Count == 0)
					{
						if (this.txtPrice.Text.Trim().ToDecimal(0) == decimal.Zero)
						{
							this.ShowMsg("??????????????????", false);
							goto end_IL_0009;
						}
						if (this.txtTotalCount.Text.Trim().ToInt(0) == 0)
						{
							this.ShowMsg("?????????????????????", false);
							goto end_IL_0009;
						}
					}
					if (!this.CPStartTime.SelectedDate.HasValue)
					{
						this.ShowMsg(Formatter.FormatErrorMessage("?????????????????????"), false);
					}
					else
					{
						fightGroupActivityInfo.StartDate = this.CPStartTime.SelectedDate.Value;
						if (!this.CPEndDate.SelectedDate.HasValue)
						{
							this.ShowMsg(Formatter.FormatErrorMessage("?????????????????????"), false);
						}
						else
						{
							fightGroupActivityInfo.EndDate = this.CPEndDate.SelectedDate.Value;
							if (fightGroupActivityInfo.StartDate >= fightGroupActivityInfo.EndDate)
							{
								this.ShowMsg(Formatter.FormatErrorMessage("?????????????????????????????????"), false);
							}
							else if (fightGroupActivityInfo.EndDate <= DateTime.Now)
							{
								this.ShowMsg(Formatter.FormatErrorMessage("???????????????????????????????????????"), false);
							}
							else if (string.IsNullOrEmpty(this.txtJoinNumber.Text.Trim()))
							{
								this.ShowMsg("?????????????????????", false);
							}
							else
							{
								fightGroupActivityInfo.JoinNumber = this.txtJoinNumber.Text.ToInt(0);
								if (fightGroupActivityInfo.JoinNumber.ToInt(0) <= 1)
								{
									this.ShowMsg("?????????????????????1???0", false);
								}
								else if (string.IsNullOrEmpty(this.txtLimitedHour.Text.Trim()))
								{
									this.ShowMsg("?????????????????????", false);
								}
								else
								{
									fightGroupActivityInfo.LimitedHour = this.txtLimitedHour.Text.ToInt(0);
									if (fightGroupActivityInfo.LimitedHour.ToInt(0) <= 0)
									{
										this.ShowMsg("?????????????????????0", false);
									}
									else if (this.txtMaxCount.Text.Trim().ToInt(0) == 0)
									{
										this.ShowMsg("???????????????????????????", false);
									}
									else
									{
										fightGroupActivityInfo.MaxCount = this.txtMaxCount.Text.ToInt(0);
										if (VShopHelper.ProductFightGroupActivitiyExist(fightGroupActivityInfo.ProductId))
										{
											this.ShowMsg("???????????????????????????????????????", false);
										}
										else
										{
											int num2 = 0;
											num2 = ((this.rptProductSkus.Items.Count == 0) ? this.txtTotalCount.Text.Trim().ToInt(0) : list.Sum((FightGroupSkuInfo c) => c.TotalCount));
											if (fightGroupActivityInfo.MaxCount > num2)
											{
												this.ShowMsg(Formatter.FormatErrorMessage("??????????????????????????????????????????"), false);
											}
											else if (list.Count > 0 && fightGroupActivityInfo.MaxCount > (from c in list
											where c.TotalCount > 0
											select c).Min((FightGroupSkuInfo c) => c.TotalCount))
											{
												this.ShowMsg(Formatter.FormatErrorMessage("?????????????????????????????????????????????????????????"), false);
											}
											else
											{
												IList<int> list2 = null;
												Dictionary<int, IList<int>> dictionary = default(Dictionary<int, IList<int>>);
												ProductInfo productDetails = ProductHelper.GetProductDetails(fightGroupActivityInfo.ProductId, out dictionary, out list2);
												if (productDetails != null)
												{
													fightGroupActivityInfo.ProductName = productDetails.ProductName;
													if (productDetails.Stock < fightGroupActivityInfo.MaxCount)
													{
														this.ShowMsg("??????????????????????????????", false);
														goto end_IL_0009;
													}
													if (productDetails.Stock < num2)
													{
														this.ShowMsg($"????????????????????? {num2}?????????????????????????????????", false);
														goto end_IL_0009;
													}
													if (list.Count > 0)
													{
														foreach (KeyValuePair<string, SKUItem> sku in productDetails.Skus)
														{
															FightGroupSkuInfo fightGroupSkuInfo = (from s in list
															where s.SkuId == sku.Value.SkuId
															select s).FirstOrDefault();
															if (fightGroupSkuInfo == null)
															{
																this.ShowMsg("???????????????????????????????????????", false);
																return;
															}
															if (fightGroupSkuInfo.TotalCount > sku.Value.Stock)
															{
																this.ShowMsg($"??????????????????????????? {fightGroupSkuInfo.TotalCount}?????????????????????????????????", false);
																return;
															}
														}
													}
												}
												if (list.Count == 0)
												{
													DataTable skusByProductId = ProductHelper.GetSkusByProductId(this.productId);
													if (skusByProductId.Rows.Count > 0)
													{
														DataRow dataRow = skusByProductId.Rows[0];
														FightGroupSkuInfo item2 = new FightGroupSkuInfo
														{
															SalePrice = this.txtPrice.Text.Trim().ToDecimal(0),
															TotalCount = num2,
															SkuId = dataRow["SkuId"].ToNullString()
														};
														list.Add(item2);
													}
												}
												if (string.IsNullOrEmpty(this.hidUploadLogo.Value))
												{
													this.ShowMsg("?????????????????????", false);
												}
												else if (VShopHelper.CanAddFightGroupActivitiy(fightGroupActivityInfo.ProductId, productDetails.ProductName, 0))
												{
													fightGroupActivityInfo.Icon = Globals.SaveFile("GroupActivitie", this.hidUploadLogo.Value, "/Storage/master/", true, false, "");
													VShopHelper.AddFightGroupActivitie(fightGroupActivityInfo, list);
													this.ShowMsg("???????????????????????????", true, "FightGroupActivitiyList.aspx");
												}
												else
												{
													this.ShowMsg("??????????????????????????????????????????????????????????????????", false);
												}
											}
										}
									}
								}
							}
						}
					}
				}
				end_IL_0009:;
			}
		}

		private void BindFormData()
		{
			if (!string.IsNullOrEmpty(this.formData))
			{
				DataForm dataForm = JsonHelper.ParseFormJson<DataForm>(this.formData);
				this.txtMaxCount.Text = dataForm.MaxCount;
				this.txtPrice.Text = dataForm.Price;
				this.txtTotalCount.Text = dataForm.TotalCount;
				this.CPStartTime.SelectedDate = dataForm.StartTime.ToDateTime();
				this.CPEndDate.SelectedDate = dataForm.EndTime.ToDateTime();
				this.hidUploadLogo.Value = dataForm.Icon;
				this.txtJoinNumber.Text = dataForm.JoinNumber;
				this.txtLimitedHour.Text = dataForm.LimitedHour;
			}
		}
	}
}
