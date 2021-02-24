#!/bin/bash

    docker-compose build && \
        docker-compose run web dotnet publish -r linux-x64 && \
        docker-compose run web dotnet publish -r win-x64 -p:PublishReadtyToRun=false
