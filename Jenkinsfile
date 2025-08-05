// pipeline {
//   agent any

//   environment {
//     IMAGE_NAME = "foodroaster-backend"
//     DOTNET_VERSION = "8.0"
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
//           bat 'dotnet restore'
//         }
//       }
//     }

//     stage('Build') {
//       steps {
//         dir('FoodRoasterServer') {
//           bat 'dotnet build --configuration Release'
//         }
//       }
//     }

//     stage('Test') {
//       steps {
//         dir('Tests/BackedTests/UnitTests') {
//           bat 'dotnet test  --verbosity normal'
//         }
//       }  // <-- close steps block here
//     } // <-- close stage block here

//     stage('Docker Build') {
//       steps {
//         dir('FoodRoasterServer') {
//           bat "docker build -t ${IMAGE_NAME}:${BUILD_NUMBER} ."
//         }
//       }
//     }
//   }

//   post {
//     always {
//       echo "Cleaning up Docker containers..."
//       bat 'docker rm -f foodroaster-api || exit 0'
//     }
//   }
// }


pipeline {
  agent any

  environment {
    IMAGE_NAME = "foodroaster-backend"
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
          bat 'dotnet test --verbosity normal'
        }
      }
    }

    stage('Docker Build') {
      steps {
        dir('FoodRoasterServer') {
          bat "docker build -t ${IMAGE_NAME}:${BUILD_NUMBER} ."
        }
      }
    }

  //    stage('Docker Build - NGINX') {
  //   steps {
  //     dir('nginx') {
  //       bat "docker build -t foodroaster-nginx:latest ."
  //     }
  //   }
  // }
      stage('Docker Build - NGINX') {
      steps {
        bat "docker build -f nginix/Dockerfile -t foodroaster-nginx:latest nginix"
      }
    }


    stage('Deploy') {
      steps {
        script {
          // Create a .env file dynamically for Docker Compose to pick up
          writeFile file: '.env', text: "BUILD_NUMBER=${BUILD_NUMBER}"
        }

        bat """
          docker-compose -p foodroaster down
          docker-compose -p foodroaster up -d --no-build
        """
      }
    }
  }

// post {
//   always {
//     echo "Cleaning up Docker Compose containers, networks, and volumes..."
//     bat 'docker-compose -p foodroaster down || exit 0'
//   }
// }
}
