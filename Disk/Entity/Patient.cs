using System;
using System.Collections.Generic;

namespace Disk.Entity;

public partial class Patient
{
    public int PatId { get; set; }

    public string PatName { get; set; } = null!;

    public string PatSurname { get; set; } = null!;

    public string? PatPatronymic { get; set; }

    public int PatAddress { get; set; }

    public string PatDateOfBirth { get; set; } = null!;

    public string PatPhoneMobile { get; set; } = null!;

    public string? PatPhoneHome { get; set; }

    public virtual ICollection<Card> Cards { get; set; } = new List<Card>();

    public virtual Address PatAddressNavigation { get; set; } = null!;

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
