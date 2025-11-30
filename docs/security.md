# Security Guidelines

## 🔒 Critical Security Notice

**This is a PUBLIC GitHub repository. Anyone on the internet can view all files, commit history, and pull requests.**

### Never Commit:
- ❌ Passwords or database credentials
- ❌ API keys or tokens
- ❌ Connection strings with embedded credentials
- ❌ Private keys or certificates
- ❌ OAuth client secrets
- ❌ Azure subscription IDs or tenant IDs (if sensitive)
- ❌ Any personally identifiable information (PII)
- ❌ Financial data or account numbers
- ❌ Session tokens or JWTs
- ❌ Environment-specific configuration with secrets

---

## Secrets Management Strategy

### Development (Local)

**Use local environment variables ONLY. Never commit .env files.**

#### Backend (.NET)
1. Create `appsettings.Development.json` for local development
2. Add `appsettings.*.json` to `.gitignore` (already configured)
3. Create `appsettings.example.json` as a template (safe to commit):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=moneymatters;Username=YOUR_USERNAME;Password=YOUR_PASSWORD"
  },
  "JwtSettings": {
    "SecretKey": "YOUR_JWT_SECRET_KEY_HERE",
    "Issuer": "MoneyMatters",
    "Audience": "MoneyMattersApp"
  }
}
```

4. Actual `appsettings.Development.json` stays LOCAL ONLY - never committed

#### Frontend (React)
1. Create `.env.local` for local development
2. `.env*` is already in `.gitignore` (except `.env.example`)
3. Create `.env.example` as a template (safe to commit):

```bash
# API Configuration
VITE_API_URL=http://localhost:5000
VITE_API_TIMEOUT=30000

# Auth Configuration (DO NOT commit actual values)
VITE_AUTH0_DOMAIN=your-domain.auth0.com
VITE_AUTH0_CLIENT_ID=your_client_id_here
```

4. Actual `.env.local` stays LOCAL ONLY - never committed

---

### Staging & Production (Azure)

**All secrets MUST be stored in Azure Key Vault.**

#### Azure Key Vault Setup

1. **Create Key Vault:**
```bash
az keyvault create \
  --name money-matters-kv \
  --resource-group money-matters-rg \
  --location eastus
```

2. **Store Secrets:**
```bash
# Database password
az keyvault secret set \
  --vault-name money-matters-kv \
  --name "DatabasePassword" \
  --value "your-secure-password"

# JWT secret key
az keyvault secret set \
  --vault-name money-matters-kv \
  --name "JwtSecretKey" \
  --value "your-jwt-secret"

# API keys
az keyvault secret set \
  --vault-name money-matters-kv \
  --name "PlaidApiKey" \
  --value "your-plaid-api-key"
```

3. **Enable Managed Identity for App Service:**
```bash
# Enable system-assigned managed identity
az webapp identity assign \
  --name money-matters-api \
  --resource-group money-matters-rg

# Grant access to Key Vault
az keyvault set-policy \
  --name money-matters-kv \
  --object-id <managed-identity-object-id> \
  --secret-permissions get list
```

4. **Reference Secrets in App Service Configuration:**
```bash
# Connection string referencing Key Vault
az webapp config connection-string set \
  --name money-matters-api \
  --resource-group money-matters-rg \
  --connection-string-type SQLAzure \
  --settings DefaultConnection="@Microsoft.KeyVault(SecretUri=https://money-matters-kv.vault.azure.net/secrets/DatabasePassword/)"
```

5. **In appsettings.json (production):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=${DB_HOST};Database=${DB_NAME};Username=${DB_USER};Password=${DB_PASSWORD}"
  }
}
```

Environment variables are injected from Key Vault at runtime.

---

### CI/CD (GitHub Actions)

**Store secrets in GitHub Secrets, NOT in workflow files.**

#### Setting Up GitHub Secrets

1. Go to repository Settings → Secrets and variables → Actions
2. Click "New repository secret"
3. Add each secret:

