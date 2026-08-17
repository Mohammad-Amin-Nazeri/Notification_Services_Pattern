<div align="center">

# Notification Services Pattern

**A reusable, provider-based .NET notification infrastructure for Email and SMS.**

[![CI](https://github.com/Mohammad-Amin-Nazeri/Notification_Services_Pattern/actions/workflows/ci.yml/badge.svg)](https://github.com/Mohammad-Amin-Nazeri/Notification_Services_Pattern/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-13.0-239120?logo=csharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)

<a href="#english"><strong>🇬🇧 English</strong></a>
&nbsp;&nbsp;•&nbsp;&nbsp;
<a href="#فارسی">🇮🇷 فارسی</a>

</div>

---

<a id="english"></a>

# 🇬🇧 English

<a href="#فارسی">🇮🇷 رفتن به فارسی</a>

## Overview

`NotificationServices` is a reusable .NET library for sending Email and SMS notifications through a small, dependency-injection-friendly API.

The project is designed around two principles:

1. **Consumers should depend on stable notification abstractions.**
2. **The library should not decide where application configuration is stored.**

The built-in configuration provider reads from `appsettings.json`, while applications can provide their own `INotificationOptionsProvider` implementation for databases, Redis, APIs, environment variables, secret stores, or other configuration sources.

The library does not require EF Core, Dapper, SQL Server, Redis, or any specific persistence technology.

## Features

- 📧 Email messages through SMTP / MailKit
- 🔐 Email OTP messages
- 📱 SMS messages
- 🔐 SMS OTP messages
- 🔌 Provider-based SMS architecture
- ⚙️ Replaceable configuration source
- 💉 Simple dependency injection registration
- 🧪 Automated unit tests
- 📊 Code coverage in CI
- 🤖 GitHub Actions CI/CD
- 📦 Unified `NotificationServices.Kit` NuGet package
- 🛡️ No database-specific coupling
- 🧩 Clear separation between public abstractions and implementations
- ⛔ Proper cancellation support through `CancellationToken`

## Installation

Install the unified consumer package:

```bash
dotnet add package NotificationServices.Kit
```

Or from Visual Studio Package Manager Console:

```powershell
Install-Package NotificationServices.Kit
```

The repository also contains the Email and SMS implementation projects, their abstraction projects, a sample application, and automated tests.

## Quick Start

### Register the services

```csharp
using NotificationServices.DependencyInjection;

services.AddNotificationServices();
```

The default registration uses the built-in `appsettings.json` configuration provider.

You can also provide your own configuration source:

```csharp
services.AddNotificationServices<DatabaseNotificationOptionsProvider>();
```

Your custom provider must implement `INotificationOptionsProvider`.

### Resolve the services

```csharp
using Microsoft.Extensions.DependencyInjection;
using NotificationServices.Email.Abstractions.Interfaces;
using NotificationServices.Sms.Abstractions.Interfaces;

var emailService = serviceProvider.GetRequiredService<IEmailService>();
var smsService = serviceProvider.GetRequiredService<ISmsService>();
```

### Send an Email

```csharp
var result = await emailService.SendMessageAsync(
    new EmailMessage(
        "user@example.com",
        "Welcome",
        "<h1>Welcome to the application.</h1>",
        isHtml: true));
```

### Send an Email OTP

```csharp
var result = await emailService.SendOtpAsync(
    new EmailOtp("user@example.com", "123456"));
```

### Send an SMS

```csharp
var result = await smsService.SendMessageAsync(
    new SmsMessage(
        "09120000000",
        "Your order has been registered."));
```

### Send an SMS OTP

```csharp
var result = await smsService.SendOtpAsync(
    new SmsOtp("09120000000", "123456"));
```

All public send operations support `CancellationToken`.

## Configuration

The built-in `appsettings.json` provider reads the `NotificationServices` section.

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

Never commit real credentials to source control. Use environment variables, .NET User Secrets, deployment secrets, a secret manager, or a custom secure configuration provider.

### Custom configuration source

Applications can replace the built-in configuration source by implementing `INotificationOptionsProvider`.

Conceptually:

```text
Application configuration
(appsettings / database / Redis / API / secrets / custom source)
                          │
                          ▼
              INotificationOptionsProvider
                          │
                          ▼
                NotificationServices
                    ┌─────┴─────┐
                    ▼           ▼
                  Email        SMS
```

Example:

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

        return MapToNotificationOptions(settings);
    }

    private static NotificationOptions MapToNotificationOptions(
        NotificationSettings settings)
    {
        // Map your application's configuration model here.
        throw new NotImplementedException();
    }
}
```

Register the custom provider:

```csharp
services.AddNotificationServices<DatabaseNotificationOptionsProvider>();
```

> The example above illustrates the extension point. The concrete repository and application-specific mapping remain outside this library.

## Email

The Email abstraction provides message and OTP operations.

```csharp
await emailService.SendMessageAsync(
    new EmailMessage(
        "user@example.com",
        "Account activated",
        "<p>Your account is now active.</p>",
        isHtml: true));
