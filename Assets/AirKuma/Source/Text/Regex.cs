using System;
using System.Collections.Generic;
using System.Text;
using AirKuma.Geom;
using UnityEngine;

namespace AirKuma.Text {

  public static class RegexExtensions {
    public static bool IsMatchedWith(this string str, string regexPattern) {
      return System.Text.RegularExpressions.Regex.IsMatch(str, regexPattern);
    }
    public static System.Text.RegularExpressions.Match RegexMatchFirst(this string str, string regexPattern) {
      return System.Text.RegularExpressions.Regex.Match(str, regexPattern);
    }
    public static bool RegexMatchFirst(this string str, string regexPattern, out System.Text.RegularExpressions.Match match) {
      match = System.Text.RegularExpressions.Regex.Match(str, regexPattern);
      return match.Success;
    }
    public static System.Text.RegularExpressions.MatchCollection RegexMatchAll(this string str, string regexPattern) {
      return System.Text.RegularExpressions.Regex.Matches(str, regexPattern);
    }
    // replacement syntax: https://docs.microsoft.com/en-us/dotnet/standard/base-types/substitutions-in-regular-expressions
    public static string RegexReplace(this string str, string regexPattern, string replacement) {
      return System.Text.RegularExpressions.Regex.Replace(str, regexPattern, replacement);
    }
  }


  public static class ChrCategory {
    public static readonly Predicate<char> WhitespacePredicate = (char c) => c == ' ' || c == '\t' || c == '\n';
  }

  public enum QuoteType {
    None, Single, Double
  }
  public enum BracketType {
    None, Opening, Closing
  }
  public enum TextBoundaryMode {
    PlainerSpaced,
    PlainerCategorified,
    BalancedSpaced,
    BalancedCategorified
  }

  public enum BalancedTermType {
    Spaced,
    Bracketed,
    Quoted,
  }

  [Flags]
  public enum SeekMode {
    None = 0,
    MoveOneChar = 1,
    UntilNewline = 2, // unmoved if already reaching/beside of newline
    LiterallySpaced = 0x10,
    _PassingQuotationOnly = 0x20,
    _BracketBalancedOnly = 0x40,
    PassingQuotation = LiterallySpaced + _PassingQuotationOnly,
    BracketBalanced = LiterallySpaced + PassingQuotation + _BracketBalancedOnly,
  }

  public enum CaretMoveMode {
    OneChar,
    OneWord,
    OneBalance,
    ToHomeOrEnd,
    ToHomeOrEndOfLine,
  }


  public struct LiteralStatus {

    public static QuoteType GetQuoteType(char c) {
      if (c == '"') {
        return QuoteType.Double;
      } else if (c == '\'') {
        return QuoteType.Single;
      } else {
        throw new Exception();
      }
    }
    public static BracketType GetBracketType(char c) {
      switch (c) {
        case '(':
        case '[':
        case '{':
          return BracketType.Opening;
        case ')':
        case ']':
        case '}':
          return BracketType.Closing;
        default:
          throw new Exception();
      }
    }

    private int quotingLevel;
    private int bracketLevel;

    public bool Balanced => bracketLevel == 0;
    public bool Quoting => quotingLevel != 0;
    public bool Effective => Balanced && !Quoting;

    public QuoteType CurrentQuoteType {
      get {
        if (quotingLevel == 0) {
          return QuoteType.None;
        }

        return quotingLevel % 2 == 0 ? QuoteType.Double : QuoteType.Single;
      }
    }
    public void PushQuote(QuoteType quoteType) {
      if (quoteType == CurrentQuoteType) {
        quotingLevel -= 1;
      } else {
        quotingLevel += 1;
      }
    }
    public void PopQuote(QuoteType quoteType) {
      if (quoteType == CurrentQuoteType) {
        quotingLevel -= 1;
      } else {
        quotingLevel += 1;
      }
    }

    public void PushBracket(BracketType type) {
      if (quotingLevel == 0) {
        if (type == BracketType.Opening) {
          bracketLevel += 1;
        } else {
          bracketLevel -= 1;
        }
      }
    }
    public void PopBracket(BracketType type) {
      if (quotingLevel == 0) {
        if (type == BracketType.Opening) {
          bracketLevel -= 1;
        } else {
          bracketLevel += 1;
        }
      }
    }
  }


  public static class ParsingExtensions {



    public static BracketType GetBracketType(this char c) {
      return LiteralStatus.GetBracketType(c);
    }
    public static QuoteType GetQuoteType(this char c) {
      return LiteralStatus.GetQuoteType(c);
    }

