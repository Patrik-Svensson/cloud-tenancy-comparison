using System;

public interface ISettingsProvider
{
     object Get(ITenantIdProvider idProvider);

}
