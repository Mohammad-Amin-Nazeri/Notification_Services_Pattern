# الگوی سرویس‌های اعلان

[🇬🇧 English](README.md) | [🇮🇷 فارسی](README.fa.md)

[![CI](https://github.com/Mohammad-Amin-Nazeri/Notification_Services_Pattern/actions/workflows/ci.yml/badge.svg)](https://github.com/Mohammad-Amin-Nazeri/Notification_Services_Pattern/actions/workflows/ci.yml)

یک زیرساخت قابل استفاده مجدد و توسعه‌پذیر برای سرویس‌های اعلان در برنامه‌های .NET که سرویس‌های Email و SMS را با Abstractionهای مشخص، Dependency Injection، معماری مبتنی بر Provider، پشتیبانی از OTP، Configuration قابل تعویض و تست‌های خودکار ارائه می‌کند.

## ✨ امکانات

- 📧 ارسال پیام عمومی Email
- 🔐 ارسال OTP از طریق Email
- 📱 ارسال پیام عمومی SMS
- 🔐 ارسال OTP از طریق SMS
- 🔌 Abstraction برای SMS Providerها
- 🏭 Factory برای انتخاب SMS Provider
- ⚙️ پشتیبانی از Configuration Provider قابل تعویض
- 💉 Extensionهای مربوط به Dependency Injection
- 🧪 Unit Test بدون ارسال واقعی SMS یا اتصال واقعی SMTP
- 🤖 GitHub Actions برای Restore، Build و Test
- 🧩 جداسازی Abstractionها از Implementationها

## 🏗️ معماری

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

## 📦 سرویس‌های پشتیبانی‌شده

### Email

Abstraction مربوط به Email دو عملیات اصلی دارد:

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

Abstraction مربوط به SMS دو عملیات اصلی دارد:

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

در Repository فعلی یک Provider برای Melipayamak وجود دارد. برای اضافه کردن Gateway جدید می‌توان `ISmsProvider` را پیاده‌سازی کرد و Provider را به `SmsProviderFactory` اضافه کرد.

## ⚙️ Dependency Injection

ثبت Email:

```csharp
services.AddEmailService();
```

ثبت SMS:

```csharp
services.AddSmsService();
```

امکان استفاده از Configuration Provider اختصاصی نیز وجود دارد:

```csharp
services.AddEmailService<MyEmailOptionsProvider>();
services.AddSmsService<MySmsOptionsProvider>();
```

## 🔧 Configuration

Implementationهای پیش‌فرض تنظیمات Provider را از Configuration دریافت می‌کنند.

نمونه:

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

رمز عبور، API Key، اطلاعات SMTP یا هر Secret واقعی را داخل Source Control قرار ندهید. برای اطلاعات حساس از Environment Variable، .NET User Secrets یا یک Configuration Provider امن استفاده کنید.

## 🧩 توسعه‌پذیری

طراحی پروژه باعث می‌شود کد اصلی برنامه به Abstractionها وابسته باشد، نه به Providerهای خاص.

ساختار SMS:

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

Configuration نیز می‌تواند از منابعی غیر از `appsettings.json` دریافت شود. برای این کار می‌توان Interfaceهای زیر را پیاده‌سازی کرد:

```csharp
IEmailProviderOptionsProvider
ISmsProviderOptionsProvider
```

## 🧪 تست‌ها

پروژه تست به‌صورت جداگانه در این مسیر قرار دارد:

```text
Tests/NotificationServices.Tests
```

تست‌ها اعتبارسنجی Serviceها، انتخاب Provider، Binding و Validation تنظیمات، Requestهای HTTP، خطاهای Provider و رفتار OTP را پوشش می‌دهند.

تست‌های SMS Provider از `Fake HttpMessageHandler` استفاده می‌کنند؛ بنابراین Unit Testها SMS واقعی ارسال نمی‌کنند و به سرویس خارجی وابسته نیستند.

اجرای تمام تست‌ها:

```bash
dotnet test
```

## 🤖 Continuous Integration

GitHub Actions برای Push و Pull Requestهای مربوط به `master` به‌صورت خودکار Pipeline زیر را اجرا می‌کند:

```text
Restore
  ↓
Build
  ↓
Test
```

Workflow در این مسیر قرار دارد:

```text
.github/workflows/ci.yml
```

## 🚀 شروع کار

Repository را Clone کنید:

```bash
git clone https://github.com/Mohammad-Amin-Nazeri/Notification_Services_Pattern.git
cd Notification_Services_Pattern
```

Restore و Build:

```bash
dotnet restore
dotnet build
```

اجرای تست‌ها:

```bash
dotnet test
```

پروژه Sample در مسیر زیر قرار دارد:

```text
samples/NotificationServices.Sample
```

## 🛠️ تکنولوژی‌ها

- C#
- .NET 10
- Dependency Injection
- MailKit
- HttpClient
- Microsoft.Extensions.Configuration
- xUnit
- Moq
- GitHub Actions

## 🎯 هدف پروژه

هدف اصلی این Repository ارائه یک زیرساخت کوچک، قابل استفاده مجدد و قابل توسعه برای Notification در برنامه‌های .NET است؛ به شکلی که بتوان آن را در پروژه‌های مختلف استفاده کرد یا در آینده به NuGet Packageهای قابل استفاده مجدد تبدیل کرد.

طراحی پروژه وابستگی کد اصلی برنامه به Gatewayهای خاص و منابع Configuration مشخص را کاهش می‌دهد.

## 🔮 Roadmap

- [ ] اضافه کردن Coverage Report و Badge
- [ ] اضافه کردن SMS Providerهای بیشتر
- [ ] ساخت NuGet Package
- [ ] اضافه کردن Metadata و Release Automation برای Package
- [ ] اضافه کردن Integration Testهای اختیاری
- [ ] امکانات بیشتر برای Email مانند Template و Attachment

## 📄 مجوز

این پروژه تحت مجوز MIT منتشر می‌شود. جزئیات در فایل [`LICENSE`](LICENSE) قرار دارد.
