FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env
WORKDIR /app
COPY SkillSystem.Core/SkillSystem.Core.csproj SkillSystem.Core/
COPY SkillSystem.Application/SkillSystem.Application.csproj SkillSystem.Application/
COPY SkillSystem.Infrastructure/SkillSystem.Infrastructure.csproj SkillSystem.Infrastructure/
COPY SkillSystem.IdentityServer4.Client/SkillSystem.IdentityServer4.Client.csproj SkillSystem.IdentityServer4.Client/
COPY SkillSystem.WebApi/SkillSystem.WebApi.csproj SkillSystem.WebApi/
RUN dotnet restore "SkillSystem.WebApi/SkillSystem.WebApi.csproj"
COPY . .
RUN dotnet publish "SkillSystem.WebApi/SkillSystem.WebApi.csproj" -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime
WORKDIR /publish
COPY --from=build-env /publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "SkillSystem.WebApi.dll"]