//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Definitions
{
    using System;
    using System.Collections.Generic;
    
    public partial class ExtraTerms
    {
        public int Id { get; set; }
        public int TermsId { get; set; }
        public string Value { get; set; }
    
        public virtual Terms Terms { get; set; }
    }
}
