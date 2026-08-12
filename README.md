<div align="center">

# Notification Services Pattern

**A reusable Email and SMS notification infrastructure for .NET applications.**

[![CI](https://github.com/Mohammad-Amin-Nazeri/Notification_Services_Pattern/actions/workflows/ci.yml/badge.svg)](https://github.com/Mohammad-Amin-Nazeri/Notification_Services_Pattern/actions/workflows/ci.yml)

</div>

## Overview

`NotificationServices` provides Email and SMS services behind dependency-injection-friendly abstractions. The library does **not** decide where configuration must come from.

Configuration is supplied through `INotificationOptionsProvider`, so an application can use:

- `appsettings.json`
- a database
- environment variables
- a secret store
- a remote configuration API
- any custom source

The package itself has no database dependency and no assumption about the application's persistence layer.

## Installation

Install the single package:

```bash
dotnet add package NotificationServices
```

The Email/SMS implementation assemblies are distributed inside this package. Consumers do not need to install separate Email or SMS packages.

## Quick Start

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationServices.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

services.AddSingleton<IConfiguration>(configuration);
services.AddNotificationServices();
```

Then resolve the normal service abstractions:

```csharp
var email = serviceProvider.GetRequiredService<IEmailService>();
var sms = serviceProvider.GetRequiredService<ISmsService>();
```

## Default AppSettings Configuration

The built-in provider reads:

```json
{
  "NotificationServices": {
    "Email": {
      "Host": "smtp.example.com",
      "Port": 587,
      "EnableSsl": true,
      "Username": "your-username",
      "Password": "your-password",
      "FromAddress": "no-reply@example.com",
      "FromName": "My Application"
    },
    "Sms": {
      "ProviderType": "Melipayamak",
      "Username": "your-username",
      "Password": "your-password",
      "From": "50004001",
      "BaseUrl": "https://example.com/send",
      "PatternBaseUrl": "https://example.com/pattern",
      "BodyId": "your-body-id"
    }
  }
}
```

## Custom Configuration Source

The library is intentionally source-agnostic. A database implementation belongs in the consuming application, not inside this package.

```csharp
public sealed class DatabaseNotificationOptionsProvider
    : INotificationOptionsProvider
{
    private readonly INotificationSettingsRepository _repository;

    public DatabaseNotificationOptionsProvider(
        INotificationSettingsRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<NotificationOptions> GetOptionsAsync(
        CancellationToken cancellationToken = default)
    {
        var settings = await _repository.GetAsync(cancellationToken);

        return new NotificationOptions
        {
            Email = new EmailOptions
            {
                Host = settings.EmailHost,
                Port = settings.EmailPort,
                EnableSsl = settings.EmailEnableSsl,
                Username = settings.EmailUsername,
                Password = settings.EmailPassword,
                FromAddress = settings.EmailFromAddress,
                FromName = settings.EmailFromName
            },
            Sms = new SmsOptions
            {
                ProviderType = settings.SmsProvider,
                Username = settings.SmsUsername,
                Password = settings.SmsPassword,
                From = settings.SmsFrom,
                BaseUrl = settings.SmsBaseUrl,
                PatternBaseUrl = settings.SmsPatternBaseUrl,
                BodyId = settings.SmsBodyId
            }
        };
    }
}
```

Register it with:

```csharp
services.AddNotificationServices<DatabaseNotificationOptionsProvider>();
```

This is the important architectural boundary: **the consumer chooses the configuration source.**

## Services

### Email

```csharp
await emailService.SendMessageAsync(
    new EmailMessage(
        "user@example.com",
        "Welcome",
        "<h1>Welcome!</h1>",
        true));
```

OTP:

```csharp
await emailService.SendOtpAsync(
    new EmailOtp("user@example.com", "123456"));
```

### SMS

```csharp
await smsService.SendMessageAsync(
    new SmsMessage("09120000000", "Your order has been registered."));
```

OTP:

```csharp
await smsService.SendOtpAsync(
    new SmsOtp("09120000000", "123456"));
```

SMS gateways use a provider abstraction and factory, so additional providers can be added without changing `SmsService`.

## Architecture

```text
Application
    │
    ├── INotificationOptionsProvider
    │       ├── AppSettingsNotificationOptionsProvider
    │       ├── DatabaseNotificationOptionsProvider
    │       └── Any custom provider
    │
    ├── IEmailService ──> EmailService
    │
    └── ISmsService ────> SmsService
                              │
                              └── ISmsProviderFactory
                                      └── ISmsProvider
```

The core package knows the contract, not the infrastructure behind it.

## Security

Never commit SMTP passwords, SMS credentials, API keys, or other secrets to source control. Use environment variables, .NET User Secrets, a managed secret store, or another secure configuration source.

## Testing

The repository contains unit tests for service registration, configuration providers, provider selection, validation, HTTP behavior, failures, and OTP flows.

Run:

```bash
dotnet test NotificationServices.slnx
```

## Build and Package

The release package is built with:

```bash
dotnet pack src/NotificationServices/NotificationServices.csproj -c Release
```

The intended consumer-facing package is:

```text
NotificationServices
```

## License

MIT
