//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Affilitest.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public System.Guid UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Nullable<long> TotalOfClick { get; set; }
        public Nullable<long> CurrentOfClick { get; set; }
        public bool Role { get; set; }
        public Nullable<int> NumberOfUrl { get; set; }
        public int DayClick { get; set; }
        public long NumberClick { get; set; }
        public string Token { get; set; }
    }
}
