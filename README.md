# Extract Jenkins jobs .NET5

This script allows to extract Jenkins job names from Jenkins API.  
To generate Jenkins API token (password) for user please use this [link](https://www.jenkins.io/blog/2018/07/02/new-api-token-system/).

Program requires two parameters user and API token to access Jenkins server.  
Execution example:

```batch
jenkins-extract-jobs.exe admin "11c4d97ce9b3dbc4d675d6fab32ebd9a4a" "http://localhost:8080/"
```

## Important notice

This script does not include any error processing. This programm should be used only as an example how to get data from Jenkins API.  
Any improvement are welcome.

## Docker setup for testing

```bash
docker run -d -p 8080:8080 jenkins/jenkins
```

Create user admin/admin.  
Create token "http://localhost:8080/user/admin/configure"
