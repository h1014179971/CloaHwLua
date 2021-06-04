---
---                 ColaHwLua
--- Copyright © 2018-2049 ColaHwLua Moha
---              EventMgr 消息派发机制
---



local EventLib = require "Tools.EventLib"

local EventMgr = {}
_G.EventMgr = EventMgr
local events = {}

function EventMgr.AddListener(event,handler)
	if not event or type(event) ~= "string" then
		log("event parameter in addlistener function has to be string, " .. type(event) .. " not right.")
	end
	if not handler or type(handler) ~= "function" then
		log("handler parameter in addlistener function has to be function, " .. type(handler) .. " not right")
	end

	if not events[event] then
		--create the Event with name
		events[event] = EventLib:new(event)
	end

	--conn this handler
	events[event]:connect(handler)
end

function EventMgr.TriggerEvent(event,...)
	if not events[event] then
		Log.error("brocast " .. event .. " has no event.")
	else
		events[event]:fire(...)
	end
end

function EventMgr.RemoveListener(event,handler)
	if not events[event] then
		log("remove " .. tostring(event) .. " has no event.")
	else
		events[event]:disconnect(handler)
	end
end

function EventMgr.RemoveAllListener(event)
	if not events[event] then
		log("remove " .. tostring(event) .. " has no event.")
	else
		events[event]:Destroy()
		events[event] = nil
	end
end


return EventMgr


