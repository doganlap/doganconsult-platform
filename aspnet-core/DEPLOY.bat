@echo off
cd /d d:\test\aspnet-core
echo Deploying to 46.224.64.95...
echo.
scp -o StrictHostKeyChecking=no docker-compose.prod.yml root@46.224.64.95:/opt/doganconsult/docker-compose.yml
scp -o StrictHostKeyChecking=no common.props root@46.224.64.95:/opt/doganconsult/
scp -o StrictHostKeyChecking=no -r src root@46.224.64.95:/opt/doganconsult/
echo.
echo Files uploaded. Starting services...
ssh root@46.224.64.95 "cd /opt/doganconsult && docker compose build && docker compose up -d"
echo.
echo Done!
pause
