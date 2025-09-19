using System;
using Sungero.Core;

namespace lenspec.Tenders.Constants
{
  public static class ProviderRegister
  {

    /// <summary>
    /// Ячейки данных анкеты квалификации.
    /// </summary>
    public static class XmlCells
    {
      /// <summary>
      /// Сведения о КА (ген. директоре/учредителе).
      /// </summary>
      public static class Counterparty
      {
        /// Должность.
        public const string Position = "C43";
        
        /// ФИО.
        public const string FullName = "C45";
        
        /// Телефон офиса.
        public const string PhoneWork = "C47";
        
        /// Мобильный телефон.
        public const string PhonePersonal = "C49";
        
        /// Электронная почта.
        public const string Email = "C51";
      }
      
      /// <summary>
      /// Сведения о контактном лице.
      /// </summary>
      public static class Contact
      {
        /// Должность.
        public const string Position = "C67";
        
        /// ФИО.
        public const string FullName = "C69";
        
        /// Телефон офиса.
        public const string PhoneWork = "C71";
        
        /// Мобильный телефон.
        public const string PhonePersonal = "C73";
        
        /// Электронная почта.
        public const string Email = "C75";
      }
    }
    
  }
}