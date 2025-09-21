using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Etalon.Person;

using PersonExcelCells = lenspec.Etalon.PublicConstants.Parties.Person.ExcelCells;
using ObjectExcelCells = lenspec.EtalonDatabooks.PublicConstants.ObjectAnSale.ExcelCells;

namespace lenspec.Etalon.Server
{
  // Добавлено avis.
  
  partial class PersonFunctions
  {
    
    [Public]
    /// <summary>
    /// Проверить наличие обязательных полей для выгрузки записи в 1С.
    /// </summary>
    /// <returns>Сообщение об ошибке.</returns>
    public string CheckRequiredFieldsForExport()
    {
      var errors = new List<string>();
      var emptyFields = new List<string>();
      if (string.IsNullOrEmpty(_obj.TIN) || string.IsNullOrWhiteSpace(_obj.TIN))
        emptyFields.Add(_obj.Info.Properties.TIN.LocalizedName);
      
      if (string.IsNullOrEmpty(_obj.LastName) || string.IsNullOrWhiteSpace(_obj.LastName))
        emptyFields.Add(_obj.Info.Properties.LastName.LocalizedName);
      
      if (string.IsNullOrEmpty(_obj.FirstName) || string.IsNullOrWhiteSpace(_obj.FirstName))
        emptyFields.Add(_obj.Info.Properties.FirstName.LocalizedName);
      
      if (_obj.Sex == null)
        emptyFields.Add(_obj.Info.Properties.Sex.LocalizedName);
      
      if (!_obj.DateOfBirth.HasValue)
        emptyFields.Add(_obj.Info.Properties.DateOfBirth.LocalizedName);
      
      if (string.IsNullOrEmpty(_obj.LegalAddress) || string.IsNullOrWhiteSpace(_obj.LegalAddress))
        emptyFields.Add(_obj.Info.Properties.LegalAddress.LocalizedName);
      
      if (string.IsNullOrEmpty(_obj.PostalAddress) || string.IsNullOrWhiteSpace(_obj.PostalAddress))
        emptyFields.Add(_obj.Info.Properties.PostalAddress.LocalizedName);
      
      if (emptyFields.Any())
        errors.Add(lenspec.Etalon.People.Resources.PersonHasEmptyRequiredFieldsFormat(string.Join(", ", emptyFields)));
      
      if (_obj.IsLawyeravis != true && _obj.IsClientBuyersavis != true &&  _obj.IsClientOwnersavis != true && _obj.IsEmployeeGKavis != true && _obj.IsOtheravis != true)
        errors.Add(lenspec.Etalon.People.Resources.PersonTypeNotFilledIn);
      
      return string.Join(Environment.NewLine, errors);
    }
    
    /// <summary>
    /// Получить ошибку пустого поля.
    /// </summary>
    /// <param name="isHasHeaders">Признак наличия заголовков у таблицы.</param>
    /// <param name="excelHeaders">Заголовки таблицы.</param>
    /// <param name="excelColumnIndex">Индекс столбца.</param>
    /// <param name="rxColumnName">Название параметра в RX.</param>
    /// <returns>Сообщение об ошибке.</returns>
    private static string GetEmptyFieldErrorMessage(bool isHasHeaders, string[] excelHeaders, int excelColumnIndex, string rxColumnName)
    {
      return isHasHeaders ? lenspec.Etalon.People.Resources.FieldIsEmptyErrorMessageFormat(excelHeaders[excelColumnIndex]) : lenspec.Etalon.People.Resources.FieldIsEmptyErrorMessageFormat(rxColumnName);
    }
    
