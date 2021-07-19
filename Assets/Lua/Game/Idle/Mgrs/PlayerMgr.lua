---
---                 ColaHwLua
--- Copyright © 2018-2049 ColaHwLua Moha
---              PlayerMgr Controller业务逻辑
---

--local json = require "cjson"
require("Game.Idle.Mgrs.Player")
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
	self:ReadFile();
	
	
end

function PlayerMgr:ReadFile()
	Log.debug("PlayerMgr ReadFile");
	self._playerFileName = "player.json";
	self._player = {};
	local dirPath =  UnityEngine.Application.persistentDataPath.."/JsonData";
	dirPath = string.gsub(dirPath,"/","\\");
	os.execute("mkdir "..dirPath);
	self._playerFilePath = dirPath.."/"..self._playerFileName;
	local json_text = util.file_load(self._playerFilePath);
	if json_text ~= nil then
		self._player = json.decode(json_text);
	else
		self:CreatePlayer();
	end	
end

function PlayerMgr:CreatePlayer()
	self._player = Table_Utils.DeepCopy(Player);
	local time = TimeUtil.getTime();
	self._player.firstLoginTime = time;
	local playerCity = Table_Utils.DeepCopy(PlayerCity);
	playerCity.cityId = self._player.cityId;
	table.insert(self._player.playerCity,#self._player.playerCity,playerCity);
	
end

function PlayerMgr:RecordPlayer()
	Log.debug("PlayerMgr RecordPlayer");
	if self._player ~= nil then
		local str = json.encode(self._player)
		util.file_save(self._playerFilePath,str)
	end	
	
end

--- Controller模块的销毁，可以在这里做清理工作和取消监听等操作
function PlayerMgr:OnDestroy()

end



return PlayerMgr