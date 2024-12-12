using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AISmart.Dto;
using AISmart.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
    
    public async Task SendMessageAsync(string sendUser,string chatId, string message,ReplyParamDto? replyParam = null)
    {
        String Token = GetAccount(sendUser);
        string url = $"https://api.telegram.org/bot{Token}/sendMessage";
        
        var parametersList = new List<KeyValuePair<string, string>>
        {
            new("chat_id", chatId),
            new("text", message)
        };
        if (replyParam != null)
        {
            parametersList.Add(new KeyValuePair<string, string>("reply_parameters",
                JsonConvert.SerializeObject(replyParam,new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                })));
        }
        FormUrlEncodedContent parameters = new(parametersList);


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

    public async Task SendPhotoAsync(string sendUser,PhotoParamsDto photoParamsDto)
    {
        var token = GetAccount(sendUser);
        var url = $"https://api.telegram.org/bot{token}/sendPhoto";
        var paramsJson = JsonConvert.SerializeObject(photoParamsDto, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });
        var content = new StringContent(paramsJson, Encoding.UTF8, "application/json");
        try
        {
            var response = await new HttpClient().PostAsync(url, content);
                
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogInformation(responseBody);
        }
        catch (HttpRequestException e)
        {
            _logger.LogError($"request error: {e.Message}");
        }
    }
}