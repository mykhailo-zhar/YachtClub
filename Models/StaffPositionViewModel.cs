using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Project.Migrations;

namespace Project.Models
{
    public class TwineViewModel<BaseType, FirstSlave, SecondSlave > 
    {
        public ObjectViewModel<BaseType> baseObject { get; set; }
        public IEnumerable<FirstSlave> FirstSlaves { get; set; }
        public IEnumerable<SecondSlave> SecondSlaves { get; set; }
    }

    public class TwineViewModelFactory<BaseType, FirstSlave, SecondSlave>
    {
        public static TwineViewModel<BaseType, FirstSlave, SecondSlave> Create(
                BaseType obj,
                IEnumerable<FirstSlave> firstSlaves,
                IEnumerable<SecondSlave> secondSlaves
            )
        {
            var bo = ObjectViewModelFactory<BaseType>.Create(obj);
            return new TwineViewModel<BaseType, FirstSlave, SecondSlave>
            {
                baseObject = bo,
                FirstSlaves =  Enumerable.Empty<FirstSlave>() ?? new List<FirstSlave> (firstSlaves),
                SecondSlaves =  Enumerable.Empty<SecondSlave>() ?? new List<SecondSlave>(secondSlaves)
            };
        }
        public static TwineViewModel<BaseType, FirstSlave, SecondSlave> Delete(
                BaseType obj,
                IEnumerable<FirstSlave> firstSlaves,
                IEnumerable<SecondSlave> secondSlaves
            )
        {
            var bo = ObjectViewModelFactory<BaseType>.Delete(obj);
            return new TwineViewModel<BaseType, FirstSlave, SecondSlave>
            {
                baseObject = bo,
                FirstSlaves = Enumerable.Empty<FirstSlave>() ?? new List<FirstSlave>(firstSlaves),
                SecondSlaves = Enumerable.Empty<SecondSlave>() ?? new List<SecondSlave>(secondSlaves)
            };
        }
        public static TwineViewModel<BaseType, FirstSlave, SecondSlave> Edit(
                BaseType obj,
                IEnumerable<FirstSlave> firstSlaves,
                IEnumerable<SecondSlave> secondSlaves
            )
        {
            var bo = ObjectViewModelFactory<BaseType>.Edit(obj);
            return new TwineViewModel<BaseType, FirstSlave, SecondSlave>
            {
                baseObject = bo,
                FirstSlaves = Enumerable.Empty<FirstSlave>() ?? new List<FirstSlave>(firstSlaves),
                SecondSlaves = Enumerable.Empty<SecondSlave>() ?? new List<SecondSlave>(secondSlaves)
            };
        }
        public static TwineViewModel<BaseType, FirstSlave, SecondSlave> Details(
                BaseType obj,
                IEnumerable<FirstSlave> firstSlaves,
                IEnumerable<SecondSlave> secondSlaves
            )
        {
            var bo = ObjectViewModelFactory<BaseType>.Details(obj);
            return new TwineViewModel<BaseType, FirstSlave, SecondSlave>
            {
                baseObject = bo,
                FirstSlaves = Enumerable.Empty<FirstSlave>() ?? new List<FirstSlave>(firstSlaves),
                SecondSlaves = Enumerable.Empty<SecondSlave>() ?? new List<SecondSlave>(secondSlaves)
            };
        }


    }

}
