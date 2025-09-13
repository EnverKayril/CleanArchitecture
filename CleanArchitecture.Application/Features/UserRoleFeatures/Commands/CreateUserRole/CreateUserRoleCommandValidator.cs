﻿using FluentValidation;

namespace CleanArchitecture.Application.Features.UserRoleFeatures.Commands.CreateUserRole;

public sealed class CreateUserRoleCommandValidator : AbstractValidator<CreateUserRoleCommand>
{
    public CreateUserRoleCommandValidator()
    {
        RuleFor(p => p.RoleId).NotEmpty().WithMessage("Rol Id boş olamaz!");
        RuleFor(p => p.RoleId).NotNull().WithMessage("Rol Id boş olamaz!");
        RuleFor(p => p.UserId).NotEmpty().WithMessage("Kullanıcı Id boş olamaz!");
        RuleFor(p => p.UserId).NotNull().WithMessage("Kullanıcı Id boş olamaz!");
    }
}
