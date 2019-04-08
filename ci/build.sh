#!/usr/bin/env sh

set -x
set -e

export PROJECT_NAME=KlSS
export BUILD_CONFIGURATION='release'

export ARTIFACT_FILENAME=${PROJECT_NAME}-${BUILD_PLATFORM}.zip
export ARTIFACTS_DIR=~/repo/artifacts
export ARTIFACT_FULL_PATH=${ARTIFACTS_DIR}/${OUTPUT_FILE}

export PUBLISH_DIR=~/repo/bin/release/netcoreapp2.2/${BUILD_PLATFORM}/publish
export MGCB_VERSION=3.7.0.4
export MGCB_PACKAGE_PATH=/root/.nuget/packages/monogame.content.builder/${MGCB_VERSION}/build/MGCB/build

# adding zip cli for artifact zipping, todo: add this to base image
apt-get update && apt-get install -y zip

dotnet restore

# workaround to ensure monogame.content.builder package binaries are executable
# TODO: check if there's a command only to download nuget from csproj only instead
dotnet build --configuration ${BUILD_CONFIGURATION} || true
chmod +x ${MGCB_PACKAGE_PATH}/ffprobe
chmod +x ${MGCB_PACKAGE_PATH}/ffmpeg

# actual build
dotnet build --configuration ${BUILD_CONFIGURATION}
dotnet publish -r ${BUILD_PLATFORM} --configuration ${BUILD_CONFIGURATION} /p:TrimUnusedDependencies=true

# artifacts
mkdir -p ${ARTIFACTS_DIR}
cd ${PUBLISH_DIR} && zip -r ${ARTIFACT_FULL_PATH} ./
