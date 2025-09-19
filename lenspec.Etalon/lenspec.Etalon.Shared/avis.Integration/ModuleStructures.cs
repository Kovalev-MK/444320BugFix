using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace lenspec.Etalon.Module.Integration.Structures.Module
{
  #region Интеграция с ЛК.
  #region Интеграция с ЛК (Данные для получения).
  [Public(Isolated = true)]
  partial class LkHeaderRequest
  {
    public string MessageId { get; set; }
  }
  
  [Public(Isolated = true)]
  partial class LkCreateCustomerRequestBody
  {
    /// <summary>
    /// ИД клиента.
    /// </summary>
    public string LkUserId { get; set; }
    /// <summary>
    /// № договора.
    /// </summary>
    public string ContractNumber { get; set; }
    /// <summary>
    /// Дата договора.
    /// </summary>
    public string ContractDate { get; set; }
    /// <summary>
    /// Телефон клиента.
    /// </summary>
    public string ClientPhoneNumber { get; set; }
    /// <summary>
    /// Дата обращения.
    /// </summary>
    public string RequestDate { get; set; }
    /// <summary>
    /// Номер обращения/маршрута.
    /// </summary>
    public string Category { get; set; }
    /// <summary>
    /// Наша организация (передается ИНН).
    /// </summary>
    public string CompanyINN { get; set; }
    /// <summary>
    /// Адрес местонахождения объекта недвижимости.
    /// </summary>
    public string TerritoryAddress { get; set; }
    /// <summary>
    /// Адрес проживания клиента.
    /// </summary>
    public string Address { get; set; }
    /// <summary>
    /// Сумма возврата.
    /// </summary>
    public string Sum { get; set; }
    /// <summary>
    /// Расчетный счет.
    /// </summary>
    public string PaymentAccount { get; set; }
    /// <summary>
    /// Банк.
    /// </summary>
    public string Bank { get; set; }
    /// <summary>
    /// БИК банка.
    /// </summary>
    public string BicBank { get; set; }
    /// <summary>
    /// ИНН банка.
    /// </summary>
    public string InnBank { get; set; }
    /// <summary>
    /// КПП банка.
    /// </summary>
    public string KppBank { get; set; }
    /// <summary>
    /// Корр. счет.
    /// </summary>
    public string CorrespondentAccount { get; set; }
    /// <summary>
    /// Получатель.
    /// </summary>
    public string Recipient { get; set; }
    /// <summary>
    /// Ссылка на файл в промежуточном хранилище.
    /// </summary>
    public List< lenspec.Etalon.Module.Integration.Structures.Module.ILKUrl> Urls { get; set; }
    /// <summary>
    /// Расширение файла из промежуточного хранилища.
    /// </summary>
    public string Extencion { get; set; }
  }
  
  /// <summary>
  /// Ссылка на документ с расширением.
  /// </summary>
  [Public(Isolated = true)]
  partial class  LKUrl
  {
    /// <summary>
    /// Ссылка на файл в промежуточном хранилище.
    /// </summary>
    public string Url { get; set; }
    /// <summary>
    /// Расширение файла из промежуточного хранилища.
    /// </summary>
    public string Extencion { get; set; }
  }
  
  [Public(Isolated = true)]
  partial class LkGetStatusCustomerRequestBody
  {
    /// <summary>
    /// ИД клиента.
    /// </summary>
    public string LkUserId { get; set; }
  }
  
  [Public(Isolated = true)]
  partial class GetBodyCustomerRequestBody
  {
    /// <summary>
    /// ИД документа.
    /// </summary>
    public long DocID { get; set; }
    /// <summary>
    /// Номер версии.
    /// </summary>
    public long VerNumber { get; set; }
  }
  
  [Public(Isolated = true)]
  partial class LkGetStatusDocRequestsByContractRequestBody
  {
    /// <summary>
    /// ИД клиента.
    /// </summary>
    public string LkUserId { get; set; }
    /// <summary>
    /// № договора.
    /// </summary>
    public string ContractNumber { get; set; }
    /// <summary>
    /// Номер обращения/маршрута.
    /// </summary>
    public string Category { get; set; }
  }
  #endregion
  
  #region Интеграция с ЛК (Пакет для ответа).
  /// <summary>
  /// Модель данных для ответа на пакет для ЛК.
  /// </summary>
  partial class LkCreateCustomerResponseModel 
  {
    public lenspec.Etalon.Module.Integration.Structures.Module.LkHeaderResponse Header { get; set; }
    public string Body { get; set; }
  }
  
  /// <summary>
  /// Модель данных для ответа на пакет для ЛК с получением статуса.
  /// </summary>
  partial class LkGetStatusCustomerResponseModel
  {
    public lenspec.Etalon.Module.Integration.Structures.Module.LkHeaderResponse Header { get; set; }
    public List<lenspec.Etalon.Module.Integration.Structures.Module.LkGetStatusCustomerResponseBody> Body { get; set; }
  }
  
  partial class LkGetBodyCustomerResponseModel
  {
    public lenspec.Etalon.Module.Integration.Structures.Module.LkHeaderResponse Header { get; set; }
    public lenspec.Etalon.Module.Integration.Structures.Module.LkGetBodyCustomerResponseBody Body { get; set; }
  }
  
  partial class LkGetStatusDocRequestsByContractResponseModel 
  {
    public lenspec.Etalon.Module.Integration.Structures.Module.LkHeaderResponse Header { get; set; }
    public List<lenspec.Etalon.Module.Integration.Structures.Module.LkGetStatusDocRequestsByContractResponseBody> Body { get; set; }
  }
  #endregion
  
  #region Интеграция с ЛК (Данные для ответа).
  /// <summary>
  /// Модель Основного ответа от сервиса интегарции ЛК.
  /// </summary>
  partial class LkHeaderResponse
  {
    public string MessageId { get; set; }
    public string Status { get; set; }
    public string StatusMessage { get; set; }
  }
  
  partial class LkGetStatusCustomerResponseBody
  {
    /// <summary>
    /// ИД обращения клиента.
    /// </summary>
    public long DocID { get; set; }
    /// <summary>
    /// ИД последней версии обращения клиента.
    /// </summary>
    public int VerNumber { get; set; }
    /// <summary>
    /// Имя обращения клиента.
    /// </summary>
    public string DocName { get; set; }
    /// <summary>
    /// Статус задачи.
    /// </summary>
    public string State { get; set; }
  }
  
  partial class LkGetBodyCustomerResponseBody
  {
    public string DocLink { get; set; }
  }
  
  partial class LkGetStatusDocRequestsByContractResponseBody
  {
    public long DocId { get; set; }
    public long VerNumber { get; set; }
    public string DocName { get; set; }
  }
  #endregion
  #endregion
  
  #region Интеграция с ПКП.
  #region Интеграция с ПКП (Получение пакетов).
  /// <summary>
  /// Модель данных для запроса CreateConstructionObjectAct.
  /// </summary>
  [Public(Isolated = true)]
  partial class PkpCreateConstructionObjectActRequestBody
  {
    public string MessageId { get; set; }
    public string InvestContractId { get; set; }
    public string DocumentKind { get; set; }
    public string DateAct { get; set; }
    public string Link { get; set; }
    public string Extension { get; set; }
  }
  
  partial class PkpCreateConstructionObjectActResponseBody
  {
    public string DirectumId { get; set; }
  }
  #endregion
  
  #region Интеграция с ПКП (Пакет для ответа).
  partial class PkpCreateConstructionObjectActResponseModel
  {
    public lenspec.Etalon.Module.Integration.Structures.Module.LkHeaderResponse Header { get; set; }
    public lenspec.Etalon.Module.Integration.Structures.Module.PkpCreateConstructionObjectActResponseBody Body { get; set; }
  }
  #endregion
 
  #endregion
  
  #region Интеграция с CRM.
  partial class CrmPutClaimRequest 
  {
    /// <summary>
    /// ИД задачи, в которую вложена карточка обращения клиента.
    /// </summary>
    public int DirectumId { get; set; }
    /// <summary>
    /// Фамилия Персоны из поля Клиент.
    /// </summary>
    public string SurName { get; set; }
    /// <summary>
    /// Имя Персоны из поля Клиент.
    /// </summary>
    public string GivenName { get; set; }
    /// <summary>
    /// Отчество Персоны из поля Клиент.
    /// </summary>
    public string MiddleName { get; set; }
    /// <summary>
    ///  “№ договора” вычисляется из Клиентского договора. 
    /// Если несколько КД, то номера всех через запятую.
    /// </summary>
    public string ContractNumber { get; set; }
    /// <summary>
    /// Контактный телефон.
    /// </summary>
    public string Phone { get; set; }
    /// <summary>
    /// Рег.номер.
    /// </summary>
    public string RegistrationNumber { get; set; }
    /// <summary>
    /// Ожидаемый срок завершения согласования документа.
    /// </summary>
    public string PlannedEndDate { get; set; }
    /// <summary>
    /// Дата документа.
    /// </summary>
    public string RegistrationDate { get; set; }
    /// <summary>
    /// Ссылка на задачу.
    /// </summary>
    public string DirectumLink { get; set; }
    /// <summary>
    /// Тип претензии.
    /// </summary>
    public string ClaimDescription { get; set; }
    /// <summary>
    /// Код категории (из Категории обращения).
    /// </summary>
    public int RouteCategory { get; set; }
    /// <summary>
    /// ???.
    /// </summary>
    public int TerritorialOffice { get; set; }
  }
  
  /// <summary>
  /// Ответ от Crm.
  /// </summary>
  partial class CrmResponse
  {
    /// <summary>
    /// Код ошибки.
    /// </summary>
    public int ResponseCode { get; set; }
    /// <summary>
    /// Текст ошибки.
    /// </summary>
    public string ErrorMessage { get; set; }
  }
  #endregion
  
  #region Остальное.
  partial class Header
  {
    public string From { get; set; }
    public string To { get; set; }
    public string Type { get; set; }
    public string Code { get; set; }
    public string ResponseTo { get; set; }
    public DateTime Cdt { get; set; }
    public DateTime Mdt { get; set; }
    public string MessageId { get; set; }
    public string RequestId { get; set; }
    public string Version { get; set; }
    public string Crc { get; set; }
    public string Sign { get; set; }
    public string Tracer { get; set; }
  }
     
  /// <summary>
  /// Пакет drx_client_updt.
  /// </summary>
  partial class Body
  {
    /// <summary>
    /// Фамилия клиента.
    /// </summary>
    public string lname { get; set; }
    /// <summary>
    /// Имя клиента.
    /// </summary>
    public string fname { get; set; }
    /// <summary>
    /// Отчество клиента.
    /// </summary>
    public string mname { get; set; }
    /// <summary>
    /// Дата рождения.
    /// </summary>
    public string birth_dt { get; set; }
    /// <summary>
    /// Основной номер телефона дольщика.
    /// </summary>
    public string phone { get; set; }
    /// <summary>
    /// Электронная почта.
    /// </summary>
    public string email { get; set; }
    /// <summary>
    /// Согласие на обработку персональных данных (0 = согласия нет, 1 = согласие есть, 2 = нет данных в Инвест). 
    /// </summary>
    public int rev_pd { get; set; }
    /// <summary>
    /// ID дольщика в Инвест.
    /// </summary>
    public string dp_id { get; set; }
    /// <summary>
    /// Пол.
    /// </summary>
    public string person_sex { get; set; }
    /// <summary>
    /// ИНН.
    /// </summary>
    public string dp_inn { get; set; }
    /// <summary>
    /// Адрес регистрации (адрес прописки) Передавать в одной строке: Регион+Город+Адрес.
    /// </summary>
    public string address_reg { get; set; }
    /// <summary>
    /// Почтовый адрес (в данном поле объединяются в одну строку все поля, описанные ниже).
    /// </summary>
    public string mail_address { get; set; }
    /// <summary>
    /// Индекс (почтовый/фактический адрес).
    /// </summary>
    public string address_zip { get; set; }
    /// <summary>
    /// Страна почтовый/фактический адрес).
    /// </summary>
    public string adress_country { get; set; }
    /// <summary>
    /// Регион почтовый/фактический адрес).
    /// </summary>
    public string address_region { get; set; }
    /// <summary>
    /// Город почтовый/фактический адрес).
    /// </summary>
    public string address_city { get; set; }
    /// <summary>
    /// Район (почтовый/фактический адрес).
    /// </summary>
    public string address_district { get; set; }
    /// <summary>
    /// Населенный пункт (почтовый/фактический адрес).
    /// </summary>
    public string address_locality { get; set; }
    /// <summary>
    /// Улица (почтовый/фактический адрес.
    /// </summary>
    public string address_street { get; set; }
    /// <summary>
    /// Дом (почтовый/фактический адрес).
    /// </summary>
    public string address_house { get; set; }
    /// <summary>
    /// Строение/корпус (почтовый/фактический адрес).
    /// </summary>
    public string address_bldg { get; set; }
    /// <summary>
    /// Квартира (почтовый/фактический адрес).
    /// </summary>
    public string address_flat { get; set; }
    /// <summary>
    /// ИД клиента в ЛК.
    /// </summary>
    public string lkuser_id { get; set; }
  }
     
  partial class BodySort
  {
    public string kvsort_id { get; set; }
    public string kvsort_title { get; set; }
    public int kvsort_actf { get; set; }
  }
      
  partial class BodyCF
  {
    public string project_id { get; set; }
    public string drxprj_id { get; set; }
  }
  
  partial class BodyObj
  {
    public string object_id { get; set; }
    public string drxobj_id { get; set; }
  }
  
  partial class BodyFlat
  {
    public string kv_id { get; set; }
    public string kvsort_id { get; set; }
    public string kv_num { get; set; }
    public string object_id { get; set; }
    public string kv_pibf { get; set; }
    public string kv_pibsqdif { get; set; }
    public string kv_pibcostdif { get; set; }
  }
   
  partial class BodyCtr
  {
    public string dog_id { get; set; }
    public string dog_num { get; set; }
    public string dog_dt { get; set; }
    public string drxobj_drxid { get; set; }
    public int ecp_fl { get; set; }
    public int dog_status { get; set; }
    public string kv_id { get; set; }
    public string app_dt { get; set; }
    public List<string> dp_id { get; set; }
    public string developer_inn { get; set; }
  }
  
  partial class BodyMsg
  {
    public string drxobj_id { get; set; }
    public string message_id { get; set; }
    public int kv_amount { get; set; }
  }
  
  /// <summary>
  /// Пакет drx_client_updt (Данные о дольщиках из Инвест).
  /// </summary>
  partial class Package
  {
    public lenspec.Etalon.Module.Integration.Structures.Module.Header Header { get; set; }
    public lenspec.Etalon.Module.Integration.Structures.Module.Body Body { get; set; }
  }
  
  partial class PackageSort
  {
    public lenspec.Etalon.Module.Integration.Structures.Module.Header Header { get; set; }
    public lenspec.Etalon.Module.Integration.Structures.Module.BodySort Body { get; set; }
  }
  
  partial class PackageCF
  {
    public lenspec.Etalon.Module.Integration.Structures.Module.Header Header { get; set; }
    public lenspec.Etalon.Module.Integration.Structures.Module.BodyCF Body { get; set; }
  }
  
  partial class PackageObj
  {
    public lenspec.Etalon.Module.Integration.Structures.Module.Header Header { get; set; }
    public lenspec.Etalon.Module.Integration.Structures.Module.BodyObj Body { get; set; }
  }
  
  partial class PackageFlat
  {
    public lenspec.Etalon.Module.Integration.Structures.Module.Header Header { get; set; }
    public lenspec.Etalon.Module.Integration.Structures.Module.BodyFlat Body { get; set; }
  }
  
  partial class PackageCtr
  {
    public lenspec.Etalon.Module.Integration.Structures.Module.Header Header { get; set; }
    public lenspec.Etalon.Module.Integration.Structures.Module.BodyCtr Body { get; set; }
  }
  
  partial class PackageMsg
  {
    public lenspec.Etalon.Module.Integration.Structures.Module.Header Header { get; set; }
    public lenspec.Etalon.Module.Integration.Structures.Module.BodyMsg Body { get; set; }
  }
  
  #endregion
}