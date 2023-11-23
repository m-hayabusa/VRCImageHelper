# VRCImageHelper

VRChatのカメラで撮った画像を圧縮して、

* ワールド名
* インスタンスにいたプレイヤー名
* VirtualLens2の絞り値
* VirtualLens2の焦点距離
* (MakerNotesに、Base64でエンコードされたJSONとして) World Name / World ID / Instance Owner / Permission / Playerのリスト

を書き込むツールです。

### インストール

1. [インストーラ](https://github.com/m-hayabusa/VRCImageHelper/releases/download/latest/VRCImageHelper.msi) をダウンロードし、インストールする

2. インストール中に設定画面が出てくるので、設定する  
	![設定画面](https://github.com/m-hayabusa/VRCImageHelper/assets/10593623/8b2b56e3-f31e-4017-9c99-e2ce636e8bfd)

	* 保存先: 変換後のファイルを保存する場所。未設定の場合はVRChatが出力したフォルダに保存される

	* 保存形式
		* ファイル名: 保存するときのファイル名。デフォルトの場合はVRChatの出力したファイルと同等となるはず
			|フォーマット|置換内容|
			|:-|:-|
			|yyyy|年|
			|MM|月|
			|dd|日|
			|hh|時|
			|mm|分|
			|ss|秒|
			|fff|秒(小数点下)|
			|XXXX|画像のピクセル数 (縦)|
			|YYYY|画像のピクセル数 (横)|

		* 形式・品質・オプション: PNG / JPEG / AVIFが選択できる
			* PNG: 品質設定とオプションは、無視される
			* JPEG: 品質設定は、0が最高、100が最低となる。オプションは無視される
			* AVIF: 品質設定は、ffmpegでそれぞれのエンコーダの
				|エンコーダ|引数|
				|:-|:-|
				|libaom-av1|`-crf`|
				|libsvtav1|`-crf`|
				|av1_qsv|`-q`|
				|av1_nvenc|`-cq`|
				|av1_amf|`-qp_i`|

				に指定する数値で、0とすると画質が最高となる(はず)  
				CPUでlibaom-av1とlibsvtav1を利用する場合と、Intel Arcでav1_qsvを利用した場合のみ確認 (NvEnc / AMFの場合の動作については一切検証できていないので、正しいパラメータの指定などあれば[教えてください](https://github.com/m-hayabusa/VRCImageHelper/issues/new))  
				オプションは、ffmpegに追加で渡す引数を入力できる
	* 保存形式(透過)  
		画像にアルファチャネルが含まれる場合の形式指定で、それ以外は上記 保存形式 と同じ。ただし:
		* JPEG: 非対応
		* AVIF: 透過に対応しないAV1エンコーダもある (手元の環境ではlibaom-av1しか透明度を処理できないようだった)
3. (もしVRChat Exif Writerをインストールしたことがあり、削除していない場合) VRChat Exif Writerを削除することについて確認メッセージが出るので、特に理由がなければ、そのまま削除してください

4. タスクバーにあるアイコン <img style="height:1em" src="https://github.com/m-hayabusa/VRCImageHelper/raw/master/VRCImageHelper/icon.ico"> を右クリックし、自動起動のチェックをつけると、次回からPCにログインした際に自動で起動するようになる

### アンインストール

Windowsの 設定/アプリ/インストールされているアプリ からアンインストールできる (同時に設定ファイルも削除される)

### ExifToolとffmpegについて
内部でexiftool.exeとffmpeg.exeを利用するが、環境変数 PATH に設定されたディレクトリ内に見つからなければ、自動でダウンロードするので、こだわりがなければ用意する必要はない。自動でダウンロードされるものは:
* ExifTool: https://sourceforge.net/projects/exiftool/files/latest/download
* ffmpeg: https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-win64-gpl.zip