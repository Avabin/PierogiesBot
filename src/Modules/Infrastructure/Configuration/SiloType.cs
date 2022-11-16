using System.Collections.Immutable;

namespace Infrastructure.Configuration;

public record SiloType(ImmutableList<Type> ExcludedGrains);