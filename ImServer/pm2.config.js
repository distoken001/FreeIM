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
            },
            // env_testing: {
            // ASPNETCORE_ENVIRONMENT: "Testing",
            //   },
            env_production: {
                ASPNETCORE_ENVIRONMENT: "Production",
            },
        },
    ],
};