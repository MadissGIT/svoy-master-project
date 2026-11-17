#!/usr/bin/env bash
set -euo pipefail
mkdir -p artifacts/smoke
for app in moviesurvey svoymaster; do
  package="ru.study.$app"
  apk=$(find Homework Project -path '*/bin/Release/*' -name "$package-Signed.apk" -print -quit)
  test -n "$apk"
  adb install -r "$apk"
  adb shell am force-stop "$package"
  adb shell monkey -p "$package" -c android.intent.category.LAUNCHER 1
  ready=false
  for attempt in $(seq 1 30); do
    if adb shell uiautomator dump /sdcard/window.xml >/dev/null 2>&1 \
      && adb pull /sdcard/window.xml "artifacts/smoke/$app.xml" >/dev/null 2>&1; then
      if grep -q "package=\"$package\"" "artifacts/smoke/$app.xml"; then
        ready=true
        break
      fi
    fi
    sleep 1
  done
  if [ "$ready" != true ]; then
    adb logcat -d > "artifacts/smoke/$app.log"
    adb exec-out screencap -p > "artifacts/smoke/$app-failed.png"
    exit 1
  fi
  adb shell pidof "$package"
  adb exec-out screencap -p > "artifacts/smoke/$app.png"
done
