# DigitalBank - Motor de Servicios para Banca Digital

## Prueba Técnica: Programador Backend

Backend en **.NET 10** desarrollado bajo los principios de **Clean Architecture**, **Domain-Driven Design (Rich Domain Model)**, **Patrón Repositorio**, persistencia en **SQLite con Entity Framework Core** y documentación interactiva mediante **Scalar API Reference**.

---

## Arquitectura del Sistema

El proyecto sigue una separación estricta de responsabilidades en 4 capas desacopladas, complementadas con una suite de pruebas unitarias:

```text
prueba_tecnica-net10/
├── DigitalBank.slnx                        # Solución modular .NET 10
├── src/
│   ├── DigitalBank.Domain/                 # Núcleo: Entidades, Enums, Constantes, Excepciones y Contratos de Repositorio
│   ├── DigitalBank.Application/            # Casos de uso: DTOs, Servicios de Negocio, Generador de Cuentas y Mappings
│   ├── DigitalBank.Infrastructure/         # Persistencia: RepositoryContext, SQLite, Fluent API, BaseRepository y UnitOfWork
│   └── DigitalBank.Api/                    # Presentación: Controladores REST, GlobalExceptionHandler (IExceptionHandler) y Scalar
└── tests/
    └── DigitalBank.UnitTests/              # Suite de Pruebas Unitarias con xUnit y Moq (24 pruebas)
```

---

## Características y Requisitos Implementados

1. **Creación de Perfil de Cliente**:
   - Registro con nombre completo, fecha de nacimiento, género e ingresos mensuales.
   - Auditoría automática de creación y actualización (`CreatedAt`, `CreatedBy`, `CreatedIp`).
2. **Creación de Cuenta Bancaria con Algoritmo Único**:
   - Validación del cliente titular.
   - Algoritmo de autogeneración estricto: `ACC-YYYYMMDD-[XXXX]` (prefijo `ACC-`, fecha UTC `yyyyMMdd` y 4 dígitos aleatorios criptográficos).
   - Generación automática del movimiento de depósito inicial si el saldo inicial es mayor a cero.
3. **Consulta de Saldo en Tiempo Real**:
   - Endpoint dedicado para consultar saldo a partir del número de cuenta único.
4. **Registro de Depósitos y Retiros**:
   - Depósitos que incrementan el saldo y actualizan trazabilidad.
   - Retiros con validación estricta de fondos en el modelo de dominio. Rechazo limpio lanzando `InsufficientFundsException` si no hay saldo disponible.
5. **Historial Cronológico de Transacciones**:
   - Resumen ordenado de todos los movimientos de la cuenta.
   - Cada registro expone su identificador único, tipo (depósito/retiro), monto, marca de tiempo y el **saldo resultante histórico**.
6. **Restricciones Arquitectónicas de Alto Nivel**:
   - **Desacoplamiento total**: Prohibida la inyección de `DbContext` en los controladores; solo se consumen interfaces de servicios de aplicación.
   - **Manejo Global de Excepciones**: Uso del servicio nativo `IExceptionHandler` de ASP.NET Core para traducir errores de negocio a respuestas HTTP limpias (`400 Bad Request`, `404 Not Found`) sin bloques `try-catch` repetitivos.
   - **Persistencia SQLite**: Índice único a nivel físico en la base de datos para la columna `AccountNumber`.

---

## Requisitos Previos

- [.NET SDK 10.0](https://dotnet.microsoft.com/download) (verificado con versión 10.0.402).
- Herramienta de Entity Framework Core CLI (opcional, para migraciones manuales):

---

## Instrucciones de Ejecución

### 1. Clonar el repositorio y ubicarse en la solución

```bash
cd prueba_tecnica-net10
```

### 2. Restaurar dependencias y compilar

```bash
dotnet restore DigitalBank.slnx
dotnet build DigitalBank.slnx
```

### 3. Ejecutar las Pruebas Unitarias

```bash
dotnet test DigitalBank.slnx
```

> Todas las 24 pruebas unitarias deben completarse con éxito (100% pass).

### 4. Iniciar la API Web

```bash
dotnet run --project src/DigitalBank.Api
```

Una vez iniciada la aplicación, la base de datos local SQLite se inicializa automáticamente y la API queda escuchando en los puertos configurados (por ejemplo, `http://localhost:5217` y `https://localhost:7198`).

---

## Documentación Interactiva (Scalar API Reference)

Al ejecutar la API en modo desarrollo, puedes acceder a la interfaz moderna interactiva en tu navegador:

**`http://localhost:5217/scalar/v1`**

Desde allí puedes:

- Inspeccionar todos los contratos y esquemas DTO.
- Probar peticiones en vivo directamente en el navegador.
- Obtener snippets de código listos para consumir en C#, JavaScript/TypeScript, Python, cURL, etc.

_Nota: La especificación cruda OpenAPI en formato JSON está disponible en `http://localhost:5217/openapi/v1.json`._

---

## Endpoints de la API REST

### Clientes (`/api/customers`)

| Método | Ruta                  | Descripción                  | Payload de Entrada  |
| ------ | --------------------- | ---------------------------- | ------------------- |
| `POST` | `/api/customers`      | Registrar un nuevo cliente   | `CustomerDtoCreate` |
| `GET`  | `/api/customers/{id}` | Consultar cliente por GUID   | Ninguno             |
| `GET`  | `/api/customers`      | Listar todos los clientes    | Ninguno             |
| `PUT`  | `/api/customers/{id}` | Actualizar datos del cliente | `CustomerDtoUpdate` |

### Cuentas Bancarias y Transacciones (`/api/bank-accounts`)

| Método | Ruta                                              | Descripción                               | Payload de Entrada         |
| ------ | ------------------------------------------------- | ----------------------------------------- | -------------------------- |
| `POST` | `/api/bank-accounts`                              | Crear cuenta bancaria                     | `BankAccountDtoCreate`     |
| `GET`  | `/api/bank-accounts/{accountNumber}`              | Consultar detalles de la cuenta           | Ninguno                    |
| `GET`  | `/api/bank-accounts/{accountNumber}/balance`      | Consultar saldo actual                    | Ninguno                    |
| `PUT`  | `/api/bank-accounts/{accountNumber}`              | Actualizar estado de la cuenta            | `BankAccountDtoUpdate`     |
| `POST` | `/api/bank-accounts/{accountNumber}/deposits`     | Registrar depósito                        | `TransactionBankDtoCreate` |
| `POST` | `/api/bank-accounts/{accountNumber}/withdrawals`  | Registrar retiro                          | `TransactionBankDtoCreate` |
| `GET`  | `/api/bank-accounts/{accountNumber}/transactions` | Historial cronológico con saldo histórico | Ninguno                    |

---

## Trazabilidad y Auditoría

Todas las entidades de persistencia (`Customers`, `BankAccounts`, `TransactionsBank`) implementan la clase base `AuditableEntity`, asegurando los siguientes campos en base de datos:

- `CreatedAt`, `CreatedBy`, `CreatedIp`
- `UpdatedAt`, `UpdatedBy`, `UpdatedIp`

Las IPs se resuelven automáticamente en la capa HTTP considerando proxies (`X-Forwarded-For`) y conexiones remotas.
