FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env
WORKDIR /app
COPY SkillSystem.IdentityServer4/SkillSystem.IdentityServer4.csproj SkillSystem.IdentityServer4/
RUN dotnet restore "SkillSystem.IdentityServer4/SkillSystem.IdentityServer4.csproj"
COPY . .
RUN dotnet publish "SkillSystem.IdentityServer4/SkillSystem.IdentityServer4.csproj" -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime
WORKDIR /publish
COPY --from=build-env /publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "SkillSystem.IdentityServer4.dll"]