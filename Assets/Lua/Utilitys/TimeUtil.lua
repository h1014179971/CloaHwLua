--[[
--  File Name: TimeUtil.lua
--  Description: 时间工具
--  Revisions:
--  ---------------------------------------
--    Date      Author    Description
--  ---------------------------------------
--  2019-07-31  liuping    Initial version
--
--
--]]

TimeUtil = {}

------------------------------------------------------------------------------------------------------------
--[[
--功能：获取当前时间
--入参：无
--返回值：
--      [1]               type:number           从1970年到当前时刻的秒数
--]]
------------------------------------------------------------------------------------------------------------
function TimeUtil.getTime()
    return os.time()
end


------------------------------------------------------------------------------------------------------------
--[[
--功能：获取当前时间
--入参：无
--返回值：
--      [1]               type:table{year = 1998, month = 9, day = 16, yday = 259, wday = 4,hour = 23, min = 48, sec = 10, isdst = false}
--]]
------------------------------------------------------------------------------------------------------------
function TimeUtil.getDate()
    return os.date()
end


------------------------------------------------------------------------------------------------------------
--[[
--功能：获取时间段
--入参：seconds           type:int              时间段（n秒）
--返回值：
--      [1]               type:table{ days = 1,hours = 23,minutes=59,seconds=59}
--]]
------------------------------------------------------------------------------------------------------------

function TimeUtil.getDateTime(seconds)
    local datetime = {}
    datetime.days = math.floor(seconds / 86400)
    datetime.hours = math.floor(seconds % 86400 / 3600)
    datetime.minutes = math.floor(seconds % 86400 % 3600 / 60)
    datetime.seconds = math.floor(seconds % 86400 % 3600 % 60)
    return datetime
end


------------------------------------------------------------------------------------------------------------
--[[
-- 功能：获取时间段字符串
-- 入参：seconds           type:int              时间段（n秒）
--       format            type:string           "hh:mm:ss"
-- 返回值：
--       [1]               type:string { 11天 23:59:59}
--]]
------------------------------------------------------------------------------------------------------------
function TimeUtil.getDateTimeString(seconds,format)
    local datetime = TimeUtil.getDateTime(seconds)
    local datetimestring = format
    --printt(datetime)
    --print(string.format("%2d",datetime.minutes))
    --datetimestring = string.gsub(datetimestring, "dd",string.format("%d",datetime.days))
    datetimestring = string.gsub(datetimestring, "hh",string.format("%02d",24*datetime.days+datetime.hours))
    datetimestring = string.gsub(datetimestring, "mm",string.format("%02d",datetime.minutes))
    datetimestring = string.gsub(datetimestring, "ss",string.format("%02d",datetime.seconds))
    --printyellow(datetimestring)
    return datetimestring
end
--获取时间段天数
function TimeUtil.getDayTime(seconds,format)
	local datetime = TimeUtil.getDateTime(seconds)
	local datetimestring = format
	if datetime.days == 0 then
		if seconds ~= 0 then
			datetime.days = 1
		end
	end
	datetimestring = string.gsub(datetimestring, "dd",string.format("%d",datetime.days))
	return datetimestring
end

------------------------------------------------------------------------------------------------------------
--[[
--功能：获取时间段
-- 入参：type:table{ days = 1,hours = 23,minutes=59,seconds=59}
--返回值：
--      [1] seconds           type:int              时间段（n秒）
--]]
------------------------------------------------------------------------------------------------------------

function TimeUtil.getSeconds(params)
    local seconds = 0
    if params.days then 
        seconds = seconds + params.days * 86400
    end 
    if params.hours then 
        seconds = seconds + params.hours * 3600
    end 
    if params.minutes then 
        seconds = seconds + params.minutes * 60
    end 
    if params.seconds then 
        seconds = seconds + params.seconds 
    end 
    return seconds
end

function TimeUtil.GetWeekday()
	local weekNum = os.date("*t").wday - 1 
	if weekNum == 0 then
		weekNum = 7
	end
	return weekNum
end

function TimeUtil.GetCurDay()
	return tonumber(os.date("%Y%m%d",TimeManager.GetCurrentTime()))
end

