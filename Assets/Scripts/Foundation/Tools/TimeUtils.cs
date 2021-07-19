using System;
using UnityEngine;
using System.Text;

public class TimeUtils 
{
	/* 客户端服务器时间校准差值 */
	public static double timeAdjustValue = 0;
	public static DateTime dt1 = DateTime.Parse("1970-1-1 8:0:0");


	#region 本地时间相关
	/// <summary>
	/// 时间戳转为C#格式时间
	/// </summary>
	/// <param name=”timeStamp”></param>
	/// <returns></returns>
	public static DateTime GetTime(string timeStamp)
	{
		DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
		long lTime = long.Parse(timeStamp + "0000000");
		TimeSpan toNow = new TimeSpan(lTime); 
		return dtStart.Add(toNow);
	}

    public static DateTime GetDateTime(long second)
    {
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
     
        return dtStart.AddSeconds(second);
    }
	

    /// <summary>
    /// DateTime时间格式转换为Unix时间戳格式
    /// </summary>
    /// <param name=”time”></param>
    /// <returns></returns>
    public static string ConvertDateTime(System.DateTime time)
    {
        //datetime计算时有时区缓存
        //TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)),东八区输出时间为1970/1/1 8:00:00
        //TimeZone.CurrentTimeZone.ToUniversalTime(new System.DateTime(1970, 1, 1))	,东八区输出时间为1969/12/31 16:00:00
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        return Convert.ToString((int)(time - startTime).TotalSeconds);
    }
    public static long ConvertLongDateTime(System.DateTime time)
    {
        string str = ConvertDateTime(time);
        return long.Parse(str);
    }
    #endregion
    #region UTC时间相关

    public static DateTime GetUtcTime(long utcSecond)
	{
		DateTime dtStart = TimeZone.CurrentTimeZone.ToUniversalTime(new DateTime(1970, 1, 1) + TimeZoneInfo.Local.BaseUtcOffset);

		return dtStart.AddSeconds(utcSecond);
	}

	public static string ConvertUtcDateTime(System.DateTime time)
    {
		//datetime计算时有时区缓存
		//TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)),东八区输出时间为1970/1/1 8:00:00
		//TimeZone.CurrentTimeZone.ToUniversalTime(new System.DateTime(1970, 1, 1))	,东八区输出时间为1969/12/31 16:00:00
		// TimeZone.CurrentTimeZone.ToUniversalTime(new System.DateTime(1970, 1, 1) + TimeZoneInfo.Local.BaseUtcOffset),输出时间为1970/1/1 00:00:00
		System.DateTime startTime = TimeZone.CurrentTimeZone.ToUniversalTime(new System.DateTime(1970, 1, 1) + TimeZoneInfo.Local.BaseUtcOffset);
        return Convert.ToString((int)(time - startTime).TotalSeconds);
    }
    public static long ConvertLongUtcDateTime(System.DateTime time)
    {
        string str = ConvertUtcDateTime(time);
        return long.Parse(str);
    }
    #endregion
    /// <summary>
    /// Adjusts the time value.
    /// 客户端服务器时间校准差值
    /// </summary>
    /// <param name="serverMilliseconds">Server milliseconds.</param>
    public static void AdjustTimeValue(long serverMilliseconds)
	{
		double clientMilliseconds = System.DateTime.Now.Subtract(System.DateTime.Parse("1970-1-1 8:0:0")).TotalMilliseconds;
		timeAdjustValue = serverMilliseconds - clientMilliseconds;
	}

	public static long getNowTicket ( ) {
		DateTime dt2 = DateTime.Parse(DateTime.Now.AddMilliseconds(timeAdjustValue).ToString());
		TimeSpan ts = dt2 - dt1;
		long ds = ts.Ticks / 10000000;
		return ds;
	}

	public static long getNowTicketMs ( ) {
		timeAdjustValue = 0;
		DateTime dt2 = DateTime.Parse(DateTime.Now.AddMilliseconds(timeAdjustValue).ToString());
		TimeSpan ts = dt2 - dt1;
		long ds = ts.Ticks  / 10000 + DateTime.Now.Millisecond;
		return ds;
	}	

	public static string ToString (int ticket) {
		DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
		long lTime = long.Parse(ticket + "0000000");
		TimeSpan toNow = new TimeSpan(lTime);
		DateTime dtResult = dtStart.Add(toNow);
		return dtResult.ToString("yyyy-MM-dd HH:mm");
	}