    /// <summary>
    /// Получить ошибку некорректного поля.
    /// </summary>
    /// <param name="isHasHeaders">Признак наличия заголовков у таблицы.</param>
    /// <param name="excelHeaders">Заголовки таблицы.</param>
    /// <param name="excelColumnIndex">Индекс столбца.</param>
    /// <param name="rxColumnName">Название параметра в RX.</param>
    /// <returns>Сообщение об ошибке.</returns>
    private static string GetIncorrectFieldErrorMessage(bool isHasHeaders, string[] excelHeaders, int excelColumnIndex, string rxColumnName)
    {
      return isHasHeaders ? lenspec.Etalon.People.Resources.IncorrectFieldErrorMessageFormat(excelHeaders[excelColumnIndex]) : lenspec.Etalon.People.Resources.IncorrectFieldErrorMessageFormat(rxColumnName);
    }
    
    /// <summary>
    /// Получить список ошибок в виде одной строки.
    /// </summary>
    /// <param name="errorList">Список ошибок.</param>
    /// <param name="logPrefix">Префикс для логгирования.</param>
    /// <returns></returns>
    private static string GetFullErrorFromErrorList(List<string> errorList, string logPrefix)
    {
      var error = lenspec.Etalon.People.Resources.ErrorMessageTemplateFormat(string.Join("; ", errorList.ToArray()));
      Logger.ErrorFormat("{0}{1}", logPrefix, error);
      return error;
    }
    
    /// <summary>
    /// Создать и заполнить Помещение на продажу данными из Excel.
    /// </summary>
    /// <param name="headers">Заголовки таблицы.</param>
    /// <param name="row">Строка таблицы.</param>
    /// <returns>Сообщение о статусе импорта.</returns>
    private static string CreateObjectAnSale(string[] headers, string[] row)
    {
      var logPrefix = "Avis - ImportPersonsAndObjectAnSale - objectAnSale - ";
      
      try
      {
        var errorList = new List<string>();
        var isHasHeaders = headers.Any();
        
        #region Проверка заполненности обязательных полей в Excel

        if (string.IsNullOrEmpty(row[ObjectExcelCells.NumberRoomMail]))
          errorList.Add(GetEmptyFieldErrorMessage(isHasHeaders, headers, ObjectExcelCells.NumberRoomMail, EtalonDatabooks.ObjectAnSales.Info.Properties.NumberRoomMail.LocalizedName));

        long objectAnProjectId;
        var isCorrectId = long.TryParse(row[ObjectExcelCells.ObjectAnProjectId], out objectAnProjectId);
        
        if (string.IsNullOrEmpty(row[ObjectExcelCells.ObjectAnProjectId]))
          errorList.Add(GetEmptyFieldErrorMessage(isHasHeaders, headers, ObjectExcelCells.ObjectAnProjectId, EtalonDatabooks.ObjectAnSales.Info.Properties.ObjectAnProject.LocalizedName));
        else if (!isCorrectId)
          errorList.Add(GetIncorrectFieldErrorMessage(isHasHeaders, headers, ObjectExcelCells.ObjectAnProjectId, EtalonDatabooks.ObjectAnSales.Info.Properties.ObjectAnProject.LocalizedName));
        
        long purposeOfPremisesId;
        isCorrectId = long.TryParse(row[ObjectExcelCells.PurposeOfPremisesId], out purposeOfPremisesId);
        
        if (string.IsNullOrEmpty(row[ObjectExcelCells.PurposeOfPremisesId]))
          errorList.Add(GetEmptyFieldErrorMessage(isHasHeaders, headers, ObjectExcelCells.PurposeOfPremisesId, EtalonDatabooks.ObjectAnSales.Info.Properties.PurposeOfPremises.LocalizedName));
        else if (!isCorrectId)
          errorList.Add(GetIncorrectFieldErrorMessage(isHasHeaders, headers, ObjectExcelCells.PurposeOfPremisesId, EtalonDatabooks.ObjectAnSales.Info.Properties.PurposeOfPremises.LocalizedName));
        
        if (errorList.Any())
          return GetFullErrorFromErrorList(errorList, logPrefix);
        
        #endregion
        
        #region Проверка наличия сущностей в RX
        
        var objectAnProject = lenspec.EtalonDatabooks.ObjectAnProjects.GetAll(x => x.Id == objectAnProjectId).FirstOrDefault();
        if (objectAnProject == null)
          errorList.Add(lenspec.Etalon.People.Resources.EntityIsNotFoundErrorMessageFormat(EtalonDatabooks.ObjectAnSales.Info.Properties.ObjectAnProject.LocalizedName, objectAnProjectId));
        
        var purposeOfPremises = lenspec.EtalonDatabooks.PurposeOfPremiseses.GetAll(x => x.Id == purposeOfPremisesId).FirstOrDefault();
        if (purposeOfPremises == null)
          errorList.Add(lenspec.Etalon.People.Resources.EntityIsNotFoundErrorMessageFormat(EtalonDatabooks.ObjectAnSales.Info.Properties.PurposeOfPremises.LocalizedName, purposeOfPremisesId));

        if (errorList.Any())
          return GetFullErrorFromErrorList(errorList, logPrefix);
        
        #endregion
        
        #region Создание нового Помещения объектов продажи
        
        var objectAnSale = lenspec.EtalonDatabooks.ObjectAnSales.Create();
        objectAnSale.Settlement = lenspec.EtalonDatabooks.ObjectAnSale.Settlement.NotRequired;
        objectAnSale.EditSquere = 0;
        objectAnSale.EditPrice = 0;
        objectAnSale.NumberRoomMail = row[ObjectExcelCells.NumberRoomMail];
        objectAnSale.PurposeOfPremises = purposeOfPremises;
        // ИСП и Адрес заполнятся автоматически.
        objectAnSale.ObjectAnProject = objectAnProject;
        objectAnSale.Save();
        
        var message = lenspec.Etalon.People.Resources.CreatedEntityMessageFormat(EtalonDatabooks.ObjectAnSales.Info.LocalizedName, objectAnSale.Id, Hyperlinks.Get(objectAnSale));
        Logger.DebugFormat("{0}{1}", logPrefix, message);
        return message;
        
        #endregion
      }
      catch(Exception ex)
      {
        Logger.Error(ex, logPrefix);
        return lenspec.Etalon.People.Resources.NotCreatedEntityMessageFormat(EtalonDatabooks.ObjectAnSales.Info.LocalizedName, ex.Message);
      }
    }
    
