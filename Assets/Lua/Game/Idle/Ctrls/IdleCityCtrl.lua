---
---                 ColaHwLua
--- Copyright © 2018-2049 ColaHwLua Moha
---              IdleCityCtrl Controller业务逻辑
---

local IdleCityCtrl = Class("IdleCityCtrl")
_G.IdleCityCtrl = IdleCityCtrl
setmetatable(_G,IdleCityCtrl)
local _instance = nil
function IdleCityCtrl.Instance()
	if nil == _instance then
		_instance = IdleCityCtrl:new()
	end
	return _instance
end
--- Controller模块的初始化，可以在这里做初始化和添加监听等操作
function IdleCityCtrl:OnInit()
	self:CreateOUtSite();
end

function IdleCityCtrl:CreateOUtSite()
	local outSite = UnityEngine.GameObject.FindGameObjectWithTag(Const.Tags.CityOutSite); 
	Log.debug("outSite=="..outSite.name);
end

--- Controller模块的销毁，可以在这里做清理工作和取消监听等操作
function IdleCityCtrl:OnDestroy()

end


return IdleCityCtrl