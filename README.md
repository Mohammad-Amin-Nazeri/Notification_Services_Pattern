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

---

## 🎯 Design Goals

This project is intentionally built to be useful in real applications, not just to demonstrate a pattern.

### Simple for the consumer

Install one package and register one service collection extension:

```bash
dotnet add package NotificationServices
```

```csharp
services.AddNotificationServices();
```

### Flexible for configuration

The library exposes a single configuration contract:

```csharp
INotificationOptionsProvider
```

The source can be anything:

```text
appsettings.json
      │
Database
      │
Environment Variables
      │
Redis / Cache
      │
Remote API
      │
Secret Store
      │
Custom Source
      ▼
INotificationOptionsProvider
      ▼
NotificationServices
```

### Extensible for providers

SMS gateways are isolated behind provider abstractions. Adding a new gateway should not require changing the main `SmsService` behavior.

---

## 📦 Installation

Install the single package:

```bash
dotnet add package NotificationServices
```

Or from Visual Studio Package Manager Console:

```powershell
Install-Package NotificationServices
```

The intended consumer experience is a **single package**. Consumers do not need to install separate Email and SMS packages.

---

## 🚀 Quick Start

### 1. Register Notification Services

```csharp
using NotificationServices.DependencyInjection;

var services = new ServiceCollection();

services.AddNotificationServices();
```

When the default registration is used, the built-in configuration provider reads notification settings from `IConfiguration`.

### 2. Resolve the services

```csharp
using NotificationServices.Email.Abstractions.Interfaces;
using NotificationServices.Sms.Abstractions.Interfaces;

var provider = services.BuildServiceProvider();

var emailService = provider.GetRequiredService<IEmailService>();
var smsService = provider.GetRequiredService<ISmsService>();
```

### 3. Send an Email

```csharp
var result = await emailService.SendMessageAsync(
    new EmailMessage(
        "user@example.com",
        "Welcome",
        "<h1>Welcome to the application.</h1>",
        isHtml: true));
```

### 4. Send an Email OTP

```csharp
var result = await emailService.SendOtpAsync(
    new EmailOtp(
        "user@example.com",
        "123456"));
```

### 5. Send an SMS

```csharp
var result = await smsService.SendMessageAsync(
    new SmsMessage(
        "09120000000",
        "Your order has been registered."));
```

### 6. Send an SMS OTP

```csharp
var result = await smsService.SendOtpAsync(
    new SmsOtp(
        "09120000000",
        "123456"));
```

---

## ⚙️ Default Configuration with appsettings.json

The built-in configuration provider reads this section:

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

Make sure the application has an `IConfiguration` registered when using the default configuration provider.

### Security

Never commit real credentials to source control.

Use one of the following instead:

- Environment variables
- .NET User Secrets
- Azure Key Vault or another secret store
- Container / deployment secrets
- Your own secure configuration provider

---

## 🔌 Custom Configuration Source

This is one of the main architectural features of the project.

The package does **not** contain a database implementation because the consuming application owns that concern.

The contract is:

```csharp
public interface INotificationOptionsProvider
{
    ValueTask<NotificationOptions> GetOptionsAsync(
        CancellationToken cancellationToken = default);
}
```

### Example: Database-backed configuration

Your application can implement the interface using its own repository, EF Core, Dapper, or anything else:

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

Register it through DI:

```csharp
services.AddNotificationServices<DatabaseNotificationOptionsProvider>();
```

The exact same approach works with another source:

```csharp
services.AddNotificationServices<RedisNotificationOptionsProvider>();
```

or:

```csharp
services.AddNotificationServices<ApiNotificationOptionsProvider>();
```

or:

```csharp
services.AddNotificationServices<MyCompanyNotificationOptionsProvider>();
```

The library does not need to know what is behind the implementation.

---

## 📧 Email API

### General Email

```csharp
var result = await emailService.SendMessageAsync(
    new EmailMessage(
        "user@example.com",
        "Account activated",
        "<p>Your account is now active.</p>",
        isHtml: true));
```

### OTP Email

```csharp
var result = await emailService.SendOtpAsync(
    new EmailOtp(
        "user@example.com",
        "123456"));
```

The service returns an `EmailResult`, allowing the application to inspect success or failure without coupling itself to the underlying SMTP implementation.

---

## 📱 SMS API

### General SMS

```csharp
var result = await smsService.SendMessageAsync(
    new SmsMessage(
        "09120000000",
        "Your verification has been completed."));
```

### OTP SMS

