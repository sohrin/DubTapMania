# DubTapMania
 tap music game

# Logicメモ
* BattleBgmStage1.wav：BPM150

# インストールするツール
* Unity(今回は2019.4.171fを利用)
* CRI ADX2 LE(今回は2021年1日1日時点の最新版2.10.06を利用)

# CRI ADX2 LEの設定手順
1. ダウンロード
https://game.criware.jp/products/adx2-le/
2. UnityプロジェクトにCRIWAREプラグインを導入
https://game.criware.jp/learn/tutorial/unity/unity_nyumon_01/
3. CRI Atom Craftをインストール
https://game.criware.jp/learn/tutorial/unity/unity_extra_01/

# CRI ADX2 LEでの基本的な音の再生方法
・【Unity：CRI】UnityでCRIを使う覚悟はいいか？オレはできてる
https://qiita.com/shirasaya0201/items/53baa38c83eebaa2cfbf

・Unity連携編.04 / CriAtomSourceを使わずに音を鳴らす
https://criware.info/adx2forunity-use-criatomexplayer/
https://game.criware.jp/manual/unity_plugin/latest/contents/cri4u_samples_criatom_primitive_scene01.html
https://game.criware.jp/manual/unity_plugin/latest/contents/classCriAtomExPlayer.html#a8230c25d7f4514ef1d05f908430a13f8
https://game.criware.jp/manual/unity_plugin/latest/contents/classCriAtomExPlayer.html#aa43bbc3b25207e2c4d665bd6d001ceb4
https://game.criware.jp/manual/unity_plugin/latest/contents/cri4u_component_initializer.html
https://game.criware.jp/manual/adx2_tool/latest/craftv2_tips_program_decide_voicepool.html


# CRI ADX2 LEでの基本的な音の再生方法
・一番初めに読み込むシーンにCriWareLibraryInitializerとCriWareErrorHandlerを追加しないと、2回ライブラリ設定する挙動になってエラーが発生する。→全シーンに必要だった模様。別シーンからコピペした。
Unity CRI ADX2LEエラー「CriAtomPlugin is not initialized」の対処
https://www.shibuya24.info/entry/CriAtomPlugin_is_not_initialized
https://game.criware.jp/manual/native/adx2/latest/criatom_trblsht_voice_does_not_exist_that_match_playback_conditions.html
https://game.criware.jp/manual/unity_plugin/latest/contents/cri4u_component_initializer.html


# Atomキューシートバイナリを更新する手順
0. （プロジェクト作成時）プロジェクトルートは「【Unityプロジェクトフォルダ】/CriAtomCraftProject」とし、プロジェクト名は「DubTapMania」とする。
0. シート削除時、「【Unityプロジェクトフォルダ】/CriAtomCraftProject/DubTapMania/Public」フォルダを削除しておく。
1. ワークユニット、キュー、ファイル名を設計する（ひとまずワークユニットはシーン毎等の大きな分け、キューはBGMは1つ・SEはある程度の分けて1つ、ファイル名はC#コード生成時がうまくいかなくなるので数字から始めないこと）
2. 「/OriginalSound/【ワークユニット名】/【キュー名】」にwavファイルを格納する。
3. CriAtomCraft上でワークユニット、キューを作成し、追加wavファイルをドロップする。シーンを追加してアクティブを切り替える流れ。

※以下URLを見ると、フォルダドロップでもいけるっぽい？
https://qiita.com/tatmos/items/858ec7f6507b6abd015c

4. 「ビルド」メニュー＞Atomキューシートバイナリ→全シートチェック、追加出力とオプションはすべてチェックし、「ビルド」ボタンを押下する。ビルド完了、エラーや警告が発生していないこと。また、詳細なビルドログをCriAtomCraftプロジェクトフォルダ直下に保存しておく。ファイル名は「BuildDetailedLog.txt」とする。
5. 「【Unityプロジェクトフォルダ】/Tools/CopyCriAtomCraftBin2Assets/CopyCriAtomCraftBin2Assets.command」（mac）用を実行し、acfファイル、acbファイル、csファイルをクリーン・コピーする。
6. Unityのウィンドウをアクティブにし、Assetsをインポートさせる（metaファイルが生成される）。



# バグ
・BattleManagerが2インスタンス存在している。（DontDestroyOnLoadの影響のようだが？）


