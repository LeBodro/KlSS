#!/usr/bin/env bash

set -e

export PROJECT_NAME=KlSS
export WORKSPACE_DIR=/tmp/workspace

# butler push ~/repo/artifacts $ITCHIO_USER/$ITCHIO_GAME:$ITCHIO_CHANNEL

butler --help

set -x

ls -la ls -la ${WORKSPACE_DIR}

ls -la ${WORKSPACE_DIR}/${PROJECT_NAME}-win-x64.zip
ls -la ${WORKSPACE_DIR}/${PROJECT_NAME}-linux-x64.zip
ls -la ${WORKSPACE_DIR}/${PROJECT_NAME}-osx-x64.zip
