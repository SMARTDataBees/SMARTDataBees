// #StartHeader# ==============================================================
//
// This file is a part of the SMARTDataBees open source project.
// 
// Copyright (C) 2016 by
//        G.E.M. Team Solutions GbR
//        CAD-Development
//
// SMARTDataBees is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SMARTDataBees is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with SMARTDataBees.  If not, see <http://www.gnu.org/licenses/>.
//
// #EndHeader# ================================================================

using System.Collections.Generic;
using SDBees.DB;

namespace SDBees.Core.Admin
{
    public partial class ViewCache
    {

        private static ViewCacheImplementation _viewCache;
        private static int _enabled;



        #region Public Properties

        public static ViewCacheImplementation Instance
        {
            get
            {
                if (_viewCache == null)
                {
#if false //PROFILER
                    System.Windows.Forms.MessageBox.Show("s_viewCache == null");
#endif

                    _viewCache = CreateViewCacheImplementation(SDBeesDBConnection.Current.Database, false);
                }

                return _viewCache;
            }
        }

        public static bool Enabled
        {
            get => (_viewCache != null) && _viewCache.Enabled;
            set
            {
                if (_viewCache != null)
                {
                    _viewCache.Enabled = value;
                }
            }
        }

        public static void Enable()
        {
            if (_enabled == 0)
            {
                const bool activateCache = true;
                _viewCache = CreateViewCacheImplementation(SDBeesDBConnection.Current.Database, activateCache);

#if PROFILER
                SDBees.Profiler.Start("Enable");
#endif
            }

            _enabled++;
        }

        public static void Disable()
        {
            --_enabled;

            if (_enabled == 0)
            {
#if PROFILER
                SDBees.Profiler.Log(s_viewCache.ToString());
                SDBees.Profiler.Stop();
#endif

                _viewCache = null;
            }
        }

        private static ViewCacheImplementation CreateViewCacheImplementation(Database database, bool enabled)
        {
            return new ViewCacheImplementationOne(database, enabled);
        }

#endregion
#region Public Classes

        internal class Cache<TKey, TValue> where TValue : new()
        {
            public Cache(string name)
            {
                _name = name;

                Clear(true);
            }

            public bool Enabled
            {
                get => _isEnabled;
                set => _isEnabled = value;
            }

            public void Clear(bool enabled)
            {
                _isEnabled = enabled;
                _cache = null;
                _hits = 0;
                _misses = 0;
                _performance = 0;
            }

            public bool TryGetValue(TKey key, out TValue value)
            {
                var result = false;

                if (_isEnabled)
                {
                    if (Instance.TryGetValue(key, out value))
                    {
                        _hits += 1;

                        result = true;
                    }
                    else
                    {
                        _misses += 1;
                    }
                }
                else
                {
                    value = new TValue();

                    _misses += 1;
                }

                _performance = 100 * _hits / (_hits + _misses);

                return result;
            }

            public void Add(TKey key, TValue value)
            {
                if (_isEnabled) Instance.Add(key, value);
            }

            public string ToString()
            {
                return string.Format("{0} = ({1}, {2}, {3}, {4})", _name, _hits + _misses, _hits, _misses, _performance);
            }

            private Dictionary<TKey, TValue> Instance
            {
                get
                {
                    if (_cache == null) _cache = new Dictionary<TKey, TValue>();

                    return _cache;
                }
            }

            private bool _isEnabled = true;

            private Dictionary<TKey, TValue> _cache;
            private readonly string _name;
            private int _hits;
            private int _misses;
            private int _performance;
        }

        #endregion
    }
}