**Required Secrets:**
- `AZURE_CREDENTIALS` - Service principal credentials for Azure deployment
- `AZURE_WEBAPP_PUBLISH_PROFILE` - Publish profile for App Service
- `DATABASE_PASSWORD` - PostgreSQL password (staging/prod)
- `JWT_SECRET_KEY` - JWT signing key
- Any third-party API keys (Plaid, SendGrid, etc.)

#### Using Secrets in GitHub Actions

```yaml
name: Deploy Backend

on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Login to Azure
        uses: azure/login@v1
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}

      - name: Deploy to Azure App Service
        env:
          DATABASE_PASSWORD: ${{ secrets.DATABASE_PASSWORD }}
          JWT_SECRET: ${{ secrets.JWT_SECRET_KEY }}
        run: |
          # Deployment commands here
```

**Never echo or print secrets in workflow logs!**

---

## .gitignore Configuration

Verify these patterns are in `.gitignore`:

```gitignore
# .NET Secrets
appsettings.Development.json
appsettings.Staging.json
appsettings.Production.json
appsettings.*.json
!appsettings.example.json
*.user
*.suo
secrets.json
user-secrets.json

# React / Environment Variables
.env
.env.local
.env.development.local
.env.test.local
.env.production.local
!.env.example

# Azure
*.publishsettings
*.pubxml
*.azurePubxml

# Certificates and Keys
*.pfx
*.p12
*.key
*.pem
*.crt
*.cer
id_rsa*
*.ppk

# Database
*.db
*.db-shm
*.db-wal
*.sqlite

# Logs (may contain sensitive data)
logs/
*.log
npm-debug.log*
yarn-debug.log*
```

---

## Pre-Commit Security Checks

### Install git-secrets

Prevent accidental commits of secrets:

```bash
# Install git-secrets
brew install git-secrets  # macOS
# or
apt-get install git-secrets  # Ubuntu

# Initialize in repository
cd /path/to/money-matters
git secrets --install
git secrets --register-aws  # Adds AWS patterns
```

### Add Custom Patterns

```bash
# Prevent common secret patterns
git secrets --add 'password\s*=\s*.+'
git secrets --add 'api[_-]?key\s*=\s*.+'
git secrets --add 'secret[_-]?key\s*=\s*.+'
git secrets --add 'ConnectionStrings.*Password=.+'
git secrets --add 'VITE_.*_SECRET.*=.+'
```

### Enable GitHub Secret Scanning

GitHub automatically scans for known secret patterns. Enable it:

1. Go to repository Settings → Security → Code security and analysis
2. Enable "Secret scanning"
3. Enable "Push protection" (prevents pushes with secrets)

---

## Code Review Checklist

Before approving any PR, verify:

- [ ] No hardcoded passwords, API keys, or credentials
- [ ] No connection strings with embedded passwords
- [ ] All secrets use environment variables or Key Vault references
- [ ] No `.env` files committed (only `.env.example`)
- [ ] No `appsettings.Development.json` or similar committed
- [ ] No credentials in comments or documentation
- [ ] No test data with real emails, phone numbers, or financial info
- [ ] No Azure subscription IDs or tenant IDs (if sensitive)
- [ ] Logs don't expose sensitive information

---

## What If a Secret Was Committed?

### Immediate Actions:

1. **Rotate the secret immediately**
   - Change database passwords
   - Regenerate API keys
   - Create new JWT signing keys
   - Update Azure Key Vault

2. **Remove from Git history** (if just committed):
```bash
# If you haven't pushed yet
git reset HEAD~1
git add .
git commit -m "Fix: Remove secrets"

# If you've already pushed
git rebase -i HEAD~1  # Mark commit for edit
# Remove the secret from files
git add .
git rebase --continue
git push --force-with-lease
```

3. **Use BFG Repo-Cleaner for older commits:**
```bash
# Install BFG
brew install bfg  # macOS

# Clone a fresh copy
git clone --mirror https://github.com/charleslbryant/money-matters.git

# Remove files with secrets
bfg --delete-files secrets.json money-matters.git
bfg --replace-text passwords.txt money-matters.git  # File with patterns to replace

# Clean up
cd money-matters.git
git reflog expire --expire=now --all
git gc --prune=now --aggressive

# Force push (WARNING: This rewrites history)
git push --force
```