## TODO（済）
・ボタンをクリックするとHPが減ってやっつけたら次の敵が出てくる
・敵データのクラス化
・敵スプライトの作成
・敵スプライトのタイマー差し替え
・攻撃音、撃破音の再生
・ビルド（手動）、DeployGate経由で動作確認
・戦闘曲のループ再生（Unity標準）
・タイトルシーンの作成
・TitleSceneでボタンのOnClickイベントが発火しない→EventSystemを追加したら解消した。
・敵データのScriptableObject化

## TODO（なう）
・倒した敵の数をカウントするようにする。
・セーブデータの保存（ローカル）

## TODO（未）
・CRIADX2のビート同期を使ってリズムに合わせた演出をつける
https://qiita.com/nishimura-ma/items/bac6e3d6528490bd9e36
・セーブデータの保存（PlayFab）
・セーブデータの復元（PlayFab）
・UniRx導入
・BPM設定から拍を判定
・タイトルシーンでのユーザ認証
・音声の読み込み方の見直し
Unity + ADX2におけるサウンドデータの読み込みと破棄
https://qiita.com/Takaaki_Ichijo/items/c7e14234f799fdca3e68
あんスタ！！MusicにおけるCRI ADX2活用事例（後編）
https://qiita.com/k7a/items/fe29f3edec3063f03a25
・マスタデータの準備（ローカル）
・マスタデータの準備（ネット）
・タップエリアの設定（ひとまずボタンから画面全体。拡張可能なようにパネルを設定したほうがよさそう）
・背景画像の設定
・タップ時の攻撃エフェクト
・BPM設定
・タイトルシーンとバトルシーンに分ける。
・認証
・マスタデータ取得APIとの通信
　　敵データ（ドロップ情報含む）
　　アイテムデータ（音と絡める）
　　ストーリー情報（API化しないほうがよさそう）
・課金システム
・動画広告と音楽ゲームを絡める
・CircleCIでのビルド・DeployGateでのデプロイの自動化
・敵データをAPIから取得してScriptableObjectに流し込む／敵データのScriptableObjectをCSVインポート




■参考サイト
・初めてUnityをインストールする手順について＜MAC編＞
https://create.unity3d.com/jp-howto-install-mac

・Unityのライセンス認証
https://www.sbcr.jp/support/15197/

・Unityプロジェクトのバージョン管理にはGitHub Desktopを使おう
https://qiita.com/hkomo746/items/a4f99261868069f6eeb5
※Github for Unityはログインすらできなかった。

・【Unity】スマホの縦向き横向きを設定する方法！画面の回転も制御できる
https://clrmemory.com/programming/unity/apps-horizontal-vertical/

・[Unity5.5] 2Dゲーム開発用に画面サイズを設定する
https://design-program.com/unity/unity-2d_screen_size_setting

・Unity入門！チュートリアルで学ぶ2Dアクションゲームの作り方
https://udemy.benesse.co.jp/design/3d/unity-2d.html
※スプライトの使い方
https://udemy.benesse.co.jp/design/3d/unity-2d.html
https://assetstore.unity.com/packages/2d/characters/aekashics-librarium-megapack-iii-130410

・【Unity入門】Buttonの作り方!押された判定はどう取るの?
https://www.sejuku.net/blog/56265

・【C#】 Listから指定したIndexで取得(get)したい
http://okaprogram.blog.fc2.com/blog-entry-7.html

・【C#入門】文字列を数値に、数値を文字列に変換する方法
https://www.sejuku.net/blog/44977

・Unity オブジェクトの画像（Sprite)を動的に変更する方法
https://qiita.com/motsat/items/927a4d2682765555b80d

・C# の const の間違った使い方をやめよう
https://qiita.com/Nossa/items/b874fa6c115898e2a9c8

・C# Listの使い方のサンプル
https://itsakura.com/csharp-list

・Unityでデータ保持用のクラスを扱う時に戸惑ったお話
https://collonville.hateblo.jp/entry/2017/09/27/021224

・UnityのC#スクリプトでクラスやGameObjectなどを呼び出す手法集
https://qiita.com/sabamotto/items/7aa3a988dd4cd45633db
※namespaceとusing

・Resources.Loadでスプライトを取得する
http://matudozer.blog.fc2.com/blog-entry-28.html

・スプライト（公式ドキュメント）
https://docs.unity3d.com/ja/2018.4/Manual/Sprites.html

・Unityで経過時間、制限時間を表示する機能を作成する
https://gametukurikata.com/program/time

