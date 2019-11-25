using UnityEditor;

namespace AirKuma.UnityCore {

#if !UNITY_TVOS
  public static class ClipboardEx {
    public static void CopyToClipboard(this string content) {
#if UNITY_EDITOR
      EditorGUIUtility.systemCopyBuffer = content;
#else
      GUIUtility.systemCopyBuffer = content;
#endif
    }
    public static void PasteFromClipboard(this string content) {
#if UNITY_EDITOR
      content = EditorGUIUtility.systemCopyBuffer;
#else
      content = GUIUtility.systemCopyBuffer;
#endif
    }
  }
#endif
}
