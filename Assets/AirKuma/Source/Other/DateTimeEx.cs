using System;
using System.Collections.Generic;
using System.Globalization;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace AirKuma.Chrono {

  public static class TimeEx {

    // todo: optimization
    public static float GetCurrentTime() {
#if UNITY_EDITOR
      return (float)EditorApplication.timeSinceStartup;
#else
      return Time.time;
#endif
    }

    public static float Now => GetCurrentTime();
  }

  public static class WorkingDayAwareDateCalculator {
    private struct WorkingDayInfoPerYear {

      public HashSet<(int month, int day)> workingDaysOnWeekend;
      public HashSet<(int month, int day)> holidaysOnNonWeekend;
    }

    public static bool IsOneOfFiveWorkingDayOfWeek(this DayOfWeek dow) {
      return dow != DayOfWeek.Saturday && dow != DayOfWeek.Sunday;
    }

    private static Dictionary<int, WorkingDayInfoPerYear> infoDict = new Dictionary<int, WorkingDayInfoPerYear> {
      { 2019, new WorkingDayInfoPerYear {
        workingDaysOnWeekend = new  HashSet<(int month, int day)>{ (1, 19), (2, 23), (10, 5) },
        holidaysOnNonWeekend = new  HashSet<(int month, int day)>{
            (1, 1),
            (2, 4), (2, 5), (2, 6), (2, 7), (2, 8),
            (2, 28), (3, 1), (4, 4), (4, 5), (6, 7), (9, 13), (10, 10), (10, 11)}
      } }
    };

    public static bool IsWorkingDay(this DateTime dateTime) {
      WorkingDayInfoPerYear info = infoDict[dateTime.Year];
      (int month, int day) md = dateTime.GetMonthDay();
      return info.workingDaysOnWeekend.Contains(md)
          || (dateTime.DayOfWeek.IsOneOfFiveWorkingDayOfWeek() && !info.holidaysOnNonWeekend.Contains(md));
    }

    public static DateTime AddWorkingDay(this DateTime dateTime, int numberOfWorkingDays) {
      for (int i = 0; i != numberOfWorkingDays; ++i) {
        do {
          dateTime = dateTime.AddDays((double)1);
        } while (!dateTime.IsWorkingDay());
      }
      return dateTime;
    }
  }

  public enum DateTimeFormat {
    YYYYMMDD,
    HHMMSS,
    YYYYMMDDHHMMSS,
    YMD,
  }

  public readonly struct Date {
    public readonly int year, month, day;

    public Date(int year, int month, int day) {
      this.year = year;
      this.month = month;
      this.day = day;
    }
    public DateTime DateTime => new DateTime(year, month, day);
    public static implicit operator DateTime(Date date) {
      return new DateTime(date.year, date.month, date.day);
    }
  }

  public static class DataTimeEx {

    //============================================================
    public static string ToRepr(this DateTime dateTime, DateTimeFormat format) {
      switch (format) {
        case DateTimeFormat.YYYYMMDD:
          return dateTime.ToYYYYMMDD();
        case DateTimeFormat.HHMMSS:
          return dateTime.ToHHMMSS();
        case DateTimeFormat.YYYYMMDDHHMMSS:
          return dateTime.ToYYYYMMDDHHMMSS();
        case DateTimeFormat.YMD:
          return dateTime.ToYMD();
        default:
          throw new Exception();
      }
    }

    public static string ToYYYYMMDD(this DateTime dateTime, char separtor = '/') {
      return $"{dateTime.Year}{separtor}{dateTime.Month.ToStringPadded(2)}{separtor}{dateTime.Day.ToStringPadded(2)}";
    }
    public static string ToHHMMSS(this DateTime dateTime, char separtor = ':') {
      return dateTime.Hour.ToStringPadded(2) + separtor + dateTime.Minute.ToStringPadded(2) + separtor + dateTime.Second.ToStringPadded(2);
    }
    public static string ToYYYYMMDDHHMMSS(this DateTime dateTime, char midSepartor = '-', char dateSepartor = '/', char timeSepartor = ':') {
      return dateTime.ToYYYYMMDD(dateSepartor) + midSepartor + dateTime.ToHHMMSS(timeSepartor);
    }
    public static string ToYMD(this DateTime dateTime, char separtor = '/') {
      return $"{dateTime.Year}{separtor}{dateTime.Month}{separtor}{dateTime.Day}";
    }
    //============================================================
    public static (int month, int day) GetMonthDay(this DateTime dateTime) {
      return (dateTime.Month, dateTime.Day);
    }
    //============================================================
    public static string ToCultureRepr(this DateTime date, Culure culture,
        bool useInternationalYearInsteadOfLocalYear = false,
        bool paddingAsTwoDigitsForMonthAndDay = false) {

      if (culture == Culure.Chinese) {
        if (useInternationalYearInsteadOfLocalYear) {
          if (paddingAsTwoDigitsForMonthAndDay)
            return $"{date.Year}年{date.Month.ToStringPadded(2)}月{date.Day.ToStringPadded(2)}日";
          else
            return $"{date.Year}年{date.Month}月{date.Day}日";
        } else {
          if (paddingAsTwoDigitsForMonthAndDay)
            throw new NotImplementedException();
          return $"{date.Year - 1911}年{date.Month}月{date.Day}日";
        }
      } else {
        if (paddingAsTwoDigitsForMonthAndDay)
          throw new NotImplementedException();
        return $"{date.Month.EnglishMonthName()} {date.Day}, {date.Year}";
      }
    }
    public static string EnglishMonthName(this int month) {
      // culture identifiers: https://msdn.microsoft.com/en-us/library/cc233982.aspx
      return new CultureInfo(0x0409).DateTimeFormat.GetMonthName(month);
      //return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
    }
    //============================================================
  }
}
