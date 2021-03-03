---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                   登录界面
---

local UIBase = require("Core.ui.UIBase")
local UIDebugPanel = Class("UIDebugPanel", UIBase)

local _instance = nil

-- virtual 子类可以初始化一些变量
function UIDebugPanel:InitParam()
    self.uiDepthLayer = ECEnumType.UIDepth.DEBUG
    self:ShowUIBlur(true)
end

function UIDebugPanel.Instance()
    if nil == _instance then
        _instance = UIDebugPanel:new()
    end
    return _instance
end

-- override UI面板创建结束后调用，可以在这里获取gameObject和component等操作
function UIDebugPanel:OnCreate()
    --self.DebugText = self.Panel:FindChildByPath("ScrollView/Viewport/Text"):GetComponent("UnityEngine.UI.Text")
    --if self.DebugText then
    --    CommonUtils.AttachScreenText(self.DebugText)
    --end
    CommonUtil.AttachScreenText(self.m_Text)
end

-- 界面可见性变化的时候触发
function UIDebugPanel:OnShow(isShow)

end

-- 界面销毁的过程中触发
function UIDebugPanel:OnDestroy()
    UIBase.OnDestroy(self)
    self.DebugText = nil
end

-- 注册UI事件监听
function UIDebugPanel:RegisterEvent()

end

-- 取消注册UI事件监听
function UIDebugPanel:UnRegisterEvent()

end

------------------- UI事件回调 --------------------------
function UIDebugPanel:onClick(name)
    if name == "BtnClose" then
        CommonUtil.UnAttachScreenText()
        self:DestroySelf()
    elseif name == "BtnClear" then
        CommonUtil.ClearSreenLog()
    end
end

function UIDebugPanel:onBoolValueChange(name, isSelect)

end

---------------------- UI事件回调 --------------------------

return UIDebugPanel