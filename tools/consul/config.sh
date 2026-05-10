#!/bin/bash
# Infrastructure
docker exec -it consul consul services register -name=RabbitMQ -address=localhost -port=5672
docker exec -it consul consul services register -name=SecretManager -address=localhost -port=8200
docker exec -it consul consul services register -name=MySql -address=localhost -port=3307
docker exec -it consul consul services register -name=MongoDb -address=localhost -port=27017
docker exec -it consul consul services register -name=Graylog -address=localhost -port=12201
docker exec -it consul consul services register -name=OpenTelemetryCollector -address=localhost -port=4317

# Services
docker exec -it consul consul services register -name=EmailsApi -address=localhost -port=60120
docker exec -it consul consul services register -name=ProductsApiWrite -address=localhost -port=60320
docker exec -it consul consul services register -name=ProductsApiRead -address=localhost -port=60321
docker exec -it consul consul services register -name=OrdersApi -address=localhost -port=60220
docker exec -it consul consul services register -name=SubscriptionsApi -address=localhost -port=60420