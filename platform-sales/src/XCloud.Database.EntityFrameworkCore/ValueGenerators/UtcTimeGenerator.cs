using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;

namespace XCloud.Database.EntityFrameworkCore.ValueGenerators;

public class NullableUtcTimeGenerator : ValueGenerator<DateTime?>
{
    public override bool GeneratesTemporaryValues => true;

    public override DateTime? Next(EntityEntry entry) => DateTime.UtcNow;
}

public class UtcTimeGenerator : ValueGenerator<DateTime>
{
    public override bool GeneratesTemporaryValues => true;

    public override DateTime Next(EntityEntry entry) => DateTime.UtcNow;
}