    /// <summary>
    /// Создать и заполнить/обновить существующую Персону данными из Excel.
    /// </summary>
    /// <param name="headers">Заголовки таблицы.</param>
    /// <param name="row">Строка таблицы.</param>
    /// <returns>Сообщение о статусе импорта.</returns>
    private static string CreateOrUpdatePerson(string[] headers, string[] row)
    {
      var logPrefix = "Avis - ImportPersonsAndObjectAnSale - person - ";
      
      try
      {
        var errorList = new List<string>();
        var isHasHeaders = headers.Any();
        
        #region Проверка заполненности обязательных полей в Excel

        if (string.IsNullOrEmpty(row[PersonExcelCells.LastName]))
          errorList.Add(GetEmptyFieldErrorMessage(isHasHeaders, headers, PersonExcelCells.LastName, Etalon.People.Info.Properties.LastName.LocalizedName));
        
        if (string.IsNullOrEmpty(row[PersonExcelCells.FirstName]))
          errorList.Add(GetEmptyFieldErrorMessage(isHasHeaders, headers, PersonExcelCells.FirstName, Etalon.People.Info.Properties.FirstName.LocalizedName));
        
        DateTime dateOfBirth;
        var isDate = Calendar.TryParseDate(row[PersonExcelCells.DateOfBirth], out dateOfBirth);
        if (!string.IsNullOrEmpty(row[PersonExcelCells.DateOfBirth]) && !isDate)
          errorList.Add(GetIncorrectFieldErrorMessage(isHasHeaders, headers, PersonExcelCells.DateOfBirth, Etalon.People.Info.Properties.DateOfBirth.LocalizedName));
        
        if (errorList.Any())
          return GetFullErrorFromErrorList(errorList, logPrefix);
        
        #endregion
        
        #region Создание/обновление персоны
        
        var isCreated = false;
        var persons = Etalon.People.GetAll(x => row[PersonExcelCells.LastName] == x.LastName && row[PersonExcelCells.FirstName] == x.FirstName);
        
        if (isDate)
          persons = persons.Where(x => dateOfBirth == x.DateOfBirth);
        
        if (!string.IsNullOrEmpty(row[PersonExcelCells.MiddleName]))
          persons = persons.Where(x => row[PersonExcelCells.MiddleName] == x.MiddleName);
        
        var person = persons.FirstOrDefault();
        if (person == null)
        {
          person = Etalon.People.Create();
          person.LastName = row[PersonExcelCells.LastName];
          person.FirstName = row[PersonExcelCells.FirstName];
          
          if (!string.IsNullOrEmpty(row[PersonExcelCells.MiddleName]))
            person.MiddleName = row[PersonExcelCells.MiddleName];
          
          if (isDate)
            person.DateOfBirth = dateOfBirth;
          
          isCreated = true;
        }
        
        person.IsClientOwnersavis = true;
        
        if (row[PersonExcelCells.Sex] == lenspec.Etalon.People.Resources.IsMale)
          person.Sex = Etalon.Person.Sex.Male;
        else if (row[PersonExcelCells.Sex] == lenspec.Etalon.People.Resources.IsFemale)
          person.Sex = Etalon.Person.Sex.Female;
        
        person.Phones = row[PersonExcelCells.Phone];
        person.Email = row[PersonExcelCells.Email];
        person.PersonalAccountavis = row[PersonExcelCells.PersonalAccount];
        person.Save();
        
        var message = string.Empty;

        if (isCreated)
          message = lenspec.Etalon.People.Resources.CreatedEntityMessageFormat(Etalon.People.Info.LocalizedName, person.Id, Hyperlinks.Get(person));
        else
          message = lenspec.Etalon.People.Resources.UpdatedEntityMessageFormat(Etalon.People.Info.LocalizedName, person.Id, Hyperlinks.Get(person));
        
        Logger.DebugFormat("{0}{1}", logPrefix, message);
        return message;
        
        #endregion
      }
      catch(Exception ex)
      {
        Logger.Error(ex, logPrefix);
        return lenspec.Etalon.People.Resources.NotCreatedEntityMessageFormat(EtalonDatabooks.ObjectAnSales.Info.LocalizedName, ex.Message);
      }
    }
    
