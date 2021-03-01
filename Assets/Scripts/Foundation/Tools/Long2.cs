using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

namespace Foundation
{
    public struct Long2 : System.IEquatable<Long2>
    {
        public float x;
        public long y;     
        public bool Equals(Long2 other)
        {
            return x == other.x && y == other.y;
        }
        public override bool Equals(System.Object obj)
        {
            if (obj == null) return false;

            var rhs = (Long2)obj;

            return x == rhs.x &&
                   y == rhs.y;
        }
        public override int GetHashCode()
        {
            return (int)((int)x * 73856093 ^ y * 19349663);
        }
        public static Long2 zero { get { return new Long2(); } }
        public static Long2 one { get { return new Long2(1,0); } }
        //public Long2() { }
        public Long2(Long2 lhr)
        {
            this.x = lhr.x;
            this.y = lhr.y;
        }
        public Long2(int _x)
        {
            if(_x ==0)
            {
                x = y = 0;
            }
            else
            {
                y = (long)Math.Round(Math.Log10(Math.Abs(_x)) - 7); //最大单位
                x = (float)(_x / Math.Pow(10, y));
                Convert(ref x,ref y);
            }
            
        }
        public Long2(long _x)
        {
            if(_x == 0)
            {
                x = y = 0;
            }
            else
            {
                y = (long)Math.Round(Math.Log10(Math.Abs(_x)) - 7); //最大单位
                x = (float)(_x / Math.Pow(10, y));
                Convert(ref x, ref y);
            }
            
        }
        public Long2(float _x)
        {
            if(_x ==0)
            {
                x = y = 0;
            }
            else
            {
                y = (long)Math.Round(Math.Log10(Math.Abs(_x)) - 7); //最大单位
                x = (float)(_x / Math.Pow(10, y));
                Convert(ref x, ref y);
            }
            
        }
        public Long2(double _x)
        {
            if(_x ==0)
            {
                x = y = 0;
            }
            else
            {
                y = (long)Math.Round(Math.Log10(Math.Abs(_x)) - 7); //最大单位
                x = (float)(_x / Math.Pow(10, y));
                Convert(ref x, ref y);
            }
           
        }
        public Long2(int _x, int _y)
        {
            x = (float)_x;
            y = _y;
            Convert(ref x, ref y);
        }
        public Long2(long _x,long _y)
        {
            x = (float)_x;
            y = _y;
            Convert(ref x, ref y);
        }
        public Long2(float _x,long _y)
        {
            x = _x;
            y = _y;
            Convert(ref x, ref y);
        }
        public Long2(string str)
        {                                     
            string[] st = str.Split(',');
            long unit = 0;
            if (st.Length > 1)
                unit = long.Parse(st[1]);  
            try
            {
                float s = float.Parse(st[0]);
                if (s == 0)
                {
                    x = 0;
                    y = 0;
                }
                else
                {
                    long moreUnit = (long)Math.Round(Math.Log10(Math.Abs(s)) - 7);  //计算多余位
                    if (moreUnit < 0)
                    {
                        x = s;
                        y = unit;
                    }
                    else
                    {
                        x = (float)(s / Math.Pow(10, moreUnit));
                        y = unit + moreUnit;
                    }
                    if(y < 0)
                    {
                        x = (float)(x * Math.Pow(10, y));
                        y = 0;
                    }
                }      
                
            }
            catch(Exception ex)
            {
                x = y = 0;                                             
                Debug.LogError($"long {str}溢出{ex.Message}");
            }  
            
        }
        public static bool operator ==(Long2 lhs, Long2 rhs)
        {
            if (lhs.x == 0 && rhs.x == 0)
                return true;
            return lhs.x == rhs.x &&
                   lhs.y == rhs.y; 
        }

