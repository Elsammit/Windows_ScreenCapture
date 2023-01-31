# ScreenCapture
## 紹介
画面スクリーンの録画が行えるアプリケーションです。

自分で指定したエリアに限定して録画をすることができます。


### コントロール画面

アプリ起動時にコントロール画面が表示されます。
こちらのコントロール画面にて録画の開始やエリア選択指示を行います。

Select Record Area：Window画面上のエリア選択画面が表示され、録画エリアを決めることができます。
Record：選択されたエリアの録画が開始できます。

![png](https://github.com/Elsammit/Windows_ScreenCapture/blob/main/Sample/TitlePage.png)


### エリア選択画面

Select Record Areaボタンを選択すると画面が薄灰色に変化し、エリア選択画面に遷移します。
この状態でマウスを操作することで録画エリアを選択できます。
尚、録画エリアは赤色枠になります。

![gif](https://github.com/Elsammit/Windows_ScreenCapture/blob/main/Sample/WindowRec1.gif)


### 録画中の画面

Recordボタンを押すと録画中画面になります。
エリア選択画面で選択した赤枠は消えてしまいますが、
録画中はエリア選択画面で選択したエリアのみ録画を行います。

録画を終了する際には再度Recordボタンを押してください。
終了すると、メッセージが表示されます。
録画データは.mp4ファイルです。

![gif](https://github.com/Elsammit/Windows_ScreenCapture/blob/main/Sample/WindowRec2.gif)



### 録画した画面

録画した結果はこちらのように指定したエリアのみが録画されます。

![gif](https://github.com/Elsammit/Windows_ScreenCapture/blob/main/Sample/RecMovie.gif)


## 動作確認環境

　・OS：Windows10 64bit

　・メモリ：8GB以上
   
　・ディスプレイ解像度：1920x1080


## 使い方

1.アプリを立ち上げる

2. Select Record Areaボタン押下する

3.エリアを指定する

4.Recordボタンをクリック


## 注意事項
・デュアルディスプレイ以上で使用している場合、プライマリディスプレイのみが録画対象になります。

・録画ファイルのフォーマットはmp4固定です。

・録画時間は最長30分です(30分を超えると強制的に録画終了します)。