    /// <summary>
    /// Загрузить Собственников и Помещения из файла.
    /// </summary>
    /// <param name="content">Содержимое документа.</param>
    /// <returns>Сообщение об ошибке. Если успешно, то пустая строка.</returns>
    [Public]
    public static string ImportPersonsAndObjectAnSale(System.IO.Stream body)
    {
      var excelSheet = lenspec.EtalonDatabooks.PublicFunctions.Module.GetExcelSheet(body);
      var excelTableData = excelSheet.Table.Rows.Select(x => x.Data);
      var importResult = new List<string[]>();
      
      foreach(var row in excelTableData)
      {
        var rowResult = new string[2];
        rowResult[0] = CreateOrUpdatePerson(excelSheet.Headers.Data, row);
        rowResult[1] = CreateObjectAnSale(excelSheet.Headers.Data, row);
        
        importResult.Add(rowResult);
      }

      return lenspec.EtalonDatabooks.PublicFunctions.Module.InsertDataToExcel(body, importResult);
    }
    
    /// <summary>
    /// Отображение поля "Код инвест" в зависимости от условий.
    /// </summary>
    private void IsVisibleCodeInvest()
    {
      // Если стоит флаг Клиент покупатель дольщик, то включаем поле.
      if (_obj.IsClientBuyersavis == true)
      {
        _obj.State.Properties.CodeInvestavis.IsVisible = true;
        return;
      }
      
      // Отключаем поле.
      _obj.State.Properties.CodeInvestavis.IsVisible = false;
    }
    
