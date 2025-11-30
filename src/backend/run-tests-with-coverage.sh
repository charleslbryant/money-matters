#!/bin/bash

# Script to run all tests with code coverage reporting
# Requires: dotnet-coverage tool (install with: dotnet tool install -g dotnet-coverage)

echo "Running all tests with code coverage..."
echo "========================================"

dotnet test MoneyMatters.sln \
  --collect:"XPlat Code Coverage" \
  --results-directory ./TestResults \
  --logger "console;verbosity=detailed"

echo ""
echo "Test run complete!"
echo "Coverage results are in ./TestResults directory"
echo ""
echo "To view coverage reports:"
echo "1. Install ReportGenerator: dotnet tool install -g dotnet-reportgenerator-globaltool"
echo "2. Generate HTML report: reportgenerator -reports:./TestResults/**/coverage.cobertura.xml -targetdir:./TestResults/CoverageReport -reporttypes:Html"
echo "3. Open ./TestResults/CoverageReport/index.html in your browser"
