#!/bin/bash
set -e

echo "=== Починаємо підготовку середовища для GitHub Actions Self-Hosted Runner ==="

echo "Оновлення пакетів..."
sudo apt-get update -y
sudo apt-get install -y curl tar perl jq

echo "Створення директорії ~/actions-runner..."
mkdir -p ~/actions-runner
cd ~/actions-runner

echo "Завантаження GitHub Actions Runner"
curl -o actions-runner-linux-x64-2.334.0.tar.gz -L https://github.com/actions/runner/releases/download/v2.334.0/actions-runner-linux-x64-2.334.0.tar.gz

echo "Перевірка хеш-суми..."
echo "048024cd2c848eb6f14d5646d56c13a4def2ae7ee3ad12122bee960c56f3d271  actions-runner-linux-x64-2.334.0.tar.gz" | shasum -a 256 -c

echo "Розпакування файлів..."
tar xzf ./actions-runner-linux-x64-2.334.0.tar.gz

echo "Встановлення залежностей (.NET Core та інші)..."
sudo ./bin/installdependencies.sh

echo "Перевірка SSH-ключів для деплою..."
if [ ! -f ~/.ssh/id_ed25519 ]; then
    echo "Генерація нового SSH-ключа (ed25519)..."
    ssh-keygen -t ed25519 -N "" -f ~/.ssh/id_ed25519
else
    echo "SSH-ключ вже існує."
fi

echo ""
echo "=========================================================================================="
echo "✅ Етап автоматичної підготовки завершено!"
echo "Відповідно до вимог безпеки, токен для реєстрації не зберігається у цьому скрипті."
echo ""
echo "КРОК 1: РЕЄСТРАЦІЯ РАНЕРА (ВРУЧНУ)"
echo "Виконайте у цій же директорії (~/actions-runner) команду з вашим токеном:"
echo "./config.sh --url https://github.com/BalalaievMaxim/summer-2026-devops-lab-3 --token <ВАШ_ТОКЕН>"
echo ""
echo "КРОК 2: ПІДГОТОВКА SSH ДЛЯ TARGET NODE"
echo "Скопіюйте цей публічний ключ та додайте його у файл ~/.ssh/authorized_keys на вашій цільовій віртуалці (target node):"
cat ~/.ssh/id_ed25519.pub
echo ""
echo "КРОК 3: ЗАПУСК РАНЕРА"
echo "Для запуску просто виконайте:"
echo "./run.sh"
echo "=========================================================================================="