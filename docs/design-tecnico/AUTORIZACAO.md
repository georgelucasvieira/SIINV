# Estratégia de Autorização e RBAC

> Sistema de autenticação e autorização com JWT, roles e políticas

---

## Visão Geral

O sistema possui **duas camadas de autorização** para atender diferentes públicos:

1. **Auth Service** - Usuários internos (gestores, backoffice)
2. **API_Investimentos** - Clientes externos (investidores)

---

## Modelo de Usuários

### Duas Tabelas Separadas (DB_Usuarios)

#### InternalUsers
- Funcionários da organização (Admin, Manager, Analyst)
- Possuem roles e permissões
- Acesso ao backoffice

#### Customers
- Clientes investidores
- Sem roles (apenas autenticação)
- Acesso apenas aos próprios dados

---

## Roles (RBAC)

### Definição de Roles

| Role | Descrição | Permissões |
|------|-----------|-----------|
| **Admin** | Administrador do sistema | CRUD completo em todas as entidades |
| **Manager** | Gerente de operações | CRUD produtos, visualizar simulações, relatórios |
| **Analyst** | Analista de dados | Apenas leitura (relatórios, analytics) |

**Nota**: Customers não possuem roles - apenas autenticação simples com acesso aos próprios dados.

---

## JWT Structure

### Token de Usuário Interno (InternalUser)

```json
{
  "sub": "550e8400-e29b-41d4-a716-446655440000",
  "email": "admin@caixa.com.br",
  "name": "João Silva",
  "type": "INTERNAL",
  "roles": ["Admin"],
  "permissions": ["produtos:write", "simulacoes:read", "users:write"],
  "nbf": 1700000000,
  "exp": 1700000900,
  "iat": 1700000000,
  "iss": "AuthService",
  "aud": "InternalBackoffice"
}
```

### Token de Cliente (Customer)

```json
{
  "sub": "660e8400-e29b-41d4-a716-446655440001",
  "email": "cliente@email.com",
  "name": "Maria Souza",
  "type": "CUSTOMER",
  "clienteId": 12345,
  "cpf": "123.456.789-00",
  "nbf": 1700000000,
  "exp": 1700000900,
  "iat": 1700000000,
  "iss": "AuthService",
  "aud": "API_Investimentos"
}
```

### Diferenças Principais

| Campo | InternalUser | Customer |
|-------|-------------|----------|
| `type` | "INTERNAL" | "CUSTOMER" |
| `roles` | Array de roles | Não possui |
| `permissions` | Array de permissões | Não possui |
| `clienteId` | Não possui | ID do cliente |
| `cpf` | Não possui | CPF do cliente |
| `aud` (audience) | "InternalBackoffice" | "API_Investimentos" |

---

## Refresh Tokens

### Características
- **Validade**: 7 dias
- **Armazenamento**: Hasheado (SHA256) no banco de dados
- **Rotação**: Automática a cada uso (security best practice)
- **Revogação**: Manual disponível (logout, suspeita de comprometimento)

### Fluxo de Rotação

```
1. Cliente envia refresh token
2. Sistema valida token (hash, expiração, não revogado)
3. Sistema revoga token antigo
4. Sistema gera novo par (access token + refresh token)
5. Sistema marca token antigo como "SubstituidoPor" novo token ID
6. Sistema retorna novos tokens
```

**Benefício de Segurança**: Se um token vazado for usado, o sistema detecta imediatamente pois o token legítimo já foi rotacionado.

---

## Endpoints e Autorização

### Auth Service (Backoffice)

**Autenticação**:
- `POST /api/auth/internal/login` - Login de usuário interno (público)
- `POST /api/auth/internal/refresh` - Refresh token (público)

**Gestão de Usuários** (apenas InternalUsers):
- `POST /api/backoffice/users` - Criar usuário (Admin)
- `DELETE /api/backoffice/users/{id}` - Deletar usuário (Admin)
- `PUT /api/backoffice/users/{id}` - Editar usuário (Admin, Manager)
- `GET /api/backoffice/users` - Listar usuários (Admin, Manager, Analyst)

