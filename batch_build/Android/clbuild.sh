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
UNITY_BATCH_EXECUTE_METHOD_ANDROID=BatchBuild.DevelopmentBuildAndroid
else
UNITY_BATCH_EXECUTE_METHOD_ANDROID=BatchBuild.ReleaseBuildAndroid
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


# �w���Unity�v���W�F�N�g���o�b�`���[�h�N�������āA�w��̃��\�b�h(UnityScript)���Ăяo��
echo "Build Android"
$UNITY_APP_PATH -batchmode -quit -projectPath "${UNITY_PROJECT_PATH}" -executeMethod $UNITY_BATCH_EXECUTE_METHOD_ANDROID
cat $UNITY_EDITOR_LOG_PATH
# Unity�ł�build�Ɏ��s�����ꍇ�͏I��
if [ $? -eq 1 ]; then
	exit 1
fi

# WorkSpace����apk���폜
rm -rf $WORKSPACE/*.apk

#--------------------------------
# apk�t�@�C�����̕ύX
#--------------------------------
APK_FILE_PATH=$BUILD_PATH/android.apk
NEW_APK_FILE_PATH=$WORKSPACE/android-$BUILD_NUMBER-$BUILD_ID.apk

# �t�@�C�����ύX
mv $APK_FILE_PATH $NEW_APK_FILE_PATH

echo "${BUILD_COMMENT}"

if [ ${DEPLOY_GATE_UPLOAD} == "true" ]; then

#------ DeployGate �֓��e -----#
DEPLOY_TOKEN=9f5267fd5f8f53b2973fdb6fd6d9924de573a65c
DEPLOY_USERNAME=kenasai

MESSAGE="Team:B  Revision:${SVN_REVISION} DebugMode:${SOURCE_DEBUG_MODE} GameServer:${SOURCE_SELECT_SERVER} ImgServer:${SOURCE_SELECT_IMG_SERVER}"

# - Android
curl -F "file=@$NEW_APK_FILE_PATH" -F "token=$DEPLOY_TOKEN" -F "message=$MESSAGE" https://deploygate.com/api/users/$DEPLOY_USERNAME/apps

fi

