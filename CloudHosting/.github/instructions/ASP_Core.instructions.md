---  
description: Back-End Development  
---  
  
> ## Important rules  
>   
> - ASP.NET core development.  
>   
> - Main goal is to achieve  
>  
> - **Always** check every .cs files in the project for interopability changes and to reduce reduntant code (if needed).  
>   
> - **Don't overthink** an idea **by creating multiple files** if it can be done in one single file or class (**in a clean and comprehensive coding manner**).  
>   
> - Follow Core - Infrastructure - Presentation desing pattern.  
>  
>> ***Keep the design pattern consistent***.  

> ## Details             
>   
> **User entity** has one-to-many database relation to the files they upload (*this service relies on another project and is not of importance in this project*, just use a suitable entity that fits the criterea*).  
>   
> Interaction with the docker is done through the Docker.NET library that is mentioned in the "Refrences".  
>   
>> Everything related to working with Docker is done through our web project, meaning no bash or anything that is out of CSharp/.NET scope.  
>   
> We want to run user's web project, it includes Dockerfile so we only have to `build` the Docker Image and run the Docker Container at user's command.  
>   
>> User's can also stop the Docker Container.  
>   
> The logs and debug info should be communicated to the user in terminal/cmd.  
>   
> Resource info includes Availbe Ram and In-Use Ram, Available CPU Cores and CPU Utilization, Available Storage and Used Storage, Latency and container load status (if possible).
>   
> **User entity** has one-to-many database relation to "cloud plans" which they buy that determines resource allocation (*this service relies on another project and is not of importance in this project*, just use a suitable entity that fits the criterea*).  
>   
> When a user wants to run a web project on our Docker, they can choose between the fetched "cloud plans" that they own.  
>   
>> "Expired" "cloud plans" should not show up in the fetched list.  

> ## Refrences  
>   
> - ASP.NET  
>  
>> Url: https://learn.microsoft.com/en-us/aspnet/core/?view=aspnetcore-9.0  
>  
> - .NET  
>  
>> Url: https://learn.microsoft.com/en-us/dotnet/  
>  
> - CSharp  
>  
>> Url: https://learn.microsoft.com/en-us/dotnet/csharp/  
>  
> - ASP.NET Clean Architecture And Design Pattern  
>  
>> Url: https://medium.com/@mohanedzekry/clean-architecture-in-asp-net-core-web-api-d44e33893e1d  
>  
> - Docker CSharp Api  
>> Url: https://github.com/dotnet/Docker.DotNet

> ## Keep in mind  
>   
> - Try to optimize for **clean code**.  
> 
> - Follow clean architecture rules and design pattern.  