・Unityで音を再生する
http://stepbystepweb.hatenablog.com/entry/2014/03/07/150204
※Loadの記法は以下URLの方法が新しい書き方の様子。

・【Unity】BGM,SEをとりあえず再生したい人へ
https://qiita.com/ioaxis/items/58ac66098545b3565dee

・【Unity】Androidビルドが2019でめっちゃ簡単になってた件
https://www.deathponta.com/entry/190510_unty2019_androidBuildEasy
※Switch PlatSformをクリックして切り替えないとBuildボタンが押せないので注意。

・Unity Andoroid実機でのビルドで文字やボタンが小さくなる場合の対処方法 (uGUI使用)
https://monaski.hatenablog.com/entry/2015/05/11/015200

・【Unity】音声（BGM・SE）の再生・ループ・フェードアウトなどの設定方法を徹底解説！
https://xr-hub.com/archives/18550

・新しいシーンを作成する - Unityプログラミング
https://www.ipentec.com/document/unity-create-new-scene

・Unityでの複数シーンを使ったゲームの実装方法とメモリリークについて
https://madnesslabo.net/utage/?page_id=11109

・初期シーンについて
https://teratail.com/questions/24535

・【Unity入門】超簡単！別のシーンへ切り替える方法
https://www.sejuku.net/blog/49352

・イベント関数の実行順序
https://docs.unity3d.com/ja/2018.4/Manual/ExecutionOrder.html#FirstSceneLoad

・複数のカメラを使用する
https://docs.unity3d.com/ja/2018.4/Manual/MultipleCameras.html
※複数のシーンにそれぞれカメラを用意している場合、Build Settingsの読み込み順ではなく、
　カメラのDepthで初めにに表示するシーン制御可能。

・SCENELOADEDで詰まる
http://kabegiwa.sakura.ne.jp/wp/2016/10/13/%E3%82%B2%E3%83%BC%E3%83%A0%E9%96%8B%E7%99%BA/sceneloaded%E3%81%A7%E8%A9%B0%E3%81%BE%E3%82%8B

・【Unity】シーンが遷移したことを検知する
https://nn-hokuson.hatenablog.com/entry/2017/05/29/204702

・Unityで複数シーンにEventSystemを追加できない場合
https://www.uowakame.com/unity-add-eventsystem/

・【Unity】俺はまだSceneManagerを全力で使っていない！
https://www.urablog.xyz/entry/2017/04/09/180155

・シーンを追加してアクティブを切り替える流れ。
http://karaagedigital.hatenablog.jp/entry/2016/11/25/121318

・【Unity】ボタンのタッチダウン時にメソッドを呼び出す
https://nn-hokuson.hatenablog.com/entry/2018/08/15/151939

