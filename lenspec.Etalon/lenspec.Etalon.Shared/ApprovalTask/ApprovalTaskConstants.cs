using System;
using Sungero.Core;

namespace lenspec.Etalon.Constants.Docflow
{
  public static class ApprovalTask
  {
    /// <summary>
    /// Наименование тэга Рег. номера для обновления параметра в теле документа.
    /// </summary>
    public const string UpdateTemplateRegistrationNumberTag = "RegistrationNumber";
    
    /// <summary>
    /// Наименование тэга Даты документа для обновления параметра в теле документа.
    /// </summary>
    public const string UpdateTemplateRegistrationDateTag = "RegistrationDate";
    
    /// <summary>
    /// Наименование тэга Рег. номера для обновления параметра в теле связанного документа.
    /// </summary>
    public const string UpdateHeadTemplateRegistrationNumberTag = "RegistrationNumHeadDocument";
    
    /// <summary>
    /// Наименование тэга Даты документа для обновления параметра в теле связанного документа.
    /// </summary>
    public const string UpdateHeadTemplateRegistrationDateTag = "RegistrationDateHeadDocument";
    
    /// <summary>
    /// Наименование тэга номера ответного письма для обновления параметра в теле связанного документа.
    /// </summary>
    public const string UpdateHeadTemplateNumberInResponseToTag = "NumberInResponseTo";
    
    /// <summary>
    /// Наименование тэга даты ответного письма для обновления параметра в теле связанного документа.
    /// </summary>
    public const string UpdateHeadTemplateDateInResponseToTag = "DateInResponseTo";
  }
}