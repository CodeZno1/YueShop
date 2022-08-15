using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hishop.Weixin.MP;
using Hishop.Weixin.MP.Request.Event;
using Hidistro.Entities.VShop;
using Hishop.Weixin.MP.Response;
using Hishop.Weixin.MP.Domain;
using System.IO;
using Hishop.Weixin.MP.Request;
using Hidistro.SaleSystem.Store;
using System.Web.Security;
using System.Text.RegularExpressions;
using Hidistro.Entities.Store;
using Hidistro.Core;
using Hidistro.Entities.Members;
using Hidistro.SaleSystem.Vshop;
using Hidistro.Context;
using System.Data;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.Context;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP;
using System.Xml.Linq;
using Senparc.Weixin.MP.Helpers;
using Hidistro.SaleSystem.Depot;
using System.Text;

using System.Collections.Specialized;
using Hidistro.Entities;
using Hidistro.SaleSystem.Statistics;
using System.Xml;
using Hidistro.SaleSystem.Members;
using Hidistro.SaleSystem.Member;

namespace Hidistro.UI.Web.API
{
    /// <summary>
    /// 微信推送消息接受页
    /// </summary>
    public class wx : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            NameValueCollection Page_Param = new NameValueCollection() { context.Request.Form, context.Request.QueryString };
            System.Web.HttpRequest Request = context.Request;

            //string token = "weixin_test";
            string token = SettingsManager.GetMasterSettings().WeixinToken;
            string signature = Request["signature"];
            string nonce = Request["nonce"];
            string timestamp = Request["timestamp"];
            string echostr = Request["echostr"];
            
