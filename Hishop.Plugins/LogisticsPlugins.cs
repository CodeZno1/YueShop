using System;
using System.IO;
using System.Web;

namespace Hishop.Plugins
{
	public class LogisticsPlugins : PluginContainer
	{
		private static readonly object LockHelper = new object();

		private static volatile LogisticsPlugins instance = null;

		protected override string PluginLocalPath
		{
			get
			{
				if (HttpContext.Current != null)
				{
					return HttpContext.Current.Request.MapPath("~/plugins/logistics");
				}
				string text = "plugins/logistics";
				text = text.Replace("/", "\\");
				if (text.StartsWith("\\"))
				{
					text = text.TrimStart('\\');
				}
				return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, text);
			}
		}

		protected override string PluginVirtualPath
		{
			get
			{
				return Utils.ApplicationPath + "/plugins/logistics";
			}
		}

		protected override string IndexCacheKey
		{
			get
			{
				return "plugin-logistics-index";
			}
		}

		protected override string TypeCacheKey
		{
			get
			{
				return "plugin-logistics-type";
			}
		}

		private LogisticsPlugins()
		{
		}

		public static LogisticsPlugins Instance()
		{
			if (LogisticsPlugins.instance == null)
			{
				lock (LogisticsPlugins.LockHelper)
				{
					if (LogisticsPlugins.instance == null)
					{
						LogisticsPlugins.instance = new LogisticsPlugins();
					}
				}
			}
			LogisticsPlugins.instance.VerifyIndex();
			return LogisticsPlugins.instance;
		}

		public override PluginItemCollection GetPlugins()
		{
			throw new NotImplementedException();
		}

		public override PluginItem GetPluginItem(string fullName)
		{
			return null;
		}
	}
}
