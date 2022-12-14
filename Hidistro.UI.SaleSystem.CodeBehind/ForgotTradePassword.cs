using Hidistro.Context;
using Hidistro.Core;
using Hidistro.Core.Configuration;
using Hidistro.Core.Enums;
using Hidistro.Entities.Members;
using Hidistro.Messages;
using Hidistro.SaleSystem.Member;
using Hidistro.UI.Common.Controls;
using System;
using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	public class ForgotTradePassword : MemberTemplatedWebControl
	{
		public string source = "pay";

		public string orderId = "";

		private HtmlGenericControl htmDivUserName;

		private TextBox txtUserName;

		private IButton btnCheckUserName;

		private IButton btnCheckMobile;

		private IButton btnCheckEmail;

		private HtmlGenericControl htmDivQuestionAndAnswer;

		private Literal litUserQuestion;

		private Literal litMobile;

		private Literal litEmail;

		private TextBox txtUserAnswer;

		private TextBox txtMobileValid;

		private TextBox txtEmailValid;

		private Literal litAnswerMessage;

		private IButton btnCheckAnswer;

		private DropDownList dropType;

		private HtmlGenericControl htmDivPassword;

		private TextBox txtPassword;

		private TextBox txtRePassword;

		private IButton btnSetPassword;

		private IButton btnSendMobile;

		private IButton btnSendEmail;

		private IButton btnPrev;

		private IButton btnPrev2;

		private IButton btnPrev3;

		private HtmlGenericControl htmDivCellPhone;

		private HtmlGenericControl htmDivEmail;

		protected override void OnInit(EventArgs e)
		{
			if (this.SkinName == null)
			{
				this.SkinName = "User/Skin-ForgotTradePasswod.html";
			}
			base.OnInit(e);
		}

		private void LoadType()
		{
			this.dropType.Items.Clear();
			MemberInfo user = HiContext.Current.User;
			if (!string.IsNullOrEmpty(user.PasswordAnswer) && !string.IsNullOrEmpty(user.PasswordQuestion))
			{
				this.dropType.Items.Add(new ListItem("通过密保问题", "0"));
			}
			if (user.CellPhoneVerification || DataHelper.IsMobile(user.UserName))
			{
				this.dropType.Items.Add(new ListItem("通过手机号码", "1"));
			}
			if (user.EmailVerification || DataHelper.IsEmail(user.UserName))
			{
				this.dropType.Items.Add(new ListItem("通过电子邮箱", "2"));
			}
			if (this.dropType.Items.Count == 0)
			{
				this.ShowMessage("您现在还不能找回交易密码,请先设置密保问题答案或者绑定手机和邮箱", false, "/User/UserDefault", 3);
			}
		}

		protected override void AttachChildControls()
		{
			this.source = this.Page.Request.QueryString["Source"].ToNullString().ToLower();
			this.orderId = this.Page.Request.QueryString["orderId"].ToNullString().ToLower();
			this.htmDivUserName = (HtmlGenericControl)this.FindControl("htmDivUserName");
			this.txtUserName = (TextBox)this.FindControl("txtUserName");
			this.btnCheckUserName = ButtonManager.Create(this.FindControl("btnNextStep"));
			this.btnCheckMobile = ButtonManager.Create(this.FindControl("btnCheckMobile"));
			this.btnCheckEmail = ButtonManager.Create(this.FindControl("btnCheckEmail"));
			this.htmDivQuestionAndAnswer = (HtmlGenericControl)this.FindControl("htmDivQuestionAndAnswer");
			this.litUserQuestion = (Literal)this.FindControl("litUserQuestion");
			this.litMobile = (Literal)this.FindControl("litMobile");
			this.litEmail = (Literal)this.FindControl("litEmail");
			this.txtUserAnswer = (TextBox)this.FindControl("txtUserAnswer");
			this.txtMobileValid = (TextBox)this.FindControl("txtMobileValid");
			this.txtEmailValid = (TextBox)this.FindControl("txtEmailValid");
			this.litAnswerMessage = (Literal)this.FindControl("litAnswerMessage");
			this.btnCheckAnswer = ButtonManager.Create(this.FindControl("btnCheckAnswer"));
			this.btnPrev = ButtonManager.Create(this.FindControl("btnPrev"));
			this.btnPrev2 = ButtonManager.Create(this.FindControl("btnPrev2"));
			this.btnPrev3 = ButtonManager.Create(this.FindControl("btnPrev3"));
			this.btnPrev.Click += this.btnPrev_Click;
			this.btnPrev2.Click += this.btnPrev_Click;
			this.btnPrev3.Click += this.btnPrev_Click;
			this.htmDivPassword = (HtmlGenericControl)this.FindControl("htmDivPassword");
			this.htmDivCellPhone = (HtmlGenericControl)this.FindControl("htmDivCellPhone");
			this.htmDivEmail = (HtmlGenericControl)this.FindControl("htmDivEmail");
			this.txtPassword = (TextBox)this.FindControl("txtPassword");
			this.txtRePassword = (TextBox)this.FindControl("txtRePassword");
			this.dropType = (DropDownList)this.FindControl("dropType");
			this.btnSetPassword = ButtonManager.Create(this.FindControl("btnSetPassword"));
			PageTitle.AddSiteNameTitle("找回交易密码");
			this.btnCheckUserName.Click += this.btnCheckUserName_Click;
			this.btnCheckMobile.Click += this.btnCheckMobile_Click;
			this.btnCheckEmail.Click += this.btnCheckEmail_Click;
			this.btnCheckAnswer.Click += this.btnCheckAnswer_Click;
			this.btnSetPassword.Click += this.btnSetPassword_Click;
			if (!this.Page.IsPostBack)
			{
				this.LoadType();
				this.panelShow("InputUserName");
			}
		}

		private void btnPrev_Click(object sender, EventArgs e)
		{
			this.LoadType();
			this.panelShow("InputUserName");
		}

		private void btnSendMobile_Click(object sender, EventArgs e)
		{
			SiteSettings siteSettings = HiContext.Current.SiteSettings;
			MemberInfo user = HiContext.Current.User;
			if (user != null)
			{
				string str = HiContext.Current.CreatePhoneCode(6, user.CellPhone, VerifyCodeType.Digital);
				string text = default(string);
				SendStatus sendStatus = Messenger.SendSMS(user.CellPhone, "您本次的验证码是:" + str, siteSettings, out text);
				if (sendStatus == SendStatus.NoProvider || sendStatus == SendStatus.ConfigError)
				{
					this.ShowMessage("后台设置错误，请自行联系后台管理员", false, "", 1);
				}
				else
				{
					switch (sendStatus)
					{
					case SendStatus.Fail:
						this.ShowMessage("发送失败", false, "", 1);
						break;
					case SendStatus.Success:
						this.ShowMessage("发送成功", true, "", 1);
						break;
					}
				}
			}
		}

		private void btnSendEmail_Click(object sender, EventArgs e)
		{
			SiteSettings siteSettings = HiContext.Current.SiteSettings;
			MemberInfo user = HiContext.Current.User;
			string arg = HiContext.Current.CreateVerifyCode(6, VerifyCodeType.Digital, "");
			string body = $"亲爱的{user.UserName}：<br>您好！感谢您使用{siteSettings.SiteName}。<br>您正在进行账户基础信息维护，请在校验码输入框中输入：{arg}，以完成操作。 <br>注意：此操作可能会修改您的交易密码、登录邮箱或绑定手机。如非本人操作，请及时登录并修改密码以保证账户安全。（工作人员不会向您索取此校验码，请勿泄漏！） ";
			string text = default(string);
			SendStatus sendStatus = Messenger.SendMail("验证码", body, user.Email, siteSettings, out text);
			if (sendStatus == SendStatus.NoProvider || sendStatus == SendStatus.ConfigError)
			{
				this.ShowMessage("后台设置错误，请自行联系后台管理员", false, "", 1);
			}
			else
			{
				switch (sendStatus)
				{
				case SendStatus.Fail:
					this.ShowMessage("发送失败", false, "", 1);
					break;
				case SendStatus.Success:
					this.ShowMessage("发送成功", true, "", 1);
					break;
				}
			}
		}

		private void btnCheckEmail_Click(object sender, EventArgs e)
		{
			if (this.txtEmailValid.Text.Length == 0)
			{
				this.ShowMessage("请输入验证码", false, "", 1);
			}
			else if (!HiContext.Current.CheckVerifyCode(this.txtEmailValid.Text.ToLower(), ""))
			{
				this.ShowMessage("验证码输入错误，请重新输入", false, "", 1);
			}
			else
			{
				this.panelShow("InputPassword");
			}
		}

		private void btnCheckMobile_Click(object sender, EventArgs e)
		{
			string msg = "";
			MemberInfo user = HiContext.Current.User;
			if (user != null)
			{
				if (this.txtMobileValid.Text.Length == 0)
				{
					this.ShowMessage("请输入验证码", false, "", 1);
				}
				else if (!HiContext.Current.CheckPhoneVerifyCode(this.txtMobileValid.Text.ToNullString().ToLower().Trim(), user.CellPhone, out msg))
				{
					this.ShowMessage(msg, false, "", 1);
				}
				else
				{
					this.panelShow("InputPassword");
				}
			}
		}

		private void btnCheckUserName_Click(object sender, EventArgs e)
		{
			MemberInfo user = HiContext.Current.User;
			int num = this.dropType.SelectedValue.ToInt(0);
			if (user != null)
			{
				switch (num)
				{
				case 0:
					if (!string.IsNullOrEmpty(user.PasswordQuestion))
					{
						if (this.litUserQuestion != null)
						{
							this.litUserQuestion.Text = user.PasswordQuestion.ToString();
						}
						this.panelShow("InputAnswer");
					}
					else
					{
						this.ShowMessage("您没有设置密保问题，无法通过密保问题找回交易密码，请通过其他方式找回密码或是联系管理员修改交易密码", false, "", 1);
					}
					break;
				case 1:
					if (!string.IsNullOrEmpty(user.CellPhone) && user.CellPhone.Length >= 11)
					{
						if (this.litMobile != null && Regex.IsMatch(user.CellPhone, "^(13|14|15|18)\\d{9}$") && user.CellPhone.Length == 11)
						{
							this.litMobile.Text = user.CellPhone.Substring(0, 3) + "****" + user.CellPhone.Substring(7);
						}
						this.panelShow("CellPhone");
					}
					else
					{
						this.ShowMessage("您没有设置手机号码或者手机号码设置错误，无法通过手机号码找回交易密码，请通过其他方式找回交易密码或是联系管理员修改交易密码", false, "", 1);
					}
					break;
				case 2:
					if (!string.IsNullOrEmpty(user.Email))
					{
						if (this.litEmail != null)
						{
							this.litEmail.Text = user.Email;
						}
						this.panelShow("Email");
					}
					else
					{
						this.ShowMessage("没有设置电子邮箱", false, "", 1);
					}
					break;
				}
			}
			else
			{
				this.ShowMessage("该用户不存在", false, "", 1);
			}
		}

		private void btnCheckAnswer_Click(object sender, EventArgs e)
		{
			MemberInfo user = HiContext.Current.User;
			if (user.PasswordAnswer == this.txtUserAnswer.Text.Trim())
			{
				this.panelShow("InputPassword");
			}
			else
			{
				this.litAnswerMessage.Visible = true;
			}
		}

		private void btnSetPassword_Click(object sender, EventArgs e)
		{
			MemberInfo user = HiContext.Current.User;
			bool flag = false;
			if (string.IsNullOrEmpty(this.txtPassword.Text.Trim()) || string.IsNullOrEmpty(this.txtRePassword.Text.Trim()))
			{
				this.ShowMessage("密码不允许为空！", false, "", 1);
			}
			else if (this.txtPassword.Text.Trim() != this.txtRePassword.Text.Trim())
			{
				this.ShowMessage("两次输入的密码需一致", false, "", 1);
			}
			else if (this.txtPassword.Text.Length < 6 || this.txtPassword.Text.Length > HiConfiguration.GetConfig().PasswordMaxLength)
			{
				this.ShowMessage($"密码的长度只能在{6}和{HiConfiguration.GetConfig().PasswordMaxLength}个字符之间", false, "", 1);
			}
			else if (MemberProcessor.ChangeTradePassword(user, this.txtPassword.Text))
			{
				Messenger.UserDealPasswordChanged(user, this.txtPassword.Text);
				string text = "/User/UserOrders?orderStatus=1";
				text = ((!(this.source == "bdraw")) ? ((!(this.source == "sdraw")) ? (string.IsNullOrEmpty(this.orderId) ? "/User/UserOrders?orderStatus=1" : "/User/UserOrders?orderStatus=1") : "/User/SplittinDraws") : "/User/RequestBalanceDraw");
				this.ShowMessage("交易密码重置成功", true, text, 2);
			}
			else
			{
				this.ShowMessage("交易密码修改失败，请重试", false, "", 1);
			}
		}

		private void panelShow(string type)
		{
			this.litAnswerMessage.Visible = false;
			if (type == "InputUserName")
			{
				this.htmDivUserName.Visible = true;
				this.htmDivQuestionAndAnswer.Visible = false;
				this.htmDivPassword.Visible = false;
				this.htmDivCellPhone.Visible = false;
				this.htmDivEmail.Visible = false;
			}
			else if (type == "CellPhone")
			{
				this.htmDivCellPhone.Visible = true;
				this.htmDivUserName.Visible = false;
				this.htmDivQuestionAndAnswer.Visible = false;
				this.htmDivPassword.Visible = false;
				this.htmDivEmail.Visible = false;
			}
			else if (type == "Email")
			{
				this.htmDivEmail.Visible = true;
				this.htmDivCellPhone.Visible = false;
				this.htmDivUserName.Visible = false;
				this.htmDivQuestionAndAnswer.Visible = false;
				this.htmDivPassword.Visible = false;
			}
			else if (type == "InputAnswer")
			{
				this.htmDivUserName.Visible = false;
				this.htmDivQuestionAndAnswer.Visible = true;
				this.htmDivPassword.Visible = false;
				this.htmDivCellPhone.Visible = false;
				this.htmDivEmail.Visible = false;
			}
			else if (type == "InputPassword")
			{
				this.htmDivUserName.Visible = false;
				this.htmDivQuestionAndAnswer.Visible = false;
				this.htmDivPassword.Visible = true;
				this.htmDivCellPhone.Visible = false;
				this.htmDivEmail.Visible = false;
			}
		}
	}
}
