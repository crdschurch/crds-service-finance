# Step 0: build, test, and publish application 
FROM microsoft/aspnetcore-build:2.0 AS build-env 
WORKDIR /app 
 
# Copy files to /app 
COPY . ./ 
 
# Change working directory to Crossroads.Service.Finance
WORKDIR /app/Crossroads.Service.Finance
 
# Declare args
# ARG CRDS_EMBED_ENV

# NPM Install from w/in the CrdsFred dir 
#RUN npm install 
 
# Change work directory back to /app - root of proj 
WORKDIR /app 
 
# Run Unit Tests 
# RUN dotnet test Crossroads.Service.Finance.Test/Crossroads.Service.Finance.Test.csproj 
# RUN dotnet test MinistryPlatform.Test/MinistryPlatform.Test.csproj 
 
# Publish build to out directory 
RUN dotnet publish -c Release -o out 
 
# Step 1: Build runtime image 
FROM microsoft/aspnetcore:2.0 
WORKDIR /app 
 
# Copy over the build from the previous step 
COPY --from=build-env /app/Crossroads.Service.Finance/out . 
 
# Run the dotnet entrypoint for the crdsfred dll 
ENTRYPOINT ["dotnet", "Crossroads.Service.Finance.dll"]