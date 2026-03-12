using System;
using System.Collections.Generic;

namespace PersonalAccount.Data.Models;

public partial class ImportSetting
{
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    public int SourceType { get; set; }

    public string Description { get; set; } = null!;

    public long StartPosition { get; set; }

    public int BatchSize { get; set; }

    public virtual Organization Organization { get; set; } = null!;
}
