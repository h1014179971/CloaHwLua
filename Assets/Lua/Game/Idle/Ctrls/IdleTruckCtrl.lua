---
---                 ColaHwLua
--- Copyright © 2018-2049 ColaHwLua Moha
---              IdleTruckCtrl Controller业务逻辑
---

require("Game.Idle.Models.IdleTruckModel")
local IdleTruckCtrl = Class("IdleTruckCtrl")
_G.IdleTruckCtrl = IdleTruckCtrl
setmetatable(_G,IdleTruckCtrl)
local _instance = nil
function IdleTruckCtrl.Instance()
	if nil == _instance then
		_instance = IdleTruckCtrl:new()
	end
	return _instance
end
--- Controller模块的初始化，可以在这里做初始化和添加监听等操作
function IdleTruckCtrl:OnInit()
	local obj = SG.ResourceManager.Instance:GetObjectFromPool("truck.prefab",true,1);
	obj.transform.position = Vector3.zero;
end

function IdleTruckCtrl:CreateOUtSite()
end

--- Controller模块的销毁，可以在这里做清理工作和取消监听等操作
function IdleTruckCtrl:OnDestroy()

end

return IdleTruckCtrl