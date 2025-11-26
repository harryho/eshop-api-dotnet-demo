#!/usr/bin/env bash
#
# Test runner for Eshop API project using .NET 8 SDK
#
# Usage:
#   ./test.sh                    - Run all tests with build
#   ./test.sh --no-build         - Run tests without rebuilding
#   ./test.sh --coverage         - Run tests with code coverage
#   ./test.sh --filter "Pattern" - Run specific tests matching pattern
#   ./test.sh --detailed         - Show detailed test output
#

set -e  # Exit on error

# ANSI color codes
CYAN='\033[0;36m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
GRAY='\033[0;37m'
NC='\033[0m' # No Color

# Parse arguments
NO_BUILD=false
DETAILED=false
COVERAGE=false
FILTER=""

while [[ $# -gt 0 ]]; do
    case $1 in
        --no-build|-n)
            NO_BUILD=true
            shift
            ;;
        --detailed|-d)
            DETAILED=true
            shift
            ;;
        --coverage|-c)
            COVERAGE=true
            shift
            ;;
        --filter|-f)
            FILTER="$2"
            shift 2
            ;;
        --help|-h)
            echo "Usage: ./test.sh [OPTIONS]"
            echo ""
            echo "Options:"
            echo "  --no-build, -n           Skip building (faster for repeated runs)"
            echo "  --detailed, -d           Show detailed test output"
            echo "  --coverage, -c           Collect code coverage and generate report"
            echo "  --filter PATTERN, -f     Filter tests by name pattern"
            echo "  --help, -h               Show this help message"
            echo ""
            echo "Examples:"
            echo "  ./test.sh"
            echo "  ./test.sh --no-build"
            echo "  ./test.sh --coverage"
            echo "  ./test.sh --filter 'GetProductV1*' --detailed"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            echo "Use --help for usage information"
            exit 1
            ;;
    esac
done

echo -e "${CYAN}🔧 Eshop API Test Runner${NC}"
echo -e "${CYAN}=========================${NC}"
echo ""

# Check for running dotnet processes and stop them (Linux/macOS)
echo -e "${CYAN}🔍 Checking for running dotnet processes...${NC}"
if pgrep -x dotnet > /dev/null; then
    PROCESS_COUNT=$(pgrep -x dotnet | wc -l)
    echo -e "${YELLOW}   Found $PROCESS_COUNT dotnet process(es)${NC}"
    echo -e "${CYAN}   Stopping dotnet processes...${NC}"
    pkill -9 dotnet 2>/dev/null || true
    sleep 0.5
    
    # Verify they're stopped
    if pgrep -x dotnet > /dev/null; then
        echo -e "${YELLOW}   ⚠️  Warning: Some dotnet processes are still running${NC}"
    else
        echo -e "${GREEN}   ✓ All dotnet processes stopped${NC}"
    fi
else
    echo -e "${GREEN}   ✓ No dotnet processes running${NC}"
fi
echo ""

# Verify .NET version
echo -e "${CYAN}📌 Checking .NET SDK version...${NC}"
DOTNET_VERSION=$(dotnet --version)
echo -e "${GREEN}   SDK: $DOTNET_VERSION${NC}"
echo ""

# Build test command
TEST_ARGS="test"

if [ "$NO_BUILD" = true ]; then
    TEST_ARGS="$TEST_ARGS --no-build"
    echo -e "${CYAN}⚡ Running tests (skipping build)...${NC}"
else
    echo -e "${CYAN}🔨 Building and running tests...${NC}"
fi

if [ "$DETAILED" = true ]; then
    TEST_ARGS="$TEST_ARGS --verbosity detailed"
fi

if [ -n "$FILTER" ]; then
    TEST_ARGS="$TEST_ARGS --filter $FILTER"
    echo -e "${CYAN}🔍 Filter: $FILTER${NC}"
fi

if [ "$COVERAGE" = true ]; then
    TEST_ARGS="$TEST_ARGS --collect:\"XPlat Code Coverage\""
    echo -e "${CYAN}📊 Coverage collection enabled${NC}"
fi

echo ""

# Run tests
eval dotnet $TEST_ARGS
TEST_EXIT_CODE=$?

if [ $TEST_EXIT_CODE -eq 0 ]; then
    echo ""
    echo -e "${GREEN}✅ All tests passed!${NC}"
    
    # Generate coverage report if requested
    if [ "$COVERAGE" = true ]; then
        echo ""
        echo -e "${CYAN}📊 Generating coverage report...${NC}"
        
        # Find the latest coverage file
        COVERAGE_FILE=$(find Eshop.Api.Test/TestResults -name "coverage.cobertura.xml" -type f -print0 2>/dev/null | xargs -0 ls -t | head -n 1)
        
        if [ -n "$COVERAGE_FILE" ]; then
            echo -e "${GRAY}   Coverage file: $COVERAGE_FILE${NC}"
            
            # Check if reportgenerator is installed
            if ! dotnet tool list -g | grep -q "dotnet-reportgenerator-globaltool"; then
                echo ""
                echo -e "${YELLOW}⚠️  Installing ReportGenerator tool...${NC}"
                dotnet tool install -g dotnet-reportgenerator-globaltool
            fi
            
            # Generate HTML report
            REPORT_PATH="TestResults/CoverageReport"
            reportgenerator "-reports:$COVERAGE_FILE" "-targetdir:$REPORT_PATH" "-reporttypes:Html;TextSummary"
            
            if [ $? -eq 0 ]; then
                echo ""
                echo -e "${GREEN}✅ Coverage report generated!${NC}"
                echo -e "${CYAN}   Report location: $REPORT_PATH/index.html${NC}"
                
                # Display summary if available
                SUMMARY_FILE="$REPORT_PATH/Summary.txt"
                if [ -f "$SUMMARY_FILE" ]; then
                    echo ""
                    echo -e "${CYAN}📈 Coverage Summary:${NC}"
                    cat "$SUMMARY_FILE" | sed "s/^/${GRAY}/" | sed "s/$/${NC}/"
                fi
                
                # Ask to open report (works on macOS and some Linux distros)
                echo ""
                read -p "Open coverage report in browser? (y/n) " -n 1 -r
                echo
                if [[ $REPLY =~ ^[Yy]$ ]]; then
                    if command -v xdg-open > /dev/null; then
                        xdg-open "$REPORT_PATH/index.html"
                    elif command -v open > /dev/null; then
                        open "$REPORT_PATH/index.html"
                    else
                        echo -e "${YELLOW}   Could not detect browser opener. Please open manually: $REPORT_PATH/index.html${NC}"
                    fi
                fi
            else
                echo -e "${YELLOW}⚠️  Failed to generate coverage report${NC}"
            fi
        else
            echo -e "${YELLOW}⚠️  Coverage file not found. Make sure coverlet.collector is installed.${NC}"
        fi
    fi
    
    exit 0
else
    echo ""
    echo -e "${RED}❌ Tests failed with exit code: $TEST_EXIT_CODE${NC}"
    exit $TEST_EXIT_CODE
fi
