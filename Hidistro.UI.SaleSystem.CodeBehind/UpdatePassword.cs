using Hidistro.Context;
using Hidistro.Core;
using Hidistro.Entities.Members;
using Hidistro.Messages;
using Hidistro.SaleSystem.Member;
using Hidistro.UI.Common.Controls;
using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	public class UpdatePassword : MemberTemplatedWebControl
	{
		private SmallStatusMessage status;

		private Literal textPasswordView;

		private Literal passwordText;

		private IButton btnChangePassword;

		private IButton btnSetPassword;

		private HtmlGenericControl LkUpdateTradePassword;

		private HtmlGenericControl setpwdPanel;

		private HtmlGenericControl changepwdPanel;

		private TextBox txtOdlPasswordChange;

		private TextBox txtNewPassword;

		private TextBox txtNewPassword2;

		private TextBox txtNewPasswordChange;

		private TextBox txtNewPassword2Change;

		protected override void OnInit(EventArgs e)
		{
			if (this.SkinName == null)
			{
				this.SkinName = "User/Skin-UpdatePassword.html";
			}
			base.OnInit(e);
		}

		protected override void AttachChildControls()
		{
			this.status = (SmallStatusMessage)this.FindControl("status");
			this.txtOdlPasswordChange = (TextBox)this.FindControl("txtOdlPasswordChange");
			this.txtNewPasswordChange = (TextBox)this.FindControl("txtNewPasswordChange");
			this.txtNewPassword2Change = (TextBox)this.FindControl("txtNewPassword2Change");
			this.txtNewPassword = (TextBox)this.FindControl("txtNewPassword");
			this.txtNewPassword2 = (TextBox)this.FindControl("txtNewPassword2");
			this.btnChangePassword = ButtonManager.Create(this.FindControl("btnChangePassword"));
			this.btnSetPassword = ButtonManager.Create(this.FindControl("btnSetPassword"));
			this.LkUpdateTradePassword = (HtmlGenericControl)this.FindControl("one2");
			this.setpwdPanel = (HtmlGenericControl)this.FindControl("setpwdPanel");
			this.changepwdPanel = (HtmlGenericControl)this.FindControl("changepwdPanel");
			this.textPasswordView = (Literal)this.FindControl("textPasswordView");
			this.passwordText = (Literal)this.FindControl("passwordText");
			this.btnChangePassword.Click += this.btnChangePassword_Click;
			this.btnSetPassword.Click += this.btnSetPassword_Click;
			if (!this.Page.IsPostBack)
			{
				MemberInfo user = HiContext.Current.User;
				if (user != null)
				{
					if (string.IsNullOrWhiteSpace(user.TradePassword))
					{
						this.LkUpdateTradePassword.Visible = false;
					}
					if (user.PasswordSalt == "Open")
					{
						this.setpwdPanel.Visible = true;
						this.changepwdPanel.Visible = false;
						this.textPasswordView.Text = "??????????????????";
						this.passwordText.Text = "??????????????????";
						PageTitle.AddSiteNameTitle("??????????????????");
					}
					else
					{
						this.changepwdPanel.Visible = true;
						this.setpwdPanel.Visible = false;
						this.textPasswordView.Text = "??????????????????";
						this.passwordText.Text = "??????????????????";
						PageTitle.AddSiteNameTitle("??????????????????");
					}
				}
			}
		}

		private void btnSetPassword_Click(object sender, EventArgs e)
		{
			string text = this.txtNewPassword2.Text.ToNullString();
			string changedPassword = text;
			MemberInfo user = HiContext.Current.User;
			if (user == null)
			{
				this.ShowMessage("???????????????", false, "", 1);
			}
			else if (user.PasswordSalt != "Open")
			{
				this.ShowMessage("???????????????????????????????????????????????????????????????", false, "", 1);
			}
			else if (text.Length < 6 || text.Length > 20)
			{
				this.ShowMessage("???????????????6-20??????????????????", false, "", 1);
			}
			else
			{
				string text2 = Globals.RndStr(128, true);
				text = (user.Password = Users.EncodePassword(text, text2));
				user.PasswordSalt = text2;
				if (MemberProcessor.UpdateMember(user))
				{
					Messenger.UserPasswordChanged(HiContext.Current.User, changedPassword);
					this.ShowMessage("??????????????????", true, "", 1);
				}
				else
				{
					this.ShowMessage("??????????????????", true, "", 1);
				}
			}
		}

		protected void btnChangePassword_Click(object sender, EventArgs e)
		{
			MemberInfo user = HiContext.Current.User;
			if (user.PasswordSalt.ToLower() != "open" && user.Password != Users.EncodePassword(this.txtOdlPasswordChange.Text, user.PasswordSalt))
			{
				this.ShowMessage("??????????????????????????????", false, "", 1);
			}
			else if (this.txtNewPassword2Change.Text.Length < 6 || this.txtNewPassword2Change.Text.Length > 20)
			{
				this.ShowMessage("???????????????6-20??????????????????", false, "", 1);
			}
			else if (MemberProcessor.ChangePassword(HiContext.Current.User, this.txtNewPassword2Change.Text))
			{
				Messenger.UserPasswordChanged(HiContext.Current.User, this.txtNewPassword2Change.Text);
				this.ShowMessage("???????????????????????????????????????", true, "", 1);
			}
		}
	}
}
