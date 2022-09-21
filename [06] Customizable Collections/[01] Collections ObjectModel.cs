using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _06__Customizable_Collections
{
  public    class _01__Collections_ObjectModel
    {
       public static void Show()
        {
            // 创建动物园
            Zoo zoo = new Zoo();
            // 添加动物
            zoo.Animals.Add(new Animal("Kangaroo", 10));
            zoo.Animals.Add(new Animal("Mr Sea Lion", 20));
            foreach (Animal a in zoo.Animals) Console.WriteLine(a.Name);
        }
    }

    public class Zoo   // The class that will expose AnimalCollection.
    {                  // This would typically have additional members.
        public readonly AnimalCollection Animals;
        public Zoo() { Animals = new AnimalCollection(this); }
    }
    public class AnimalCollection : Collection<Animal>
    {
        Zoo zoo;
        public AnimalCollection(Zoo zoo) { this.zoo = zoo; }

        protected override void InsertItem(int index, Animal item)
        {
            base.InsertItem(index, item);
            item.Zoo = zoo;                 // 添加动物 顺便为该动物指定动物园
        }
        protected override void SetItem(int index, Animal item)
        {
            base.SetItem(index, item);
            item.Zoo = zoo;
        }
        protected override void RemoveItem(int index)
        {
            this[index].Zoo = null;
            base.RemoveItem(index);
        }
        protected override void ClearItems()
        {
            foreach (Animal a in this) a.Zoo = null;
            base.ClearItems();
        }
    }

    public class Animal
    {
        public string Name;
        public int Popularity;
        public Zoo Zoo { get; internal set; }

        public Animal(string name, int popularity)
        {
            Name = name; Popularity = popularity;
        }
    }

}
