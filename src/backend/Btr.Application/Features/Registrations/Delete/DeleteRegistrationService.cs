using Btr.Application.Abstractions.Persistence;

namespace Btr.Application.Features.Registrations.Delete;

public sealed class DeleteRegistrationService
{
    private readonly IRegistrationRepository _registrationRepository;

    public DeleteRegistrationService(IRegistrationRepository registrationRepository)
    {
        _registrationRepository = registrationRepository;
    }

    public async Task<DeleteRegistrationResult> ExecuteAsync(Guid registrationId, CancellationToken cancellationToken = default)
    {
        if (registrationId == Guid.Empty)
        {
            return DeleteRegistrationResult.Fail("RegistrationId is required.");
        }

        var existing = await _registrationRepository.GetByIdAsync(registrationId, cancellationToken);
        if (existing is null)
        {
            return DeleteRegistrationResult.Fail("Registration not found.");
        }

        await _registrationRepository.DeleteAsync(existing, cancellationToken);
        return DeleteRegistrationResult.Ok();
    }
}
