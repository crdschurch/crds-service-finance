# crds-service-finance

[![Teamcity Status](<https://ci.crossroads.net/app/rest/builds/buildType:(id:Api_Finance_Deploy_2_1Integration)/statusIcon.svg>)](https://ci.crossroads.net/viewType.html?buildTypeId=Api_Finance_Deploy_2_1Integration)

Finance for Micro-Services in dotnet core

### Install dependencies

```
dotnet restore
```

### See endpoints via swagger

run project, visit `http://localhost:<port>`

### Environment variables

copy the .env.example file to a .env file at the root directory, filling out the values for each

### Running the app

```sh
cd Crossroads.Service.Finance
dotnet run
# or if you want to watch files for changes
dotnet watch run
```

### Running tests

```sh
cd Crossroads.Service.Finance.Test
dotnet test
# or if you want to watch test files for changes
dotnet watch test
```


## Hangfire

[Hangfire](https://www.hangfire.io/) is being used as a job scheduler due to Pushpay sending webhooks right after a donation is created in their system and it not necessarily being created in MP by their integration at the time the webhook comes in. Currently, the job checks every minute for 10 minutes to see if it created. Hangfire connects directly to the Hangfire database that sits alongside Ministry Platform.

## Container

##### build an image

```
docker-compose -f deployment/docker/docker-compose-prod.yml build
```

##### create container, run image

```
docker-compose -f deployment/docker/docker-compose-prod.yml up
```
 