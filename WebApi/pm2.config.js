module.exports = {
    apps: [
        {
            name: "WebApi",
            cmd: "dotnet",
            args: "WebApi.dll",
            watch: false,
            autorestart: true,
            env_development: {
                ASPNETCORE_ENVIRONMENT: "Development",
                PORT: 5001 // 设置生产环境下应用程序监听的端口号
            },
            // env_testing: {
            // ASPNETCORE_ENVIRONMENT: "Testing",
            //   },
            env_production: {
                ASPNETCORE_ENVIRONMENT: "Production",
                PORT: 5001// 设置生产环境下应用程序监听的端口号
            },
        },
    ],
};