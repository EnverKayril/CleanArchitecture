using FluentValidation;

namespace CleanArchitecture.Application.Features.AuthFeatures.Commands.CreateNewTokenByRefreshToken;

public sealed class CreateNewTokenByRefreshTokenCommandValidator : AbstractValidator<CreateNewTokenByRefreshTokenCommand>
{
    public CreateNewTokenByRefreshTokenCommandValidator()
    {
        RuleFor(p => p.UserId).NotEmpty().WithMessage("Kullanıcı bilgisi boş olamaz!");
        RuleFor(p => p.UserId).NotNull().WithMessage("Kullanıcı bilgisi boş olamaz!");
        RuleFor(p => p.RefreshToken).NotNull().WithMessage("Refresh Token bilgisi boş olamaz!");
        RuleFor(p => p.RefreshToken).NotEmpty().WithMessage("Refresh Token bilgisi boş olamaz!");
    }
}
