using FluentValidation;
using HostelFinder.Application.DTOs.Hostel.Requests;

namespace HostelFinder.Application.Validations.Hostel
{
    public class CreateHostelRequestValidation : AbstractValidator<AddHostelRequestDto>
    {
        public CreateHostelRequestValidation()
        {
            RuleFor(x => x.LandlordId)
                .NotNull().WithMessage("Mã chủ trọ không được để trống.");

            RuleFor(x => x.HostelName)
                .NotEmpty().WithMessage("Tên trọ không được để trống.")
                .MinimumLength(2).WithMessage("Tên trọ phải chứa ít nhất 2 kí tự.")
                .MaximumLength(100).WithMessage("Tên trọ chứa tối đa 100 kí tự.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Mô tả không được để trống.")
                .MinimumLength(10).WithMessage("Mô tả phải chứa ít nhất 10 kí tự.")
                .MaximumLength(500).WithMessage("Mô tả chứa tối đa 500 kí tự.");

            RuleFor(x => x.Address)
                .NotNull().WithMessage("Địa chỉ không được để trống.");

            RuleFor(x => x.Size)
                .GreaterThan(0).WithMessage("Kích thước phải là số dương.")
                .When(x => x.Size.HasValue);

            RuleFor(x => x.NumberOfRooms)
                .GreaterThan(0).WithMessage("Số phòng phải là số dương.");

            RuleFor(x => x.Coordinates)
                .NotEmpty().WithMessage("Tọa độ không được để trống.");
        }
    }
}
