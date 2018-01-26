# crds-service-finance

Finance for Micro-Services in dotnet core

### Install dependencies

```
dotnet restore
```

### Environment variables

create a .env file in the project root with the following variables and set
values

```
ASPNETCORE_ENVIRONMENT=Development
API_USER=
API_PASSWORD=
MP_OAUTH_BASE_URL=
MP_REST_API_ENDPOINT=
CRDS_MP_COMMON_CLIENT_ID=
PUSHPAY_CLIENT_ID=
PUSHPAY_CLIENT_SECRET=
PUSHPAY_AUTH_ENDPOINT=
PUSHPAY_API_ENDPOINT=
PUSHPAY_MERCHANT_KEY=
PUSHPAY_DEPOSIT_ENDPOINT=
FINANCE_PATH=
MP_API_DB_USER=
MP_API_DB_PASSWORD=
```

### Running the app

```sh
cd Crossroads.Service.Finance
dotnet run
# or if you want to watch files for changes
dotnet watch run
```

> Your project will run at http://localhost:5000 and a 'hello world' route can
> be found at GET http://localhost:5000/api/contact/hello

### Running tests

```sh
cd Crossroads.Service.Finance.Tests
dotnet test
# or if you want to watch test files for changes
dotnet watch test
```


## Hangfire

[Hangfire](https://www.hangfire.io/) is being used as a job scheduler due to Pushpay sending webhooks right after a donation is created in their system and it not necessarily being created in MP by their integration at the time the webhook comes in. Currently, the job checks every minute for 10 minutes to see if it created. Hangfire connects directly to the Hangfire database that sits alongside Ministry Platform.
