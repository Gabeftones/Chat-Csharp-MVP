# Chat em rede com ASP.NET Core

Este projeto é um chat web em tempo real com autenticação simples por usuário e senha.

Ele foi desenvolvido para atender aos requisitos de:

- login com usuário e senha
- usuários previamente cadastrados em arquivo JSON no servidor
- funcionamento em rede e com múltiplas telas
- histórico salvo em JSON por sala e participantes

## Requisitos

- .NET 9 SDK
- Visual Studio Code ou terminal com comando `dotnet`

## Como executar

1. Abra o terminal na pasta do projeto:

   ```bash
   cd ChatMVC
   ```

2. Execute a aplicação:

   ```bash
   dotnet run
   ```

3. Acesse no navegador:

   ```text
   http://localhost:5099
   ```

> Se a porta 5099 estiver ocupada, o projeto pode usar outra porta configurada pelo ASP.NET Core.

## Usuários de teste

Ao iniciar a aplicação, o sistema cria automaticamente um arquivo JSON com usuários padrão:

- admin / 123456
- aluno / 123456
- professor / 123456

Esses usuários ficam salvos em uma pasta local do servidor, no arquivo:

```text
ChatMVC/Data/UsuariosChat.json
```

## Como usar

1. Na tela de login, informe um usuário e senha válidos.
2. Informe o nome da sala.
3. Clique em entrar.
4. Abra outra aba ou outra janela do navegador para testar o chat em múltiplas telas.
5. Faça login com outro usuário e acesse a mesma sala.
6. As mensagens aparecerão em tempo real para todos da sala.

## Histórico do chat

As conversas são gravadas em JSON no servidor, em:

```text
ChatMVC/Data/SalasChat.json
```

Esse histórico pertence apenas às pessoas participantes da sala e fica disponível para consulta ao acessar a mesma conversa novamente.

## Estrutura principal

- `Controllers/` - controle das telas e login
- `Hubs/` - comunicação em tempo real
- `Models/` - modelos de usuário, sala e mensagem
- `Services/` - lógica de persistência e autenticação em JSON
- `Views/` - páginas do chat
- `Data/` - arquivos JSON do servidor

## Observação

Este projeto foi pensado como uma solução simples de estudo e demonstração. O objetivo é mostrar a persistência em JSON e o funcionamento do chat em rede sem a necessidade de banco de dados.
