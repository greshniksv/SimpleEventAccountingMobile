﻿namespace SimpleEventAccountingMobile.Dtos
{
    public class ClientGroupDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<Guid> Clients { get; set; }
    }

}
