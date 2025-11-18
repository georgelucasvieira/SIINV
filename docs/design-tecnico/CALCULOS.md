# Cálculos de Rentabilidade de Investimentos

> Fórmulas matemáticas, IR regressivo, taxas e exemplos práticos

---

## Fundamentos

### Conceitos Básicos

| Termo | Definição | Exemplo |
|-------|-----------|---------|
| **Principal (P)** | Valor investido inicial | R$ 10.000,00 |
| **Taxa (i)** | Taxa de juros (anual ou mensal) | 12% a.a. = 0.12 |
| **Prazo (n)** | Período em meses ou anos | 12 meses |
| **Montante (M)** | Valor final (principal + juros) | R$ 11.200,00 |
| **Rentabilidade** | Percentual de ganho | (M - P) / P = 12% |

### Tipos de Capitalização

- **Simples**: Juros calculados apenas sobre o principal
- **Composta**: Juros calculados sobre montante acumulado (usado na maioria dos investimentos)

---

## Fórmulas por Tipo de Produto

### 1. CDB (Certificado de Depósito Bancário)

**Indexador**: CDI (Certificado de Depósito Interbancário)

**Fórmula**:
```
Taxa Efetiva = (% do CDI) × (Taxa CDI)
Montante = P × (1 + Taxa_Mensal)^n

Onde:
- Taxa_Mensal = ((1 + Taxa_Anual)^(1/12)) - 1
```

**Exemplo**: CDB 100% do CDI
```
Dados:
- Principal: R$ 10.000,00
- CDI: 13,65% a.a. (taxa Selic atual aproximada)
- Percentual: 100% do CDI
- Prazo: 12 meses

Cálculo:
Taxa Anual = 0,1365
Taxa Mensal = (1,1365)^(1/12) - 1 = 0,010711 (1,0711% a.m.)

Montante Bruto = 10.000 × (1,010711)^12
               = 10.000 × 1,1365
               = R$ 11.365,00

Rentabilidade Bruta = 13,65%
```

**Exemplo**: CDB 110% do CDI
```
Taxa Efetiva = 1,10 × 13,65% = 15,015% a.a.
Taxa Mensal = (1,15015)^(1/12) - 1 = 0,011776

Montante Bruto = 10.000 × (1,011776)^12
               = R$ 11.501,50

Rentabilidade Bruta = 15,015%
```

---

### 2. Tesouro Direto - Selic (LFT)

**Indexador**: Taxa Selic

**Característica**: Pós-fixado, rentabilidade diária

**Fórmula**:
```
Montante = P × (1 + Taxa_Selic_Diária)^(dias)

Onde:
- Taxa_Selic_Diária = ((1 + Taxa_Selic_Anual)^(1/252)) - 1
- 252 = dias úteis no ano
```

**Exemplo**: Tesouro Selic
```
Dados:
- Principal: R$ 10.000,00
- Selic: 13,75% a.a.
- Prazo: 365 dias

Cálculo:
Taxa Diária = (1,1375)^(1/252) - 1 = 0,000517

Montante Bruto = 10.000 × (1,000517)^252
               = 10.000 × 1,1375
               = R$ 11.375,00
```

---

### 3. Tesouro Prefixado (LTN)

**Indexador**: Nenhum (taxa fixa)

**Fórmula**:
```
Montante = P × (1 + Taxa)^(n/12)

Onde:
- n = prazo em meses
- Taxa = taxa anual fixa
```

**Exemplo**: Tesouro Prefixado 2027 (12% a.a.)
```
Dados:
- Principal: R$ 10.000,00
- Taxa: 12% a.a.
- Prazo: 24 meses

Cálculo:
Montante Bruto = 10.000 × (1,12)^(24/12)
               = 10.000 × (1,12)^2
               = 10.000 × 1,2544
               = R$ 12.544,00
```

---

### 4. Tesouro IPCA+ (NTN-B Principal)

**Indexador**: IPCA + taxa fixa

**Fórmula**:
```
Montante = P × (1 + IPCA)^(n/12) × (1 + Taxa_Fixa)^(n/12)
```

**Exemplo**: Tesouro IPCA+ 2035 (IPCA + 6% a.a.)
```
Dados:
- Principal: R$ 10.000,00
- IPCA projetado: 4,5% a.a.
- Taxa fixa: 6% a.a.
- Prazo: 12 meses

Cálculo:
Fator IPCA = (1,045)^(12/12) = 1,045
Fator Taxa = (1,06)^(12/12) = 1,06

Montante Bruto = 10.000 × 1,045 × 1,06
               = 10.000 × 1,1077
               = R$ 11.077,00

Rentabilidade Nominal = 10,77% (IPCA + 6%)
```

---

### 5. LCI / LCA (Letras de Crédito)

**Indexador**: Geralmente CDI

**Característica**: **Isentos de Imposto de Renda**

**Fórmula**: Igual ao CDB, mas sem desconto de IR

**Valor líquido = Valor bruto** (sem IR)

---

### 6. Fundos de Investimento

**Indexador**: Variável (depende da estratégia do fundo)

**Taxas**:
- **Taxa de Administração**: Anual sobre patrimônio
- **Taxa de Performance**: % sobre o lucro que excede benchmark

**Fórmula**:
```
Rentabilidade Bruta = Rentabilidade_Histórica (média ou projetada)
Taxa Admin Mensal = Taxa_Admin_Anual / 12

Montante com Admin = P × (1 + Rent_Mensal - Taxa_Admin_Mensal)^n

Se houver performance:
    Lucro = Montante - P
    Taxa Performance = 20% × max(0, Lucro - Benchmark)
    Montante Final = Montante - Taxa Performance
```

