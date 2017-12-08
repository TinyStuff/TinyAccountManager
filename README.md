# TinyAccountManager
Account manager for Xamarin and UWP. Store account information in your app in a secure way.

## Build status
| Platform | Status |
|---|---|
| iOS | <img src="https://io2gamelabs.visualstudio.com/_apis/public/build/definitions/be16d002-5786-41a1-bf3b-3e13d5e80aa0/11/badge" /> |
| Android | <img src="https://io2gamelabs.visualstudio.com/_apis/public/build/definitions/be16d002-5786-41a1-bf3b-3e13d5e80aa0/12/badge" /> |
| UWP | <img src="https://io2gamelabs.visualstudio.com/_apis/public/build/definitions/be16d002-5786-41a1-bf3b-3e13d5e80aa0/10/badge" /> |

## Get started
This is as short guide how to get started using TinyAccountManager.

### How to install
The easiest way is to install the package from NuGet:

```
Install-Package TinyAccountManager
```

You should install it on all your platform project and if you have other projects in your solution where you want to access it, you should install it there as well.

### How to use
Here is a small get started guide, there are also a sample project if you take a look in the src folder.

#### Initialization
The first you need to do is to initialize the AccountManager per platform.

```csharp
//iOS
TinyAccountManager.iOS.AccountManager.Initialize();

//Android
TinyAccountManager.Droid.AccountManager.Initialize();

//UWP
TinyAccountManager.UWP.AccountManager.Initialize();
```

#### Save
The only filed that are required is ServiceId.

```csharp
var account = new Account()
{
    ServiceId = "TinyAccountManagerSample",
    Username = "dhindrik"
};

account.Properties.Add("Password", "MySecretPassword");

await AccountMananger.Current.Save(account);
```

#### Get and Exists
It's recommended that you use Exists before Get, if you using Get and there is no matching account it will throw an exception.
```csharp
Account account = null;

var exists = await AccountManager.Current.Exists("TinyAccountManagerSample")

if(exists)
  account = await AccountManager.Current.Get("TinyAccountManagerSample")
```

#### Remove
```csharp
await AccountManager.Current.Remove("TinyAccountManagerSample")
```

#### IOC
If you want to use IOC instead of the singleton pattern, you just register the implemenation for each platform with the IAccountManager interface. If you select this way you don't have to run Initialize on each platform

**iOS:** iOSAccountManager

**Android:** AndroidAccountManager

**UWP:** UWPAccountManager
