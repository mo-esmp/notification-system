# Notification System

To run project via docker, in project root folder:
* Run `docker-compose build`
* Run `docker-compose up`
* To access swagger UI and test APIs, enter http://localhost:8080/api-docs address in browser
* To access notification job dashboard, enter http://localhost:8080/hangfire address in browser

To run project via Visual Studio:
* Change connection string in `appsettings.json` file to your SQL Server instance address 

To test APIs 40, accounts exist. First 5 accounts (1-5) are related to HR department, next 25 accounts (6-30) are related to Development department, next 5 accounts (31-35) are related to DevOps department and last 5 accounts (36-40) are related to Management department. Check out `api/v1/accounts/login` to see to how to login.
