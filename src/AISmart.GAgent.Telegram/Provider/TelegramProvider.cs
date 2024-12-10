using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AISmart.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace AISmart.Provider;

public class TelegramProvider : ITelegramProvider,ISingletonDependency
{
    private readonly ILogger<TelegramProvider> _logger;
    private readonly IOptionsMonitor<TelegramOptions> _telegramOptions;

    public TelegramProvider(ILogger<TelegramProvider> logger,IOptionsMonitor<TelegramOptions> telegramOptions)
    {
        _logger = logger;
        _telegramOptions = telegramOptions;
    }
    
    public async Task SendMessageAsync(string sendUser,string chatId, string message)
    {
        String Token = GetAccount(sendUser);
        string url = $"https://api.telegram.org/bot{Token}/sendMessage";

        var parameters = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("chat_id", chatId),
            new KeyValuePair<string, string>("text", message)
        });

        try
        {
            HttpResponseMessage response = await new HttpClient().PostAsync(url, parameters);
                
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogInformation(responseBody);
        }
        catch (HttpRequestException e)
        {
            _logger.LogError($"request error: {e.Message}");
        }
    }
    
    public async Task<string> GetUpdatesAsync(string sendUser)
    {
        String Token = GetAccount(sendUser);

        string url = $"https://api.telegram.org/bot{Token}/GetUpdates";
        try
        {
            HttpResponseMessage response = await new HttpClient().GetAsync(url);
                
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogInformation(responseBody);
            return responseBody;
        }
        catch (HttpRequestException e)
        {
            _logger.LogError($"request error: {e.Message}");
        }

        return null;
    }
    
    private string GetAccount(string accountName)
    {
        var optionExists = _telegramOptions.CurrentValue.AccountDictionary.TryGetValue(accountName, out var account);
        if (!optionExists)
        {
            throw new UserFriendlyException($"Telegram Account {accountName} not found");
        }
        return account;
    }
}