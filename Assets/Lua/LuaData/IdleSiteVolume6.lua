local IdleSiteVolume6 = {
		{
			Lv = 1,
			price = "20000000,2",
			volume = 2,
			shelfRes = "101",
		},
		{
			Lv = 2,
			price = "50000000,2",
			volume = 4,
			shelfRes = "202",
		},
		{
			Lv = 3,
			price = "79000000,2",
			volume = 6,
			shelfRes = "301",
		},
		{
			Lv = 4,
			price = "11000000,3",
			volume = 8,
			shelfRes = "301,101",
		},
		{
			Lv = 5,
			price = "14000000,3",
			volume = 10,
			shelfRes = "301,202",
		},
		{
			Lv = 6,
			price = "19000000,3",
			volume = 12,
			shelfRes = "301,301",
		},
		{
			Lv = 7,
			price = "24000000,3",
			volume = 14,
			shelfRes = "301,301,101",
		},
		{
			Lv = 8,
			price = "33000000,3",
			volume = 16,
			shelfRes = "301,301,202",
		},
		{
			Lv = 9,
			price = "49000000,3",
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
_G.IdleSiteVolume6 = IdleSiteVolume6
setmetatable(_G,IdleSiteVolume6)
return IdleSiteVolume6