﻿using MediatR;

namespace Captive.Applications.Bank.Command.CreateBankInfo
{
    public class CreateBankInfoCommand:IRequest<Unit>
    {
        public int? Id { get; set; }
        public required string BankName { get; set; }

        public required string ShortName { get; set; }
        public required string Description { get; set; }
    }
}
