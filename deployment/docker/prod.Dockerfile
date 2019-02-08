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
RUN dotnet test Crossroads.Service.Finance.Test/Crossroads.Service.Finance.Test.csproj 
RUN dotnet test MinistryPlatform.Test/MinistryPlatform.Test.csproj
RUN dotnet test Pushpay.Test/Pushpay.Test.csproj
 
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

# Install wget
RUN echo 'installing wget' \
&& apt-get update \
&& apt-get install -y wget

RUN echo 'installing gnupg' \
&& apt-get install -y gnupg

# Install new relic
RUN echo 'deb http://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
&& wget -O- https://download.newrelic.com/548C16BF.gpg | apt-key add - \
&& apt-get update \
&& apt-get install newrelic-netcore20-agent

ENV CORECLR_NEWRELIC_HOME=/usr/local/newrelic-netcore20-agent

CMD $CORECLR_NEWRELIC_HOME/run.sh dotnet Crossroads.Service.Finance.dll

# Run the dotnet entrypoint for the dll
# ENTRYPOINT ["dotnet", "Crossroads.Service.Finance.dll"]