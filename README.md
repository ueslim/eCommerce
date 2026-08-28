# Teste Prático — Desenvolvedor .NET Senior

Este é o backend de um sistema de gestão de pedidos para um e-commerce simples, desenvolvido utilizando as práticas de desenvolvimento de software sob a plataforma **.NET 10**.

---

## 🛠️ Stack Técnica e Decisões de Arquitetura

- **.NET 10 Web API (Controllers):** Conforme permitido pelas diretrizes do teste, optei pelo uso de **Controllers tradicionais** em vez de Minimal APIs. Em arquiteturas corporativas complexas que utilizam CQRS, a organização em Controllers estruturados melhora sensivelmente a legibilidade, o encapsulamento de rotas e o suporte nativo a filtros do ASP.NET. Em conjunto com o **MediatR**, implementamos o padrão de *Thin Controllers* (controladores magros), onde nenhuma lógica de negócio ou persistência reside nos controladores, atuando apenas como despacho de intenções para a camada de Application.
- **Clean Architecture:** Divisão rigorosa em 4 camadas (`Domain`, `Application`, `Infrastructure` e `API`) para garantir o isolamento absoluto das regras de domínio de preocupações externas.
- **CQRS com MediatR:** Separação clara dos fluxos de leitura (Queries) e escrita (Commands), otimizando a evolução e manutenção do sistema.
- **EF Core com SQLite & Auto-migrations:** O banco de dados SQLite é provisionado localmente de forma isolada. Para garantir o princípio de funcionamento imediato (*plug-and-play*), a aplicação executa as migrações automaticamente no banco de dados na inicialização do pipeline do ASP.NET.
- **FluentValidation com Pipeline Behavior:** Validação de entrada interceptada de forma transversal. Antes de qualquer comando atingir seu Handler, as regras definidas com FluentValidation são validadas de forma centralizada pelo MediatR Pipeline.
- **Gerenciamento de Ambiente via `.env`:** Todas as credenciais de segurança e configurações sensíveis (portas, strings de conexão e chaves JWT) são centralizadas em um arquivo `.env` seguro e não versionado no Git, em conformidade com as boas práticas de segurança de credenciais.

---

## 📐 Evolutividade & Prontidão para Microsserviços

Embora o desafio exija a entrega de uma API única, a solução foi projetada sob o conceito de **Monólito Modular**. Isso significa que as barreiras físicas e lógicas da aplicação foram desenhadas para que, caso o negócio cresça, o sistema possa ser fatiado em **Microsserviços independentes** de forma cirúrgica e sem atrito.

### Como essa transição é viabilizada na prática:

1. **Desacoplamento Vertical com CQRS & MediatR:**
   Cada caso de uso (como a criação ou cancelamento de um pedido) foi encapsulado em um fluxo vertical isolado (um Command/Query e seu respectivo Handler). Como essas fatias não compartilham estado ou lógica interna na aplicação, extrair a funcionalidade de "Pedidos" para um microsserviço dedicado de *Sales/Ordering* seria tão simples quanto mover as pastas de domínio e aplicação correspondentes para um novo repositório, sem quebrar o restante do ecossistema.

2. **Isolamento de Infraestrutura (Clean Architecture):**
   A camada de `Domain` e `Application` é totalmente agnóstica de banco de dados e frameworks de persistência. Se o volume de pedidos crescer e demandar a substituição do SQLite por um banco relacional altamente escalável (como PostgreSQL/SQL Server) ou até mesmo NoSQL (como MongoDB), a alteração afetará estritamente a camada de `Infrastructure`. Bastará criar um novo projeto de infraestrutura que assine o contrato da interface `IOrderRepository`, blindando as regras de negócio de qualquer impacto colateral.

3. **Prontidão para Arquitetura Direcionada a Eventos (EDA):**
   A estrutura dos Handlers facilita muito a acoplagem de mecanismos de mensageria (como Azure Service Bus ou RabbitMQ). Se no futuro precisarmos notificar outros microsserviços de forma assíncrona sobre a criação de um pedido, podemos publicar um evento de integração (ex: `OrderCreatedEvent`) no final do pipeline do Handler correspondente, sem poluir os controllers ou as regras centrais de validação.

---

## 📂 Estrutura de Pastas

```text
eCommerce/
├── src/
│   ├── OrderManagement.Domain/          # Entidades, Enums e Interfaces
│   ├── OrderManagement.Application/     # CQRS (Commands, Queries, Handlers e Validators)
│   ├── OrderManagement.Infrastructure/  # DbContext, Migrations e Repositórios
│   └── OrderManagement.API/             # Controllers, Dockerfile, Program.cs e configurações
├── tests/
│   ├── OrderManagement.UnitTests/       # Testes unitários com xUnit
│   └── OrderManagement.IntegrationTests/ # Testes de integração
├── docker-compose.yml                   # Orquestração do container mapeando as variáveis
├── .env.example                         # Arquivo de exemplo com as variáveis necessárias
└── README.md                            # Guia do projeto
```
---

## 🚀 Como Executar o Projeto

### Pré-requisitos
Para a avaliação completa, compilação e execução dos testes do projeto, você precisará de:
- **.NET 10 SDK** instalado (obrigatório para restaurar dependências, compilar e rodar a suíte de testes locais).
- **Docker Desktop** instalado e ativo (com suporte a Docker Compose para execução isolada da API).

### Configuração do Ambiente (.env)
1. Na raiz do repositório, duplique o arquivo `.env.example` e salve-o com o nome de `.env`:
   ```bash
   cp .env.example .env
   ```
2. Se optar por executar a aplicação localmente pelo Visual Studio (`F5`), certifique-se de copiar o mesmo arquivo `.env` para a raiz do projeto de inicialização da API em: `src/OrderManagement.API/.env`.

---

### Opção A: Execução via Docker (Recomendado)
Para rodar toda a aplicação (API, Banco de Dados SQLite e aplicação automática de migrations) sob container, execute na raiz da solução:

```bash
docker-compose up --build
```

A API estará de pé e a documentação interativa pronta em:
👉 **http://localhost:8080/swagger/index.html**

---

### Opção B: Execução Local (.NET CLI)
Caso prefira rodar a API de forma nativa no seu ambiente:

1. Navegue até a pasta da API:
   ```bash
   cd src/OrderManagement.API
   ```
2. Inicie o servidor:
   ```bash
   dotnet run
   ```

A API subirá utilizando as variáveis de ambiente locais e aplicará as migrações automaticamente no banco SQLite gerado no diretório.

---

## 🧪 Testes Unitários
Seguindo o edital de avaliação, o projeto contém uma suíte completa de testes unitários para validar o comportamento dos Handlers do MediatR e as regras críticas de negócio usando **xUnit** e bibliotecas de isolamento.

Para rodar os testes, execute na raiz do repositório:
```bash
dotnet test
```

---

## 🔑 Credenciais para Autenticação (JWT)
> ⚠️ **Nota de Design:** Em um cenário de produção real, as credenciais de usuários jamais seriam expostas ou mantidas em memória/hardcoded. Elas foram configuradas de forma fixa exclusivamente para atender aos requisitos de avaliação deste teste técnico.

Endpoints de pedidos requerem autenticação por token `Bearer`. As credenciais fixas exigidas para teste são:
- **Endpoint de Login:** `POST /auth/login`
- **Usuário:** `wes@tech.com`
- **Senha:** `Senha@123`

Após requisitar o login, utilize o token JWT recebido no botão **Authorize** no canto superior direito do Swagger no formato `Bearer {token}`.