    // finding is start from item at index of start
    // return the index of the found item
    // if not found => str.Length
    public static int FindFirst(this StringView str, System.Predicate<char> pred, int start = 0) {
      Debug.Assert(start.IsWithinRange(0, str.Length + 1));
      for (int i = start; i != str.Length; ++i) {
        if (pred(str[i])) {
          return i;
        }
      }
      return str.Length;
    }
    // finding is start from item at index of (start - 1), reversely
    // return the index of the found item plus 1
    // if not found => 0
    public static int FindLast(this StringView str, System.Predicate<char> pred, int start) {
      Debug.Assert(start.IsWithinRange(0, str.Length + 1));
      for (int i = start; i != 0; --i) {
        if (pred(str[i - 1])) {
          return i;
        }
      }
      return 0;
    }
    public static int FindLast(this StringView str, System.Predicate<char> pred) {
      return str.FindLast(pred, str.Length);
    }

    public static int NextSpace(this StringView str, int start) {
      for (int i = start; i != str.Length; ++i) {
        if (str[i] == ' ') {
          return i;
        }
      }
      return str.Length;
    }
    public static int NextNonSpace(this StringView str, int start) {
      for (int i = start; i != str.Length; ++i) {
        if (str[i] != ' ') {
          return i;
        }
      }
      return str.Length;
    }

    public static int FindFirst(this StringView str, char c, int start = 0) {
      return str.FindFirst((char chr) => chr == c, start);
    }
    public static int FindLast(this StringView str, char c, int start) {
      return str.FindLast((char chr) => chr == c, start);
    }
    public static int FindLast(this StringView str, char c) {
      return str.FindLast((char chr) => chr == c, str.Length);
    }

    //============================================================
    public static int GetFirstSpaceBoundary(this StringView str) {
      //return str.FindFirst(ChrCategory.WhitespacePredicate);
      return str.FindFirst(ChrCategory.WhitespacePredicate.InvertCondition());
    }
    public static int GetLastSpaceBoundary(this StringView str) {
      return str.FindLast(ChrCategory.WhitespacePredicate.InvertCondition());
    }
    public static StringView WithoutOutermostSpaces(this StringView str) {
      return str.GetSubView(str.GetFirstSpaceBoundary(), str.GetLastSpaceBoundary());
    }

    //============================================================
    public static IEnumerable<StringView> GetWords(this StringView str, Predicate<char> whitespacePred) {
      int p = str.GetFirstSpaceBoundary();
      int n = str.GetLastSpaceBoundary();
      // if all are whitespaces
      if (p > n) {
        yield break;
      }

      for (int i = p; i != n; ++i) {
        if (str[i] == ' ') {
          yield return str.GetSubView(p, i);
          p = i = str.FindFirst((char c) => c != ' ', i);
        }
      }
      yield return str.GetSubView(p, n);
    }
    public static IEnumerable<StringView> GetWords(this StringView str) {
      return str.GetWords(ChrCategory.WhitespacePredicate);
    }

    public static StringView GetWordAt(this StringView str, int wordIndex) {
      return str.GetWords().YieldItemAt(wordIndex);
    }

    //============================================================
    public static int BalanceBracket(this StringView str, int openingIndex) {
      char openingBracket = str[openingIndex];
      Debug.Assert(openingBracket.IsOpeningBracket());
      char closingBracket = openingBracket.CoBracket();
      int count = 0;
      for (int i = openingIndex; i != str.Length; ++i) {
        if (str[i] == openingBracket) {
          ++count;
        } else if (str[i] == closingBracket) {
          --count;
          if (count == 0) {
            return i;
          }
        }
      }
      return str.Length;
    }
    public static int BalanceQuote(this StringView str, int openingIndex) {
      char quoteChr = str[openingIndex];
      Debug.Assert(quoteChr.IsQuote());
      for (int i = openingIndex + 1; i != str.Length; ++i) {
        if (str[i] == quoteChr) {
          return i;
        }
      }
      return str.Length;
    }
    //------------------------------------------------------------
    // ???
    //public static int Balance(this StringView str, int indexOfOpeningBracketOrQuote) {
    //  var literalStatus = new LiteralStatus();
    //  for (int i = indexOfOpeningBracketOrQuote; i != str.Length; ++i) {
    //    if (str[i].IsBracket()) {
    //      literalStatus.PushBracket(str[i].GetBracketType());
    //      if (literalStatus.Effective) {
    //        return i + 1;
    //      }
    //    }
    //    else if (str[i].IsQuote()) {
    //      literalStatus.PushQuote(str[i].GetQuoteType());
    //      if (literalStatus.Effective) {
    //        return i + 1;
    //      }
    //    }
    //  }
    //  return str.Length;
    //}
    //============================================================

