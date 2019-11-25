using UnityEngine;
using System;
using System.Collections.Generic;

namespace AirKuma {

  public struct HsvColor {

    public float hue;
    public float saturation;
    public float value;

    public HsvColor(float hue, float saturation, float value) {
      this.hue = hue;
      this.saturation = saturation;
      this.value = value;
    }

    public Color GetRgb() {
      return (Color)this;
    }

    public static implicit operator Color(HsvColor hsv) => Color.HSVToRGB(hsv.hue, hsv.saturation, hsv.value);
    public static implicit operator HsvColor(Color color) {
      var hsv = new HsvColor();
      Color.RGBToHSV(color, out hsv.hue, out hsv.saturation, out hsv.value);
      return hsv;
    }
  }

  public static class ColorEx {

    public static Color GrayScale(float lightness) {
      return new Color(lightness, lightness, lightness, 1.0f);
    }

    public static Color Transparent => new Color(0, 0, 0, 0);

    public static Color OfRandomHue(int? seed = null, float saturation = 1.0f, float value = 1.0f) {
      return new HsvColor(FloatEx.Random(seed), saturation, value).GetRgb();
    }

    public static Color Light(this Color color) {
      return color.WithValue(FloatEx.ThreeForths);
    }

    public static Color Dark(this Color color) {
      return color.WithValue(FloatEx.OneForth);
    }

    public static Color MidValue(this Color color) {
      return color.WithValue(FloatEx.Half);
    }

    public static Color LerpWith(this Color self, Color other, float progress) {
      return Color.Lerp(self, other, progress);
    }

    public static Color WithValue(this Color self, float value) {
      HsvColor h = self.GetHsv();
      h.value = value;
      return h.GetRgb();
    }
    public static Color WithSaturation(this Color self, float saturation) {
      HsvColor h = self.GetHsv();
      h.saturation = saturation;
      return h.GetRgb();
    }
    public static Color WithHue(this Color self, float hue) {
      HsvColor h = self.GetHsv();
      h.hue = hue;
      return h.GetRgb();
    }

    public enum ColorHexFormat {
      RRGGBB,
      RRGGBBAA,
    }

    public static HsvColor GetHsv(this Color color) {
      return (HsvColor)color;
    }

    //============================================================
    // supported color names: red, cyan, blue, darkblue, lightblue, purple, yellow, lime, fuchsia, white, silver, grey, black, orange, brown, maroon, green, olive, navy, teal, aqua, magenta..
    // hex format
    //# RGB (becomes RRGGBB)
    //# RRGGBB
    //# RGBA (becomes RRGGBBAA)
    //# RRGGBBAA
    public static Color ToColor(this string hexOrName) {
      if (ColorUtility.TryParseHtmlString(hexOrName, out Color color)) {
        return color;
      }

      throw new Exception();
    }
    public static Color GetNamedColor(this string colorName) {
      colorName.StartsWith("#").Not().Assert();
      return colorName.ToColor();
    }
    public static Color GetColorByHex(this string hex) {
      hex.StartsWith("#").Assert();
      (hex.Length.Equals(7) || hex.Length.Equals(9)).Assert();
      return hex.ToColor();
    }

    //============================================================
    public static string ToHex(this Color color, ColorHexFormat format) {
      if (format == ColorHexFormat.RRGGBB) {
        return ColorUtility.ToHtmlStringRGB(color);
      } else if (format == ColorHexFormat.RRGGBBAA) {
        return ColorUtility.ToHtmlStringRGBA(color);
      }

      throw new Exception();
    }

    //============================================================
    #region named colors

    public static readonly Color White = new Color(1.0f, 1.0f, 1.0f);
    public static readonly Color Black = new Color(0.0f, 0.0f, 0.0f);

    public static readonly Color VeryLightGrey = new Color(0.875f, 0.875f, 0.875f);
    public static readonly Color LightGrey = new Color(0.75f, 0.75f, 0.75f);
    public static readonly Color Grey = new Color(0.5f, 0.5f, 0.5f);
    public static readonly Color DarkGrey = new Color(0.25f, 0.25f, 0.25f);
    public static readonly Color VeryDarkGrey = new Color(0.125f, 0.125f, 0.125f);

    public static readonly Color Red = new Color(1.0f, 0.0f, 0.0f);
    public static readonly Color Green = new Color(0.0f, 1.0f, 0.0f);
    public static readonly Color Blue = new Color(0.0f, 0.0f, 1.0f);

    public static readonly Color Cyan = new Color(0.0f, 1.0f, 1.0f);
    public static readonly Color Magenta = new Color(1.0f, 0.0f, 1.0f);
    public static readonly Color Yellow = new Color(1.0f, 1.0f, 1.0f);

    public static Color Purple => nameof(Purple).GetNamedColor();
    public static Color Lime => nameof(Lime).GetNamedColor();
    public static Color Fuchsia => nameof(Fuchsia).GetNamedColor();
    public static Color Orange => nameof(Orange).GetNamedColor();
    public static Color Brown => nameof(Brown).GetNamedColor();
    public static Color Olive => nameof(Olive).GetNamedColor();
    public static Color Navy => nameof(Navy).GetNamedColor();
    public static Color Teal => nameof(Teal).GetNamedColor();
    public static Color Aqua => nameof(Aqua).GetNamedColor();

    #endregion
    //============================================================

    public static IEnumerable<(string, Color)> CoreColors {
      get {
        yield return (nameof(White), White);
        yield return (nameof(Black), Black);
        yield return (nameof(Grey), Grey);
        yield return (nameof(Red), Red);
        yield return (nameof(Green), Green);
        yield return (nameof(Blue), Blue);
        yield return (nameof(Cyan), Cyan);
        yield return (nameof(Magenta), Magenta);
        yield return (nameof(Yellow), Yellow);
      }
    }
  }
}

