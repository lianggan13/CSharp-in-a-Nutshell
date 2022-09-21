using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLinq
{
    /// <summary>
    /// 学生实体
    /// </summary>
    public class Student
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        public void Study()
        {
            Console.WriteLine("{0} {1}跟着Eleven老师学习.net高级开发", this.Id, this.Name);
        }

        public void StudyHard()
        {
            Console.WriteLine("{0} {1}跟着Eleven老师努力学习.net高级开发", this.Id, this.Name);
        }

        public void Sing()
        {
            Console.WriteLine("Sing a Song");
        }
    }

    /// <summary>
    /// 班级实体
    /// </summary>
    public class Class
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
    }
}
