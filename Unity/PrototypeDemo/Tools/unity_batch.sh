#!/usr/bin/env bash
# Usage: Tools/unity_batch.sh <Namespace.Class.Method> <logname> [extra unity args]
set -u
UNITY="/c/Program Files/Unity/Hub/Editor/6000.3.24f1/Editor/Unity.exe"
PROJ="C:\Juego2-Console\Unity\PrototypeDemo"
METHOD="$1"; LOG="$2"; shift 2
"$UNITY" -batchmode -quit -projectPath "$PROJ" -executeMethod "$METHOD" -logFile "C:/Juego2-Console/Unity/PrototypeDemo/Logs/$LOG.log" "$@"
code=$?
L="/c/Juego2-Console/Unity/PrototypeDemo/Logs/$LOG.log"
grep -E "error CS|\[Proto\]|Exception|Error:|executeMethod" "$L" | grep -v "Licensing" | head -40
echo "exit=$code"
