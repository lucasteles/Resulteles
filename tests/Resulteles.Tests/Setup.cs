using FsCheck;

namespace Resulteles.Tests;

public record OkResult<TOk, TError>(Result<TOk, TError> Item);

public record ErrorResult<TOk, TError>(Result<TOk, TError> Item);

public class Generators
{
    protected Generators() { }

    public static Arbitrary<Result<TOk, TError>> GenerateResult<TOk, TError>() =>
        Gen.OneOf(
                Arb.From<TOk>().Generator.Select(Result.Ok<TOk, TError>),
                Arb.From<TError>().Generator.Select(Result.Error<TOk, TError>)
            )
            .ToArbitrary();

    public static Arbitrary<OkResult<TOk, TError>> GenerateOkResult<TOk, TError>() =>
        Arb.From<TOk>().Generator.Select(v => new OkResult<TOk, TError>(Result.Ok<TOk, TError>(v))).ToArbitrary();

    public static Arbitrary<ErrorResult<TOk, TError>> GenerateErrorResult<TOk, TError>() =>
        Arb.From<TError>().Generator.Select(v => new ErrorResult<TOk, TError>(Result.Error<TOk, TError>(v)))
            .ToArbitrary();
}

[Serializable, AttributeUsage(AttributeTargets.Method)]
public sealed class PropertyTestAttribute : FsCheck.NUnit.PropertyAttribute
{
    public PropertyTestAttribute() => QuietOnSuccess = true;
}

[SetUpFixture]
public class SetupFixture
{
    [OneTimeSetUp]
    public void OneTimeSetup() => FsCheck.Arb.Register<Generators>();
}