**Exemplo**: Fundo Multimercado
```
Dados:
- Principal: R$ 10.000,00
- Rentabilidade projetada: 15% a.a.
- Taxa Administração: 2% a.a.
- Taxa Performance: 20% sobre excesso CDI
- Prazo: 12 meses
- CDI: 13% a.a.

Cálculo:
Rent. Mensal = (1,15)^(1/12) - 1 = 0,011715
Taxa Admin Mensal = 0,02 / 12 = 0,001667

Montante = 10.000 × (1 + 0,011715 - 0,001667)^12
         = 10.000 × (1,010048)^12
         = R$ 11.270,00

Lucro = 11.270 - 10.000 = R$ 1.270,00
Benchmark (CDI) = 10.000 × 1,13 - 10.000 = R$ 1.300,00

Excesso = max(0, 1.270 - 1.300) = 0 (fundo não bateu CDI)
Taxa Performance = 0

Montante Final = R$ 11.270,00
```

---

## Imposto de Renda (IR Regressivo)

### Tabela de Alíquotas

| Prazo | Alíquota IR |
|-------|-------------|
| Até 180 dias (6 meses) | 22,5% |
| 181 a 360 dias (6-12 meses) | 20% |
| 361 a 720 dias (12-24 meses) | 17,5% |
| Acima de 720 dias (> 24 meses) | 15% |

### Produtos com IR

- **Tem IR**: CDB, Tesouro Direto (todos), Fundos de Investimento
- **Isento**: LCI / LCA

### Fórmula de IR

```
Lucro Bruto = Montante Bruto - Principal
IR = Lucro Bruto × Alíquota
Montante Líquido = Montante Bruto - IR
```

---

## Exemplos Completos

### Exemplo 1: CDB 110% CDI - 12 meses

```
Input:
- Valor: R$ 10.000,00
- Prazo: 12 meses
- Produto: CDB 110% CDI
- CDI: 13,65% a.a.

Passo 1: Calcular montante bruto
Taxa Efetiva = 1,10 × 13,65% = 15,015%
Taxa Mensal = (1,15015)^(1/12) - 1 = 0,011776
Montante Bruto = 10.000 × (1,011776)^12 = R$ 11.501,50

Passo 2: Calcular IR (prazo = 12 meses)
Alíquota = 20%
Lucro Bruto = 11.501,50 - 10.000 = R$ 1.501,50
IR = 1.501,50 × 0,20 = R$ 300,30

Passo 3: Montante líquido
Montante Líquido = 11.501,50 - 300,30 = R$ 11.201,20

Output:
{
  "valorInvestido": 10000.00,
  "prazoMeses": 12,
  "montanteBruto": 11501.50,
  "montanteLiquido": 11201.20,
  "valorIR": 300.30,
  "aliquotaIR": 0.20,
  "rentabilidadeBruta": 0.15015,
  "rentabilidadeLiquida": 0.12012
}
```

### Exemplo 2: LCI 90% CDI - 24 meses (Isento IR)

```
Input:
- Valor: R$ 10.000,00
- Prazo: 24 meses
- Produto: LCI 90% CDI
- CDI: 13,65% a.a.

Passo 1: Calcular montante bruto
Taxa Efetiva = 0,90 × 13,65% = 12,285%
Taxa Mensal = (1,12285)^(1/12) - 1 = 0,009698
Montante Bruto = 10.000 × (1,009698)^24 = R$ 12.638,00

Passo 2: IR = 0 (LCI é isenta)

Passo 3: Montante líquido
Montante Líquido = R$ 12.638,00 (= bruto)

Output:
{
  "valorInvestido": 10000.00,
  "prazoMeses": 24,
  "montanteBruto": 12638.00,
  "montanteLiquido": 12638.00,
  "valorIR": 0,
  "aliquotaIR": 0,
  "rentabilidadeBruta": 0.2638,
  "rentabilidadeLiquida": 0.2638
}
```

---

## Validações de Entrada

### Regras de Validação

- `ValorInvestido`:
  - Maior que 0
  - Menor ou igual a R$ 1.000.000
  - Respeitar `ValorMinimoInvestimento` do produto
  - Respeitar `ValorMaximoInvestimento` do produto (se houver)

- `PrazoMeses`:
  - Entre 1 e 360 meses
  - Respeitar `PrazoMinimoMeses` do produto
  - Respeitar `PrazoMaximoMeses` do produto

- `TipoProduto`:
  - Deve ser um valor válido do enum (CDB, LCI, LCA, TESOURO_SELIC, etc.)

---

## Checklist de Implementação

- Fórmulas de juros compostos implementadas
- Cálculo de CDB (% CDI)
- Cálculo de Tesouro Selic (pós-fixado)
- Cálculo de Tesouro Prefixado (taxa fixa)
- Cálculo de Tesouro IPCA (híbrido)
- Cálculo de LCI/LCA (isento IR)
- Cálculo de Fundos (com taxa admin + performance)
- IR regressivo (tabela completa)
- Validações de limites por produto
- Factory pattern para extensibilidade
- Testes unitários com casos realistas
- Precisão decimal (sem float)
- Arredondamento correto (2 casas)

---

**Versão**: 1.0
**Última atualização**: 2025-11-18
**Status**: Pronto para Revisão
