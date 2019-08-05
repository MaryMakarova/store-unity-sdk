﻿using System;

namespace Xsolla.Store
{
	[Serializable]
	public class CartItem
	{
		public string sku;
		public string name;
		public string[] attributes;
		public string[] media_list;
		public string type;
		public CartPrice price;
		public string[] vc_prices;
		public string[] groups;
		public string long_description;
		public string description;
		public string image_url;
		public string promotion;
		public bool is_free;
		public int order;
		public string purchase_limit;
		public int quantity;
	}
}