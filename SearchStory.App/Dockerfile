# FROM mcr.microsoft.com/dotnet/sdk:5.0

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal
# RUN apt update && apt install -y nodejs npm
ENV NODE_VERSION=12.6.0
RUN apt install -y curl
RUN curl -o- https://raw.githubusercontent.com/creationix/nvm/v0.34.0/install.sh | bash
ENV NVM_DIR=/root/.nvm
RUN . "$NVM_DIR/nvm.sh" && nvm install ${NODE_VERSION}
RUN . "$NVM_DIR/nvm.sh" && nvm use v${NODE_VERSION}
RUN . "$NVM_DIR/nvm.sh" && nvm alias default v${NODE_VERSION}
ENV PATH="/root/.nvm/versions/node/v${NODE_VERSION}/bin/:${PATH}"
RUN node --version
RUN npm --version
COPY . /app
WORKDIR /app
RUN ["dotnet", "build", "-r", "linux-x64"]
ENV DOTNET_USE_POLLING_FILE_WATCHER=true  
ENV DOTNET_URLS="http://0.0.0.0:5000;https://0.0.0.0:5001"
ENV ASPNETCORE_URLS="http://0.0.0.0:5000;https://0.0.0.0:5001"
EXPOSE 5000/tcp
EXPOSE 5001/tcp
RUN chmod +x ./entrypoint.sh
CMD /bin/bash ./entrypoint.sh