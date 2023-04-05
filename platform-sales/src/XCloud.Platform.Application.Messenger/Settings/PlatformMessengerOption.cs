using Volo.Abp.Application.Dtos;

namespace XCloud.Platform.Application.Messenger.Settings;

public class PlatformMessengerOption : IEntityDto
{
    public PlatformMessengerOption()
    {
        //
    }

    public bool Enabled { get; set; } = false;
}