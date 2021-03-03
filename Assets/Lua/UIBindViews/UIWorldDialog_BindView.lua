--[[Notice:This lua uiview file is auto generate by UIViewExporter，don't modify it manually! --]]

local public = {}
local cachedViews = nil

public.viewPath = "Arts/UI/Prefabs/UIWorldDialog.prefab"

function public.BindView(uiView, Panel)
	cachedViews = {}
	if nil ~= Panel then
		local collection = Panel:GetComponent("UIComponentCollection")
		if nil ~= collection then
			uiView.m_UIModelBtn = collection:Get(0)
			uiView.m_showLogBtn = collection:Get(1)
		else
			error("BindView Error! UIComponentCollection is nil!")
		end
	else
		error("BindView Error! Panel is nil!")
	end
end

function public.UnBindView(uiView)
	cachedViews = nil
	uiView.m_UIModelBtn = nil
	uiView.m_showLogBtn = nil
end

return public