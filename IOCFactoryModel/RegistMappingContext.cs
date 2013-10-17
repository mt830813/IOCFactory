using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace IOCFactoryModel
{
    //[DataContract]
    public class RegistMappingContext
    {
        #region read from file
        //[DataMember(IsRequired = true)]
        public string PTypeStr { get; set; }

        //[DataMember(IsRequired = true)]
        public string CTypeStr { get; set; }

        //[DataMember]
        public string Name { get; set; }

        //[DataMember(IsRequired = true)]
        public string InstTypeStr { get; set; }
        #endregion

        #region can't read from file
        private Type _PType;
        public Type PType
        {
            get
            {
                if (_PType == null)
                {
                    try
                    {
                        _PType = Type.GetType(PTypeStr);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("{0} not a valid Type", PTypeStr));
                    }
                }
                return _PType;
            }
        }

        private Type _CType;
        public Type CType
        {
            get
            {
                if (_CType == null)
                {
                    try
                    {
                        _CType = Type.GetType(CTypeStr);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("{0} not a valid Type", CTypeStr));
                    }
                }
                return _CType;
            }
        }

        private InstType? _InstType;
        public InstType InstType
        {
            get
            {
                if (_InstType == null)
                {
                    try
                    {
                        _InstType = (InstType)System.Enum.Parse(typeof(InstType), this.InstTypeStr);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("{0} not a InstType", InstTypeStr));
                    }
                }
                return _InstType.Value;
            }
        }

        #endregion

    }
}
