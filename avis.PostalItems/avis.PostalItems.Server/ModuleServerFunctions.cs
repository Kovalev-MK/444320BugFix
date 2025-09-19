using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Dadata;
using Dadata.Model;

namespace avis.PostalItems.Server
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Найти почтовые отправления, подходящие по критериям поиска.
    /// </summary>
    /// <returns>Список почтовых отправлений.</returns>
    [Remote(IsPure = true)]
    public static IQueryable<avis.PostalItems.ILetterComponentDocument> SearchLetterComponents(int typeIndex, DateTime? createdFrom, DateTime? createdTo, Sungero.Company.IEmployee author)
    {
      var components = avis.PostalItems.LetterComponentDocuments.GetAll();
      
      switch(typeIndex)
      {
        case 0:
          var notification = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.PostalItemNotification);
          if (notification != null)
          {
            components = components.Where(x => x.DocumentKind.Equals(notification));
          }
          break;
        case 1:
          var check = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.PostalItemCheck);
          if (check != null)
          {
            components = components.Where(x => x.DocumentKind.Equals(check));
          }
          break;
        case 2:
          var envelope = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Constants.Module.PostalItemEnvelope);
          if (envelope != null)
          {
            components = components.Where(x => x.DocumentKind.Equals(envelope));
          }
          break;
      }
      
      if (createdFrom != null)
        components = components.Where(x => x.Created >= createdFrom);
      
      if (createdTo != null)
      {
        createdTo = createdTo.Value.Date.EndOfDay();
        components = components.Where(x => x.Created <= createdTo);
      }
      
      if (author != null)
        components = components.Where(x => x.Author.Equals(author));
      
      return components;
    }
    
    /// <summary>
    /// Найти почтовые отправления, подходящие по критериям поиска.
    /// </summary>
    /// <returns>Список почтовых отправлений.</returns>
    [Remote(IsPure = true)]
    public static IQueryable<avis.PostalItems.IPostalItem> SearchPostalItems(DateTime? createdFrom, DateTime? createdTo, Sungero.Company.IEmployee author)
    {
      var postalItems = avis.PostalItems.PostalItems.GetAll();
      
      if (createdFrom != null)
        postalItems = postalItems.Where(x => x.DateCreated >= createdFrom);
      
      if (createdTo != null)
      {
        createdTo = createdTo.Value.Date.EndOfDay();
        postalItems = postalItems.Where(x => x.DateCreated <= createdTo);
      }
      
      if (author != null)
        postalItems = postalItems.Where(x => x.Employee.Equals(author));
      
      return postalItems;
    }
    
    // Добавлено avis.
    
    /// <summary>
    /// Получить почтовый индекс по адресу (из сервиса dadata).
    /// </summary>
    /// <param name="address">Адрес.</param>
    /// <returns>Почтовый индекс.</returns>
    private static string GetPostalCode(string address)
    {
      // Получаем токены.
      var settings = lenspec.Etalon.Integrationses.GetAll(s => s.Code == lenspec.Etalon.Module.Integration.PublicConstants.Module.DadataCode).FirstOrDefault();
      if (settings == null || string.IsNullOrEmpty(settings.ConnectionParams))
        throw AppliedCodeException.Create(avis.PostalItems.Resources.ErrorConnectionSettingsNotFound);
      
      // Расшифровываем и разделяем на токен и секретный ключ.
      var settingsString = Encryption.Decrypt(settings.ConnectionParams);
      var connectionString = settingsString.Split(';');
      if (string.IsNullOrEmpty(connectionString[0]))
        throw AppliedCodeException.Create(avis.PostalItems.Resources.ErrorWrongToken);
      
      var token = connectionString[0];
      var secret = connectionString[1];
      var api = new CleanClientAsync(token, secret);
      var result = api.Clean<Address>(address);
      
      return result.Result.postal_code;
    }
    
    /// <summary>
    /// Получить почтовый индекс по адресу (из сервиса dadata).
    /// </summary>
    /// <param name="address">Адрес.</param>
    /// <returns>Результаты запроса на получение индекса.</returns>
    [Public]
    public static Structures.PostalItem.IPostalCodeReceiveResult TryGetPostalCode(string address)
    {
      var result = Structures.PostalItem.PostalCodeReceiveResult.Create();
      if (string.IsNullOrEmpty(address))
      {
        result.IsSuccess = true;
        result.Code = string.Empty;
        return result;
      }
      
      try
      {
        result.Code = GetPostalCode(address);
        result.IsSuccess = true;
      }
      catch (Exception ex)
      {
        result.IsSuccess = false;
        result.Error = avis.PostalItems.PostalItems.Resources.PostalCodeCalculationErrorFormat(ex.Message);
        result.Code = string.Empty;
      }
      return result;
    }
    
    // Конец добавлено avis.
  }
}