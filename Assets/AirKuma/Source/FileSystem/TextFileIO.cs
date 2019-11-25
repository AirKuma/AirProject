using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AirKuma.FileSys {

  public struct FileProxy {
    public readonly string path;
    public FileProxy(string path) {
      this.path = path;
    }
    public string LoadText() {
      return path.LoadTextFromFile();
    }
  }

  public static class TextFileIO {

    private static UTF8Encoding UTF8WithoutBOM => new UTF8Encoding(false, true);

    public static void SaveTextToFile(this string textToSave, string filePath) {
      System.IO.File.WriteAllText(filePath, textToSave, UTF8WithoutBOM);
    }
    public static string LoadTextFromFile(this string filePath) {
      return System.IO.File.ReadAllText(filePath, UTF8WithoutBOM);
    }

    //------------------------------------------------------------
    private const string AirTextIOFilePath = "AirTextIO.txt";
    public static void AirOutputText(this string textToSave) {
      if ("AirTextIO.txt".IsExistingPath()) {
        AirTextIOFilePath.AirBackup();
      }

      textToSave.SaveTextToFile(AirTextIOFilePath);
    }
    public static string AirInputText() {
      return AirTextIOFilePath.LoadTextFromFile();
    }
  }
}
