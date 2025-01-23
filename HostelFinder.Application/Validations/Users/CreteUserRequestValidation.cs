using FluentValidation;
using HostelFinder.Application.DTOs.Users.Requests;

namespace HostelFinder.Application.Validations.Users
{
    public class CreteUserRequestValidation : AbstractValidator<CreateUserRequestDto>
    {
        public CreteUserRequestValidation()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .MinimumLength(2).WithMessage("Tên người dùng phải chứa ít nhất 2 kí tự.")
                .MaximumLength(100).WithMessage("Tên người dùng chứa tối đa 100 kí tự.");
            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6).WithMessage("Mật khẩu phải chứa ít nhất 6 kí tự.")
                .MaximumLength(100).WithMessage("Mật khẩu chứa tối đa 100 kí tự.");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống.")
                .EmailAddress().WithMessage("Email không hợp lệ.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Số điện thoại không được để trống.")
                .Matches(@"^(0|\+84)[3|5|7|8|9][0-9]{8}$").WithMessage("Số điện thoại phải là số hợp lệ.");

        }
    }
}
