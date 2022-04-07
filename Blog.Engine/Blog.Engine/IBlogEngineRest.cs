using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Blog.Engine
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IBlogEngineRest" in both code and config file together.
    [ServiceContract]
    public interface IBlogEngineRest
    {
        [OperationContract]
        void DoWork();
    }
}
