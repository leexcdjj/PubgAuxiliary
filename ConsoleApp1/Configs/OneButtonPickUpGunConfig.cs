namespace ConsoleApp1.Configs;

/// <summary>
/// 一键捡枪配置
/// </summary>
public class OneButtonPickUpGunConfig
{
    /// <summary>
    /// 打开背包后延迟
    /// </summary>
    public int BagSleep { get; set; }
    
    /// <summary>
    /// 拖动后延迟
    /// </summary>
    public int MoveSleep { get; set; }
    
    /// <summary>
    /// 拖动间隔延迟
    /// </summary>
    public int MoveIntervalSleep { get; set; }
    
    /// <summary>
    /// 拖动坐标配置
    /// </summary>
    public List<MoveCoordinatesConfig> MoveCoordinates { get; set; }
}