# microservices-dot-net

Project in .NET 7, made of 2 microservices (platforms and commands) that can communicate using RabbitMQ and GRPC.
It includes also postman collection to better test the code.

In particular:
- a get and a create platform endpoints are exposed
- a get and a create command for a platform endpoints are exposed
- rabbitMQ is used to keep the list of platform syncronized in the commands service respect to the one in the platform service
- grpc is used with commands service acting as client in order to get all the platforms at the application startup

Credits to Les Jackson for its tutorial
