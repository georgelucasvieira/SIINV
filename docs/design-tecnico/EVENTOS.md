# Eventos do RabbitMQ

> Schema de eventos, exchanges, queues e processamento assíncrono

---

## Arquitetura de Mensageria

### Componentes

```
API_Investimentos → RabbitMQ → Worker Service
     Publisher    Exchange+Queues    Consumer
```

### Pattern: Publish/Subscribe com Topic Exchange

```
Exchange (Topic)
    ↓
Routing Keys
    ↓
Queues
    ↓
Consumers
```

---

## Exchanges e Queues

### Exchange: `simulacoes.events`

**Tipo**: Topic Exchange
**Durável**: Sim
**Auto-delete**: Não

### Queues

| Queue | Routing Key | Consumer | TTL | DLQ |
|-------|-------------|----------|-----|-----|
| `simulacoes.created` | `simulacao.created` | Worker | 7 dias | Sim |
| `simulacoes.updated` | `simulacao.updated` | Worker | 7 dias | Sim |
| `perfil-risco.recalcular` | `perfil.recalcular` | Worker | 3 dias | Sim |
| `auditoria.events` | `*.auditoria` | Analytics Worker | 30 dias | Sim |

### Dead Letter Queues (DLQ)

```
simulacoes.created.dlq
simulacoes.updated.dlq
perfil-risco.recalcular.dlq
auditoria.events.dlq
```

**Estratégia**: Mensagens com falha após 3 tentativas vão para DLQ.

---

## Configuração do RabbitMQ

### Connection Settings

| Configuração | Valor |
|-------------|-------|
| Host | localhost |
| Port | 5672 |
| Username | admin |
| Password | admin123 |
| VirtualHost | / |
| PrefetchCount | 10 |
| RetryCount | 3 |
| RetryDelay | 5 segundos |

### Exchange Declaration

```
Exchange: simulacoes.events
Type: Topic
Durable: true
Auto-delete: false
```

### Queue Declaration

```
Queue: simulacoes.created
Durable: true
Exclusive: false
Auto-delete: false
Arguments:
  - x-message-ttl: 604800000 (7 dias)
  - x-dead-letter-exchange: simulacoes.events.dlx
  - x-dead-letter-routing-key: simulacao.created.dead

Binding:
  Exchange: simulacoes.events
  Routing Key: simulacao.created
```

### Dead Letter Queue Setup

```
Exchange DLX: simulacoes.events.dlx
Type: Direct
Durable: true

Queue DLQ: simulacoes.created.dlq
Durable: true
Exclusive: false
Auto-delete: false

Binding:
  Exchange: simulacoes.events.dlx
  Routing Key: simulacao.created.dead
```

---

## Eventos

### 1. SimulacaoCriadaEvent

**Routing Key**: `simulacao.created`

**Payload**:
```json
{
  "eventId": "550e8400-e29b-41d4-a716-446655440000",
  "eventType": "SimulacaoCriada",
  "timestamp": "2025-11-16T14:30:00.000Z",
  "version": "1.0",
  "correlationId": "abc123-def456-ghi789",
  "causationId": "xyz789-uvw456-rst123",
  "data": {
    "simulacaoId": 150,
    "clienteId": 123,
    "produtoId": 5,
    "valorInvestido": 10000.00,
    "prazoMeses": 12,
    "valorFinal": 11201.20,
    "valorBruto": 11501.50,
    "valorImpostos": 300.30,
    "valorTaxas": 0,
    "rentabilidadeEfetiva": 0.12012,
    "rentabilidadeNominal": 0.15015,
    "aliquotaIR": 0.20,
    "dataSimulacao": "2025-11-16T14:30:00.000Z",
    "usuarioId": "660e8400-e29b-41d4-a716-446655440001",
    "ipOrigem": "192.168.1.100"
  },
  "metadata": {
    "source": "API_Investimentos",
    "sourceVersion": "1.0.0",
    "environment": "production"
  }
}
```

**Processamento pelo Worker**:

1. Verificar idempotência (evitar duplicatas)
2. Buscar simulação (pode já existir no banco)
3. Atualizar status para PROCESSADO
4. Criar registro de auditoria
5. Salvar no banco de dados
6. ACK da mensagem

---

### 2. PerfilRiscoRecalcularEvent

**Routing Key**: `perfil.recalcular`

**Payload**:
```json
{
  "eventId": "770e8400-e29b-41d4-a716-446655440002",
  "eventType": "PerfilRiscoRecalcular",
  "timestamp": "2025-11-16T15:00:00.000Z",
  "version": "1.0",
  "correlationId": "abc123-def456-ghi789",
  "data": {
    "clienteId": 123,
    "motivo": "NOVA_SIMULACAO",
    "prioridade": "NORMAL"
  },
  "metadata": {
    "source": "API_Investimentos",
    "triggeredBy": "SimulacaoCriada"
  }
}
```

**Processamento pelo Worker**:

1. Calcular novo perfil de risco usando o motor de recomendação
2. Buscar perfil anterior do cliente
3. Verificar se houve mudança de perfil (ex: CONSERVADOR → MODERADO)
4. Se mudou, publicar evento `PerfilRiscoAlteradoEvent`
5. Salvar novo perfil no banco
6. Definir `DataProximaRevisao = hoje + 3 meses`
7. ACK da mensagem

---

### 3. PerfilRiscoAlteradoEvent (Notificação)

**Routing Key**: `perfil.alterado`

