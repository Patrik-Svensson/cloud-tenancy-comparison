using System;

public interface ISettingsProvider
{ 
    string GetDisplayName();

    string GetConnectionString();
}
