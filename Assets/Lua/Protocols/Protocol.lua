---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                Protocol 协议文件
---

local Protocol = {}
_G.Protocol = Protocol
setmetatable(Protocol,{__index = _G})

Protocol.C2S_PING = 1
Protocol.C2S_Login = 2

return Protocol