**Payload**:
```json
{
  "eventId": "880e8400-e29b-41d4-a716-446655440003",
  "eventType": "PerfilRiscoAlterado",
  "timestamp": "2025-11-16T15:05:00.000Z",
  "version": "1.0",
  "correlationId": "abc123-def456-ghi789",
  "data": {
    "clienteId": 123,
    "perfilAnterior": "CONSERVADOR",
    "perfilNovo": "MODERADO",
    "pontuacaoAnterior": 28,
    "pontuacaoNova": 52,
    "dataMudanca": "2025-11-16T15:05:00.000Z",
    "motivo": "Aumento de volume e frequência de investimentos"
  }
}
```

**Uso**: Poderia disparar notificação por email/push para o cliente.

---

### 4. AuditoriaEvent

**Routing Key**: `simulacao.auditoria`, `perfil.auditoria`, etc.

**Payload**:
```json
{
  "eventId": "990e8400-e29b-41d4-a716-446655440004",
  "eventType": "Auditoria",
  "timestamp": "2025-11-16T14:30:05.000Z",
  "version": "1.0",
  "correlationId": "abc123-def456-ghi789",
  "data": {
    "entidade": "Simulacoes",
    "entidadeId": 150,
    "operacao": "INSERT",
    "usuarioId": "660e8400-e29b-41d4-a716-446655440001",
    "valoresAntigos": null,
    "valoresNovos": {
      "clienteId": 123,
      "produtoId": 5,
      "valorInvestido": 10000.00,
      "prazoMeses": 12
    },
    "ipOrigem": "192.168.1.100",
    "dataOperacao": "2025-11-16T14:30:00.000Z"
  }
}
```

---

## Publisher (API_Investimentos)

### Outbox Pattern

A API não publica diretamente no RabbitMQ. Em vez disso:

1. **Na mesma transação** que cria a simulação, cria uma mensagem na tabela `OutboxMessages`
2. Um **background service** lê mensagens pendentes da tabela `OutboxMessages` a cada 5 segundos
3. Publica cada mensagem no RabbitMQ
4. Marca a mensagem como `Publicado = true` no banco

**Benefício**: Garante consistência - se a transação falhar, a mensagem não é criada. Se a publicação no RabbitMQ falhar, a mensagem fica pendente na outbox e é retentada.

### Fluxo de Publicação com Transação

1. Iniciar transação no banco de dados
2. Criar simulação na tabela `Simulacoes`
3. Criar mensagem na tabela `OutboxMessages` com payload do evento
4. Commit da transação (ACID - tudo ou nada)
5. Retornar resposta síncrona ao cliente
6. Background service processa outbox assincronamente

### Retry na Outbox

- Até 3 tentativas de publicação
- Exponential backoff: 5s, 10s, 20s
- Após 3 falhas, mensagem fica marcada como falha (requer investigação manual)

---

## Consumer (Worker Service)

### Configuração do Consumer

```
Queue: simulacoes.created
Auto-ACK: false (manual ACK para garantir processamento)
Prefetch Count: 10 (processa até 10 mensagens em paralelo)
```

### Fluxo de Consumo

1. Receber mensagem da fila
2. Deserializar payload JSON
3. Processar evento (handler específico)
4. Se sucesso: **ACK** (remove da fila)
5. Se falha: **NACK com requeue** (retenta até 3 vezes)
6. Após 3 falhas: mensagem vai para DLQ automaticamente

### Idempotência

Todos os handlers devem ser idempotentes:

- Verificar se a simulação já foi processada antes de reprocessar
- Usar `SimulacaoId` ou `EventId` como chave de deduplicação
- Evitar duplicatas mesmo que a mensagem seja consumida múltiplas vezes

---

## Retry Strategy

### Configuração de Retries

**Política**: Exponential Backoff

```
Tentativa 1: Imediato
Tentativa 2: Após 2^1 = 2 segundos
Tentativa 3: Após 2^2 = 4 segundos
Tentativa 4: Após 2^3 = 8 segundos
```

**Máximo de tentativas**: 3

**Após 3 falhas**: Mensagem movida para Dead Letter Queue (DLQ)

### Dead Letter Queue (DLQ)

- Worker separado monitora DLQs a cada 5 minutos
- Se DLQ contém mensagens, gera alerta (log, Slack, email)
- Mensagens na DLQ requerem investigação manual
- Após correção do problema, mensagens podem ser republicadas manualmente

---

## Monitoramento

### Métricas a Coletar (OpenTelemetry)

| Métrica | Tipo | Descrição |
|---------|------|-----------|
| `rabbitmq_messages_published_total` | Counter | Total de mensagens publicadas |
| `rabbitmq_messages_consumed_total` | Counter | Total de mensagens consumidas |
| `rabbitmq_messages_failed_total` | Counter | Total de mensagens que falharam |
| `rabbitmq_message_processing_duration_ms` | Histogram | Tempo de processamento por mensagem |
| `rabbitmq_queue_depth` | Gauge | Quantidade de mensagens na fila |
| `rabbitmq_dlq_depth` | Gauge | Quantidade de mensagens na DLQ |

### Alertas Recomendados

- DLQ contém mais de 10 mensagens
- Fila principal com mais de 1000 mensagens (backlog)
- Taxa de falha acima de 5%
- Tempo médio de processamento acima de 5 segundos

---

## Checklist

- Exchange Topic configurado
- Queues com durabilidade
- Dead Letter Queues (DLQ)
- TTL configurado (7 dias)
- Prefetch count otimizado (10)
- Outbox Pattern implementado
- Retry com exponential backoff
- Manual ACK (não auto-ack)
- Idempotência no consumer
- Correlation IDs em todos os eventos
- Logging estruturado
- Métricas OpenTelemetry
- Monitoramento de DLQ
- Versionamento de eventos (v1.0)

---

**Versão**: 1.0
**Última atualização**: 2025-11-18
**Status**: Pronto para Revisão