4. **Notify the team** that history was rewritten

5. **Verify the secret is gone:**
```bash
git log -S "the-secret-string" --all
```

---

## Environment-Specific Configuration Examples

### Backend: appsettings.example.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=moneymatters;Username=YOUR_DB_USER;Password=YOUR_DB_PASSWORD"
  },
  "JwtSettings": {
    "SecretKey": "REPLACE_WITH_SECURE_RANDOM_KEY_AT_LEAST_32_CHARS",
    "Issuer": "MoneyMatters",
    "Audience": "MoneyMattersApp",
    "ExpirationMinutes": 60
  },
  "AzureKeyVault": {
    "VaultUri": "https://YOUR-KEYVAULT-NAME.vault.azure.net/"
  },
  "Plaid": {
    "ClientId": "YOUR_PLAID_CLIENT_ID",
    "Secret": "YOUR_PLAID_SECRET",
    "Environment": "sandbox"
  }
}
```

### Frontend: .env.example

```bash
# API Configuration
VITE_API_URL=http://localhost:5000
VITE_API_TIMEOUT=30000

# Authentication (Replace with your values)
VITE_AUTH0_DOMAIN=your-tenant.auth0.com
VITE_AUTH0_CLIENT_ID=your_client_id_here
VITE_AUTH0_AUDIENCE=https://api.moneymatters.com

# Feature Flags
VITE_ENABLE_PLAID=true
VITE_ENABLE_ANALYTICS=false

# DO NOT COMMIT ACTUAL VALUES!
# Copy this file to .env.local and replace with real values
```

---

## Azure Key Vault Integration Example

### Backend (.NET)

**Program.cs:**
```csharp
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

var builder = WebApplication.CreateBuilder(args);

// Add Azure Key Vault
if (!builder.Environment.IsDevelopment())
{
    var keyVaultUri = builder.Configuration["AzureKeyVault:VaultUri"];
    var credential = new DefaultAzureCredential();

    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUri),
        credential
    );
}

// Services can now access secrets from Key Vault
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
```

---

## Monitoring & Alerts

### Setup Alerts for Security Events

1. **Azure Monitor** - Alert on:
   - Failed authentication attempts (> 10 in 5 minutes)
   - Unusual access patterns
   - Key Vault access from unexpected IPs

2. **GitHub** - Enable notifications for:
   - Secret scanning alerts
   - Dependabot security alerts
   - Code scanning findings

3. **Application Insights** - Monitor:
   - Exception rates
   - Unauthorized access attempts
   - SQL injection attempts

---

## Security Best Practices Summary

✅ **DO:**
- Use Azure Key Vault for all production secrets
- Use environment variables for local development
- Create `.example` files as templates
- Rotate secrets regularly (every 90 days minimum)
- Use strong, randomly generated secrets (32+ characters)
- Enable MFA on Azure portal and GitHub
- Review all PRs for security issues
- Use Managed Identities instead of connection strings when possible
- Enable GitHub secret scanning and push protection

❌ **DON'T:**
- Commit secrets to the repository
- Share secrets via email, Slack, or Teams
- Use weak or default passwords
- Hardcode credentials in source code
- Store secrets in comments
- Use the same secret across environments
- Disable security features for convenience
- Ignore security warnings from GitHub or Azure

---

## Questions or Concerns?

If you discover a security vulnerability or have committed a secret accidentally:

1. **DO NOT** create a public GitHub issue
2. Contact the repository owner directly
3. Rotate the secret immediately
4. Document the incident for future prevention

---

## Additional Resources

- [Azure Key Vault Documentation](https://docs.microsoft.com/en-us/azure/key-vault/)
- [GitHub Secret Scanning](https://docs.github.com/en/code-security/secret-scanning)
- [OWASP Secrets Management Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Secrets_Management_Cheat_Sheet.html)
- [git-secrets GitHub Repository](https://github.com/awslabs/git-secrets)
- [BFG Repo-Cleaner](https://rtyley.github.io/bfg-repo-cleaner/)

---

🤖 Submitted by George with love ♥
