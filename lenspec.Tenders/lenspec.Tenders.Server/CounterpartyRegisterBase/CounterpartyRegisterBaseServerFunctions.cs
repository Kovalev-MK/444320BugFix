using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using lenspec.Tenders.CounterpartyRegisterBase;

namespace lenspec.Tenders.Server
{
  partial class CounterpartyRegisterBaseFunctions
  {

    /// <summary>
    /// Выгрузить данные о КА из документа в карточку реестра.
    /// </summary>
    /// <param name="tenderAccreditationForm">Анкета квалификации.</param>
    /// <returns>Пустая строка, если выполнено без ошибок; Иначе – текст ошибки.</returns>
    /// <remarks>Не сохраняет сущность.</remarks>
    [Public, Remote(IsPure = false)]
    public virtual string UploadCounterpartyData(Tenders.ITenderAccreditationForm tenderAccreditationForm)
    {
      if (tenderAccreditationForm == null)
        return new ArgumentNullException(Tenders.Resources.TenderQualificationForm).Message;
      
      if (!tenderAccreditationForm.HasVersions)
        return lenspec.Tenders.CounterpartyRegisterBases.Resources.DocumentVersionsNotFoundFormat(tenderAccreditationForm.Info.LocalizedName);
      
      try
      {
        var data = GetData(tenderAccreditationForm);
        // Заполняем найденные данные.
        FillData(data);
        // Проверяем незаполненные ячейки.
        ValidateData(data);
      }
      catch (Exception ex)
      {
        return lenspec.Tenders.CounterpartyRegisterBases.Resources.ErrorLoadingDataFromQuestionnaireFormat(ex.Message);
      }
      
      return string.Empty;
    }
    
    /// <summary>
    /// Проверить консистентость данных о КА из анкеты.
    /// </summary>
    /// <param name="data">Данные о КА из ячеек таблицы.</param>
    /// <exception cref="Exception">Список пустых ячеек.</exception>
    [Public]
    public virtual void ValidateData(System.Collections.Generic.Dictionary<string, string> data)
    {
      // Проверка наличия всех данных в анкете:
      var emptyCells = new LinkedList<string>();
      foreach (var record in data)
        if (string.IsNullOrWhiteSpace(record.Value))
          emptyCells.AddLast(record.Key);
      
      if (emptyCells.Any())
      {
        var emptyCellsString = string.Join(", ", emptyCells);
        var tenderAccreditationFormName = lenspec.Tenders.TenderAccreditationForms.Info.LocalizedName.ToLower();
        Logger.Error(lenspec.Tenders.CounterpartyRegisterBases.Resources.MissingDataInTenderAccreditationFormFormat(tenderAccreditationFormName, emptyCellsString));
        throw new Exception(lenspec.Tenders.CounterpartyRegisterBases.Resources.CheckTenderAccreditationForm);
      }
    }
    
    /// <summary>
    /// Получить данные о КА из анкеты.
    /// </summary>
    /// <param name="tenderAccreditationForm">Анкета квалификации.</param>
    /// <returns>Данные о КА из ячеек таблицы.</returns>
    public virtual System.Collections.Generic.Dictionary<string, string> GetData(Tenders.ITenderAccreditationForm tenderAccreditationForm)
    {
      var version = tenderAccreditationForm.LastVersion;
      var extension = version.AssociatedApplication.Extension.ToUpper();
      var expectedExtension = lenspec.Tenders.CounterpartyRegisterBases.Resources.ExpectedExtension;
      if (extension != expectedExtension)
      {
        var message = Tenders.CounterpartyRegisterBases.Resources.VersionFormatExceptionFormat(
          Tenders.TenderAccreditationForms.Info.LocalizedName,
          extension,
          expectedExtension);
        throw new FormatException(message);
      }
      
      // Данные анкеты квалификации.
      Dictionary<string, string> data;
      using (var stream = new System.IO.MemoryStream())
      {
        tenderAccreditationForm.LastVersion.Body.Read().CopyTo(stream);
        data = Tenders.IsolatedFunctions.XmlParseArea.GetCellsData(stream, GetCellsWithData());
      }
      return data;
    }
    
    /// <summary>
    /// Получить наименования ячеек Excel с данными о КА.
    /// </summary>
    /// <returns>Наименования ячеек Excel.</returns>
    [Public]
    public virtual List<string> GetCellsWithData()
    {
      // Перекрыта в наследниках.
      throw new NotImplementedException();
    }
    
    /// <summary>
    /// Заполнить данные о КА в карточке реестра.
    /// </summary>
    /// <param name="data">Данные о КА из ячеек таблицы.</param>
    /// <remarks>Не сохраняет сущность.</remarks>
    [Public]
    public virtual void FillData(System.Collections.Generic.Dictionary<string, string> data)
    {
      // Перекрыта в наследниках.
      throw new NotImplementedException();
    }
    
  }
}