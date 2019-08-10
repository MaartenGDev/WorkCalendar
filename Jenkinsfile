pipeline {
  agent {
    kubernetes {
      label 'mypod'
      yaml """
apiVersion: v1
kind: Pod
namespace: 'jenkins'
spec:
  containers:
  - name: docker
    image: docker:1.11
    command: ['cat']
    tty: true
    volumeMounts:
    - name: dockersock
      mountPath: /var/run/docker.sock
  volumes:
  - name: dockersock
    hostPath:
      path: /var/run/docker.sock
"""
    }
  }
  environment {
    GIT_COMMIT_SHORT = sh(
                  script: "printf \$(git rev-parse --short ${GIT_COMMIT})",
                  returnStdout: true
    ) 
    DOCKER_IMAGE_NAME = "maartendev/work-calendar"
    DOCKER_REGISTRY= "340535573758.dkr.ecr.eu-west-1.amazonaws.com"
    DOCKER_IMAGE_URL = "${env.DOCKER_REGISTRY}/${env.DOCKER_IMAGE_NAME}"
  }
  stages {
        stage('Build') {
            steps {
                container('docker') {
                    script {
                        app = docker.build("${DOCKER_IMAGE_NAME}:${env.GIT_COMMIT_SHORT}")
                    }
                }
            }
        }
        stage('Push Docker Image') {
            steps {
                container('docker') {
                    script {
                        docker.withRegistry("https://${DOCKER_REGISTRY}", 'ecr:eu-west-1:docker_registry_login') {
                            app.push("${env.GIT_COMMIT_SHORT}")
                            app.push("latest")
                        }
                    }
                }
            }
        }
        stage('Deploy') {
            steps {
                kubernetesDeploy(
                    kubeconfigId: 'kubeconfig',
                    configs: 'work-calendar-k8s.yml',
                    enableConfigSubstitution: true
                )
            }
        }
    }
}