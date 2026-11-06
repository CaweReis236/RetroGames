#!/usr/bin/env bash
set -euo pipefail

# Execute este script na pasta raiz do repositório RetroGames.
# Requer .NET SDK 10 e Node.js/npm compatíveis com a versão atual do Vite.

echo "==> Verificando ferramentas..."
command -v dotnet >/dev/null 2>&1 || {
  echo "ERRO: .NET SDK não encontrado. Instale o .NET 10 SDK e tente novamente."
  exit 1
}
command -v node >/dev/null 2>&1 || {
  echo "ERRO: Node.js não encontrado."
  exit 1
}
command -v npm >/dev/null 2>&1 || {
  echo "ERRO: npm não encontrado."
  exit 1
}

DOTNET_VERSION="$(dotnet --version)"
echo "SDK .NET encontrado: ${DOTNET_VERSION}"
case "${DOTNET_VERSION}" in
  10.*) ;;
  *)
    echo "ERRO: este projeto exige o .NET SDK 10. Versão encontrada: ${DOTNET_VERSION}"
    exit 1
    ;;
esac

# Evita sobrescrever projetos existentes.
for caminho in Model Repository Services WebApi.ApiModels WebApi WebApi.UnitTests retrogames.web RetroGames.sln RetroGames.slnx; do
  if [ -e "$caminho" ]; then
    echo "ERRO: '$caminho' já existe. O script foi interrompido para não sobrescrever arquivos."
    exit 1
  fi
done

echo "==> Criando a solução RetroGames.sln..."
dotnet new sln --name RetroGames --format sln

echo "==> Criando os projetos .NET 10..."
dotnet new classlib --name Model --output Model --framework net10.0
dotnet new classlib --name Repository --output Repository --framework net10.0
dotnet new classlib --name Services --output Services --framework net10.0
dotnet new classlib --name WebApi.ApiModels --output WebApi.ApiModels --framework net10.0
dotnet new webapi --name WebApi --output WebApi --framework net10.0 --use-controllers
dotnet new xunit --name WebApi.UnitTests --output WebApi.UnitTests --framework net10.0

echo "==> Adicionando os seis projetos à solução..."
dotnet sln RetroGames.sln add \
  Model/Model.csproj \
  Repository/Repository.csproj \
  Services/Services.csproj \
  WebApi.ApiModels/WebApi.ApiModels.csproj \
  WebApi/WebApi.csproj \
  WebApi.UnitTests/WebApi.UnitTests.csproj

echo
echo "==> Criando o front-end React + Vite (JavaScript)..."
npm create vite@latest retrogames.web -- --template react
cd retrogames.web

echo "==> Instalando dependências do front-end..."
npm install
npm install bootstrap axios

cd ..

echo
echo "ESTRUTURA CRIADA."
echo "ATENÇÃO: as referências entre projetos ainda NÃO foram adicionadas."
echo "A tabela 3.2 da especificação do professor é necessária para adicioná-las sem adivinhar."
echo
echo "Próximos comandos, depois de inserir as referências da tabela 3.2:"
echo "  dotnet build RetroGames.sln"
echo "  cd retrogames.web && npm run dev"