```

OTP:

```csharp
await emailService.SendOtpAsync(
    new EmailOtp("user@example.com", "123456"));
```

The implementation uses MailKit to connect to the configured SMTP server and returns an `EmailResult` describing the send outcome.

## SMS

The SMS abstraction keeps the consumer-facing API independent from the underlying gateway.

```csharp
await smsService.SendMessageAsync(
    new SmsMessage(
        "09120000000",
        "Your verification has been completed."));
```

OTP:

```csharp
await smsService.SendOtpAsync(
    new SmsOtp("09120000000", "123456"));
```

## SMS Provider Architecture

The current provider flow is:

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
    ├── Future provider
    └── Custom provider
```

The consumer uses `ISmsService`; provider-specific HTTP details stay behind the provider abstraction.

The repository currently includes a Melipayamak provider implementation.

## Architecture

At a high level:

```text
┌───────────────────────────────────────────────┐
│                  Application                  │
│                                               │
│ appsettings / DB / Redis / API / Secrets      │
└───────────────────────┬───────────────────────┘
                        │
                        ▼
             INotificationOptionsProvider
                        │
                        ▼
               NotificationServices
                    ┌─────┴─────┐
                    ▼           ▼
              Email Service  SMS Service
                    │           │
                    ▼           ▼
                  SMTP     Provider Factory
                                │
                                ▼
                           SMS Provider
```

The main design goal is to keep application-specific infrastructure outside the notification library while keeping Email and SMS implementations replaceable and testable.

## Dependency Injection

The public entry points are:

```csharp
services.AddNotificationServices();
```

and:

```csharp
services.AddNotificationServices<TOptionsProvider>();
```

The registration adds the common services, HTTP client support, Email services, SMS services, provider factory, and configuration adapters.

## Error Handling and Cancellation

The library uses result objects for normal send failures and preserves cancellation semantics by rethrowing `OperationCanceledException`.

A simplified usage pattern is:

```csharp
var result = await emailService.SendMessageAsync(
    message,
    cancellationToken);

if (!result.IsSuccess)
{
    // Handle the failure according to your application policy.
}
```

Application-specific retry, logging, alerting, and persistence policies should remain outside the core library.

## Testing

The repository contains automated tests covering areas such as:

- Dependency injection registration
- Configuration providers and adapters
- Validation
- SMS provider selection
- HTTP request behavior
- OTP behavior
- Failure handling
- Cancellation behavior

Run the full test suite locally:

```bash
dotnet test NotificationServices.slnx
```

## CI/CD

GitHub Actions currently validates the project through a workflow that performs:

```text
Restore
  ↓
Build
  ↓
Tests + Coverage
  ↓
Pack NotificationServices.Kit
  ↓
Verify package contents
  ↓
Upload artifacts
```

The CI workflow is located at:

