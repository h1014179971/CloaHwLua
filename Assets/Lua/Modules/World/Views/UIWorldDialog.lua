---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                UIWorldDialog界面
---

local UIBase = require("Core.ui.UIBase")
local UIWorldDialog = Class("UIWorldDialog",UIBase)

local _instance = nil

-- 获取UI实例的接口
function UIWorldDialog.Instance()
    if nil == _instance then
        _instance = UIWorldDialog:new()
    end
    return _instance
end

-- virtual 子类可以初始化一些变量
function UIWorldDialog:InitParam()

end

-- override UI面板创建结束后调用，可以在这里获取gameObject和component等操作
function UIWorldDialog:OnCreate()
    local fullMask = self.Panel:FindChildByPath("FullScreenMask")
    CommonUtil.HandleMainCameraEvent(fullMask)
end

-- 界面可见性变化的时候触发
function UIWorldDialog:OnShow(isShow)

end

-- 界面销毁的过程中触发
function UIWorldDialog:OnDestroy()
	UIBase.OnDestroy(self)
end

-- 注册UI事件监听
function UIWorldDialog:RegisterEvent()

end

-- 取消注册UI事件监听
function UIWorldDialog:UnRegisterEvent()

end

------------------- UI事件回调 --------------------------
function UIWorldDialog:onClick(name)
    if name == "showLogBtn" then
        UIManager.Open(ECEnumType.UIEnum.DebugPanel)
    elseif name == "UIModelBtn" then
        UIManager.Open(ECEnumType.UIEnum.UIModel)
    end
end

function UIWorldDialog:onBoolValueChange(name, isSelect)

end

---------------------- UI事件回调 --------------------------

return UIWorldDialog