﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuyBackPriceChange
{
	//  Properties ------------------------------------
	float buyBackPriceChange { get; set; }

	//  Methods ---------------------------------------
	float GetBuyBackPriceChange(float basePrice, float delta); //return về một tổng delta cũ và delta mới (delta là số tiền thay đổi)
														   //ví dụ: status a +10% base, status b +20% base, status c +20% hiện tại
														   //khi chạy status a, base = x, delta = 0
														   //sau khi chạy, base = x, delta = delta + 0.1*base
														   //sau khi chạy b, base = x, delta = delta + 0.2*base
														   //sau khi chạy c, base = x, delta = delta + 0.2(base + delta)
}