**Gestão de Produtos**:
- `POST /api/backoffice/produtos` - Criar produto (Admin, Manager)
- `PUT /api/backoffice/produtos/{id}` - Editar produto (Admin, Manager)
- `DELETE /api/backoffice/produtos/{id}` - Deletar produto (Admin)

**Analytics**:
- `GET /api/backoffice/simulacoes` - Ver todas as simulações (Admin, Manager, Analyst)
- `GET /api/backoffice/relatorios/*` - Relatórios (Admin, Manager, Analyst)

### API_Investimentos (Clientes)

**Autenticação** (proxy para Auth Service):
- `POST /api/v1/auth/login` - Login de cliente
- `POST /api/v1/auth/refresh` - Refresh token
- `POST /api/v1/auth/register` - Registrar novo cliente (público)

**Produtos** (público ou autenticado):
- `GET /api/v1/produtos` - Listar produtos ativos (público)
- `GET /api/v1/produtos-recomendados/{perfil}` - Produtos por perfil (público)

**Simulações** (autenticado):
- `POST /api/v1/simulacoes` - Criar simulação (cliente só pode simular para si mesmo)
- `GET /api/v1/simulacoes` - Listar simulações do cliente (retorna apenas dados próprios)
- `GET /api/v1/simulacoes/{id}` - Detalhe da simulação (valida ownership)

**Perfil de Risco** (autenticado):
- `GET /api/v1/perfil-risco` - Buscar perfil do cliente (apenas dados próprios)

---

## Segurança

### Password Hashing
- **Algoritmo**: BCrypt
- **Work Factor**: 12 (equilíbrio entre segurança e performance)
- **Salt**: Gerado automaticamente pelo BCrypt

### JWT
- **Algoritmo**: HS256 (HMAC-SHA256)
- **Secret**: Mínimo 256 bits
- **Access Token**: 15 minutos de validade
- **Refresh Token**: 7 dias de validade

### Validações
- **Audience Validation**: Token interno não funciona na API pública (e vice-versa)
- **Ownership Validation**: Cliente só acessa seus próprios dados
- **Rate Limiting**: Proteção contra brute force em endpoints de autenticação

---

## Fluxos de Autenticação

### 1. Login de Cliente

```
1. Cliente envia email + password
2. Sistema busca Customer por email
3. Sistema verifica hash BCrypt
4. Sistema gera Access Token (JWT, 15min)
5. Sistema gera Refresh Token (opaco, 7 dias)
6. Sistema salva Refresh Token hasheado no banco
7. Sistema retorna ambos os tokens
```

### 2. Requisição Autenticada

```
1. Cliente envia request com Authorization: Bearer <token>
2. Middleware valida JWT (assinatura, expiração, audience)
3. Middleware extrai claims (clienteId, userId, type)
4. Controller usa claims para filtrar dados do cliente
```

### 3. Refresh de Token

```
1. Cliente envia refresh token
2. Sistema busca token por hash no banco
3. Sistema valida (não expirado, não revogado)
4. Sistema rotaciona tokens (revoga antigo, gera novos)
5. Sistema retorna novos tokens
```

---

## Checklist de Segurança

- Passwords hasheados com BCrypt (work factor 12)
- JWT assinado com HS256 (secret > 256 bits)
- Access tokens com expiração curta (15 min)
- Refresh tokens com rotação automática
- Refresh tokens armazenados hasheados (SHA256)
- Audience validation (evita uso cruzado de tokens)
- HTTPS obrigatório (production)
- Rate limiting em endpoints de login/refresh
- CORS configurado corretamente
- Revogação de refresh tokens disponível
- Validação de ownership em endpoints (cliente só vê seus dados)
- Claims customizadas (clienteId, type, roles)
- Policies baseadas em permissões granulares

---

**Versão**: 1.0
**Última atualização**: 2025-11-18
**Status**: Pronto para Revisão
