#!/usr/bin/env bash
#
# Build script for Eshop API project using .NET 8 SDK
#
# Usage:
#   ./scripts/build.sh           - Build in Debug mode
#   ./scripts/build.sh --clean   - Clean before building
#   ./scripts/build.sh --release - Build in Release configuration
#

set -e  # Exit on error

# ANSI color codes
CYAN='\033[0;36m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Parse arguments
CLEAN=false
RELEASE=false

for arg in "$@"; do
    case $arg in
        --clean|-c)
            CLEAN=true
            shift
            ;;
        --release|-r)
            RELEASE=true
            shift
            ;;
        --help|-h)
            echo "Usage: ./scripts/build.sh [OPTIONS]"
            echo ""
            echo "Options:"
            echo "  --clean, -c     Clean before building"
            echo "  --release, -r   Build in Release configuration"
            echo "  --help, -h      Show this help message"
            exit 0
            ;;
        *)
            ;;
    esac
done

echo -e "${CYAN}🔧 Eshop API Builder${NC}"
echo -e "${CYAN}====================${NC}"
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

# Clean if requested
if [ "$CLEAN" = true ]; then
    echo -e "${CYAN}🧹 Cleaning solution...${NC}"
    dotnet clean
    if [ $? -ne 0 ]; then
        echo -e "${RED}❌ Clean failed${NC}"
        exit 1
    fi
    echo ""
fi

# Build
CONFIGURATION="Debug"
if [ "$RELEASE" = true ]; then
    CONFIGURATION="Release"
fi

echo -e "${CYAN}🔨 Building solution ($CONFIGURATION)...${NC}"

BUILD_ARGS="build"
if [ "$RELEASE" = true ]; then
    BUILD_ARGS="build --configuration Release"
fi

dotnet $BUILD_ARGS

if [ $? -eq 0 ]; then
    echo ""
    echo -e "${GREEN}✅ Build successful!${NC}"
    exit 0
else
    echo ""
    echo -e "${RED}❌ Build failed${NC}"
    exit 1
fi
