# Sensor Data Ingestion API

## Integrantes do Grupo

- Saulo Szmyhiel Ganança
- Leonardo Bernardes
- Rodrigo Ferreira
- Renato Ventura

## Descrição

Este é um projeto de pós-graduação em Arquitetura de Software, desenvolvido pelo Grupo 8 da FIAP. O **Microserviço de Ingestão de Sensores** tem como objetivo fornecer uma estrutura moderna e escalável para recebimento e enfileiramento de dados de telemetria de sensores IoT, aplicando os principais conceitos de arquitetura de software, boas práticas de desenvolvimento e padrões de mercado.

O serviço foi desenvolvido com .NET 8, seguindo padrões **DDD + Clean Architecture**, sendo responsável por:

- Receber dados brutos de telemetria via API REST
- Validar JWT e API Key
- Publicar dados na fila do RabbitMQ para processamento assíncrono

**Não processa dados - apenas recebe e enfileira.**

## Arquitetura

```
┌─────────────────────────────────────────────────────────────────┐
│                         API Layer                                │
│  ┌─────────────┐  ┌──────────────┐  ┌─────────────────────────┐ │
│  │ Controllers │  │  Middlewares │  │  Filters (JWT/API Key) │ │
│  └─────────────┘  └──────────────┘  └─────────────────────────┘ │
└───────────────────────────┬─────────────────────────────────────┘
                            │
┌───────────────────────────▼─────────────────────────────────────┐
│                      Application Layer                           │
│  ┌──────────────┐  ┌─────────────┐  ┌─────────────────────────┐ │
│  │   Commands   │  │  Handlers   │  │      Validators         │ │
│  └──────────────┘  └─────────────┘  └─────────────────────────┘ │
└───────────────────────────┬─────────────────────────────────────┘
                            │
┌───────────────────────────▼─────────────────────────────────────┐
│                       Domain Layer                               │
│  ┌──────────────┐  ┌─────────────┐  ┌─────────────────────────┐ │
│  │   Entities   │  │   Events    │  │      Interfaces         │ │
│  └──────────────┘  └─────────────┘  └─────────────────────────┘ │
└───────────────────────────┬─────────────────────────────────────┘
                            │
┌───────────────────────────▼─────────────────────────────────────┐
│                    Infrastructure Layer                          │
│  ┌──────────────────────┐  ┌──────────────────────────────────┐ │
│  │  RabbitMQ/MassTransit│  │         Repositories             │ │
│  └──────────────────────┘  └──────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

## Estrutura do Projeto

```
sensor-data-ingestion/
├── src/
│   ├── SensorDataIngestion.Domain/           # Entidades e interfaces de domínio
│   ├── SensorDataIngestion.Application/      # Casos de uso, commands, handlers
│   ├── SensorDataIngestion.Infrastructure/   # Implementações (RabbitMQ, repositórios)
│   └── SensorDataIngestion.API/              # Controllers, middlewares, configuração
├── monitoring/
│   ├── prometheus/                           # Configuração do Prometheus
│   └── grafana/                              # Dashboards e configuração do Grafana
├── docker/
│   └── rabbitmq/                             # Configuração do RabbitMQ
├── docker-compose.yml
├── Dockerfile
└── README.md
```

## Tecnologias

- **.NET 8** - Framework principal
- **MassTransit + RabbitMQ** - Mensageria
- **MediatR** - Mediator pattern para CQRS
- **FluentValidation** - Validação de dados
- **Prometheus** - Métricas
- **Grafana** - Dashboards
- **Serilog + Seq** - Logging estruturado
- **JWT Bearer** - Autenticação
- **Swagger/OpenAPI** - Documentação da API

## Requisitos

- Docker & Docker Compose
- .NET 8 SDK (para desenvolvimento local)

## Como Executar

### Com Docker Compose (Recomendado)

```bash
# Subir toda a infraestrutura
docker-compose up -d

# Ver logs
docker-compose logs -f sensor-api

# Derrubar tudo
docker-compose down
```

### Desenvolvimento Local

```bash
# Subir dependências
docker-compose up -d rabbitmq prometheus grafana seq

# Executar a API
cd src/SensorDataIngestion.API
dotnet run
```

## Endpoints

### POST /api/telemetria

Recebe dados de telemetria do sensor.

**Headers obrigatórios:**

- `Authorization: Bearer {jwt_token}`
- `X-Api-Key: {api_key_do_sensor}`

**Request Body:**

```json
{
  "sensorId": "SENSOR-001",
  "temperatura": 25.5,
  "umidade": 60.0,
  "dataLeitura": "2025-01-11T10:00:00Z"
}
```

**Response (202 Accepted):**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "sensorId": "SENSOR-001",
  "mensagem": "Telemetria recebida e enfileirada com sucesso",
  "dataRecebimento": "2025-01-11T10:00:00Z",
  "sucesso": true
}
```

### GET /api/telemetria/health

Health check do endpoint.

## Sensores de Teste

Para desenvolvimento, os seguintes sensores estão pré-cadastrados:

| SensorId   | API Key            | Nome                        |
| ---------- | ------------------ | --------------------------- |
| SENSOR-001 | api-key-sensor-001 | Sensor Temperatura Sala 1   |
| SENSOR-002 | api-key-sensor-002 | Sensor Temperatura Sala 2   |
| SENSOR-003 | api-key-sensor-003 | Sensor Umidade Laboratório |

## URLs de Acesso

| Serviço   | URL                    | Descrição                 |
| ---------- | ---------------------- | --------------------------- |
| API        | http://localhost:8080  | Swagger UI                  |
| RabbitMQ   | http://localhost:15672 | Management UI (guest/guest) |
| Prometheus | http://localhost:9090  | Métricas                   |
| Grafana    | http://localhost:3000  | Dashboards (admin/admin)    |
| Seq        | http://localhost:5341  | Logs centralizados          |

## Métricas Disponíveis

- `sensor_telemetria_recebida_total` - Total de telemetrias recebidas
- `sensor_telemetria_processing_duration_seconds` - Latência de processamento
- `sensor_temperatura_celsius` - Temperatura atual por sensor
- `sensor_umidade_percentual` - Umidade atual por sensor
- `sensor_telemetria_erros_total` - Total de erros por tipo

## Configuração

### Variáveis de Ambiente

| Variável          | Descrição          | Padrão                |
| ------------------ | -------------------- | ---------------------- |
| RabbitMq__Host     | Host do RabbitMQ     | localhost              |
| RabbitMq__Port     | Porta do RabbitMQ    | 5672                   |
| RabbitMq__Username | Usuário do RabbitMQ | guest                  |
| RabbitMq__Password | Senha do RabbitMQ    | guest                  |
| Jwt__Secret        | Chave secreta do JWT | -                      |
| Jwt__Issuer        | Emissor do JWT       | SensorDataIngestion    |
| Jwt__Audience      | Audiência do JWT    | SensorDataIngestionAPI |

## Gerando Token JWT (Para Testes)

Use qualquer ferramenta de geração de JWT com as configurações:

- **Secret**: A chave configurada em `Jwt__Secret`
- **Issuer**: `SensorDataIngestion`
- **Audience**: `SensorDataIngestionAPI`

## Licença

MIT License - FIAP Tech Challenge 2025
