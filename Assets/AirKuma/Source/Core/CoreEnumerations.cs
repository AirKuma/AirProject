using System;

namespace AirKuma {

  [Flags]
  public enum LeftOrRight {
    None = 0,
    Left = 0x1,
    Right = 0x2,
  }

  [Flags]
  public enum Ward1D {
    None = 0,
    Forward = 0x1, Backward = 0x2,
  }

  [Flags]
  public enum Ward2D {
    None = 0,
    LeftWard = 0x1, Rightward = 0x2,
    Upward = 0x4, Downward = 0x8,
  }

  [Flags]
  public enum Ward3D {
    None = 0,
    Forward = 0x1, Backward = 0x2,
    LeftWard = 0x4, Rightward = 0x8,
    Upward = 0x10, Downward = 0x20,
  }

  public enum CrossDirection2D {
    Horizontal, Vertical
  }

  public enum CrossDirection3D {
    Horizontal, Vertical, Depth
  }

  // named as html's
  public enum TextAlignment {
    Center = 0, Left, Right
  }

  [Flags]
  public enum Axis2D {
    None = 0,
    X = 0x1,
    Y = 0x2,
  }

  [Flags]
  public enum Axis3D {
    None = 0,
    [IterableEnumVal]
    X = 0x1,
    [IterableEnumVal]
    Y = 0x2,
    [IterableEnumVal]
    Z = 0x4,
  }


  public enum NegOrPos {
    Neg,
    Pos
  }

  public enum EventPhase {
    Capturing,
    Bubbling
  }

  public enum FitMode {
    Shrink, //  uniform scale to make it inside the region
    Extented, // uniform scale to cover over the region
  }
  public enum CoverMode {
    Extented, // uniform scale to cover over the region
    Stretch, // non-uniform scale to make it precisely cover the region
  }

  public enum GameCameraType {
    Top, Front, TopDown, Isometric
  }

  public enum SingleLineOrMultiLine {
    SingleLine,
    MultiLine,
  }

  [Flags]
  public enum FaceSide {
    None = 0,
    [IterableEnumVal]
    Left = 0x1,
    [IterableEnumVal]
    Right = 0x2,
    [IterableEnumVal]
    Top = 0x4,
    [IterableEnumVal]
    Bottom = 0x8,
    [IterableEnumVal]
    Back = 0x10,
    [IterableEnumVal]
    Front = 0x20,
  }

  public enum NextOrPrev {
    Previous,
    Next
  }

  public enum Gender {
    Male,
    Female
  }

  public enum Culure {
    English,
    Chinese,
  }

  public enum SetAction {
    Toggle,
    Add,
    Remove,
  }
  public enum SelectionState {
    Selected,
    NotSelected,
  }
  public enum SelectionTargetMode {
    Individual,
    Continuous,
  }


  public static class CoreSemanticExtensions {



    public static string ToCultureRepr(this Gender gender, Culure culture) {
      if (culture == Culure.Chinese) {
        return gender == Gender.Male ? "男" : "女";
      } else {
        return gender == Gender.Male ? "Male" : "Female";
      }
    }

    public static CrossDirection2D Invert(this CrossDirection2D direction) {
      return direction == CrossDirection2D.Horizontal ? CrossDirection2D.Vertical : CrossDirection2D.Horizontal;
    }
    public static Axis2D Invert(this Axis2D axis) {
      return axis == Axis2D.X ? Axis2D.Y : Axis2D.X;
    }
    public static Axis2D ToAxis(this CrossDirection2D direction) {
      return direction == CrossDirection2D.Horizontal ? Axis2D.X : Axis2D.Y;
    }
    public static CrossDirection2D ToCrossDirection(this Axis2D axis) {
      return axis == Axis2D.X ? CrossDirection2D.Horizontal : CrossDirection2D.Vertical;

    }
  }

  public enum SliceType {
    Enumerated,
    Segmented,
    Ranged,
  }

  public enum RunningEnv {
    None = 0,

    Game, // for GameView or Built Game
    Scene, // for ScenView
    Editor, // for any user Editor Window derived from UserWindow

    ////SceneView = NonPlaying

    ////SceneView = InSceneView | Playing | NonPlaying,
    ////GameView = InGameView | Playing | NonPlaying,
    ////EditorWindow = InEditorWindow | Playing | NonPlaying,

    ////RuntimePlayer = InBuiltGame | Playing,
    ////EditorPlayer = InGameView | Playing,

    ////Game = InBuiltGame | InGameView | Playing,
    ////Scene = InBuiltGame | InSceneView | InGameView | Playing | NonPlaying,
  }

  public enum CommonFileCategory {
    Document, Image, Audio, Video
  }

  public enum CollectionAction {
    AddToCollection,
    ToggleInCollection,
    RemoveFromCollection,
    ClearCollection,
  }





}
