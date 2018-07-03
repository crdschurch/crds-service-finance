# Step 0: build, test, and publish application 
FROM microsoft/aspnetcore-build:2.0 AS build-env 
WORKDIR /app 
 
# Copy files to /app 
COPY . ./ 
 
# Change working directory to Crossroads.Service.Finance
WORKDIR /app/Crossroads.Service.Finanace
 
# Declare args
ARG CRDS_EMBED_ENV

# NPM Install from w/in the CrdsFred dir 
#RUN npm install 
 
# Change work directory back to /app - root of proj 
WORKDIR /app 
 
# Run Unit Tests 
RUN dotnet test Crossroads.Service.Finance.Tests/Crossroads.Service.Finance.Tests.csproj 
RUN dotnet test MinistryPlatform.Tests/MinistryPlatform.Tests.csproj 
 
# Publish build to out directory 
RUN dotnet publish -c Release -o out 
 
# Step 1: Build runtime image 
FROM microsoft/aspnetcore:2.0 
WORKDIR /app 
 
# Copy over the build from the previous step 
COPY --from=build-env /app/Crossroads.Service.Finanace/out . 
 
# Run the dotnet entrypoint for the crdsfred dll 
ENTRYPOINT ["dotnet", "Crossroads.Service.Finanace.dll"]