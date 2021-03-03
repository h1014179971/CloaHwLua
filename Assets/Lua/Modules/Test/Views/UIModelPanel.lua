---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                UIModelPanel界面
---

local UIBase = require("Core.ui.UIBase")
local UIModelPanel = Class("UIModelPanel", UIBase)

local _instance = nil

local SettingNames = {
    "Merchant_female",
    "Boy",
    "Girl",
}

local ResPath = {
    "Arts/Avatar/Merchant_female.prefab",
    "Arts/Avatar/Boy.prefab",
    "Arts/Avatar/Girl.prefab",
}

local AnimNames = {
    "Idle",
    "Walk",
    "Talk",
    "Attack01",
    "Damage",
    "Dead",
}

-- 获取UI实例的接口
function UIModelPanel.Instance()
    if nil == _instance then
        _instance = UIModelPanel:new()
    end
    return _instance
end

-- virtual 子类可以初始化一些变量
function UIModelPanel:InitParam()
    self.animIndex = 1
    self.curChar = nil
end

-- override UI面板创建结束后调用，可以在这里获取gameObject和component等操作
function UIModelPanel:OnCreate()
    self.uiModel = self.Panel:GetComponentByPath("UIModel", "UGUIModel")
    self.uiModel.onModelClick = function(name)
        self:OnModelClick(name)
    end

    --测试多个UIModel是否可以正确工作
    self.uiModel2 = self.Panel:GetComponentByPath("UIModel2", "UGUIModel")
    self.uiModel2.onModelClick = function(name)
        self:OnModelClick(name)
    end
end

-- 界面可见性变化的时候触发
function UIModelPanel:OnShow(isShow)

end

-- 界面销毁的过程中触发
function UIModelPanel:OnDestroy()
    UIBase.OnDestroy(self)
    self.animIndex = -1;
    self.curChar = nil
end

-- 注册UI事件监听
function UIModelPanel:RegisterEvent()

end

-- 取消注册UI事件监听
function UIModelPanel:UnRegisterEvent()

end

------------------- UI事件回调 --------------------------
function UIModelPanel:onClick(name)
    if name == "Btn_One" then
        self:UpdateModel(1)
    elseif name == "Btn_Two" then
        self:UpdateModel(2)
    elseif name == "Btn_Three" then
        self:UpdateModel(3)
    elseif name == "Btn_Close" then
        self:DestroySelf()
    elseif name == "Btn_Switch" then
        CommonUtil.GetSceneMgr():UnloadSceneAsync("xinshoucun",nil)
    elseif name == "Btn_AnimBefore" then
        if nil ~= self.curChar then
            self.animIndex = self.animIndex -1
            if self.animIndex < 1 then
                self.animIndex = #AnimNames
            end
            self.curChar:PlayAnimation(AnimNames[self.animIndex])
        end
    elseif name == "Btn_AnimBack" then
        if nil ~= self.curChar then
            self.animIndex = self.animIndex + 1
            if self.animIndex > #AnimNames then
                self.animIndex = 1
            end
            self.curChar:PlayAnimation(AnimNames[self.animIndex])
        end
        if nil ~= self.curChar2 then
            self.animIndex = self.animIndex + 1
            if self.animIndex > #AnimNames then
                self.animIndex = 1
            end
            self.curChar2:PlayAnimation(AnimNames[self.animIndex])
        end
    end
end

function UIModelPanel:onBoolValueChange(name, isSelect)

end

---------------------- UI事件回调 --------------------------

function UIModelPanel:OnModelClick(name)
    print("----------->点击了", name)
end

function UIModelPanel:UpdateModel(index)
    local isModelExist = self.uiModel:IsModelExist(index)
    if not isModelExist then
        local character = SceneCharacter.CreateSceneCharacterInf(ResPath[index] or "",AnimCtrlEnum.CharAnimation,false)
        self.uiModel:SetModelAt(index,character)
    end
    self.uiModel:UpdateModelShownIndex(index)
    self.uiModel:ImportSetting(SettingNames[index] or "")
    self.curChar = self.uiModel:GetModelAt(index)

    --测试多个UIModel是否可以正确工作
    isModelExist = self.uiModel2:IsModelExist(index)
    if not isModelExist then
        local character = SceneCharacter.CreateSceneCharacterInf(ResPath[index] or "",AnimCtrlEnum.CharAnimation,false)
        self.uiModel2:SetModelAt(index,character)
    end
    self.uiModel2:UpdateModelShownIndex(index)
    self.uiModel2:ImportSetting(SettingNames[index] or "")
    self.curChar2 = self.uiModel2:GetModelAt(index)
end

return UIModelPanel