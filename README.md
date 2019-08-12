# KK.DotNet.BackgroundTasks.Scheduled

This project contains the code for the NuGet Package to get scheduled tasks in an asp.net core application.  

## Usage

### Task

You can add multiple tasks in your projekt. The task has to implement the `IScheduledTask` interface.  
The options for the task are provided by the DI system and you can request the options with `IScheduledTaskOptions<SampleTask>` as shown below.  
The `ExecuteAsync` Method will be executed when the task is triggert.

```C#
public class SampleTask : IScheduledTask
{
    public SampleTask(
        IScheduledTaskOptions<SampleTask> options
    )
    {
        this.Options = options;           
    }

    public IScheduledTaskOptions<IScheduledTask> Options { get; }

    public async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Stuff to do...
    }
}
```

> **INFO** You shoud use the provided `CancellationToken` in your code to support cancelling of your task.

You have to register the task in your `public void ConfigureServices` of the `Startup.cs`. You can use the provided `AddScheduledTask` Method:

```C#
services.AddScheduledTask<SampleTask>();
```

The Task will be added as a `Singleton` with this method.  
If you does not want this you can also register it by yourself:

```C#
services.AddScoped<IScheduledTask, SampleTask>();
```


### Options

The options are provided as an `IScheduledTaskOptions` of type `YourSampleTask` through the DI system

| Property | Description | Default |
| --- | --- | --- |
| `Schedule` | A CronTab sting which describes the execution schedule | **nothing**, must be set by the user. For the format have a look [here](https://github.com/HangfireIO/Cronos#cron-format) |
| `CronFormat` | A `Cronos.CronFormat` value, this must only be set when you ant to use seconds in your schedule | `Cronos.CronFormat.Standard`, more information [here](https://github.com/HangfireIO/Cronos) |

In your `public void ConfigureServices` of the `Startup.cs` you can add options like so:

```C#
services.AddSingleton<IScheduledTaskOptions<SampleTask>>(
    new ScheduledTaskOptions<SampleTask>
    {
        Schedule = "*/10 * * * * *",
        CronFormat = Cronos.CronFormat.IncludeSeconds
    }
);
```

### SchedulerHost

After the registration of all your tasks there is only one thing left to do.  
Register the `SchedulerHostedService` in your `Startup.cs` like so:

```C#
services.AddScheduler();
```

If a custom implemented scheduler should be used, use this kind of registration:

```C#
services.AddHostedService<CustomSchedulerHostedService>();
```

`CustomSchedulerHostedService` must implement `ISchedulerHostedService`.

### Scheduler

When using the default SchedulerHostedService than there is also a `Scheduler`.  
This can be used to get access to the scheduled task list for example.

### Sample

You can find a sample how to get this working in the `samples` folder. Please have a look there and read the comments.

## NuGet

| Feed | Name | Status |
| --- | --- | --- |
| NuGet.org | [KK.DotNet.BackgroundTasks.Scheduled](https://www.nuget.org/packages/KK.DotNet.BackgroundTasks.Scheduled/) | [![NuGet Badge](https://img.shields.io/nuget/v/KK.DotNet.BackgroundTasks.Scheduled.svg)](https://www.nuget.org/packages/KK.DotNet.BackgroundTasks.Scheduled/) |
| Azure DevOps | [KK.DotNet.BackgroundTasks.Scheduled](https://dev.azure.com/kirkone/KK.DotNet.BackgroundTasks.Scheduled/_packaging?_a=package&feed=70450bc2-9936-4d1b-b153-be005873090e&package=f46d7f0d-1a7b-4006-8177-46359dcf8ad5) | [![KK.DotNet.BackgroundTasks.Scheduled package in KK.DotNet.BackgroundTasks.Scheduled feed in Azure Artifacts](https://feeds.dev.azure.com/kirkone/_apis/public/Packaging/Feeds/d2b0f7d3-06ab-484c-84ed-49af019c20c0/Packages/26191217-af92-48b4-ab59-8ce125f02723/Badge)](https://dev.azure.com/kirkone/KK.DotNet.BackgroundTasks.Scheduled/_packaging?_a=package&feed=d2b0f7d3-06ab-484c-84ed-49af019c20c0&package=26191217-af92-48b4-ab59-8ce125f02723&preferRelease=true) |

You can add the package for example with the following `dotnet` command:

```Shell
dotnet add package KK.DotNet.BackgroundTasks.Scheduled
```

Pre-releases of this Package are pushed to an internal feed an Azure DevOps. There is no public access to this feeds at the moment.

The build environment for this project is on Azure DevOps and can be found here [dev.azure.com/kirkone/KK.DotNet.BackgroundTasks.Scheduled](https://dev.azure.com/kirkone/KK.DotNet.BackgroundTasks.Scheduled/_build)

#### Build

| Name | Status |
| --- | --- |
| KK.DotNet.BackgroundTasks.Scheduled CI | [![Build Status](https://dev.azure.com/kirkone/KK.DotNet.BackgroundTasks.Scheduled/_apis/build/status/KK.DotNet.BackgroundTasks.Scheduled%20CI?branchName=master)](https://dev.azure.com/kirkone/KK.DotNet.BackgroundTasks.Scheduled/_build/latest?definitionId=30&branchName=master) |

#### Release

| Name | Status |
| --- | --- |
| KK.DotNet.BackgroundTasks.Scheduled CD | |
| Alpha | [![Alpha](https://vsrm.dev.azure.com/kirkone/_apis/public/Release/badge/b1423fc9-d9b5-4555-8599-ff7a1fdea8f9/2/2)](https://dev.azure.com/kirkone/KK.DotNet.BackgroundTasks.Scheduled/_release?view=all&definitionId=2&_a=releases) |
| Beta | [![Beta](https://vsrm.dev.azure.com/kirkone/_apis/public/Release/badge/b1423fc9-d9b5-4555-8599-ff7a1fdea8f9/2/3)](https://dev.azure.com/kirkone/KK.DotNet.BackgroundTasks.Scheduled/_release?view=all&definitionId=2&_a=releases) |
| Release | [![Release](https://vsrm.dev.azure.com/kirkone/_apis/public/Release/badge/b1423fc9-d9b5-4555-8599-ff7a1fdea8f9/2/4)](https://dev.azure.com/kirkone/KK.DotNet.BackgroundTasks.Scheduled/_release?view=all&definitionId=2&_a=releases) |


## Authors

-   **Kirsten Kluge** - _Initial work_ - [kirkone](https://github.com/kirkone)
-   **paule96** - _Refactoring and understanding awesome interface stuff_ - [paule96](https://github.com/paule96)
-   **TiltonJH** - _Unbelievable knowledge about awesome interface stuff_ - [TiltonJH](https://github.com/TiltonJH)

See also the list of [contributors](https://github.com/kirkone/KK.DotNet.BackgroundTasks.Scheduled/graphs/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

-   Inspired by this [Microsoft documentation](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-2.2) and this [Blog post](https://blog.maartenballiauw.be/post/2017/08/01/building-a-scheduled-cache-updater-in-aspnet-core-2.html) repo
