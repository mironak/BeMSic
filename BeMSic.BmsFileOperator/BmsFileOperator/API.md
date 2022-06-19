## BmsFileOperator API

BMSファイルの操作に関するクラスを入れる。

# WavFileUnit

#WAVに関するデータ

* public int Num { get; set; }
    * #WAVインデックス

* public string Name { get; set; }
    * 音声ファイル名



# FileList

wavファイルと#WAVインデックスのリスト処理。

* public static WavFileUnit[] CreateFileList(string bmsFilePath)
  * 入力1：bmsファイルパス
  * 出力： #WAV一覧
  * 概要：bmsファイルから#WAVのリストを取得する。

* public static WavFileUnit[] GetPartialWavList(WavFileUnit[] fileList, int start, int end)
  * 入力1：#WAV一覧
  * 入力2：開始インデックス
  * 入力3：最終インデックス
  * 出力： #WAV一覧
  * 概要：#WAV一覧からインデックスがstartからendの間の#WAV一覧を取得する。
  * ex) Num:[01 02 03 04 05] -> if start = 02, end = 04 => Num:[02 03 04]


# BmsManager

BMSコマンド行操作

* public enum BmsCommand
  * 概要：BMSコマンド列挙体
  * NONE: 置換対象外
  * WAV: #WAV
  * MAIN: 譜面レーン

* public static BmsCommand GetLineCommand(string line)
  * 入力1：bmsファイル内の1行
  * 出力： BMSコマンド
  * bmsファイルの行テキストから、行のコマンドを取得する。

* public static (string, string) GetWavData(string line)
  * 入力1：bmsファイル内の1行
  * 出力1： #WAVインデックス
  * 出力2： 音声ファイル名
  * #WAVコマンド行から、#WAVインデックスとファイル名を取得する。
  * "#WAV01 a.wav" -> ("01", "a.wav")
  * **bmsファイル内のwavのパスが相対パスなので分けている。WavFileUnitで返してその先で絶対パスにしてもいいかも。**

* public static string ReplaceLineDefinition(string line, WavFileUnit[] originalWavFiles, WavFileUnit[] newWavFiles)
  * 入力1：bmsファイル内の1行
  * 入力2：置換前#WAV一覧
  * 入力3：置換後#WAV一覧
  * 出力： 置換後のbmsファイル内の1行
  * 入力行の#WAVインデックスを置換した行を作成する。

* public static string ReductLineDefinition(string line, List<int> replacedTable)
  * 入力1：bmsファイル内の1行
  * 入力2：使用されている#WAVインデックス一覧
  * 出力： 置換後のbmsファイル内の1行
  * 入力行の#WAVインデックスの未使用定義を詰める。

* public static List<int> GetLineDefinition(string line)
  * 入力1：bmsファイル内の1行
  * 出力： 使用されている#WAVインデックス一覧
  * MAINコマンド行から使用されている#WAVインデックスのリストを取得する。



## BmsDefinitionReplace

BMS定義の置換に関する処理。

* public static string GetReplacedBmsFile(string bmsFileText, WavFileUnit[] originalWavFiles, WavFileUnit[] newWavFiles)
  * 入力1：bmsファイルテキスト(置換前)
  * 入力2：置換前#WAV一覧
  * 入力3：置換後#WAV一覧
  * 出力： bmsファイルテキスト(置換後)
  * bmsファイルのoriginalWavFilesの#WAVインデックスをnewWavFilesのものに置換する。

* public static string GetUnusedWavDeletedBmsFile(string bmsFileText)
  * 入力1：bmsファイルテキスト(未使用#WAV定義削除前)
  * 出力： bmsファイルテキスト(未使用#WAV定義削除後)
  * 未使用#WAV(譜面に未定義のもの。無音ノートは使用とする)を削除したbmsファイルテキストを取得する。

* public static string GetWavArrangedBmsFile(string bmsFileText)
  * 入力1：bmsファイルテキスト(未使用#WAV定義を詰める前)
  * 出力： bmsファイルテキスト(未使用#WAV定義を詰めた後)
  * 未使用#WAVを詰めたbmsファイルテキストを取得する。


## BmseClipboardBuilder

* public BmseClipboardBuilder()
  * コンストラクタ

* public void Append(int index, Note note)
  * 入力1：#WAVインデックス(10進数)
  * 入力2：ノート(bmsonに対応した型)
  * BMSEクリップボードフォーマットのノートを追加する。

* public void Get()
  * 出力：BMSEクリップボードテキスト
