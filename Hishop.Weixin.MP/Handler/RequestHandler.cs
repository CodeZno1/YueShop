using Hishop.Weixin.MP.Request;
using Hishop.Weixin.MP.Request.Event;
using Hishop.Weixin.MP.Util;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Hishop.Weixin.MP.Handler
{
	public abstract class RequestHandler
	{
		public XDocument RequestDocument
		{
			get;
			set;
		}

		public string ResponseDocument
		{
			get
			{
				if (this.ResponseMessage == null)
				{
					return string.Empty;
				}
				return EntityHelper.ConvertEntityToXml(this.ResponseMessage).ToString();
			}
		}

		public AbstractRequest RequestMessage
		{
			get;
			set;
		}

		public AbstractResponse ResponseMessage
		{
			get;
			set;
		}

		public RequestHandler(Stream inputStream)
		{
			using (XmlReader reader = XmlReader.Create(inputStream))
			{
				this.RequestDocument = XDocument.Load(reader);
				this.Init(this.RequestDocument);
			}
		}

		public RequestHandler(string xml)
		{
			using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
			{
				this.RequestDocument = XDocument.Load(reader);
				this.Init(this.RequestDocument);
			}
		}

		private void Init(XDocument requestDocument)
		{
			this.RequestDocument = requestDocument;
			this.RequestMessage = RequestMessageFactory.GetRequestEntity(this.RequestDocument);
		}

		public void Execute()
		{
			if (this.RequestMessage != null)
			{
				switch (this.RequestMessage.MsgType)
				{
				case RequestMsgType.Text:
					this.ResponseMessage = this.OnTextRequest(this.RequestMessage as TextRequest);
					break;
				case RequestMsgType.Image:
					this.ResponseMessage = this.OnImageRequest(this.RequestMessage as ImageRequest);
					break;
				case RequestMsgType.Voice:
					this.ResponseMessage = this.OnVoiceRequest(this.RequestMessage as VoiceRequest);
					break;
				case RequestMsgType.Video:
					this.ResponseMessage = this.OnVideoRequest(this.RequestMessage as VideoRequest);
					break;
				case RequestMsgType.Location:
					this.ResponseMessage = this.OnLocationRequest(this.RequestMessage as LocationRequest);
					break;
				case RequestMsgType.Link:
					this.ResponseMessage = this.OnLinkRequest(this.RequestMessage as LinkRequest);
					break;
				case RequestMsgType.Event:
					this.ResponseMessage = this.OnEventRequest(this.RequestMessage as EventRequest);
					break;
				default:
					throw new WeixinException("?????????MsgType????????????");
				}
			}
		}

		public abstract AbstractResponse DefaultResponse(AbstractRequest requestMessage);

		public virtual AbstractResponse OnTextRequest(TextRequest textRequest)
		{
			return this.DefaultResponse(textRequest);
		}

		public virtual AbstractResponse OnImageRequest(ImageRequest imageRequest)
		{
			return this.DefaultResponse(imageRequest);
		}

		public virtual AbstractResponse OnVoiceRequest(VoiceRequest voiceRequest)
		{
			return this.DefaultResponse(voiceRequest);
		}

		public virtual AbstractResponse OnVideoRequest(VideoRequest videoRequest)
		{
			return this.DefaultResponse(videoRequest);
		}

		public virtual AbstractResponse OnLocationRequest(LocationRequest locationRequest)
		{
			return this.DefaultResponse(locationRequest);
		}

		public virtual AbstractResponse OnLinkRequest(LinkRequest linkRequest)
		{
			return this.DefaultResponse(linkRequest);
		}

		public AbstractResponse OnEventRequest(EventRequest eventRequest)
		{
			AbstractResponse result = null;
			switch (eventRequest.Event)
			{
			case RequestEventType.Subscribe:
				result = this.OnEvent_SubscribeRequest(eventRequest as SubscribeEventRequest);
				break;
			case RequestEventType.UnSubscribe:
				result = this.OnEvent_UnSubscribeRequest(eventRequest as UnSubscribeEventRequest);
				break;
			case RequestEventType.Scan:
				this.ResponseMessage = this.OnEvent_ScanRequest(eventRequest as ScanEventRequest);
				break;
			case RequestEventType.Location:
				result = this.OnEvent_LocationRequest(eventRequest as LocationEventRequest);
				break;
			case RequestEventType.Click:
				result = this.OnEvent_ClickRequest(eventRequest as ClickEventRequest);
				break;
			default:
				throw new WeixinException("?????????Event??????????????????");
			}
			return result;
		}

		public virtual AbstractResponse OnEvent_ClickRequest(ClickEventRequest clickEventRequest)
		{
			return this.DefaultResponse(clickEventRequest);
		}

		public virtual AbstractResponse OnEvent_LocationRequest(LocationEventRequest locationEventRequest)
		{
			return this.DefaultResponse(locationEventRequest);
		}

		public virtual AbstractResponse OnEvent_ScanRequest(ScanEventRequest scanEventRequest)
		{
			return this.DefaultResponse(scanEventRequest);
		}

		public virtual AbstractResponse OnEvent_UnSubscribeRequest(UnSubscribeEventRequest unSubscribeEventRequest)
		{
			return this.DefaultResponse(unSubscribeEventRequest);
		}

		public virtual AbstractResponse OnEvent_SubscribeRequest(SubscribeEventRequest subscribeEventRequest)
		{
			return this.DefaultResponse(subscribeEventRequest);
		}
	}
}
