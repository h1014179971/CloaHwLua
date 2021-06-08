---
---                 ColaHwLua
--- Copyright © 2018-2049 ColaHwLua Moha
---              PlayerMgr Controller业务逻辑
---

--local json = require "cjson"
local json_safe = require "cjson.safe"
local util = require "3rd.cjson.util"
local PlayerMgr = Class("PlayerMgr")
_G.PlayerMgr = PlayerMgr
setmetatable(_G,PlayerMgr)
local _instance = nil
function PlayerMgr.Instance()
	if nil == _instance then
		_instance = PlayerMgr:new()
	end
	return _instance
end

--- Controller模块的初始化，可以在这里做初始化和添加监听等操作
function PlayerMgr:OnInit()

	Log.debug("PlayerMgr OnInit")
	self._playerFileName = "player.json"
	local dirPath =  UnityEngine.Application.persistentDataPath.."/JsonData"
	dirPath = string.gsub(dirPath,"/","\\")
	os.execute("mkdir "..dirPath)
	self._playerFilePath = dirPath.."/"..self._playerFileName
	local player = {
	money = 1000,
	}
	local str = json.encode(player)
	----local json_text = util.file_load(jsonPath)
	util.file_save(self._playerFilePath,str)
	local json_text = util.file_load(self._playerFilePath)
	local data = json.decode(json_text)
	
	local a = 10^7
	local b = math.log10(a)
	--local l1 = Foundation.Long2(10,2)
	--local l2 = Foundation.Long2(100,2)
	--local l3 = Foundation.Long2.op_Addition(l1,l2)
	--Log.debug("l1="..l1,"l2="..l2,"l3="..l3)
end

--- Controller模块的销毁，可以在这里做清理工作和取消监听等操作
function PlayerMgr:OnDestroy()

end



return PlayerMgr