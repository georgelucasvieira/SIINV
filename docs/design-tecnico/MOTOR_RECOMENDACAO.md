# Motor de Recomendação - Perfil de Risco

> Algoritmo de classificação de perfil de investidor baseado em comportamento

---

## Objetivo

Calcular automaticamente o perfil de risco do cliente baseado em:
- Volume de investimentos
- Frequência de movimentações
- Histórico de simulações
- Preferências de liquidez vs rentabilidade

---

## Sistema de Pontuação (0-100)

### Faixas de Perfil

| Pontuação | Perfil | Característica |
|-----------|--------|----------------|
| 0-35 | **CONSERVADOR** | Baixo risco, alta liquidez, rentabilidade modesta |
| 36-65 | **MODERADO** | Risco equilibrado, mix de liquidez e rentabilidade |
| 66-100 | **AGRESSIVO** | Alto risco, foco em rentabilidade máxima |

---

## Fatores de Cálculo

### 1. Volume de Investimentos (0-25 pontos)

**Lógica**: Investidores com maior patrimônio tendem a aceitar mais risco.

```
Pontuação = min(25, (VolumeTotal / 100.000) × 10)

Onde:
- VolumeTotal = soma de todos os investimentos ativos
- Máximo: 25 pontos (atingido em R$ 250.000+)
```

**Exemplos**:
- R$ 5.000 → 0,5 pontos
- R$ 50.000 → 5 pontos
- R$ 100.000 → 10 pontos
- R$ 250.000+ → 25 pontos

---

### 2. Frequência de Movimentação (0-20 pontos)

**Lógica**: Investidores ativos tendem a ser mais agressivos.

```
Frequência = QuantidadeSimulacoes / MesesDesdeRegistro

Pontuação = min(20, Frequência × 2)

Onde:
- MesesDesdeRegistro = diferença entre hoje e primeiro investimento
```

**Exemplos**:
- 2 simulações/mês → 4 pontos
- 5 simulações/mês → 10 pontos
- 10+ simulações/mês → 20 pontos

---

### 3. Preferência de Rentabilidade vs Liquidez (0-30 pontos)

**Lógica**: Analisar produtos simulados/investidos.

```
Para cada simulação:
- Produto de alta rentabilidade/baixa liquidez → +2 pontos
- Produto de média rentabilidade/média liquidez → +1 ponto
- Produto de baixa rentabilidade/alta liquidez → 0 pontos

Pontuação = min(30, SomaPontos / QuantidadeSimulacoes × 10)
```

**Classificação de Produtos**:

| Produto | Rentabilidade | Liquidez | Pontos |
|---------|--------------|----------|--------|
| Poupança | Baixa | Alta | 0 |
| LCI/LCA curto prazo | Baixa | Média | 0 |
| CDB curto prazo (100% CDI) | Média | Alta | 1 |
| Tesouro Selic | Média | Alta | 1 |
| CDB longo prazo (120% CDI) | Alta | Baixa | 2 |
| Tesouro Prefixado | Alta | Baixa | 2 |
| Tesouro IPCA+ | Alta | Baixa | 2 |
| Fundo Renda Fixa | Média | Média | 1 |
| Fundo Multimercado | Alta | Média | 2 |
| Fundo Ações | Muito Alta | Baixa | 2 |

---

### 4. Prazo Médio de Investimento (0-15 pontos)

**Lógica**: Prazos maiores indicam maior tolerância a risco.

```
PrazoMédio = média de prazos das simulações

Pontuação = min(15, PrazoMédio / 4)

Onde:
- PrazoMédio em meses
```

**Exemplos**:
- Prazo médio 6 meses → 1,5 pontos
- Prazo médio 12 meses → 3 pontos
- Prazo médio 24 meses → 6 pontos
- Prazo médio 60+ meses → 15 pontos

---

### 5. Diversificação (0-10 pontos)

**Lógica**: Investidores experientes diversificam.

```
TiposUnicos = quantidade de tipos diferentes de produtos simulados

Pontuação = min(10, TiposUnicos × 2)
```

**Exemplos**:
- 1 tipo → 2 pontos
- 3 tipos → 6 pontos
- 5+ tipos → 10 pontos

---

## Algoritmo Completo

### Fórmula Final

```
Pontuação Total = Volume (0-25)
                + Frequência (0-20)
                + Preferência (0-30)
                + Prazo (0-15)
                + Diversificação (0-10)

Máximo: 100 pontos
```

### Classificação

```
Perfil = case Pontuação Total
    when 0-35   => CONSERVADOR
    when 36-65  => MODERADO
    when 66-100 => AGRESSIVO
```

---

## Fluxo de Cálculo

### Etapas de Processamento

1. **Buscar dados do cliente**:
   - Todas as simulações realizadas
   - Histórico de investimentos ativos
   - Data do primeiro investimento

2. **Calcular volume total**:
   - Somar valores de todos os investimentos com status ATIVO

3. **Calcular cada fator de pontuação**:
   - Volume (0-25 pontos)
   - Frequência (0-20 pontos)
   - Preferência de produtos (0-30 pontos)
   - Prazo médio (0-15 pontos)
   - Diversificação (0-10 pontos)

