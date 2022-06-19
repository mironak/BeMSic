# Helpers API

BMSやwav操作にかかわらないような処理を入れる。


## RadixConvert

基数変換に関する処理

public static string IntToZZ(int dec)
  * 数値(0～1295)を36進2桁に変換する。

public static int ZZToInt(string zz)
  * 36進数2桁を数値に変換する。