    public static StringView StripBoundaryWhitespaces(this StringView str) {
      int p = str.GetFirstSpaceBoundary();
      int n = str.GetLastSpaceBoundary();
      return str.GetSubView(p, n);
    }
    public static StringBuilder RemoveContinousWhitespaces(this StringBuilder str) {
      bool previousIsWhitespace = false;
      int j = 0;
      for (int i = 0; i != str.Length; ++i) {
        if (str[i].IsWhitespace()) {
          if (!previousIsWhitespace) {
            str[j++] = str[i];
          }
          previousIsWhitespace = true;
        } else {
          str[j++] = str[i];
          previousIsWhitespace = false;
        }
      }
      str.Length = j;
      return str;
    }

    public static char CoBracket(this char c) {
      switch (c) {
        case '(':
          return ')';
        case '[':
          return ']';
        case '{':
          return '}';
        case ')':
          return '(';
        case ']':
          return '[';
        case '}':
          return '{';
        default:
          throw new Exception();
      }
    }

    public static StringBuilder InsertOuterSpaces(this StringView str) {

      var builder = new StringBuilder();
      bool isQuoting = false;

      void EnsureLeftSpace(int i) {
        char c = str[(i - 1).ClampMin(0)];
        if (!c.IsWhitespace()) {
          builder.Append(' ');
        }
        builder.Append(str[i]);
      }
      void EnsureRightSpace(int i) {
        builder.Append(str[i]);
        char c = str[(i + 1).ClampMax(str.Length - 1)];
        if (!c.IsWhitespace()) {
          builder.Append(' ');
        }
      }

      for (int i = 0; i != str.Length; ++i) {
        if (!isQuoting) {
          if (str[i].IsOpeningBracket()) {
            EnsureLeftSpace(i);
          } else if (str[i].IsClosingBracket()) {
            EnsureRightSpace(i);
          } else if (str[i].IsQuote()) {
            EnsureLeftSpace(i);
            isQuoting = true;
          } else {
            builder.Append(str[i]);
          }
        } else {
          if (str[i].IsQuote()) {
            EnsureRightSpace(i);
            isQuoting = false;
          } else {
            builder.Append(str[i]);
          }
        }
      }

      return builder;
    }

    // delimitiers: ' " ( [ { ) ] } space
    // two continuous splitter can form a word boundary without presence of space
    // result of Bracketed or Quoted may not balanced or matched
    public static IEnumerable<(BalancedTermType termType, StringView termStr)> EachTerm(this StringView str) {
      int n = str.Length;
      int p, i;
      i = p = str.NextNonSpace(0);
      while (i != n) {
        char c = str[i];
        if (c == ' ') {
          yield return (BalancedTermType.Spaced, str.GetSubView(p, i));
          i = p = str.NextNonSpace(i);
        } else if (c.IsOpeningBracket()) {
          if (p != i) {
            yield return (BalancedTermType.Spaced, str.GetSubView(p, i));
            p = i;
          }
          if ((i = str.BalanceBracket(i)) == str.Length) {
            throw new Exception("bracket are not balanced");
          }
          yield return (BalancedTermType.Bracketed, str.GetSubView(p, i + 1));
          i = p = str.NextNonSpace(i + 1);
        } else if (c.IsQuote()) {
          if (p != i) {
            yield return (BalancedTermType.Spaced, str.GetSubView(p, i));
            p = i;
          }
          if ((i = str.BalanceQuote(i)) == str.Length) {
            throw new Exception("quote are not balanced");
          }

          yield return (BalancedTermType.Quoted, str.GetSubView(p, i + 1));
          i = p = str.NextNonSpace(i + 1);
        } else {
          ++i;
        }
      }
      if (i != p) {
        yield return (BalancedTermType.Spaced, str.GetSubView(p, i));
      }
    }
    //============================================================
    //------------------------------------------------------------
    //private static void TopDownTerms(AirTree<StringView>.Node node, StringView str) {
    //  foreach ((BalancedTermType termType, StringView termStr) in str.EachTerm()) {
    //    AirTree<StringView>.Node child = node.AddChild(termStr);
    //    if (termType == BalancedTermType.Bracketed) {
    //      TopDownTerms(child, termStr.GetSubView(+1, termStr.Length - 1));
    //    }
    //  }
    //}
    //public static AirTree<StringView> TopDownTerms(this StringView str) {
    //  var tree = new AirTree<StringView>(str);
    //  TopDownTerms(tree.root, str);
    //  return tree;
    //}
    //public static string TopDownTerms(this string str) {
    //  return str.GetFullView().TopDownTerms().ToString();
    //}
    //------------------------------------------------------------
    //private static void HeadTail(AirTree<StringView>.Node node, StringView str) {
    //  int count = 0;
    //  foreach ((BalancedTermType termType, StringView termStr) in str.EachTerm()) {
    //    if (count++ == 0) {
    //      node.Value = termStr;
    //    } else {
    //      AirTree<StringView>.Node child = node.AddChild(termStr);
    //      if (termType == BalancedTermType.Bracketed) {
    //        HeadTail(child, termStr.GetSubView(+1, termStr.Length - 1));
    //      }
    //    }
    //  }
    //}
    //public static AirTree<StringView> HeadTail(this StringView str) {
    //  var tree = new AirTree<StringView>(str);
    //  HeadTail(tree.root, str);
    //  return tree;
    //}
    //public static string HeadTail(this string str) {
    //  return str.GetFullView().HeadTail().ToString();
    //}
    //============================================================

