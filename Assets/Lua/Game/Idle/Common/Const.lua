---------------------------------------------------------------------
-- CloaHwLua (C) CompanyName, All Rights Reserved
-- Created by: AuthorName
-- Date: 2021-06-09 15:38:50
---------------------------------------------------------------------

-- To edit this template in: Data/Config/Template.lua
-- To disable this template, check off menuitem: Options-Enable Template File

---@class Const
local Const = {
	Tags = {
		CityOutSite = "CityOutSite",
		},
	
}
_G.Const = Const;
setmetatable(_G,Const);
return Const;