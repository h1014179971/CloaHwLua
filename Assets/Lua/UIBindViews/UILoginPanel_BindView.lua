--[[Notice:This lua uiview file is auto generate by UIViewExporter，don't modify it manually! --]]

local public = {}
local cachedViews = nil

public.viewPath = "Arts/UI/Prefabs/UILoginPanel.prefab"

function public.BindView(uiView, Panel)
	cachedViews = {}
	if nil ~= Panel then
		local collection = Panel:GetComponent("UIComponentCollection")
		if nil ~= collection then
			uiView.m_usernameInput = collection:Get(0)
			uiView.m_passwordInput = collection:Get(1)
			uiView.m_bg = collection:Get(2)
			uiView.m_cancelBtn = collection:Get(3)
			uiView.m_okBtn = collection:Get(4)
			uiView.m_Item = collection:Get(5)
			uiView.m_Template = collection:Get(6)
			uiView.m_Dropdown = collection:Get(7)
			uiView.m_vertical_tableview = collection:Get(8)
		else
			error("BindView Error! UIComponentCollection is nil!")
		end
	else
		error("BindView Error! Panel is nil!")
	end
end

function public.UnBindView(uiView)
	cachedViews = nil
	uiView.m_usernameInput = nil
	uiView.m_passwordInput = nil
	uiView.m_bg = nil
	uiView.m_cancelBtn = nil
	uiView.m_okBtn = nil
	uiView.m_Item = nil
	uiView.m_Template = nil
	uiView.m_Dropdown = nil
	uiView.m_vertical_tableview = nil
end

function public.GetCellView(uiView,tableview, cell)
	local cellView = cachedViews[cell]
	if nil == cellView then
		cellView = {}
		if tableview == uiView.m_vertical_tableview then
			local collection = cell:GetComponent("UIComponentCollection")
			cellView.m_Text = collection:Get(0)
			cellView.m_Button = collection:Get(1)
		end
		cachedViews[cell] = cellView
	end
	return cellView
end

return public