	public static string toStringMMdd (int ticket) {
		DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
		long lTime = long.Parse(ticket + "0000000");
		TimeSpan toNow = new TimeSpan(lTime);
		DateTime dtResult = dtStart.Add(toNow);
		return dtResult.ToString("MM/dd");
	}

	public static string toStringHHmm (int ticket) {
		DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
		long lTime = long.Parse(ticket + "0000000");
		TimeSpan toNow = new TimeSpan(lTime);
		DateTime dtResult = dtStart.Add(toNow);
		return dtResult.ToString("HH:mm");
	}

	public static string getDateFormat (int ticket) {
		DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
		long lTime = long.Parse(ticket + "0000000");
		TimeSpan toNow = new TimeSpan(lTime);
		DateTime dtResult = dtStart.Add(toNow);
		return dtResult.Month + "月" + dtResult.Day + "日";
	}



	#region 切分时间，输出对应年月日时分秒星期 返回DataTime

	/// <summary>
	/// Split the specified ticket, yr, mm, dd, hh, ms, ss and dayofweek.
	/// 切分时间，输出对应年月日时分秒星期
	/// </summary>
	/// <param name="ticket">Ticket.</param>
	/// <param name="yr">Yr.</param>
	/// <param name="mm">Mm.</param>
	/// <param name="dd">Dd.</param>
	/// <param name="hh">Hh.</param>
	/// <param name="ms">Ms.</param>
	/// <param name="ss">Ss.</param>
	/// <param name="dayofweek">Dayofweek.</param>
	public static DateTime split(uint ticket, out int yr, out int mm, out int dd, out int hh, out int ms, out int ss, out int dayofweek) {
		DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
		long lTime = long.Parse(ticket + "0000000");
		TimeSpan toNow = new TimeSpan(lTime);
		DateTime dtResult = dtStart.Add(toNow);
		yr = dtResult.Year;
		mm = dtResult.Month;
		dd = dtResult.Day;
		hh = dtResult.Hour;
		ms = dtResult.Minute;
		ss = dtResult.Second;
		dayofweek = (int) dtResult.DayOfWeek;
		if (dayofweek == 0)
			dayofweek = 7;
		return dtResult;
	}

	#endregion

	#region 格式化为xx分xx秒

	/// <summary>
	/// Secondses to string.
	/// 格式化为 xx : xx
	/// </summary>
	/// <returns>The to string.</returns>
	/// <param name="seconds">Seconds.</param>
	public static string secondsToString (int seconds) {
		int minute = Mathf.CeilToInt(seconds / 60);
		int second = Mathf.CeilToInt(seconds - minute * 60);

		string min_str = string.Format("{0:00}", minute);
		string second_str = string.Format("{0:00}", second);
		return min_str + ":" + second_str;
	}

	#endregion
    /// <summary>
    /// 格式化为xx分xx秒
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public static string secondsToString1(int seconds)
    {
        int minute = Mathf.CeilToInt(seconds / 60);
        int second = Mathf.CeilToInt(seconds - minute * 60);

        string min_str = string.Format("{0:00}", minute);
        string second_str = string.Format("{0:00}", second);
        return min_str + "分" + second_str + "秒";
    }

	#region 格式化位xx天xx时xx分xx秒

	/// <summary>
	/// Secondses to string4.
	/// 格式化为xx天xx时xx分xx秒
	/// </summary>
	/// <returns>The to string4.</returns>
	/// <param name="seconds">Seconds.</param>
	public static string secondsToString4(int seconds)
	{
		StringBuilder buffer = new StringBuilder ();

		int day = seconds / (24 * 3600);
		int hour = Mathf.CeilToInt((seconds % (24 * 3600)) / 3600);
		int minute = Mathf.CeilToInt( (seconds % (24 * 3600)) % 3600 / 60);
		int second = Mathf.CeilToInt( (seconds % (24 * 3600)) % 3600 % 60);
		if(day>0){
			buffer.Append (day.ToString ());
			buffer.Append ("天");
		}

		if(0 <= hour  && hour  <= 9){
			buffer.Append("0");
		}
		buffer.Append(hour.ToString());
		buffer.Append("时");

		if(0 <= minute  && minute  <= 9){
			buffer.Append("0");
		}
		buffer.Append(minute.ToString());
		buffer.Append("分");

		if(0 <= second  && second  <= 9){
			buffer.Append("0");
		}
		buffer.Append(second.ToString());
		buffer.Append("秒");

		string timeText = buffer.ToString ();
		buffer.Remove (0, buffer.Length);

		return timeText;
	}

