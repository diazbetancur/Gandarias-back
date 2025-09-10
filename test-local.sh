#!/bin/bash
# Local Testing Script para Gandarias API antes de App Runner deployment

echo "?? Testing Gandarias API locally before deployment..."
echo "=================================================="

# Function to check if Docker is running
check_docker() {
    if ! docker info > /dev/null 2>&1; then
        echo "? Docker is not running. Please start Docker first."
        exit 1
    fi
    echo "? Docker is running"
}

# Function to build and test locally
test_local() {
    echo "?? Building Docker image..."
    if docker build -t gandarias-api-test .; then
        echo "? Docker image built successfully"
    else
        echo "? Docker build failed"
        exit 1
    fi
    
    echo "?? Starting container on port 5000..."
    docker run -d -p 5000:80 \
        -e ASPNETCORE_ENVIRONMENT=Development \
        -e DATABASE_URL="Host=postgres;Port=5432;Username=test;Password=test;Database=test" \
        -e JWT_SECRET_KEY="test-key-for-local-development" \
        --name gandarias-test \
        gandarias-api-test
    
    echo "? Waiting for application to start..."
    sleep 10
    
    # Test health endpoint
    echo "?? Testing health endpoint..."
    if curl -f http://localhost:5000/health > /dev/null 2>&1; then
        echo "? Health check passed!"
    else
        echo "? Health check failed!"
        docker logs gandarias-test
        cleanup
        exit 1
    fi
    
    # Test swagger endpoint
    echo "?? Testing Swagger endpoint..."
    if curl -f http://localhost:5000/swagger/index.html > /dev/null 2>&1; then
        echo "? Swagger endpoint accessible!"
    else
        echo "??  Swagger endpoint not accessible (might be normal in production mode)"
    fi
    
    echo "? All tests passed!"
    echo "?? Local API available at: http://localhost:5000"
    echo "?? Local Swagger at: http://localhost:5000/swagger"
    
    cleanup
}

# Function to cleanup
cleanup() {
    echo "?? Cleaning up..."
    docker stop gandarias-test > /dev/null 2>&1
    docker rm gandarias-test > /dev/null 2>&1
    docker rmi gandarias-api-test > /dev/null 2>&1
    echo "? Cleanup completed"
}

# Main execution
main() {
    check_docker
    test_local
    echo ""
    echo "?? Ready for AWS App Runner deployment!"
    echo "?? Run: ./deploy-app-runner.sh"
}

# Run if not sourced
if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
    main "$@"
fi