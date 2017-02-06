#!/bin/sh

#-----------------------------------------------------------------------------------------------
# Unityをバッチモードで起動し、xcodeプロジェクト,apkファイルを出力する
#-----------------------------------------------------------------------------------------------
# Unityアプリパス
UNITY_APP_PATH=/Applications/Unity/Unity.app/Contents/MacOS/Unity
# 対象のUnityプロジェクトパス
UNITY_PROJECT_PATH=$WORKSPACE

# バッチモードで起動後に呼び出すメソッド
if [ ${DEVELOPMENT_BUILD} == "true" ]; then
UNITY_BATCH_EXECUTE_METHOD_ANDROID=BatchBuild.DevelopmentBuildAndroid
else
UNITY_BATCH_EXECUTE_METHOD_ANDROID=BatchBuild.ReleaseBuildAndroid
fi

# Unity Editor ログファイルパス
UNITY_EDITOR_LOG_PATH=~/Library/Logs/Unity/Editor.log

# ipa apkのビルド先、BatchBuildと合わせること
BUILD_PATH=/Users/iPhoneBuild/build

#-- buildパスを削除
rm -rf $BUILD_PATH

#-- ビルドパスを作成
mkdir $BUILD_PATH

# admobでいらないファイルを削除
rm -rf $WORKSPACE/Assets/Editor/PostprocessBuildPlayer
rm -rf $WORKSPACE/Assets/Editor/PostprocessBuildPlayer.meta


# 指定のUnityプロジェクトをバッチモード起動させて、指定のメソッド(UnityScript)を呼び出す
echo "Build Android"
$UNITY_APP_PATH -batchmode -quit -projectPath "${UNITY_PROJECT_PATH}" -executeMethod $UNITY_BATCH_EXECUTE_METHOD_ANDROID
cat $UNITY_EDITOR_LOG_PATH
# Unityでのbuildに失敗した場合は終了
if [ $? -eq 1 ]; then
	exit 1
fi

# WorkSpace内のapkを削除
rm -rf $WORKSPACE/*.apk

#--------------------------------
# apkファイル名の変更
#--------------------------------
APK_FILE_PATH=$BUILD_PATH/android.apk
NEW_APK_FILE_PATH=$WORKSPACE/android-$BUILD_NUMBER-$BUILD_ID.apk

# ファイル名変更
mv $APK_FILE_PATH $NEW_APK_FILE_PATH

echo "${BUILD_COMMENT}"

if [ ${DEPLOY_GATE_UPLOAD} == "true" ]; then

#------ DeployGate へ投稿 -----#
DEPLOY_TOKEN=9f5267fd5f8f53b2973fdb6fd6d9924de573a65c
DEPLOY_USERNAME=kenasai

MESSAGE="Team:B  Revision:${SVN_REVISION} DebugMode:${SOURCE_DEBUG_MODE} GameServer:${SOURCE_SELECT_SERVER} ImgServer:${SOURCE_SELECT_IMG_SERVER}"

# - Android
curl -F "file=@$NEW_APK_FILE_PATH" -F "token=$DEPLOY_TOKEN" -F "message=$MESSAGE" https://deploygate.com/api/users/$DEPLOY_USERNAME/apps

fi

