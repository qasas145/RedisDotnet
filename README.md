# Redis With Dotnet 

> First you have to install redis 

***For installation run these commands***

- `docker network create my-network`

- `docker container create --network my-network -h my-redis --name my-redis redis `

- `docker container start my-redis`

- `docker container exec -it my-redis redis-cli`


> if you want to see the mapped commands of dotnet ones in redis run `monitor` in redis and start the Dotnet application