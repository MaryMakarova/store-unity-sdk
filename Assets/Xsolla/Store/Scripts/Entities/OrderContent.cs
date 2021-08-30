﻿using System;

namespace Xsolla.Store
{
	[Serializable]
	public class OrderContent
	{
		public Price price;
		public VirtualPrice virtual_price;
		public string is_free;
		public OrderItem[] items;
	}
}