using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AirKuma {

  public struct CharArr {
    private string internalStr;

    public bool Empty => internalStr is null || internalStr == "";

    public CharArr(string str) {
      internalStr = str ?? throw new ArgumentNullException(nameof(str));
    }

    public static implicit operator CharArr(string str) {
      return new CharArr(str);
    }
    public static implicit operator string(CharArr str) {
      return str.internalStr ?? "";
    }

    public override string ToString() {
      return internalStr ?? "";
    }
  }


  public static class ReprCache {

    static class CacheData<T> where T : IEquatable<T> {

      static UnorderedMap<T, string> map = new UnorderedMap<T, string>();

      static CacheData() { }

      public static string Get(T value) {
        if (!map.TryGetValue(value, out string str)) {
          str = value.ToString();
          map.Add(value, str);
        }
        return str;
      }
    }

    struct FloatReprKey : IEquatable<FloatReprKey> {

      float value;

      public FloatReprKey(float value) {
        this.value = (float)Math.Truncate(value * 1000f) / 1000f;
      }

      public bool Equals(FloatReprKey other) {
        return this.value == other.value;
      }

      public override int GetHashCode() {
        return this.value.GetHashCode();
      }

      public override string ToString() {
        return this.value.GetReprOfDotWith3Digits();
      }
    }

    public static string GetReprCache(this int value) {
      return CacheData<int>.Get(value);
    }
    public static string GetReprCache(this (int, int) value) {
      return CacheData<(int, int)>.Get(value);
    }

    public static string GetReprCache(this float value) {
      return CacheData<FloatReprKey>.Get(new FloatReprKey(value));
    }
    public static string GetReprCache(this (float v0, float v1) t) {
      return CacheData<(FloatReprKey, FloatReprKey)>.Get((new FloatReprKey(t.v0), new FloatReprKey(t.v1)));
    }

    //public static class EnumReprCache {
    //  static UnorderedMap<TypeKey, UnorderedMap<int, string>> map = new UnorderedMap<TypeKey, UnorderedMap<int, string>>();
    //  static EnumReprCache() {
    //  }
    //  public static string ToStringCached<TEnum>(this int enumVal) where TEnum : Enum {
    //    if (!map.TryGetValue(typeof(TEnum), out UnorderedMap<int, string> submap)) {
    //      submap = new UnorderedMap<int, string>();
    //      map.Add(typeof(TEnum), submap);
    //    }
    //    if (!submap.TryGetValue(enumVal, out string enumStr)) {
    //      enumStr = System.Enum.GetName(typeof(TEnum), enumVal);
    //      submap.Add(enumVal, enumStr);
    //    }
    //    return enumStr;
    //  }
    //}
    public static string GetReprCache<T>(this T enumVal) where T : System.Enum {
      return CacheData<EnumKey<T>>.Get(new EnumKey<T>(enumVal));
    }
  }


  public static class StringEx {

    public static string GetIndentationString(this int level) {
      var sb = new StringBuilder();
      for (int i = 0; i != level; ++i) {
        sb.Append('\t');
      }
      return sb.ToString();
    }

    public static string Reverse(this string str) {
      StringBuilder sb = new StringBuilder();
      for (int i = str.Length; i != 0; --i) {
        sb.Append(str[i - 1]);
      }
      return sb.ToString();
    }

    public static IEnumerable<string> StrippedSplit(this string str, char sep) {
      if (str.Length == 0)
        yield break;
      foreach (string term in str.Split(sep)) {
        if (term.Length != 0)
          yield return term;
      }
    }

    //============================================================
    public static string Repeat(this string str, int times) {
      var sb = new StringBuilder();
      for (int i = 0; i != times; ++i) {
        sb.Append(str);
      }
      return sb.ToString();
    }

    //============================================================
    public static string IncrComparisonOrdinal(this string str) {
      (str.Length > 0).Assert();
      return str.Substring(0, str.Length - 1).ConcateChr((str[str.Length - 1].Ord() + 1).Chr());
    }

    //============================================================
    public static string WithInsertedChr(this string str, int atIndex, char chr) {
      return str.Substring(0, atIndex) + chr + str.Substring(atIndex, str.Length - atIndex);
    }
    public static string WithDeletedChr(this string str, int deleteAtIndex, int deleteCount = 1) {
      return str.Substring(0, deleteAtIndex) + str.Substring(deleteAtIndex + deleteCount, str.Length - (deleteAtIndex + deleteCount));
    }

    //============================================================
    public static string ConcateChr(this string self, char other) {
      return self + other.ToString();
    }
    public static string Concate(this string self, string other) {
      return self + other;
    }


    //============================================================
    public static string PrettifyCamel(this string str) {
      var result = new StringBuilder();
      int i = 0;
      IEnumerable<string> words = str.GetIdentifierTerms();
      int n = words.Count();
      foreach (string word in words) {
        result.Append(word);
        if (++i != n) {
          result.Append(' ');
        }
      }
      return result.ToString();
    }
    public static string PrettifySnake(this string str) {
      return str.Replace('_', ' ');
    }
    public static string Prettify(this string str) {
      return str.PrettifySnake().PrettifyCamel();
    }
    //============================================================
    //public static string KumaFormat(this string str) {
    //  return str.GetFullView().HeadTail().ToString();
    //}

    //============================================================

    public static string ClarifyRawString(this string str) {
      var result = new StringBuilder();
      int i = 0;
      i = str.SkipWhile(i, (char c) => c.IsWhitespace());

      for (; i != str.Length;) {
        result.Append(str[i]);
        if (str[i] == '\n') {
          i = str.SkipWhile(i, (char c) => c.IsWhitespace());
        } else {
          ++i;
        }
      }
      return result.ToString();
    }

    //============================================================

    public static int SkipUntil(this string str, int index, Predicate<char> pred) {
      for (int i = index; i != str.Length; ++i) {
        if (pred(str[i])) {
          return i;
        }
      }
      return str.Length;
    }
    public static int SkipWhile(this string str, int index, Predicate<char> pred) {
      for (int i = index; i != str.Length; ++i) {
        if (!pred(str[i])) {
          return i;
        }
      }
      return str.Length;
    }

    public static StringView GetView(this string str, int begin, int end) {
      return new StringView(str, begin, end);
    }
    public static StringView GetFullView(this string str) {
      return str.GetView(0, str.Length);
    }
    //============================================================
    // Anonymous => Annyms
    // Position => Pstn
    public static string RemoveTailVowels(this string str) {
      if (str.Length == 0) {
        return "";
      }

      var sb = new StringBuilder();
      sb.Append(str[0]);
      for (int i = 1; i != str.Length; ++i) {
        if (!str[i].IsVowel()) {
          sb.Append(str[i]);
        }
      }
      return sb.ToString();
    }
    public static bool IsWithoutTailVowels(this string str) {
      for (int i = 1; i != str.Length; ++i) {
        if (str[i].IsVowel()) {
          return false;
        }
      }
      return true;
    }

    //============================================================

    public static int SeekCamelWord(this string str, int startIndex) {
      for (int i = startIndex + 1; i != str.Length; ++i) {
        if (str[i].IsAsciiUppercase()) {
          return i;
        }
      }
      return str.Length;
    }
    //============================================================
    public static int Seek(this string str, int start, Predicate<char> pred) {
      for (int i = start; i != str.Length; ++i) {
        if (pred(str[i])) {
          return i;
        }
      }
      return str.Length;
    }
    //============================================================
    public static string Concate(this IEnumerable<char> chrs) {
      var sb = new StringBuilder();
      foreach (char c in chrs) {
        sb.Append(c);
      }
      return sb.ToString();
    }
    public static string Concate(this IEnumerable<string> strs) {
      var sb = new StringBuilder();
      foreach (string str in strs) {
        sb.Append(str);
      }
      return sb.ToString();
    }
    public static string ConcateLine(this string self, string other) {
      return self + other + "\n";
    }
    //============================================================
    public static bool IsIdentifier(this string str) {
      return str.Length > 0
        && (str[0].IsAsciiLetter() || str[0] == '_')
        && str.IsEachThat((char c) => c.IsAsciiLetter() || c.IsDigit() || c == '_');
    }
    //------------------------------------------------------------
    public static int CountIdentifierTerms(this string str) {
      return str.GetIdentifierTerms().Count();
    }

    [AirTest("a")]
    [AirTest("AirKumaSays_that123_34aBCde_Fg_h")]
    public static IEnumerable<string> GetIdentifierTerms(this string identifier) {
      identifier.IsIdentifier().Assert();
      for (int i = 0; i != identifier.Length;) {
        if (identifier[i].IsAsciiLetter()) {
          int next = identifier.Seek(i + 1, (char c) => c.IsAsciiUppercase() || c.IsDigit() || c == '_');
          yield return identifier.Substring(i, next - i);
          i = next;
        } else if (identifier[i].IsDigit()) {
          int next = identifier.Seek(i + 1, (char c) => c.IsAsciiLetter() || c == '_');
          yield return identifier.Substring(i, next - i);
          i = next;
        } else if (identifier[i] == '_') {
          ++i;
        } else {
          throw new ArgumentException();
        }
      }
    }
    //------------------------------------------------------------
    public static IEnumerable<char> GetAcronymChars(this string str)
        => from word in str.GetIdentifierTerms() select word[0];
    public static IEnumerable<char> GetAcronym(this string str)
        => str.GetAcronymChars().Concate();
    //============================================================

    public static int CompareByOrdinal(this string left, string right) {
      return string.Compare(left, right, StringComparison.Ordinal);
    }

    public static string FirstLetterToUppercase(this string str) {
      return str[0].ToAsciiUppercase() + str.Substring(1, str.Length - 1);
    }

  }
  public static class CharEx {

    public static string Repeat(this char chr, int count) {
      var str = new StringBuilder();
      for (int i = 0; i != count; ++i) {
        str.Append(chr);
      }
      return str.ToString();
    }

    public static bool IsAsciiUppercase(this char c) {
      return 'A' <= c && c <= 'Z';
    }
    public static bool IsAsciiLowercase(this char c) {
      return 'a' <= c && c <= 'z';
    }
    public static bool IsAsciiLetter(this char c) {
      return c.IsAsciiLowercase() || c.IsAsciiUppercase();
    }
    public static bool IsAsciiChr(this char c) {
      return c.Ord() < 128;
    }
    //============================================================
    public static bool IsOpeningBracket(this char c) {
      return c == '(' || c == '[' || c == '{';
    }
    public static bool IsClosingBracket(this char c) {
      return c == ')' || c == ']' || c == '}';
    }
    public static bool IsBracket(this char c) {
      return c.IsOpeningBracket() || c.IsClosingBracket();
    }
    //============================================================
    public static bool IsQuote(this char c) {
      return c == '"' || c == '\'';
    }
    //============================================================
    public static bool IsWhitespace(this char c) {
      return c == ' ' || c == '\t' || c.IsNewlineChar();
    }
    public static bool IsNewlineChar(this char c) {
      return c == '\n' || c == '\r';
    }
    //============================================================
    public static char Chr(this int code) {
      return (char)code;
    }
    public static int Ord(this char c) {
      return (int)c;
    }
    //============================================================
    public static char ToAsciiUppercase(this char c) {
      return (c.Ord() + ('A'.Ord() - 'a'.Ord())).Chr();
    }
    //============================================================
    public static bool IsVowel(this char c) {
      return c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u';
    }
    //============================================================
    public static bool IsDigit(this char c) {
      return c.Ord() >= 48 && c.Ord() <= 57;
    }
    //============================================================
    public static bool IsPrintable(this char c) {
      return c.Ord() >= 32 && c.Ord() <= 126;
    }
    //============================================================
  }


  public class RichString {

    public static implicit operator string(RichString str) {
      return str.FullString;
    }

    //============================================================
    private StringBuilder stringBuilder = new StringBuilder();
    private string cacheAsString;
    public event Action<string> OnChange;
    public event Action<int, char> OnInsertChar;
    public event Action<int, string> OnInsertString;

    //============================================================
    public int Length { get { return stringBuilder.Length; } }

    public char this[int index] {
      get {
        return stringBuilder[index];
      }
      set {
        stringBuilder[index] = value;
      }
    }

    public StringView GetView(int start, int length) {
      return new StringView(FullString, start, length);
    }
    public StringView GetFullView() {
      string str = FullString;
      return new StringView(str, 0, str.Length);
    }

    //============================================================

    private void InvalidateCache() {
      cacheAsString = null;
    }

    private void EnsureUpdatingCache() {
      if (cacheAsString == null) {
        cacheAsString = stringBuilder.ToString();
      }
    }

    //============================================================
    public string FullString {
      get {
        EnsureUpdatingCache();
        return cacheAsString;
      }
    }
    public string Substring(int start, int length) {
      return stringBuilder.ToString(start, length);
    }

    //============================================================

    private void NotifyCharInsertion(int index, string str) {
      if (OnInsertChar != null) {
        for (int i = 0; i != str.Length; ++i) {
          OnInsertChar(index++, str[i]);
        }
      }
    }

    public void Append(char chr) {
      int index = stringBuilder.Length;
      int l = Length;
      stringBuilder.Append(chr);
      InvalidateCache();
      OnChange?.Invoke(FullString);
      OnInsertChar?.Invoke(index, chr);
      OnInsertString?.Invoke(index, chr.ToString());
    }

    public void Append(string str) {
      int index = stringBuilder.Length;
      int l = Length;
      stringBuilder.Append(str);
      InvalidateCache();
      OnChange?.Invoke(FullString);
      NotifyCharInsertion(index, str);
      OnInsertString?.Invoke(l, str);
    }
    public void Insert(int index, char c) {
      stringBuilder.Insert(index, c);
      OnChange?.Invoke(FullString);
      OnInsertChar?.Invoke(index, c);
      OnInsertString?.Invoke(index, c.ToString());
    }
    public void Insert(int index, string str) {
      stringBuilder.Insert(index, str);
      OnChange?.Invoke(FullString);
      NotifyCharInsertion(index, str);
      OnInsertString?.Invoke(index, str);
    }

    public void RemoveAt(int start, int length = 1) {
      Debug.Assert(start >= 0);
      Debug.Assert(start + length <= Length);
      stringBuilder.Remove(start, length);
      InvalidateCache();
      OnChange?.Invoke(FullString);
    }
    public void RemoveBack(int count = 1) {
      Debug.Assert(Length >= count);
      RemoveAt(Length - count, count);
      InvalidateCache();
      OnChange?.Invoke(FullString);
    }

    //public void Reinit() {
    //  stringBuilder.Clear();
    //  InvalidateCache();
    //  OnChange?.Invoke(FullString);
    //}
  }

  public readonly struct StringView : IDataView<char> {

    public readonly string Str;
    public int Begin { get; }
    public int End { get; }
    public int Length => End - Begin;

    public static StringView Empty(string str) {
      return new StringView(str, 0, 0);
    }

    public StringView(string str, int begin, int end) {
      Str = str ?? throw new ArgumentNullException(nameof(str));
      Begin = begin;
      End = end;
    }

    public void ValidateIndex(int index) {
      Debug.Assert(index.IsWithinRange(0, Length));
    }
    public void ValidiateViewIndex(int viewIndex) {
      Debug.Assert(viewIndex >= 0 && viewIndex <= Length);
    }

    public StringView Retarget(int begin, int end) {
      return new StringView(Str, begin, end);
    }
    public StringView Offset(int beginOffset, int endOffset) {
      return new StringView(Str, Begin + beginOffset, End + endOffset);
    }
    public StringView GetSubView(int subStart, int subEnd) {
      ValidiateViewIndex(subStart);
      ValidiateViewIndex(subEnd);
      //Debug.Assert(subStart >= 0 && subStart  <= Length);
      //Debug.Assert(subEnd >= 0 && subEnd <= Length);
      Debug.Assert(subStart <= subEnd);
      return new StringView(Str, Begin + subStart, Begin + subEnd);
    }

    public char this[int index] {
      get {
        ValidateIndex(index);
        return Str[Begin + index];
      }
    }

    public static implicit operator bool(StringView view) {
      return view.NullOrEmpty();
    }
    public static implicit operator string(StringView view) {
      return view.Str.Substring(view.Begin, view.Length);
    }
    public override string ToString() {
      return Str.Substring(Begin, Length);
    }
    public static implicit operator StringView(string str) {
      return new StringView(str, 0, str.Length);
    }

    public StringBuilder Builder {
      get {
        return new StringBuilder(ToString());
      }
    }

    public bool NullOrEmpty() {
      return Str == null || Begin == End;
    }

    public IEnumerator<char> GetEnumerator() {
      foreach (char c in ToString()) {
        yield return c;
      }
    }
    IEnumerator IEnumerable.GetEnumerator() {
      foreach (char c in ToString()) {
        yield return c;
      }
    }
  }
}
