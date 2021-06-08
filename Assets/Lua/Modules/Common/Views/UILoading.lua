---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                 UILoading UI类
---


--local file = require("3rd.cjson.util")
local UIBase = require("Core.ui.UIBase")
local UILoading = Class("UILoading",UIBase)

local _instance = nil

-- 获取UI实例的接口
function UILoading.Instance()
    if nil == _instance then
        _instance = UILoading:new()
    end
    return _instance
end

-- virtual 子类可以初始化一些变量
function UILoading:InitParam()
	
end

-- override UI面板创建结束后调用，可以在这里获取gameObject和component等操作
function UILoading:OnCreate()
	self.m_loadTxt.text = "哈乐沃德"
	self.m_process.value = 0
	--self:Test()
	--Timer.New(self.Test,1,3):Start()
	coroutine.start(self.Test);
	--local jsonPath1 =  UnityEngine.Application.persistentDataPath.."/JsonData/player.json"
	----os.execute("mkdir C:\\Users\\hw-ZGM\\AppData\\LocalLow\\DefaultCompany\\CloaHwLua\\JsonData\\player")
	----local jsonPath1 = "C:/Users/hw-ZGM/AppData/LocalLow/DefaultCompany/CloaHwLua/JsonData/player/player.json"
	--jsonPath1 = string.gsub(jsonPath1,"/","\\\\")
	--local jsonPath = "C:/Users/hw-ZGM/Desktop/test.json"
	--local player = {
		--money = 1000,
	--}
	--local str = json.encode(player)
	------local json_text = util.file_load(jsonPath)
	--util.file_save(jsonPath1,str)
end

-- 界面可见性变化的时候触发
function UILoading:OnShow(isShow)

end

-- 界面销毁的过程中触发
function UILoading:OnDestroy()
	UIBase.OnDestroy(self)
end

-- 注册UI事件监听
function UILoading:RegisterEvent()
	EventMgr.RegisterEvent(ECEventType.UIEvent.Loading_progress, self.SceneProgress,self)
	
	
end

-- 取消注册UI事件监听
function UILoading:UnRegisterEvent()
	
end

------------------- UI事件回调 --------------------------
function UILoading:onClick(name)

end

function UILoading:onBoolValueChange(name, isSelect)

end

---------------------- UI事件回调 --------------------------
function UILoading:Test()
	--self.m_process.value = 0.5
	coroutine.wait(5)
	Log.debug("Test")
	
end
function UILoading:SceneProgress(progress)
	Log.debug("UILoading progress"..progress)
	self.m_process.value = progress
end

return UILoading