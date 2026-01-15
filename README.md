# Product Catalog API

API REST para gerenciamento de catálogo de produtos com autenticação JWT, MongoDB e PostgreSQL.

## ?? Como Rodar

### Pré-requisitos
- .NET 8 SDK
- PostgreSQL 14+
- MongoDB 5+
- Docker

### Instalação Local

1. **Clone o repositório:**
```bash
git clone https://github.com/JoseLSousa/Deal.ProductCatalog
cd ProductCatalog
```

2. **Configure as variáveis de ambiente:**

Edite o arquivo `API/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "Postgres": "Host=localhost;Port=5432;Database=productcatalog;Username=postgres;Password=your_password",
    "MongoDB": "mongodb://localhost:27017"
  },
  "JwtSettings": {
    "SecretKey": "your_super_secret_key_min_32_characters_long!",
    "Issuer": "ProductCatalogAPI",
    "Audience": "ProductCatalogClient",
    "ExpiryInMinutes": 60
  }
}
```

3. **Inicie o PostgreSQL e MongoDB:**

```bash
# PostgreSQL
docker run -d --name postgres -e POSTGRES_PASSWORD=your_password -p 5432:5432 postgres:14

# MongoDB
docker run -d --name mongodb -p 27017:27017 mongo:5
```

4. **Aplique as migrations:**
```bash
cd Infra
dotnet ef database update
cd ..
```

5. **Execute a aplicação:**
```bash
cd API
dotnet run
```

A API estará disponível em `https://localhost:5001`

### Executar com Docker Compose

```bash
docker-compose up -d
dotnet ef database update
cd API && dotnet run
```

---

## ?? Rotas Principais

### Authentication

#### Registrar novo usuário
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123!",
  "role": "Viewer"
}
```

**Response (200):**
```json
{
  "message": "Usuário registrado com sucesso"
}
```

#### Fazer login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123!"
}
```

