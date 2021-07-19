--[[Notice:This lua uiview file is auto generate by UIViewExporter，don't modify it manually! --]]

local public = {}
local cachedViews = nil

public.viewPath = "UILoading.prefab"

function public.BindView(uiView, Panel)
	cachedViews = {}
	if nil ~= Panel then
		local collection = Panel:GetComponent("UIComponentCollection")
		if nil ~= collection then
			uiView.m_loadTxt = collection:Get(0)
			uiView.m_loadingText = collection:Get(1)
			uiView.m_processText = collection:Get(2)
			uiView.m_process = collection:Get(3)
			uiView.m_MyText = collection:Get(4)
		else
			error("BindView Error! UIComponentCollection is nil!")
		end
	else
		error("BindView Error! Panel is nil!")
	end
end

function public.UnBindView(uiView)
	cachedViews = nil
	uiView.m_loadTxt = nil
	uiView.m_loadingText = nil
	uiView.m_processText = nil
	uiView.m_process = nil
	uiView.m_MyText = nil
end

return public