using FluentValidation;
using Common.Core.Models;

namespace Common.Core.Validators;

public class SettlementRequestValidator : AbstractValidator<SettlementRequest>
{
    public SettlementRequestValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("患者ID不能为空")
            .Length(1, 50).WithMessage("患者ID长度必须在1-50之间");

        RuleFor(x => x.PatientName)
            .NotEmpty().WithMessage("患者姓名不能为空")
            .Length(1, 100).WithMessage("患者姓名长度必须在1-100之间");

        RuleFor(x => x.PatientCardNo)
            .NotEmpty().WithMessage("患者卡号不能为空")
            .Length(1, 50).WithMessage("患者卡号长度必须在1-50之间");

        RuleFor(x => x.TotalAmount)
            .GreaterThan(0).WithMessage("结算金额必须大于0");

        RuleFor(x => x.DiagnosisCode)
            .NotEmpty().WithMessage("诊断编码不能为空")
            .Length(1, 50).WithMessage("诊断编码长度必须在1-50之间");

        RuleFor(x => x.DiagnosisName)
            .NotEmpty().WithMessage("诊断名称不能为空")
            .Length(1, 200).WithMessage("诊断名称长度必须在1-200之间");

        RuleFor(x => x.HospitalCode)
            .NotEmpty().WithMessage("医院编码不能为空")
            .Length(1, 50).WithMessage("医院编码长度必须在1-50之间");

        RuleFor(x => x.HospitalName)
            .NotEmpty().WithMessage("医院名称不能为空")
            .Length(1, 200).WithMessage("医院名称长度必须在1-200之间");
    }
}