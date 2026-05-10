# Shared.Communication

Para permitir **comunicaciÃ³n asÃ­ncrona** creamos una abstracciÃ³n sobre la lÃ³gica del patrÃ³n `Producers/Consumers`.

Esta lÃ³gica debe ser implementada por la abstracciÃ³n que deseemos utilizar para implementar el **service bus**. En nuestro caso RabbitMQ, pero puede ser cualquier otra como Kafka, ActiveMQ, Mosqito, etc.

## Tipos de mensajes
Es comÃºn, cuando creamos sistemas ditribuidos crear lo que se denomina mensajes de integraciÃ³n (`IntegrationMessages`), para los eventos que van fuera de nuestro dominio, y mensajes de dominio (`DomainMessages`) para los que son dentro de nuestro dominio.

Otro ejemplo muy comÃºn es cuando separamos las lecturas (reads) de las escrituras(writes) en la base de datos.
1. A travÃ©s de nuestro caso de uso guardamos la informacion en la `WriteStore`
2. Generamos un evento/mensaje de dominio, el cual es interceptado por un handler que lo insertara en la `ReadStore`

### IntegrationMessages
Son aquellos mensajes que vamos a generar dentro de nuestro dominio pero van a ser escuchados por otros servicios.

Ten en cuenta que el microservicio que ejecuta los mensajes de integraciÃ³n nunca va a escucharlos, sino que va a escuchar los mensajes de integraciÃ³n de otros servivcios.

Y ten en cuenta que X nÃºmero de microservicios pueden estar escuchando el mismo mensaje de integraciÃ³n.




### DomainMessages
Son aquellos mensajes que vamos a generar para que el dominio los escuche. 

Algunas veces tenemos aplicaciones que cruzan dominios; No es lo recomendable, pero puede pasar.


### Contenido de los mensjaes
Debido a la naturaleza de nuestra aplicaciÃ³n la estructura de los mensajes es la misma, pero en ambientes de producciÃ³n mas complejos lo mÃ¡s comÃºn es que sea diferente.

Los mensajes contienen los siguientes atributos:
- MessageIdentifier: Identificador Ãºnico del mensaje. No esta relacionado con ningÃºn ID.
- Name: nombre del Evento. Util para aplicaciones externas como por ejemplo los logs, para ser capaces de localizarlo
- Metadata: Contiene la informaciÃ³n generada por el mensaje original, como la fecha de creaciÃ³n, en un ambiente con mÃºltples tenants, deberÃ­a contener el tenant.
- Content: Contiene el mensaje como tal, osea `T`.

## ConfiguraciÃ³n con RabbitMQ
Para conectarnos a rabbitMQ debemos especifiar el host, usuario y password. 
Ello lo debemos hacer en una seccion de `appsettings` llamada `Bus:RabbitMQ`:

````json

"Bus": {
    "RabbitMQ": {
      "Hostname" : "localhost",
      "Username": "DistSysAdmin",
      "Password" : "DistSysPass",
    }
  }
````

### Publisher
Debe incluir a la secciÃ³n anterior una subseccion llamada `Publisher`. La cual contendra la informaciÃ³n del exange al que vas a enviar dichos eventos.

````json
 "RabbitMQ": {
      ...
      "Publisher": {
        "IntegrationExchange": "name.exange",
        "DomainExchange" : "another.name"
      }
````

Nota: si eliminas una propiedad, esta serÃ¡ null, y el cÃ³digo funcionara igual, pero no publicara, obviamente.


#### Domain
Para publicar mensajes debes incluir `Services.AddServiceBusDomainPublisher(Iconfiguration);` e inyectar la interfaz `IDomainMessagePublisher`
#### Integration
Para publicar mensajes debes incluir `Services.AddServiceBusIntegrationPublisher(Iconfiguration);` e inyectar la interfaz `IIntegrationMessagePublisher`


Finalmente utiilzar `_publisher.Publish(T)` para publicar mensajes.

### Consumer
para consumir mensajes, debes crear un controller y heredar de  `ConsumerController<T>` donde `T` es o `IntegrationMessage`o `DomainMessage`.
AdemÃ¡s la configuraciÃ³n es la siguiente:
````json
 "RabbitMQ": {
        ...
      "Consumer": {
        "IntegrationExchanges" : "int.exchange",
        "IntegrationQueue" : "integration-queue"
        "DomainExchanges" : "dom.exchange",
        "DomainQueue" : "domain-queue"
      }
    }

````

debes incluir en el contenedor de dependencias o bien `Services.AddServiceBusIntegrationConsumer(Iconfiguration);` o `Services.AddServiceBusDomainConsumer(Iconfiguration);` asÃ­ como incluir los handlers.

#### Handler
Para procesar los mensajes, debes crear una clase que herede de `IIntegrationMessageHandler<T>` o `: IDomainMessageHandler<T>` donde `T` es el tipo de la clase que quieres procesar.

por ejemplo un handler con `IIntegrationMessageHandler<SubscriptionDto>` procesara todos los mensajes del tipo `SbuscriptionDto`.

