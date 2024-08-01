using ECER.Utilities.DataverseSdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECER.Resources.Documents.Applications.ChildrenServices;

internal interface IApplicationChildService<T>
{
  Task Update(ecer_Application application, List<T> updatedEntities);
}
