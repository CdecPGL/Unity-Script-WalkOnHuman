[English version is here.](https://github.com/CdecPGL/Unity-Script-WalkOnHuman/blob/master/README_en.md)

Unityスクリプト WalkOnHuman
====

Unityで人形モデルの上を歩くためのスクリプト。

## 概要

数個のスクリプトをアタッチするだけで、人形モデルの上を歩けるようになります。ベータ版のため、うまく立てないなど不安定な場合がありますがご了承ください。

## デモ

![ユニティちゃんの上を走るデモ](https://github.com/CdecPGL/ImageRepository/blob/master/WalkOnHumanDemo.gif?raw=true)

## 動作条件

Unity5.5で動作を確認済みです。

## 使い方

- 地面となる人形モデルの設定

    BoneGravityControllerとHumanColliderControllerをアタッチしてください。

    モデルの形状がアニメーションにより変化する場合に形状の変化をColliderに反映させたい時は、HumanColliderControllerのEnableRealtimeUpdateをtrueに設定してください。

    EnableRealtimeUpdateをtrueにした場合、動作が重くなります。EnableTimeLagUpdateをtrueにすると、精度と引き換えに動作が軽くなる可能性があります。

- 人形モデルの上を歩かせたいゲームオブジェクトの設定

    GravityReceiverをアタッチしてください。重力が弱い場合は、accelerationで調整できます。

- カメラの設定

    AngleFreeFollowCameraをアタッチし、targetに追従したいゲームオブジェクトを指定してください。

    また、distanceとheightを設定することでカメラの位置を調整できます。

- Unityの物理演算による重力の設定

    Unityの物理演算による重力が働いていると、うまく人形モデルの上に立てない場合があります。

    その場合は Edit->Project Settings->Physics から重力加速度を0に設定するか、RigidBodyのUseGravityをfalseに設定してください。

    それらの設定が行えない場合は、GravityReceiverのForceToInvalidateRigidBodyGravityをtrueに設定してください。

## インストール

使用したいUnityプロジェクトでWalkOnHuman.unitypackageをインポートすることで使用できるようになります。

このスクリプトのサンプルも必要な場合は、WalkOnHumanWithSample.unitypackegeをインポートしてください。

## ライセンス

[ユニティちゃんライセンス](http://unity-chan.com/contents/license_jp/)に基づきます。

Unityパッケージに含まれるサンプル内で[ユニティちゃん](http://unity-chan.com/)を使用しています。

![© Unity Technologies Japan/UCL](http://unity-chan.com/images/imageLicenseLogo.png "UCL")

## 作者

Cdec
- [GitHub](https://github.com/CdecPGL)
- [ホームページ](http://planetagamelabo.com/homepage)