```text
.github/workflows/ci.yml
```

NuGet publishing is handled by:

```text
.github/workflows/publish.yml
```

The publish workflow uses NuGet trusted publishing with GitHub Actions OIDC rather than storing a long-lived NuGet API key in the repository.

## Packaging

The consumer-facing package is:

```text
NotificationServices.Kit
```

The package is built from:

```bash
dotnet pack src/NotificationServices/NotificationServices.csproj -c Release
```

The CI workflow also verifies that the unified package contains the expected consumer assemblies before publishing artifacts.

## Local Development

Clone the repository:

```bash
git clone https://github.com/Mohammad-Amin-Nazeri/Notification_Services_Pattern.git
cd Notification_Services_Pattern
```

Restore, build, test, and pack:

```bash
dotnet restore NotificationServices.slnx
dotnet build NotificationServices.slnx -c Release
dotnet test NotificationServices.slnx -c Release
dotnet pack src/NotificationServices/NotificationServices.csproj -c Release
```

## Repository Structure

```text
.
├── .github/
│   └── workflows/
│       ├── ci.yml
│       └── publish.yml
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
├── NotificationServices.slnx
├── LICENSE
└── README.md
```

## Roadmap

The project is intentionally evolving in stages. Planned directions include:

- Stronger provider contract tests
- More SMS providers
- Additional notification channels
- Better package/versioning guarantees
- Observability hooks
- Resilience capabilities such as timeout, retry, and failover where appropriate
- Broader production-hardening of public APIs and provider integrations

The goal is to add these capabilities without making the consumer API unnecessarily complex.

## Contributing

Ideas, bug reports, provider integrations, and public API improvements are welcome.

Please open an issue or pull request with a clear description of the problem or proposed change.

Useful contribution areas include:

- New SMS providers
- Email provider improvements
- New notification channels
- Configuration integrations
- Tests
- Documentation
- Reliability and API design improvements

## Support and Contact

**Mohammad Amin Nazeri**

