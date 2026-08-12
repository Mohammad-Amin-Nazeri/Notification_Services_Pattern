# Notification Services Pattern

[🇬🇧 English](#english) | [🇮🇷 فارسی](#فارسی)

[![CI](https://github.com/Mohammad-Amin-Nazeri/Notification_Services_Pattern/actions/workflows/ci.yml/badge.svg)](https://github.com/Mohammad-Amin-Nazeri/Notification_Services_Pattern/actions/workflows/ci.yml)

<a id="english"></a>

## 🇬🇧 English

A reusable and extensible notification infrastructure for .NET applications, providing Email and SMS services with clear abstractions, dependency injection, provider-based SMS architecture, OTP support, configuration isolation, automated tests, and continuous integration.

### ✨ Features

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

### 🏗️ Architecture

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

### 📦 Supported Services

#### Email

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

#### SMS

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

The repository currently includes a Melipayamak provider implementation. New gateways can be added through the provider abstraction.

### ⚙️ Dependency Injection

Register Email:

```csharp
services.AddEmailService();
```

Register SMS:

```csharp
services.AddSmsService();
```

Custom configuration providers can be supplied without changing the core services:

```csharp
services.AddEmailService<MyEmailOptionsProvider>();
services.AddSmsService<MySmsOptionsProvider>();
```

### 🔧 Configuration

The default implementations read provider settings from configuration.

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

Never commit real passwords, API keys, SMTP credentials, or SMS provider secrets to source control. Use environment variables, .NET User Secrets, or another secure configuration provider.

### 🧩 Extensibility

Application code depends on abstractions rather than provider-specific implementations:

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

### 🧪 Testing

The repository contains a dedicated test project:

```text
Tests/NotificationServices.Tests
```

The tests cover service validation, provider selection, configuration binding and validation, HTTP requests, provider failures, and OTP behavior.

SMS provider tests use a fake `HttpMessageHandler`, so unit tests do not send real SMS messages or require external services.

Run all tests locally:

```bash
dotnet test
```

### 🤖 Continuous Integration

GitHub Actions automatically performs:

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

### 🚀 Getting Started

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

### 🌟 Support the Project

If this project is useful to you, please consider giving the repository a ⭐ on GitHub. It helps the project gain visibility and motivates further development.

### 🤝 Contributing & Suggesting New Services

Contributions are welcome.

Have an idea for a new notification service or provider? Open a GitHub **Issue** and describe the use case and proposed integration.

Examples of possible future services:

- WhatsApp
- Telegram
- Push Notifications
- Microsoft Teams
- Discord
- Other SMS gateways
- Other email providers

You can also open a Pull Request with a new provider implementation.

### 🛠️ Technologies

- C#
- .NET 10
- Dependency Injection
- MailKit
- HttpClient
- Microsoft.Extensions.Configuration
- xUnit
- Moq
- GitHub Actions

### 🎯 Project Goals

The goal is to provide a small, reusable notification infrastructure that can be integrated into different .NET applications while keeping business code independent from specific gateways and configuration sources.

### 🔮 Roadmap

- [ ] Coverage reporting and badge
- [ ] Additional SMS providers
- [ ] Additional notification channels
- [ ] NuGet packages
- [ ] Package metadata and release automation
- [ ] Optional integration tests
- [ ] Email templates and attachments

### 👨‍💻 Developer

Developed and maintained by **Mohammad Amin Nazeri**.

- 💻 GitHub: [Mohammad-Amin-Nazeri](https://github.com/Mohammad-Amin-Nazeri)
- 💼 LinkedIn: [Mohammad Amin Nazeri](https://www.linkedin.com/in/mohammad-amin-nazeri/)
- 📱 Telegram: [@nazrymhmdamyn85](https://t.me/nazrymhmdamyn85)
- 📸 Instagram: [@nazrymhmdamyn85](https://www.instagram.com/nazrymhmdamyn85/)

### 📄 License

MIT License. See [`LICENSE`](LICENSE).

---

<a id="فارسی"></a>

## 🇮🇷 فارسی

یک زیرساخت قابل استفاده مجدد و قابل توسعه برای ارسال اعلان در پروژه‌های .NET که سرویس‌های Email و SMS را با Abstractionهای مشخص، Dependency Injection، معماری مبتنی بر Provider، پشتیبانی از OTP، جداسازی Configuration، تست واحد و CI ارائه می‌کند.

### ✨ امکانات

- 📧 ارسال ایمیل‌های عمومی
- 🔐 ارسال OTP از طریق ایمیل
- 📱 ارسال پیامک‌های عمومی
- 🔐 ارسال OTP از طریق پیامک
- 🔌 Abstraction برای Providerهای پیامک
- 🏭 Factory برای انتخاب SMS Provider
- ⚙️ امکان جایگزینی منبع Configuration
- 💉 Extensionهای مربوط به Dependency Injection
- 🧪 تست واحد بدون ارسال واقعی SMS یا SMTP
- 🤖 اجرای خودکار Build و Test با GitHub Actions
- 🧩 جداسازی کامل Abstraction و Implementation

### 🏗️ معماری

```text
ISmsService
    ↓
SmsService
    ↓
ISmsProviderFactory
    ↓
ISmsProvider
    ├── MelipayamakSmsProvider
    └── Providerهای آینده
```

در بخش Email نیز سرویس اصلی از Abstractionهای مربوط به Configuration استفاده می‌کند تا منبع تنظیمات بتواند در آینده از `appsettings.json`، Database یا هر منبع دیگری تأمین شود.

### 📦 نمونه استفاده

ارسال پیامک عادی:

```csharp
await smsService.SendMessageAsync(
    new SmsMessage(
        "09120000000",
        "سفارش شما با موفقیت ثبت شد."));
```

ارسال OTP پیامکی:

```csharp
await smsService.SendOtpAsync(
    new SmsOtp(
        "09120000000",
        "123456"));
```

ارسال ایمیل:

```csharp
await emailService.SendMessageAsync(
    new EmailMessage(
        "user@example.com",
        "خوش آمدید",
        "<h1>Welcome!</h1>",
        true));
```

ارسال OTP ایمیلی:

```csharp
await emailService.SendOtpAsync(
    new EmailOtp(
        "user@example.com",
        "123456"));
```

### ⚙️ ثبت در Dependency Injection

```csharp
services.AddEmailService();
services.AddSmsService();
```

همچنین می‌توان Provider تنظیمات سفارشی تعریف کرد:

```csharp
services.AddEmailService<MyEmailOptionsProvider>();
services.AddSmsService<MySmsOptionsProvider>();
```

### 🧪 تست‌ها

تست‌ها در مسیر زیر قرار دارند:

```text
Tests/NotificationServices.Tests
```

تست‌های پروژه رفتار سرویس‌ها، اعتبارسنجی، Factory، Configuration، درخواست‌های HTTP، خطاهای Provider و OTP را پوشش می‌دهند.

برای اجرای تمام تست‌ها:

```bash
dotnet test
```

### 🤝 مشارکت و پیشنهاد سرویس جدید

مشارکت در توسعه پروژه آزاد است.

اگر به یک Provider یا کانال اعلان جدید نیاز دارید، یک **Issue** در GitHub ایجاد کنید و کاربرد و پیشنهاد خود را توضیح دهید.

برای نمونه می‌توان در آینده سرویس‌هایی مانند موارد زیر را اضافه کرد:

- WhatsApp
- Telegram
- Push Notification
- Microsoft Teams
- Discord
- Providerهای بیشتر SMS
- Providerهای بیشتر Email

همچنین می‌توانید برای Provider جدید Pull Request ارسال کنید.

### 🌟 حمایت از پروژه

اگر این پروژه برای شما مفید بود، با دادن یک ⭐ به Repository در GitHub از پروژه حمایت کنید. این کار به دیده‌شدن پروژه و ادامه توسعه آن کمک می‌کند.

### 👨‍💻 توسعه‌دهنده

توسعه داده و نگهداری می‌شود توسط **محمدامین ناظری**.

- 💻 GitHub: [Mohammad-Amin-Nazeri](https://github.com/Mohammad-Amin-Nazeri)
- 💼 LinkedIn: [Mohammad Amin Nazeri](https://www.linkedin.com/in/mohammad-amin-nazeri/)
- 📱 Telegram: [@nazrymhmdamyn85](https://t.me/nazrymhmdamyn85)
- 📸 Instagram: [@nazrymhmdamyn85](https://www.instagram.com/nazrymhmdamyn85/)

### 📄 مجوز

این پروژه تحت مجوز MIT منتشر شده است. جزئیات در فایل [`LICENSE`](LICENSE) قرار دارد.
