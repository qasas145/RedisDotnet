# Redis With Dotnet 

> First you have to install redis 

***Run these commands***

- `docker network create my-network`

- `docker container create --network my-network -h my-redis --name my-redis redis `

- `docker container start my-redis`

- `docker container exec -it my-redis redis-cli`
