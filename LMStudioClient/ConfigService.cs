using Microsoft.Extensions.Options;

namespace LMStudioClient;
public class ConfigService
{
    private LMStudioConfig _config;

    public ConfigService(IOptions<LMStudioConfig> config)
    {
        _config = config.Value;
    }

    public LMStudioConfig Get() => _config;

    public void Update(LMStudioConfig newConfig)
    {
        _config = newConfig;
    }

    public void UpdateModel(string model)
    {
        _config.Llm = model;
    }
}
