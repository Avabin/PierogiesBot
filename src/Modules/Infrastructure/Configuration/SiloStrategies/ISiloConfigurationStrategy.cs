﻿namespace Infrastructure.Configuration.SiloStrategies;

public interface ISiloConfigurationStrategy
{
    void Apply(ISiloBuilder builder);
}