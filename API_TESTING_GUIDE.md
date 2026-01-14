# ?? EXEMPLOS DE TESTES DA API

Este arquivo contém exemplos práticos de como testar todos os endpoints da API usando curl, Postman ou qualquer cliente HTTP.

---

## ?? **PRÉ-REQUISITOS**

1. API rodando em `https://localhost:5001`
2. PostgreSQL e MongoDB rodando (use `docker-compose up -d`)
3. Migrations aplicadas

---

## 1?? **AUTENTICAÇÃO**

### **Registrar Novo Usuário**
```bash
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "email": "admin@teste.com",
    "password": "Admin@123",
    "role": "Admin"
  }'
```

**Resposta Esperada:**
```json
{
  "message": "Usuário registrado com sucesso."
}
```

---

### **Fazer Login**
```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "Admin@123"
  }'
```

**Resposta Esperada:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "admin",
  "email": "admin@teste.com",
  "role": "Admin",
  "expiration": "2024-01-15T11:30:00Z"
}
```

**?? Copie o token para usar nos próximos requests!**

---

### **Obter Informações do Usuário**
```bash
curl -X GET https://localhost:5001/api/auth/me \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

---

## 2?? **PRODUTOS - CRUD**

### **Listar Todos os Produtos**
```bash
curl -X GET https://localhost:5001/api/product \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

---

### **Buscar Produto por ID**
```bash
curl -X GET https://localhost:5001/api/product/{id} \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

---

### **Criar Produto** (Admin/Editor)
```bash
curl -X POST https://localhost:5001/api/product \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Notebook Dell Inspiron 15",
    "description": "Notebook com Intel Core i7, 16GB RAM, 512GB SSD",
    "price": 3500.00,
    "active": true,
    "categoryId": "GUID_DA_CATEGORIA"
  }'
```

**Status Esperado:** `201 Created`

---

### **Atualizar Produto** (Admin/Editor)
```bash
curl -X PUT https://localhost:5001/api/product/{id} \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Notebook Dell XPS 15",
    "description": "Notebook premium",
    "price": 7500.00,
    "active": true,
    "categoryId": "GUID_DA_CATEGORIA"
  }'
```

**Status Esperado:** `204 No Content`

---

### **Atualizar Apenas Preço** (Admin/Editor)
```bash
curl -X PATCH https://localhost:5001/api/product/{id}/price \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -H "Content-Type: application/json" \
  -d '3200.00'
```

---

### **Deletar Produto (Soft Delete)** (Admin)
```bash
curl -X DELETE https://localhost:5001/api/product/{id} \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

---

### **Restaurar Produto Deletado** (Admin)
```bash
curl -X PATCH https://localhost:5001/api/product/{id}/restore \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

---

### **Ativar Produto** (Admin/Editor)
```bash
curl -X PATCH https://localhost:5001/api/product/{id}/activate \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

---

### **Desativar Produto** (Admin/Editor)
```bash
curl -X PATCH https://localhost:5001/api/product/{id}/deactivate \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

---

## 3?? **BUSCA AVANÇADA**

### **Busca com Filtros e Paginação**
```bash
curl -X GET "https://localhost:5001/api/product/search?term=notebook&minPrice=1000&maxPrice=5000&active=true&sortBy=price&sortDescending=false&page=1&pageSize=10" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

**Parâmetros Disponíveis:**
- `term`: Busca em nome e descrição
- `categoryId`: GUID da categoria
- `minPrice`: Preço mínimo
- `maxPrice`: Preço máximo
- `active`: true/false
- `tags`: Lista de tags (separadas por vírgula)
- `sortBy`: name, price, date
- `sortDescending`: true/false
- `page`: Número da página (padrão: 1)
- `pageSize`: Itens por página (padrão: 10, máx: 100)

**Resposta Esperada:**
```json
{
  "items": [...],
  "totalItems": 50,
  "totalPages": 5,
  "currentPage": 1,
  "pageSize": 10,
  "averagePrice": 3200.50,
  "itemsByCategory": {
    "Eletrônicos": 30,
    "Periféricos": 20
  }
}
```

---

## 4?? **IMPORTAÇÃO DE API EXTERNA**

### **Importar Produtos da FakeStore API** (Admin/Editor)
```bash
curl -X POST https://localhost:5001/api/import \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

**Resposta Esperada:**
```json
{
  "message": "Importação concluída.",
  "data": {
    "totalFetched": 20,
    "imported": 18,
    "skipped": 2,
    "messages": [
      "Produto 'Fjallraven - Foldsack No. 1 Backpack' importado com sucesso.",
      "Produto 'Mens Casual Premium Slim Fit T-Shirts' já existe.",
      ...
    ]
  }
}
```

---

