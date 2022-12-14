using System.ComponentModel;

namespace Hidistro.Entities.Commodities
{
	public enum ApiErrorCode
	{
		[Description("成功")]
		Success = 100,
		[Description("操作失败")]
		Failed = 0,
		[Description("参数错误")]
		Paramter_Error = 101,
		[Description("参数格式错误")]
		Format_Eroor,
		[Description("签名错误")]
		Signature_Error,
		[Description("值不能为空")]
		Empty_Error,
		[Description("不存在值错误")]
		NoExists_Error,
		[Description("已存在错误")]
		Exists_Error,
		[Description("参数不一致")]
		Paramter_Diffrent,
		[Description("设置错误")]
		SettingsError = 107,
		[Description("值未定义")]
		ValueUndefined = 133,
		[Description("不是图片文件")]
		NotImageFile = 112,
		[Description("团购未结束")]
		Group_Error = 108,
		[Description("订单未付款或不是等待发货")]
		NoPay_Error,
		[Description("无配送方式")]
		NoShippingMode,
		[Description("运货单号错误")]
		ShipingOrderNumber_Error,
		[Description("订单号为空")]
		OrderNumber_Empty,
		[Description("错误的订单号")]
		OrderNumber_Error,
		[Description("订单状态不正确")]
		OrderStatus_Error,
		[Description("快递公司名称错误")]
		ExpressCode_Error,
		[Description("商品列表为空")]
		OrderList_Empty,
		[Description("退款金额错误")]
		RefundMoney_Error,
		[Description("订单不存在退款记录")]
		Order_NoExistRefund,
		[Description("订单或者商品不存在售后记录")]
		OrderOrProduct_NoExistReturn,
		[Description("退款原路返回错误")]
		Refund_BackReturnError,
		[Description("必需填写备注信息")]
		Must_WriteRemark,
		[Description("订单不是门店收款")]
		Order_NoStoreCollect,
		[Description("没有记录被更新成功")]
		NoRecord_Updated,
		[Description("订单不包含指定规格商品")]
		Order_NoSkuId,
		[Description("退款方式错误")]
		RefundType_Error,
		[Description("申请退货时失败")]
		Return_ApplyError,
		[Description("地区编号错误")]
		RegionId_Error,
		[Description("时间范围值未定义")]
		TimeRange_NoDefined,
		[Description("支付方式值未定义")]
		PayType_NoDefined,
		[Description("订单类型值未定义")]
		OrderType_NoDefined,
		[Description("请填写银行帐号信息")]
		Must_WriteBankInfo,
		[Description("错误的售后状态")]
		AfterSale_StatusError = 131,
		[Description("权限不足")]
		NoEnoughRight,
		[Description("平台未开启支付宝提现")]
		PlatNotOpenAlipayDraw = 134,
		[Description("请填写退款原因")]
		RefundReasonEmpty,
		[Description("退款数量错误")]
		RefundQuantityError,
		[Description("核销密码错误")]
		VerificationPasswordError,
		[Description("请选择取消发单原因")]
		CancelSendGoodsReasonEmpty,
		[Description("SessionId为空")]
		Session_Empty = 200,
		[Description("SessionId错误")]
		Session_Error,
		[Description("Session超时")]
		Session_TimeOut,
		[Description("用户名已存在")]
		Username_Exist,
		[Description("禁止注册")]
		Ban_Register,
		[Description("用户未开启预付款帐户")]
		User_NotOpenBlance,
		[Description("文件路径错误")]
		FilePath_Error,
		[Description("文件路径不存在")]
		FilePath_NoExist,
		[Description("用户不存在")]
		NotExistUser,
		[Description("不是该门店的子帐号")]
		SubAccountNotRelationStore,
		[Description("不是门店导购发展的会员")]
		UserNotRelationStoreOrSupShoppingGuider,
		[Description("用户名为空,或者不符合规范")]
		UserName_Error,
		[Description("密码为空,或者不符合规范")]
		Password_Error,
		[Description("两次密码输入不一致")]
		RePasswordNoEqualsPassword,
		[Description("用户未登录")]
		UserNoLogin,
		[Description("手机号码已经绑定了帐号")]
		MobbileIsBinding,
		[Description("信任登录帐号已经存在")]
		ExistTrustLogin,
		[Description("用户已绑定过信任登录帐号")]
		UserHasBindTrustLogin,
		[Description("用户还不是分销员")]
		UserIsNotReferral,
		[Description("用户消费金额不足")]
		UserConsumeNotEnough,
		[Description("用户消费金额不足")]
		UserNotOpenBalance,
		[Description("参数长度不符合规则")]
		Paramter_LongerOrShort = 99,
		[Description("商品已下架或者删除")]
		SaleState_Error = 300,
		[Description("库存不足")]
		Stock_Error,
		[Description("商品列表为空")]
		ProductList_Empty,
		[Description("门店ID错误")]
		StoreId_Error = 500,
		[Description("门店不存在")]
		Store_NoExist,
		[Description("管理员与门店不匹配")]
		NoStore_Admin,
		[Description("门店管理员用户名为空")]
		StoreAdminName_Empty,
		[Description("门店管理员密码为空")]
		StoreAdminPassword_Empty,
		[Description("门店管理员用户名或者密码错误")]
		StoreAdminNameOrPassword_Error,
		[Description("门店商品库存不足")]
		StoreStock_NotEnough,
		[Description("订单与门店不匹配")]
		NoStore_Order,
		[Description("错误的提货码")]
		TakeCode_Error,
		[Description("提货码已经使用")]
		TakeCode_AlreadyUsed,
		[Description("订单已被确认过")]
		StoreOrder_IsConfirmed,
		[Description("SessionId为空")]
		SessionId_Empty,
		[Description("登录超时或者已在其他端登录")]
		SessionId_NoRelationStore,
		[Description("商品在门店未上架")]
		Product_NoRelationStore,
		[Description("门店管理员原登录密码错误")]
		StoreOldPassword_InValid,
		[Description("消息类型错误")]
		MsgType_Error,
		[Description("列表为空")]
		List_Empty,
		[Description("无修改商品价格和库存的权限")]
		NotModifyProduct,
		[Description("门店价格超出设置范围")]
		StorePriceError,
		[Description("交易密码已设置过了")]
		TradePasswordAlreadySet,
		[Description("错误的SessionId")]
		SessionId_Error,
		[Description("交易密码错误")]
		TradePassword_Error,
		[Description("交易密码未设置")]
		TradePassword_NoSet,
		[Description("门店未绑定支付宝帐号信息")]
		StoreNotBindAlipayInfo,
		[Description("门店未绑定银行卡帐号信息")]
		StoreNotBindBankCardInfo,
		[Description("门店余额不足")]
		BalanceNotEnough,
		[Description("提现金额错误")]
		RequestAmountError,
		[Description("导购账号被冻结")]
		ShoppingGuiderIsFreezed,
		[Description("无上架商品的权限")]
		NotShelvesProduct = 531,
		[Description("未开启门店")]
		NoOpenMultStore,
		[Description("门店已被关闭")]
		StoreIsClosed,
		[Description("没有匹配的门店")]
		NonMatchStore,
		[Description("参数错误")]
		CouponParamter_Error = 601,
		[Description("优惠券不存在或已被删除")]
		CouponNotExists_Error,
		[Description("用户信息不符")]
		CouponUserInfo_Error,
		[Description("该优惠券已被领完")]
		CouponNotStock_Error,
		[Description("该优惠券不能被领取")]
		CouponNotSupportGet_Error,
		[Description("优惠券已过期")]
		CouponOverDue_Error,
		[Description("限领数量错误")]
		CouponLimitNum_Error,
		[Description("营销图库ID不存在或者已被删除")]
		ImageIdNotExists_Error = 701,
		[Description("系统异常错误")]
		Unknown_Error = 999,
		[Description("活动不存在或者已删除")]
		Activity_NoExist = 701,
		[Description("活动关联商品不存在")]
		Activity_RelationProduct_NoExist,
		[Description("拼团信息不存在")]
		FightGroup_NoExist,
		[Description("核销码无效")]
		VerificationItem_NoExist = 801,
		[Description("非本门店核销码")]
		VerificationItemNotStore,
		[Description("商品不支持退款")]
		Product_NoSupportRefund,
		[Description("商品不支持过期退款")]
		Product_NoSupportExpireRefund
	}
}
