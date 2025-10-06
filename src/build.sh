#!/bin/bash
#$(aws ecr get-login --no-include-email)
docker build -t khteh/cinema-reservation .
docker push khteh/cinema-reservation:latest
