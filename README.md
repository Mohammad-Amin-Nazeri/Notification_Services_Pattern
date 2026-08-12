# Notification Services Pattern

[🇬🇧 English](README.md) | [🇮🇷 فارسی](README.fa.md)

[![CI](https://github.com/Mohammad-Amin-Nazeri/Notification_Services_Pattern/actions/workflows/ci.yml/badge.svg)](https://github.com/Mohammad-Amin-Nazeri/Notification_Services_Pattern/actions/workflows/ci.yml)

A reusable and extensible notification infrastructure for .NET applications, providing Email and SMS services with clear abstractions, dependency injection, provider-based SMS architecture, OTP support, configuration isolation, and automated tests.

## ✨ Features

- 📧 General-purpose Email messages
- 🔐 Email OTP messages
- 📱 General-purpose SMS messages
- 🔐 SMS OTP messages
- 🔌 Provider abstraction for SMS gateways
- 🏭 SMS Provider Factory
- ⚙️ Replaceable configuration providers
- 💉 Dependency Injection extensions
- 🧪 Unit tests without external SMS/SMTP calls
- 🤖 GitHub Actions CI for restore, build, and test
- 🧩 Separation between abstractions and implementations

## 🏗️ Architecture

```text
Notification_Services_Pattern
│
├── Email
│   ├── NotificationServices.Email.Abstractions
│   │   ├── Interfaces
│   │   └── Models
│   │
│   └── NotificationServices.Email
│       ├── ConfigurationProviders
│       ├── DependencyInjection
│       └── EmailService
│
├── Sms
│   ├── NotificationServices.Sms.Abstractions
│   │   ├── Enums
│   │   ├── Interfaces
│   │   └── Models
│   │
│   └── NotificationServices.Sms
│       ├── ConfigurationProviders
│       ├── DependencyInjection
│       ├── Providers
│       ├── SmsProviderFactory
│       └── SmsService
│
├── Tests
│   └── NotificationServices.Tests
│
├── samples
│   └── NotificationServices.Sample
│
└── NotificationServices.slnx
```

## 📦 Supported Services

### Email

The Email abstraction exposes two operations:

```csharp
await emailService.SendMessageAsync(
    new EmailMessage(
        "user@example.com",
        "Welcome",
        "<h1>Welcome!</h1>",
        true));
```

```csharp
await emailService.SendOtpAsync(
    new EmailOtp(
        "user@example.com",
        "123456"));
```

### SMS

The SMS abstraction exposes two operations:

```csharp
await smsService.SendMessageAsync(
    new SmsMessage(
        "09120000000",
        "Your order has been registered."));
```

```csharp
await smsService.SendOtpAsync(
    new SmsOtp(
        "09120000000",
        "123456"));
```

The default repository includes a Melipayamak provider implementation. Additional gateways can be added by implementing `ISmsProvider` and wiring the provider into `SmsProviderFactory`.

## ⚙️ Dependency Injection

Register Email:

```csharp
services.AddEmailService();
```

Register SMS:

```csharp
services.AddSmsService();
```

A custom configuration provider can be supplied without changing the core service:

```csharp
services.AddEmailService<MyEmailOptionsProvider>();
services.AddSmsService<MySmsOptionsProvider>();
```

## 🔧 Configuration

The default implementations read provider settings from configuration.

Example:

```json
{
  "SmsProvider": {
    "ProviderType": "Melipayamak",
    "Username": "your-username",
    "Password": "your-password",
    "From": "50004001",
    "BaseUrl": "https://example.com/send",
    "PatternBaseUrl": "https://example.com/pattern",
    "BodyId": "your-body-id"
  },
  "EmailProvider": {
    "Host": "smtp.example.com",
    "Port": 587,
    "EnableSsl": true,
    "Username": "your-smtp-username",
    "Password": "your-smtp-password",
    "FromAddress": "no-reply@example.com",
    "FromName": "My Application"
  }
}
```

Do not commit real passwords, API keys, SMTP credentials, or provider secrets to source control. Use environment variables, .NET User Secrets, or another secure configuration provider for sensitive values.

## 🧩 Extensibility

The project is designed so that application code depends on abstractions rather than provider-specific implementations.

For SMS providers:

```text
ISmsService
    ↓
SmsService
    ↓
ISmsProviderFactory
    ↓
ISmsProvider
    ├── MelipayamakSmsProvider
    └── Future providers...
```

Configuration can also come from sources other than `appsettings.json` by implementing:

```csharp
IEmailProviderOptionsProvider
ISmsProviderOptionsProvider
```

## 🧪 Testing

The repository contains a dedicated test project:

```text
Tests/NotificationServices.Tests
```

The tests cover service validation, provider selection, configuration binding and validation, provider HTTP requests, provider failures, and OTP behavior.

The SMS provider tests use a fake `HttpMessageHandler`, so unit tests do not send real SMS messages or require external services.

Run all tests locally with:

```bash
dotnet test
```

## 🤖 Continuous Integration

GitHub Actions automatically runs the following pipeline for pushes and pull requests targeting `master`:

```text
Restore
  ↓
Build
  ↓
Test
```

Workflow:

```text
.github/workflows/ci.yml
```

## 🚀 Getting Started

Clone the repository:

```bash
git clone https://github.com/Mohammad-Amin-Nazeri/Notification_Services_Pattern.git
cd Notification_Services_Pattern
```

Restore and build:

```bash
dotnet restore
dotnet build
```

Run tests:

```bash
dotnet test
```

The sample application is available under:

```text
samples/NotificationServices.Sample
```

## 🛠️ Technologies

- C#
- .NET 10
- Dependency Injection
- MailKit
- HttpClient
- Microsoft.Extensions.Configuration
- xUnit
- Moq
- GitHub Actions

## 🎯 Project Goals

The main goal of this repository is to provide a small, reusable notification infrastructure that can be copied into different .NET applications or evolved into reusable NuGet packages.

The design keeps application code independent from specific notification gateways and configuration sources.

## 🔮 Roadmap

- [ ] Coverage reporting and badge
- [ ] Additional SMS providers
- [ ] NuGet packages
- [ ] Package metadata and release automation
- [ ] Optional integration tests
- [ ] Additional email features such as templates and attachments

## 📄 License

MIT License. See [`LICENSE`](LICENSE).
