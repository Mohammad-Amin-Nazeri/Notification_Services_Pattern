# Tenant-aware Notification Configuration

`NotificationServices.Kit` intentionally does not own tenant, license, database, or user-resolution logic.

The application owns that context and supplies notification settings through `INotificationOptionsProvider`.

## Recommended model

```text
Request
  │
  ▼
Tenant / License Context
  │
  ▼
Application Repository / Cache
  │
  ▼
INotificationOptionsProvider
  │
  ├── Email options
  └── SMS options
        │
        ▼
  ISmsProviderFactory
        │
        ├── Melipayamak
        ├── Kavenegar
        └── Other registered provider
```

The important rule is that the options provider should be **scoped** when its result depends on the current tenant, license, user, or request.

## Example

A consuming application may store this information in its own database:

```text
License
--------
Id
TenantId
SmsProvider
SmsUsername
SmsPassword
SmsFrom
SmsBaseUrl
SmsPatternBaseUrl
SmsBodyId
EmailHost
EmailPort
EmailUsername
EmailPassword
EmailFromAddress
EmailFromName
```

The application can then implement:

```csharp
public sealed class LicenseNotificationOptionsProvider(
    ILicenseContext licenseContext,
    ILicenseRepository licenseRepository)
    : INotificationOptionsProvider
{
    public async ValueTask<NotificationOptions> GetOptionsAsync(
        CancellationToken cancellationToken = default)
    {
        var license = await licenseRepository.GetAsync(
            licenseContext.LicenseId,
            cancellationToken);

        if (license is null)
            throw new InvalidOperationException("Notification license was not found.");

        return new NotificationOptions
        {
            Email = new EmailOptions
            {
                Host = license.EmailHost,
                Port = license.EmailPort,
                EnableSsl = license.EmailEnableSsl,
                Username = license.EmailUsername,
                Password = license.EmailPassword,
                FromAddress = license.EmailFromAddress,
                FromName = license.EmailFromName
            },
            Sms = new SmsOptions
            {
                ProviderType = license.SmsProvider,
                Username = license.SmsUsername,
                Password = license.SmsPassword,
                From = license.SmsFrom,
                BaseUrl = license.SmsBaseUrl,
                PatternBaseUrl = license.SmsPatternBaseUrl,
                BodyId = license.SmsBodyId
            }
        };
    }
}
```

Register it as the application-owned provider:

```csharp
services.AddScoped<ILicenseContext, LicenseContext>();
services.AddNotificationServices<LicenseNotificationOptionsProvider>();
```

## Provider registration

Provider selection is runtime-driven. For example:

```csharp
services.AddSmsProvider<MelipayamakSmsProvider>("Melipayamak");
services.AddSmsProvider<KavenegarSmsProvider>("Kavenegar");
```

The license can therefore select `Melipayamak` for one tenant and `Kavenegar` for another without changing `ISmsService` or the notification package.

## Important security rules

- Never store SMS or SMTP passwords in source control.
- Prefer a secret manager, encrypted configuration, or protected database fields for credentials.
- Do not cache tenant-specific options in a singleton.
- A tenant/license context should be scoped to the current request or operation.
- Validate the license before returning notification credentials.
- If configuration is cached, include the tenant/license identity in the cache key and apply an appropriate expiration strategy.

This design keeps the package independent of EF Core, Dapper, SQL Server, Redis, or any particular licensing system while still supporting per-license provider selection.
