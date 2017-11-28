# crds-service-template

Template for Micro-Services in dotnet core

### Install dependencies

```
dotnet restore
```

### Environment variables

create a .env file in the project root with the following variables and set
values

```
API_USER=
API_PASSWORD=
MP_OAUTH_BASE_URL=
MP_REST_API_ENDPOINT=
CRDS_MP_COMMON_CLIENT_ID=
```

### Running the app

```
cd Crossroads.Service.Template
dotnet run
```

> Your project will run at http://localhost:49266 and a 'hello world' route can
> be found at GET http://localhost:49266/api/hello

### Running tests

```
cd Crossroads.Service.Template.Tests
dotnet test
```

or with live-reload:

```
dotnet watch test
```
