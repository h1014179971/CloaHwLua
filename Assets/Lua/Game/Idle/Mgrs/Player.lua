local Player = {
	cityId = 1001,
	playerCity = {},
	playerGuide = {},
	firstLoginTime = nil,
}
_G.Player = Player
setmetatable(_G,Player)

local PlayerCity = {
	cityId = nil,
	money = nil,
	loginTime = nil,
	leaveTime = nil,
	playerSites = {},
}
_G.PlayerCity = PlayerCity
setmetatable(_G,PlayerCity)

local PlayerSite = {
	id = nil,
	cityId = nil,	
	isLock = nil,
	siteBaseLv = nil,
	siteTimeLv = nil,
	siteVolumeLv = nil,
	loadItemNum = nil,
}