            if (Request.HttpMethod == "GET")
            {
                //get method - 仅在微信后台填写URL验证时触发
                if (Hishop.Weixin.MP.Util.CheckSignature.Check(signature, timestamp, nonce, token))
                    context.Response.Write(echostr);
                else
                    context.Response.Write("");
                context.Response.End();
            }
            else
            {
                try
                {

                    //使用新的微信第三方接口 
                    XDocument wxDocumentTarget = null;
                    if (NewWXHandler(Request, out wxDocumentTarget))
                    {
                       // Globals.AppendLog(wxDocumentTarget.ToNullString(), "1", "", "WX");
                        SaveMenuClick(wxDocumentTarget);
                        return;
                    }
                    else
                    {
                        SaveMenuClick(wxDocumentTarget);
                       // Globals.AppendLog(wxDocumentTarget.ToNullString(), "2", "", "WX");
                    }
                    var handler = new CustomMsgHandler(wxDocumentTarget.ToString());
                    handler.Execute();
                    context.Response.Write(handler.ResponseDocument);
                }
                catch (Exception ex)
                {
                   // Globals.WriteExceptionLog_Page(ex, Page_Param, "WXAPI");
                }
            }

        }
        /// <summary>
        /// 保存菜单点击信息
        /// </summary>
        /// <param name="xml"></param>
        private void SaveMenuClick(XDocument doc)
        {
            if (doc != null)
            {

                //{<FromUserName><![CDATA[owkp7s03-lzQ8Om8L7HHV_5v1KvA]]></FromUserName>
                //<CreateTime>1480509437</CreateTime>
                //<MsgType><![CDATA[event]]></MsgType>
                //<Event><![CDATA[CLICK]]></Event>
                //<EventKey><![CDATA[14]]></EventKey>
                XElement el = doc.Root.Element("MsgType");
                if (el != null)
                {
                    string fromusername = "", eventkey = "", eventstr = "", createtime = "";
                    Int32 menuId = 0;
                    string msgtype = el.Value;
                    if (msgtype == "event")
                    {
                        
                        el = doc.Root.Element("Event");
                        if (el != null)
                            eventstr = el.Value;
                        string tempEvent = eventstr.ToNullString().ToLower();
                        if (tempEvent == "click" || tempEvent == "view" || tempEvent == "link")
                        {
                            el = doc.Root.Element("FromUserName");
                            if (el != null)
                                fromusername = el.Value;
                            el = doc.Root.Element("CreateTime");
                            if (el != null)
                                createtime = el.Value;

                            el = doc.Root.Element("EventKey");
                            if (el != null)
                                eventkey = el.Value;
                            el = doc.Root.Element("MenuId");
                            if (el != null)
                                menuId = el.Value.ToInt();
                            IDictionary<string, string> menuParam = new Dictionary<string, string>();
                            menuParam.Add("CreateTime", createtime.ToNullString());
                            menuParam.Add("Event", eventstr.ToNullString());
                            menuParam.Add("EventKey", eventkey.ToNullString());
                            menuParam.Add("FromUserName", fromusername.ToNullString());
                            menuParam.Add("MsgType", msgtype.ToNullString());
                            menuParam.Add("menuId", menuId.ToNullString());
                            int id = eventkey.ToInt();
                            if (id == 0)
                            {
                                id = menuId;
                            }
                            try
                            {

                                //更新点击次数，
                                WXFansHelper.UpdateClickStatistical(fromusername, id);
                               // Globals.AppendLog(menuParam, "微信菜单点击", "", "", "WXMenuClick");
                            }
                            catch (Exception ex)
                            {
                                Globals.WriteExceptionLog(ex, menuParam, "WXMenuClick_Ex");
                            }

                        }
                    }

                }
            }
        }

        private bool NewWXHandler(System.Web.HttpRequest Request, out XDocument wxDocumentTarget)
        {
            NameValueCollection HandlerParam = new NameValueCollection() { Request.Form, Request.QueryString };
            bool bolFlag = false;
            wxDocumentTarget = null;
            try
            {
                wxDocumentTarget = RequestMessageFactory.GetRequestEntityDocument(Request.InputStream);
                RequestMessageBase requestMessage = null;
                Senparc.Weixin.MP.RequestMsgType msgType;
                msgType = MsgTypeHelper.GetRequestMsgType(wxDocumentTarget);

                switch (msgType)
                {
                    case Senparc.Weixin.MP.RequestMsgType.Event:
                        //判断Event类型
                        switch (wxDocumentTarget.Root.Element("Event").Value.ToUpper())
                        {
                            case "POI_CHECK_NOTIFY"://审核结果事件推送
                                requestMessage = new RequestMessageEvent_Poi_Check_Notify();
                                bolFlag = true;
                                break;
                            default://其他意外类型（也可以选择抛出异常）
                                break;
                        }
                        break;
                    default:
                        break;
                }
                EntityHelper.FillEntityWithXml(requestMessage, wxDocumentTarget);
                if (requestMessage != null)
                {
                    IDictionary<string, string> menuParam = new Dictionary<string, string>();
                    menuParam.Add("msgType", msgType.ToNullString());
                    menuParam.Add("wxDocumentTarget", wxDocumentTarget.ToNullString());
                    menuParam.Add("MsgId", requestMessage.MsgId.ToNullString());
                    menuParam.Add("MsgType", requestMessage.MsgType.ToNullString());
                    menuParam.Add("FromUserName", requestMessage.FromUserName.ToNullString());
                    menuParam.Add("ToUserName", requestMessage.ToUserName.ToNullString());

                    Globals.AppendLog(menuParam, "微信菜单点击", "", "", "NewWXHandler");
                }
                switch (msgType)
                {
                    case Senparc.Weixin.MP.RequestMsgType.Event:
                        //判断Event类型
                        switch (wxDocumentTarget.Root.Element("Event").Value.ToUpper())
                        {
                            case "POI_CHECK_NOTIFY"://审核结果事件推送
                                StoresHelper.UpdateStoreFromWX(requestMessage);
                                break;
                            default://其他意外类型（也可以选择抛出异常）

                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (ArgumentException ex)
            {
                Globals.WriteExceptionLog_Page(ex, HandlerParam, "WXAPI");
            }
            return bolFlag;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

    public class CustomMsgHandler : Hishop.Weixin.MP.Handler.RequestHandler
    {
        public CustomMsgHandler(string xml)
            : base(xml)
        {

        }
        /// <summary>
        /// 判断是否打开了多客服
        /// </summary>
        /// <returns><
        public bool IsOpenManyService()
        {
            SiteSettings siteSettings = SettingsManager.GetMasterSettings();
            return siteSettings.OpenManyService;
        }
        /// <summary>
        /// 转向多客服
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public AbstractResponse GotoManyCustomerService(AbstractRequest requestMessage)
        {
            if (!IsOpenManyService()) return null;
            AbstractResponse response = new AbstractResponse();
            response.FromUserName = requestMessage.ToUserName;
            response.ToUserName = requestMessage.FromUserName;
            response.MsgType = Hishop.Weixin.MP.ResponseMsgType.transfer_customer_service;
            return response;
        }

        public override AbstractResponse DefaultResponse(AbstractRequest requestMessage)
        {
            IDictionary<string, string> menuParam = new Dictionary<string, string>();
            menuParam.Add("CreateTime", requestMessage.CreateTime.ToNullString());
            menuParam.Add("FromUserName", requestMessage.FromUserName.ToNullString());
            menuParam.Add("MsgId", requestMessage.MsgId.ToNullString());
            menuParam.Add("MsgType", requestMessage.MsgType.ToNullString());

            ReplyInfo reply = ReplyHelper.GetMismatchReply();
            if (reply == null || IsOpenManyService())//如果未设置无匹配回复或者已开启多客服则转向多客服
                return GotoManyCustomerService(requestMessage);
            AbstractResponse response = GetResponse(reply, requestMessage.FromUserName);
            //如果获取回复响应为null则转向多客服
            if (response == null)
                return GotoManyCustomerService(requestMessage);
            response.ToUserName = requestMessage.FromUserName;
            response.FromUserName = requestMessage.ToUserName;

            return response;

        }
        public new AbstractResponse OnLinkRequest(LinkRequest request)
        {
            return null;
        }
        public override AbstractResponse OnTextRequest(TextRequest textRequest)
        {
            #region 关键字优先

            AbstractResponse resp = GetKeyResponse(textRequest.Content, textRequest);
            if (resp != null)
                return resp;
            #endregion

            #region 关键字回复

            IList<ReplyInfo> list = ReplyHelper.GetReplies(ReplyType.Keys);
            if (list == null || list.Count == 0 && IsOpenManyService())//关键字匹配失败且开启了多客服时转向多客服
                return GotoManyCustomerService(textRequest);

            foreach (var reply in list)
            {
                if (reply.MatchType == MatchType.Equal && reply.Keys == textRequest.Content)
                {
                    AbstractResponse response = GetResponse(reply, textRequest.FromUserName);
                    response.ToUserName = textRequest.FromUserName;
                    response.FromUserName = textRequest.ToUserName;
                    return response;
                }

                if (reply.MatchType == MatchType.Like && reply.Keys.Contains(textRequest.Content))
                {
                    AbstractResponse response = GetResponse(reply, textRequest.FromUserName);
                    response.ToUserName = textRequest.FromUserName;
                    response.FromUserName = textRequest.ToUserName;
                    return response;
                }
            }
            #endregion

            return DefaultResponse(textRequest);
        }
        /// <summary>
        /// 取消关注时事件
        /// </summary>
        /// <param name="unSubscribeEventRequest"></param>
        /// <returns></returns>
        public override AbstractResponse OnEvent_UnSubscribeRequest(UnSubscribeEventRequest unSubscribeEventRequest)
        {
            string openId = unSubscribeEventRequest.FromUserName;
            try
            {
                //更新会员是否已关注字段为已关注
                MemberProcessor.UpdateWXUserIsSubscribeStatus(openId, false);
            }
            catch (Exception ex)
            {
                IDictionary<string, string> subParam = new Dictionary<string, string>();
                subParam.Add("openId", openId);
                Globals.WriteExceptionLog(ex, subParam, "UpdateWXUserIsSubscribeStatus");
            }
            return base.OnEvent_UnSubscribeRequest(unSubscribeEventRequest);
        }
        /// <summary>
        /// 关注时回复
        /// </summary>
        /// <param name="subscribeEventRequest"></param>
        /// <returns></returns>
        public override AbstractResponse OnEvent_SubscribeRequest(SubscribeEventRequest subscribeEventRequest)
        {
            string openId = subscribeEventRequest.FromUserName;
            try
            {
                //更新会员是否已关注字段为已关注
                MemberProcessor.UpdateWXUserIsSubscribeStatus(openId, true);
            }catch(Exception ex)
            {
                IDictionary<string, string> subParam = new Dictionary<string, string>();
                subParam.Add("openId", openId);
                Globals.WriteExceptionLog(ex, subParam, "UpdateWXUserIsSubscribeStatus");
            }
            ReplyInfo reply = ReplyHelper.GetSubscribeReply();

            if (reply == null)
            {
                reply = new ReplyInfo();
            }
            reply.Keys = "登录";

            //处理红包领取提醒
            if (WeiXinRedEnvelopeProcessor.CheckRedEnvelopeGetRecordNoAttentionIsExist(subscribeEventRequest.FromUserName))
            {
                reply.Keys = "红包";
            }

            var eventRequest = subscribeEventRequest as SubscribeEventRequest;
            if (eventRequest != null)
            {
                if (!string.IsNullOrEmpty(eventRequest.EventKey) && eventRequest.EventKey.Split('_').Length == 2)
                {
                    string id = eventRequest.EventKey.Split('_')[1];
                   
                    if (id == "referralregister")
                    {
                        reply.Keys = "分销注册";
                    }
                    else if (id.Contains("referraluserid"))
                    {
                        //上级分销员
                        if (id.Split(':').Length == 2)
                        {
                            //在微信关注时记录下分销记录
                            int referralUserId = id.Split(':')[1].ToInt();
                            MemberWXReferralInfo memberWXReferralInfo = VShopHelper.GetWXReferral(subscribeEventRequest.FromUserName);
                            if (memberWXReferralInfo != null && memberWXReferralInfo.Id > 0)
                                VShopHelper.UpdateWXReferral(subscribeEventRequest.FromUserName, referralUserId);//修改记录分销记录
                            else
                                VShopHelper.AddWXReferral(subscribeEventRequest.FromUserName, referralUserId);//记录分销记录
                        }
                    }
                    else if (id.Contains("shoppingguiderid"))
                    {
                        //导购ID
                        if (id.Split(':').Length == 2)
                        {
                            //在微信关注时记录下导购ID
                            int shoppingGuiderId = id.Split(':')[1].ToInt();
                            MemberWXShoppingGuiderInfo memberWXShoppingGuiderInfo = MemberHelper.GetMemberWXShoppingGuider(subscribeEventRequest.FromUserName);
                            if (memberWXShoppingGuiderInfo != null && memberWXShoppingGuiderInfo.Id > 0)
                                MemberHelper.UpdateWXShoppingGuider(subscribeEventRequest.FromUserName, shoppingGuiderId);//修改记录分销记录
                            else
                                MemberHelper.AddWXShoppingGuider(subscribeEventRequest.FromUserName, shoppingGuiderId);//记录分销记录
                        }

                    }
                    else
                    {
                        Int32 redEnvelopeId = id.ToInt();
                        if (redEnvelopeId > 0)
                        {
                            RedEnvelopeSendRecord redEnvelopeSendRecord = WeiXinRedEnvelopeProcessor.GetRedEnvelopeSendRecordById(redEnvelopeId);
                            if (redEnvelopeSendRecord != null)
                            {
                                if (WeiXinRedEnvelopeProcessor.IsGetInToday(subscribeEventRequest.FromUserName, redEnvelopeSendRecord.SendCode, true))
                                {
                                    reply.Keys = "今日已领红包";
                                }
                            }
                        }
                    }
                }

            }

            AbstractResponse response = GetResponse(reply, subscribeEventRequest.FromUserName, subscribeEventRequest);

            if (response == null)
            {
                return GotoManyCustomerService(subscribeEventRequest);
            }

            response.ToUserName = subscribeEventRequest.FromUserName;
            response.FromUserName = subscribeEventRequest.ToUserName;

            return response;
        }
        /// <summary>
        /// 微信扫码记录
        /// </summary>
        /// <param name="scanEventRequest"></param>
        /// <returns></returns>
        public override AbstractResponse OnEvent_ScanRequest(ScanEventRequest scanEventRequest)
        {
            ReplyInfo reply = ReplyHelper.GetSubscribeReply();
            string openId = scanEventRequest.FromUserName;
            try
            {
                //更新会员是否已关注字段为已关注
                MemberProcessor.UpdateWXUserIsSubscribeStatus(openId, true);
            }
            catch (Exception ex)
            {
                IDictionary<string, string> subParam = new Dictionary<string, string>();
                subParam.Add("openId", openId);
                Globals.WriteExceptionLog(ex, subParam, "UpdateWXUserIsSubscribeStatus");
            }
            if (reply == null)
            {
                reply = new ReplyInfo();
            }
            reply.Keys = "登录";

            var eventRequest = scanEventRequest as ScanEventRequest;
            if (eventRequest != null)
            {
                if (!string.IsNullOrEmpty(eventRequest.EventKey) && eventRequest.EventKey.Split('_').Length >= 2)
                {
                    string id = eventRequest.EventKey.Split('_')[1];

                    if (id == "referralregister")
                    {
                        reply.Keys = "分销注册";
                    }
                    else if (id.Contains("referraluserid"))
                    {
                        //上级分销员
                        if (id.Split(':').Length == 2)
                        {
                            //在微信关注时记录下分销记录
                            int referralUserId = id.Split(':')[1].ToInt();
                            MemberWXReferralInfo memberWXReferralInfo = VShopHelper.GetWXReferral(scanEventRequest.FromUserName);
                            if (memberWXReferralInfo != null && memberWXReferralInfo.Id > 0)
                                VShopHelper.UpdateWXReferral(scanEventRequest.FromUserName, referralUserId);//修改记录分销记录
                            else
                                VShopHelper.AddWXReferral(scanEventRequest.FromUserName, referralUserId);//记录分销记录
                        }
                    }
                    else if (id.Contains("shoppingguiderid"))
                    {
                        //导购ID
                        if (id.Split(':').Length == 2)
                        {
                            //在微信关注时记录下导购ID
                            int shoppingGuiderId = id.Split(':')[1].ToInt();
                            MemberWXShoppingGuiderInfo memberWXShoppingGuiderInfo = MemberHelper.GetMemberWXShoppingGuider(scanEventRequest.FromUserName);
                            if (memberWXShoppingGuiderInfo != null && memberWXShoppingGuiderInfo.Id > 0)
                                MemberHelper.UpdateWXShoppingGuider(scanEventRequest.FromUserName, shoppingGuiderId);//修改记录分销记录
                            else
                                MemberHelper.AddWXShoppingGuider(scanEventRequest.FromUserName, shoppingGuiderId);//记录分销记录
                        }

                    }
                }

            }

            AbstractResponse response = GetResponse(reply, scanEventRequest.FromUserName, scanEventRequest);

            if (response == null)
            {
                return GotoManyCustomerService(scanEventRequest);
            }

            response.ToUserName = scanEventRequest.FromUserName;
            response.FromUserName = scanEventRequest.ToUserName;

            return response;
        }
        /// <summary>
        /// 菜单点击事件
        /// </summary>
        /// <param name="clickEventRequest"></param>
        /// <returns></returns>
        public override AbstractResponse OnEvent_ClickRequest(ClickEventRequest clickEventRequest)
        {
            int id = clickEventRequest.EventKey.ToInt();
            MenuInfo menu = null;
            if (id > 0)
                menu = VShopHelper.GetMenu(id);
            if (menu == null)
                return null;


            ReplyInfo reply = ReplyHelper.GetReply(menu.ReplyId);
            if (reply == null)
                return null;

            #region 关键字优先

            AbstractResponse resp = GetKeyResponse(reply.Keys, clickEventRequest);
            if (resp != null)
                return resp;
            #endregion

            AbstractResponse response = GetResponse(reply, clickEventRequest.FromUserName);
            if (response == null)
                return GotoManyCustomerService(clickEventRequest);
            response.ToUserName = clickEventRequest.FromUserName;
            response.FromUserName = clickEventRequest.ToUserName;

            return response;
        }
        /// <summary>
        /// 定位
        /// </summary>
        /// <param name="locationRequest"></param>
        /// <returns></returns>
        public override AbstractResponse OnLocationRequest(LocationRequest locationRequest)
        {
            return base.OnLocationRequest(locationRequest);
        }

        public override AbstractResponse OnEvent_LocationRequest(LocationEventRequest locationEventRequest)
        {
            return base.OnEvent_LocationRequest(locationEventRequest);
        }


        private AbstractResponse GetKeyResponse(string key, AbstractRequest request)
        {


            #region 投票回复

            IList<ReplyInfo> tplist = ReplyHelper.GetReplies(ReplyType.Vote);
            if (tplist != null && tplist.Count > 0)
            {
                foreach (var reply in tplist)
                {
                    if (reply.Keys == key)
                    {
                        VoteInfo act = StoreHelper.GetVoteById(reply.ActivityId);
                        if (act == null || !act.IsBackup)
                            continue;

                        NewsResponse response = new NewsResponse();
                        response.CreateTime = DateTime.Now;
                        response.FromUserName = request.ToUserName;
                        response.ToUserName = request.FromUserName;
                        response.Articles = new List<Hishop.Weixin.MP.Domain.Article>();

                        response.Articles.Add(new Hishop.Weixin.MP.Domain.Article()
                        {
                            Description = act.VoteName,
                            PicUrl = String.Format("http://{0}{1}"
                                , HttpContext.Current.Request.Url.Host
                                , act.ImageUrl),
                            Title = act.VoteName,
                            Url = String.Format("http://{0}/vshop/Vote.aspx?voteId={1}"
                                , HttpContext.Current.Request.Url.Host
                                , act.VoteId)
                        });

                        return response;
                    }
                }
            }

            #endregion

            #region 大转盘回复
            IList<ReplyInfo> cjlist = ReplyHelper.GetReplies(ReplyType.Wheel);
            if (cjlist != null && cjlist.Count > 0)
            {
                foreach (var reply in cjlist)
                {
                    if (reply.Keys == key)
                    {
                        LotteryActivityInfo act = VShopHelper.GetLotteryActivityInfo(reply.ActivityId);
                        if (act == null)
                            continue;

                        NewsResponse response = new NewsResponse();
                        response.CreateTime = DateTime.Now;
                        response.FromUserName = request.ToUserName;
                        response.ToUserName = request.FromUserName;
                        response.Articles = new List<Hishop.Weixin.MP.Domain.Article>();

                        response.Articles.Add(new Hishop.Weixin.MP.Domain.Article()
                        {
                            Description = act.ActivityDesc,
                            PicUrl = String.Format("http://{0}{1}"
                                , HttpContext.Current.Request.Url.Host
                                , act.ActivityPic),
                            Title = act.ActivityName,
                            Url = String.Format("http://{0}/vshop/BigWheel.aspx?activityId={1}"
                                , HttpContext.Current.Request.Url.Host
                                , act.ActivityId)
                        });

                        return response;
                    }
                }
            }
            #endregion

            #region 刮刮卡回复
            IList<ReplyInfo> ggklist = ReplyHelper.GetReplies(ReplyType.Scratch);
            if (ggklist != null && ggklist.Count > 0)
            {
                foreach (var reply in ggklist)
                {
                    if (reply.Keys == key)
                    {
                        LotteryActivityInfo act = VShopHelper.GetLotteryActivityInfo(reply.ActivityId);
                        if (act == null)
                            continue;

                        NewsResponse response = new NewsResponse();
                        response.CreateTime = DateTime.Now;
                        response.FromUserName = request.ToUserName;
                        response.ToUserName = request.FromUserName;
                        response.Articles = new List<Hishop.Weixin.MP.Domain.Article>();

                        response.Articles.Add(new Hishop.Weixin.MP.Domain.Article()
                        {
                            Description = act.ActivityDesc,
                            PicUrl = String.Format("http://{0}{1}"
                                , HttpContext.Current.Request.Url.Host
                                , act.ActivityPic),
                            Title = act.ActivityName,
                            Url = String.Format("http://{0}/vshop/Scratch.aspx?activityId={1}"
                                , HttpContext.Current.Request.Url.Host
                                , act.ActivityId)
                        });

                        return response;
                    }
                }
            }
            #endregion

            #region 砸金蛋回复
            IList<ReplyInfo> zjdlist = ReplyHelper.GetReplies(ReplyType.SmashEgg);
            if (zjdlist != null && zjdlist.Count > 0)
            {
                foreach (var reply in zjdlist)
                {
                    if (reply.Keys == key)
                    {
                        LotteryActivityInfo act = VShopHelper.GetLotteryActivityInfo(reply.ActivityId);
                        if (act == null)
                            continue;

                        NewsResponse response = new NewsResponse();
                        response.CreateTime = DateTime.Now;
                        response.FromUserName = request.ToUserName;
                        response.ToUserName = request.FromUserName;
                        response.Articles = new List<Hishop.Weixin.MP.Domain.Article>();

                        response.Articles.Add(new Hishop.Weixin.MP.Domain.Article()
                        {
                            Description = act.ActivityDesc,
                            PicUrl = String.Format("http://{0}{1}"
                                , HttpContext.Current.Request.Url.Host
                                , act.ActivityPic),
                            Title = act.ActivityName,
                            Url = String.Format("http://{0}/vshop/SmashEgg.aspx?activityId={1}"
                                , HttpContext.Current.Request.Url.Host
                                , act.ActivityId)
                        });

                        return response;
                    }
                }
            }
            #endregion

            #region 微报名活动回复
            IList<ReplyInfo> actlist = ReplyHelper.GetReplies(ReplyType.SignUp);
            if (actlist != null && actlist.Count > 0)
            {
                foreach (var reply in actlist)
                {
                    if (reply.Keys == key)
                    {
                        VActivityInfo act = VShopHelper.GetActivity(reply.ActivityId);
                        if (act == null)
                            continue;

                        NewsResponse response = new NewsResponse();
                        response.CreateTime = DateTime.Now;
                        response.FromUserName = request.ToUserName;
                        response.ToUserName = request.FromUserName;
                        response.Articles = new List<Hishop.Weixin.MP.Domain.Article>();

                        response.Articles.Add(new Hishop.Weixin.MP.Domain.Article()
                        {
                            Description = act.Description,
                            PicUrl = String.Format("http://{0}{1}"
                                , HttpContext.Current.Request.Url.Host
                                , act.PicUrl),
                            Title = act.Name,
                            Url = String.Format("http://{0}/vshop/Activity.aspx?id={1}"
                                , HttpContext.Current.Request.Url.Host
                                , act.ActivityId)
                        });

                        return response;
                    }
                }
            }

            #endregion

            #region 微抽奖回复
            IList<ReplyInfo> signlist = ReplyHelper.GetReplies(ReplyType.Ticket);
            if (signlist != null && signlist.Count > 0)
            {
                foreach (var reply in signlist)
                {
                    if (reply.Keys == key)
                    {

                        LotteryTicketInfo act = VShopHelper.GetLotteryTicket(reply.ActivityId);
                        if (act == null)
                            continue;

                        NewsResponse response = new NewsResponse();
                        response.CreateTime = DateTime.Now;
                        response.FromUserName = request.ToUserName;
                        response.ToUserName = request.FromUserName;
                        response.Articles = new List<Hishop.Weixin.MP.Domain.Article>();

                        response.Articles.Add(new Hishop.Weixin.MP.Domain.Article()
                        {
                            Description = act.ActivityDesc,
                            PicUrl = String.Format("http://{0}{1}"
                                , HttpContext.Current.Request.Url.Host
                                , act.ActivityPic),
                            Title = act.ActivityName,
                            Url = String.Format("http://{0}/vshop/SignUp.aspx?id={1}"
                                , HttpContext.Current.Request.Url.Host
                                , act.ActivityId)
                        });

                        return response;
                    }
                }
            }

            #endregion


            return null;
        }

        /// <summary>
        /// 获取对应的响应输出
        /// </summary>
        public AbstractResponse GetResponse(ReplyInfo reply, string openId, AbstractRequest bstractRequest = null)
        {
            if (reply.MessageType == MessageType.Text)
            {
                var textreply = reply as TextReplyInfo;
                TextResponse textResp = new TextResponse();
                textResp.CreateTime = DateTime.Now;
                if (textreply != null && !string.IsNullOrEmpty(textreply.Text))
                {
                    textResp.Content = textreply.Text;
                }
                
                if (reply.Keys == "登录")
                {
                    string url = String.Format("http://{0}/Vshop/MemberCenter.aspx?SessionId={1}", HttpContext.Current.Request.Url.Host, openId);

                    textResp.Content = textResp.Content.Replace("$login$", string.Format("<a href=\"{0}\">一键登录</a>", url));
                }

                if (reply.Keys == "红包")
                {
                    string id = ((SubscribeEventRequest)bstractRequest).EventKey.Split('_')[1];
                    RedEnvelopeSendRecord redEnvelopeSendRecord = WeiXinRedEnvelopeProcessor.GetRedEnvelopeSendRecordById(Convert.ToInt32(id));
                    if (redEnvelopeSendRecord != null)
                    {
                        string url = String.Format("http://{0}/Vshop/GetRedEnvelope?SendCode={1}&OrderId={2}", HttpContext.Current.Request.Url.Host, redEnvelopeSendRecord.SendCode, redEnvelopeSendRecord.OrderId);
                        textResp.Content = textResp.Content + string.Format("<a href=\"{0}\">立即领取红包</a>", url);
                    }
                }

                if (reply.Keys == "今日已领红包")
                {
                    string url = String.Format("http://{0}/Vshop/Default.aspx", HttpContext.Current.Request.Url.Host);
                    textResp.Content = textResp.Content + string.Format("<a href=\"{0}\">你今日已领取过了，立即购物</a>", url);
                }
                if (reply.Keys == "分销注册")
                {
                    string url = String.Format("http://{0}/Vshop/ReferralRegister.aspx?again=1", HttpContext.Current.Request.Url.Host);
                    textResp.Content = textResp.Content + string.Format("感谢您的关注。点击<a href=\"{0}\">注册</a>成为本商城分销员，赚取丰厚奖励！", url);
                }

                return textResp;
            }
            else
            {
                HttpContext context = HttpContext.Current;
                NewsResponse news = new NewsResponse();
                news.CreateTime = DateTime.Now;
                news.Articles = new List<Hishop.Weixin.MP.Domain.Article>();
                foreach (var item in (reply as NewsReplyInfo).NewsMsg)
                {
                    var art = new Hishop.Weixin.MP.Domain.Article()
                    {
                        Description = item.Description,
                        PicUrl = String.Format("{0}://{1}{2}", Globals.GetProtocal(context), HttpContext.Current.Request.Url.Host, item.PicUrl),
                        Title = item.Title,
                        Url = String.IsNullOrEmpty(item.Url)
                            ? String.Format("{0}://{1}/Vshop/ImageTextDetails.aspx?messageId={2}", Globals.GetProtocal(context), HttpContext.Current.Request.Url.Host, item.Id)
                            : item.Url
                    };
                    news.Articles.Add(art);
                }

                return news;
            }
        }
    }



}