4. **Somar pontuação total** (máximo 100)

5. **Classificar perfil**:
   - 0-35 → CONSERVADOR
   - 36-65 → MODERADO
   - 66-100 → AGRESSIVO

6. **Calcular preferências de liquidez e rentabilidade** (escala 0-10)

7. **Gerar resultado detalhado** com todas as pontuações individuais

---

## Recomendação de Produtos

### Produtos por Perfil

| Perfil | Produtos Recomendados | Evitar |
|--------|----------------------|--------|
| **CONSERVADOR** | Poupança, Tesouro Selic, CDB curto prazo, LCI/LCA | Ações, Multimercado, longo prazo |
| **MODERADO** | CDB médio prazo, Tesouro Prefixado, Fundos Renda Fixa, IPCA+ | Fundos Ações, produtos sem liquidez |
| **AGRESSIVO** | Todos, priorizando alta rentabilidade | - |

### Critérios de Filtragem

**Conservador**:
- `PerfilRiscoMinimo = "CONSERVADOR"`
- `NivelRisco = "BAIXO"`
- `LiquidezDias <= 90`

**Moderado**:
- `PerfilRiscoMinimo IN ("CONSERVADOR", "MODERADO")`
- `NivelRisco IN ("BAIXO", "MEDIO")`

**Agressivo**:
- Todos os produtos disponíveis
- Ordenar por rentabilidade decrescente

---

## Fluxo de Recálculo

### Quando Recalcular?

1. **Automático** (cron job):
   - A cada 3 meses para todos os clientes
   - Ou quando `DataProximaRevisao` venceu

2. **Manual** (on-demand):
   - Quando cliente faz nova simulação
   - Quando cliente faz novo investimento
   - Quando cliente solicita explicitamente

### Processo de Recálculo Periódico

1. Worker busca perfis com `DataProximaRevisao` vencida
2. Para cada perfil expirado:
   - Recalcular pontuação e perfil
   - Verificar se houve mudança de perfil (ex: CONSERVADOR → MODERADO)
   - Se mudou, publicar evento `PerfilRiscoAlteradoEvent`
   - Salvar novo perfil no banco
   - Definir `DataProximaRevisao = hoje + 3 meses`
3. Worker aguarda 1 hora antes de verificar novamente

---

## Response DTO

### Exemplo de Resposta

```json
{
  "clienteId": 123,
  "perfilAtual": "MODERADO",
  "pontuacao": 52,
  "descricao": "Perfil equilibrado entre segurança e rentabilidade.",
  "volumeInvestimentos": 50000.00,
  "quantidadeSimulacoes": 15,
  "frequenciaMovimentacao": 2.5,
  "preferenciaLiquidez": 6,
  "preferenciaRentabilidade": 4,
  "dataCalculo": "2025-11-16T10:00:00Z",
  "dataProximaRevisao": "2026-02-16T10:00:00Z",
  "produtosRecomendados": [
    {
      "id": 5,
      "nome": "CDB 110% CDI 12 meses",
      "tipo": "CDB",
      "rentabilidade": 0.15015,
      "risco": "MEDIO"
    },
    {
      "id": 8,
      "nome": "Tesouro Prefixado 2027",
      "tipo": "TESOURO_PREFIXADO",
      "rentabilidade": 0.12,
      "risco": "MEDIO"
    }
  ],
  "detalhamento": {
    "pontuacaoVolume": 5.0,
    "pontuacaoFrequencia": 10.0,
    "pontuacaoPreferencia": 20.0,
    "pontuacaoPrazo": 7.5,
    "pontuacaoDiversificacao": 6.0,
    "total": 48.5
  }
}
```

### Campos do Response

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `clienteId` | int | ID do cliente |
| `perfilAtual` | string | CONSERVADOR, MODERADO ou AGRESSIVO |
| `pontuacao` | int | Pontuação total (0-100) |
| `descricao` | string | Descrição textual do perfil |
| `volumeInvestimentos` | decimal | Soma de investimentos ativos |
| `quantidadeSimulacoes` | int | Total de simulações realizadas |
| `frequenciaMovimentacao` | decimal | Simulações por mês |
| `preferenciaLiquidez` | int | Escala 0-10 (quanto mais alto, mais conservador) |
| `preferenciaRentabilidade` | int | Escala 0-10 (quanto mais alto, mais agressivo) |
| `dataCalculo` | datetime | Quando o perfil foi calculado |
| `dataProximaRevisao` | datetime | Quando recalcular novamente |
| `produtosRecomendados` | array | Lista de produtos adequados ao perfil |
| `detalhamento` | object | Breakdown das pontuações por fator |

---

## Checklist

- Sistema de pontuação 0-100 definido
- 5 fatores de cálculo implementados
- Classificação em 3 perfis (Conservador, Moderado, Agressivo)
- Recomendação de produtos por perfil
- Recálculo periódico (3 meses)
- Versionamento de algoritmo (v1.0)
- Eventos de mudança de perfil
- Auditoria de detalhamento (pontuações individuais)
- Response DTO completo

---

**Versão**: 1.0
**Última atualização**: 2025-11-18
**Status**: Pronto para Revisão
