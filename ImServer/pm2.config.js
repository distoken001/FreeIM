module.exports = {
    apps: [
        {
            name: "ImServer",
            cmd: "dotnet",
            args: "ImServer.dll",
            watch: false,
            autorestart: true,
            env_development: {
                ASPNETCORE_ENVIRONMENT: "Development",
                PORT: 6001 // 设置生产环境下应用程序监听的端口号
            },
            // env_testing: {
            // ASPNETCORE_ENVIRONMENT: "Testing",
            //   },
            env_production: {
                ASPNETCORE_ENVIRONMENT: "Production",
                PORT: 6001 // 设置生产环境下应用程序监听的端口号
            },
        },
    ],
};