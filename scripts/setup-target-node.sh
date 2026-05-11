#!/bin/bash
set -e

echo "=== Starting Target Node Setup ==="

echo "Updating packages..."
sudo apt-get update -y
sudo apt-get install -y ca-certificates curl gnupg ufw openssh-server

echo "Installing Docker and Docker Compose..."
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
sudo apt-get install -y docker-compose-plugin
sudo usermod -aG docker $USER

echo "Creating directory for the project..."
APP_DIR="/opt/mywebapp"
sudo mkdir -p $APP_DIR
sudo chown -R $USER:$USER $APP_DIR

echo "Setting up Docker auto-start..."
sudo systemctl enable docker
sudo systemctl start docker

echo "Creating systemd-unit for container management..."
cat <<EOF | sudo tee /etc/systemd/system/mywebapp.service > /dev/null
[Unit]
Description=My Web App Service (Docker Compose)
Requires=docker.service
After=docker.service

[Service]
Type=simple
WorkingDirectory=$APP_DIR
ExecStart=/usr/bin/docker compose up
ExecStop=/usr/bin/docker compose down
Restart=always

[Install]
WantedBy=multi-user.target
EOF

sudo systemctl daemon-reload

echo "Setting up firewall (UFW)..."
sudo ufw allow OpenSSH
sudo ufw allow 80/tcp
sudo ufw --force enable

echo ""
echo "=========================================================================================="
echo "✅ Stage of automatic preparation completed!"
echo ""
echo "STEP 1: CONFIGURING ACCESS FROM RUNNER TO TARGET NODE"
echo "For the GitHub Runner to connect to this machine via SSH and deploy code,"
echo "please follow these steps:"
echo "1. On the machine with the Runner, display the public key:"
echo "   cat ~/.ssh/id_ed25519.pub"
echo "2. On THIS machine (Target Node), create the SSH authorization files:"
echo "   mkdir -p ~/.ssh && chmod 700 ~/.ssh"
echo "   nano ~/.ssh/authorized_keys"
echo "3. Paste the Runner's public key into the open file, save it, and set the permissions:"
echo "   chmod 600 ~/.ssh/authorized_keys"
echo ""
echo "STEP 2: ADDING USER TO DOCKER GROUP"
echo "To allow the CD script to run docker commands without sudo, execute now:"
echo "   sudo usermod -aG docker \$USER"
echo "   (After this, you need to log out and back in or run 'newgrp docker')"
echo ""
echo "STEP 3: DEPLOYING WITH CD"
echo "Your GitHub Actions CD pipeline should copy docker-compose.yaml and nginx.conf"
echo "to the $APP_DIR directory, then restart the service with:"
echo "   sudo systemctl restart mywebapp.service"
echo "=========================================================================================="