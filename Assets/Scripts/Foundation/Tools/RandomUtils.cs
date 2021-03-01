using UnityEngine;
using System.Collections;
using System;

public class RandomUtils 
{
	private static string RandomString = "0123456789ABCDEFGHIJKMLNOPQRSTUVWXYZ";
	private static System.Random Random 
	{ 
		get 
		{ 
			return new System.Random(DateTime.Now.Second);
		} 
	}

	#region public static string RandomString(int count) 产生count位随机字符
	/// <summary>
	/// 产生随机字符
	/// </summary>
	/// <returns>字符串</returns>
	public static string GetRandomString(int count)
	{
		string returnValue = string.Empty;
		if (count > 0)
		{
			for (int i = 0; i < count; i++)
			{
				int r = Random.Next(0, RandomString.Length - 1);
				returnValue += RandomString[r];
			}
		}
		return returnValue;
	}
	#endregion


	#region public string GetRandomCode(string allChar, int CodeCount) 从字符串里随机得到，规定个数的字符串.

	/// <summary>
	/// 从字符串里随机得到，规定个数的字符串.
	/// </summary>
	/// <param name="allChar"></param>
	/// <param name="CodeCount"></param>
	/// <returns></returns>
	public string GetRandomCode(string allChar, int CodeCount)
	{
		//string allChar = "1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,i,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z"; 
		string[] allCharArray = allChar.Split(',');
		string RandomCode = "";
		int temp = -1;
		System.Random rand = new System.Random();
		for (int i = 0; i < CodeCount; i++)
		{
			if (temp != -1)
			{
				rand = new System.Random(temp * i * ((int)DateTime.Now.Ticks));
			}

			int t = rand.Next(allCharArray.Length - 1);

			while (temp == t)
			{
				t = rand.Next(allCharArray.Length - 1);
			}

			temp = t;
			RandomCode += allCharArray[t];
		}
		return RandomCode;
	}

	#endregion

	#region public static int GetRandom(int minimum, int maximal)
	/// <summary>
	/// 产生随机数
	/// </summary>
	/// <param name="minimum">最小值</param>
	/// <param name="maximal">最大值</param>
	/// <returns>随机数</returns>
	public static int GetRandom(int minimum, int maximal)
	{
		return Random.Next(minimum, maximal);
	}
	#endregion


	#region 对一个数组进行随机排序
	/// <summary>
	/// 对一个数组进行随机排序
	/// </summary>
	/// <typeparam name="T">数组的类型</typeparam>
	/// <param name="arr">需要随机排序的数组</param>
	public void GetRandomArray<T>(T[] arr)
	{
		//对数组进行随机排序的算法:随机选择两个位置，将两个位置上的值交换

		//交换的次数,这里使用数组的长度作为交换次数
		int count = arr.Length;

		//开始交换
		for (int i = 0; i < count; i++)
		{
			//生成两个随机数位置
			int randomNum1 = GetRandomInt(0, arr.Length);
			int randomNum2 = GetRandomInt(0, arr.Length);

			//定义临时变量
			T temp;

			//交换两个随机数位置的值
			temp = arr[randomNum1];
			arr[randomNum1] = arr[randomNum2];
			arr[randomNum2] = temp;
		}
	}
	#endregion


	#region 生成一个随机整数
	/// <summary>
	/// 生成一个随机整数
	/// </summary>
	public static int GetRandomInt()
	{
		return Random.Next();
	}
	#endregion


	#region 生成一个不大于maxNum的随机整数
	/// <summary>
	/// 生成一个不大于maxNum的随机整数
	/// </summary>
	/// <param name="maxNum">最大值</param>
	public static int GetRandomInt(int maxNum)
	{
		return Random.Next(maxNum);
	}
	#endregion

	#region 生成一个指定范围的随机整数
	/// <summary>
	/// 生成一个指定范围的随机整数，该随机数范围包括最小值，但不包括最大值
	/// </summary>
	/// <param name="minNum">最小值</param>
	/// <param name="maxNum">最大值</param>
	public static int GetRandomInt(int minNum, int maxNum)
	{
		return Random.Next(minNum, maxNum);
	}
	#endregion


	#region 生成一个0.0到1.0的随机小数
	/// <summary>
	/// 生成一个0.0到1.0的随机小数
	/// </summary>
	public static double GetRandomDouble()
	{
		return Random.NextDouble();
	}
	#endregion

	#region 生成一个0.0到maxNum的随机小数
	/// <summary>
	/// 生成一个0.0到maxNum的随机小数
	/// </summary>
	public static double GetRandomDouble(double _maxNum)
	{
		return Random.NextDouble() * _maxNum;
	}
	#endregion

	#region 生成一个minNum到maxNum的随机小数
	/// <summary>
	/// 生成一个minNum到maxNum的随机小数
	/// </summary>
	public static double GetRandomDouble(double minNum, double maxNum)
	{
		return Random.NextDouble() * (maxNum - minNum) + minNum;
	}
	#endregion


	// 一：随机生成不重复数字字符串  
	private static int rep = 0;
	public string GenerateCheckCodeNum(int codeCount)
	{
		string str = string.Empty;
		long num2 = DateTime.Now.Ticks + rep;
		rep++;
		System.Random random = new System.Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
		for (int i = 0; i < codeCount; i++)
		{
			int num = random.Next();
			str = str + ((char)(0x30 + ((ushort)(num % 10)))).ToString();
		}
		return str;
	}

	#region public string GenerateCheckCode(int codeCount) 随机生成字符串（数字和字母混和）
	/// <summary>
	/// Generates the check code.
	/// 随机生成字符串（数字和字母混和）
	/// </summary>
	/// <returns>The check code.</returns>
	/// <param name="codeCount">Code count.</param>
	public static string GetCheckCode(int codeCount)
	{
		string str = string.Empty;
		long num2 = DateTime.Now.Ticks + rep;
		rep++;
		System.Random random = new System.Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
		for (int i = 0; i < codeCount; i++)
		{
			char ch;
			int num = random.Next();
			if ((num % 2) == 0)
			{
				ch = (char)(0x30 + ((ushort)(num % 10)));
			}
			else
			{
				ch = (char)(0x41 + ((ushort)(num % 0x1a)));
			}
			str = str + ch.ToString();
		}
		return str;
	}
	#endregion



}