        public static bool operator !=(Long2 lhs, Long2 rhs)
        {
            return lhs.x != rhs.x ||
                   lhs.y != rhs.y; 
        }
        public static bool operator <(Long2 lhs, Long2 rhs)
        {
            Long2 l = lhs - rhs;
            if (l.x < 0) return true;
            else return false; 
                             
        }
        public static bool operator <=(Long2 lhs, Long2 rhs)
        {
            Long2 l = lhs - rhs;
            if (l.x <= 0) return true;
            else return false;
        }
        public static bool operator >(Long2 lhs, Long2 rhs)
        {

            Long2 l = lhs - rhs;
            if (l.x >0) return true;
            else return false;
        }
        public static bool operator >=(Long2 lhs, Long2 rhs)
        {
            Long2 l = lhs - rhs;
            if (l.x >= 0) return true;
            else return false;
        }
        public static Long2 operator -(Long2 lhs, Long2 rhs)
        {
            long unit = Math.Max(lhs.y,rhs.y); //最大单位
            lhs.x = (float)(lhs.x * Math.Pow(10, lhs.y - unit));
            lhs.y = unit;
            rhs.x = (float)(rhs.x * Math.Pow(10, rhs.y - unit));
            rhs.y = unit;
            lhs.x = lhs.x - rhs.x;
            lhs = Convert(lhs);
            return lhs;
        }
        public static Long2 operator -(Long2 lhs)
        {
            lhs.x = -lhs.x;
            lhs = Convert(lhs);
            return lhs;
        }
        public static Long2 operator +(Long2 lhs, Long2 rhs)
        {
            long unit = Math.Max(lhs.y, rhs.y); //最大单位
            lhs.x = (float)(lhs.x * Math.Pow(10, lhs.y - unit));
            lhs.y = unit;
            rhs.x = (float)(rhs.x * Math.Pow(10, rhs.y - unit));
            rhs.y = unit;
            lhs.x = lhs.x + rhs.x;
            float a = 57894212357.5679f;
            if (lhs.x == 0)
                return Long2.zero;
            long moreUnit = (long)Math.Round(Math.Log10(Math.Abs(lhs.x)) - 7);  //计算多余位
            lhs.x = (float)(lhs.x / Math.Pow(10, moreUnit));
            lhs.y += moreUnit;
            lhs = Convert(lhs);
            return lhs;
        }
        public static Long2 operator *(Long2 lhs, Long2 rhs)
        {
            lhs.x *= rhs.x;
            if (lhs.x == 0)
                return Long2.zero;
            long moreUnit = (long)Math.Round(Math.Log10(Math.Abs(lhs.x)) - 7);  //计算多余位
            lhs.x = (float)(lhs.x / Math.Pow(10, moreUnit));
            lhs.y = lhs.y + rhs.y + moreUnit;
            lhs = Convert(lhs);
            return lhs;
        }
        public static Long2 operator *(Long2 lhs, int rhs)
        {
            lhs.x *= rhs;
            if (lhs.x == 0)
                return Long2.zero;
            long moreUnit = (long)Math.Round((Math.Log10(Math.Abs(lhs.x)) - 7));  //计算多余位
            lhs.x = (float)(lhs.x / Math.Pow(10, moreUnit));
            lhs.y = lhs.y  + moreUnit;
            lhs = Convert(lhs);
            return lhs;
        }
        public static Long2 operator *(Long2 lhs, float rhs)
        {
            float hs = lhs.x * rhs;
            if (hs == 0)
                return Long2.zero;
            long moreUnit = (long)Math.Round((Math.Log10(Math.Abs(hs)) - 7));  //计算多余位
            lhs.x = (float)(hs / Math.Pow(10, moreUnit));
            lhs.y = lhs.y + moreUnit;
            lhs = Convert(lhs);
            return lhs;
        }
        public static Long2 operator *(Long2 lhs, double rhs)
        {
            float hs = (float)(lhs.x * rhs);
            if (hs == 0)
                return Long2.zero;
            long moreUnit = (long)Math.Round((Math.Log10(Math.Abs(hs)) - 7));  //计算多余位
            lhs.x = (float)(hs / Math.Pow(10, moreUnit));
            lhs.y = lhs.y + moreUnit;
            lhs = Convert(lhs);
            return lhs;
        }
        public static Long2 operator *(Long2 lhs, long rhs)
        {
            lhs.x *=  rhs;
            if (lhs.x == 0)
                return Long2.zero;
            long moreUnit = (long)Math.Round(Math.Log10(Math.Abs(lhs.x)) - 7);  //计算多余位
            lhs.x = (float)(lhs.x / Math.Pow(10, moreUnit));
            lhs.y = lhs.y + moreUnit;
            lhs = Convert(lhs);
            return lhs;
        }
        public static Long2 operator /(Long2 lhs, Long2 rhs)
        {
            if(rhs == Long2.zero)
            {
                Debug.LogError($"被除数为零");
                return lhs;
            }
            float hs =  lhs.x / rhs.x;
            if (hs == 0)
                return Long2.zero;
            long moreUnit = (long)Math.Round((Math.Log10(Math.Abs(hs)) - 7));  //计算多余位   
            lhs.x = (float)(hs / Math.Pow(10, moreUnit)); 
            lhs.y = lhs.y - rhs.y + moreUnit;
            lhs = Convert(lhs);
            return lhs; 
        }
        public static Long2 operator /(Long2 lhs, long rhs)
        {
            if (rhs == 0)
            {
                Debug.LogError($"被除数为零");
                return lhs;
            }
            float hs = lhs.x / rhs;
            if (hs == 0)
                return Long2.zero;
            long moreUnit = (long)Math.Round((Math.Log10(Math.Abs(hs)) - 7));  //计算多余位
            lhs.x = (float)(hs / Math.Pow(10, moreUnit));
            lhs.y = lhs.y + moreUnit;
            lhs = Convert(lhs);
            return lhs;
        }
        public static Long2 operator /(Long2 lhs, float rhs)
        {
            if (rhs == 0)
            {
                Debug.LogError($"被除数为零");
                return lhs;
            }
            float hs = lhs.x / rhs;
            if (hs == 0)
                return Long2.zero;
            long moreUnit = (long)Math.Round((Math.Log10(Math.Abs(hs)) - 7));  //计算多余位
            lhs.x = (float)(hs / Math.Pow(10, moreUnit));
            lhs.y = lhs.y + moreUnit;
            lhs = Convert(lhs);
            return lhs;
        }
        public static Long2 operator /(Long2 lhs, double rhs)
        {
            if (rhs == 0)
            {
                Debug.LogError($"被除数为零");
                return lhs;
            }
            float hs = (float)(lhs.x / rhs);
            if (hs == 0)
                return Long2.zero;
            long moreUnit = (long)Math.Round((Math.Log10(Math.Abs(hs)) - 7));  //计算多余位
            lhs.x = (float)(hs / Math.Pow(10, moreUnit));
            lhs.y = lhs.y + moreUnit;
            lhs = Convert(lhs);
            return lhs;
        }
        public static Long2 operator /(Long2 lhs, int rhs)
        {
            if (rhs == 0)
            {
                Debug.LogError($"被除数为零");
                return lhs;
            }
            float hs = lhs.x / rhs;
            if (hs == 0)
                return Long2.zero;
            long moreUnit = (long)Math.Round((Math.Log10(Math.Abs(hs)) - 7));  //计算多余位
            lhs.x = (float)(hs / Math.Pow(10, moreUnit));
            lhs.y = lhs.y + moreUnit;
            lhs = Convert(lhs);
            return lhs;
        }
        public object this[int i]
        {
            get
            {
                return i == 0 ? x : y;
            }
            set
            {
                if (i == 0) x = (float)value;
                else y = (long)value;   
            }
        }
        public static Long2 Convert(Long2 lhs)
        {
            long moreUnit = -7;
            if (lhs.x !=0)
                moreUnit = (long)Math.Round((Math.Log10(Math.Abs(lhs.x)) - 7));  //计算多余位
            lhs.x = (float)(lhs.x / Math.Pow(10, moreUnit));
            lhs.y = lhs.y + moreUnit;
            if (lhs.y < 0)
            {
                lhs.x = (float)(lhs.x * Math.Pow(10, lhs.y));
                lhs.y = 0;   
            }     
            return lhs;
        }
        public static void Convert(ref float x,ref long y)
        {
            long moreUnit = -7;
            if (x != 0)
                moreUnit = (long)Math.Round((Math.Log10(Math.Abs(x)) - 7));  //计算多余位
            x = (float)(x / Math.Pow(10, moreUnit));
            y = y + moreUnit;
            if (y < 0)
            {
                x = (float)(x * Math.Pow(10, y));
                y = 0;
            }
        }
        public static implicit operator string(Long2 obj)
        {
            return obj.ToString();
        } 

        /** Returns a nicely formatted string representing the vector */
        public override string ToString()
        {
            return x + "," + y ;
        } 
    }  
}


