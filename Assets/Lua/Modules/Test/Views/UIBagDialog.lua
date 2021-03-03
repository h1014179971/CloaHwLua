---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                 UIBagDialog UI类
---

local UIBase = require("Core.ui.UIBase")
local UIBagDialog = Class("UIBagDialog",UIBase)

local _instance = nil

-- 获取UI实例的接口
function UIBagDialog.Instance()
    if nil == _instance then
        _instance = UIBagDialog:new()
    end
    return _instance
end

-- virtual 子类可以初始化一些变量
function UIBagDialog:InitParam()

end

-- override UI面板创建结束后调用，可以在这里获取gameObject和component等操作
function UIBagDialog:OnCreate()

end

-- 界面可见性变化的时候触发
function UIBagDialog:OnShow(isShow)

end

-- 界面销毁的过程中触发
function UIBagDialog:OnDestroy()
	UIBase.OnDestroy(self)
end

-- 注册UI事件监听
function UIBagDialog:RegisterEvent()

end

-- 取消注册UI事件监听
function UIBagDialog:UnRegisterEvent()

end

------------------- UI事件回调 --------------------------
function UIBagDialog:onClick(name)

end

function UIBagDialog:onBoolValueChange(name, isSelect)

end

---------------------- UI事件回调 --------------------------

return UIBagDialog