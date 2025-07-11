using System;
using System.Collections.Generic;

namespace Vuzmir.UnityWebRequestInterface
{
    public class WebRequestForm
    {
        private Dictionary<string, object> formFields = null;

        public IEnumerable<KeyValuePair<string, object>> Fields
        {
            get
            {
                if (formFields == null)
                {
                    return Array.Empty<KeyValuePair<string, object>>();
                }
                return formFields;
            }
        }

        public WebRequestForm Set(string name, object value)
        {
            if (value == null)
            {
                formFields?.Remove(name);
                return this;
            }

            formFields ??= new Dictionary<string, object>();
            formFields[name] = value;
            return this;
        }
    }
}