[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/mohammad-amin-nazeri)
[![GitHub](https://img.shields.io/badge/GitHub-181717?style=for-the-badge&logo=github&logoColor=white)](https://github.com/Mohammad-Amin-Nazeri)
[![Telegram](https://img.shields.io/badge/Telegram-2CA5E2?style=for-the-badge&logo=telegram&logoColor=white)](https://t.me/Aminn02)
[![Instagram](https://img.shields.io/badge/Instagram-E4405F?style=for-the-badge&logo=instagram&logoColor=white)](https://www.instagram.com/mohammad_amin_nazeri/)

## License

MIT License. See [LICENSE](LICENSE).

---

<a id="فارسی"></a>

# 🇮🇷 فارسی

<a href="#english">🇬🇧 رفتن به انگلیسی</a>

## معرفی

`NotificationServices` یک کتابخانه قابل استفاده مجدد برای .NET است که ارسال اعلان از طریق Email و SMS را با API ساده، Dependency Injection و معماری Provider-Based فراهم می‌کند.

پروژه بر پایه دو اصل اصلی طراحی شده است:

1. **مصرف‌کننده باید به Abstractionهای پایدار Notification وابسته باشد.**
2. **کتابخانه نباید تصمیم بگیرد Configuration پروژه از کجا ذخیره یا خوانده شود.**

Provider پیش‌فرض Configuration اطلاعات را از `appsettings.json` می‌خواند، اما پروژه مصرف‌کننده می‌تواند `INotificationOptionsProvider` خودش را برای دیتابیس، Redis، API، Environment Variables، Secret Store یا هر منبع دیگری پیاده‌سازی کند.

کتابخانه به EF Core، Dapper، SQL Server، Redis یا هیچ تکنولوژی ذخیره‌سازی خاصی وابسته نیست.

## امکانات

- 📧 ارسال Email از طریق SMTP / MailKit
- 🔐 ارسال OTP با Email
- 📱 ارسال SMS
- 🔐 ارسال OTP با SMS
- 🔌 معماری Provider-Based برای Gatewayهای پیامکی
- ⚙️ منبع Configuration قابل تعویض
- 💉 ثبت ساده با Dependency Injection
- 🧪 تست‌های خودکار
- 📊 Code Coverage در CI
- 🤖 GitHub Actions برای CI/CD
- 📦 یک Package اصلی با نام `NotificationServices.Kit`
- 🛡️ بدون وابستگی به دیتابیس
- 🧩 جداسازی Abstractionهای عمومی از Implementationها
- ⛔ پشتیبانی مناسب از `CancellationToken`

## نصب

Package اصلی را نصب کنید:

```bash
dotnet add package NotificationServices.Kit
```

یا از Package Manager Console:

```powershell
Install-Package NotificationServices.Kit
```

در Repository علاوه بر Package اصلی، پروژه‌های Abstraction و Implementation مربوط به Email و SMS، یک Sample و تست‌های خودکار نیز وجود دارد.

## شروع سریع

### ثبت سرویس‌ها

```csharp
using NotificationServices.DependencyInjection;

services.AddNotificationServices();
```

این ثبت پیش‌فرض از Provider داخلی `appsettings.json` استفاده می‌کند.

برای استفاده از منبع Configuration سفارشی نیز می‌توانید بنویسید:

```csharp
services.AddNotificationServices<DatabaseNotificationOptionsProvider>();
```

کلاس سفارشی شما باید `INotificationOptionsProvider` را پیاده‌سازی کند.

### دریافت سرویس‌ها

```csharp
using Microsoft.Extensions.DependencyInjection;
using NotificationServices.Email.Abstractions.Interfaces;
using NotificationServices.Sms.Abstractions.Interfaces;

var emailService = serviceProvider.GetRequiredService<IEmailService>();
var smsService = serviceProvider.GetRequiredService<ISmsService>();
```

### ارسال Email

```csharp
var result = await emailService.SendMessageAsync(
    new EmailMessage(
        "user@example.com",
        "Welcome",
        "<h1>Welcome to the application.</h1>",
        isHtml: true));
```

### ارسال OTP با Email

```csharp
var result = await emailService.SendOtpAsync(
    new EmailOtp("user@example.com", "123456"));
```

### ارسال SMS

```csharp
var result = await smsService.SendMessageAsync(
    new SmsMessage(
        "09120000000",
        "سفارش شما ثبت شد."));
```

### ارسال OTP با SMS

```csharp
var result = await smsService.SendOtpAsync(
    new SmsOtp("09120000000", "123456"));
```

تمام عملیات ارسال عمومی از `CancellationToken` پشتیبانی می‌کنند.

## Configuration

Provider داخلی Configuration بخش `NotificationServices` را از `appsettings.json` می‌خواند.

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

هرگز اطلاعات واقعی مانند Username، Password، API Key یا Secret را داخل Repository قرار ندهید. برای این اطلاعات از Environment Variables، .NET User Secrets، Secret Manager، Deployment Secrets یا Provider امن سفارشی استفاده کنید.

### منبع Configuration سفارشی

پروژه مصرف‌کننده می‌تواند Provider داخلی را با پیاده‌سازی `INotificationOptionsProvider` جایگزین کند.

به‌صورت مفهومی:

```text
Configuration پروژه
(appsettings / database / Redis / API / secrets / منبع سفارشی)
                              │
                              ▼
                  INotificationOptionsProvider
                              │
                              ▼
                    NotificationServices
                         ┌─────┴─────┐
                         ▼           ▼
                       Email        SMS
```

نمونه:

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

        return MapToNotificationOptions(settings);
    }

    private static NotificationOptions MapToNotificationOptions(
        NotificationSettings settings)
    {
        // Mapping مخصوص پروژه مصرف‌کننده را اینجا انجام دهید.
        throw new NotImplementedException();
    }
}
```

ثبت Provider سفارشی:

```csharp
services.AddNotificationServices<DatabaseNotificationOptionsProvider>();
```

> این نمونه فقط Extension Point را نشان می‌دهد. Repository و مدل Configuration مربوط به دیتابیس باید در پروژه مصرف‌کننده پیاده‌سازی شوند و بخشی از این Library نیستند.

## Email

Abstraction مربوط به Email عملیات ارسال پیام و OTP را ارائه می‌دهد.

```csharp
await emailService.SendMessageAsync(
    new EmailMessage(
        "user@example.com",
        "فعال شدن حساب",
        "<p>حساب شما فعال شد.</p>",
        isHtml: true));
```

OTP:

```csharp
await emailService.SendOtpAsync(
    new EmailOtp("user@example.com", "123456"));
```

Implementation مربوط به Email از MailKit برای اتصال به SMTP استفاده می‌کند و نتیجه ارسال را در قالب `EmailResult` برمی‌گرداند.

## SMS

Abstraction مربوط به SMS باعث می‌شود API مصرف‌کننده مستقل از Gateway پیامکی باقی بماند.

```csharp
await smsService.SendMessageAsync(
    new SmsMessage(
        "09120000000",
        "کد تأیید شما با موفقیت ارسال شد."));
```

OTP:

```csharp
await smsService.SendOtpAsync(
    new SmsOtp("09120000000", "123456"));
```

## معماری Provider پیامک

جریان فعلی Providerها به شکل زیر است:

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
    ├── Providerهای آینده
    └── Provider سفارشی
```

مصرف‌کننده فقط با `ISmsService` کار می‌کند و جزئیات HTTP و Gateway پشت Abstraction مربوط به Provider قرار دارند.

در حال حاضر Repository شامل Implementation مربوط به Melipayamak است.

## معماری کلی

```text
┌───────────────────────────────────────────────┐
│                   Application                │
│                                               │
│ appsettings / DB / Redis / API / Secrets      │
└───────────────────────┬───────────────────────┘
                        │
                        ▼
             INotificationOptionsProvider
                        │
                        ▼
               NotificationServices
                    ┌─────┴─────┐
                    ▼           ▼
              Email Service  SMS Service
                    │           │
                    ▼           ▼
                  SMTP     Provider Factory
                                │
                                ▼
                           SMS Provider
```

هدف اصلی معماری این است که Infrastructure وابسته به پروژه مصرف‌کننده داخل خود Library قرار نگیرد و Implementationهای Email و SMS قابل تعویض و قابل تست باقی بمانند.

## Dependency Injection

دو Entry Point اصلی برای ثبت سرویس‌ها وجود دارد:

```csharp
services.AddNotificationServices();
```

و:

```csharp
services.AddNotificationServices<TOptionsProvider>();
```

ثبت سرویس‌ها شامل Configuration، پشتیبانی HTTP Client، Email، SMS، Provider Factory و Adapterهای مربوط به Configuration است.

## مدیریت خطا و Cancellation

کتابخانه برای خطاهای عادی ارسال از Result Objectها استفاده می‌کند و Cancellation را با حفظ `OperationCanceledException` مدیریت می‌کند.

نمونه:

```csharp
var result = await emailService.SendMessageAsync(
    message,
    cancellationToken);

if (!result.IsSuccess)
{
    // بر اساس Policy پروژه خود خطا را مدیریت کنید.
}
```

تصمیم‌های مربوط به Retry، Logging، Alerting و ذخیره وضعیت باید در سطح پروژه مصرف‌کننده یا Infrastructure بالاتر انجام شوند.

## تست‌ها

Repository شامل تست‌های خودکار برای بخش‌هایی مانند موارد زیر است:

- ثبت Dependency Injection
- Configuration Providerها و Adapterها
- Validation
- انتخاب SMS Provider
- رفتار HTTP Requestها
- OTP
- مدیریت Failure
- Cancellation

اجرای تمام تست‌ها:

```bash
dotnet test NotificationServices.slnx
```

## CI/CD

GitHub Actions در حال حاضر این مراحل را اجرا می‌کند:

```text
Restore
  ↓
Build
  ↓
Tests + Coverage
  ↓
Pack NotificationServices.Kit
  ↓
Verify package contents
  ↓
Upload artifacts
```

Workflow مربوط به CI:

```text
.github/workflows/ci.yml
```

Workflow انتشار NuGet:

```text
.github/workflows/publish.yml
```

فرآیند انتشار از Trusted Publishing و GitHub Actions OIDC استفاده می‌کند و به یک API Key بلندمدت NuGet در Repository وابسته نیست.

## Packaging

Package اصلی برای مصرف‌کننده:

```text
NotificationServices.Kit
```

ساخت Package:

```bash
dotnet pack src/NotificationServices/NotificationServices.csproj -c Release
```

CI نیز قبل از انتشار Artifact بررسی می‌کند که Assemblyهای مورد انتظار داخل Package قرار گرفته باشند.

## توسعه محلی

Repository را Clone کنید:

```bash
git clone https://github.com/Mohammad-Amin-Nazeri/Notification_Services_Pattern.git
cd Notification_Services_Pattern
```

سپس Restore، Build، Test و Pack را اجرا کنید:

```bash
dotnet restore NotificationServices.slnx
dotnet build NotificationServices.slnx -c Release
dotnet test NotificationServices.slnx -c Release
dotnet pack src/NotificationServices/NotificationServices.csproj -c Release
```

## ساختار Repository

```text
.
├── .github/
│   └── workflows/
│       ├── ci.yml
│       └── publish.yml
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
├── NotificationServices.slnx
├── LICENSE
└── README.md
```

## مسیر توسعه آینده

پروژه به‌صورت مرحله‌ای توسعه پیدا می‌کند. مسیرهای پیشنهادی آینده شامل موارد زیر هستند:

- تست‌های Contract قوی‌تر برای Providerها
- SMS Providerهای بیشتر
- اضافه شدن کانال‌های Notification جدید
- بهبود Package و Versioning
- Hookهای مربوط به Observability
- قابلیت‌های Resilience مانند Timeout، Retry و Failover در موارد مناسب
- Hardening بیشتر برای Public API و Provider Integrationها

هدف این است که قابلیت‌های جدید بدون پیچیده‌کردن غیرضروری API مصرف‌کننده اضافه شوند.

## مشارکت

ایده‌ها، گزارش Bug، Providerهای جدید و پیشنهادهای مرتبط با Public API قابل استقبال هستند.

برای مشارکت، یک Issue یا Pull Request با توضیح روشن درباره مشکل یا پیشنهاد خود ایجاد کنید.

زمینه‌های مناسب برای مشارکت:

- SMS Providerهای جدید
- بهبود Email Provider
- کانال‌های Notification جدید
- Configuration Integrationها
- تست‌ها
- مستندات
- بهبود Reliability و API Design

## پشتیبانی و ارتباط

**Mohammad Amin Nazeri**

[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/mohammad-amin-nazeri)
[![GitHub](https://img.shields.io/badge/GitHub-181717?style=for-the-badge&logo=github&logoColor=white)](https://github.com/Mohammad-Amin-Nazeri)
[![Telegram](https://img.shields.io/badge/Telegram-2CA5E2?style=for-the-badge&logo=telegram&logoColor=white)](https://t.me/Aminn02)
[![Instagram](https://img.shields.io/badge/Instagram-E4405F?style=for-the-badge&logo=instagram&logoColor=white)](https://www.instagram.com/mohammad_amin_nazeri/)

## مجوز

این پروژه تحت مجوز MIT منتشر شده است. جزئیات در فایل [LICENSE](LICENSE) قرار دارد.

---

<div align="center">

**Notification Services Pattern · Email + SMS infrastructure for .NET**

</div>
