using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;

namespace ASP.NET_Humans.ML
{
    public class GenderPrediction
    {
        [ColumnName("Male")]
        public string PredictedMale { get; set; }

    }
}
