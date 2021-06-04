local IdleSpecialEvent = {
		{
			id = 1,
			desc = "观看视频后，叉车单次运输数量上限*2，大货车载量*2.持续60s",
			stayTime = 60,
			btnRes = "btn-11",
			spriteRes = "icon-11",
		},
		{
			id = 2,
			desc = "观看视频后，所有总站叉车，配送人员，配送车辆，驿站员工移动速度全部*2，持续60s",
			stayTime = 60,
			btnRes = "btn-shopping",
			spriteRes = "icon-shopping",
		},
}
_G.IdleSpecialEvent = IdleSpecialEvent
return IdleSpecialEvent