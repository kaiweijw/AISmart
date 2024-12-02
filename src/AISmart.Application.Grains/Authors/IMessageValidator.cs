namespace AISmart.Application.Grains.Authors;

public interface IMessageValidator : IGrainWithGuidKey
{
    Task<bool> IsOffensive(string message);
}