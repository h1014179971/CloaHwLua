local _, LuaDebuggee = pcall(require, 'LuaDebuggee')
if LuaDebuggee and LuaDebuggee.StartDebug then
	if LuaDebuggee.StartDebug('127.0.0.1', 9826) then
		print('LuaPerfect: Successfully connected to debugger!')
	else
		print('LuaPerfect: Failed to connect debugger!')
	end
else
	print('LuaPerfect: Check documents at: https://luaperfect.net')
end



--require("3rd.BigNum.test_BigRat")
--require("LuaData.LuaData")
--主入口函数。从这里开始lua逻辑
local rawset = rawset


-- 全局函数
-- 用于声明全局变量
function define(name, value)
    rawset(_G, name, value)
end

local function initialize()
    Log.initialize()-- LuaLogHelper.initialize()
    NetManager.Initialize()

    --UIManager初始化
    UIManager.initialize()
    -- 模块开始加载
    Modules.PriorityBoot()
end

-- 在此处定义注册一些全局变量
local function gloablDefine()
	
	require("require")
	--require("LuaData.IdleCityTruckRes")
	
    --define("LuaLogHelper", require("Utilitys.LuaLogHelper"))
	--require("3rd.BigNum.test_BigRat")
	
    -- 模块初始化
    Modules.Initialize()
    
end

-- 初始化一些参数
local function initParam()
    -- 初始化随机种子
    math.randomseed(tostring(os.time()):reverse():sub(1, 6))

    --垃圾收集器间歇率控制着收集器需要在开启新的循环前要等待多久。 增大这个值会减少收集器的积极性。
    --当这个值比 100 小的时候，收集器在开启新的循环前不会有等待。 设置这个值为 200 就会让收集器等到总内存使用量达到之前的两倍时才开始新的循环。
    collectgarbage("setpause", 99)
    --垃圾收集器步进倍率控制着收集器运作速度相对于内存分配速度的倍率。 增大这个值不仅会让收集器更加积极，还会增加每个增量步骤的长度。
    --不要把这个值设得小于 100 ， 那样的话收集器就工作的太慢了以至于永远都干不完一个循环。 默认值是 200 ，这表示收集器以内存分配的"两倍"速工作。
    collectgarbage("setstepmul", 2000)
    --重启垃圾收集器的自动运行
    collectgarbage("restart")
end

function InitLua()
	gloablDefine()
	initParam()
	initialize()
	Log.debug("init lua");
	UIManager.Open(ECEnumType.UIEnum.UILoading);
end


function Main()
    gloablDefine()
    initParam()
    initialize()
	Log.debug("init main lua111");
    --UIManager.Open(ECEnumType.UIEnum.UIBagDialog);
	--EventMgr.DispatchEvent(ECEventType.UIEvent.CREATE_PANEL,ECEnumType.UIEnum.UILoading)
	UIManager.Open(ECEnumType.UIEnum.UILoading);
	--local IdleCityTruckRes = require("LuaData.IdleCityTruckRes")
	
	Log.debug(IdleCityTruckRes[1].lvMin);
	--Log.debug("Item="..IdleItem[1].icon)
	PlayerMgr.Instance():OnInit()
	--local txt = AssetLoader.LoadAsync("IdleSite.json",type(TextAsset),function(obj)
			--print("init main lua222");
		--if obj ~= nil then
				--local jsonStr = obj.text;
				--print("init main lua333",jsonStr);
		--end		
	--end);
	
    CommonUtil.GetSceneMgr():LoadSceneAdditiveAsync("IdleScene_1001.unity",function(progress)
		Log.debug("progress=="..progress)
		--EventMgr.DispatchEvent(Modules.moduleId.Common, Modules.notifyId.Common.Loading_progress, progress)
		EventMgr.DispatchEvent(ECEventType.UIEvent.Loading_progress,progress)
	end, function(sceneName)
			Log.debug("is done")
			UIManager.Close(ECEnumType.UIEnum.UILoading)
			IdleCityCtrl.Instance():OnInit();
			IdleTruckCtrl.Instance():OnInit();
			--require("3rd.BigNum.test_BigRat")	
			--require("3rd.BigNum.test_BigNumber")
        --EventMgr.DispatchEvent(Modules.moduleId.Common, Modules.notifyId.Common.CREATE_PANEL, ECEnumType.UIEnum.Login)
        --UIManager.Close(ECEnumType.UIEnum.Loading)
    end)
end

--场景切换通知
function OnLevelWasLoaded(level)
    collectgarbage("collect")
    Time.timeSinceLevelLoad = 0
end

function OnApplicationPause(pause)
	Log.debug("Main OnApplicationPause:"..tostring(pause));
	if pause then
		PlayerMgr.Instance():RecordPlayer();
	end
end

function OnApplicationQuit()
	PlayerMgr.Instance():RecordPlayer();
end
