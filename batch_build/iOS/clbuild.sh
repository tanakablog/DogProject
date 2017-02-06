#!/bin/sh

#-----------------------------------------------------------------------------------------------
# Unity���o�b�`���[�h�ŋN�����Axcode�v���W�F�N�g,apk�t�@�C�����o�͂���
#-----------------------------------------------------------------------------------------------
# Unity�A�v���p�X
UNITY_APP_PATH=/Applications/Unity/Unity.app/Contents/MacOS/Unity
# �Ώۂ�Unity�v���W�F�N�g�p�X
UNITY_PROJECT_PATH=$WORKSPACE

# �o�b�`���[�h�ŋN����ɌĂяo�����\�b�h
if [ ${DEVELOPMENT_BUILD} == "true" ]; then
UNITY_BATCH_EXECUTE_METHOD_IOS=BatchBuild.DevelopmentBuildiOS
else
UNITY_BATCH_EXECUTE_METHOD_IOS=BatchBuild.ReleaseBuildiOS
fi

# Unity Editor ���O�t�@�C���p�X
UNITY_EDITOR_LOG_PATH=~/Library/Logs/Unity/Editor.log

# ipa apk�̃r���h��ABatchBuild�ƍ��킹�邱��
BUILD_PATH=/Users/iPhoneBuild/build

#-- build�p�X���폜
rm -rf $BUILD_PATH

#-- �r���h�p�X���쐬
mkdir $BUILD_PATH

# admob�ł���Ȃ��t�@�C�����폜
rm -rf $WORKSPACE/Assets/Editor/PostprocessBuildPlayer
rm -rf $WORKSPACE/Assets/Editor/PostprocessBuildPlayer.meta

echo "Build iOS"
$UNITY_APP_PATH -batchmode -quit -projectPath "${UNITY_PROJECT_PATH}" -executeMethod $UNITY_BATCH_EXECUTE_METHOD_IOS
cat $UNITY_EDITOR_LOG_PATH
# Unity�ł�build�Ɏ��s�����ꍇ�͏I��
if [ $? -eq 1 ]; then
exit 1
fi

#----------------------------------------------------------------------
#  Admob�̐ݒ�
#----------------------------------------------------------------------
chmod 777 ${WORKSPACE}/batch_build/iOS/AdmobXcodeEdit
${WORKSPACE}/batch_build/iOS/AdmobXcodeEdit "${BUILD_PATH}/ios"

#----------------------------------------------------------------------
# �R�}���h���C���ł�ipa��.dSYM�t�@�C���̍쐬
#----------------------------------------------------------------------

#------- �ؖ����C���|�[�g�Ƃƃv���r�W���j���O�t�@�C���̃R�s�[ -------#

# Keychain Location
KEYCHAIN_LOCATION=~/Library/Keychains/login.keychain
# Mac OSX�Ǘ��p�X���[�h
OSX_ADMIN_PASSWORD=iPhone

# �ؖ����t�@�C���p�X
IOS_P12_FILE_PATH=$WORKSPACE/batch_build/iOS/ios_distribution.p12
# �ؖ����t�@�C���p�X���[�h
IOS_P12_PASSWORD=visualize
# �v���r�W���j���O�t�@�C���p�X
IOS_PROVISIONING_FILE_PATH=$WORKSPACE/batch_build/iOS/AlchemiaDevelop.mobileprovision
# �v���r�W���j���O�t�@�C����UUID
PROFILE_UUID=`grep "UUID" ${IOS_PROVISIONING_FILE_PATH} -A 1 --binary-files=text 2>/dev/null |grep string|sed -e 's:^  *::g' -e 's:<string>::' -e 's:</string>::'`


# Keychain���A�����b�N�ɂ���
security default-keychain -s "${KEYCHAIN_LOCATION}"
security unlock-keychain -p $OSX_ADMIN_PASSWORD "${KEYCHAIN_LOCATION}"

# �ؖ�����import(���ł�import�ς݂ł����s�j
security import "${IOS_P12_FILE_PATH}" -f pkcs12 -P $IOS_P12_PASSWORD -k "${KEYCHAIN_LOCATION}" -T /usr/bin/codesign

