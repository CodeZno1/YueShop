using System;

namespace HiShop.API.Setting.Exceptions
{
	public class WeixinMenuException : WeixinException
	{
		public WeixinMenuException(string message)
			: base(message, null)
		{
		}

		public WeixinMenuException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
