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
UNITY_BATCH_EXECUTE_METHOD_IOS=BatchBuild.DevelopmentBuildiOS
else
UNITY_BATCH_EXECUTE_METHOD_IOS=BatchBuild.ReleaseBuildiOS
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

echo "Build iOS"
$UNITY_APP_PATH -batchmode -quit -projectPath "${UNITY_PROJECT_PATH}" -executeMethod $UNITY_BATCH_EXECUTE_METHOD_IOS
cat $UNITY_EDITOR_LOG_PATH
# Unityでのbuildに失敗した場合は終了
if [ $? -eq 1 ]; then
exit 1
fi

#----------------------------------------------------------------------
#  Admobの設定
#----------------------------------------------------------------------
chmod 777 ${WORKSPACE}/batch_build/iOS/AdmobXcodeEdit
${WORKSPACE}/batch_build/iOS/AdmobXcodeEdit "${BUILD_PATH}/ios"

#----------------------------------------------------------------------
# コマンドラインでのipaと.dSYMファイルの作成
#----------------------------------------------------------------------

#------- 証明書インポートととプロビジョニングファイルのコピー -------#

# Keychain Location
KEYCHAIN_LOCATION=~/Library/Keychains/login.keychain
# Mac OSX管理パスワード
OSX_ADMIN_PASSWORD=iPhone

# 証明書ファイルパス
IOS_P12_FILE_PATH=$WORKSPACE/batch_build/iOS/ios_distribution.p12
# 証明書ファイルパスワード
IOS_P12_PASSWORD=visualize
# プロビジョニングファイルパス
IOS_PROVISIONING_FILE_PATH=$WORKSPACE/batch_build/iOS/AlchemiaDevelop.mobileprovision
# プロビジョニングファイルのUUID
PROFILE_UUID=`grep "UUID" ${IOS_PROVISIONING_FILE_PATH} -A 1 --binary-files=text 2>/dev/null |grep string|sed -e 's:^  *::g' -e 's:<string>::' -e 's:</string>::'`


# Keychainをアンロックにする
security default-keychain -s "${KEYCHAIN_LOCATION}"
security unlock-keychain -p $OSX_ADMIN_PASSWORD "${KEYCHAIN_LOCATION}"

# 証明書のimport(すでにimport済みでも実行）
security import "${IOS_P12_FILE_PATH}" -f pkcs12 -P $IOS_P12_PASSWORD -k "${KEYCHAIN_LOCATION}" -T /usr/bin/codesign

# プロビジョニングファイルをコピーする（本来は改行してないです）
cp "$IOS_PROVISIONING_FILE_PATH" "/Users/iPhoneBuild/Library/MobileDevice/Provisioning Profiles/$PROFILE_UUID.mobileprovision"

#------- xcodeプロジェクトをビルドして .appと.dSYMを生成する -------#

# xcodeプロジェクトパス
XCODE_PROJECT_PATH=$BUILD_PATH/ios
XCODE_PROJECT_CONFIG_PATH=$XCODE_PROJECT_PATH/Unity-iPhone.xcodeproj
# ビルド CONFIGURATION設定
CONFIGURATION=Release
# コード署名用 IDENTITY
IDENTITY="iPhone Distribution: KENICHI ASAI"

# .dSYM生成のためのビルド設定
BUILD_OPT_MAKE_DSYM="GCC_GENERATE_DEBUGGING_SYMBOLS=YES DEBUG_INFORMATION_FORMAT=dwarf-with-dsym DEPLOYMENT_POSTPROCESSING=YES STRIP_INSTALLED_PRODUCT=YES SEPARATE_STRIP=YES COPY_PHASE_STRIP=NO"

# ビルド開始（本来は改行してないです）
xcodebuild -project "${XCODE_PROJECT_CONFIG_PATH}" -configuration "${CONFIGURATION}" CODE_SIGN_IDENTITY="${IDENTITY}" PROVISIONING_PROFILE="${PROFILE_UUID}" $BUILD_OPT_MAKE_DSYM

# xcodeプロジェクトのビルドに失敗した場合
EXIT_CODE=$?
if [ $EXIT_CODE -ne 0 ]; then
  cat "can not build app file"
  exit $EXIT_CODE
fi

#------- .appからipa作成 -------#

rm -rf $WORKSPACE/*.ipa
rm -rf $WORKSPACE/*.zip

# .appのPATH設定
TARGET_APP_PATH=$XCODE_PROJECT_PATH/build/AlchemiaDevelop.app
# ipaのPATH設定
IPA_FILE_PATH=$WORKSPACE/AlchemiaDevelop-$BUILD_NUMBER-$BUILD_ID.ipa

# ipa生成開始（本来は改行してないです）
/usr/bin/xcrun -sdk iphoneos PackageApplication -v "${TARGET_APP_PATH}" -o "${IPA_FILE_PATH}"

# ipaへの変換に失敗した場合
EXIT_CODE=$?
if [ $EXIT_CODE -ne 0 ]; then
  cat "can not build ipa file"
  exit $EXIT_CODE
fi

#------ .dSYMをzip圧縮する -----#

# .dSYMのPATH設定
TARGET_DSYM=AlchemiaDevelop.app.dSYM
# Zip出力する.dSYMファイルのPATH設定
DSYM_ZIP_PATH=$WORKSPACE/AlchemiaDevelop.app.dSYM-$BUILD_NUMBER-$BUILD_ID.zip

# 対象.dSYMが存在するディレクトリに移動
cd $XCODE_PROJECT_PATH/build/

# zip圧縮開始
zip -r $DSYM_ZIP_PATH $TARGET_DSYM


if [ ${DEPLOY_GATE_UPLOAD} == "true" ]; then

#------ DeployGate へ投稿 -----#
DEPLOY_TOKEN=9f5267fd5f8f53b2973fdb6fd6d9924de573a65c
DEPLOY_USERNAME=kenasai

MESSAGE="Team:B  Revision:${SVN_REVISION} DebugMode:${SOURCE_DEBUG_MODE} GameServer:${SOURCE_SELECT_SERVER} ImgServer:${SOURCE_SELECT_IMG_SERVER}"

# - Android
curl -F "file=@$IPA_FILE_PATH" -F "token=$DEPLOY_TOKEN" -F "message=$MESSAGE" https://deploygate.com/api/users/$DEPLOY_USERNAME/apps

fi