# �v���r�W���j���O�t�@�C�����R�s�[����i�{���͉��s���ĂȂ��ł��j
cp "$IOS_PROVISIONING_FILE_PATH" "/Users/iPhoneBuild/Library/MobileDevice/Provisioning Profiles/$PROFILE_UUID.mobileprovision"

#------- xcode�v���W�F�N�g���r���h���� .app��.dSYM�𐶐����� -------#

# xcode�v���W�F�N�g�p�X
XCODE_PROJECT_PATH=$BUILD_PATH/ios
XCODE_PROJECT_CONFIG_PATH=$XCODE_PROJECT_PATH/Unity-iPhone.xcodeproj
# �r���h CONFIGURATION�ݒ�
CONFIGURATION=Release
# �R�[�h�����p IDENTITY
IDENTITY="iPhone Distribution: KENICHI ASAI"

# .dSYM�����̂��߂̃r���h�ݒ�
BUILD_OPT_MAKE_DSYM="GCC_GENERATE_DEBUGGING_SYMBOLS=YES DEBUG_INFORMATION_FORMAT=dwarf-with-dsym DEPLOYMENT_POSTPROCESSING=YES STRIP_INSTALLED_PRODUCT=YES SEPARATE_STRIP=YES COPY_PHASE_STRIP=NO"

# �r���h�J�n�i�{���͉��s���ĂȂ��ł��j
xcodebuild -project "${XCODE_PROJECT_CONFIG_PATH}" -configuration "${CONFIGURATION}" CODE_SIGN_IDENTITY="${IDENTITY}" PROVISIONING_PROFILE="${PROFILE_UUID}" $BUILD_OPT_MAKE_DSYM

# xcode�v���W�F�N�g�̃r���h�Ɏ��s�����ꍇ
EXIT_CODE=$?
if [ $EXIT_CODE -ne 0 ]; then
  cat "can not build app file"
  exit $EXIT_CODE
fi

#------- .app����ipa�쐬 -------#

rm -rf $WORKSPACE/*.ipa
rm -rf $WORKSPACE/*.zip

# .app��PATH�ݒ�
TARGET_APP_PATH=$XCODE_PROJECT_PATH/build/AlchemiaDevelop.app
# ipa��PATH�ݒ�
IPA_FILE_PATH=$WORKSPACE/AlchemiaDevelop-$BUILD_NUMBER-$BUILD_ID.ipa

# ipa�����J�n�i�{���͉��s���ĂȂ��ł��j
/usr/bin/xcrun -sdk iphoneos PackageApplication -v "${TARGET_APP_PATH}" -o "${IPA_FILE_PATH}"

# ipa�ւ̕ϊ��Ɏ��s�����ꍇ
EXIT_CODE=$?
if [ $EXIT_CODE -ne 0 ]; then
  cat "can not build ipa file"
  exit $EXIT_CODE
fi

#------ .dSYM��zip���k���� -----#

# .dSYM��PATH�ݒ�
TARGET_DSYM=AlchemiaDevelop.app.dSYM
# Zip�o�͂���.dSYM�t�@�C����PATH�ݒ�
DSYM_ZIP_PATH=$WORKSPACE/AlchemiaDevelop.app.dSYM-$BUILD_NUMBER-$BUILD_ID.zip

# �Ώ�.dSYM�����݂���f�B���N�g���Ɉړ�
cd $XCODE_PROJECT_PATH/build/

# zip���k�J�n
zip -r $DSYM_ZIP_PATH $TARGET_DSYM


if [ ${DEPLOY_GATE_UPLOAD} == "true" ]; then

#------ DeployGate �֓��e -----#
DEPLOY_TOKEN=9f5267fd5f8f53b2973fdb6fd6d9924de573a65c
DEPLOY_USERNAME=kenasai

MESSAGE="Team:B  Revision:${SVN_REVISION} DebugMode:${SOURCE_DEBUG_MODE} GameServer:${SOURCE_SELECT_SERVER} ImgServer:${SOURCE_SELECT_IMG_SERVER}"

# - Android
curl -F "file=@$IPA_FILE_PATH" -F "token=$DEPLOY_TOKEN" -F "message=$MESSAGE" https://deploygate.com/api/users/$DEPLOY_USERNAME/apps

fi