・列挙型 (C# リファレンス)
https://docs.microsoft.com/ja-jp/dotnet/csharp/language-reference/builtin-types/enum

・ドキュメント コメント用の推奨タグ (C# プログラミング ガイド)
https://docs.microsoft.com/ja-jp/dotnet/csharp/programming-guide/xmldoc/recommended-tags-for-documentation-comments

・数値を0埋めで文字列にするには？（ToString編）［C#、VB］
https://www.atmarkit.co.jp/ait/articles/0909/17/news134.html

・List<T>.Count プロパティ
https://docs.microsoft.com/ja-jp/dotnet/api/system.collections.generic.list-1.count?view=net-5.0

・Macでシェルスクリプトをダブルクリックで実行
https://qiita.com/hkwid/items/c7453abac6aa6449e50b

・Macでバッチ（command）ファイルを作る方法
https://qiita.com/TatsuyaOGth/items/f15bfa9aeb68d8ecfc67

・【Unity】ScriptableObjectにマスタデータを持たせるメリットについて
https://ekulabo.com/scriptableobject-for-master-data#outline__5


・作成したScriptableObjectを呼び出す
http://tsubakit1.hateblo.jp/entry/20140322/1395476579

・【Unity】CSVからScriptableObjectにデータを流し込むEditor拡張サンプル



■参考サイト（没）
・MusicEngine for Unity/ADX2を使った簡単音楽同期ゲーム制作
https://www.slideshare.net/geekdrums/about-musicengine-46632468?next_slideshow=1

・[Unity] 0から始めるMusicEngine
https://qiita.com/lycoris102/items/0969a07d09e3655b11ce






■参考サイト（未）
・透過PNGの設定方法　Unity
https://note.com/app49/n/n4e5a5f7aac2c

・UnityでScriptableObjectを使ってアイテムデータベースを作成する
https://gametukurikata.com/program/scriptableobjectitemdatabase

・今日からUnity + Visual Studio Codeを用いた快適な開発生活(随時更新中)
https://qiita.com/4_mio_11/items/e7b0a5e65c89ac9d6d7f

・Unityゲーム開発におけるAndroidのサウンド再生遅延対策
https://qiita.com/Takaaki_Ichijo/items/a774ecc3483e1761776f
https://game.criware.jp/products/adx2-le/
https://logmi.jp/tech/articles/321019

・初心者が送る UnityでAPI通信講座
https://qiita.com/pchan52/items/feca16ea98289ec31c65

・Unity 設定情報(Config値)をスマートに管理する
https://qiita.com/toRisouP/items/7765cf891a93bdcf65bc

・【Unity】今から始めるAssetBundle【2019】
https://light11.hatenadiary.com/entry/2019/06/24/212437

・【Unity】どのシーンから起動しても共通の初期化を呼び出したい
https://noracle.jp/unity-initialize-scene/

・Unityで「There are 2 audio listeners in the scene」のエラーが出た場合
https://qiita.com/sheltie-fusafusa/items/7f921a95e9ebc6c9e7d0
https://qiita.com/unsoluble_sugar/items/80b8666c4196992d08ae

・GitHub ActionsでUnityのAndroid, iOSビルドをやってみる
https://qiita.com/johro/items/0a846bebfe3899410164

・GitHub Actions のセルフホストランナーで Unity ビルドする
https://framesynthesis.jp/tech/github/actions-unity/

・CircleCIでUnityのTest&Buildを雰囲気理解で走らせた
https://qiita.com/MizoTake/items/2d822e85d33bee359f98

・CRI Atom Encoderコンソール版
https://game.criware.jp/manual/adx2_tool/latest/criatom_tools_criatomenc_cui.html

・JUCEでADX2LEを使ってAUプラグインを作る
https://qiita.com/tatmos/items/67c9b5c7fbf221178f26

・MakeAtomCraftData
https://github.com/tatmos/MakeAtomCraftData

・AssetBundleを完全に理解する
https://qiita.com/k7a/items/d27640ac0276214fc850

・Unity + PlayFab + ADX2でAsset Bundleを介さないサウンドデータの配信を実装する
https://qiita.com/Takaaki_Ichijo/items/bc6ef09fa55d496e5800

・サウンドフォントからADX2LE変換するSF2ADX2を作ってみた
https://qiita.com/tatmos/items/cfd625c7a4363a3dfe97

・[Unity]ADX2 LEを使ったときに実機で音が出ないときのメモ
https://qiita.com/haifuri/items/e27a9712d203bdbcdb09

・Unity + ADX2におけるサウンドデータの読み込みと破棄
https://qiita.com/Takaaki_Ichijo/items/c7e14234f799fdca3e68

・Unity + ADX2環境でゲーム中の音を録音する
https://qiita.com/Takaaki_Ichijo/items/faf96c22740ff5a7587d

・ADX2 for Unityで楽曲がリアルタイムに変化するBGMを再生（インタラクティブミュージック）
https://qiita.com/Takaaki_Ichijo/items/4ffd0fa28f196089defd











■UnityがクラッシュしてHierarchyがUntitledのみになった場合の戻し方
①シーンをドラッグ＆ドロップ
②Untitledシーンを削除
③Gameタブの画面設定がFree Aspectになっているので「16:9 Portrait (9:16)」を選び直す。






■並行してやりたいこと
・ゲーム用API（.NET 5とか）
・認証（firebase？）


■目的



■ターゲット
・全世界
・ゲーセンの待ちの合間にリズムを養う人
・タップゲーが好きな人
・可愛さで非ゲーマーも狙う

■ネタメモ
・メタルたこやき：スカッ！スカッ！スカッ！カキーン！（敵スプライトコード41（避け）：ニヤケ顔、残像）※大半の敵は必中。一部のみスカる音が音楽的に気持ちいいようにする。
・たこやき、放置すると「たこ焼き」を食べる（敵スプライトコード51（挑発、退屈））
・各ステージに「ドラムモード」「ベースモード」「メロディモード（リフ、アルペジオ含む）」
・敵ごとのピカグレ幅があってもいいかも。

■敵進化メモ
・たこぶね：銀だこのように集まったたこやき