    public static int Seek(this StringView str, int start, SeekMode mode, Ward1D ward) {
      str.ValidateIndex(start);
      return Mathf.Clamp(str.SeekWithoutClamping(start, mode, ward), 0, str.Length);
    }

    public static int CountNewlines(this StringView str, bool countsMissingEndNewline = false) {
      int c = 0;
      for (int i = 0; i != str.Length; ++i) {
        if (str[i] == '\n') {
          ++c;
        }
      }
      if (countsMissingEndNewline && str.Length > 0 && str[str.Length - 1] != '\n') {
        ++c;
      }
      return c;
    }

    //============================================================
    // todo: improve implementation

    // failed
    public static (int lower, int upper) LongestGap(this StringView str, char needle) {
      int p = 0;
      int maxLength = 0;
      (int lower, int upper) = (0, 0);
      for (int i = 0; i != str.Length; ++i) {
        if (str[i] == needle) {
          if (i - p > maxLength) {
            lower = p;
            upper = i;
          }
          p = i;
        }
      }
      if (str.Length - p > maxLength) {
        lower = p;
        upper = str.Length;
      }
      return (lower, upper);
    }
    public static string[] SplitLines(this StringView str) {
      return str.ToString().Split('\n');
    }
    public static string GetLongest(this string[] list) {
      int maxLength = 0;
      string longest = null;
      for (int i = 0; i != list.Length; ++i) {
        string item = list[i];
        if (item.Length > maxLength) {
          longest = item;
          maxLength = item.Length;
        }
      }
      return longest;
    }

    public static StringView LongestLine(this StringView str) {
      //(int lower, int upper) = str.LongestGap('\n');
      //return str.GetSubView(lower, upper);
      return str.SplitLines().GetLongest();
    }
    //============================================================

    private static int SeekWithoutClamping(this StringView str, int start, SeekMode mode, Ward1D ward) {
      if (mode == SeekMode.MoveOneChar) {
        return start + ward.GetOffset();
      } else if (mode == SeekMode.UntilNewline) {
        if (ward == Ward1D.Backward) {
          return str.FindLast('\n', start + 1);
        } else if (ward == Ward1D.Forward) {
          return str.FindFirst('\n', start);
        }
      } else {
        Debug.Assert((mode
          & (SeekMode.LiterallySpaced | SeekMode.PassingQuotation | SeekMode.BracketBalanced)) != 0);
        if (ward == Ward1D.Backward) {
          return str.SeekToSpaceBalanced(start, mode, -1, -1) - 1;
        } else if (ward == Ward1D.Forward) {
          return str.SeekToSpaceBalanced(start + 1, mode, 1, 0);
        }
      }
      throw new Exception();
    }
    private static int SeekToSpaceBalanced(this StringView str, int start, SeekMode mode, int step, int indexer_offset) {
      int bracketRank = 0;
      bool quotingFlag = false;
      int i = start;
      for (; i.IsWithinRange(0, str.Length); i += step) {
        char chr = str[i + indexer_offset];
        if ((mode & SeekMode.PassingQuotation).Flagged()) {
          if (chr == '\'') {
            quotingFlag = !quotingFlag;
          }
        }
        if (!quotingFlag) {
          if ((mode & SeekMode.BracketBalanced).Flagged()) {
            if (chr.IsOpeningBracket()) {
              bracketRank += 1;
            }
            if (chr.IsClosingBracket()) {
              bracketRank -= 1;
            }
            if (bracketRank == 0) {
              if (chr == ' ') {
                return i;
              }
            }
          }
        }
      }
      return i;
    }
  }
}

