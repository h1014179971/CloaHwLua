---
---                 ColaHwLua
--- Copyright © 2018-2049 ColaHwLua Moha
---              Handler Module业务逻辑
---

-- 函数句柄
local Handler = {}
-- caller 执行域
-- func 处理方法
function Handler.Create(func, caller)
	return function ( ... )
		func(caller, ...)
	end
end

function Handler.Create2(func)
	return function (...)
		func(...)
	end
end
_G.Handler = Handler