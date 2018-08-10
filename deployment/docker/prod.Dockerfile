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

# new relic env vars
ENV CORECLR_ENABLE_PROFILING=1 \
CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
CORECLR_NEWRELIC_HOME=/usr/local/newrelic-netcore20-agent \
CORECLR_PROFILER_PATH=/usr/local/newrelic-netcore20-agent/libNewRelicProfiler.so

WORKDIR /app/finance
 
# Copy over the build from the previous step 
COPY --from=build-env /app/Crossroads.Service.Finance/out . 

# copy new relic files
WORKDIR /app/newrelic
COPY ./newrelic .

RUN dpkg -i ./newrelic-netcore20-agent_8.4.880.0_amd64.deb

# then "whatever the hell this is" -dillon
ENV ASPNETCORE_URLS http://+:80
EXPOSE 80

WORKDIR /app/finance

# Run the dotnet entrypoint for the crdsfred dll 
ENTRYPOINT ["dotnet", "Crossroads.Service.Finance.dll"]