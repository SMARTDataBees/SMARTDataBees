using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SDBees.Plugs.Properties
{
    //using System.Windows.Forms;

    //static class Program
    //{
    //    [STAThread]
    //    static void Main()
    //    {
    //        Application.EnableVisualStyles();
    //        Application.SetCompatibleTextRenderingDefault(false);

    //        Bag bag = new Bag();            
            
    //        Bag.AddProperty<int>("TestProp", new
    //DefaultValueAttribute(5));
    //        Bag.AddProperty<string>("Name");


    //        using (Form form = new Form())
    //        using (Button b1 = new Button())
    //        using (Label l1 = new Label())
    //        using (Label l2 = new Label())
    //        {
    //            form.DataBindings.Add("Text", bag, "Name");
    //            l1.DataBindings.Add("Text", bag, "Name");
    //            l2.DataBindings.Add("Text", bag, "TestProp");
    //            b1.Dock = l1.Dock = l2.Dock = DockStyle.Top;
    //            b1.Text = "Update name";
    //            b1.Click += delegate
    //            {
    //                TypeDescriptor.GetProperties(bag)["Name"].SetValue(bag,
    //DateTime.Now.ToLongTimeString());
    //            };
    //            form.Controls.AddRange(new Control[] { b1, l1, l2 });
    //            form.ShowDialog();
    //        }
    //    }
    //}

    interface IBag
    {
        void OnAfterValueChanged(string propertyName);
        void AddHandler(object key, EventHandler value);
        void RemoveHandler(object key, EventHandler value);
        void OnEvent(object key);
        IBagValue GetValue(string propertyName);
    }
    interface IBagValue
    {
        void ResetValue();
        bool IsDefaultValue { get; }
        object Value { get; set; }
        event EventHandler ValueChanged;
    }
    interface IBagDefinition
    {
        IBagValue Create(IBag bag);
        PropertyDescriptor Property { get; }
    }
    sealed class BagDefinition<T> : IBagDefinition
    {
        private readonly PropertyDescriptor property;
        public PropertyDescriptor Property { get { return property; } }
        private readonly T defaultValue;
        public T DefaultValue { get { return defaultValue; } }
        public string Name { get { return Property.Name; } }
        IBagValue IBagDefinition.Create(IBag bag)
        {
            return new BagValue<T>(bag, this);
        }
        public BagDefinition(string propertyName, Attribute[] attributes)
        {
            defaultValue = default(T);
            if (attributes != null)
            { // check for A default value
                foreach (var attrib in attributes)
                {
                    var defAttrib = attrib as
    DefaultValueAttribute;
                    if (defAttrib != null)
                    {
                        defaultValue = (T)defAttrib.Value;
                        break;
                    }
                }
            }
            property = new BagPropertyDescriptor(propertyName, attributes);
        }
        
        internal class BagPropertyDescriptor : PropertyDescriptor
        {
            public BagPropertyDescriptor(string name, Attribute[] attributes)
                : base(name, attributes) { }
            private IBagValue GetBagValue(object component)
            {
                return ((IBag)component).GetValue(Name);
            }
            public override object GetValue(object component)
            {
                return GetBagValue(component).Value;
            }
            public override void SetValue(object component, object value)
            {
                GetBagValue(component).Value = value;
            }
            public override Type ComponentType
            {
                get { return typeof(Bag); }
            }
            public override Type PropertyType
            {
                get { return typeof(T); }
            }
            public override bool IsReadOnly
            {
                get { return false; }
            }
            public override bool CanResetValue(object component)
            {
                return true;
            }
            public override void ResetValue(object component)
            {
                GetBagValue(component).ResetValue();
            }
            public override bool ShouldSerializeValue(object component)
            {
                return !GetBagValue(component).IsDefaultValue;
            }
            public override bool SupportsChangeEvents
            {
                get { return true; }
            }
            public override void AddValueChanged(object component,
    EventHandler handler)
            {
                GetBagValue(component).ValueChanged += handler;
            }
            public override void RemoveValueChanged(object component,
    EventHandler handler)
            {
                GetBagValue(component).ValueChanged -= handler;
            }

        }
    }
    sealed class BagValue<T> : IBagValue, ITypeDescriptorContext
    {
        private T value;
        private readonly IBag bag;
        void IBagValue.ResetValue()
        {
            Value = Definition.DefaultValue;
        }
        bool IBagValue.IsDefaultValue
        {
            get
            {
                return EqualityComparer<T>.Default.Equals(Value,
                    Definition.DefaultValue);
            }
        }
        private readonly BagDefinition<T> definition;
        public IBag Bag { get { return bag; } }
        public BagDefinition<T> Definition { get { return definition; } }
        public BagValue(IBag bag, BagDefinition<T> definition)
        {
            if (bag == null) throw new ArgumentNullException("bag");
            if (definition == null) throw new
    ArgumentNullException("definition");
            this.bag = bag;
            this.definition = definition;
            Value = Definition.DefaultValue;
        }
        public T Value
        {
            get { return value; }
            set
            {
                if (EqualityComparer<T>.Default.Equals(Value, value))
                    return;
                this.value = value;
                Bag.OnAfterValueChanged(Definition.Name);
                Bag.OnEvent(Definition);
            }
        }
        public event EventHandler ValueChanged
        {
            add { Bag.AddHandler(Definition, value); }
            remove { Bag.RemoveHandler(Definition, value); }
        }
        object IBagValue.Value
        {
            get { return Value; }
            set { Value = (T)value; }
        }

        IContainer ITypeDescriptorContext.Container
        {
            get
            {
                return
                    null;
            }
        }
        object ITypeDescriptorContext.Instance { get { return Bag; } }
        void ITypeDescriptorContext.OnComponentChanged()
        {
            Bag.OnAfterValueChanged(Definition.Name);
        }
        bool ITypeDescriptorContext.OnComponentChanging() { return true; }
        PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor
        {
            get
            { return Definition.Property; }
        }
        object IServiceProvider.GetService(Type serviceType)
        {
            return
                null;
        }
    }

    sealed class Bag : IBag, INotifyPropertyChanged
    {
        private EventHandlerList events;
        void IBag.AddHandler(object key, EventHandler handler)
        {
            AddHandler(key, handler);
        }
        void IBag.RemoveHandler(object key, EventHandler handler)
        {
            RemoveHandler(key, handler);
        }
        void IBag.OnEvent(object key)
        {
            OnEvent(key);
        }
        private void AddHandler(object key, Delegate handler)
        {
            if (handler == null) return;
            if (events == null) events = new EventHandlerList();
            events.AddHandler(key, handler);
        }
        private void RemoveHandler(object key, Delegate handler)
        {
            if (events == null || handler == null) return;
            events.RemoveHandler(key, handler);
        }
        private void OnEvent(object key)
        {
            if (events == null) return;
            var handler = events[key] as EventHandler;
            if (handler != null) handler(this, EventArgs.Empty);
        }
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { AddHandler(EVENT_PropertyChanged, value); }
            remove { RemoveHandler(EVENT_PropertyChanged, value); }
        }
        private static readonly object EVENT_PropertyChanged = new
    object();
        private void OnPropertyChanged(string propertyName)
        {
            if (events == null) return;
            var handler =
    events[EVENT_PropertyChanged] as PropertyChangedEventHandler;
            if (handler != null) handler(this, new
    PropertyChangedEventArgs(propertyName));
        }
        void IBag.OnAfterValueChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
        IBagValue IBag.GetValue(string propertyName)
        {
            return GetValue(propertyName);
        }
        private readonly Dictionary<string, IBagValue> values =
            new Dictionary<string,
    IBagValue>(StringComparer.InvariantCulture);
        private IBagValue GetValue(string propertyName)
        {
            lock (values)
            {
                IBagValue value;
                if (!values.TryGetValue(propertyName, out value))
                {
                    value = CreateValue(this, propertyName);
                    values.Add(propertyName, value);
                }
                return value;
            }
        }

        static readonly Dictionary<string, IBagDefinition> defintions =
            new Dictionary<string,
    IBagDefinition>(StringComparer.InvariantCulture);

        public static void AddProperty<T>(string propertyName, params 
Attribute[] attributes)
        {
            var def = new BagDefinition<T>(propertyName,
    attributes);
            lock (defintions)
            {
                defintions.Add(propertyName, def);
                BagDescriptionProvider.ResetProperties();
            }
        }
        internal static PropertyDescriptorCollection GetProperties()
        {
            lock (defintions)
            {
                var props = new
    PropertyDescriptor[defintions.Count];
                var i = 0;
                foreach (var def in defintions.Values)
                {
                    props[i++] = def.Property;
                }
                return new PropertyDescriptorCollection(props, true);
            }
        }
        static IBagValue CreateValue(IBag bag, string propertyName)
        {
            lock (defintions)
            {
                return defintions[propertyName].Create(bag);
            }
        }

        static Bag()
        {
            BagDescriptionProvider.Initialize();
        }

    }

    sealed class BagDescriptionProvider : TypeDescriptionProvider
    {
        static readonly BagTypeDescriptor descriptor;

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void Initialize() { } // to force static ctor
        static BagDescriptionProvider()
        {
            var parent =
    TypeDescriptor.GetProvider(typeof(Bag)).GetTypeDescriptor(typeof(Bag));
            descriptor = new BagTypeDescriptor(parent);
            TypeDescriptor.AddProvider(new BagDescriptionProvider(),
    typeof(Bag));
        }
        public override ICustomTypeDescriptor GetTypeDescriptor(Type
    objectType, object instance)
        {
            return descriptor;
        }
        internal static void ResetProperties()
        {
            descriptor.ResetProperties();
        }
    }
    sealed class BagTypeDescriptor : CustomTypeDescriptor
    {
        public BagTypeDescriptor(ICustomTypeDescriptor parent)
            : base(parent)
        {
            if (parent == null) throw new ArgumentNullException("parent");
        }
        private PropertyDescriptorCollection properties;
        internal void ResetProperties()
        {
            properties = null;
        }
        public override PropertyDescriptorCollection
    GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }
        public override PropertyDescriptorCollection GetProperties()
        {
            if (properties == null)
            {
                properties = Bag.GetProperties();
            }
            return properties;
        }
    }


}
