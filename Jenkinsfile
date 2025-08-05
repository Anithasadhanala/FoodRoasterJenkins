pipeline {
  agent any

  environment {
    IMAGE_NAME = "foodroaster-backend2"
    DOTNET_VERSION = "8.0"
  }

  stages {
    stage('Checkout') {
      steps {
        checkout scm
      }
    }

    stage('Restore') {
      steps {
        dir('FoodRoasterServer') {
          bat 'dotnet restore'
        }
      }
    }

    stage('Build') {
      steps {
        dir('FoodRoasterServer') {
          bat 'dotnet build --configuration Release'
        }
      }
    }

    stage('Test') {
      steps {
        dir('Tests/BackedTests/UnitTests') {
          bat 'dotnet test --no-build --verbosity normal'
        }
      }  // <-- close steps block here
    } // <-- close stage block here

    stage('Docker Build') {
      steps {
        dir('FoodRoasterServer') {
          bat "docker build -t %IMAGE_NAME% ."
        }
      }
    }
  }

  post {
    always {
      echo "Cleaning up Docker containers..."
      bat 'docker rm -f foodroaster-api || exit 0'
    }
  }
}
