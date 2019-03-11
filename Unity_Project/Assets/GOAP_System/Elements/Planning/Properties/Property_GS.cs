namespace GOAP_S.Planning
{
    public class Property_GS
    {
        //Content fields
        private string _target_UUID = null; //Target agent property UUID
        private string _key = null; //Property key, what we use to detect a key
        private object _value = null; //Target variable we are working with

        //Constructors
        public Property_GS(string new_target_UUID, string new_key, object new_value)
        {
            //Set property fields
            _target_UUID = new_target_UUID;
            _key = new_key;
            _value = new_value;
        }

        //Get/Set Methods =============
        public string target_UUID
        {
            get
            {
                return _target_UUID;
            }
        }

        public string key
        {
            get
            {
                return _key;
            }
        }

        public object value
        {
            get
            {
                return _value;
            }
        }

        //Planning Methods ============
        public bool Compare(Property_GS compare_property)
        {
            return (
                string.Compare(_target_UUID, compare_property.target_UUID) == 0
                && string.Compare(_key, compare_property.key) == 0
                && _value == compare_property._value
                );
        }
    }
}
