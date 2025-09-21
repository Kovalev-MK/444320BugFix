using System;
using Sungero.Core;

namespace lenspec.Tenders.Constants
{
  public static class ContractorRegister
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
        public const string Position = "C40";
        
        /// ФИО.
        public const string FullName = "C42";
        
        /// Телефон офиса.
        public const string PhoneWork = "C44";
        
        /// Мобильный телефон.
        public const string PhonePersonal = "C46";
        
        /// Электронная почта.
        public const string Email = "C48";
      }
      
      /// <summary>
      /// Сведения о контактном лице.
      /// </summary>
      public static class Contact
      {
        /// Должность.
        public const string Position = "F15";
        
        /// ФИО.
        public const string FullName = "F17";
        
        /// Телефон офиса.
        public const string PhoneWork = "F19";
        
        /// Мобильный телефон.
        public const string PhonePersonal = "F21";
        
        /// Электронная почта.
        public const string Email = "F23";
      }
    }
    
  }
}