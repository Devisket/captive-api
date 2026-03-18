using MediatR;

namespace Captive.Applications.CheckInventory.Commands.InitiateCheckInventory
{
    public class InitiateCheckInventoryCommandHandler : IRequestHandler<InitiateCheckInventoryCommand, Unit>
    {
        public Task<Unit> Handle(InitiateCheckInventoryCommand request, CancellationToken cancellationToken)
        {
            throw new NotSupportedException("InitiateCheckInventory is no longer supported. Create check inventories directly via the AddCheckInventory endpoint.");
        }
    }
}
