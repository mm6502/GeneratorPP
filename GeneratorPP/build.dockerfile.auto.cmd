@echo off
cd ..
docker build --tag mm6502/generatorpp.auto:latest -f ./GeneratorPP/Dockerfile.Auto .
pause