function TimeUtil.GetCurMonth()
    return tonumber(os.date("%m",TimeManager.GetCurrentTime()))
end

function TimeUtil.GetCurYear()
    return tonumber(os.date("%y",TimeManager.GetCurrentTime()))+2000
end

function TimeUtil.GetCurHour()
	local datetime = os.date("%H",TimeManager.GetCurrentTime())
	return datetime
end

--得到当前月结束时间戳
function TimeUtil.GetMonthEndTime(y,m)
    m = m + 1
    if m == 1 then
        y = y + 1
        m = 1
    end
    return (os.time({year=y,month=m,day = 0})+12*3600)
end

--得到时间戳
function TimeUtil.GetTimeByTime(str)
    local strList = StringUtil.StringSplit(str,",")
    return os.time({day=tonumber(strList[3]),month=tonumber(strList[2]),year=tonumber(strList[1])
        ,hour=tonumber(strList[4]),minute=tonumber(strList[5]),second=tonumber(strList[6])
    })
end

--转化时间
function TimeUtil.FormatTimePower(time)
    local m = math.floor((time%3600)/60)
    local s = math.floor((time%3600)%60)
	if m<10 then
		m = "0"..m	
	end
	if s<10 then
		s = "0"..s
	end
    return string.format("%s:%s",m,s)
end

--转化时间
function TimeUtil.FormatTimePower1(time)
	local m = math.floor((time%3600)/60)
	local s = math.floor((time%3600)%60)
	if m<10 then
		m = "0"..m
	end
	if s<10 then
		s = "0"..s
	end
	return string.format("%m%s",m,s)
end

--转化时间
function TimeUtil.FormatTime(time)
    local day = math.floor(time/(3600*24))
    local h = math.floor((time/3600)%24)
    local m = math.floor((time%3600)/60)
    local s = math.floor((time%3600)%60)
    if tonumber(day) == 0 then
        return string.format("%d时%d分%d秒",h,m,s)
    else
        return string.format("%d天%d时%d分%d秒",day,h,m,s)
    end
end

--转化时间
function TimeUtil.FormatTimeForHero(time)
    local h = math.floor((time/3600))
    local m = math.floor((time%3600)/60)
    local s = math.floor((time%3600)%60)
    local strH = "00:"
    if h ~= 0 then
        strH = tostring(h)..":"
        if h < 10 then
            strH = "0"..strH
        end
    end
    local strM = "00:"
    if m ~= 0 then
        strM = tostring(m)..":"
        if m < 10 then
            strM = "0"..strM
        end
    end
    local strS = "00"
    if s ~= 0 then
        strS = tostring(s)
        if s < 10 then
            strS = "0"..strS
        end
    end
    return strH..strM..strS
end

--得到天数
function TimeUtil.GetDayTime(time)
    return math.floor(time/(3600*24))
end

--比较两个时间戳，返回 年，月，日，周，时，分 差值
function TimeUtil.TimeUtilCompare(time1, time2)
	local tab1 = os.date("*t", time1)
	local tab2 = os.date("*t", time2)
	local year = tab1.year - tab2.year
	local month = tab1.month - tab2.month
	local day = tab1.day - tab2.day
	local wday = tab1.wday - tab2.wday
	local hour = tab1.hour - tab2.hour
	local min = tab1.min - tab2.min
	local ret = { year, month, day, wday, hour, min }
	return ret
end

----比较两个时间戳的间隔
function TimeUtil.TimeUtilIntervals(time1, time2)
	local diff = time1 - time2
	local day = math.floor(diff/86400)
	diff = diff - day * 86400
	local hour = math.floor(diff/3600)
	diff = diff - hour * 3600
	local min =  math.floor(diff/60)
	diff = diff - min * 60
	return {day=day,hour=hour,min=min,diff=diff}
end

function TimeUtil.StringToTime(str, delimiter)
	local _, _, y, m, d, _hour, _min, _sec = string.find(str, "(%d+)"..delimiter.."(%d+)"..delimiter.."(%d+)%s*(%d+):(%d+):(%d+)")
	local timestamp = os.time({year=y, month = m, day = d, hour = _hour, min = _min, sec = _sec})
	return timestamp
end