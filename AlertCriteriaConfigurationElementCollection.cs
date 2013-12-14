using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace CanaryRelic
{
    public class AlertCriteriaCollection : ConfigurationElementCollection, IEnumerable<AlertCriteriaConfigurationElement>
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new AlertCriteriaConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AlertCriteriaConfigurationElement) element).Name;
        }

        public AlertCriteriaConfigurationElement this[int index]
        {
            get { return (AlertCriteriaConfigurationElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(AlertCriteriaConfigurationElement serviceConfig)
        {
            BaseAdd(serviceConfig);
        }

        public void Clear()
        {
            BaseClear();
        }

        public void Remove(AlertCriteriaConfigurationElement serviceConfig)
        {
            BaseRemove(serviceConfig.Name);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public new IEnumerator<AlertCriteriaConfigurationElement> GetEnumerator()
        {
            return BaseGetAllKeys().Select(key => (AlertCriteriaConfigurationElement) BaseGet(key)).GetEnumerator();
        }
    }
}
