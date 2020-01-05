using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal interface ISaveable
{
    object Load(string data);

    string Save();
}