```csharp
var result = await smsService.SendOtpAsync(
    new SmsOtp(
        "09120000000",
        "123456"));
```

The same simple API is used regardless of which SMS provider is configured underneath.

---

## 🔌 SMS Provider Architecture

SMS delivery is provider-based:

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
     ├── FutureProviderA
     ├── FutureProviderB
     └── YourCustomProvider
```

This keeps gateway-specific HTTP/API details outside the consumer-facing service API.

### Current provider

The repository currently includes a Melipayamak implementation.

### Adding a new provider

A provider should implement the existing `ISmsProvider` abstraction and then be registered in the provider-selection layer.

The goal is to make future providers additive instead of forcing changes through the entire notification stack.

---

## 🏗️ Architecture Overview

```text
┌─────────────────────────────────────────────┐
│                 Application                 │
│                                             │
│  appsettings / DB / Redis / API / Secrets   │
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

### Main boundaries

**Configuration boundary**

`INotificationOptionsProvider` isolates the notification infrastructure from the configuration source.

**Email boundary**

`IEmailService` exposes application-facing email operations.

**SMS boundary**

`ISmsService` exposes application-facing SMS operations.

**Provider boundary**

`ISmsProvider` isolates provider-specific SMS gateway logic.

**Dependency Injection boundary**

`AddNotificationServices()` is the main registration entry point for consumers.

---

## 🧪 Testing

The repository contains automated tests covering important behavior such as:

- Dependency injection registration
- Default configuration provider
- Custom configuration provider
- Email validation and result handling
- SMS validation
- SMS provider selection
- HTTP request construction
- OTP behavior
- Provider failures
- Cancellation behavior

Run the full test suite:

```bash
dotnet test NotificationServices.slnx
```

### CI

GitHub Actions validates the project automatically by running:

```text
Restore
   ↓
Build
   ↓
Tests + Coverage
   ↓
Pack
   ↓
Verify Package
   ↓
Upload Artifacts
```

Workflow:

```text
.github/workflows/ci.yml
```

---

## 📦 Packaging

The consumer-facing package is:

```text
NotificationServices
```

Build it locally with:

```bash
dotnet pack src/NotificationServices/NotificationServices.csproj -c Release
```

The project is intentionally structured so Email and SMS implementation assemblies are distributed through the unified package rather than requiring consumers to install separate packages.

---

## 🧰 Local Development

Clone the repository:

```bash
git clone https://github.com/Mohammad-Amin-Nazeri/Notification_Services_Pattern.git
cd Notification_Services_Pattern
```

Restore:

```bash
dotnet restore NotificationServices.slnx
```

Build:

```bash
dotnet build NotificationServices.slnx
```

Test:

```bash
dotnet test NotificationServices.slnx
```

Pack:

```bash
dotnet pack src/NotificationServices/NotificationServices.csproj -c Release
```

---

## 🤝 Contributing

Contributions are welcome.

### Add a new SMS provider

If you want to add another SMS gateway:

1. Implement `ISmsProvider`.
2. Add provider-specific configuration requirements.
3. Update provider selection.
4. Add unit tests.
5. Update the documentation.
6. Open a Pull Request.

### Suggest a new notification service

You can also suggest an entirely new channel, such as:

- WhatsApp
- Telegram
- Push Notifications
- Microsoft Teams
- Discord
- Additional SMS gateways
- Additional Email providers
- Other messaging channels

Open a GitHub Issue and describe the use case, expected API, provider requirements, and why the integration would be useful.

---

## ⭐ Support the Project

If this project is useful to you, please consider giving the repository a **⭐ Star** on GitHub.

A star helps the project gain visibility, helps other developers discover it, and is a very low-cost way to say that the project was useful. Humanity has built entire recommendation systems around fewer meaningful signals.

