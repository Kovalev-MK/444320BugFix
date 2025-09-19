using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace avis.EtalonContracts.Server
{
  public class ModuleJobs
  {

    /// <summary>
    /// фоновый процесс "Загрузка видов работ и детализации видов работ из интеграционной базы".
    /// </summary>
    public virtual void LoadingWorkTypeFromIntegraDB()
    {
      //Ищем настройки подключения
      var settings = lenspec.Etalon.Integrationses.GetAll(s => s.Code == avis.EtalonContracts.PublicConstants.Module.KindsWorkTypesCode).FirstOrDefault();
      if (settings != null)
      {
        DateTime startDate = Calendar.Now;
        DateTime? lastStartDate = settings.SyncDateTime;
        
        //Создаем запись в истории загрузки
        var runEntry = settings.RunHistory.AddNew();

        // инициализируем helper для получения данных из интеграционной базы
        string connectionString = Encryption.Decrypt(settings.ConnectionParams);
        if (connectionString != string.Empty)
        {
          // Создаём репозиторий для подключения к БД.
          List<AvisIntegrationHelper.KindsWorkTypes> kindsWorkTypesList;
          kindsWorkTypesList = AvisIntegrationHelper.DataBaseHelper.GetKindsWorkTypes(connectionString, lastStartDate.Value);
          // Получаем все записи по видам работ.
          foreach(AvisIntegrationHelper.KindsWorkTypes integraBI in kindsWorkTypesList)
          {
            CreateOrUpdateWorkType(integraBI.KindsWorkTypeID, integraBI.Description, integraBI.Action);
          }
          
        }
        
        // Создаём репозиторий для подключения к БД.
        List<AvisIntegrationHelper.WorkTypes> workTypesList;
        workTypesList = AvisIntegrationHelper.DataBaseHelper.GetWorkTypes(connectionString, lastStartDate.Value);
        // Получаем все записи по Детализации видов работ.
        foreach(AvisIntegrationHelper.WorkTypes integraBI in workTypesList)
        {
          CreateOrUpdateDetailingdWorkType(integraBI.WorkTypeID, integraBI.Description, integraBI.Action, integraBI.IDKindsWorkTypes);
        }
      }
    }
    
    /// <summary>
    /// Создаём или обновляем запись "Детализация видов работ".
    /// </summary>
    /// <param name="code1c">Код 1с.</param>
    /// <param name="name">Наименование.</param>
    /// <param name="action">Действие: значения 1, 2 или 3.</param>
    /// <param name="idKindsWorkTypes">Код1с типа работ.</param>
    private void CreateOrUpdateDetailingdWorkType(string code1c, string name, int? action, string workTypeCode1c)
    {
      // Находим запись по коду 1с.
      var detailingWorkType = avis.EtalonContracts.DetailingWorkTypes.GetAll(d => d.Code1c == code1c).FirstOrDefault();
      
      // Если запись не найдено, а код закрытия, пропускаем.
      if (detailingWorkType == null && action == 3)
        return;
      
      // Если запись не найдена, то создаём новую.
      if (detailingWorkType == null)
        detailingWorkType = avis.EtalonContracts.DetailingWorkTypes.Create();
      
      // Находим тип работ по коду 1с.
      var workType = avis.EtalonContracts.WorkTypes.GetAll(w => w.Code1c == workTypeCode1c).FirstOrDefault();
      
      // Заполняем основные поля.
      detailingWorkType.Name = name;
      detailingWorkType.Code1c = code1c;
      detailingWorkType.WorkType = workType;
      
      // Обновляем статус.
      if (action == 1 || action == 2)
        detailingWorkType.Status = avis.EtalonContracts.DetailingWorkType.Status.Active;
      
      if (action == 3)
        detailingWorkType.Status = avis.EtalonContracts.DetailingWorkType.Status.Closed;
      
      // Сохраняем.
      detailingWorkType.Save();
    }
    
    /// <summary>
    /// Создаём или обновляем запись "Виды работ".
    /// </summary>
    /// <param name="code1c">Код 1с.</param>
    /// <param name="name">Наименование.</param>
    /// <param name="action">Действие: значения 1, 2 или 3.</param>
    private void CreateOrUpdateWorkType(string code1c, string name, int? action)
    {
      // Находим запись по коду 1с.
      var workType = avis.EtalonContracts.WorkTypes.GetAll(k => k.Code1c == code1c).FirstOrDefault();
      
      // Если запись не найдено, а код закрытия, пропускаем.
      if (workType == null && action == 3)
        return;
      
      // Если запись не найдена, то создаём новую.
      if (workType == null)
        workType = avis.EtalonContracts.WorkTypes.Create();
      
      // Заполняем основные поля.
      workType.Name = name;
      workType.Code1c = code1c;
      
      // Обновляем статус.
      if (action == 1 || action == 2)
        workType.Status = avis.EtalonContracts.WorkType.Status.Active;
      
      if (action == 3)
        workType.Status = avis.EtalonContracts.WorkType.Status.Closed;
      
      // Сохраняем.
      workType.Save();
    }
  }
}