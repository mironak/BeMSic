## Bmson API

BMSONフォーマットの操作に関するクラスを入れる。

# BmsBuilder

BMSONからBMSファイルを作成する。

public BmsBuilder(Bmson? bmson)
  * コンストラクタ。ヘッダ情報を入れておく。

public void AppendWav(WavFileUnit wav)
  * wavを追加。

public void Write(string saveFileName)
  * BMSファイルの出力。
  * **テキストを返して返却先でファイル出力したほうがいいかも**

public static string GetBmsLaneNum(int? lane)
  * BMSONのレーン番号からBMSのレーン番号(文字列)を取得


# Bmson

BMSONフォーマットクラス
https://bmson-spec.readthedocs.io/en/master/doc/index.html


# BmsonParser

public BmsonMain(string fileName)
  * BMSONファイルを読み込む。

public void CutWav(string saveBmsFilePath, string readWavFilePath, int chIndex)
  * readWavFilePathにあるBMSONフォーマットのchIndex番目のwavをsaveBmsFilePathのディレクトリに保存。

 
