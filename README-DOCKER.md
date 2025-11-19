# Docker Compose - Infraestrutura de Desenvolvimento

Este arquivo contém instruções para subir a infraestrutura necessária para o desenvolvimento do projeto.

## Serviços Disponíveis

- **SQL Server 2022** (porta 1433)
- **RabbitMQ** com Management UI (portas 5672 e 15672)

## Como Usar

### Subir todos os serviços

```bash
docker-compose up -d
```

### Subir apenas o SQL Server

```bash
docker-compose up -d sqlserver
```

### Subir apenas o RabbitMQ

```bash
docker-compose up -d rabbitmq
```

### Verificar status dos serviços

```bash
docker-compose ps
```

### Ver logs dos serviços

```bash
# Todos os serviços
docker-compose logs -f

# Apenas SQL Server
docker-compose logs -f sqlserver

# Apenas RabbitMQ
docker-compose logs -f rabbitmq
```

### Parar os serviços

```bash
docker-compose stop
```

### Parar e remover os serviços (mantém os volumes de dados)

```bash
docker-compose down
```

### Parar e remover tudo, incluindo volumes de dados

```bash
docker-compose down -v
```

## Credenciais

### SQL Server

- **Servidor**: localhost,1433
- **Usuário**: sa
- **Senha**: YourStrong@Passw0rd
- **Database**: DB_Investimentos

**Connection String**:
```
Server=localhost,1433;Database=DB_Investimentos;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;
```

### RabbitMQ

- **Host**: localhost
- **Porta AMQP**: 5672
- **Porta Management UI**: 15672
- **Usuário**: admin
- **Senha**: admin123

**Management UI**: http://localhost:15672

**Connection String**:
```
amqp://admin:admin123@localhost:5672
```

## Healthchecks

Os serviços possuem healthchecks configurados:

- **SQL Server**: Verifica a cada 10s se o servidor está respondendo
- **RabbitMQ**: Verifica a cada 10s se o serviço está ativo

Use `docker-compose ps` para ver o status de saúde dos containers.

## Volumes

Os dados são persistidos em volumes Docker:

- `sqlserver_data`: Dados do SQL Server
- `rabbitmq_data`: Dados do RabbitMQ

Esses volumes garantem que seus dados não sejam perdidos quando os containers forem parados.

## Troubleshooting

### SQL Server não está aceitando conexões

1. Verifique se o container está healthy: `docker-compose ps`
2. Aguarde alguns segundos após o `docker-compose up` - o SQL Server leva um tempo para inicializar
3. Verifique os logs: `docker-compose logs sqlserver`

### RabbitMQ Management UI não abre

1. Verifique se o container está rodando: `docker-compose ps`
2. Tente acessar após alguns segundos
3. Verifique os logs: `docker-compose logs rabbitmq`

### Porta já está em uso

Se as portas 1433, 5672 ou 15672 já estiverem em uso, você pode alterá-las no `docker-compose.yml`:

```yaml
ports:
  - "PORTA_HOST:PORTA_CONTAINER"
```

Por exemplo, para mudar a porta do SQL Server de 1433 para 1434:

```yaml
ports:
  - "1434:1433"
```

Não esqueça de atualizar a connection string na aplicação!
