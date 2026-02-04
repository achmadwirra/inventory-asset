using FluentValidation;
using Inventory.Application.DTOs;

namespace Inventory.Application.Validators;

public class CreateAssetRequestValidator : AbstractValidator<CreateAssetRequest>
{
    public CreateAssetRequestValidator()
    {
        RuleFor(x => x.AssetCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.PurchaseDate).NotEmpty(); // or some logic like <= Today
        RuleFor(x => x.Location).NotEmpty();
    }
}

public class AssignAssetRequestValidator : AbstractValidator<AssignAssetRequest>
{
    public AssignAssetRequestValidator()
    {
        RuleFor(x => x.AssetId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