    /// <summary>
    /// Отображение поля "Согласие на обработку перс. данных" в зависимости от условий.
    /// </summary>
    private void IsVisibleIsPersonalDataavis()
    {
      if (_obj.IsClientBuyersavis == true)
      {
        _obj.State.Properties.IsPersonalDataavis.IsVisible = true;
        return;
      }
      
      _obj.State.Properties.IsPersonalDataavis.IsVisible = false;
    }
    
    /// <summary>
    /// Разблокируем поля для чекбокса "Клиенты собственники".
    /// </summary>
    private void ActiveFromClient()
    {
      _obj.State.Properties.LastName.IsEnabled = true;
      _obj.State.Properties.FirstName.IsEnabled = true;
      _obj.State.Properties.MiddleName.IsEnabled = true;
      _obj.State.Properties.TIN.IsEnabled = true;
      _obj.State.Properties.Status.IsEnabled = true;
      _obj.State.Properties.INILA.IsEnabled = true;
      _obj.State.Properties.Code.IsEnabled = true;
      _obj.State.Properties.Sex.IsEnabled = true;
      _obj.State.Properties.Phones.IsEnabled = true;
      _obj.State.Properties.LegalAddress.IsEnabled = true;
      _obj.State.Properties.Phones.IsEnabled = true;
      _obj.State.Properties.Homepage.IsEnabled= true;
      _obj.State.Properties.Email.IsEnabled = true;
    }
    
    /// <summary>
    /// Разблокировать/Заблокировать вкладку адреса.
    /// </summary>
    /// <param name="isEnable"></param>
    private void AddressesIsEnable(bool isEnable)
    {
      _obj.State.Properties.Countryavis.IsEnabled = isEnable;
      _obj.State.Properties.Regionavis.IsEnabled = isEnable;
      _obj.State.Properties.Cityavis.IsEnabled = isEnable;
      _obj.State.Properties.HomeAddressavis.IsEnabled = isEnable;
      _obj.State.Properties.Indexavis.IsEnabled = isEnable;
    }
    
    /// <summary>
    /// В зависимости от установленных чек-боксов делаем обязательные поля.
    /// </summary>
    private void CheckBoxUpdate()
    {
      // Настраиваем доступность полей вкладки адресс.
      if (_obj.IsClientOwnersavis == true)
        AddressesIsEnable(false);
      else if (_obj.IsClientBuyersavis == true || _obj.IsEmployeeGKavis == true || _obj.IsOtheravis == true || _obj.IsLawyeravis == true)
        AddressesIsEnable(true);

      // Активируем доп поля.
      if (_obj.IsClientBuyersavis == true || _obj.IsEmployeeGKavis == true)
      {
        _obj.State.Properties.IsLawyeravis.IsEnabled = true;
        _obj.State.Properties.IsOtheravis.IsEnabled = true;
        _obj.State.Properties.Email.IsEnabled = true;
      }
      else
      {
        // Делаем доступными поля для поля для клиент собственники.
        if (_obj.IsOtheravis == true || _obj.IsLawyeravis == true || _obj.IsClientOwnersavis == true)
        {
          ActiveFromClient();
        }
      }
      
      // Разрешаем редактирование полей для роли "Полные права на управление архивом для управляющих компаний (ЖКХ)".
      var fullPermitArhivJKHGuid = lenspec.Etalon.Module.Parties.PublicConstants.Module.FullPermitArhivJKH;
      var fullPermitArhivJKHRole = Roles.GetAll(r => Equals(r.Sid, fullPermitArhivJKHGuid)).FirstOrDefault();
      if (fullPermitArhivJKHRole != null && Users.Current.IncludedIn(fullPermitArhivJKHRole))
        _obj.State.Properties.IsClientOwnersavis.IsEnabled = true;
    }
    
