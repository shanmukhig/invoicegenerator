using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Attributes;
using FluentValidation.Validators;

namespace InvoiceGenerator.Entities
{
  //public class ContainerValidatorFactory : IValidatorFactory, IParameterValidatorFactory
  //{
  //  private readonly IContainer container;

  //  public ContainerValidatorFactory(IContainer container)
  //  {
  //    this.container = container;
  //  }

  //  public IValidator<T> GetValidator<T>()
  //  {
  //    return (IValidator<T>) this.GetValidator(typeof(T));
  //    //return this.container.GetInstance<IValidator<T>>();
  //  }

  //  public IValidator GetValidator(Type type)
  //  {
  //    if (type == (Type) null)
  //    {
  //      return (IValidator) null;
  //    }

  //    Type closedType = typeof(IValidator<>).MakeGenericType(type);
  //    return (IValidator)this.container.GetInstance(closedType);
  //  }

  //  public IValidator GetValidator(ParameterInfo parameterInfo)
  //  {
  //    if (parameterInfo == null)
  //    {
  //      return (IValidator) null;
  //    }
  //    return (IValidator)this.container.GetInstance(parameterInfo.ParameterType);
  //  }
  //}

  [Validator(typeof(CustomerValidator))]
  public class Customer : BaseEntity
  {
    public string CustomerName { get; set; }
    public string ContactName { get; set; }
    public Address Address { get; set; }
    public IEnumerable<Tax> Taxes { get; set; }
    public string Comments { get; set; }
    public string Currency { get; set; }
  }
  
  public class Tax
  {
    public string Name { get; set; }
    public decimal Percent { get; set; }
  }

  public enum PaymentType
  {
    None = 0,
    Ifsc = 1,
    Swift = 2
  }

  public class StopOnFirstFailureValidator<T> : AbstractValidator<T>
  {
    protected readonly string text = @"^[a-zA-Z0-9\s.\-,@]+$";
    protected readonly string _decimal = @"^\d*(\.\d+)?$";
    protected readonly string email = @"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+(?:[A-Z]{2}|com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum|in|co\.in|edu)\b$";
    protected readonly string phone = @"^(\+\d{1,3} )?\d{2,4}[\- ]?\d{3,4}[\- ]?\d{4}$";
    protected readonly string zip = @"^\d{5,6}([\-]?\d{4})?$";
    protected readonly string percent = @"^(100(?:\.0{1,2})?|0{0,2}?\.[\d]{1,2}|[\d]{1,2}(?:\.[\d]{1,2})?)$";
    protected readonly string integer = @"^\d{1,6}$";
    protected readonly RegexOptions options = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline;
    public StopOnFirstFailureValidator()
    {
      CascadeMode = CascadeMode.StopOnFirstFailure;
    }
  }

  public class CompanyValidator : StopOnFirstFailureValidator<Company>
  {
    public CompanyValidator()
    {
      RuleFor(company => company.Address).SetValidator(new AddressValidator());
      RuleFor(company => company.Cheque).SetValidator(new PaymentTypeValidator(PaymentType.None));
      RuleFor(company => company.Ifsc).SetValidator(new PaymentTypeValidator(PaymentType.Ifsc));
      RuleFor(company => company.Swift).SetValidator(new PaymentTypeValidator(PaymentType.Swift));
      RuleFor(company => company.Comments).Matches(this.text, this.options).When(company => !string.IsNullOrWhiteSpace(company.Comments));
      RuleFor(company => company.CompanyName).NotEmpty().Matches(this.text, this.options);
      RuleFor(company => company.Currency).NotEmpty();
      RuleFor(company => company.Logo).NotEmpty();
      RuleFor(company => company.Support).SetValidator(new SupportValidator());
      RuleFor(company => company.Tax).SetValidator(new CompanyTaxValidator());

      //RuleFor(company => company.Id)
      //  .MustAsync(async (s, token) => await repository.GetById(s) != null)
      //  .When(company => !string.IsNullOrWhiteSpace(company.Id));
    }
  }

  public class CompanyTaxValidator : StopOnFirstFailureValidator<CompanyTax>
  {
    public CompanyTaxValidator()
    {
      RuleFor(tax => tax.Cin).NotEmpty().Matches(this.text, this.options);
      RuleFor(tax => tax.Pan).NotEmpty().Matches(this.text, this.options);
      RuleFor(tax => tax.ServiceTaxNo).NotEmpty().Matches(this.text, this.options);
      RuleFor(tax => tax.Tin).NotEmpty().Matches(this.text, this.options);
    }
  }

