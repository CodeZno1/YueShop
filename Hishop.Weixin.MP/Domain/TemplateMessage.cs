using System.Collections.Generic;

namespace Hishop.Weixin.MP.Domain
{
	public class TemplateMessage
	{
		public class MessagePart
		{
			public string Name
			{
				get;
				set;
			}

			public string Value
			{
				get;
				set;
			}

			public string Color
			{
				get;
				set;
			}

			public MessagePart()
			{
				this.Color = "#000099";
			}
		}

		public string Touser
		{
			get;
			set;
		}

		public string TemplateId
		{
			get;
			set;
		}

		public string Url
		{
			get;
			set;
		}

		public string Page
		{
			get;
			set;
		}

		public string FormId
		{
			get;
			set;
		}

		public string Topcolor
		{
			get;
			set;
		}

		public IEnumerable<MessagePart> Data
		{
			get;
			set;
		}

		public string EmphasisKeyword
		{
			get;
			set;
		}

		public TemplateMessage()
		{
			this.Topcolor = "#00FF00";
		}
	}
}