## 5?? **EXPORTAÇÃO DE RELATÓRIOS**

### **Baixar Relatório CSV**
```bash
curl -X GET https://localhost:5001/api/reports/items/csv \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -o relatorio.csv
```

---

### **Baixar Relatório JSON**
```bash
curl -X GET https://localhost:5001/api/reports/items/json \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -o relatorio.json
```

---

### **Ver Dados do Relatório (sem download)**
```bash
curl -X GET https://localhost:5001/api/reports/items \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

**Resposta Esperada:**
```json
{
  "products": [
    {
      "name": "Notebook Dell",
      "description": "...",
      "category": "Eletrônicos",
      "price": 3500.00,
      "tags": "notebook,dell",
      "createdAt": "2024-01-15T10:00:00Z"
    }
  ],
  "statistics": {
    "totalActiveProducts": 50,
    "averagePrice": 2500.00,
    "productsByCategory": {
      "Eletrônicos": 30,
      "Periféricos": 20
    },
    "top3MostExpensive": [
      { "name": "MacBook Pro", "price": 12000.00 },
      { "name": "iPhone 14", "price": 8000.00 },
      { "name": "iPad Pro", "price": 6500.00 }
    ]
  }
}
```

---

## 6?? **HEALTH CHECKS**

### **Verificar Saúde da API**
```bash
curl -X GET https://localhost:5001/health
```

**Resposta Esperada:**
```json
{
  "status": "Healthy",
  "checks": [
    {
      "name": "postgres",
      "status": "Healthy",
      "description": "PostgreSQL está funcionando.",
      "duration": 45.23
    },
    {
      "name": "mongodb",
      "status": "Healthy",
      "description": "MongoDB está funcionando.",
      "duration": 12.56
    }
  ],
  "totalDuration": 57.79
}
```

---

## 7?? **CATEGORIAS**

### **Listar Todas as Categorias**
```bash
curl -X GET https://localhost:5001/api/category \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

### **Criar Categoria** (Admin/Editor)
```bash
curl -X POST https://localhost:5001/api/category \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Eletrônicos"
  }'
```

---

## 8?? **TAGS**

### **Listar Todas as Tags**
```bash
curl -X GET https://localhost:5001/api/tag \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

### **Criar Tag** (Admin/Editor)
```bash
curl -X POST https://localhost:5001/api/tag \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "notebook"
  }'
```

---

## ?? **CENÁRIOS DE TESTE**

### **Cenário 1: Fluxo Completo de Produto**
1. Registrar usuário Admin
2. Fazer login e obter token
3. Criar categoria
4. Criar produto
5. Buscar produtos com filtros
6. Atualizar preço do produto
7. Desativar produto
8. Gerar relatório CSV

---

### **Cenário 2: Importação e Exportação**
1. Fazer login como Admin
2. Importar produtos da API externa
3. Verificar produtos importados
4. Exportar relatório JSON
5. Verificar estatísticas no relatório

---

### **Cenário 3: Controle de Acesso**
1. Registrar usuário Viewer
2. Fazer login
3. Tentar criar produto (deve falhar - 403 Forbidden)
4. Listar produtos (deve funcionar)

---

## ?? **TESTES DE ERRO**

### **Validação de Email Inválido**
```bash
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "teste",
    "email": "email_invalido",
    "password": "Test@123",
    "role": "Admin"
  }'
```

**Resposta Esperada:** `400 Bad Request`

---

### **Senha Fraca**
```bash
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "teste",
    "email": "teste@teste.com",
    "password": "123",
    "role": "Admin"
  }'
```

**Resposta Esperada:** `400 Bad Request`

---

### **Produto com Preço Negativo**
```bash
curl -X POST https://localhost:5001/api/product \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Produto Inválido",
    "description": "Teste",
    "price": -10.00,
    "active": true,
    "categoryId": "GUID_DA_CATEGORIA"
  }'
```

**Resposta Esperada:** `400 Bad Request`

---

## ?? **DICAS**

1. **Substitua `SEU_TOKEN_AQUI`** pelo token JWT obtido no login
2. **Substitua `{id}` e GUIDs** pelos valores reais do seu banco
3. **Use Postman ou Insomnia** para facilitar os testes
4. **Verifique os logs do MongoDB** para auditoria:
   ```bash
   docker exec -it productcatalog-mongodb mongosh
   use ProductCatalogAudit
   db.audit_logs.find().pretty()
   ```

---

## ?? **SUPORTE**

Se encontrar algum problema, verifique:
1. A API está rodando?
2. PostgreSQL e MongoDB estão acessíveis?
3. O token JWT está válido?
4. Você tem permissão para a operação?

**Logs da aplicação:**
```bash
dotnet run --project API
```

**Última atualização**: 15 de Janeiro de 2026