  public class SupportValidator : StopOnFirstFailureValidator<Support>
  {
    public SupportValidator()
    {
      RuleFor(support => support.Email).NotEmpty().Matches(this.email, this.options);
      RuleFor(support => support.Phone).NotEmpty().Matches(this.phone, this.options);
    }
  }

  public class PaymentTypeValidator : StopOnFirstFailureValidator<Bank>
  {
    public PaymentTypeValidator(PaymentType paymentType)
    {
      RuleFor(bank => bank.BankName).NotEmpty().Matches(this.text, this.options);
      When(Predicate(paymentType), () =>
      {
        RuleFor(bank => bank.AccountNo).NotEmpty().Matches(this.text, this.options);
        RuleFor(bank => bank.Beneficiary).NotEmpty().Matches(this.text, this.options);
        RuleFor(bank => bank.Branch).NotEmpty().Matches(this.text, this.options);
        RuleFor(bank => bank.Code).NotEmpty().Matches(this.text, this.options);
      });
    }

    private static Func<Bank, bool> Predicate(PaymentType paymentType)
    {
      return bank => paymentType == PaymentType.Ifsc || paymentType == PaymentType.Swift;
    }
  }

  public class AddressValidator : StopOnFirstFailureValidator<Address>
  {
    public AddressValidator()
    {
      RuleFor(address => address.Address1).NotEmpty().Matches(this.text, this.options);
      RuleFor(address => address.City).NotEmpty().Matches(this.text, this.options);
      RuleFor(address => address.Address2).NotEmpty().Matches(this.text, this.options);
      RuleFor(address => address.CountryCode).NotEmpty().Matches(this.text, this.options);
      RuleFor(address => address.State).NotEmpty().Matches(this.text, this.options);
      RuleFor(address => address.Zip).NotEmpty().Matches(this.zip, this.options);
      RuleFor(address => address.Email).NotEmpty().Matches(this.email, this.options);
      RuleFor(address => address.Phone).NotEmpty().Matches(this.phone, this.options);
    }
  }

  public class CustomerValidator : StopOnFirstFailureValidator<Customer>
  {
    public CustomerValidator()
    {
      RuleFor(customer => customer.Comments).Matches(this.text, this.options).When(customer => !string.IsNullOrWhiteSpace(customer.Comments));
      RuleFor(customer => customer.ContactName).NotEmpty().Matches(this.text, this.options);
      RuleFor(customer => customer.Currency).NotEmpty();
      RuleFor(customer => customer.CustomerName).NotEmpty().Matches(this.text, this.options);
      RuleFor(customer => customer.Taxes).SetCollectionValidator(new CustomerTaxValidator());
      RuleFor(customer => customer.Address).SetValidator(customer => new AddressValidator());
      //.SetValidator(new AddressValidator());

      //RuleFor(customer => customer.Id)
      //  .MustAsync(async (s, token) => await repository.GetById(s) != null)
      //  .When(customer => !string.IsNullOrWhiteSpace(customer.Id));
    }
  }

  public class CustomerTaxValidator : StopOnFirstFailureValidator<Tax>
  {
    public CustomerTaxValidator()
    {
      RuleFor(tax => tax.Name).NotEmpty().Matches(this.text, this.options).WithMessage("tax name must be a valid string");
      RuleFor(tax => tax.Percent).NotEmpty().WithMessage("tax percent must be a valid decimal number > 0 and <=100").GreaterThan(0).WithMessage("tax percent must be a valid decimal number > 0 and <=100").LessThanOrEqualTo(100).WithMessage("tax percent must be a valid decimal number > 0 and <=100").WithMessage("tax percent must be a valid decimal number > 0 and <=100");
    }
  }

  public class ProductValidator : StopOnFirstFailureValidator<Product>
  {
    public ProductValidator()
    {
      RuleFor(product => product.ProductName).NotEmpty().Matches(this.text, this.options);
      RuleFor(product => product.Price).NotEmpty().GreaterThan(0);
      RuleFor(product => product.Comments).Matches(this.text, this.options).When(product => !string.IsNullOrWhiteSpace(product.Comments));
      RuleFor(product => product.CountryCode).NotEmpty();
      RuleFor(product => product.Currency).NotEmpty();
      RuleFor(product => product.Quantity).NotEmpty().GreaterThan(0);

      //RuleFor(product => product.Id)
      //  .MustAsync(async (s, token) => await repository.GetById(s) != null)
      //  .When(product => !string.IsNullOrWhiteSpace(product.Id));
    }
  }

