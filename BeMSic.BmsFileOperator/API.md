## BmsFileOperator API

BMSファイルの操作に関するクラスを入れる。

# WavFileUnit

#WAVに関するデータ

* public int Num { get; set; }
    * #WAVインデックス

* public string Name { get; set; }
    * 音声ファイル名



# FileList

* public static class FileList

wavファイルと#WAVインデックスのリスト処理。
        

* public static List<WavFileUnit> GetWavsFullPath(string bms, string bmsDirectory)
  * 入力1：BMSテキスト
  * 入力2：wavファイルディレクトリ
  * 出力： #WAV一覧(wavは絶対パス)
  * 概要：#WAV一覧を取得(wavは絶対パス)

* public static List<WavFileUnit> GetWavsRelativePath(string bms)
  * 入力1：BMSテキスト
  * 出力： #WAV一覧(wavは絶対パス)
  * 概要：#WAV一覧を取得(wavは相対パス)


# BmsManager

*  static public class BmsManager


BMSコマンド行操作
        

* public static string ReplaceLineDefinition(string line, List<BmsReplace> replaces)
  * 入力1：bmsファイル内の1行
  * 入力2：置換テーブル
  * 出力： 置換後の行
  * MAIN行の#WAVインデックスをreplacesで置換する

* public static string OffsettedLineDefinition(string line, List<int> wavs, int offset)
  * 入力1：bmsファイル内の1行
  * 入力2： 使用されている#WAV定義一覧
  * 入力3： ずらす数
  * 出力： 置換後の行
  * line内の#WAVインデックスをoffsetの分加算する

* public static string GetArrangedLine(string line, List<int> nowWavs)
  * 入力1：bmsファイル内の1行
  * 入力2：使用されている#WAV定義一覧
  * 出力： 置換後のbmsファイル内の1行
  * 概要：入力行の#WAVインデックスの未使用定義を詰める。

* public static List<int> GetLineDefinition(string line)
  * 入力1：bmsファイル内の1行
  * 出力： 使用されている#WAV定義一覧
  * 概要：MAIN行1行に含まれる#WAV定義一覧を返す



## BmsDefinitionReplace

*  public static class BmsDefinitionReplace


BMS定義の置換に関する処理。

* public static string GetReplacedBmsFile(string bmsFileText, List<BmsReplace> replaces)
  * 入力1：bmsファイルテキスト(置換前)
  * 入力2：置換テーブル
  * 出力： bmsファイルテキスト(置換後)
  * MAIN行の#WAVをreplacesで置換したBMSテキストを取得

* public static string GetUnusedWavDeletedBmsFile(string bmsFileText)
  * 入力1：bmsファイルテキスト(未使用#WAV定義削除前)
  * 出力： bmsファイルテキスト(未使用#WAV定義削除後)
  * MAIN行で使用されていない#WAV定義を削除する

* public static string GetWavArrangedBmsFile(string bmsFileText)
  * 入力1：bmsファイルテキスト(未使用#WAV定義を詰める前)
  * 出力： bmsファイルテキスト(未使用#WAV定義を詰めた後)
  * #WAV定義を01から順に詰める
  
* public static string GetOffsetedBmsFile(string bmsFileText, int offset)
  * 入力1：bmsファイルテキスト(置換前)
  * 入力2： ずらす数
  * 出力： bmsファイルテキスト(置換後)
  * BMSテキストの#WAV番号をoffsetだけ後ろにずらす

* public static List<int> GetUsedWavList(string bmsFileText)
  * 入力1：bmsファイルテキスト(未使用#WAV定義削除前)
  * 出力： bmsファイル内で使用されている#WAV定義一覧
  * bmsファイル内で使用されている#WAV定義一覧を取得する
  
* public static string GetMargedBms(string bms1, string bms2)
  * 入力1：bmsファイルテキスト1
  * 入力2：bmsファイルテキスト2
  * 出力： bmsファイルテキスト(マージ後)
  * BMS1にBMS2を追加する

* public static string GetReplacedArrangedData(string bmsFileText, List<int> uniqueList)
  * 入力1：bmsファイルテキスト(置換前)
  * 入力2：使用されている#WAV定義一覧
  * 出力： bmsファイルテキスト(置換後)
  * 概要：#WAVインデックスを前詰めしたBMSデータを取得する
