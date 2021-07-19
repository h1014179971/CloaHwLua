---
---                 ColaHwLua
--- Copyright © 2018-2049 ColaHwLua Moha
---              IdleTruckModel Module业务逻辑
---

--- 公有字段和方法
local IdleTruckModel = {}


--- Module模块的初始化，可以在这里做初始化和添加监听等操作
function IdleTruckModel:OnInit(obj)
	self._seeker = obj:GetComponent(Seeker);
end

--- Module模块的销毁，可以在这里做清理工作和取消监听等操作
function IdleTruckModel:OnDestroy()

end



return IdleTruckModel