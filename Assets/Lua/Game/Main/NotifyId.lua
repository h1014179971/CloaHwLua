---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---            NotifyId 事件声明的总集合
---

local NotifyId = {}

NotifyId.Common = {
	CREATE_PANEL = 0,       -- 创建UIPanel
	DESTROY_PANEL = 1,      -- 销毁UIPanel
	ALLUI_SHOWSTATE_CHANGED = 2,  --所有的UI显隐状态变化
	Loading_progress = 3, --loading进度条 
} 

return NotifyId