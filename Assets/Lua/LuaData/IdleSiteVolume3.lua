local IdleSiteVolume3 = {
		{
			Lv = 1,
			price = "20000,0",
			volume = 2,
			shelfRes = "101",
		},
		{
			Lv = 2,
			price = "50000,0",
			volume = 4,
			shelfRes = "202",
		},
		{
			Lv = 3,
			price = "76000,0",
			volume = 6,
			shelfRes = "301",
		},
		{
			Lv = 4,
			price = "99000,0",
			volume = 8,
			shelfRes = "301,101",
		},
		{
			Lv = 5,
			price = "150000,0",
			volume = 10,
			shelfRes = "301,202",
		},
		{
			Lv = 6,
			price = "210000,0",
			volume = 12,
			shelfRes = "301,301",
		},
		{
			Lv = 7,
			price = "350000,0",
			volume = 14,
			shelfRes = "301,301,101",
		},
		{
			Lv = 8,
			price = "950000,0",
			volume = 16,
			shelfRes = "301,301,202",
		},
		{
			Lv = 9,
			price = "12000000,0",
			volume = 18,
			shelfRes = "301,301,301",
		},
		{
			Lv = 10,
			price = "-1,0",
			volume = 18,
			shelfRes = "301,301,301",
		},
}
_G.IdleSiteVolume3 = IdleSiteVolume3
setmetatable(_G,IdleSiteVolume3)
return IdleSiteVolume3