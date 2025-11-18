# Contratos de API (DTOs - Data Transfer Objects)

> Requests, Responses e validações de todos os endpoints

---

## Índice

- [Autenticação](#autenticação)
- [Simulações](#simulações)
- [Produtos](#produtos)
- [Perfil de Risco](#perfil-de-risco)
- [Telemetria](#telemetria)
- [Validações](#validações)

---

## Autenticação

### POST /api/v1/auth/register

**Request**:
```json
{
  "email": "cliente@email.com",
  "password": "SenhaForte@123",
  "confirmPassword": "SenhaForte@123",
  "nome": "Maria Silva",
  "cpf": "123.456.789-00"
}
```

**Response (201 Created)**:
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "email": "cliente@email.com",
  "nome": "Maria Silva",
  "clienteId": 12345,
  "dataCriacao": "2025-11-16T10:00:00Z"
}
```

**Validation**:
```csharp
public class RegisterCustomerValidator : AbstractValidator<RegisterCustomerRequest>
{
    public RegisterCustomerValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("Senha deve conter: maiúscula, minúscula, número e caractere especial");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("Senhas não conferem");

        RuleFor(x => x.CPF)
            .NotEmpty()
            .Must(ValidarCPF)
            .WithMessage("CPF inválido");

        RuleFor(x => x.Nome)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(200);
    }
}
```

---

### POST /api/v1/auth/login

**Request**:
```json
{
  "email": "cliente@email.com",
  "password": "SenhaForte@123"
}
```

**Response (200 OK)**:
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "abc123def456ghi789...",
  "expiresIn": 900,
  "tokenType": "Bearer",
  "user": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "email": "cliente@email.com",
    "nome": "Maria Silva",
    "clienteId": 12345
  }
}
```

---

### POST /api/v1/auth/refresh

**Request**:
```json
{
  "refreshToken": "abc123def456ghi789..."
}
```

**Response (200 OK)**:
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "xyz789uvw456rst123...",
  "expiresIn": 900,
  "tokenType": "Bearer"
}
```

---

## Simulações

### POST /api/v1/simulacoes

**Request**:
```json
{
  "clienteId": 123,
  "valor": 10000.00,
  "prazoMeses": 12,
  "tipoProduto": "CDB"
}
```

**Response (201 Created)**:
```json
{
  "produtoValidado": true,
  "id": 101,
  "nome": "CDB Caixa 110% CDI 12 meses",
  "tipo": "CDB",
  "rentabilidade": 0.15015,
  "risco": "MEDIO",
  "resultadoSimulacao": {
    "valorInvestido": 10000.00,
    "prazoMeses": 12,
    "valorBruto": 11501.50,
    "valorImpostos": 300.30,
    "valorTaxas": 0,
    "valorFinal": 11201.20,
    "rentabilidadeNominal": 0.15015,
    "rentabilidadeEfetiva": 0.12012,
    "aliquotaIR": 0.20,
    "dataSimulacao": "2025-11-16T14:30:00Z"
  },
  "detalhamento": {
    "indexador": "CDI",
    "percentualIndexador": 110.00,
    "taxaCDIAtual": 0.1365,
    "liquidezDias": 0,
    "temIR": true,
    "valorMinimoInvestimento": 1000.00
  }
}
```

**Validation**:
```csharp
public class SimularInvestimentoValidator : AbstractValidator<SimularInvestimentoCommand>
{
    public SimularInvestimentoValidator()
    {
        RuleFor(x => x.ClienteId)
            .GreaterThan(0);

        RuleFor(x => x.Valor)
            .GreaterThan(0).WithMessage("Valor deve ser positivo")
            .LessThanOrEqualTo(1_000_000).WithMessage("Valor máximo: R$ 1.000.000");

        RuleFor(x => x.PrazoMeses)
            .InclusiveBetween(1, 360).WithMessage("Prazo: 1-360 meses");

        RuleFor(x => x.TipoProduto)
            .NotEmpty()
            .Must(BeValidTipoProduto)
            .WithMessage("Tipo de produto inválido");
    }

    private bool BeValidTipoProduto(string tipo)
    {
        return Enum.TryParse<TipoProduto>(tipo, ignoreCase: true, out _);
    }
}
```

---

### GET /api/v1/simulacoes

**Query Parameters**:
```
?pageSize=20&pageNumber=1&ordenarPor=dataSimulacao&ordem=desc
```

**Response (200 OK)**:
```json
{
  "data": [
    {
      "id": 150,
      "clienteId": 123,
      "produto": {
        "id": 5,
        "nome": "CDB 110% CDI",
        "tipo": "CDB"
      },
      "valorInvestido": 10000.00,
      "valorFinal": 11201.20,
      "prazoMeses": 12,
      "rentabilidadeEfetiva": 0.12012,
      "dataSimulacao": "2025-11-16T14:30:00Z"
    },
    {
      "id": 149,
      "clienteId": 123,
      "produto": {
        "id": 8,
        "nome": "Tesouro Selic 2027",
        "tipo": "TESOURO_SELIC"
      },
      "valorInvestido": 5000.00,
      "valorFinal": 5687.50,
      "prazoMeses": 12,
      "rentabilidadeEfetiva": 0.1375,
      "dataSimulacao": "2025-11-15T10:00:00Z"
    }
  ],
  "pagination": {
    "pageNumber": 1,
    "pageSize": 20,
    "totalRecords": 45,
    "totalPages": 3,
    "hasPrevious": false,
    "hasNext": true
  }
}
```

---

### GET /api/v1/simulacoes/{id}

**Response (200 OK)**:
```json
{
  "id": 150,
  "clienteId": 123,
  "produto": {
    "id": 5,
    "nome": "CDB Caixa 110% CDI 12 meses",
    "tipo": "CDB",
    "rentabilidade": 0.15015,
    "risco": "MEDIO",
    "liquidezDias": 0,
    "valorMinimoInvestimento": 1000.00
  },
  "valorInvestido": 10000.00,
  "prazoMeses": 12,
  "valorBruto": 11501.50,
  "valorImpostos": 300.30,
  "valorTaxas": 0,
  "valorFinal": 11201.20,
  "rentabilidadeNominal": 0.15015,
  "rentabilidadeEfetiva": 0.12012,
  "aliquotaIR": 0.20,
  "dataSimulacao": "2025-11-16T14:30:00Z",
  "statusProcessamento": "PROCESSADO",
  "dataProcessamento": "2025-11-16T14:30:05Z"
}
```

**Error (403 Forbidden)** - Cliente tentando acessar simulação de outro cliente:
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Acesso negado",
  "status": 403,
  "detail": "Você não tem permissão para acessar esta simulação",
  "traceId": "00-abc123def456-ghi789-00"
}
```

---

### GET /api/v1/simulacoes/por-produto-dia

**Query Parameters**:
```
?dataInicio=2025-11-01&dataFim=2025-11-30
```

**Response (200 OK)**:
```json
[
  {
    "produto": "CDB Caixa 110% CDI",
    "data": "2025-11-16",
    "quantidadeSimulacoes": 15,
    "valorTotalInvestido": 150000.00,
    "mediaValorFinal": 168015.00,
    "mediaRentabilidade": 0.12012
  },
  {
    "produto": "Tesouro Selic 2027",
    "data": "2025-11-16",
    "quantidadeSimulacoes": 8,
    "valorTotalInvestido": 80000.00,
    "mediaValorFinal": 91000.00,
    "mediaRentabilidade": 0.1375
  }
]
```

---

## Produtos

### GET /api/v1/produtos

**Query Parameters**:
```
?tipo=CDB&ativo=true&ordenarPor=rentabilidade&ordem=desc
```

**Response (200 OK)**:
```json
[
  {
    "id": 5,
    "nome": "CDB Caixa 120% CDI 24 meses",
    "tipo": "CDB",
    "taxaRentabilidadeAnual": 0.1638,
    "indexador": "CDI",
    "percentualIndexador": 120.00,
    "nivelRisco": "MEDIO",
    "liquidezDias": 0,
    "valorMinimoInvestimento": 5000.00,
    "valorMaximoInvestimento": null,
    "prazoMinimoMeses": 12,
    "prazoMaximoMeses": 60,
    "temIR": true,
    "perfilRecomendado": ["MODERADO", "AGRESSIVO"],
    "descricao": "CDB com rendimento de 120% do CDI, ideal para objetivos de médio prazo"
  },
  {
    "id": 3,
    "nome": "CDB Caixa 110% CDI 12 meses",
    "tipo": "CDB",
    "taxaRentabilidadeAnual": 0.15015,
    "indexador": "CDI",
    "percentualIndexador": 110.00,
    "nivelRisco": "MEDIO",
    "liquidezDias": 0,
    "valorMinimoInvestimento": 1000.00,
    "valorMaximoInvestimento": null,
    "prazoMinimoMeses": 6,
    "prazoMaximoMeses": 36,
    "temIR": true,
    "perfilRecomendado": ["CONSERVADOR", "MODERADO"],
    "descricao": "CDB com rendimento de 110% do CDI e liquidez diária"
  }
]
```

---

### GET /api/v1/produtos-recomendados/{perfil}

**Path Parameters**: `perfil` = CONSERVADOR | MODERADO | AGRESSIVO

**Response (200 OK)**:
```json
[
  {
    "id": 1,
    "nome": "Tesouro Selic 2027",
    "tipo": "TESOURO_SELIC",
    "rentabilidade": 0.1375,
    "risco": "BAIXO",
    "liquidezDias": 0,
    "valorMinimoInvestimento": 100.00,
    "recomendacao": {
      "motivo": "Baixo risco e alta liquidez, ideal para perfil conservador",
      "destaque": "Garantido pelo Tesouro Nacional"
    }
  },
  {
    "id": 2,
    "nome": "LCI 90% CDI 12 meses",
    "tipo": "LCI",
    "rentabilidade": 0.12285,
    "risco": "BAIXO",
    "liquidezDias": 90,
    "valorMinimoInvestimento": 1000.00,
    "recomendacao": {
      "motivo": "Isento de IR, rentabilidade atrativa",
      "destaque": "Sem imposto de renda"
    }
  }
]
```

---

## Perfil de Risco

### GET /api/v1/perfil-risco

**Response (200 OK)**:
```json
{
  "clienteId": 123,
  "perfilAtual": "MODERADO",
  "pontuacao": 52,
  "descricao": "Perfil equilibrado entre segurança e rentabilidade.",
  "caracteristicas": {
    "toleranciaRisco": "Média",
    "horizonteTempo": "Médio prazo (1-3 anos)",
    "objetivoPrincipal": "Equilibrar proteção do capital com crescimento"
  },
  "volumeInvestimentos": 50000.00,
  "quantidadeSimulacoes": 15,
  "frequenciaMovimentacao": 2.5,
  "preferenciaLiquidez": 6,
  "preferenciaRentabilidade": 4,
  "dataCalculo": "2025-11-16T10:00:00Z",
  "dataProximaRevisao": "2026-02-16T10:00:00Z",
  "detalhamento": {
    "pontuacaoVolume": 5.0,
    "pontuacaoFrequencia": 10.0,
    "pontuacaoPreferencia": 20.0,
    "pontuacaoPrazo": 7.5,
    "pontuacaoDiversificacao": 6.0,
    "total": 48.5
  },
  "evolucao": [
    {
      "data": "2024-08-16",
      "perfil": "CONSERVADOR",
      "pontuacao": 28
    },
    {
      "data": "2025-11-16",
      "perfil": "MODERADO",
      "pontuacao": 52
    }
  ]
}
```

---

## Telemetria

### GET /api/v1/telemetria

**Query Parameters**:
```
?dataInicio=2025-11-01&dataFim=2025-11-30
```

**Response (200 OK)**:
```json
{
  "servicos": [
    {
      "nome": "simular-investimento",
      "quantidadeChamadas": 1250,
      "mediaTempoRespostaMs": 187,
      "p95TempoRespostaMs": 350,
      "p99TempoRespostaMs": 520,
      "taxaSucesso": 0.9872,
      "taxaErro": 0.0128,
      "errosPorStatus": {
        "400": 10,
        "422": 5,
        "500": 1
      }
    },
    {
      "nome": "perfil-risco",
      "quantidadeChamadas": 320,
      "mediaTempoRespostaMs": 142,
      "p95TempoRespostaMs": 280,
      "p99TempoRespostaMs": 410,
      "taxaSucesso": 0.9969,
      "taxaErro": 0.0031
    },
    {
      "nome": "listar-produtos",
      "quantidadeChamadas": 850,
      "mediaTempoRespostaMs": 45,
      "p95TempoRespostaMs": 95,
      "p99TempoRespostaMs": 150,
      "taxaSucesso": 1.0,
      "taxaErro": 0.0
    }
  ],
  "periodo": {
    "inicio": "2025-11-01T00:00:00Z",
    "fim": "2025-11-30T23:59:59Z",
    "totalDias": 30
  },
  "resumoGeral": {
    "totalChamadas": 2420,
    "mediaGeralTempoRespostaMs": 158,
    "taxaGeralSucesso": 0.9917,
    "taxaGeralErro": 0.0083
  }
}
```

---

## Tratamento de Erros

### Formato Padrão (RFC 7807 - Problem Details)

```json
{
  "type": "https://api.exemplo.com/errors/validation",
  "title": "Um ou mais erros de validação ocorreram",
  "status": 400,
  "detail": "Verifique os campos destacados",
  "traceId": "00-abc123def456-ghi789-00",
  "errors": {
    "ValorInvestido": [
      "Valor investido deve ser positivo"
    ],
    "PrazoMeses": [
      "Prazo deve estar entre 1 e 360 meses"
    ]
  }
}
```

### Códigos HTTP Utilizados

| Código | Significado | Exemplo |
|--------|-------------|---------|
| 200 | OK | GET com sucesso |
| 201 | Created | POST simulação criada |
| 400 | Bad Request | Validação falhou |
| 401 | Unauthorized | Token ausente/inválido |
| 403 | Forbidden | Sem permissão |
| 404 | Not Found | Recurso não existe |
| 409 | Conflict | Email já cadastrado |
| 422 | Unprocessable Entity | Regra de negócio violada |
| 429 | Too Many Requests | Rate limit excedido |
| 500 | Internal Server Error | Erro não tratado |

---

## Paginação Padrão

Todos os endpoints de listagem seguem o mesmo padrão:

**Request**:
```
GET /api/v1/simulacoes?pageSize=20&pageNumber=1&ordenarPor=dataSimulacao&ordem=desc
```

**Response**:
```json
{
  "data": [ /* ... */ ],
  "pagination": {
    "pageNumber": 1,
    "pageSize": 20,
    "totalRecords": 150,
    "totalPages": 8,
    "hasPrevious": false,
    "hasNext": true,
    "links": {
      "self": "/api/v1/simulacoes?pageNumber=1&pageSize=20",
      "first": "/api/v1/simulacoes?pageNumber=1&pageSize=20",
      "last": "/api/v1/simulacoes?pageNumber=8&pageSize=20",
      "next": "/api/v1/simulacoes?pageNumber=2&pageSize=20"
    }
  }
}
```

---

## Headers Obrigatórios

### Requisições Autenticadas

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json
Accept: application/json
X-Correlation-Id: 550e8400-e29b-41d4-a716-446655440000 (opcional)
```

### Respostas

```http
Content-Type: application/json
X-Correlation-Id: 550e8400-e29b-41d4-a716-446655440000
X-Request-Id: abc123def456
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1700000000
```

---

## Checklist

- DTOs para todos os endpoints
- Validações com FluentValidation
- Tratamento de erros padronizado (RFC 7807)
- Paginação consistente
- Headers de rastreamento
- Rate limiting headers
- Exemplos JSON completos
- Documentação de códigos HTTP
- Versionamento de API (v1)

---

**Versão**: 1.0
**Última atualização**: 2025-11-16
**Status**: Pronto para Revisão