  public class PaymentValidator : StopOnFirstFailureValidator<Payment>
  {
    public PaymentValidator()
    {
      RuleFor(payment => payment.Amount).NotEmpty().GreaterThan(0);
      RuleFor(payment => payment.Comments).Matches(this.text, this.options).When(payment => !string.IsNullOrWhiteSpace(payment.Comments));
      RuleFor(payment => payment.Currency).NotEmpty();
      RuleFor(payment => payment.InvoiceId).NotEmpty().Matches(this.text, this.options);
      //RuleFor(payment => payment.InvoiceNo).Matches("").When(payment => !string.IsNullOrWhiteSpace(payment.InvoiceNo));
      RuleFor(payment => payment.PaymentDate).NotEmpty();

      //RuleFor(payment => payment.Id)
      //  .MustAsync(async (s, token) => await repository.GetById(s) != null)
      //  .When(payment => !string.IsNullOrWhiteSpace(payment.Id));
    }
  }

  public class InvoiceValidator : StopOnFirstFailureValidator<Invoice>
  {
    public InvoiceValidator()
    {
      RuleFor(invoice => invoice.Adjustments).GreaterThanOrEqualTo(0);
      RuleFor(invoice => invoice.BalanceCarryForward).GreaterThanOrEqualTo(0);
      RuleFor(invoice => invoice.BillDate).NotEmpty();
      RuleFor(invoice => invoice.Bills).NotNull().SetCollectionValidator(new BillValidator());
      RuleFor(invoice => invoice.CompanyId).NotEmpty().Matches(this.text, this.options);
      RuleFor(invoice => invoice.Currency).NotEmpty();
      //RuleFor(invoice => invoice.CurrentCharges).NotEmpty().Must(arg => true);
      RuleFor(invoice => invoice.CustomerId).NotEmpty().Matches(this.text, this.options);
      RuleFor(invoice => invoice.DueDate).NotEmpty();
      RuleFor(invoice => invoice.EndDate).NotEmpty();
      RuleFor(invoice => invoice.PreviousAmt).GreaterThanOrEqualTo(0);
      RuleFor(invoice => invoice.StartDate).NotEmpty();
      RuleFor(invoice => invoice.PreviousPayment).GreaterThanOrEqualTo(0);

      //RuleFor(invoice => invoice.Id)
      //  .MustAsync(async (s, token) => await repository.GetById(s) != null)
      //  .When(invoice => !string.IsNullOrWhiteSpace(invoice.Id));
      //RuleFor(invoice => invoice.TotalDue).Must(arg => true);
    }
  }

  public class BillValidator : StopOnFirstFailureValidator<Bill>
  {
    public BillValidator()
    {
      RuleFor(bill => bill.BillingFrequency).NotEmpty().MustBeValidEnum();
      RuleFor(bill => bill.EndDate).NotEmpty();
      RuleFor(bill => bill.Price).NotEmpty().GreaterThanOrEqualTo(0);
      RuleFor(bill => bill.ProductId).NotEmpty().Matches(this.text, this.options);
      RuleFor(bill => bill.Quantity).NotEmpty().GreaterThan(0);
      RuleFor(bill => bill.StartDate).NotEmpty();
    }
  }

  public static class RuleBuilderExtentions
  {
    public static IRuleBuilderOptions<T, TProperty> MustBeValidEnum<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
    {
      return ruleBuilder.SetValidator(new MustBeValidEnumValidator<TProperty>());
    }
  }

  public class MustBeValidEnumValidator<T> : PropertyValidator
  {
    public MustBeValidEnumValidator() : base($"Property {{PropertyName}} is not valid enum ({typeof(T)})")
    {
    }

    protected override bool IsValid(PropertyValidatorContext context)
    {
      Type enumType = typeof(BillingFrequency);
      BillingFrequency frequency = (BillingFrequency)context.PropertyValue;
      return Enum.IsDefined(enumType, frequency);
    }
  }
}