using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;


	// subclass this class for a singleton like structure

	// NOTE: 'ImplicitInstantiation' classes are still created but not persisted
	// TODO: use reflection to avoid creating UTR where 'ImplicitInstatiation' unnecicarily
	
    public class UniqueTypeRegistry {


        // find the list of all subclasses once
        private static IEnumerable<UniqueTypeRegistry> types;
		
		// shortcut to all 'Types'
        public static List<string> AllTypes = new List<string>();

		// shortcut to all 'BaseTypes'
        public static List<string> AllBaseTypes = new List<string>();

		 static UniqueTypeRegistry() {
			types = typeof(UniqueTypeRegistry)
    		.Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(UniqueTypeRegistry)) && !t.IsAbstract).Select(t => {
                UniqueTypeRegistry utr = (UniqueTypeRegistry)Activator.CreateInstance(t);
                if (!utr.ImplicitInstantiation) return null;
                return utr;
            });
			foreach (UniqueTypeRegistry v in types) {
                if (!AllTypes.Contains(v.Type) && !v.BaseOnly) AllTypes.Add(v.MatchByClassName ? v.GetType().Name : v.Type);
                if (!AllBaseTypes.Contains(v.BaseType)) AllBaseTypes.Add(v.BaseType);
                v.Constructor();
            }
		}

		// optionally implement to be notified when a UTR with 'ImplicitInstantiation'=false is created or deleted
		// can be used if 'ImplicitInstantion'= true
		public virtual void Constructor() {}

		public virtual void Destructor() {}

        // optionally instatiate types by 'BaseType'
        // NOTE: not by types
        public static void InstantiateImplicits(string baseType = null)
        {
            types.Concat<UniqueTypeRegistry>(typeof(UniqueTypeRegistry)
            .Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(UniqueTypeRegistry)) && !t.IsAbstract).Select(t =>
            {
                UniqueTypeRegistry utr = (UniqueTypeRegistry)Activator.CreateInstance(t);
				if (utr.ImplicitInstantiation) return null; // ignore UTR=ImplicitInstantitian
				if (types.ToList().Find(tutr => tutr.GetType() == utr.GetType()) != null) return null; // ignore existing types already
				if (baseType != null && utr.BaseType != baseType) return null; // if baseType is null then ignore if mismatching
                utr.Constructor();
                utr._constructed = true;
                return utr;
        	}));
        }

		public static void InstantiateImplicits<T>() where T : UniqueTypeRegistry, new() {
            InstantiateImplicits((new T() as UniqueTypeRegistry).BaseType);
        }

		// optionally delete types by 'BaseType'
		// NOTE: not by types
		public static void DestroyImplicits(string baseType = null) {
			// keep UTR's where 'ImplicitInstantiation' is true
			// if baseType isnt null then remove non matching BaseType's 
            types = types.Where(utr =>
            {
                bool r = utr.ImplicitInstantiation && (utr.BaseType != baseType || baseType == null);
                if (!r)
                {
                    utr.Destructor();
                    utr._constructed = false;
                }
                return r;

            });
        }

		public static void DestroyImplicits<T>() where T : UniqueTypeRegistry, new() {
            DestroyImplicits((new T() as UniqueTypeRegistry).BaseType);
        }
		
		// cache input is a string list of:
		//		T, type, baseType (convert null to string null)
		// cache result is of index in types
		internal static Dictionary<string, int> findCache = new Dictionary<string, int>();
		
		// cache input is a string list of:
		//		T, baseType (conver null to string null)
		// cache result is a list in indexes
		internal static Dictionary<string, List<int>> findAllCache = new Dictionary<string, List<int>>();

        // returns an subclass of T
		// generic type used for base type
        // searches by param class type or Type if defined
        public static T Find<T>(string type, string baseType = null) where T : UniqueTypeRegistry, new() {
			string key = typeof(T).ToString() + type + (baseType == null ? "null" : baseType);
			UniqueTypeRegistry found = null;
			if (!findCache.ContainsKey(key)) {
				// case: search and put in cache
				baseType = baseType == null ? (new T() as UniqueTypeRegistry).BaseType : baseType;
				int i = 0;
				foreach (var v in types) {
					if (!v.BaseOnly) {
						if (baseType == null || v.BaseType == baseType) { // match baseTypes if baseType != not null
							if (v.MatchByClassName ? v.GetType().Name == type : v.Type == type) { found = v; break; } // match Object.GetType if UniqueTypeRegistry.Type is null
						}
					}
					i += 1;
				}
				if (i != types.Count()) findCache.Add(key, i); // don't add invalid results to cache
			} else {
				// case: find from cache
				found = types.ElementAt(findCache[key]);
			}
			return found == null ? default(T) : (T)(object)found;
        }

		// returns all matching BaseTypes given a base type param
		public static List<T> FindAll<T>(string baseType = null) where T : UniqueTypeRegistry, new() {
			string key = typeof(T).ToString() + (baseType == null ? "null" : baseType);
            List<T> list = new List<T>();
            if (findAllCache.ContainsKey(key)) {
                // case: add from cache
                foreach (int index in findAllCache[key]) {
                    list.Add((T)(object)types.ElementAt(index));
                }
                return list;
            }
            // case: find in types and add to cache
			baseType = baseType == null ? (new T() as UniqueTypeRegistry).BaseType : baseType;
            List<int> cl = new List<int>(); // cache list of integers added associated with the key
            int i = 0;
            foreach (var v in types)
            {
                if (!v.BaseOnly)
                {
                    if (baseType == null || v.BaseType == baseType) // match baseTypes
                    {
                        list.Add((T)(object)v);
                        cl.Add(i);
                    }
                }
                i += 1;
            }
            if (cl.Count > 0) findAllCache.Add(key, cl); // only add to cache if base types were actually found
            return list;
        }

		/* 
			UTR's can check if they have been 'really' constructed, since 'ImplicitConstruction' constructs multiple times
		*/
        internal bool _constructed = false;
        public bool Constructed {
            get => _constructed;
        }

        /*
			True to ignore in 'Find/FindAll' and in AllTypes
			By default is only true when Type is nill
		*/
        public virtual bool BaseOnly => Type == null;

        /*
			Create this class by default when the runtime is initiated.
			If fault, you must invoke 'InstantiateImplicits' and 'DestroyImplicits'
		*/
        public virtual bool ImplicitInstantiation => true;

        /*
			True to match by classname instead of Type
		*/
        public virtual bool MatchByClassName => false;

        // matched by this string or classname
        // virtual to be null by default ( gets ignored in search and types )
        public virtual string Type => null;

        // optional base type to group classes of this type
        public virtual string BaseType => "base";
}
