<div align="center">

# Notification Services Pattern

**A clean, reusable, and extensible .NET notification infrastructure for Email and SMS.**

[![CI](https://github.com/Mohammad-Amin-Nazeri/Notification_Services_Pattern/actions/workflows/ci.yml/badge.svg)](https://github.com/Mohammad-Amin-Nazeri/Notification_Services_Pattern/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-13.0-239120?logo=csharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)

<a href="#english"><strong>🇬🇧 English</strong></a>
&nbsp;&nbsp;•&nbsp;&nbsp;
<a href="#فارسی"><strong>🇮🇷 فارسی</strong></a>

</div>

---

<a id="english"></a>

# 🇬🇧 English

<a href="#فارسی">🇮🇷 رفتن به فارسی</a>

## 📌 What is Notification Services Pattern?

`NotificationServices` is a reusable .NET library for sending **Email** and **SMS** notifications through a small, dependency-injection-friendly API.

The project is designed around one important rule:

> **The notification library should not decide where your configuration comes from. The consuming application decides.**

You can keep configuration in `appsettings.json`, load it from a database, read it from environment variables, use a secret store, call a remote configuration API, or provide your own implementation.

The library itself does not depend on EF Core, Dapper, SQL Server, Redis, or any specific persistence system.

### What you get

- 📧 Email messages
- 🔐 Email OTP messages
- 📱 SMS messages
- 🔐 SMS OTP messages
- 🔌 Extensible SMS provider architecture
- ⚙️ Replaceable configuration source
- 💉 Simple dependency injection registration
- 🧪 Automated unit tests
- 🤖 GitHub Actions CI
- 📦 A single consumer-facing NuGet package
- 🛡️ No database-specific coupling
- 🧩 Clear separation between application infrastructure and notification infrastructure

## 🎯 Design Goals

This project is intentionally built to be useful in real applications, not just to demonstrate a pattern.

### Simple for the consumer

```bash
dotnet add package NotificationServices.Kit
```

```csharp
services.AddNotificationServices();
```

### Flexible for configuration

```text
appsettings.json / Database / Redis / API / Secrets / Custom Source
                              │
                              ▼
                INotificationOptionsProvider
                              │
                              ▼
                    NotificationServices
```

### Extensible for providers

SMS gateways are isolated behind provider abstractions. Adding a new gateway should not require changing the main `SmsService` behavior.

---

## 📦 Installation

```bash
dotnet add package NotificationServices.Kit
```

Or from Visual Studio Package Manager Console:

```powershell
Install-Package NotificationServices.Kit
```

The intended consumer experience is a **single package**. Consumers do not need to install separate Email and SMS packages.

---

## 🚀 Quick Start

### Register Notification Services

```csharp
using NotificationServices.DependencyInjection;

services.AddNotificationServices();
```

### Resolve the services

```csharp
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
    new SmsMessage("09120000000", "Your order has been registered."));
```

### Send an SMS OTP

```csharp
var result = await smsService.SendOtpAsync(
    new SmsOtp("09120000000", "123456"));
```

---

## ⚙️ Default Configuration with appsettings.json

The built-in configuration provider reads:

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

Never commit real credentials. Use Environment Variables, .NET User Secrets, a secret store, deployment secrets, or a custom secure configuration provider.

---

## 🔌 Custom Configuration Source

The package intentionally does **not** contain a Database/EF Core/Dapper implementation. The consuming application owns that infrastructure.

The contract is:

```csharp
public interface INotificationOptionsProvider
{
    ValueTask<NotificationOptions> GetOptionsAsync(
        CancellationToken cancellationToken = default);
}
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

Register it:

```csharp
services.AddNotificationServices<DatabaseNotificationOptionsProvider>();
```

The same pattern works for Redis, APIs, secret stores, or any application-specific source.

---

## 📧 Email API

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

The service returns an `EmailResult` for success/failure handling.

---

## 📱 SMS API

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

The consumer API remains the same regardless of the underlying SMS gateway.

---

## 🔌 SMS Provider Architecture

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
     ├── MelipayamakSmsProvider
     ├── Future Provider
     └── Your Custom Provider
```

The repository currently includes a Melipayamak implementation. New providers can be introduced behind the same abstraction.

---

## 🏗️ Architecture Overview

```text
┌─────────────────────────────────────────────┐
│                 Application                 │
│                                             │
│ appsettings / DB / Redis / API / Secrets   │
└──────────────────────┬──────────────────────┘
                       │
                       ▼
          INotificationOptionsProvider
                       │
                       ▼
             NotificationOptions
                 │           │
                 ▼           ▼
          Email Service   SMS Service
                 │           │
                 │           ▼
                 │     ISmsProviderFactory
                 │           │
                 │           ▼
                 │       ISmsProvider
                 │
                 ▼
             SMTP / MailKit
```

The core package knows the configuration contract, not the infrastructure behind it.

---

## 🧪 Testing & CI

The repository contains automated tests for registration, configuration providers, validation, provider selection, HTTP requests, OTP behavior, failures, and cancellation.

Run all tests:

```bash
dotnet test NotificationServices.slnx
```

GitHub Actions validates:

```text
Restore → Build → Tests + Coverage → Pack → Verify Package → Upload Artifacts
```

Workflow:

```text
.github/workflows/ci.yml
```

---

## 📦 Packaging

The consumer-facing package is:

```text
NotificationServices.Kit
```

Build locally:

```bash
dotnet pack src/NotificationServices/NotificationServices.csproj -c Release
```

Email and SMS implementations are distributed through the unified package rather than requiring separate consumer packages.

---

## 🧰 Local Development

```bash
git clone https://github.com/Mohammad-Amin-Nazeri/Notification_Services_Pattern.git
cd Notification_Services_Pattern

dotnet restore NotificationServices.slnx
dotnet build NotificationServices.slnx
dotnet test NotificationServices.slnx
dotnet pack src/NotificationServices/NotificationServices.csproj -c Release
```

---

## 🤝 Suggestions, New Services & Contact

Have an idea for a new notification service, SMS gateway, Email provider, integration, or improvement?

**You do not need to prepare a Pull Request or figure out the repository workflow first. Just contact the developer directly and share the idea.**

Useful examples include:

- WhatsApp
- Telegram
- Push Notifications
- Microsoft Teams
- Discord
- New SMS gateways
- New Email providers
- New configuration integrations
- Improvements to the public API
- Features that would make the library more useful in real projects

### 📬 Contact the developer

**Mohammad Amin Nazeri**

[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/mohammad-amin-nazeri)
[![GitHub](https://img.shields.io/badge/GitHub-181717?style=for-the-badge&logo=github&logoColor=white)](https://github.com/Mohammad-Amin-Nazeri)
[![Telegram](https://img.shields.io/badge/Telegram-2CA5E2?style=for-the-badge&logo=telegram&logoColor=white)](https://t.me/Aminn02)
[![Instagram](https://img.shields.io/badge/Instagram-E4405F?style=for-the-badge&logo=instagram&logoColor=white)](https://www.instagram.com/mohammad_amin_nazeri/)

You can also reach the developer through the contact links above to discuss a feature, suggest a service, report an issue, or propose an improvement.

---

## ⭐ Support the Project

If `NotificationServices.Kit` is useful to you, please consider giving the repository a **⭐ Star**.

A Star helps more developers discover the project and supports continued development.

👉 **[⭐ Star Notification Services Pattern on GitHub](https://github.com/Mohammad-Amin-Nazeri/Notification_Services_Pattern)**

---

## 📄 License

MIT License. See [LICENSE](LICENSE).

---

<a id="فارسی"></a>

# 🇮🇷 فارسی

<a href="#english">🇬🇧 رفتن به انگلیسی</a>

## 📌 Notification Services Pattern چیست؟

`NotificationServices` یک کتابخانه قابل استفاده مجدد برای پروژه‌های .NET است که ارسال **ایمیل** و **پیامک** را با API ساده، Dependency Injection و معماری قابل توسعه فراهم می‌کند.

اصل مهم معماری پروژه:

> **کتابخانه نباید تصمیم بگیرد تنظیمات از کجا خوانده شوند؛ انتخاب منبع Configuration بر عهده پروژه مصرف‌کننده است.**

بنابراین می‌توانید Configuration را از `appsettings.json`، دیتابیس، Environment Variables، Redis، Secret Store، API یا هر منبع دلخواه دیگری دریافت کنید.

کتابخانه به EF Core، Dapper، SQL Server، Redis یا هیچ سیستم ذخیره‌سازی خاصی وابسته نیست.

### امکانات

- 📧 ارسال ایمیل
- 🔐 ارسال OTP با ایمیل
- 📱 ارسال پیامک
- 🔐 ارسال OTP با پیامک
- 🔌 معماری Provider-Based برای Gatewayهای پیامکی
- ⚙️ Configuration قابل تعویض
- 💉 ثبت ساده با Dependency Injection
- 🧪 تست‌های خودکار
- 🤖 GitHub Actions و CI
- 📦 یک Package اصلی برای مصرف‌کننده
- 🛡️ بدون وابستگی به دیتابیس
- 🧩 جداسازی Infrastructure از Notification Service

## 🎯 اهداف طراحی

این پروژه برای استفاده واقعی طراحی شده است، نه فقط نمایش یک Pattern.

### ساده برای استفاده

```bash
dotnet add package NotificationServices.Kit
```

```csharp
services.AddNotificationServices();
```

### منعطف برای Configuration

```text
appsettings / Database / Redis / API / Secrets / منبع سفارشی
                              │
                              ▼
                INotificationOptionsProvider
                              │
                              ▼
                    NotificationServices
```

### قابل توسعه برای Providerها

جزئیات Gateway پیامکی پشت abstraction قرار دارند تا اضافه کردن Provider جدید به تغییر سرویس اصلی نیاز نداشته باشد.

---

## 📦 نصب

```bash
dotnet add package NotificationServices.Kit
```

یا:

```powershell
Install-Package NotificationServices.Kit
```

هدف پروژه این است که کاربر **یک Package** نصب کند و Email و SMS را در اختیار داشته باشد.

---

## 🚀 شروع سریع

### ثبت سرویس‌ها

```csharp
using NotificationServices.DependencyInjection;

services.AddNotificationServices();
```

### دریافت سرویس‌ها

```csharp
using NotificationServices.Email.Abstractions.Interfaces;
using NotificationServices.Sms.Abstractions.Interfaces;

var emailService = serviceProvider.GetRequiredService<IEmailService>();
var smsService = serviceProvider.GetRequiredService<ISmsService>();
```

### ارسال ایمیل

```csharp
await emailService.SendMessageAsync(
    new EmailMessage(
        "user@example.com",
        "خوش آمدید",
        "<h1>به برنامه خوش آمدید.</h1>",
        isHtml: true));
```

### ارسال OTP ایمیل

```csharp
await emailService.SendOtpAsync(
    new EmailOtp("user@example.com", "123456"));
```

### ارسال پیامک

```csharp
await smsService.SendMessageAsync(
    new SmsMessage("09120000000", "سفارش شما با موفقیت ثبت شد."));
```

### ارسال OTP پیامکی

```csharp
await smsService.SendOtpAsync(
    new SmsOtp("09120000000", "123456"));
```

---

## ⚙️ تنظیمات appsettings.json

Provider پیش‌فرض از بخش زیر می‌خواند:

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

رمز عبور، API Key و Secret واقعی را داخل Git Commit نکنید. از Environment Variables، User Secrets، Secret Store یا Provider امن خودتان استفاده کنید.

---

## 🔌 Configuration سفارشی

کتابخانه خودش Database Provider ندارد؛ چون دیتابیس و Infrastructure متعلق به پروژه مصرف‌کننده است.

قرارداد اصلی:

```csharp
public interface INotificationOptionsProvider
{
    ValueTask<NotificationOptions> GetOptionsAsync(
        CancellationToken cancellationToken = default);
}
```

نمونه Provider دیتابیس:

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

ثبت:

```csharp
services.AddNotificationServices<DatabaseNotificationOptionsProvider>();
```

همین الگو برای Redis، API، Secret Store یا هر Provider اختصاصی دیگر قابل استفاده است.

---

## 📧 Email

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

نتیجه عملیات از طریق `EmailResult` برگردانده می‌شود.

---

## 📱 SMS

```csharp
await smsService.SendMessageAsync(
    new SmsMessage(
        "09120000000",
        "عملیات شما با موفقیت انجام شد."));
```

OTP:

```csharp
await smsService.SendOtpAsync(
    new SmsOtp("09120000000", "123456"));
```

Application از API یکسان استفاده می‌کند و نیازی نیست بداند کدام Gateway پشت سرویس قرار دارد.

---

## 🔌 معماری Provider پیامک

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
     ├── MelipayamakSmsProvider
     ├── Provider آینده
     └── Provider اختصاصی شما
```

در حال حاضر Provider مربوط به Melipayamak در پروژه وجود دارد.

---

## 🏗️ نمای کلی معماری

```text
┌─────────────────────────────────────────────┐
│                 Application                 │
│                                             │
│ appsettings / DB / Redis / API / Secrets   │
└──────────────────────┬──────────────────────┘
                       │
                       ▼
          INotificationOptionsProvider
                       │
                       ▼
             NotificationOptions
                 │           │
                 ▼           ▼
           Email Service   SMS Service
                 │           │
                 │           ▼
                 │     ISmsProviderFactory
                 │           │
                 │           ▼
                 │       ISmsProvider
                 │
                 ▼
             SMTP / MailKit
```

کتابخانه فقط Contractها و Notification Infrastructure را می‌شناسد و درباره منبع Configuration تصمیم نمی‌گیرد.

---

## 🧪 تست و CI

تست‌های خودکار بخش‌های مهم مانند DI، Configuration Providerها، Validation، انتخاب Provider، HTTP، OTP، خطاها و Cancellation را پوشش می‌دهند.

اجرای تست‌ها:

```bash
dotnet test NotificationServices.slnx
```

GitHub Actions این مراحل را بررسی می‌کند:

```text
Restore → Build → Tests + Coverage → Pack → Verify Package → Upload Artifacts
```

Workflow:

```text
.github/workflows/ci.yml
```

---

## 📦 ساخت Package

Package اصلی:

```text
NotificationServices.Kit
```

```bash
dotnet pack src/NotificationServices/NotificationServices.csproj -c Release
```

Email و SMS از طریق همین Package در اختیار مصرف‌کننده قرار می‌گیرند.

---

## 🧰 اجرای پروژه در حالت Development

```bash
git clone https://github.com/Mohammad-Amin-Nazeri/Notification_Services_Pattern.git
cd Notification_Services_Pattern

dotnet restore NotificationServices.slnx
dotnet build NotificationServices.slnx
dotnet test NotificationServices.slnx
dotnet pack src/NotificationServices/NotificationServices.csproj -c Release
```

---

## 🤝 پیشنهاد سرویس، ایده یا بهبود

اگر برای پروژه **سرویس جدید، SMS Gateway جدید، Email Provider جدید، Integration، قابلیت جدید یا پیشنهادی برای بهتر شدن Library** دارید، خوشحال می‌شوم مستقیماً با من در ارتباط باشید.

**لازم نیست خودتان Pull Request بسازید یا درگیر Workflow پروژه شوید.** ایده یا نیازتان را با من مطرح کنید تا بررسی کنیم و درباره بهترین روش اضافه شدن آن به پروژه تصمیم بگیریم.

نمونه ایده‌ها:

- WhatsApp
- Telegram
- Push Notification
- Microsoft Teams
- Discord
- SMS Gatewayهای جدید
- Email Providerهای جدید
- Integrationهای جدید
- Providerهای Configuration
- بهبود API عمومی
- قابلیت‌هایی برای استفاده ساده‌تر در پروژه‌های واقعی

### 📬 ارتباط با توسعه‌دهنده

**محمد امین ناظری | Mohammad Amin Nazeri**

[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/mohammad-amin-nazeri)
[![GitHub](https://img.shields.io/badge/GitHub-181717?style=for-the-badge&logo=github&logoColor=white)](https://github.com/Mohammad-Amin-Nazeri)
[![Telegram](https://img.shields.io/badge/Telegram-2CA5E2?style=for-the-badge&logo=telegram&logoColor=white)](https://t.me/Aminn02)
[![Instagram](https://img.shields.io/badge/Instagram-E4405F?style=for-the-badge&logo=instagram&logoColor=white)](https://www.instagram.com/mohammad_amin_nazeri/)

از طریق لینک‌های بالا می‌توانید برای پیشنهاد سرویس، ایده، گزارش مشکل، پیشنهاد بهبود یا همکاری با توسعه‌دهنده ارتباط بگیرید.

---

## ⭐ حمایت از پروژه

اگر `NotificationServices.Kit` برایتان مفید است، لطفاً Repository را ⭐ **Star** کنید.

Star کردن پروژه باعث می‌شود افراد بیشتری آن را ببینند و به ادامه توسعه و نگهداری آن کمک می‌کند.

👉 **[⭐ Star کردن Notification Services Pattern در GitHub](https://github.com/Mohammad-Amin-Nazeri/Notification_Services_Pattern)**

---

## 📄 لایسنس

این پروژه تحت **MIT License** منتشر شده است.

---

<div align="center">

### ⭐ اگر پروژه برایتان مفید بود، یک Star بدهید

[🇬🇧 English](#english)
&nbsp;&nbsp;•&nbsp;&nbsp;
[🇮🇷 فارسی](#فارسی)

</div>
