# Millonario.NET 💰

Aplicación full-stack del clásico juego **¿Quién quiere ser millonario?** desarrollada con **.NET** (backend) y frontend separado.

## Descripción

Proyecto completo que incluye:

- **Backend (API):** Arquitectura en capas (Domain, Application, Infrastructure) con ASP.NET.
- **Frontend:** Interfaz del juego (carpeta `millonario-frontend`).

## Estructura

```
millonarioApi/
  ├── Millonario.Domain/         # Entidades y reglas de dominio
  ├── Millonario.Application/    # Casos de uso / servicios
  ├── Millonario.Infrastructure/ # Persistencia y externos
  └── millonarioApi/             # API ASP.NET

millonario-frontend/             # Frontend del juego
```

## Tecnologías

- C# / .NET
- Arquitectura limpia (Clean Architecture)
- ASP.NET Core (API)
- Frontend (según lo implementado en `millonario-frontend`)

## Cómo ejecutarlo

### Backend

1. Abre `millonarioApi/millonarioApi.sln` en Visual Studio o VS Code.
2. Restaura los paquetes NuGet.
3. Configura la cadena de conexión si es necesario.
4. Ejecuta el proyecto de la API.

### Frontend

```bash
cd millonario-frontend
# Sigue las instrucciones del package.json o README del frontend
```

## Autor

Daniel Rueda — [GitHub](https://github.com/dsrueda3691)