👉 **[⭐ Star Notification Services Pattern on GitHub](https://github.com/Mohammad-Amin-Nazeri/Notification_Services_Pattern)**

---

## 👨‍💻 Developer

### Mohammad Amin Nazeri

Developer and maintainer of **Notification Services Pattern**.

<a href="https://github.com/Mohammad-Amin-Nazeri">Mohammad Amin Nazeri</a>

[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/mohammad-amin-nazeri)
[![GitHub](https://img.shields.io/badge/GitHub-181717?style=for-the-badge&logo=github&logoColor=white)](https://github.com/Mohammad-Amin-Nazeri)
[![Telegram](https://img.shields.io/badge/Telegram-2CA5E2?style=for-the-badge&logo=telegram&logoColor=white)](https://t.me/Aminn02)
[![Instagram](https://img.shields.io/badge/Instagram-E4405F?style=for-the-badge&logo=instagram&logoColor=white)](https://www.instagram.com/mohammad_amin_nazeri/)

---

## 📄 License

This project is licensed under the **MIT License**.

See [LICENSE](LICENSE) for details.

---

<a id="فارسی"></a>

# 🇮🇷 فارسی

<a href="#english">🇬🇧 رفتن به انگلیسی</a>

## 📌 Notification Services Pattern چیست؟

`NotificationServices` یک کتابخانه قابل استفاده مجدد برای پروژه‌های .NET است که ارسال **ایمیل** و **پیامک** را با API ساده، Dependency Injection و معماری قابل توسعه فراهم می‌کند.

اصل مهم معماری پروژه این است:

> **کتابخانه نباید تصمیم بگیرد تنظیمات از کجا خوانده شوند. انتخاب منبع Configuration بر عهده پروژه مصرف‌کننده است.**

بنابراین می‌توانید تنظیمات را از `appsettings.json` بخوانید یا آن را از دیتابیس، Environment Variables، Redis، Secret Store، API یا هر منبع دلخواه دیگری دریافت کنید.

کتابخانه به EF Core، Dapper، SQL Server، Redis یا هیچ سیستم ذخیره‌سازی خاصی وابسته نیست.

### امکانات

- 📧 ارسال ایمیل عادی
- 🔐 ارسال OTP با ایمیل
- 📱 ارسال پیامک عادی
- 🔐 ارسال OTP با پیامک
- 🔌 معماری Provider-Based برای سرویس‌های پیامکی
- ⚙️ منبع Configuration قابل تعویض
- 💉 ثبت ساده سرویس‌ها با Dependency Injection
- 🧪 تست‌های خودکار
- 🤖 GitHub Actions و CI
- 📦 یک Package اصلی برای مصرف‌کننده
- 🛡️ بدون وابستگی به دیتابیس
- 🧩 جداسازی مناسب Infrastructure از Application

---

## 🎯 اهداف طراحی پروژه

هدف این پروژه صرفاً نمایش Pattern نیست؛ قرار است بتوان از آن در پروژه‌های واقعی استفاده کرد.

### ساده برای استفاده

فقط یک Package نصب کنید:

```bash
dotnet add package NotificationServices
```

و سرویس را ثبت کنید:

```csharp
services.AddNotificationServices();
```

### انعطاف‌پذیر در Configuration

قرارداد اصلی Configuration این است:

```csharp
INotificationOptionsProvider
```

منبع می‌تواند هر چیزی باشد:

```text
appsettings.json
      │
Database
      │
Environment Variables
      │
Redis / Cache
      │
Remote API
      │
Secret Store
      │
منبع سفارشی
      ▼
INotificationOptionsProvider
      ▼
NotificationServices
```

### قابل توسعه برای Providerها

جزئیات سرویس‌دهنده‌های پیامکی پشت abstraction قرار گرفته‌اند تا اضافه کردن Provider جدید نیازمند تغییر در سرویس اصلی SMS نباشد.

---

## 📦 نصب

نصب Package اصلی:

```bash
dotnet add package NotificationServices
```

یا در Package Manager Console:

```powershell
Install-Package NotificationServices
```

هدف این پروژه این است که کاربر **یک Package** نصب کند و به Email و SMS دسترسی داشته باشد.

---

## 🚀 شروع سریع

### 1. ثبت سرویس‌ها

```csharp
using NotificationServices.DependencyInjection;

var services = new ServiceCollection();

services.AddNotificationServices();
```

در حالت پیش‌فرض، Provider داخلی از `IConfiguration` تنظیمات Notification را دریافت می‌کند.

### 2. دریافت Email و SMS Service

```csharp
using NotificationServices.Email.Abstractions.Interfaces;
using NotificationServices.Sms.Abstractions.Interfaces;

var provider = services.BuildServiceProvider();

var emailService = provider.GetRequiredService<IEmailService>();
var smsService = provider.GetRequiredService<ISmsService>();
```

### 3. ارسال ایمیل

```csharp
var result = await emailService.SendMessageAsync(
    new EmailMessage(
        "user@example.com",
        "خوش آمدید",
        "<h1>به برنامه خوش آمدید.</h1>",
        isHtml: true));
```

### 4. ارسال OTP ایمیل

```csharp
var result = await emailService.SendOtpAsync(
    new EmailOtp(
        "user@example.com",
        "123456"));
```

### 5. ارسال پیامک

```csharp
var result = await smsService.SendMessageAsync(
    new SmsMessage(
        "09120000000",
        "سفارش شما با موفقیت ثبت شد."));
```

### 6. ارسال OTP پیامکی

```csharp
var result = await smsService.SendOtpAsync(
    new SmsOtp(
        "09120000000",
        "123456"));
```

---

## ⚙️ تنظیمات با appsettings.json

Provider پیش‌فرض تنظیمات را از بخش زیر می‌خواند:

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

### امنیت

رمز عبور SMTP، اطلاعات سرویس پیامکی، API Key و Secretها را داخل Git Commit نکنید.

از گزینه‌هایی مانند Environment Variables، User Secrets، Secret Store یا Provider سفارشی امن استفاده کنید.

---

## 🔌 استفاده از منبع Configuration سفارشی

یکی از مهم‌ترین بخش‌های معماری همین قسمت است.

کتابخانه خودش Database Provider ندارد، چون دیتابیس و Infrastructure متعلق به برنامه مصرف‌کننده است.

قرارداد اصلی:

```csharp
public interface INotificationOptionsProvider
{
    ValueTask<NotificationOptions> GetOptionsAsync(
        CancellationToken cancellationToken = default);
}
```

### نمونه استفاده از دیتابیس

در پروژه خودتان می‌توانید این Interface را با Repository خودتان، EF Core، Dapper یا هر روش دیگری پیاده‌سازی کنید:

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

ثبت Provider سفارشی:

```csharp
services.AddNotificationServices<DatabaseNotificationOptionsProvider>();
```

برای Redis:

```csharp
services.AddNotificationServices<RedisNotificationOptionsProvider>();
```

برای API:

```csharp
services.AddNotificationServices<ApiNotificationOptionsProvider>();
```

یا هر Provider اختصاصی دیگر:

```csharp
services.AddNotificationServices<MyCompanyNotificationOptionsProvider>();
```

در این حالت خود NotificationServices نیازی ندارد بداند پشت Provider چه چیزی قرار گرفته است.

---

## 📧 سرویس ایمیل

### ارسال ایمیل عادی

```csharp
var result = await emailService.SendMessageAsync(
    new EmailMessage(
        "user@example.com",
        "فعال شدن حساب",
        "<p>حساب شما فعال شد.</p>",
        isHtml: true));
```

### ارسال OTP ایمیل

```csharp
var result = await emailService.SendOtpAsync(
    new EmailOtp(
        "user@example.com",
        "123456"));
```

نتیجه عملیات از طریق `EmailResult` برگردانده می‌شود تا Application به جزئیات SMTP وابسته نشود.

---

## 📱 سرویس پیامک

### ارسال پیامک عادی

```csharp
var result = await smsService.SendMessageAsync(
    new SmsMessage(
        "09120000000",
        "عملیات شما با موفقیت انجام شد."));
```

### ارسال OTP پیامکی

```csharp
var result = await smsService.SendOtpAsync(
    new SmsOtp(
        "09120000000",
        "123456"));
```

Application از API یکسانی استفاده می‌کند و لازم نیست بداند کدام Gateway در پشت آن قرار دارد.

---

## 🔌 معماری Provider برای پیامک

ساختار پیامک به این شکل است:

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
     ├── Provider آینده 1
     ├── Provider آینده 2
     └── Provider اختصاصی شما
```

در حال حاضر Provider مربوط به Melipayamak در پروژه وجود دارد و معماری به‌گونه‌ای است که Providerهای بعدی نیز قابل اضافه شدن باشند.

---

## 🏗️ تصویر کلی معماری

```text
┌─────────────────────────────────────────────┐
│                  Application                │
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

### مرزهای اصلی معماری

**مرز Configuration**

`INotificationOptionsProvider` منبع تنظیمات را از Notification Infrastructure جدا می‌کند.

**مرز Email**

`IEmailService` API مورد نیاز Application را فراهم می‌کند.

**مرز SMS**

`ISmsService` عملیات پیامکی را ارائه می‌دهد.

**مرز Provider**

`ISmsProvider` جزئیات Gatewayهای پیامکی را جدا می‌کند.

**مرز Dependency Injection**

`AddNotificationServices()` نقطه ورود اصلی برای ثبت Library است.

---

## 🧪 تست و CI

پروژه تست‌های خودکار برای بخش‌های مهم دارد، از جمله:

- ثبت Dependency Injection
- Provider پیش‌فرض Configuration
- Provider سفارشی Configuration
- Validation ایمیل و پیامک
- انتخاب Provider پیامک
- ساخت HTTP Request
- رفتار OTP
- مدیریت خطاها
- Cancellation

اجرای تست‌ها:

```bash
dotnet test NotificationServices.slnx
```

GitHub Actions نیز مراحل زیر را بررسی می‌کند:

```text
Restore
   ↓
Build
   ↓
Tests + Coverage
   ↓
Pack
   ↓
Package Verification
   ↓
Artifact Upload
```

Workflow:

```text
.github/workflows/ci.yml
```

---

## 📦 ساخت Package

Package اصلی مصرف‌کننده:

```text
NotificationServices
```

ساخت Package:

```bash
dotnet pack src/NotificationServices/NotificationServices.csproj -c Release
```

هدف ساختار فعلی این است که Email و SMS در قالب همان Package اصلی در اختیار مصرف‌کننده قرار بگیرند.

---

## 🧰 اجرای پروژه در حالت Development

Clone:

```bash
git clone https://github.com/Mohammad-Amin-Nazeri/Notification_Services_Pattern.git
cd Notification_Services_Pattern
```

Restore:

```bash
dotnet restore NotificationServices.slnx
```

Build:

```bash
dotnet build NotificationServices.slnx
```

Test:

```bash
dotnet test NotificationServices.slnx
```

Pack:

```bash
dotnet pack src/NotificationServices/NotificationServices.csproj -c Release
```

---

## 🤝 مشارکت در پروژه

مشارکت در پروژه آزاد است.

### اضافه کردن SMS Provider جدید

برای اضافه کردن Gateway جدید:

1. `ISmsProvider` را پیاده‌سازی کنید.
2. تنظیمات اختصاصی Provider را مشخص کنید.
3. انتخاب Provider را به Factory اضافه کنید.
4. تست‌های مربوطه را بنویسید.
5. مستندات را به‌روزرسانی کنید.
6. Pull Request ایجاد کنید.

### پیشنهاد سرویس جدید

اگر فکر می‌کنید کانال Notification دیگری لازم است، می‌توانید Issue ایجاد کنید و ایده خود را مطرح کنید:

- WhatsApp
- Telegram
- Push Notification
- Microsoft Teams
- Discord
- SMS Gatewayهای جدید
- Email Providerهای جدید
- کانال‌های ارتباطی دیگر

در Issue بهتر است Use Case، API پیشنهادی، نیازهای Provider و دلیل مفید بودن Integration را توضیح دهید.

---

## ⭐ حمایت از پروژه

اگر این پروژه برای شما مفید است، لطفاً Repository را ⭐ **Star** کنید.

یک Star باعث می‌شود پروژه بیشتر دیده شود و توسعه و نگهداری آن انگیزه بیشتری پیدا کند.

👉 **[⭐ Star کردن Notification Services Pattern در GitHub](https://github.com/Mohammad-Amin-Nazeri/Notification_Services_Pattern)**

اگر پروژه را در محصول یا پروژه شخصی خود استفاده می‌کنید، بازخورد و پیشنهادهای شما نیز بسیار ارزشمند است.

---

## 👨‍💻 توسعه‌دهنده پروژه

### محمد امین ناظری | Mohammad Amin Nazeri

توسعه‌دهنده و نگهدارنده پروژه **Notification Services Pattern**.

<a href="https://github.com/Mohammad-Amin-Nazeri">Mohammad Amin Nazeri</a>

[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/mohammad-amin-nazeri)
[![GitHub](https://img.shields.io/badge/GitHub-181717?style=for-the-badge&logo=github&logoColor=white)](https://github.com/Mohammad-Amin-Nazeri)
[![Telegram](https://img.shields.io/badge/Telegram-2CA5E2?style=for-the-badge&logo=telegram&logoColor=white)](https://t.me/Aminn02)
[![Instagram](https://img.shields.io/badge/Instagram-E4405F?style=for-the-badge&logo=instagram&logoColor=white)](https://www.instagram.com/mohammad_amin_nazeri/)

---

## 📄 لایسنس

این پروژه تحت **MIT License** منتشر شده است.

جزئیات در فایل [LICENSE](LICENSE) قرار دارد.

---

<div align="center">

### ⭐ اگر پروژه برایتان مفید بود، یک Star بدهید

[⬆️ بازگشت به ابتدای انگلیسی](#english)
&nbsp;&nbsp;•&nbsp;&nbsp;
[⬆️ بازگشت به ابتدای فارسی](#فارسی)

</div>
