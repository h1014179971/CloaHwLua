﻿---
---                 ColaHwLua
--- Copyright © 2018-2049 ColaHwLua Moha
---              require Module业务逻辑
---

--控制全局变量的新建与访问
--require("Utilitys.LuaGlobalCheck")
require("Common.ECEnumType")
require("Common.ECEventType")
require("Common.LuaAppConst")
require("Core.middleclass")
require("Mgrs.EventMgr")
require("Tools.Handler")
require("Utilitys.LuaLogHelper")
require("Game.Main.Modules")
require("Game.Main.GUICollections")
require("Core.Net.NetManager")
require("Mgrs.UIManager")
require("Mgrs.ConfigMgr")
require("Protocols.Protocol")
require("Tools.Timer")

require("LuaData.LuaData")
json = require "cjson"
require("Utilitys.Table_Utils")
require("utilitys.TimeUtil")
--游戏逻辑业务--
require("Game.Idle.Mgrs.PlayerMgr")
require("Game.Idle.Ctrls.IdleCityCtrl")
require("Game.Idle.Ctrls.IdleTruckCtrl")
require("Game.Idle.Common.Const")