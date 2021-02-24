#!/bin/bash

docker-compose build && docker-compose run web dotnet watch run -r linux-x64 --urls "http://*:5000;https://*:5001"