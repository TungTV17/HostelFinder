using HostelFinder.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace HostelFinder.Application.DTOs.Room.Responses
{
    public class GetAllInformationRoomResponseDto
    {
        // Thông tin của phòng trọ
        public RoomInfoDetailResponseDto? RoomInfoDetail { get; set; }

        // ảnh của phòng trọ
        public List<string>? PictureRoom { get; set; }

        //Thông tin khách thuê phòng

        public List<InformationTenacyReponseDto>? InfomationTenacy{ get; set; }

        //thông tin hóa đơn mới nhất
        public RoomInvoiceHistoryDetailsResponseDto? InvoiceDetailInRoom { get; set; }

        //thông tin hợp đồng hiện tại

        public RoomContractHistoryResponseDto? ContractDetailInRoom { get; set; }

        public RoomRepairHostoryResponseDto? RoomRepairHostory { get; set; }

    }
}
