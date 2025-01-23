namespace HostelFinder.Domain.Enums
{
    public enum TemporaryResidenceStatus
    {
        /// <summary>
        /// Chưa khai báo tạm trụ tạm vắng
        /// </summary>
        /// 
        NotDeclared,

        /// <summary>
        /// Tạm trú có thời hạn
        /// </summary>
        /// 
        TemporaryWithTerm, 

        /// <summary>
        /// Tạm trú không thời hạn
        /// </summary>
        TemporaryWithoutTerm   
    }
}
