#!/bin/bash

TARGET_IP=$1
if [ -z "$TARGET_IP" ]; then
    echo "ERROR: Target node IP is not provided."
    echo "Usage: ./verify-deployment.sh <target_ip>"
    exit 1
fi

EXIT_CODE=0

echo "=== Starting verification of deployment on $TARGET_IP ==="

echo "Checking service availability..."
STATUS_ROOT=$(curl -o /dev/null -s -w "%{http_code}" http://$TARGET_IP/)
if [ "$STATUS_ROOT" -eq 200 ]; then
    echo "✅ Main page is available (HTTP 200)."
else
    echo "❌ Error: Main page returned status $STATUS_ROOT."
    EXIT_CODE=1
fi

echo "Checking Nginx restrictions for /health/..."
STATUS_HEALTH=$(curl -o /dev/null -s -w "%{http_code}" http://$TARGET_IP/health/alive)
if [ "$STATUS_HEALTH" -eq 403 ]; then
    echo "✅ Access to /health/ is blocked by Nginx (HTTP 403), as configured."
else
    echo "❌ Error: Nginx should return 403 for /health/, but received $STATUS_HEALTH."
    EXIT_CODE=1
fi

echo "Checking API (/notes)..."
STATUS_NOTES=$(curl -o /dev/null -s -w "%{http_code}" http://$TARGET_IP/notes)
if [ "$STATUS_NOTES" -eq 200 ]; then
    echo "✅ API endpoint /notes is available (HTTP 200)."
else
    echo "❌ Error: Endpoint /notes is not available (HTTP $STATUS_NOTES)."
    EXIT_CODE=1
fi

echo "========================================"
if [ $EXIT_CODE -eq 0 ]; then
    echo "🎉 Verification completed successfully!"
else
    echo "⚠️ Verification detected issues in the deployment."
fi

exit $EXIT_CODE