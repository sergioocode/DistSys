# Shared.Discovery

Para **evitar** tener que indicar la URL de cada servicio de forma manual ya sea en los ficheros de configuraciÃ³n o directamente en el cÃ³digo.
Podemos utilizar un `Service Discovery` (TambiÃ©n llamado Service Registry). 

En nuestro caso, vamos a utilizar `Consul`, pero puede ser cualquier otro.


# ImplementaciÃ³n de DistSys.Shared.Discovery

Para implementarlo hay que importar el paquete `DistSys.Shared.Discovery``y despuÃ©s en tu contenedor de dependencias utilizar el mÃ©todo `.AddDiscovery(Iconfiguration)`.

En la configuraciÃ³n, dentro del `appsettings` tiene que contener la siguiente informaciÃ³n:

````json
{
  ...
  "Discovery": {
    "Address": "http://localhost:8500"
  },
  ...
}
````
Donde `localhost:8500` es la localizaciÃ³n de tu Consul service.

Ahora, dentro de tu cÃ³digo Ãºnicamente debes inyectar la interfaz `IServiceDiscovery` y usarlo de la siguiente manera:

````csharp
public class YourClass
{
    private readonly IServiceDiscovery _discovery;

    public ProductsHealthCheck(IServiceDiscovery discovery)
    {
        _discovery = discovery;
    }

    public async Task Execute(CancellationToken cancellationToken = default)
    {
      
        string productsReadApi =
            await _discovery.GetFullAddress(Product.Service.Name, cancellationToken);
    }
}

````

Hay dos opciones, una para recibir la url entera, como acabamos de ver `.GetFullAddress(name, cancellationToken)`

o la opciÃ³n `.GetDiscoveryData(name, cancellationToken)` que devuelve `DiscoveryData` el cual contiene las propiedades `Server` y `Port` las cuales son definidas dentro del servicio de registro.


### Incluir informaciÃ³n en el servicio de registro consul

En nuestro caso utilizaremos Consul, puedes hacerlo de forma manual, o por la CLI con el comando 
```bash
consul services register -name=RabbitMQ -address=localhost
```

