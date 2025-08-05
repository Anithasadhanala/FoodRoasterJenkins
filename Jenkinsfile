// // pipeline {
// //   agent any

// //   environment {
// //     IMAGE_NAME = "foodroaster-backend"
// //     DOTNET_VERSION = "8.0"
// //   }

// //   stages {
// //     stage('Checkout') {
// //       steps {
// //         checkout scm
// //       }
// //     }

// //     stage('Restore') {
// //       steps {
// //         dir('FoodRoasterServer') {
// //           bat 'dotnet restore'
// //         }
// //       }
// //     }

// //     stage('Build') {
// //       steps {
// //         dir('FoodRoasterServer') {
// //           bat 'dotnet build --configuration Release'
// //         }
// //       }
// //     }

// //     stage('Test') {
// //       steps {
// //         dir('Tests/BackedTests/UnitTests') {
// //           bat 'dotnet test --no-build --verbosity normal'
// //         }
// //       }
// //     }

// //     stage('Docker Build') {
// //       steps {
// //         dir('FoodRoasterServer') {
// //           bat "docker build -t %IMAGE_NAME% ."
// //         }
// //       }
// //     }
// //   }

// //   post {
// //     always {
// //       echo "Cleaning up Docker containers..."
// //       bat 'docker rm -f foodroaster-api || exit 0'
// //     }
// //   }
// // }



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
//           bat '''
//           REM Generate timestamp
//           set TIMESTAMP=%DATE:~10,4%-%DATE:~4,2%-%DATE:~7,2%_%TIME:~0,2%-%TIME:~3,2%-%TIME:~6,2%
//           set TIMESTAMP=%TIMESTAMP: =0%
//           set RESULTS_DIR=TestResults\\%TIMESTAMP%
//           mkdir %RESULTS_DIR%

//           REM Run tests and save results
//           dotnet test --no-build --logger "trx;LogFileName=test_results.trx" --results-directory %RESULTS_DIR%

//           REM Install trx2junit if not already installed
//           dotnet tool install --global trx2junit --ignore-failed-sources

//           REM Add tools to PATH
//           set PATH=%PATH%;%USERPROFILE%\\.dotnet\\tools

//           REM Convert .trx to JUnit XML
//           trx2junit %RESULTS_DIR%\\test_results.trx
//           '''
//         }
//       }
//     }

//     stage('Docker Build') {
//       steps {
//         dir('FoodRoasterServer') {
//           bat "docker build -t %IMAGE_NAME% ."
//         }
//       }
//     }
//   }

//   post {
//     always {
//       echo "Cleaning up Docker containers..."
//       bat 'docker rm -f -api || exit 0'

//       script {
//         // Dynamically find the latest test results folder
//         def resultsFolder = bat(script: 'for /f "delims=" %i in (\'dir /b /ad /o:-d Tests\\BackedTests\\UnitTests\\TestResults\') do @echo %i & goto :eof', returnStdout: true).trim()

//         // Publish test results to Jenkins UI
//         junit testResults: "Tests/BackedTests/UnitTests/TestResults/${resultsFolder}/test_results.xml"

//         // Optional: archive HTML report if generated
//         archiveArtifacts artifacts: "Tests/BackedTests/UnitTests/TestResults/${resultsFolder}/Report/**", fingerprint: true
//       }
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
          bat '''
          REM === Generate timestamp ===
          for /f "tokens=1-4 delims=/ " %%a in ("%date%") do set dt=%%d-%%b-%%c_%%a
          for /f "tokens=1-2 delims=: " %%a in ("%time%") do set tm=%%a-%%b
          set TIMESTAMP=%dt%_%tm%
          set TIMESTAMP=%TIMESTAMP: =0%
          set RESULTS_DIR=TestResults\\%TIMESTAMP%
          mkdir %RESULTS_DIR%

          REM === Run tests and output trx file ===
          dotnet test --no-build --logger "trx;LogFileName=test_results.trx" --results-directory %RESULTS_DIR%

          REM === Install tools if missing ===
          dotnet tool install --global trx2junit --ignore-failed-sources

          REM === Add tools to PATH ===
          set PATH=%PATH%;%USERPROFILE%\\.dotnet\\tools

          REM === Convert trx to junit XML ===
          trx2junit %RESULTS_DIR%\\test_results.trx
          '''
        }
      }
    }

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
      bat 'docker rm -f api || exit 0'

      script {
        echo "Publishing test results..."
        try {
          def resultsFolder = bat(
            script: '''
              @echo off
              for /f "delims=" %%i in ('dir /b /ad /o:-d Tests\\BackedTests\\UnitTests\\TestResults') do (
                echo %%i
                goto :EOF
              )
            ''',
            returnStdout: true
          ).trim()

          junit testResults: "Tests/BackedTests/UnitTests/TestResults/${resultsFolder}/test_results.xml"

          archiveArtifacts artifacts: "Tests/BackedTests/UnitTests/TestResults/${resultsFolder}/Report/**", fingerprint: true
        } catch (Exception e) {
          echo "⚠️ Could not find or publish test results: ${e.message}"
        }
      }
    }
  }
}