**Response (200):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "user@example.com",
  "role": "Viewer",
  "expiration": "2024-01-15T10:30:00Z"
}
```

#### Obter informações do usuário
```http
GET /api/auth/me
Authorization: Bearer {token}
```

**Response (200):**
```json
{
  "userId": "uuid",
  "email": "user@example.com",
  "userName": "user@example.com",
  "roles": ["Viewer"]
}
```

### Produtos

#### Listar todos os produtos
```http
GET /api/products
Authorization: Bearer {token}
```

**Response (200):**
```json
[
  {
    "productId": "uuid",
    "name": "Product Name",
    "description": "Product Description",
    "price": 99.99,
    "active": true,
    "categoryId": "uuid"
  }
]
```

#### Buscar produto por ID
```http
GET /api/products/{id}
Authorization: Bearer {token}
```

**Response (200):**
```json
{
  "productId": "uuid",
  "name": "Product Name",
  "description": "Product Description",
  "price": 99.99,
  "active": true,
  "categoryId": "uuid"
}
```

#### Criar novo produto
```http
POST /api/products
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "New Product",
  "description": "Product Description",
  "price": 99.99,
  "active": true,
  "categoryId": "uuid"
}
```

**Response (201):**
```json
{
  "productId": "uuid",
  "name": "New Product",
  "description": "Product Description",
  "price": 99.99,
  "active": true,
  "categoryId": "uuid"
}
```

#### Atualizar produto
```http
PUT /api/products/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Updated Product",
  "description": "Updated Description",
  "price": 129.99,
  "active": true
}
```

**Response (200):**
```json
{
  "productId": "uuid",
  "name": "Updated Product",
  "description": "Updated Description",
  "price": 129.99,
  "active": true,
  "categoryId": "uuid"
}
```

#### Deletar produto
```http
DELETE /api/products/{id}
Authorization: Bearer {token}
```

**Response (204):** No Content

#### Listar produtos por categoria
```http
GET /api/products/category/{categoryId}
Authorization: Bearer {token}
```

**Response (200):**
```json
[
  {
    "productId": "uuid",
    "name": "Product Name",
    "description": "Product Description",
    "price": 99.99,
    "active": true,
    "categoryId": "uuid"
  }
]
```

### Categorias

#### Listar todas as categorias
```http
GET /api/categories
Authorization: Bearer {token}
```

**Response (200):**
```json
[
  {
    "categoryId": "uuid",
    "name": "Electronics",
    "description": "Electronic products"
  }
]
```

#### Criar nova categoria
```http
POST /api/categories
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "New Category",
  "description": "Category Description"
}
```

**Response (201):**
```json
{
  "categoryId": "uuid",
  "name": "New Category",
  "description": "Category Description"
}
```

#### Atualizar categoria
```http
PUT /api/categories/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Updated Category",
  "description": "Updated Description"
}
```

**Response (200):**
```json
{
  "categoryId": "uuid",
  "name": "Updated Category",
  "description": "Updated Description"
}
```

#### Deletar categoria
```http
DELETE /api/categories/{id}
Authorization: Bearer {token}
```

**Response (204):** No Content

### Tags

#### Listar todas as tags
```http
GET /api/tags
Authorization: Bearer {token}
```

**Response (200):**
```json
[
  {
    "tagId": "uuid",
    "name": "New"
  }
]
```

#### Criar nova tag
```http
POST /api/tags
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Premium"
}
```

**Response (201):**
```json
{
  "tagId": "uuid",
  "name": "Premium"
}
```

### Importação e Exportação

#### Importar produtos de API externa
```http
POST /api/import
Authorization: Bearer {token}
```

**Response (200):**
```json
{
  "message": "Importação concluída.",
  "data": {
    "totalFetched": 20,
    "imported": 18,
    "skipped": 2,
    "messages": ["Produto importado com sucesso", "..."]
  }
}
```

#### Gerar relatório de produtos
```http
GET /api/reports/products?format=json
Authorization: Bearer {token}
```

**Query Parameters:**
- `format`: json ou csv (padrão: json)

**Response (200):**
```json
{
  "reportDate": "2024-01-15",
  "totalProducts": 150,
  "statistics": {
    "activeProducts": 145,
    "deletedProducts": 5,
    "averagePrice": 89.50
  },
  "products": [...]
}
```

### Busca

#### Buscar produtos com filtros
```http
GET /api/products/search?name=test&minPrice=10&maxPrice=100&active=true
Authorization: Bearer {token}
```

**Query Parameters:**
- `name`: Nome do produto (partial match)
- `minPrice`: Preço mínimo
- `maxPrice`: Preço máximo
- `active`: true/false
- `page`: Número da página (padrão: 1)
- `pageSize`: Quantidade por página (padrão: 10)

**Response (200):**
```json
{
  "totalCount": 45,
  "pageNumber": 1,
  "pageSize": 10,
  "items": [...]
}
```

### Health Check

#### Verificar saúde da aplicação
```http
GET /health
```

**Response (200):**
```json
{
  "status": "Healthy",
  "checks": {
    "postgres": "Healthy",
    "mongodb": "Healthy"
  }
}
```

---

## ?? Autenticação

Todas as rotas (exceto `/health`, `/api/auth/register` e `/api/auth/login`) requerem autenticação via JWT.

Inclua o token no header da requisição:
```
Authorization: Bearer {token}
```

## ?? Roles Disponíveis

- **Admin**: Acesso total a todas as funcionalidades
- **Editor**: Permissão para criar e editar produtos
- **Viewer**: Apenas visualização de produtos (leitura)

---

## ?? Tecnologias

- **.NET 8**: Framework
- **Entity Framework Core**: ORM
- **PostgreSQL**: Banco de dados relacional
- **MongoDB**: Banco de dados NoSQL
- **ASP.NET Identity**: Autenticação
- **JWT**: Tokens de autorização
- **FluentValidation**: Validação de dados
- **xUnit**: Testes unitários

---

## ?? Licença

Este projeto está sob a licença MIT.

## ????? Autor

José Luiz Sousa - [GitHub](https://github.com/JoseLSousa)