	#endregion

	/// <summary>
	/// Secondses to string3.
	/// </summary>
	/// <returns>The to string3.</returns>
	/// <param name="seconds">Seconds.</param>
	public static string secondsToString3 (int seconds) {
		
		if (seconds < 60)
		{
			string second_str = string.Format("{0:00}", seconds);
			return second_str + "秒";
		}
		else if (seconds < 3600)
		{
			int minute = Mathf.CeilToInt(seconds / 60);
			//int second = Mathf.CeilToInt(seconds - minute * 60);

			string min_str = string.Format("{0:00}", minute);
			//string second_str = string.Format("{0:00}", second);
			return min_str + "分";
		}
		else if (seconds < 24 * 3600)
		{
			int hour = Mathf.CeilToInt(seconds / 3600);
			//int minute = Mathf.CeilToInt( seconds % 3600 / 60);
			//int second = Mathf.CeilToInt( seconds % 3600 % 60);

			string shour = "";
			//string sminute = "";
			//string ssecond = "";

			if(0 <= hour  && hour  <= 9)
				shour = "0" + hour;
			else
				shour = hour.ToString();
			//if(0 <= minute && minute  <= 9)
			//	sminute = "0" + minute;
			//else
			//	sminute = minute.ToString();
			//if(0 <= second && second  <= 9)
			//	ssecond = "0" + second;
			//else
			//	ssecond = second.ToString();

			return shour + "时";
		}
		else
		{
			int day = seconds / (24 * 3600);
			return day * 24 + "天";
		}
	}

	#region 时间秒数转换成XX:XX:XX格式

	/// <summary>
	/// Secondses to string2.
	/// 时间秒数转换成XX:XX:XX格式
	/// 倒计时或者计时使用
	/// </summary>
	/// <returns>The to string2.</returns>
	/// <param name="seconds">Seconds.</param>
	public static string secondsToString2(int seconds)
	{
		int hour = Mathf.CeilToInt(seconds / 3600);
		int minute = Mathf.CeilToInt( seconds % 3600 / 60);
		int second = Mathf.CeilToInt( seconds % 3600 % 60);

		string shour = "";
		string sminute = "";
		string ssecond = "";

		if(0 <= hour  && hour  <= 9)
			shour = "0" + hour;
		else
			shour = hour.ToString();
		if(0 <= minute && minute  <= 9)
			sminute = "0" + minute;
		else
			sminute = minute.ToString();
		if(0 <= second && second  <= 9)
			ssecond = "0" + second;
		else
			ssecond = second.ToString();

		return shour + ":" + sminute + ":" + ssecond;
	}
	#endregion
    /// <summary>
    /// 根据秒数获取天数
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public static int SecondsToDay(long seconds)
    {
        int day = (int)seconds / 3600 / 24;
        return day;
    }

	#region 登陆时间判断
	/// <summary>
	/// Secondses to tips.
	/// 是否在线，登陆时间
	/// 离线
	/// xx年前
	/// xx个月前
	/// xx天前
	/// xx小时前
	/// xx分钟前
	/// 刚刚
	/// </summary>
	/// <returns>The to tips.</returns>
	/// <param name="seconds">Seconds.</param>
	public static string secondsToTips (int seconds)
	{
		int YEAR = 365*24*60*60;
		int MONTH = 30*24*60*60;
		int DAY = 24*60*60;
		int HOUR = 60*60;
		int MINUTE = 60;
		long timeGap = getNowTicket()-seconds;
		string timeStr="";
		if(seconds == 0)
		{
			timeStr = "离线";
		}
		else if(timeGap>YEAR)
		{
			timeStr = timeGap/YEAR + "年前";
		} else if(timeGap>MONTH)
		{
			timeStr = timeGap/MONTH + "个月前";
		} else if(timeGap>DAY)
		{
			timeStr = timeGap/DAY + "天前";
		} else if(timeGap>HOUR)
		{
			timeStr = timeGap/HOUR + "小时前";
		} else if(timeGap>MINUTE)
		{
			timeStr = timeGap/MINUTE + "分钟前";
		} else
		{
			timeStr = "刚刚";
		}
		return timeStr;
	}
	#endregion
}

