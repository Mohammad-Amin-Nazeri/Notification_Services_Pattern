# 📦 NotificationServices.Kit

A reusable **.NET 10** notification package for sending **Email and SMS** through simple abstractions, dependency injection, and provider-based SMS integration.

The package keeps application code independent from provider-specific implementations and configuration sources.

[![CI](https://github.com/Mohammad-Amin-Nazeri/Notification_Services_Pattern/actions/workflows/ci.yml/badge.svg)](https://github.com/Mohammad-Amin-Nazeri/Notification_Services_Pattern/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/NotificationServices.Kit?label=NuGet)](https://www.nuget.org/packages/NotificationServices.Kit)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

<a href="#english">🇬🇧 English</a> &nbsp;•&nbsp; <a href="#فارسی">🇮🇷 فارسی</a>

---

<a id="english"></a>

## 🇬🇧 English

### Overview

`NotificationServices.Kit` provides a small, reusable API for Email and SMS notifications without coupling consuming applications to a specific SMS gateway or configuration source.

```text
Application
    │
    ├── IEmailService
    └── ISmsService
           │
           ▼
 NotificationServices.Kit
       ┌───────┴───────┐
       ▼               ▼
    Email             SMS
    SMTP          Provider Factory
                       │
                       ▼
                  SMS Provider
```

### Features

- 📧 Email messages through SMTP / MailKit
- 🔐 Email OTP
- 📱 SMS messages
- 🔐 SMS OTP
- 🔌 Provider-based SMS architecture
- ⚙️ Replaceable configuration source
- 💉 Dependency injection registration
- ⛔ `CancellationToken` support
- 🧪 Automated tests
- 📊 CI and coverage
- 📦 Unified NuGet package
- 🛡️ No database or persistence dependency

### Installation

```bash
dotnet add package NotificationServices.Kit
```

Or:

```powershell
Install-Package NotificationServices.Kit
```

The current package version is **2.0.1**. It targets **.NET 10** and packages the Email and SMS implementation assemblies. fileciteturn159file0

### Quick Start

Register the services:

```csharp
using NotificationServices.DependencyInjection;

builder.Services.AddNotificationServices();
```

Resolve the services through their abstractions:

```csharp
using NotificationServices.Email.Abstractions.Interfaces;
using NotificationServices.Sms.Abstractions.Interfaces;

var emailService = serviceProvider.GetRequiredService<IEmailService>();
var smsService = serviceProvider.GetRequiredService<ISmsService>();
```

### Send Email

```csharp
var result = await emailService.SendMessageAsync(
    new EmailMessage(
        "user@example.com",
        "Welcome",
        "<h1>Welcome to the application.</h1>",
        isHtml: true));
```

### Send Email OTP

```csharp
var result = await emailService.SendOtpAsync(
    new EmailOtp("user@example.com", "123456"));
```

### Send SMS

```csharp
var result = await smsService.SendMessageAsync(
    new SmsMessage(
        "09120000000",
        "Your order has been registered."));
```

### Send SMS OTP

```csharp
var result = await smsService.SendOtpAsync(
    new SmsOtp("09120000000", "123456"));
```

Both public notification interfaces expose asynchronous operations and accept `CancellationToken`. fileciteturn160file0 fileciteturn161file0

### Configuration

The default provider reads the `NotificationServices` section from application configuration.

```json
{
  "NotificationServices": {
    "Email": {
      "Host": "smtp.example.com",
      "Port": 587,
      "EnableSsl": true,
      "Username": "your-smtp-username",
      "Password": "your-smtp-password",
      "FromAddress": "no-reply@example.com",
      "FromName": "My Application"
    },
    "Sms": {
      "ProviderType": "Melipayamak",
      "Username": "your-sms-username",
      "Password": "your-sms-password",
      "From": "50004001",
      "BaseUrl": "https://example.com/send",
      "PatternBaseUrl": "https://example.com/pattern",
      "BodyId": "your-pattern-id"
    }
  }
}
```

Never commit real credentials. Use environment variables, .NET User Secrets, deployment secrets, a secret manager, or another secure configuration source.

### Custom Configuration Provider

Configuration retrieval is abstracted behind `INotificationOptionsProvider`.

Default:

```csharp
builder.Services.AddNotificationServices();
```

Custom source:

```csharp
builder.Services.AddNotificationServices<DatabaseNotificationOptionsProvider>();
```

The custom provider must implement `INotificationOptionsProvider`. The DI registration supports both the default provider and a custom provider. fileciteturn162file0

This allows configuration to come from sources such as:

```text
appsettings.json ──┐
Database ──────────┤
Redis ─────────────┤──► INotificationOptionsProvider
API ───────────────┤
Secret Store ──────┘
```

### Email API

`IEmailService` exposes two high-level operations:

```csharp
Task<EmailResult> SendMessageAsync(...);
Task<EmailResult> SendOtpAsync(...);
```

The Email implementation uses MailKit/SMTP while consumers depend only on the abstraction. fileciteturn160file0

### SMS Provider Architecture

```text
ISmsService
    │
    ▼
SmsService
    │
    ▼
ISmsProviderFactory
    │
    ▼
ISmsProvider
    │
    ├── Melipayamak
    └── Additional providers
```

Consumers use `ISmsService`; provider-specific HTTP behavior remains behind the provider abstraction. The repository currently includes a **Melipayamak** provider. fileciteturn161file0

### Public API

```text
IEmailService
    ├── SendMessageAsync
    └── SendOtpAsync

ISmsService
    ├── SendMessageAsync
    └── SendOtpAsync
```

The API returns result objects so consuming applications do not need to understand provider-specific response formats.

### Dependency Injection

The package provides two registration entry points:

```csharp
builder.Services.AddNotificationServices();
```

or:

```csharp
builder.Services.AddNotificationServices<MyOptionsProvider>();
```

The registration wires the configuration provider, HTTP client support, Email services, SMS services, adapters, and provider factory. fileciteturn162file0

### Error Handling

Normal send operations return result objects so the application can decide how to handle provider failures.

```csharp
var result = await emailService.SendMessageAsync(
    message,
    cancellationToken);

if (!result.IsSuccess)
{
    // Apply your application's error policy.
}
```

Application-specific retry, logging, alerting, and persistence policies should remain outside the package.

### Architecture

```text
Application
    │
    ▼
INotificationOptionsProvider
    │
    ▼
NotificationServices
   ┌─┴─────────┐
   ▼           ▼
 Email        SMS
   │           │
 SMTP       Provider Factory
               │
               ▼
          SMS Provider
```

The repository separates public abstractions from implementations and keeps provider-specific infrastructure behind interfaces.

### Repository Structure

```text
Notification_Services_Pattern/
├── Email/
│   ├── NotificationServices.Email.Abstractions/
│   └── NotificationServices.Email/
├── Sms/
│   ├── NotificationServices.Sms.Abstractions/
│   └── NotificationServices.Sms/
├── src/
│   └── NotificationServices/
├── Tests/
│   └── NotificationServices.Tests/
├── samples/
│   └── NotificationServices.Sample/
├── .github/workflows/
├── NotificationServices.slnx
├── LICENSE
└── README.md
```

The unified `NotificationServices.Kit` package references the Email and SMS implementations and packages their assemblies for consumers. fileciteturn159file0

### Testing

The repository contains automated tests around configuration, dependency injection, validation, provider selection, HTTP behavior, OTP flows, failure handling, and cancellation.

```bash
dotnet test NotificationServices.slnx
```

### CI/CD

GitHub Actions validates the project and package through a pipeline similar to:

```text
Restore → Build → Test + Coverage → Pack → Verify → Publish
```

The repository contains separate CI and NuGet publishing workflows.

### Packaging

The consumer-facing package is:

```text
NotificationServices.Kit
```

Create a Release package locally:

```bash
dotnet pack src/NotificationServices/NotificationServices.csproj -c Release
```

The package metadata targets .NET 10, uses MIT licensing, and includes the repository README and license. fileciteturn159file0

### Local Development

```bash
git clone https://github.com/Mohammad-Amin-Nazeri/Notification_Services_Pattern.git
cd Notification_Services_Pattern

dotnet restore NotificationServices.slnx
dotnet build NotificationServices.slnx -c Release
dotnet test NotificationServices.slnx -c Release
dotnet pack src/NotificationServices/NotificationServices.csproj -c Release
```

### Roadmap

- More SMS providers
- Additional notification channels
- Stronger provider contract tests
- Observability and resilience hooks
- Further production hardening
- Continued public API and package-versioning improvements

### Contributing

Contributions are welcome, especially for new providers, notification channels, tests, configuration integrations, documentation, and reliability improvements.

Please open an issue or pull request with a clear description of the change.

### License

MIT License. See [LICENSE](LICENSE).

### Author

**Mohammad Amin Nazeri**


[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/mohammad-amin-nazeri)
[![GitHub](https://img.shields.io/badge/GitHub-181717?style=for-the-badge&logo=github&logoColor=white)](https://github.com/Mohammad-Amin-Nazeri)
[![Telegram](https://img.shields.io/badge/Telegram-2CA5E2?style=for-the-badge&logo=telegram&logoColor=white)](https://t.me/Aminn02)
[![Instagram](https://img.shields.io/badge/Instagram-E4405F?style=for-the-badge&logo=instagram&logoColor=white)](https://www.instagram.com/mohammad_amin_nazeri/)

---

<a id="فارسی"></a>

## 🇮🇷 فارسی

### معرفی

`NotificationServices.Kit` یک پکیج قابل استفاده مجدد برای **.NET 10** است که ارسال Notification از طریق **Email و SMS** را با API ساده، Dependency Injection و معماری Provider-Based فراهم می‌کند.

هدف اصلی پکیج این است که Application به Abstractionهای پایدار وابسته باشد و جزئیات Provider و Configuration از کد مصرف‌کننده جدا بماند.

```text
Application
    │
    ├── IEmailService
    └── ISmsService
           │
           ▼
 NotificationServices.Kit
       ┌───────┴───────┐
       ▼               ▼
    Email             SMS
    SMTP          Provider Factory
                       │
                       ▼
                  SMS Provider
```

### ✨ قابلیت‌ها

- 📧 ارسال Email از طریق SMTP / MailKit
- 🔐 ارسال Email OTP
- 📱 ارسال SMS
- 🔐 ارسال SMS OTP
- 🔌 معماری Provider-Based برای SMS
- ⚙️ Configuration قابل تعویض
- 💉 Dependency Injection
- ⛔ پشتیبانی از `CancellationToken`
- 🧪 تست‌های خودکار
- 📊 CI و Coverage
- 📦 یک Package اصلی NuGet
- 🛡️ بدون وابستگی به Database یا Persistence خاص

### 📥 نصب

```bash
dotnet add package NotificationServices.Kit
```

یا:

```powershell
Install-Package NotificationServices.Kit
```

نسخه فعلی Package برابر **2.0.1** است و برای **.NET 10** ساخته می‌شود. Assemblyهای Email و SMS نیز در Package اصلی قرار می‌گیرند. fileciteturn159file0

### ⚡ شروع سریع

ثبت سرویس‌ها:

```csharp
using NotificationServices.DependencyInjection;

builder.Services.AddNotificationServices();
```

دریافت سرویس‌ها:

```csharp
using NotificationServices.Email.Abstractions.Interfaces;
using NotificationServices.Sms.Abstractions.Interfaces;

var emailService = serviceProvider.GetRequiredService<IEmailService>();
var smsService = serviceProvider.GetRequiredService<ISmsService>();
```

### 📧 ارسال Email

```csharp
var result = await emailService.SendMessageAsync(
    new EmailMessage(
        "user@example.com",
        "Welcome",
        "<h1>Welcome to the application.</h1>",
        isHtml: true));
```

### 🔐 ارسال Email OTP

```csharp
var result = await emailService.SendOtpAsync(
    new EmailOtp("user@example.com", "123456"));
```

### 📱 ارسال SMS

```csharp
var result = await smsService.SendMessageAsync(
    new SmsMessage(
        "09120000000",
        "Your order has been registered."));
```

### 🔐 ارسال SMS OTP

```csharp
var result = await smsService.SendOtpAsync(
    new SmsOtp("09120000000", "123456"));
```

هر دو Interface اصلی عملیات Async دارند و `CancellationToken` را پشتیبانی می‌کنند. fileciteturn160file0 fileciteturn161file0

### ⚙️ Configuration

Provider پیش‌فرض بخش `NotificationServices` را از Application Configuration می‌خواند.

```json
{
  "NotificationServices": {
    "Email": {
      "Host": "smtp.example.com",
      "Port": 587,
      "EnableSsl": true,
      "Username": "your-smtp-username",
      "Password": "your-smtp-password",
      "FromAddress": "no-reply@example.com",
      "FromName": "My Application"
    },
    "Sms": {
      "ProviderType": "Melipayamak",
      "Username": "your-sms-username",
      "Password": "your-sms-password",
      "From": "50004001",
      "BaseUrl": "https://example.com/send",
      "PatternBaseUrl": "https://example.com/pattern",
      "BodyId": "your-pattern-id"
    }
  }
}
```

Credential واقعی را داخل Git Commit نکنید. برای محیط‌های واقعی از Environment Variables، User Secrets، Secret Manager یا منبع امن دیگر استفاده کنید.

### 🔌 Configuration Provider سفارشی

Configuration از طریق `INotificationOptionsProvider` از منطق Notification جدا شده است.

حالت پیش‌فرض:

```csharp
builder.Services.AddNotificationServices();
```

منبع سفارشی:

```csharp
builder.Services.AddNotificationServices<DatabaseNotificationOptionsProvider>();
```

Provider سفارشی باید `INotificationOptionsProvider` را پیاده‌سازی کند. Registration مربوط به DI از هر دو حالت پشتیبانی می‌کند. fileciteturn162file0

بنابراین Configuration می‌تواند از منابع مختلف تأمین شود:

```text
appsettings.json ──┐
Database ──────────┤
Redis ─────────────┤──► INotificationOptionsProvider
API ───────────────┤
Secret Store ──────┘
```

### 📧 API مربوط به Email

`IEmailService` دو عملیات اصلی دارد:

```csharp
Task<EmailResult> SendMessageAsync(...);
Task<EmailResult> SendOtpAsync(...);
```

Implementation مربوط به Email از MailKit/SMTP استفاده می‌کند و مصرف‌کننده فقط به Abstraction وابسته است. fileciteturn160file0

### 📱 معماری SMS Provider

```text
ISmsService
    │
    ▼
SmsService
    │
    ▼
ISmsProviderFactory
    │
    ▼
ISmsProvider
    │
    ├── Melipayamak
    └── Providerهای بعدی
```

مصرف‌کننده فقط `ISmsService` را می‌شناسد و جزئیات HTTP مربوط به Provider پشت Abstraction باقی می‌ماند. در Repository فعلی Provider مربوط به **Melipayamak** وجود دارد. fileciteturn161file0

### 🧱 API عمومی

```text
IEmailService
    ├── SendMessageAsync
    └── SendOtpAsync

ISmsService
    ├── SendMessageAsync
    └── SendOtpAsync
```

Result Objectها باعث می‌شوند Application مصرف‌کننده به Response Format اختصاصی Providerها وابسته نباشد.

### 💉 Dependency Injection

دو روش اصلی برای Registration وجود دارد:

```csharp
builder.Services.AddNotificationServices();
```

یا:

```csharp
builder.Services.AddNotificationServices<MyOptionsProvider>();
```

این Registration، Configuration Provider، HttpClient، سرویس‌های Email و SMS، Adapterها و Provider Factory را ثبت می‌کند. fileciteturn162file0

### 🧩 ساختار Repository

```text
Notification_Services_Pattern/
├── Email/
│   ├── NotificationServices.Email.Abstractions/
│   └── NotificationServices.Email/
├── Sms/
│   ├── NotificationServices.Sms.Abstractions/
│   └── NotificationServices.Sms/
├── src/
│   └── NotificationServices/
├── Tests/
│   └── NotificationServices.Tests/
├── samples/
│   └── NotificationServices.Sample/
├── .github/workflows/
├── NotificationServices.slnx
├── LICENSE
└── README.md
```

Package اصلی `NotificationServices.Kit` Implementationهای Email و SMS را در اختیار مصرف‌کننده قرار می‌دهد. fileciteturn159file0

### 🧪 تست

Repository دارای تست‌های خودکار برای بخش‌هایی مانند Configuration، Dependency Injection، Validation، Provider Selection، HTTP، OTP، Failure Handling و Cancellation است.

```bash
dotnet test NotificationServices.slnx
```

### 🤖 CI/CD

GitHub Actions پروژه را با جریان کلی زیر بررسی و Package را آماده انتشار می‌کند:

```text
Restore → Build → Test + Coverage → Pack → Verify → Publish
```

### 📦 ساخت Package

Package اصلی:

```text
NotificationServices.Kit
```

ساخت نسخه Release:

```bash
dotnet pack src/NotificationServices/NotificationServices.csproj -c Release
```

Metadata پکیج برای .NET 10 تنظیم شده، License آن MIT است و README و LICENSE نیز داخل Package قرار می‌گیرند. fileciteturn159file0

### 🛠️ توسعه محلی

```bash
git clone https://github.com/Mohammad-Amin-Nazeri/Notification_Services_Pattern.git
cd Notification_Services_Pattern

dotnet restore NotificationServices.slnx
dotnet build NotificationServices.slnx -c Release
dotnet test NotificationServices.slnx -c Release
dotnet pack src/NotificationServices/NotificationServices.csproj -c Release
```

### 🗺️ Roadmap

- Providerهای بیشتر برای SMS
- Channelهای بیشتر Notification
- تست‌های قوی‌تر برای Providerها
- Observability و Resilience بیشتر
- Production Hardening بیشتر
- بهبود Public API و Package Versioning

### 🤝 مشارکت

Contribution برای Provider جدید، Channel جدید، تست، Configuration Integration، مستندات و بهبود Reliability مورد استقبال است.

برای تغییرات مهم یک Issue یا Pull Request با توضیح واضح ایجاد کنید.

### 📄 License

MIT License. فایل [LICENSE](LICENSE) را مشاهده کنید.

### 👨‍💻 توسعه‌دهنده

**Mohammad Amin Nazeri**


[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/mohammad-amin-nazeri)
[![GitHub](https://img.shields.io/badge/GitHub-181717?style=for-the-badge&logo=github&logoColor=white)](https://github.com/Mohammad-Amin-Nazeri)
[![Telegram](https://img.shields.io/badge/Telegram-2CA5E2?style=for-the-badge&logo=telegram&logoColor=white)](https://t.me/Aminn02)
[![Instagram](https://img.shields.io/badge/Instagram-E4405F?style=for-the-badge&logo=instagram&logoColor=white)](https://www.instagram.com/mohammad_amin_nazeri/)
