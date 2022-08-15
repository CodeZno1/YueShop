using System;

namespace Hidistro.Entities.Sales
{
	[Serializable]
	public class ShippingRegionInfo
	{
		public int TemplateId
		{
			get;
			set;
		}

		public int GroupId
		{
			get;
			set;
		}

		public int RegionId
		{
			get;
			set;
		}
	}
}
