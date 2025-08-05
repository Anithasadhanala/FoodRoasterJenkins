// pipeline {
//   agent any

//   environment {
//     IMAGE_NAME = "foodroaster-backend"
//     DOTNET_VERSION = "8.0" // adjust if needed
//   }

//   stages {
//     stage('Checkout') {
//       steps {
//         checkout scm
//       }
//     }

//     stage('Restore') {
//       steps {
//         dir('FoodRoasterServer') {
//           sh 'dotnet restore'
//         }
//       }
//     }

//     stage('Build') {
//       steps {
//         dir('FoodRoasterServer') {
//           sh 'dotnet build --configuration Release'
//         }
//       }
//     }

//     stage('Test') {
//       steps {
//         dir('Tests/BackedTests/UnitTests') {
//           sh 'dotnet test --no-build --verbosity normal'
//         }
//       }
//     }

//     stage('Docker Build') {
//       steps {
//         dir('FoodRoasterServer') {
//           sh 'docker build -t $IMAGE_NAME .'
//         }
//       }
//     }

//   }

//   post {
//     always {
//       echo "Cleaning up Docker containers..."
//       sh 'docker rm -f foodroaster-api || true'
//     }
//   }
// }



pipeline {
  agent any

  environment {
    IMAGE_NAME = "foodroaster-backend"
    // .NET 8.0 is used for build
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
          sh 'dotnet restore'
        }
      }
    }

    stage('Build') {
      steps {
        dir('FoodRoasterServer') {
          sh 'dotnet build --configuration Release'
        }
      }
    }

    stage('Test') {
      steps {
        dir('Tests/BackedTests/UnitTests') {
          sh 'dotnet test --no-build --verbosity normal'
        }
      }
    }

    stage('Docker Build') {
      steps {
        dir('FoodRoasterServer') {
          sh "docker build -t ${IMAGE_NAME} ."
        }
      }
    }
  }

  post {
    always {
      echo "Cleaning up Docker containers..."
      sh 'docker rm -f foodroaster-api || true'
    }
  }
}

