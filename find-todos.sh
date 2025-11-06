#!/bin/bash
echo "========================================"
echo "TODO Finder - Scanning for TODOs..."
echo "========================================"
echo

cd "$(dirname "$0")"
dotnet run --project TodoAttribute.Sol/TodoFinder/TodoFinder.csproj

echo
echo "========================================"
read -p "Press Enter to exit..."
