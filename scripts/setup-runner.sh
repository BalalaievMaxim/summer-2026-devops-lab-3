#!/bin/bash
set -e

echo "=== Starting GitHub Actions Self-Hosted Runner Setup ==="

echo "Updating packages..."
sudo apt-get update -y
sudo apt-get install -y curl tar perl jq

echo "Creating directory ~/actions-runner..."
mkdir -p ~/actions-runner
cd ~/actions-runner

echo "Downloading GitHub Actions Runner"
curl -o actions-runner-linux-x64-2.334.0.tar.gz -L https://github.com/actions/runner/releases/download/v2.334.0/actions-runner-linux-x64-2.334.0.tar.gz

echo "Checking hash sum..."
echo "048024cd2c848eb6f14d5646d56c13a4def2ae7ee3ad12122bee960c56f3d271  actions-runner-linux-x64-2.334.0.tar.gz" | shasum -a 256 -c

echo "Extracting files..."
tar xzf ./actions-runner-linux-x64-2.334.0.tar.gz

echo "Installing dependencies..."
sudo ./bin/installdependencies.sh

echo "Checking SSH keys for deployment..."
if [ ! -f ~/.ssh/id_ed25519 ]; then
    echo "Generating new SSH key (ed25519)..."
    ssh-keygen -t ed25519 -N "" -f ~/.ssh/id_ed25519
else
    echo "SSH key already exists."
fi

echo ""
echo "=========================================================================================="
echo "✅ Stage of automatic preparation completed!"
echo "According to security requirements, the token for registration is not stored in this script."
echo ""
echo "STEP 1: REGISTER THE RUNNER (MANUALLY)"
echo "Execute the command in the same directory (~/actions-runner) with your token:"
echo "./config.sh --url https://github.com/BalalaievMaxim/summer-2026-devops-lab-3 --token <YOUR_TOKEN>"
echo ""
echo "STEP 2: PREPARE SSH FOR TARGET NODE"
echo "Copy this public key and add it to the ~/.ssh/authorized_keys file on your target node:"
cat ~/.ssh/id_ed25519.pub
echo ""
echo "STEP 3: START THE RUNNER"
echo "To start the runner, simply execute:"
echo "./run.sh"
echo "=========================================================================================="