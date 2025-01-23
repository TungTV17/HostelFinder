namespace HostelFinder.Domain.Enums;

public enum ChargingMethod
{
    /// <summary>
    /// Tính theo đơn vị sử dụng
    /// </summary>
    PerUsageUnit,
    
    /// <summary>
    /// Tính theo số người
    /// </summary>
    PerPerson,
    /// <summary>
    /// Không tính phí
    /// </summary>
    Free,
    
    /// <summary>
    /// Tính phí cố định
    /// </summary>
    FlatFee
}