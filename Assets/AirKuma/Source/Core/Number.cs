using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AirKuma {

  public enum OddEven {
    Odd,
    Even
  }

  public static class IntEx {

    public static int DigitCount(this int number) {
      return number.Log(10).FloorToInt() + 1;
    }
    

    public static int Random(int inclusiveLowerBound, int exclusiveUpperBound) {
      return UnityEngine.Random.Range(inclusiveLowerBound, exclusiveUpperBound);
    }
    public static float Random(int? seed, int inclusiveLowerBound, int exclusiveUpperBound) {
      if (seed.HasValue)
        return (float)new System.Random(seed.Value).Next(inclusiveLowerBound, exclusiveUpperBound);
      else
        return Random(inclusiveLowerBound, exclusiveUpperBound);
    }

    public static bool IsNegPosOne(this int num) {
      return num == -1 || num == 1;
    }
    public static string ToHexString(this int num) {
      return num.ToString("X");
    }

    public static string ToStringPadded(this int num, int paddingCount) {
      return num.ToString().PadLeft(paddingCount, '0');
    }
  }
  public static class FloatEx {

    public static string GetReprOfDotWith3Digits(this float num) {
      return num.ToString(".000");
    }

    //============================================================
    public static float Random() {
      return UnityEngine.Random.value;
      //return (float)new System.Random().NextDouble();
    }
    public static float Random(int? seed) {
      if (seed.HasValue)
        return (float)new System.Random(seed.Value).NextDouble();
      else
        return Random();
    }

    //============================================================
    #region constants
    //------------------------------------------------------------
    public const float Half = 0.5f;

    public const float OneForth = 0.25f;
    public const float ThreeForths = 0.75f;

    public const float OneEighth = 0.125f;
    public const float ThreeEighths = 0.375f;
    public const float FiveEighths = 0.625f;
    public const float SevenEighths = 0.875f;

    public const float OneSixteenth = 0.0625f;

    //------------------------------------------------------------
    public const float Sqrt2 = 1.41421356237f;
    public const float Sqrt3 = 1.73205080757f;
    public const float Sqrt5 = 2.2360679775f;

    #endregion
    //============================================================
  }

  public static class NumericExtensions {

    //============================================================
    public static int ToInt(this float num) {
      return (int)num;
    }
    public static float ToFloat(this double num) {
      // todo: checking overflow
      return (float)num;
    }
    public static float ToFloat(this int num) {
      return (float)num;
    }
    public static double ToDouble(this float num) {
      return num;
    }
    public static string GetStringRepr(this int num) {
      return num.ToString();
    }
    public static string StringRepr(this float num) {
      return num.ToString();
    }
    public static string EnglishOrdinal(this int number) {
      Debug.Assert(number >= 1);
      if (number == 1) {
        return "1st";
      } else if (number == 2) {
        return "2nd";
      } else if (number == 3) {
        return "3rd";
      } else {
        return number.ToString() + "th";
      }
    }

    public static string GetChineseReprFromOneToNine(this int num) {
      switch (num % 10) {
        case 1:
          return "一";
        case 2:
          return "二";
        case 3:
          return "三";
        case 4:
          return "四";
        case 5:
          return "五";
        case 6:
          return "六";
        case 7:
          return "七";
        case 8:
          return "八";
        case 9:
          return "九";
        default:
          throw new Exception();
      }
    }
    public static string GetChineseRepr(this int num) {
      Debug.Assert(num >= 0 && num < 100);
      if (num == 0) {
        return "零";
      }
      var str = new StringBuilder();
      if (num >= 10) {
        if (num / 10 != 1) {
          str.Append((num / 10).GetChineseReprFromOneToNine());
        }

        str.Append("十");
      }
      if (num % 10 > 0 && num % 10 < 10) {
        str.Append((num % 10).GetChineseReprFromOneToNine());
      }
      return str.ToString();
    }
    //------------------------------------------------------------
    public static int ParseAsInt(this string str) {
      if (Int32.TryParse(str, out int r)) {
        return r;
      }
      throw new Exception();
    }
    public static float ParseAsFloat(this string str) {
      if (Single.TryParse(str, out float r)) {
        return r;
      }
      throw new Exception();
    }
    public static bool TryParse(this string str, out int r) {
      if (Int32.TryParse(str, out r)) {
        return true;
      }
      return false;
    }
    public static bool TryParse(this string str, out float r) {
      if (Single.TryParse(str, out r)) {
        return true;
      }
      return false;
    }
    //============================================================
    //inline size_t minimal_power_of_2(size_t min) {
    //  must(min > 0);
    //  size_t given = 1;
    //  while (given < min) given <<= 1;
    //  return given;
    //}
    public static int GetCeilOfPowerOf2(this int v) {
      if (v <= 0)
        throw new ArgumentException();
      v--;
      v |= v >> 1;
      v |= v >> 2;
      v |= v >> 4;
      v |= v >> 8;
      v |= v >> 16;
      v++;
      return v;
    }
    //============================================================
    public static bool LT(this int self, int other) {
      return self < other;
    }
    public static bool LE(this int self, int other) {
      return self <= other;
    }
    public static bool GT(this int self, int other) {
      return self > other;
    }
    public static bool GE(this int self, int other) {
      return self >= other;
    }
    //------------------------------------------------------------
    public static bool ApproxEqualTo(this float self, float other, float tolerance = 0.001f) {
      return self + tolerance > other && self - tolerance < other;
    }
    //------------------------------------------------------------
    public static bool IsWithinRange(this int value, int min, int max) {
      return value >= min && value < max;
    }
    public static bool IsWithinRange(this float value, float min, float max) {
      return value >= min && value < max;
    }
    //============================================================
    public static int GetOneIfZero(this int value) {
      return value == 0 ? 1 : value;
    }
    //============================================================
    //------------------------------------------------------------
    public static int Max(this int left, int right) {
      return Math.Max(left, right);
    }
    public static int Min(this int left, int right) {
      return Math.Min(left, right);
    }
    public static int Average(this int left, int right) {
      return (left + right) / 2;
    }
    //------------------------------------------------------------
    public static float Max(this float left, float right) {
      return Math.Max(left, right);
    }
    public static float Min(this float left, float right) {
      return Math.Min(left, right);
    }
    public static float Average(this float left, float right) {
      return (left + right) / 2;
    }
    //============================================================
    public static (int, int) GetOrderedPairWith(this int left, int right) {
      if (left < right) {
        return (left, right);
      } else {
        return (right, left);
      }
    }
    //============================================================
    public static float PrograssBewteen(this float value, float min, float max) {
      return (value - min) / (max - min);
    }
    //============================================================
    public static float Sqrt(this float num) {
      return Math.Sqrt(num).ToFloat();
    }
    public static float Pow(this float num, float exp) {
      return Math.Pow(num, exp).ToFloat();
    }
    public static float Pow2(this float num) {
      return num.Pow(2);
    }
    public static int Pow(this int num, int exp) {
      return (int)Math.Pow((double)num, (double)exp);
    }
    //============================================================
    public static float Sin(this float num) {
      return Math.Sin(num).ToFloat();
    }
    public static float Cos(this float num) {
      return Math.Cos(num).ToFloat();
    }
    public static float Tan(this float num) {
      return Math.Tan(num).ToFloat();
    }
    //============================================================
    public static float Log(this int number, int logBase) {
      return (int)Mathf.Log(number, logBase);
    }
    public static float Log(this float number, float logBase) {
      return Mathf.Log(number, logBase);
    }
    //============================================================
    //------------------------------------------------------------
    public static int Clamp(this int num, int min, int max) {
      return num < min ? min : (num > max ? max : num);
    }
    public static int ClampMin(this int num, int min) {
      return num < min ? min : num;
    }
    public static int ClampMax(this int num, int max) {
      return num > max ? max : num;
    }
    //------------------------------------------------------------
    public static float Clamp(this float num, float min, float max) {
      return num < min ? min : (num > max ? max : num);
    }
    public static float ClampMin(this float num, float min) {
      return num < min ? min : num;
    }
    public static float ClampMax(this float num, float max) {
      return num > max ? max : num;
    }
    //============================================================
    public static bool IsFraction(this float value) {
      return value >= 0.0f && value < 1.0f;
    }
    public static float ClampFraction(this float num) {
      return num.Clamp(0.0f, 1.0f);
    }
    public static float OffsetFraction(this float num, float offset) {
      return (num + offset).ClampFraction();
    }
    public static bool TryOffsetFraction(this float num, float offset, out float n) {
      bool r = (num + offset).IsFraction();
      if (r) {
        n = num.OffsetFraction(offset);
      } else
        n = num;
      return r;
    }
    //============================================================
    public static int Abs(this int num) {
      return Math.Abs(num);
    }
    public static float Abs(this float num) {
      return Math.Abs(num);
    }
    //============================================================
    public static float Round(this float num) {
      return Math.Round(num).ToFloat();
    }
    public static int RoundToInt(this float num) {
      return (int)num.Round();
    }
    public static float Ceil(this float num) {
      return Math.Ceiling(num).ToFloat();
    }
    public static int CeilToInt(this float num) {
      return (int)num.Ceil();
    }
    public static float Floor(this float num) {
      return Math.Floor(num).ToFloat();
    }
    public static int FloorToInt(this float num) {
      return (int)num.Floor();
    }
    public static float HalfFloor(this float value) {
      return (value).Floor() + 0.5f;
    }
    //============================================================
    public static bool IsOdd(this int num) {
      return num % 2 == 1;
    }
    public static bool IsEven(this int num) {
      return num % 2 == 0;
    }
    public static OddEven GetOddEven(this int num) {
      return num % 2 == 0 ? OddEven.Even : OddEven.Odd;
    }
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //============================================================
    public static int Neg(this int self) {
      return -self;
    }
    public static int Add(this int self, int other) {
      return self + other;
    }
    public static int Sub(this int self, int other) {
      return self - other;
    }
    public static int Mul(this int self, int other) {
      return self * other;
    }
    public static int Div(this int self, int other) {
      return self / other;
    }
    public static int Mod(this int self, int other) {
      return self % other;
    }
    //============================================================
    public static float Neg(this float self) {
      return -self;
    }
    public static float Add(this float self, float other) {
      return self + other;
    }
    public static float Sub(this float self, float other) {
      return self - other;
    }
    public static float Mul(this float self, float other) {
      return self * other;
    }
    public static float Div(this float self, float other) {
      return self / other;
    }
    public static float Mod(this float self, float other) {
      return self % other;
    }
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //============================================================
    public static float HalfInvert(this float rad) {
      (rad >= 0.0f && rad <= Mathf.PI / 2.0f).Assert();
      return Mathf.PI / 2.0f - rad;
    }
    //============================================================
    public static bool IsPowerOfTwo(this int x) {
      return (x != 0) && x.IsPowerOfTwoOrZero();
    }
    public static bool IsPowerOfTwoOrZero(this int x) {
      return ((x & (x - 1)) == 0);
    }
  }
}
