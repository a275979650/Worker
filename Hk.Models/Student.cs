using Hk.Core.Util.Aspects;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hk.Models
{
    [Table("Student")]
    public class Student
    {
        [Key]
        public string id { get; set; }
        public string name { get; set; }
        public string sex { get; set; }
        [Ignore]
        public string yyy { get; set; }
    }
}