    /// <summary>
    /// Разрешаем редактирование полей для роли "Права на создание всех видов контрагентов".
    /// </summary>
    /// <param name="createCounterpartyRole">Роль, права на создание всех видов контрагентов.</param>
    [Public]
    public void IsCounterpartyRole(IRole createCounterpartyRole)
    {
      if (Users.Current.IncludedIn(createCounterpartyRole))
      {
        _obj.State.Properties.IsLawyeravis.IsEnabled = true;
        _obj.State.Properties.IsOtheravis.IsEnabled = true;
      }
      else
      {
        _obj.State.Properties.IsLawyeravis.IsEnabled = false;
        _obj.State.Properties.IsOtheravis.IsEnabled = false;
      }
    }
    
    /// <summary>
    /// Заблокировать все свойства карточки.
    /// </summary>
    [Public]
    public void BlockAllProperties()
    {
      // ЕСЛИ ТЫ АДМИН ОСТАВЛЯЕМ СТАНДАРТНУЮ КАРТОЧКУ ПЕРСОНЫ.
      if (Users.Current.IncludedIn(Roles.Administrators))
      {
        IsAdministrator();
        return;
      }
      
      _obj.State.Properties.FirstName.IsEnabled = false;
      _obj.State.Properties.LastName.IsEnabled = false;
      _obj.State.Properties.MiddleName.IsEnabled = false;
      _obj.State.Properties.Phones.IsEnabled = false;
      _obj.State.Properties.LegalAddress.IsEnabled = false;
      _obj.State.Properties.Sex.IsEnabled = false;
      _obj.State.Properties.INILA.IsEnabled = false;
      _obj.State.Properties.Cityavis.IsEnabled = false;
      _obj.State.Properties.Regionavis.IsEnabled = false;
      _obj.State.Properties.Sex.IsEnabled = false;
      _obj.State.Properties.Indexavis.IsEnabled = false;
      _obj.State.Properties.Email.IsEnabled = false;
      _obj.State.Properties.Code.IsEnabled = false;
      _obj.State.Properties.Homepage.IsEnabled = false;
      _obj.State.Properties.Status.IsEnabled = false;
      _obj.State.Properties.TIN.IsEnabled = false;
      _obj.State.Properties.HomeAddressavis.IsEnabled = false;
      _obj.State.Properties.Countryavis.IsEnabled = false;
      _obj.State.Properties.DateOfBirth.IsEnabled = false;
      
      // Видимость поля "Код инвест".
      IsVisibleCodeInvest();
      
      // Видимость поля "Согласие на обработку перс. Данных".
      IsVisibleIsPersonalDataavis();
      
      // В зависимости от установленных чек-боксов делаем обязательные поля.
      CheckBoxUpdate();
    }
    
    /// <summary>
    /// Разблокируем все поля для администратора.
    /// </summary>
    private void IsAdministrator()
    {
      _obj.State.Properties.IsClientBuyersavis.IsEnabled = true;
      _obj.State.Properties.IsEmployeeGKavis.IsEnabled = true;
      _obj.State.Properties.IsClientOwnersavis.IsEnabled = true;
      _obj.State.Properties.IsOtheravis.IsEnabled = true;
      _obj.State.Properties.IsLawyeravis.IsEnabled = true;
      _obj.State.Properties.Countryavis.IsEnabled = true;
      _obj.State.Properties.Regionavis.IsEnabled = true;
      _obj.State.Properties.Cityavis.IsEnabled = true;
      _obj.State.Properties.HomeAddressavis.IsEnabled = true;
      _obj.State.Properties.Indexavis.IsEnabled = true;
      _obj.State.Properties.PostalAddress.IsEnabled = true;
      _obj.State.Properties.DateOfBirth.IsEnabled = true;
      _obj.State.Properties.ExternalCodeavis.IsEnabled = true;
      
      _obj.State.Properties.IsPersonalDataavis.IsVisible = true;
      _obj.State.Properties.IsPersonalDataavis.IsEnabled = true;
      
      _obj.State.Properties.CodeInvestavis.IsVisible = true;
      _obj.State.Properties.CodeInvestavis.IsEnabled = true;
    }
  }
  
  // Конец добавлено avis.
}