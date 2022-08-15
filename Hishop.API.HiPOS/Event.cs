namespace HiShop.API.HiPOS
{
	public enum Event
	{
		ENTER,
		LOCATION,
		subscribe,
		unsubscribe,
		CLICK,
		scan,
		VIEW,
		MASSSENDJOBFINISH,
		TEMPLATESENDJOBFINISH,
		scancode_push,
		scancode_waitmsg,
		pic_sysphoto,
		pic_photo_or_album,
		pic_weixin,
		location_select,
		card_pass_check,
		card_not_pass_check,
		user_get_card,
		user_del_card,
		kf_create_session,
		kf_close_session,
		kf_switch_session,
		poi_check_notify,
		WifiConnected,
		user_consume_card,
		user_view_card,
		user_enter_session_from_card
	}
}