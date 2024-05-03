using ConsoleApp1.Configs;
using Microsoft.Extensions.Configuration;

namespace ConsoleApp1;

/// <summary>
/// 配置管理器类，负责加载配置文件并创建配置对象
/// </summary>
public class ConfigurationManager
{
    public static SystemConfig LoadConfig()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        return config.Get<SystemConfig>();
    }
}