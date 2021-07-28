using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TrueSync;
using SimpleJSON;

namespace BeastBear {

    public class UniqueTypeRegistry {


        // find the list of all subclasses once
        private static IEnumerable<UniqueTypeRegistry> types;
		
		// shortcut to all 'Types'
        public static List<string> AllTypes = new List<string>();

		// shortcut to all 'BaseTypes'
        public static List<string> AllBaseTypes = new List<string>();

		 static UniqueTypeRegistry() {
			types = typeof(UniqueTypeRegistry)
    		.Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(UniqueTypeRegistry)) && !t.IsAbstract).Select(t => (UniqueTypeRegistry)Activator.CreateInstance(t));
			foreach (UniqueTypeRegistry v in types) {
                if (!AllTypes.Contains(v.Type) && !v.BaseOnly) AllTypes.Add(v.MatchClassName ? v.GetType().ToString() : v.Type);
                if (!AllBaseTypes.Contains(v.BaseType)) AllBaseTypes.Add(v.BaseType);
            }
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
							if (v.MatchClassName ? v.GetType().ToString() == type : v.Type == type) { found = v; break; } // match Object.GetType if UniqueTypeRegistry.Type is null
						}
					}
					i += 1;
				}
				if (i != types.Count) findCache.Add(key, i); // don't add invalid results to cache
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
			True to ignore in 'Find/FindAll' and in AllTypes
			By default is only true when Type is null 
		*/
        public virtual bool BaseOnly {
			get {
                return Type == null;
            }
		}

		/*
			True to match by classname instead of Type
		*/
		public virtual bool MatchClassName {
            get => false;
        }

        // matched by this string or classname
        // virtual to be null by default ( gets ignored in search and types )
        public virtual string Type {
			get {
                return null;
            }
		}
	
		// optional base type to group classes of this type
        public virtual string BaseType {
			get {
				return "base";
			}
		}
 }

}